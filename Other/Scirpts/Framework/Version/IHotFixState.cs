using Lib;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Util;


using System.Linq;
using UnityEngine.ResourceManagement;
using System;

namespace Framework
{
    public interface IHotFixState
    {
        EHotFixState State { get; }
        void OnEnter();
        void OnExit();
        void OnUpdate();
    }
    public enum AssetType
    {
        config = 0,
        addressable = 1,
        catalog = 2,
    }
    public class DownloadAssetsInfo
    {
        public AssetsInfo assetInfo;
        public UnityWebRequest WebRequest;
        public bool RemoveFlag = false;
        public bool LoadCompletedFlag = false;
        public float m_TimeoutTimer;
        string persistFilePath;
        string relativeFilePath;
        string HotFixConfigUrl;

        int m_TimeoutCount = 0;//请求超时的次数
        int Timeout = 10;
        int RetryCount = 3;
        //long m_totalDownloadedBytes = 0;
        ulong m_LastDownloadedByteCount = 0;
        int m_TimeoutOverFrames = 0;
        int m_Retries;

        ulong m_BytesToDownload;
        ulong m_DownloadedBytes;
        ulong m_RecordDownloadedBytes;
        public void Start(AssetsInfo info)
        {
            assetInfo = info;
            RemoveFlag = false;
            LoadCompletedFlag = false;
            m_TimeoutCount = 0;
            m_RecordDownloadedBytes = 0;
            m_DownloadedBytes = 0;
            m_BytesToDownload = 0;
            m_TimeoutTimer = 0;
            m_TimeoutOverFrames = 0;
            m_Retries = 0;

            if (assetInfo.AssetType == (int)AssetType.config)
            {
                relativeFilePath = info.AssetName.Replace("\\", "/");
                persistFilePath = AssetPath.GetPersistentFullPath(relativeFilePath);
            }
            else if (assetInfo.AssetType == (int)AssetType.addressable)
            {
                relativeFilePath = string.Format("{0}/{1}/{2}.bundle", AssetPath.sAddressableDir, HotFixStateManager.Instance.PackagePlatformName, assetInfo.AssetName);
                persistFilePath = string.Format("{0}/{1}/{2}/__data", Caching.defaultCache.path, info.BundleName, info.HashOfBundle);//下载后填充到__data
            }
            else
            {
                string catalogHashName = info.AssetName.Split('/')[1];
                relativeFilePath = string.Format("{0}/{1}", AssetPath.sAddressableDir, catalogHashName);
                persistFilePath = AssetPath.GetPersistentFullPath(info.AssetName);
            }

            #region 缓存记录：是否需要删除 + 是否需要加入缓存记录

            //如果不是缓存则先删除 || 或者缓存了，但记录该文件的缓存列表被删，仍需要重新下载(因为记录该文件信息的md5已丢失，不确定该文件远端是否又有更新)
            if (HotFixStateManager.Instance.CacheAssetList == null || !HotFixStateManager.Instance.CacheAssetList.Contents.ContainsKey(assetInfo.AssetName))
            {
                if (File.Exists(persistFilePath))
                {
                    File.Delete(persistFilePath);
                }
            }

            string dir = Path.GetDirectoryName(persistFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //校验失败的 需要删除再次下载
            if (HotFixStateManager.Instance.RetryDownloadCountDict.ContainsKey(assetInfo.AssetName))
            {
                if (File.Exists(persistFilePath))
                {
                    File.Delete(persistFilePath);
                }
            }

            //获取下载文件长度  大于1M放入缓存
            long totalLength = (long)assetInfo.Size;
            if ((ulong)totalLength >= HotFixStateManager.Instance.CacheFileMinSize)
            {
                if (HotFixStateManager.Instance.CacheAssetList == null)
                    HotFixStateManager.Instance.CacheAssetList = new AssetList();
                HotFixStateManager.Instance.CacheAssetList.Contents[assetInfo.AssetName] = assetInfo;

                string cacheAssetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);
                VersionHelper.WriteAssetList(cacheAssetsListPath, HotFixStateManager.Instance.CacheAssetList);
            }
            #endregion

            m_BytesToDownload = BytesToDownload();
            HotFixConfigUrl = HotFixStateManager.Instance.DownloadABUrl;
        }

        ulong BytesToDownload()
        {
            //下载到一半的文件 计算剩余需要下载的资源大小
            if (File.Exists(persistFilePath))
            {
                FileInfo file = new FileInfo(persistFilePath);
                if ((ulong)file.Length > assetInfo.Size)
                {
                    DebugUtil.LogErrorFormat("cacheFileLength:{0} > assetInfoSize:{1} ,path:{2}", file.Length, assetInfo.Size, persistFilePath);
                    return 0;
                }
                else
                {
                    return assetInfo.Size - (ulong)file.Length;
                }
            }
            else
                return assetInfo.Size;
        }

        public ulong GetDownLoadSize()
        {
            ulong tempDownloadBytes;
            if (LoadCompletedFlag)
            {
                tempDownloadBytes = m_DownloadedBytes;
            }
            else
            {
                tempDownloadBytes = m_RecordDownloadedBytes;

                //if (WebRequest != null && string.IsNullOrEmpty(WebRequest.error))
                //{
                //    m_DownloadedBytes = WebRequest.downloadedBytes;

                //    //m_LastDownloadedBytes:用于记录下载到一半出错的，进度条显示不能回滚
                //    if (m_DownloadedBytes > m_LastDownloadedBytes)
                //    {
                //        tempDownloadBytes = m_DownloadedBytes;
                //        m_LastDownloadedBytes = m_DownloadedBytes;
                //    }
                //    else
                //    {
                //        tempDownloadBytes = m_LastDownloadedBytes;
                //    }
                //}
                //else
                //{
                //    DebugUtil.LogErrorFormat("GetDownLoadSize : WebRequest = {0} ", WebRequest == null ? "null" : WebRequest.error);
                //    tempDownloadBytes = m_LastDownloadedBytes;
                //}
            }
            return tempDownloadBytes;
        }



        public bool HasTimedOut => m_TimeoutTimer >= Timeout && m_TimeoutOverFrames > 5;
        public bool IsDone()
        {
            if (WebRequest == null || (WebRequest != null && WebRequest.isDone))
                return true;
            return false;
        }

        public bool IsError()
        {
            if (WebRequest == null || !WebRequest.isDone)
                return false;
            var isError = WebRequest.isHttpError || WebRequest.isNetworkError;
            return isError;
        }

        public void Dispose(bool fsReset = false)
        {
            FilesDownloadHandler filesDownloadHandler = WebRequest?.downloadHandler as FilesDownloadHandler;
            if (filesDownloadHandler != null)
            {
                filesDownloadHandler.ErrorDispose(fsReset);
                filesDownloadHandler.Dispose();
            }

            if (WebRequest != null)
                WebRequest.Dispose();
            WebRequest = null;

            m_LastDownloadedByteCount = 0;
        }

        public void CompleteDispose()
        {
            LoadCompletedFlag = true;
            ++HotFixStateManager.Instance.LoadedHotFixFileCount;
            // HotFixStateManager.Instance.HotFixSize += (ulong)GetDownLoadSize();

            if (WebRequest != null)
            {
                m_DownloadedBytes = (ulong)WebRequest.downloadedBytes;
                VertifyConfigAssetMD5(WebRequest.url);
            }

            if (m_DownloadedBytes != m_BytesToDownload)
            {
                DebugUtil.LogErrorFormat("需要下载大小:{0}，下载完成大小:{1}, path:{2}", m_BytesToDownload, m_DownloadedBytes, persistFilePath);
            }

            Dispose();
        }

        public void Exit()
        {
            if (WebRequest != null)
            {
                if (WebRequest.isDone)
                {
                    //error reset fs
                    if (UnityWebRequestUtilities.RequestHasErrors(WebRequest, out UnityWebRequestResult uwrResult))
                        Dispose(true);
                    else
                        Dispose();
                }
                else
                {
                    WebRequest.Abort();
                    Dispose(true);
                }
            }
            m_RecordDownloadedBytes = 0;
            m_LastDownloadedByteCount = 0;
            m_TimeoutTimer = 0;
            m_TimeoutOverFrames = 0;
            m_Retries = 0;
            m_TimeoutCount = 0;
            assetInfo = null;
        }

