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

        int m_TimeoutCount = 0;//����ʱ�Ĵ���
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
                persistFilePath = string.Format("{0}/{1}/{2}/__data", Caching.defaultCache.path, info.BundleName, info.HashOfBundle);//���غ���䵽__data
            }
            else
            {
                string catalogHashName = info.AssetName.Split('/')[1];
                relativeFilePath = string.Format("{0}/{1}", AssetPath.sAddressableDir, catalogHashName);
                persistFilePath = AssetPath.GetPersistentFullPath(info.AssetName);
            }

            #region �����¼���Ƿ���Ҫɾ�� + �Ƿ���Ҫ���뻺���¼

            //������ǻ�������ɾ�� || ���߻����ˣ�����¼���ļ��Ļ����б�ɾ������Ҫ��������(��Ϊ��¼���ļ���Ϣ��md5�Ѷ�ʧ����ȷ�����ļ�Զ���Ƿ����и���)
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

            //У��ʧ�ܵ� ��Ҫɾ���ٴ�����
            if (HotFixStateManager.Instance.RetryDownloadCountDict.ContainsKey(assetInfo.AssetName))
            {
                if (File.Exists(persistFilePath))
                {
                    File.Delete(persistFilePath);
                }
            }

            //��ȡ�����ļ�����  ����1M���뻺��
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
            //���ص�һ����ļ� ����ʣ����Ҫ���ص���Դ��С
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

                //    //m_LastDownloadedBytes:���ڼ�¼���ص�һ�����ģ���������ʾ���ܻع�
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
                DebugUtil.LogErrorFormat("��Ҫ���ش�С:{0}��������ɴ�С:{1}, path:{2}", m_BytesToDownload, m_DownloadedBytes, persistFilePath);
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
                Debug.LogError(string.Format("��������У���Ϊ���Դ������ Count:{0}", VersionHelper.HotFixUrlArr.Length * RetryCount));
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
                //m_LastDownloadedByteCount��ֻ�Ǳ���WebRequest����
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

                //*****˵������������ʧ�ܣ��������ļ�ָ���λ�ã����Բ������ļ�ͷ��һ�µ����,ɾ���ᵼ�������ļ���С���ü��㣨ɾ���ᵼ���ļ������£�������******/
                //�����ڻ����Ӻ�Ҫ��ǰһ�����ӱ����ɾ�� ����ҪŪ���ͬһ���ļ�����ͬcdn��ַ�µ��ļ�ͷһ�����
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
                Debug.LogError("�ߵ�������������Ŷ�����Ѿ���������cdn��ַ�ɳ��ԵĴ�������ô�죿�ʳ���������");
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

                Debug.LogErrorFormat("MD5��֤ʧ��:{0} ��¼md5:{1}  ����md5:{2}", assetInfo.AssetName, assetInfo.AssetMD5, md5);

                //У��ʧ�ܵ����⴦��,ֱ�ӱ�ע������
                if (HotFixStateManager.Instance.RetryDownloadCountDict.ContainsKey(assetInfo.AssetName))
                {
                    HotFixStateManager.Instance.RetryDownloadCountDict[assetInfo.AssetName]++;
                }
                else
                {
                    HotFixStateManager.Instance.RetryDownloadCountDict.Add(assetInfo.AssetName, 1);
                }
            }

            //���绺���������������ص�  ��ô��������ɺ�Ҫɾ���������������صļ�¼
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

        #region ����ɳ��Ŀ¼���ȸ��б�
        private void LoadPersistentAssetList()
        {
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);
            if (File.Exists(assetsListPath))
            {
                assetsListPath = string.Format("{0}{1}", AssetPath.sPersistentPrefix, assetsListPath);
                DebugUtil.LogFormat(ELogType.eNone, "����Persistent��Դ�б� {0}", assetsListPath);

                UnityWebRequest request = UnityWebRequest.Get(new System.Uri(assetsListPath));
                webAsyncOperation = request.SendWebRequest();
                webAsyncOperation.completed += PersistWebAsyncOperation_completed;
            }
            else
            {
                DebugUtil.LogFormat(ELogType.eNone, "Persistent������ {0}", assetsListPath);
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
                //�������
                MemoryStream ms = new MemoryStream(webReq.downloadHandler.data);
                VersionHelper.SetPersistentAssetsList(ms);

                m_nextStatus = CheckHotFixList.LoadCache;
            }
            else
            {
                Debug.LogFormat("����Persistent��Դ�б�ʧ�� {0}", webReq.error);
                AppManager.ResponseCode = uwrResult.ResponseCode;
                AppManager.eAppError = EAppError.HttpError;
                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;
            }

            webAsyncOperation.completed -= PersistWebAsyncOperation_completed;
            webAsyncOperation.webRequest.Dispose();
            webAsyncOperation = null;
        }
        #endregion

        #region ���ػ����б�

        private void LoadCacheAssetList()
        {
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);

            if (File.Exists(assetsListPath))
            {
                assetsListPath = string.Format("{0}{1}", AssetPath.sPersistentPrefix, assetsListPath);
                Debug.LogFormat("����Persistent��Դ�б� {0}", assetsListPath);

                UnityWebRequest request = UnityWebRequest.Get(new System.Uri(assetsListPath));
                webAsyncOperation = request.SendWebRequest();
                webAsyncOperation.completed += CacheWebAsyncOperation_completed;
            }
            else
            {
                DebugUtil.LogFormat(ELogType.eNone, "Persistent������ {0}", assetsListPath);
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
                Debug.LogFormat("����Persistent��Դ�б�ʧ�� {0}", webReq.error);
                AppManager.ResponseCode = uwrResult.ResponseCode;
                AppManager.eAppError = EAppError.HttpError;
                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Invalid;
            }

            webAsyncOperation.completed -= CacheWebAsyncOperation_completed;
            webAsyncOperation.webRequest.Dispose();
            webAsyncOperation = null;
        }
        #endregion

        #region �ȶ���Դ�б�
        private void CompareAssetsList(float startProgress = 0f, float endProgress = 1f)
        {
            DebugUtil.LogFormat(ELogType.eNone, "�Ա���Դ�б�");
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

                #region ����һ�����������ɾ��ɳ��·������Դ����ɾ����¼��HotfixList.txt�ļ�����ô�Ѿ����ع�����Դ Ҳ���������أ���˫�ر���
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


                if (VersionHelper.mPersistentAssetsList == null //HotFixList.txt�ļ�������
                    || !VersionHelper.mPersistentAssetsList.Contents.TryGetValue(remoteInfo.AssetName, out localInfo)
                    || !remoteInfo.AssetMD5.Equals(localInfo.AssetMD5)
                    || remoteInfo.State != localInfo.State
                    || remoteInfo.Version != localInfo.Version)
                {
                    //0 = �޸Ļ������� 1 = ɾ��
                    if (remoteInfo.State == 0)
                    {
                        AssetsInfo cacheInfo = null;
                        //�����¼���ļ�(>1M �ϵ�����)�Ļ����ļ��б�HotFixCacheList.txt��Ϊɾ������ʱ���ļ���Ҫ�������أ������߶ϵ�����
                        //���绺���ļ��б�HotFixCacheList.txt���ڣ���Ϊɾ��HotFixCacheList.txt�м�¼�Ķ�Ӧ��Դ�ļ�
                        if (HotFixStateManager.Instance.CacheAssetList == null
                            || !HotFixStateManager.Instance.CacheAssetList.Contents.TryGetValue(remoteInfo.AssetName, out cacheInfo)
                            || cacheInfo.AssetMD5 != remoteInfo.AssetMD5
                            || cacheInfo.Version != remoteInfo.Version)
                        {
                            HotFixStateManager.Instance.NewAssets.Add(remoteInfo);
                            //������£������ļ���dll�ļ�����Դ��С
                            HotFixStateManager.Instance.HotFixTotalSize += remoteInfo.Size;
                            ++HotFixStateManager.Instance.HotFixFileCount;
                        }
                        else
                        {
                            //���ص�һ����ļ��������ļ��б����               
                            HotFixStateManager.Instance.NewAssets.Insert(0, remoteInfo);

                            //���ص�һ����ļ� ����ʣ����Ҫ���ص���Դ��С
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
                    //yield return null;���������Ϊ�˷�ֹ���٣���ʱ�Ȳ�����
                }
            }

            //����Ҫ������ļ��б�>1M�����浽����
            if (HotFixStateManager.Instance.CacheAssetList != null)
                HotFixStateManager.Instance.CacheAssetList.Contents.Clear();
            HotFixStateManager.Instance.CacheAssetList = newCacheList;
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);
            VersionHelper.WriteAssetList(assetsListPath, HotFixStateManager.Instance.CacheAssetList);

            //������Ҫ���ص���Դд�ص�ɳ��HotFixList.txt
            VersionHelper.AppendAssetInfoList(tempRecordList);

            DebugUtil.LogFormat(ELogType.eNone, "��Ҫ���µ���Դ(config��dll) {0}", HotFixStateManager.Instance.HotFixFileCount);
            DebugUtil.LogFormat(ELogType.eNone, "��Ҫɾ������Դ(config��dll) {0}", HotFixStateManager.Instance.ExcessAssets.Count);
            DebugUtil.LogFormat(ELogType.eNone, "�Ѿ��в������ص���Դ(config��dll) {0}", HotFixStateManager.Instance.CacheAssetList.Contents.Count);

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
        int m_TimeoutCount = 0;//����ʱ�Ĵ���
        string m_MD5;
        string m_url;

        private bool HasTimedOut => m_TimeoutTimer >= Timeout && m_TimeoutOverFrames > 5;
        string RemoteHotFixAssetsListUrl;
        bool m_WebRequestCompletedCallbackCalled = false;//ִֹͣ�лص������߻ص����ִ��
        private UnityWebRequestAsyncOperation webAsyncOperation;

        public void OnEnter()
        {
            m_Retries = 0;
            m_TimeoutCount = 0;
            m_WebRequestCompletedCallbackCalled = false;

            //��ʱ�����ļ�
            string filePath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixTempListName);
            if (File.Exists(filePath))
                File.Delete(filePath);

            HotFixStateManager.Instance.CheckVersionProgress = 0.3f;

            DebugUtil.LogFormat(ELogType.eNone, "����״̬��EHotFixState.DownLoad_HotFixList");
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
            //���¼�ⳬʱ
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

                //Ҫ����ϱ�
                HotFixStateManager.HotFixHttpErrorHitPoint(uwrResult);

                //�ظ�����3��
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
            DebugUtil.LogFormat(ELogType.eNone, "����HotFixList {0}", m_url);
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
                //�������
                string url = webReq.url;
                webAsyncOperation.completed -= AsyncOperation_Complete;
                webReq.downloadHandler.Dispose();
                webReq.Dispose();
                webAsyncOperation = null;


                //���ع�����ҪУ��
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
                        DebugUtil.LogErrorFormat("������:{0}, ����loadedMD5Ϊ��:{1}", filePath, string.IsNullOrEmpty(loadedMD5));
                    }

                    DebugUtil.LogErrorFormat("MD5��֤ʧ��:{0} Header��¼md5:{1}  ������ɺ�md5:{2}", url, m_MD5, loadedMD5);
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
                        //����ͬһϵ���ȸ���Դ ���Ҳ���ִ��Ҫ�ȸ��ģ��������ȸ����̣��������ִ������Ѹ�����ʾ
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

                //Ҫ����ϱ�
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
            DebugUtil.LogFormat(ELogType.eNone, "����״̬��EHotFixState.Check_AddressCatalog");

            //д����catalog.hash
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

        #region ��ʼ��
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


        #region ���Ŀ¼�Ƿ����
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


        #region Ŀ¼����
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


        #region �������ش�С

        private void GetDownLoadSize()
        {
            // ���� Addressable����
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
            DebugUtil.LogFormat(ELogType.eNone, "����״̬��EHotFixState.Check_HotFixList");
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
                    //�ƶ����� >100M����ʾ
                    if (HotFixStateManager.Instance.HotFixTotalSize > HotFixStateManager.Instance.HotFixNeedTipSize && NetworkReachability.ReachableViaCarrierDataNetwork == Application.internetReachability) //--�ȹر��ֻ�����
                    {
                        HitPointManager.HitPoint("game_update_show");
                        HotFixStateManager.Instance.NextHotFixState = EHotFixState.Wait_FixAsset;//�ֻ�>100M,������ͬ�⣬��������Դ״̬
                    }
                    else
                    {   //wifiֱ��������Դ
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
                //����Դ�汾�ű�ǲ�һ�� ����û����Դ���µ�ʱ�� ������Դ�汾�Ÿ�����
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
            DebugUtil.LogFormat(ELogType.eNone, "����״̬��EHotFixState.Destroy_SpareAssets");
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
                    //ɾ�����治ʹ�õ� config/dll
                    string fileNamePath = AssetPath.GetPersistentAssetFullPath(info.AssetName);
                    if (File.Exists(fileNamePath))
                        File.Delete(fileNamePath);
                    VersionHelper.AppendAssetInfoToPersistent(info);//Ҫ��Ҫ�ǲ��Ƕ����ԣ�
                }
                else
                {
                    //ɾ�����治ʹ�õ�bundle
                    string hashPathDir = string.Format("{0}/{1}", Caching.defaultCache.path, info.AssetName);
                    FileOperationHelper.DeleteDirectory(hashPathDir);
                }

                ++num;
                if (num > numMax)
                {//ÿ300���ļ�ɾ�����ȵ���ִ֡�У����⿨��
                    break;
                }
            }


            if (HotFixStateManager.Instance.ExcessAssets.Count <= 0)
            {
                isUpdate = false;
                //��㣬�ȸ�����Դ��ʼ����
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
            DebugUtil.LogFormat(ELogType.eNone, "����״̬��EHotFixState.DownLoad_AddressAsset");
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
                DebugUtil.LogFormat(ELogType.eNone, "��Ҫ���µ���Դ(Bundle) Size:{0}", HotFixStateManager.Instance.HotFixTotalSize);
                DebugUtil.LogFormat(ELogType.eNone, "��Ҫ���µ���Դ(Bundle) Count:{0}", VersionHelper.mRemoteAaBundleList.Count);

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
                DebugUtil.LogFormat(ELogType.eNone, string.Format("�Ѿ�����Bundle Size:{0} byte", downloadStatus.TotalBytes));

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

        //������Դ�б� ���ض���
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
            DebugUtil.LogFormat(ELogType.eNone, "����״̬��EHotFixState.DownLoad_ConfigAsset");
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
                //˵����Դ��ȫ������  s_ActiveRequests s_QueuedOperations �Ѿ���� 
                Debug.Log("�ȸ��� ��Դ��ȫ��������");
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

            Debug.Log("��Ҫ������Դ Count:{0}" + HotFixStateManager.Instance.NewAssets.Count);
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
            DebugUtil.LogFormat(ELogType.eNone, "����״̬��EHotFixState.ReCheck_HotFixList");
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
                //������Դ������� (Catalog.json�������,��Ҫд��Catalog.hash�ļ���
                string catalogHashName = string.Format("{0}/catalog_{1}.{2}.hash", AssetPath.sAddressableCatalogDir, VersionHelper.StreamingBuildVersion, "0");
                string catalogHashPath = AssetPath.GetPersistentFullPath(catalogHashName);
                if (File.Exists(catalogHashPath))
                    File.Delete(catalogHashPath);

                string catalogName = string.Format("{0}/catalog_{1}.{2}.json", AssetPath.sAddressableCatalogDir, VersionHelper.StreamingBuildVersion, "0");
                if (VersionHelper.mRemoteAssetsList.Contents.TryGetValue(catalogName, out AssetsInfo assetsInfo))
                {
                    File.WriteAllText(catalogHashPath, assetsInfo.AssetMD5);
                }

                //���д��汾�ļ�
                string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
                VersionHelper.SetPersistentVersion(VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion);
                VersionHelper.WritePersistentVersion(persistentVersionPath);

                HotFixStateManager.Instance.NextHotFixState = EHotFixState.Success;
                HitPointManager.HitPoint("game_update_success");
            }
        }
    }

}