        public bool JudageEnqueue()
        {
            if (m_Retries < RetryCount)
            {
                return true;
            }
            else if (m_TimeoutCount < VersionHelper.HotFixUrlArr.Length - 1)
            {
                return true;
            }
            else
            {
                Debug.LogError(string.Format("不能入队列，因为尝试次数完毕 Count:{0}", VersionHelper.HotFixUrlArr.Length * RetryCount));
            }

            return false;
        }

        public void ErrorDispose()
        {
            if (WebRequest != null)
            {
                AppManager.ResponseCode = WebRequest.responseCode;

                if (WebRequest.isNetworkError)
                    AppManager.eAppError = EAppError.NetworkError;
                else
                    AppManager.eAppError = EAppError.HttpError;
            }
            else
            {
                AppManager.eAppError = EAppError.HttpError;
            }

            Dispose(true);
        }



        public void Update()
        {
            if (WebRequest != null && !WebRequest.isDone)
            {
                //m_LastDownloadedByteCount：只是本次WebRequest下载
                if (m_LastDownloadedByteCount != WebRequest.downloadedBytes)
                {
                    m_TimeoutTimer = 0;
                    m_TimeoutOverFrames = 0;
                    m_LastDownloadedByteCount = WebRequest.downloadedBytes;

                    if (m_LastDownloadedByteCount > m_RecordDownloadedBytes)
                    {
                        m_RecordDownloadedBytes = m_LastDownloadedByteCount;
                    }
                }
                else
                {
                    m_TimeoutTimer += Time.unscaledDeltaTime;
                    //if (HasTimedOut)
                    //    WebRequest.Abort();

                    m_TimeoutOverFrames++;
                }
            }
        }


        public void StartDownload()
        {
            RemoveFlag = false;
            string tsUrl = string.Empty;
            if (m_Retries < RetryCount)
            {
                tsUrl = string.Format("{0}/{1}?ts={2}", HotFixConfigUrl, relativeFilePath, TimeManager.ClientNowMillisecond());
            }
            else if (m_TimeoutCount < VersionHelper.HotFixUrlArr.Length - 1)
            {
                m_TimeoutCount++;
                m_Retries = 0;

                //*****说明：由于下载失败，会重置文件指针的位置，所以不存在文件头不一致的情况,删除会导致下载文件大小不好计算（删除会导致文件重新下？？？）******/
                //这里在换链接后，要把前一个连接保存的删除 （需要弄清楚同一份文件，不同cdn地址下的文件头一样嘛？）
                //if (File.Exists(persistFilePath))
                //    File.Delete(persistFilePath);

                HotFixConfigUrl = string.Format("{0}/{1}/{2}.{3}", VersionHelper.HotFixUrlArr[m_TimeoutCount],
                        AssetPath.sPlatformName,
                        VersionHelper.RemoteBuildVersion,
                        VersionHelper.RemoteAssetVersion);

                tsUrl = string.Format("{0}/{1}?ts={2}", HotFixConfigUrl, relativeFilePath, TimeManager.ClientNowMillisecond());
                DebugUtil.LogFormat(ELogType.eNone, string.Format("Web request replace cdn:{0}, retrying {1}/{2}", HotFixConfigUrl, m_Retries, RetryCount));
            }
            else
            {
                Debug.LogError("走到这里代表出错了哦！当已经超过所有cdn地址可尝试的次数该怎么办？问陈铭！！！");
            }

            if (!string.IsNullOrEmpty(tsUrl))
            {
                WebRequest = new UnityWebRequest(tsUrl, UnityWebRequest.kHttpVerbGET);
                FilesDownloadHandler filesDownload = new FilesDownloadHandler(persistFilePath);
                WebRequest.downloadHandler = filesDownload;
                WebRequest.disposeDownloadHandlerOnDispose = true;
                WebRequest.SetRequestHeader("Range", "bytes=" + filesDownload.StartFileLen + "-");
                WebRequest.SendWebRequest();
                m_Retries++;
            }

            m_TimeoutTimer = 0;
            m_TimeoutOverFrames = 0;
            m_LastDownloadedByteCount = 0;
        }


        private void VertifyConfigAssetMD5(string url)
        {
            string md5 = FrameworkTool.GetFileMD5(persistFilePath);

            if (string.Equals(assetInfo.AssetMD5, md5, StringComparison.Ordinal))
            {
                VersionHelper.AppendAssetInfoToPersistent(assetInfo);

                if (HotFixStateManager.Instance.RetryDownloadCountDict.ContainsKey(assetInfo.AssetName))
                {
                    HotFixStateManager.Instance.RetryDownloadCountDict.Remove(assetInfo.AssetName);
                }
            }
            else
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("file_url", url);
                dict.Add("file_md5_correct", assetInfo.AssetMD5);
                dict.Add("file_md5_error", md5);
                HitPointManager.HitPoint("game_update_md5error", dict);

                Debug.LogErrorFormat("MD5验证失败:{0} 记录md5:{1}  下载md5:{2}", assetInfo.AssetName, assetInfo.AssetMD5, md5);

                //校验失败的特殊处理,直接标注清除标记
                if (HotFixStateManager.Instance.RetryDownloadCountDict.ContainsKey(assetInfo.AssetName))
                {
                    HotFixStateManager.Instance.RetryDownloadCountDict[assetInfo.AssetName]++;
                }
                else
                {
                    HotFixStateManager.Instance.RetryDownloadCountDict.Add(assetInfo.AssetName, 1);
                }
            }

            //假如缓存里面有正在下载的  那么在下载完成后要删除缓存里正在下载的记录
            if (HotFixStateManager.Instance.CacheAssetList != null && HotFixStateManager.Instance.CacheAssetList.Contents != null
                && HotFixStateManager.Instance.CacheAssetList.Contents.ContainsKey(assetInfo.AssetName))
            {
                HotFixStateManager.Instance.CacheAssetList.Contents.Remove(assetInfo.AssetName);
                string cacheAssetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);
                VersionHelper.WriteAssetList(cacheAssetsListPath, HotFixStateManager.Instance.CacheAssetList);
            }
        }


        public void HttpErrorReport()
        {
            if (WebRequest != null)
            {
                UnityWebRequestResult uwrResult = null;
                if (UnityWebRequestUtilities.RequestHasErrors(WebRequest, out uwrResult))
                {
                    if (uwrResult != null)
                        HotFixStateManager.HotFixHttpErrorHitPoint(uwrResult);
                }
                else
                {

                    if (uwrResult == null)
                        uwrResult = new UnityWebRequestResult(WebRequest);

                    if (HasTimedOut)
                        uwrResult.Error = "Request timeout";

                    if (uwrResult != null)
                        HotFixStateManager.HotFixHttpErrorHitPoint(uwrResult);
                }
            }
        }
    }

    public abstract class CheckHotFixBase
    {
        public enum CheckHotFixList
        {
            Invalid = 0,
            LoadPersistent,
            LoadCache,
            CompareList,
            Complete
        }

        private CheckHotFixList m_currentStatus = CheckHotFixList.Invalid;
        private CheckHotFixList m_nextStatus = CheckHotFixList.Invalid;
        private UnityWebRequestAsyncOperation webAsyncOperation;

        public bool IsDone()
        {
            if (m_currentStatus == CheckHotFixList.Complete)
                return true;
            return false;
        }

        public void OnEnter(CheckHotFixList tempType)
        {
            m_nextStatus = tempType;
        }

        public virtual void OnExit()
        {
            if (webAsyncOperation != null)
            {
                //webAsyncOperation.completed -= PersistWebAsyncOperation_completed;
                webAsyncOperation.webRequest.Dispose();
                webAsyncOperation = null;
            }
            m_currentStatus = CheckHotFixList.Invalid;
        }

        public virtual void OnUpdate()
        {
            if (m_nextStatus == CheckHotFixList.Invalid)
                return;

            if (m_nextStatus == m_currentStatus)
                return;

            m_currentStatus = m_nextStatus;
            m_nextStatus = CheckHotFixList.Invalid;

            switch (m_currentStatus)
            {
                case CheckHotFixList.LoadPersistent:
                    LoadPersistentAssetList();
                    break;
                case CheckHotFixList.LoadCache:
                    LoadCacheAssetList();
                    break;
                case CheckHotFixList.CompareList:
                    CompareAssetsList(0.5f, 1f);
                    break;
            }
        }

        #region 加载沙盒目录下热更列表
        private void LoadPersistentAssetList()
        {
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);
            if (File.Exists(assetsListPath))
            {
                assetsListPath = string.Format("{0}{1}", AssetPath.sPersistentPrefix, assetsListPath);
                DebugUtil.LogFormat(ELogType.eNone, "加载Persistent资源列表 {0}", assetsListPath);

                UnityWebRequest request = UnityWebRequest.Get(new System.Uri(assetsListPath));
                webAsyncOperation = request.SendWebRequest();
                webAsyncOperation.completed += PersistWebAsyncOperation_completed;
            }
            else
            {
                DebugUtil.LogFormat(ELogType.eNone, "Persistent不存在 {0}", assetsListPath);
                m_nextStatus = CheckHotFixList.LoadCache;
            }
        }

        private void PersistWebAsyncOperation_completed(AsyncOperation obj)
        {
            webAsyncOperation = obj as UnityWebRequestAsyncOperation;
            var webReq = webAsyncOperation?.webRequest;
            UnityWebRequestResult uwrResult = null;
            if (webReq != null && !UnityWebRequestUtilities.RequestHasErrors(webReq, out uwrResult))
            {
                //下载完成
                MemoryStream ms = new MemoryStream(webReq.downloadHandler.data);
                VersionHelper.SetPersistentAssetsList(ms);

                m_nextStatus = CheckHotFixList.LoadCache;
            }
            else
            {
                Debug.LogFormat("加载Persistent资源列表失败 {0}", webReq.error);
                AppManager.ResponseCode = uwrResult.ResponseCode;
                AppManager.eAppError = EAppError.HttpError;
                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;
            }

            webAsyncOperation.completed -= PersistWebAsyncOperation_completed;
            webAsyncOperation.webRequest.Dispose();
            webAsyncOperation = null;
        }
        #endregion

        #region 加载缓存列表

        private void LoadCacheAssetList()
        {
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);

            if (File.Exists(assetsListPath))
            {
                assetsListPath = string.Format("{0}{1}", AssetPath.sPersistentPrefix, assetsListPath);
                Debug.LogFormat("加载Persistent资源列表 {0}", assetsListPath);

                UnityWebRequest request = UnityWebRequest.Get(new System.Uri(assetsListPath));
                webAsyncOperation = request.SendWebRequest();
                webAsyncOperation.completed += CacheWebAsyncOperation_completed;
            }
            else
            {
                DebugUtil.LogFormat(ELogType.eNone, "Persistent不存在 {0}", assetsListPath);
                m_nextStatus = CheckHotFixList.CompareList;
            }
        }

        private void CacheWebAsyncOperation_completed(AsyncOperation obj)
        {
            webAsyncOperation = obj as UnityWebRequestAsyncOperation;
            var webReq = webAsyncOperation?.webRequest;
            UnityWebRequestResult uwrResult = null;
            if (webReq != null && !UnityWebRequestUtilities.RequestHasErrors(webReq, out uwrResult))
            {
                MemoryStream ms = new MemoryStream(webReq.downloadHandler.data);
                HotFixStateManager.Instance.CacheAssetList = AssetList.Deserialize(ms);
                ms.Close();
                ms.Dispose();

                m_nextStatus = CheckHotFixList.CompareList;
            }
            else
            {
                Debug.LogFormat("加载Persistent资源列表失败 {0}", webReq.error);
                AppManager.ResponseCode = uwrResult.ResponseCode;
                AppManager.eAppError = EAppError.HttpError;
                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;
            }

            webAsyncOperation.completed -= CacheWebAsyncOperation_completed;
            webAsyncOperation.webRequest.Dispose();
            webAsyncOperation = null;
        }
        #endregion

        #region 比对资源列表
        private void CompareAssetsList(float startProgress = 0f, float endProgress = 1f)
        {
            DebugUtil.LogFormat(ELogType.eNone, "对比资源列表");
            float halfProgress = (endProgress - startProgress) / 2f + startProgress;

            //HotFixSize = 0L;
            //HotFixTotalSize = 0L;
            HotFixStateManager.Instance.HotFixFileCount = 0;
            HotFixStateManager.Instance.HotFixTotalSize = 0;

            HotFixStateManager.Instance.NewAssets.Clear();
            HotFixStateManager.Instance.ExcessAssets.Clear();

            AssetList newCacheList = new AssetList();
            List<AssetsInfo> tempRecordList = new List<AssetsInfo>();

            AssetsInfo remoteInfo = null;
            AssetsInfo localInfo = null;
            int index = 0;
            int count = 0;

            index = 0;
            count = VersionHelper.mRemoteAssetsList.Contents.Count;
            var remoteItor = VersionHelper.mRemoteAssetsList.Contents.GetEnumerator();

            int curAssetVersion = 0;
            int.TryParse(VersionHelper.PersistentAssetVersion, out curAssetVersion);

            while (remoteItor.MoveNext())
            {
                remoteInfo = remoteItor.Current.Value;
                if (remoteInfo.Version <= curAssetVersion)
                {
                    continue;
                }

                #region 出现一种情况，刻意删除沙盒路径的资源或者删除记录的HotfixList.txt文件，那么已经下载过的资源 也会重新下载，做双重保险
                if (VersionHelper.mPersistentAssetsList == null
                    || !VersionHelper.mPersistentAssetsList.Contents.TryGetValue(remoteInfo.AssetName, out localInfo))
                {
                    if (remoteInfo.AssetType == (int)AssetType.addressable)
                    {
                        string bundlePath = string.Format("{0}/{1}/__data", Caching.defaultCache.path, remoteInfo.AssetName);
                        if (File.Exists(bundlePath) && remoteInfo.State == 0 && string.Equals(remoteInfo.AssetMD5, FrameworkTool.GetFileMD5(bundlePath), StringComparison.Ordinal))
                        {
                            tempRecordList.Add(remoteInfo);
                            continue;
                        }
                    }
                    else
                    {
                        //config,dll,catalog.json
                        string path = remoteInfo.AssetName.Replace("\\", "/");
                        string filePath = AssetPath.GetPersistentFullPath(path);
                        if (File.Exists(filePath) && remoteInfo.State == 0 && string.Equals(remoteInfo.AssetMD5, FrameworkTool.GetFileMD5(filePath), StringComparison.Ordinal))
                        {
                            tempRecordList.Add(remoteInfo);
                            continue;
                        }
                    }
                }
                #endregion


                if (VersionHelper.mPersistentAssetsList == null //HotFixList.txt文件不存在
                    || !VersionHelper.mPersistentAssetsList.Contents.TryGetValue(remoteInfo.AssetName, out localInfo)
                    || !remoteInfo.AssetMD5.Equals(localInfo.AssetMD5)
                    || remoteInfo.State != localInfo.State
                    || remoteInfo.Version != localInfo.Version)
                {
                    //0 = 修改或者新增 1 = 删除
                    if (remoteInfo.State == 0)
                    {
                        AssetsInfo cacheInfo = null;
                        //假如记录大文件(>1M 断点续传)的缓存文件列表HotFixCacheList.txt人为删除，此时该文件需要重新下载，不在走断点续传
                        //假如缓存文件列表HotFixCacheList.txt存在，人为删除HotFixCacheList.txt中记录的对应资源文件
                        if (HotFixStateManager.Instance.CacheAssetList == null
                            || !HotFixStateManager.Instance.CacheAssetList.Contents.TryGetValue(remoteInfo.AssetName, out cacheInfo)
                            || cacheInfo.AssetMD5 != remoteInfo.AssetMD5
                            || cacheInfo.Version != remoteInfo.Version)
                        {
                            HotFixStateManager.Instance.NewAssets.Add(remoteInfo);
                            //计算更新（配置文件，dll文件）资源大小
                            HotFixStateManager.Instance.HotFixTotalSize += remoteInfo.Size;
                            ++HotFixStateManager.Instance.HotFixFileCount;
                        }
                        else
                        {
                            //下载到一半的文件，缓存文件列表存在               
                            HotFixStateManager.Instance.NewAssets.Insert(0, remoteInfo);

                            //下载到一半的文件 计算剩余需要下载的资源大小
                            ulong needLoadcacheLength;
                            FileInfo fileInfo;
                            if (remoteInfo.AssetType == (int)AssetType.config || remoteInfo.AssetType == (int)AssetType.catalog)
                            {
                                string path = cacheInfo.AssetName.Replace("\\", "/");
                                string filePath = AssetPath.GetPersistentFullPath(path);
                                if (File.Exists(filePath))
                                {
                                    newCacheList.Contents.Add(cacheInfo.AssetName, cacheInfo);
                                    fileInfo = new FileInfo(filePath);
                                    needLoadcacheLength = remoteInfo.Size - (ulong)fileInfo.Length;
                                }
                                else
                                {
                                    needLoadcacheLength = remoteInfo.Size;
                                }
                            }
                            else
                            {
                                string filePath = string.Format("{0}/{1}/__data", Caching.defaultCache.path, remoteInfo.AssetName);
                                if (File.Exists(filePath))
                                {
                                    newCacheList.Contents.Add(cacheInfo.AssetName, cacheInfo);
                                    fileInfo = new FileInfo(filePath);
                                    needLoadcacheLength = remoteInfo.Size - (ulong)fileInfo.Length;
                                }
                                else
                                {
                                    needLoadcacheLength = remoteInfo.Size;
                                }
                            }


                            HotFixStateManager.Instance.HotFixTotalSize += needLoadcacheLength;
                            ++HotFixStateManager.Instance.HotFixFileCount;
                        }
                    }
                    else
                    {
                        HotFixStateManager.Instance.ExcessAssets.Enqueue(remoteInfo);
                    }
                }

                ++index;
                if (index % 80 == 0 || index == count)
                {
                    HotFixStateManager.Instance.SetProgress(startProgress, halfProgress, (float)index / (float)count);
                    //yield return null;这个处理是为了防止卡顿，暂时先不处理
                }
            }

            //将需要缓存的文件列表（>1M）缓存到本地
            if (HotFixStateManager.Instance.CacheAssetList != null)
                HotFixStateManager.Instance.CacheAssetList.Contents.Clear();
            HotFixStateManager.Instance.CacheAssetList = newCacheList;
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);
            VersionHelper.WriteAssetList(assetsListPath, HotFixStateManager.Instance.CacheAssetList);

            //将不需要下载的资源写回到沙盒HotFixList.txt
            VersionHelper.AppendAssetInfoList(tempRecordList);

            DebugUtil.LogFormat(ELogType.eNone, "需要更新的资源(config，dll) {0}", HotFixStateManager.Instance.HotFixFileCount);
            DebugUtil.LogFormat(ELogType.eNone, "需要删除的资源(config，dll) {0}", HotFixStateManager.Instance.ExcessAssets.Count);
            DebugUtil.LogFormat(ELogType.eNone, "已经有部分下载的资源(config，dll) {0}", HotFixStateManager.Instance.CacheAssetList.Contents.Count);

            m_nextStatus = CheckHotFixList.Complete;
        }
        #endregion
    }

    public class HotFix_DownLoadAssetList : IHotFixState
    {
        public EHotFixState State
        {
            get
            {
                return EHotFixState.DownLoad_HotFixList;
            }
        }


        int Timeout = 10;
        float m_TimeoutTimer = 0;
        int m_TimeoutOverFrames = 0;
        ulong m_LastDownloadedByteCount = 0;
        int m_Retries;
        int RetryCount = 3;
        int m_TimeoutCount = 0;//请求超时的次数
        string m_MD5;
        string m_url;

        private bool HasTimedOut => m_TimeoutTimer >= Timeout && m_TimeoutOverFrames > 5;
        string RemoteHotFixAssetsListUrl;
        bool m_WebRequestCompletedCallbackCalled = false;//停止执行回调，或者回调完成执行
        private UnityWebRequestAsyncOperation webAsyncOperation;

        public void OnEnter()
        {
            m_Retries = 0;
            m_TimeoutCount = 0;
            m_WebRequestCompletedCallbackCalled = false;

            //临时缓存文件
            string filePath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixTempListName);
            if (File.Exists(filePath))
                File.Delete(filePath);

            HotFixStateManager.Instance.CheckVersionProgress = 0.3f;

            DebugUtil.LogFormat(ELogType.eNone, "进入状态：EHotFixState.DownLoad_HotFixList");
            HitPointManager.HitPoint("game_hotfix_assetlist_start");

            RemoteHotFixAssetsListUrl = string.Format("{0}/{1}/{2}.{3}/{4}?ts={5}",
          VersionHelper.HotFixUrl,
          AssetPath.sPlatformName,
          VersionHelper.RemoteBuildVersion,
          VersionHelper.RemoteAssetVersion,
          AssetPath.sHotFixListName,
          TimeManager.ClientNowMillisecond());

            BeginHeaderOperation();
        }


        public void OnUpdate()
        {
            //更新检测超时
            if (webAsyncOperation != null && !webAsyncOperation.isDone)
            {
                if (m_LastDownloadedByteCount != webAsyncOperation.webRequest.downloadedBytes)
                {
                    m_TimeoutTimer = 0;
                    m_TimeoutOverFrames = 0;
                    m_LastDownloadedByteCount = webAsyncOperation.webRequest.downloadedBytes;
                }
                else
                {
                    m_TimeoutTimer += Time.unscaledDeltaTime;
                    if (HasTimedOut)
                        webAsyncOperation.webRequest.Abort();

                    m_TimeoutOverFrames++;
                }
            }
        }

        private void BeginHeaderOperation()
        {
            m_WebRequestCompletedCallbackCalled = false;
            m_url = string.Format("{0}?ts={1}", RemoteHotFixAssetsListUrl, TimeManager.ClientNowMillisecond());
            DebugUtil.LogFormat(ELogType.eNone, "get header HotFixList {0}", m_url);
            UnityWebRequest HttpReq = UnityWebRequest.Head(m_url);
            webAsyncOperation = HttpReq.SendWebRequest();
            webAsyncOperation.completed += AsyncOperation_HeaderComplete;
        }




        private void AsyncOperation_HeaderComplete(AsyncOperation obj)
        {
            if (m_WebRequestCompletedCallbackCalled)
                return;

            m_WebRequestCompletedCallbackCalled = true;

            webAsyncOperation = obj as UnityWebRequestAsyncOperation;
            var webReq = webAsyncOperation?.webRequest;

            UnityWebRequestResult uwrResult = null;
            if (webReq != null && !UnityWebRequestUtilities.RequestHasErrors(webReq, out uwrResult))
            {
                m_MD5 = webReq.GetResponseHeader("x-amz-meta-cdn-md5");
                webAsyncOperation.completed -= AsyncOperation_HeaderComplete;
                webReq.Dispose();
                webAsyncOperation = null;

                HotFixStateManager.Instance.CheckVersionProgress = 0.7f;
                m_Retries = 0;
                m_TimeoutCount = 0;
                RemoteHotFixAssetsListUrl = string.Format("{0}/{1}/{2}.{3}/{4}?ts={5}",
              VersionHelper.HotFixUrl,
              AssetPath.sPlatformName,
              VersionHelper.RemoteBuildVersion,
              VersionHelper.RemoteAssetVersion,
              AssetPath.sHotFixListName,
              TimeManager.ClientNowMillisecond());
                BeginOperation();
            }
            else
            {
                webAsyncOperation.completed -= AsyncOperation_Complete;

                if (uwrResult == null)
                    uwrResult = new UnityWebRequestResult(webReq);

                if (HasTimedOut)
                {
                    uwrResult.Error = "Request timeout";
                }

                webReq.Dispose();
                webAsyncOperation = null;

                //要打点上报
                HotFixStateManager.HotFixHttpErrorHitPoint(uwrResult);

                //重复尝试3次
                if (m_Retries < RetryCount && uwrResult.Error != "Request aborted")
                {
                    m_Retries++;
                    DebugUtil.LogFormat(ELogType.eNone, string.Format("get header request failed url:{0}, retrying {1}/{2}", uwrResult.Url, m_Retries, RetryCount));
                    BeginHeaderOperation();
                }
                else if (m_TimeoutCount < VersionHelper.HotFixUrlArr.Length - 1)
                {
                    m_TimeoutCount++;
                    m_Retries = 0;
                    RemoteHotFixAssetsListUrl = string.Format("{0}/{1}/{2}.{3}/{4}", VersionHelper.HotFixUrlArr[m_TimeoutCount],
                       AssetPath.sPlatformName,
                       VersionHelper.RemoteBuildVersion,
                       VersionHelper.RemoteAssetVersion,
                        AssetPath.sHotFixListName);
                    DebugUtil.LogFormat(ELogType.eNone, string.Format("get header request replace cdn:{0}, retrying {1}/{2}", RemoteHotFixAssetsListUrl, m_Retries, RetryCount));
                    BeginHeaderOperation();
                }
                else
                {
                    AppManager.ResponseCode = uwrResult.ResponseCode;
                    AppManager.eAppError = EAppError.HttpError;
                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;
                }
            }
        }



        private void BeginOperation()
        {
            m_WebRequestCompletedCallbackCalled = false;
            string filePath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixTempListName);
            m_url = string.Format("{0}?ts={1}", RemoteHotFixAssetsListUrl, TimeManager.ClientNowMillisecond());
            DebugUtil.LogFormat(ELogType.eNone, "下载HotFixList {0}", m_url);
            UnityWebRequest webRequest = UnityWebRequest.Get(m_url);
            webRequest.downloadHandler = new DownloadHandlerFile(filePath);
            webAsyncOperation = webRequest.SendWebRequest();
            webAsyncOperation.completed += AsyncOperation_Complete;
        }

        private void AsyncOperation_Complete(AsyncOperation obj)
        {
            if (m_WebRequestCompletedCallbackCalled)
                return;

            m_WebRequestCompletedCallbackCalled = true;

            webAsyncOperation = obj as UnityWebRequestAsyncOperation;
            var webReq = webAsyncOperation?.webRequest;

            UnityWebRequestResult uwrResult = null;
            if (webReq != null && !UnityWebRequestUtilities.RequestHasErrors(webReq, out uwrResult))
            {
                //下载完成
                string url = webReq.url;
                webAsyncOperation.completed -= AsyncOperation_Complete;
                webReq.downloadHandler.Dispose();
                webReq.Dispose();
                webAsyncOperation = null;


                //下载过后需要校验
                string filePath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixTempListName);
                string loadedMD5 = FrameworkTool.GetFileMD5(filePath);
                if (m_MD5 != loadedMD5)
                {
                    HotFixStateManager.Instance.CheckVersionProgress = 1.0f;
                    AppManager.eAppError = EAppError.HotFixAssetListMD5Error;
                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;

                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("file_url", url);
                    dict.Add("file_md5_correct", m_MD5);
                    dict.Add("file_md5_error", loadedMD5);
                    HitPointManager.HitPoint("game_update_md5error", dict);

                    if (!File.Exists(filePath) || string.IsNullOrEmpty(loadedMD5))
                    {
                        DebugUtil.LogErrorFormat("不存在:{0}, 或者loadedMD5为空:{1}", filePath, string.IsNullOrEmpty(loadedMD5));
                    }

                    DebugUtil.LogErrorFormat("MD5验证失败:{0} Header记录md5:{1}  下载完成后md5:{2}", url, m_MD5, loadedMD5);
                }
                else
                {
                    HitPointManager.HitPoint("game_hotfix_assetlist_success");
                    HotFixStateManager.Instance.CheckVersionProgress = 1.0f;
                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.Check_HotFixList;

                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    VersionHelper.SetRemoteAssetsList(fileStream);
                    fileStream.Close();
                    fileStream.Dispose();

                    if (!string.Equals(VersionHelper.ResourceHotFixUniqueIdentifier, VersionHelper.mRemoteAssetsList.VersionIdentifier, StringComparison.Ordinal))
                    {
                        //不是同一系列热更资源 并且不是执意要热更的，可以走热更流程，不过出现错误，我已给出提示
                        if (!HotFixStateManager.Instance.HotFixSeriesErrorRetry)
                        {
                            HotFixStateManager.Instance.CheckVersionProgress = 1.0f;
                            AppManager.eAppError = EAppError.HotFixSeriesError;
                            HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;
                        }
                    }
                }

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            else
            {
                webAsyncOperation.completed -= AsyncOperation_Complete;

                if (uwrResult == null)
                    uwrResult = new UnityWebRequestResult(webReq);

                if (HasTimedOut)
                {
                    uwrResult.Error = "Request timeout";
                }

                webReq.downloadHandler.Dispose();
                webReq.Dispose();
                webAsyncOperation = null;

                //要打点上报
                HotFixStateManager.HotFixHttpErrorHitPoint(uwrResult);

                if (m_Retries < RetryCount && uwrResult.Error != "Request aborted")
                {
                    m_Retries++;
                    DebugUtil.LogFormat(ELogType.eNone, string.Format("Web request failed url:{0}, retrying {1}/{2}", uwrResult.Url, m_Retries, RetryCount));
                    BeginOperation();
                }
                else if (m_TimeoutCount < VersionHelper.HotFixUrlArr.Length - 1)
                {
                    m_TimeoutCount++;
                    m_Retries = 0;
                    RemoteHotFixAssetsListUrl = string.Format("{0}/{1}/{2}.{3}/{4}", VersionHelper.HotFixUrlArr[m_TimeoutCount],
                       AssetPath.sPlatformName,
                       VersionHelper.RemoteBuildVersion,
                       VersionHelper.RemoteAssetVersion,
                        AssetPath.sHotFixListName);
                    DebugUtil.LogFormat(ELogType.eNone, string.Format("Web request replace cdn:{0}, retrying {1}/{2}", RemoteHotFixAssetsListUrl, m_Retries, RetryCount));
                    BeginOperation();
                }
                else
                {
                    AppManager.ResponseCode = uwrResult.ResponseCode;
                    AppManager.eAppError = EAppError.HttpError;
                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;
                }
            }
        }

        public void OnExit()
        {
            if (webAsyncOperation != null)
            {
                DebugUtil.LogFormat(ELogType.eNone, "DownLoadAssetList onExit =========");
                if (webAsyncOperation.isDone)
                {
                    m_WebRequestCompletedCallbackCalled = true;
                    webAsyncOperation.webRequest.Abort();
                    webAsyncOperation.webRequest.downloadHandler.Dispose();
                    webAsyncOperation.webRequest.Dispose();
                    webAsyncOperation.completed -= AsyncOperation_Complete;
                    webAsyncOperation = null;
                }
                else
                {
                    m_WebRequestCompletedCallbackCalled = true;
                    webAsyncOperation.webRequest.Abort();
                    webAsyncOperation.webRequest.downloadHandler.Dispose();
                    webAsyncOperation.webRequest.Dispose();
                    webAsyncOperation.completed -= AsyncOperation_Complete;
                    webAsyncOperation = null;
                }
            }
        }


    }

    public class HotFix_AddressCheckCatalog : IHotFixState
    {
        public EHotFixState State
        {
            get
            {
                return EHotFixState.Check_AddressCatalog;
            }
        }

        public enum CheckCatalog
        {
            Invalid = 0,
            InitializeAsync = 1,
            CheckUpdate = 2,
            UpdateCatalog = 3,
            GetDownLoadSize = 4
        }

        private CheckCatalog m_currentStatus = CheckCatalog.Invalid;
        private CheckCatalog m_nextStatus = CheckCatalog.Invalid;

        List<string> checkResult;

        AsyncOperationHandle<IResourceLocator> initHandle;
        AsyncOperationHandle<List<string>> checkHandle;
        AsyncOperationHandle<List<IResourceLocator>> updateHandle;
        AsyncOperationHandle<long> sizeHandle;
        public void OnEnter()
        {
            DebugUtil.LogFormat(ELogType.eNone, "进入状态：EHotFixState.Check_AddressCatalog");

            //写本地catalog.hash
            //string catalogHashName = string.Format("{0}/catalog_{1}.{2}.hash", AssetPath.sAddressableCatalogDir, VersionHelper.StreamingBuildVersion, VersionHelper.StreamingAssetVersion);
            //string catalogHashPath = AssetPath.GetPersistentFullPath(catalogHashName);
            //if (File.Exists(catalogHashPath))
            //    File.Delete(catalogHashPath);

            //string catalogName = string.Format("{0}/catalog_{1}.{2}.json", AssetPath.sAddressableCatalogDir, VersionHelper.StreamingBuildVersion, VersionHelper.StreamingAssetVersion);
            //if (VersionHelper.mRemoteAssetsList.Contents.TryGetValue(catalogName, out AssetsInfo assetsInfo))
            //{
            //    File.WriteAllText(catalogHashPath, assetsInfo.AssetMD5);
            //}

            //HotFixStateManager.Instance.NextHotFixState = EHotFixState.Success;
            m_nextStatus = CheckCatalog.InitializeAsync;
        }

        public void OnExit()
        {
            switch (m_currentStatus)
            {
                case CheckCatalog.InitializeAsync:
                    if (initHandle.IsValid())
                        AddressablesUtil.Release(ref initHandle, InitHandle_Completed);
                    break;
                case CheckCatalog.CheckUpdate:
                    if (checkHandle.IsValid())
                        AddressablesUtil.Release(ref checkHandle, Checkhandle_Completed);
                    break;
                case CheckCatalog.UpdateCatalog:
                    if (updateHandle.IsValid())
                        AddressablesUtil.Release(ref updateHandle, UpdateHandle_Completed);
                    break;
                case CheckCatalog.GetDownLoadSize:
                    if (sizeHandle.IsValid())
                        AddressablesUtil.Release(ref sizeHandle, SizeHandle_Completed);
                    break;
            }
        }

        public void OnUpdate()
        {
            if (m_nextStatus == CheckCatalog.Invalid)
                return;

            if (m_nextStatus == m_currentStatus)
                return;

            m_currentStatus = m_nextStatus;
            m_nextStatus = CheckCatalog.Invalid;

            switch (m_currentStatus)
            {
                case CheckCatalog.InitializeAsync:
                    InitAsync();
                    break;
                case CheckCatalog.CheckUpdate:
                    CheckUpdate();
                    break;
                case CheckCatalog.UpdateCatalog:
                    UpdateCatalog();
                    break;
                case CheckCatalog.GetDownLoadSize:
                    GetDownLoadSize();
                    break;
            }

        }

        #region 初始化
        public void InitAsync()
        {
            initHandle = Addressables.InitializeAsync();
            initHandle.Completed += InitHandle_Completed;
        }

        private void InitHandle_Completed(AsyncOperationHandle<IResourceLocator> obj)
        {
            AddressablesUtil.Release(ref initHandle, InitHandle_Completed);
            m_nextStatus = CheckCatalog.CheckUpdate;
        }

        #endregion


        #region 检查目录是否更新
        private void CheckUpdate()
        {
            checkHandle = Addressables.CheckForCatalogUpdates(false);
            checkHandle.Completed += Checkhandle_Completed;
        }
        private void Checkhandle_Completed(AsyncOperationHandle<List<string>> obj)
        {
            if (checkHandle.IsDone && checkHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (checkHandle.Result.Count > 0)
                {
                    checkResult = checkHandle.Result;
                    m_nextStatus = CheckCatalog.UpdateCatalog;
                }
                else
                {
                    HotFixStateManager.Instance.CheckVersionProgress = 0.4f;
                    m_nextStatus = CheckCatalog.GetDownLoadSize;
                }
            }
            else
            {
                DebugUtil.LogError("Addressable CheckForCatalogUpdates failed !!!");

                AppManager.eAppError = EAppError.HttpError;
                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;
            }

            AddressablesUtil.Release(ref checkHandle, Checkhandle_Completed);
        }
        #endregion


        #region 目录更新
        private void UpdateCatalog()
        {
            updateHandle = Addressables.UpdateCatalogs(checkResult, false);
            updateHandle.Completed += UpdateHandle_Completed;
        }

        private void UpdateHandle_Completed(AsyncOperationHandle<List<IResourceLocator>> obj)
        {
            if (updateHandle.IsDone && updateHandle.Status == AsyncOperationStatus.Succeeded)
            {
                HotFixStateManager.Instance.CheckVersionProgress = 0.4f;
                m_nextStatus = CheckCatalog.GetDownLoadSize;
            }
            else
            {
                DebugUtil.LogError("Addressable UpdateCatalogs failed !!!");

                AppManager.eAppError = EAppError.HttpError;
                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;
            }

            AddressablesUtil.Release(ref updateHandle, UpdateHandle_Completed);
        }

        #endregion


        #region 计算下载大小

        private void GetDownLoadSize()
        {
            // 不走 Addressable计算
            //if (VersionHelper.mRemoteAaBundleList.Count > 0)
            //{
            //    sizeHandle = Addressables.GetDownloadSizeAsync(VersionHelper.mRemoteAaBundleList);
            //    sizeHandle.Completed += SizeHandle_Completed;
            //}
            //else
            //{
            //    HotFixStateManager.Instance.CheckVersionProgress = 0.45f;
            //    HotFixStateManager.Instance.NextHotFixState = EHotFixState.Check_HotFixList;
            //}

            HotFixStateManager.Instance.CheckVersionProgress = 0.45f;
            HotFixStateManager.Instance.NextHotFixState = EHotFixState.Check_HotFixList;
        }

        private void SizeHandle_Completed(AsyncOperationHandle<long> obj)
        {
            if (sizeHandle.IsDone && sizeHandle.Status == AsyncOperationStatus.Succeeded)
            {
                HotFixStateManager.Instance.CheckVersionProgress = 0.45f;
                HotFixStateManager.Instance.HotFixTotalSize = (ulong)sizeHandle.Result;
                HotFixStateManager.Instance.AssetBundleTotalSize = (ulong)sizeHandle.Result;

                // HotFixStateManager.Instance.NextHotFixState = EHotFixState.Check_HotFixList;
                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Success;

                DebugUtil.LogFormat(ELogType.eNone, string.Format("Load Addressable Bundle Size:{0}", HotFixStateManager.Instance.HotFixTotalSize));
            }
            else
            {
                Debug.LogError("Load Addressable Bundle Size failed !!!");

                AppManager.eAppError = EAppError.HttpError;
                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;
            }

            AddressablesUtil.Release(ref sizeHandle, SizeHandle_Completed);
        }

        #endregion
    }

    public class HotFix_CheckHotFixList : CheckHotFixBase, IHotFixState
    {
        public EHotFixState State
        {
            get { return EHotFixState.Check_HotFixList; }
        }

        private bool isUpdate = false;
        public void OnEnter()
        {
            DebugUtil.LogFormat(ELogType.eNone, "进入状态：EHotFixState.Check_HotFixList");
            isUpdate = true;
            base.OnEnter(CheckHotFixList.LoadPersistent);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {
            if (isUpdate)
            {
                base.OnUpdate();
                if (base.IsDone())
                {
                    isUpdate = false;
                    MakeSureStatus();
                }
            }
        }


        private void MakeSureStatus()
        {
            if (HotFixStateManager.Instance.HotFixFileCount > 0 || HotFixStateManager.Instance.ExcessAssets.Count > 0 || HotFixStateManager.Instance.HotFixTotalSize > 0)
            {
                HitPointManager.HitPoint("game_hotfix_size");
                if (HotFixStateManager.Instance.MemoryEnough())
                {
                    //移动网络 >100M给提示
                    if (HotFixStateManager.Instance.HotFixTotalSize > HotFixStateManager.Instance.HotFixNeedTipSize && NetworkReachability.ReachableViaCarrierDataNetwork == Application.internetReachability) //--先关闭手机数据
                    {
                        HitPointManager.HitPoint("game_update_show");
                        HotFixStateManager.Instance.NextHotFixState = EHotFixState.Wait_FixAsset;//手机>100M,需等玩家同意，待下载资源状态
                    }
                    else
                    {   //wifi直接下载资源
                        if (HotFixStateManager.Instance.ExcessAssets.Count > 0)
                        {
                            HotFixStateManager.Instance.NextHotFixState = EHotFixState.Destroy_SpareAssets;
                        }
                        else
                        {
                            //if (VersionHelper.mRemoteAaBundleList.Count > 0)
                            {
                                //HotFixStateManager.Instance.NextHotFixState = EHotFixState.DownLoad_AddressAsset;
                                //Todo -yd Temp
                                if (HotFixStateManager.Instance.NewAssets.Count > 0)
                                {
                                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.DownLoad_ConfigAsset;
                                }
                                else
                                {
                                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.ReCheck_HotFixList;
                                }
                            }
                            //else if (HotFixStateManager.Instance.NewAssets.Count > 0)
                            //{
                            //    HotFixStateManager.Instance.NextHotFixState = EHotFixState.DownLoad_ConfigAsset;
                            //}
                            //else
                            //{
                            //    HotFixStateManager.Instance.NextHotFixState = EHotFixState.ReCheck_HotFixList;
                            //}
                        }
                    }
                }

                HotFixStateManager.Instance.CheckVersionProgress = 1f;
            }
            else
            {
                //当资源版本号标记不一致 但又没有资源更新的时候 仅将资源版本号更新下
                string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
                VersionHelper.SetPersistentVersion(VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion);
                VersionHelper.WritePersistentVersion(persistentVersionPath);

                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Success;
            }
        }
    }

    public class HotFix_DestroySpareAssets : IHotFixState
    {
        public EHotFixState State
        {
            get { return EHotFixState.Destroy_SpareAssets; }
        }

        string persistentAssetsListPath;
        int count;
        int numMax;
        float i;
        bool isUpdate = false;

        public void OnEnter()
        {
            isUpdate = true;
            count = HotFixStateManager.Instance.ExcessAssets.Count;
            numMax = count / 30;
            numMax = numMax < 10 ? 10 : numMax;
            persistentAssetsListPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sHotFixListName);
            DebugUtil.LogFormat(ELogType.eNone, "进入状态：EHotFixState.Destroy_SpareAssets");
        }

        public void OnExit()
        {
            persistentAssetsListPath = null;
        }

        public void OnUpdate()
        {
            if (isUpdate)
                DestroyAsset();
        }

        private void DestroyAsset()
        {
            int num = 0;
            while (HotFixStateManager.Instance.ExcessAssets.Count > 0)
            {
                HotFixStateManager.Instance.SetProgress(0f, 1f, i / count);
                i = i + 1;

                AssetsInfo info = HotFixStateManager.Instance.ExcessAssets.Dequeue();

                if (info.AssetType == (int)AssetType.config || info.AssetType == (int)AssetType.catalog)
                {
                    //删除缓存不使用的 config/dll
                    string fileNamePath = AssetPath.GetPersistentAssetFullPath(info.AssetName);
                    if (File.Exists(fileNamePath))
                        File.Delete(fileNamePath);
                    VersionHelper.AppendAssetInfoToPersistent(info);//要不要是不是都可以？
                }
                else
                {
                    //删除缓存不使用的bundle
                    string hashPathDir = string.Format("{0}/{1}", Caching.defaultCache.path, info.AssetName);
                    FileOperationHelper.DeleteDirectory(hashPathDir);
                }

                ++num;
                if (num > numMax)
                {//每300个文件删除，等到下帧执行，避免卡顿
                    break;
                }
            }


            if (HotFixStateManager.Instance.ExcessAssets.Count <= 0)
            {
                isUpdate = false;
                //打点，热更新资源开始下载
                if (HotFixStateManager.Instance.NewAssets.Count > 0)
                {
                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.DownLoad_ConfigAsset;
                }
                else
                {
                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.ReCheck_HotFixList;
                }
            }
        }
    }

    public class HotFix_DownLoadAddressAsset : IHotFixState
    {
        public EHotFixState State
        {
            get { return EHotFixState.DownLoad_AddressAsset; }
        }

        AsyncOperationHandle downloadHandle;
        public void OnEnter()
        {
            DebugUtil.LogFormat(ELogType.eNone, "进入状态：EHotFixState.DownLoad_AddressAsset");
            //HitPointManager.HitPoint("game_hotfix_asset_start");
            DownloadAssets();
        }

        public void OnExit()
        {
            if (downloadHandle.IsValid())
                AddressablesUtil.Release(ref downloadHandle, DownloadHandle_Completed);
        }

        public void OnUpdate()
        {
            if (downloadHandle.IsValid() && !downloadHandle.IsDone)
            {
                DownloadStatus downloadStatus = downloadHandle.GetDownloadStatus();
                if (downloadStatus.DownloadedBytes > 0)
                    HotFixStateManager.Instance.HotFixSize = (ulong)downloadStatus.DownloadedBytes;
            }
        }


        private void DownloadAssets()
        {
            if (VersionHelper.mRemoteAaBundleList != null && HotFixStateManager.Instance.AssetBundleTotalSize > 0)
            {
                DebugUtil.LogFormat(ELogType.eNone, "需要更新的资源(Bundle) Size:{0}", HotFixStateManager.Instance.HotFixTotalSize);
                DebugUtil.LogFormat(ELogType.eNone, "需要更新的资源(Bundle) Count:{0}", VersionHelper.mRemoteAaBundleList.Count);

                downloadHandle = Addressables.DownloadDependenciesAsync(VersionHelper.mRemoteAaBundleList, Addressables.MergeMode.Union);
                downloadHandle.Completed += DownloadHandle_Completed;
            }
            else
            {
                if (HotFixStateManager.Instance.NewAssets.Count > 0)
                {
                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.DownLoad_ConfigAsset;
                }
                else
                {
                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.ReCheck_HotFixList;
                }
            }
        }

        private void DownloadHandle_Completed(AsyncOperationHandle obj)
        {
            if (downloadHandle.IsDone && downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                DownloadStatus downloadStatus = downloadHandle.GetDownloadStatus();
                DebugUtil.LogFormat(ELogType.eNone, string.Format("已经下载Bundle Size:{0} byte", downloadStatus.TotalBytes));

                AddressablesUtil.Release(ref downloadHandle, null);
                if (HotFixStateManager.Instance.NewAssets.Count > 0)
                {
                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.DownLoad_ConfigAsset;
                }
                else
                {
                    HotFixStateManager.Instance.NextHotFixState = EHotFixState.ReCheck_HotFixList;
                }
            }
            else
            {
                Debug.LogError("Load Addressable Bundle Asset failed !!!");
                if (downloadHandle.OperationException.InnerException != null &&
                       downloadHandle.OperationException.InnerException.InnerException != null &&
                       downloadHandle.OperationException.InnerException.InnerException.InnerException != null)
                {
                    //RemoteProviderException -> ProviderException -> OperationException -> Exception
                    UnityEngine.ResourceManagement.Exceptions.RemoteProviderException providerException = downloadHandle.OperationException.InnerException.InnerException.InnerException as UnityEngine.ResourceManagement.Exceptions.RemoteProviderException;
                    if (providerException != null && providerException.WebRequestResult != null)
                    {
                        UnityWebRequestResult webRequestResult = providerException.WebRequestResult;
                        AppManager.ResponseCode = webRequestResult.ResponseCode;//404
                    }
                }

                AppManager.eAppError = EAppError.HttpError;
            }

            AddressablesUtil.Release(ref downloadHandle, DownloadHandle_Completed);

        }
    }

    public class HotFix_DownLoadConfigAsset : IHotFixState
    {
        public EHotFixState State
        {
            get
            {
                return EHotFixState.DownLoad_ConfigAsset;
            }
        }

        //string persistentAssetsListPath;
        //List<ConfigAssetProvider> configProviderList;

        string cacheAssetsListPath;
        bool isUpdate = false;

        //所有资源列表 下载队列
        int s_MaxRequest = 5;
        List<DownloadAssetsInfo> s_RecordAllAssets = new List<DownloadAssetsInfo>();
        List<DownloadAssetsInfo> s_ActiveRequests = new List<DownloadAssetsInfo>();
        Queue<DownloadAssetsInfo> s_QueuedOperations = new Queue<DownloadAssetsInfo>();


        public void OnEnter()
        {
            isUpdate = true;
            HotFixStateManager.Instance.LoadedHotFixFileCount = 0;
            cacheAssetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);
            //persistentAssetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);

            HitPointManager.HitPoint("game_update_start");
            DebugUtil.LogFormat(ELogType.eNone, "进入状态：EHotFixState.DownLoad_ConfigAsset");
            StartDownloadAsset();
        }

        public void OnExit()
        {
            for (int i = 0; i < s_ActiveRequests.Count; ++i)
            {
                s_ActiveRequests[i].Exit();
            }
            s_ActiveRequests.Clear();


            for (int i = 0; i < s_QueuedOperations.Count; i++)
            {
                DownloadAssetsInfo queueAsset = s_QueuedOperations.Dequeue();
                queueAsset.Exit();
            }
            s_QueuedOperations.Clear();


            s_RecordAllAssets.Clear();
        }


        public void OnUpdate()
        {
            if (!isUpdate)
                return;

            bool loadcomplete = HotFixStateManager.Instance.AssetsLoadComplete();
            if (loadcomplete)
            {
                isUpdate = false;
                //说明资源已全部下完  s_ActiveRequests s_QueuedOperations 已经清空 
                Debug.Log("热更新 资源已全部下载完");
                if (File.Exists(cacheAssetsListPath))
                    File.Delete(cacheAssetsListPath);

                HitPointManager.HitPoint("game_update_download_success");
                HotFixStateManager.Instance.NextHotFixState = EHotFixState.ReCheck_HotFixList;
            }
            else
            {
                //active complete + update
                for (int i = 0; i < s_ActiveRequests.Count; ++i)
                {
                    DownloadAssetsInfo tempAssetInfo = s_ActiveRequests[i];
                    if (tempAssetInfo == null)
                    {
                        Debug.Log("DownloadAssetsInfo tempAssetInfo == null");
                        continue;
                    }

                    if (tempAssetInfo.IsDone())
                    {
                        if (!tempAssetInfo.IsError())
                        {
                            tempAssetInfo.RemoveFlag = true;
                            tempAssetInfo.CompleteDispose();
                        }
                        else
                        {
                            tempAssetInfo.RemoveFlag = true;
                            tempAssetInfo.HttpErrorReport();
                            if (tempAssetInfo.JudageEnqueue())
                            {
                                tempAssetInfo.Dispose(true);
                                s_QueuedOperations.Enqueue(s_ActiveRequests[i]);
                            }
                            else
                            {
                                //error + dispose
                                tempAssetInfo.ErrorDispose();
                            }
                        }
                    }
                    else if (tempAssetInfo.HasTimedOut)
                    {
                        tempAssetInfo.RemoveFlag = true;
                        tempAssetInfo.HttpErrorReport();
                        if (tempAssetInfo.JudageEnqueue())
                        {
                            tempAssetInfo.Dispose(true);
                            s_QueuedOperations.Enqueue(tempAssetInfo);
                        }
                        else
                        {
                            //error + dispose
                            tempAssetInfo.ErrorDispose();
                        }
                    }
                    else
                    {
                        tempAssetInfo.Update();
                    }
                }

                //remove old Asset from ActiveList
                for (int i = s_ActiveRequests.Count - 1; i >= 0; --i)
                {
                    DownloadAssetsInfo tempInfo = s_ActiveRequests[i];
                    if (tempInfo.RemoveFlag == true || tempInfo == null)
                        s_ActiveRequests.Remove(tempInfo);
                }

                //add ActiveList from Queque
                if (s_ActiveRequests.Count < s_MaxRequest)
                {
                    if (s_QueuedOperations.Count > 0)
                    {
                        DownloadAssetsInfo queueInfo = s_QueuedOperations.Dequeue();
                        queueInfo.StartDownload();
                        s_ActiveRequests.Add(queueInfo);
                    }
                }
            }

            //real-time compulate downLoadedAsset size
            ulong hotfixsize = 0;
            for (int i = 0; i < s_RecordAllAssets.Count; i++)
            {
                hotfixsize += s_RecordAllAssets[i].GetDownLoadSize();
            }
            HotFixStateManager.Instance.HotFixSize = hotfixsize;
        }


        private void StartDownloadAsset()
        {
            foreach (var item in HotFixStateManager.Instance.NewAssets)
            {
                DownloadAssetsInfo tempInfo = new DownloadAssetsInfo();
                tempInfo.Start(item);
                s_RecordAllAssets.Add(tempInfo);

                if (s_ActiveRequests.Count < s_MaxRequest)
                {
                    tempInfo.StartDownload();
                    s_ActiveRequests.Add(tempInfo);
                }
                else
                {
                    s_QueuedOperations.Enqueue(tempInfo);
                }
            }

            Debug.Log("需要下载资源 Count:{0}" + HotFixStateManager.Instance.NewAssets.Count);
        }


    }

    public class HotFix_ReCheckHotFixList : CheckHotFixBase, IHotFixState
    {
        public EHotFixState State
        {
            get
            {
                return EHotFixState.ReCheck_HotFixList;
            }
        }

        private bool isUpdate = false;
        public void OnEnter()
        {
            isUpdate = true;
            base.OnEnter(CheckHotFixList.CompareList);
            DebugUtil.LogFormat(ELogType.eNone, "进入状态：EHotFixState.ReCheck_HotFixList");
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {
            if (isUpdate)
            {
                base.OnUpdate();
                if (base.IsDone())
                {
                    isUpdate = false;
                    ReCheckAssetList();
                }
            }
        }
        private void ReCheckAssetList()
        {
            if (HotFixStateManager.Instance.NewAssets.Count > 0)
            {
                AppManager.eAppError = EAppError.AssetVerifyError;
            }
            else
            {
                //所有资源下载完成 (Catalog.json下载完成,需要写入Catalog.hash文件）
                string catalogHashName = string.Format("{0}/catalog_{1}.{2}.hash", AssetPath.sAddressableCatalogDir, VersionHelper.StreamingBuildVersion, "0");
                string catalogHashPath = AssetPath.GetPersistentFullPath(catalogHashName);
                if (File.Exists(catalogHashPath))
                    File.Delete(catalogHashPath);

                string catalogName = string.Format("{0}/catalog_{1}.{2}.json", AssetPath.sAddressableCatalogDir, VersionHelper.StreamingBuildVersion, "0");
                if (VersionHelper.mRemoteAssetsList.Contents.TryGetValue(catalogName, out AssetsInfo assetsInfo))
                {
                    File.WriteAllText(catalogHashPath, assetsInfo.AssetMD5);
                }

                //最后写入版本文件
                string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
                VersionHelper.SetPersistentVersion(VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion);
                VersionHelper.WritePersistentVersion(persistentVersionPath);

                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Success;
                HitPointManager.HitPoint("game_update_success");
            }
        }
    }

}
