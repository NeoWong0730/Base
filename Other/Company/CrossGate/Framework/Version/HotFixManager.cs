using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

//#if USE_ADDRESSABLE_ASSET
using System.Linq;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using System;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement;
//#endif




namespace Framework
{
    public enum EHotFixPipeline
    {
        None = 0,
        HotFixSeries,//是不是同系列的热更
        AddressablesCheckCatalog,
        CheckAssetList,
        WaitFixAssets,
        DestroyAssets,
        FixAssets,
        AddressablesFixAssets,
        RecheckAssetList,
        Success,
        WaitFail,
        Fail,
    }

    public enum EUserConfirm
    {
        Yes,
        No,
    }

    // public class AssetDownloader
    // {
    //     public AssetsInfo mAssetsInfo = null;
    //     private UnityWebRequest webRequest = null;
    //     private FileStream fileStream = null;
    // 
    //     private string filePath;
    //     private string url;
    // 
    //     public long cacheLength { get; private set; }
    //     private int readOffset = 0;
    //     private int step = 0;
    // 
    //     public bool IsDone
    //     {
    //         get
    //         {
    //             return step == 3;
    //         }
    //     }
    // 
    //     public void Start(AssetsInfo info)
    //     {
    //         mAssetsInfo = info;
    //         url = Path.Combine(HotFixManager.Instance.DownloadABUrl, mAssetsInfo.AssetName);
    //         filePath = AssetPath.GetPersistentAssetFullPath(mAssetsInfo.AssetName);
    // 
    //         step = 1;
    //         webRequest = UnityWebRequest.Head(url);
    //         webRequest.SendWebRequest();
    //     }
    // 
    //     public void StartDownload()
    //     {
    //         step = 2;
    //         
    //         long totalLength = 0L;
    //         if (!long.TryParse(webRequest.GetResponseHeader("Content-Length"), out totalLength))
    //         {
    //             if (File.Exists(filePath))
    //             {
    //                 File.Delete(filePath);
    //             }
    //         }
    //         webRequest.Dispose();
    // 
    //         string dir = Path.GetDirectoryName(filePath);
    //         if (!Directory.Exists(dir))
    //         {
    //             Directory.CreateDirectory(dir);
    //         }
    // 
    //         cacheLength = fileStream.Length;
    //         fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
    //         fileStream.Seek(totalLength, SeekOrigin.Begin);
    // 
    //         readOffset = 0;
    // 
    //         webRequest = UnityWebRequest.Get(url);
    //         webRequest.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
    //         webRequest.SetRequestHeader("Pragma", "no-cache");
    //         if (totalLength > 0L)
    //         {
    //             webRequest.SetRequestHeader("Range", "bytes=" + cacheLength + "-" + totalLength);
    //         }
    //         webRequest.SendWebRequest();
    // 
    //         Debug.LogFormat("开始下载资源{0}", url);
    //     }
    // 
    //     public void CacheDownload()
    //     {
    //         byte[] buff = webRequest.downloadHandler.data;
    //         int readLength = buff.Length - readOffset;
    //         fileStream.Write(buff, readOffset, readLength);
    // 
    //         readOffset += readLength;
    //         cacheLength += readLength;
    //     }
    // 
    //     public void EndDownload()
    //     {
    //         step = 3;
    // 
    //         string md5 = MD5Utility.GetFileMD5(filePath);
    //         if (string.Equals(mAssetsInfo.AssetMD5, md5))
    //         {
    //             VersionHelper.AppendAssetInfo(persistentAssetsListPath, mAssetsInfo);
    //         }
    // 
    //         webRequest.Dispose();
    //         fileStream.Close();
    //         fileStream.Dispose();
    //     }
    // 
    //     public void CheckDownload()
    //     {
    //         if (step == 1)
    //         {
    //             if (webRequest.isDone)
    //             {
    //                 StartDownload();
    //             }
    //         }
    //         else if (step == 2)
    //         {
    //             CacheDownload();
    //             if (webRequest.isDone)
    //             {
    //                 if (webRequest.isNetworkError || webRequest.isHttpError || webRequest.responseCode != 200)
    //                 {
    //                     AppManager.eAppError = EAppError.HttpError;
    //                     AppManager.ResponseCode = webRequest.responseCode;
    //                 }
    // 
    //                 EndDownload();
    //             }
    //         }
    //     }
    // }

    public class HotFixManager : TSingleton<HotFixManager>
    {
        public const ulong KiloByte = 1024UL;
        public const ulong MegaByte = 1048576UL;
        public const ulong GigaByte = 1073741824UL;

        public const string sByteFormat = "{0:F2}Byte";
        public const string sKiloByteFormat = "{0:F2}K";
        public const string sMegaByteFormat = "{0:F2}M";
        public const string sGigaByteFormat = "{0:F2}G";

        /// <summary>需要提示的大小</summary>
        public readonly ulong HotFixNeedTipSize = MegaByte * 100UL;

        /// <summary>热更大小的倍率，用于计算内存剩余空间 </summary>
        public readonly float HotFixSizeRate = 2f;

        /// <summary>开启断点续传 最小文件大小</summary>
        public readonly ulong CacheFileMinSize = MegaByte;

        /// <summary>版本检测进度</summary>
        public float CheckVersionProgress { get; private set; }

        /// <summary>热更新文件的总数</summary>
        public int HotFixFileCount { get; private set; }

        /// <summary>下载速度</summary>
        public ulong DownloadSpeed { get; private set; }

        /// <summary>Bundle资源大小</summary>
        public ulong AssetBundleTotalSize { get; private set; }

        /// <summary>表格和代码资源总大小</summary>
        public ulong CSVAndDllTotalSize { get; private set; }

        /// <summary>更新总大小</summary>
        public ulong HotFixTotalSize { get; private set; }

        /// <summary>已经更新的大小</summary>
        public ulong HotFixSize { get; set; }

        /// <summary>远端catalog的大小 KB为单位</summary>
        public readonly ulong NeedLoadCatalogSize = 1024 * 20UL;


        /// <summary> 最后一次计算下载速度时的已更新大小 </summary>
        private ulong RecodeHotFixSize = 0;

        private float RecodeTime = 0;

        public int LoadedHotFixFileCount = 0;


        public EHotFixPipeline HotFixPipeline { get; private set; } = EHotFixPipeline.None;
        public EHotFixPipeline NextHotFixPipeline { get; private set; } = EHotFixPipeline.None;

        //备用下载远端资源地址
        public string DownloadABUrl;
        private string RemoteHotFixAssetsListUrl;
        // private string RemoteHotFixUniqueIdentifierUrl;

        private List<AssetsInfo> NewAssets = new List<AssetsInfo>();
        private Queue<AssetsInfo> ExcessAssets = new Queue<AssetsInfo>();
        public Dictionary<string, int> RetryDownloadCountDict = new Dictionary<string, int>();
        private int RetryCount = 3;
        private int ReallyRetryCount = 6;
        private int RecordDownloadCount = 0;




        private UnityWebRequest HttpReq = null;

        public AssetList mCacheAssetList = null;

        public static bool IsExitErrorAsset = false;
        public static Action CheckMD5AssetList;

       /* public static string CountSize(ulong Size)
        {
            string m_strSize = string.Empty;
            ulong FactSize = Size;

            if (Size < KiloByte)
            {
                m_strSize = string.Format(sByteFormat, FactSize);
            }
            else if (Size >= KiloByte && Size < MegaByte)
            {
                FactSize = Size / KiloByte;

                m_strSize = string.Format(sKiloByteFormat, FactSize);
            }
            else if (Size >= MegaByte && Size < GigaByte)
            {
                FactSize = Size / MegaByte;
                m_strSize = string.Format(sMegaByteFormat, FactSize);
            }
            else
            {
                FactSize = Size / GigaByte;
                m_strSize = string.Format(sGigaByteFormat, FactSize);

            }
            return m_strSize;
        }*/

        public void OnEnter()
        {
            VersionHelper.Clear();

            CheckVersionProgress = 0;
            RecodeHotFixSize = 0;
            RecodeTime = 0;

            DownloadABUrl = string.Format("{0}/{1}/{2}.{3}", VersionHelper.HotFixUrl, AssetPath.sPlatformName, VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion);

            RemoteHotFixAssetsListUrl = string.Format("{0}/{1}/{2}.{3}/{4}", VersionHelper.HotFixUrl, AssetPath.sPlatformName, VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion, AssetPath.sHotFixListName);

            HotFixPipeline = EHotFixPipeline.None;

            NextHotFixPipeline = EHotFixPipeline.HotFixSeries;

        }
        public void OnExit()
        {
            VersionHelper.Clear();
            //ResourceManager.HttpErrorOnComplete = null;

        }
        public void OnUpdate()
        {
            if (NextHotFixPipeline != HotFixPipeline)
            {
                HotFixPipeline = NextHotFixPipeline;

                switch (HotFixPipeline)
                {
                    case EHotFixPipeline.HotFixSeries:
                        CoroutineManager.Instance.StartHandler(CheckHotFixSeriesList());
                        break;
                    case EHotFixPipeline.AddressablesCheckCatalog:
                
                        CoroutineManager.Instance.StartHandler(AddressablesCheckCatalog());
                        break;
                    case EHotFixPipeline.CheckAssetList:
                        CoroutineManager.Instance.StartHandler(CheckAssetList());

                        break;
                    case EHotFixPipeline.WaitFixAssets:
                        break;
                    case EHotFixPipeline.DestroyAssets:
                        CoroutineManager.Instance.StartHandler(DestroyAssets());
                        break;
                    case EHotFixPipeline.AddressablesFixAssets:
                        RecodeTime = 0;
                        RecodeHotFixSize = 0;
                        CoroutineManager.Instance.StartHandler(AddressablesDownloadAssets());
                        break;
                    case EHotFixPipeline.FixAssets:
                        //RecodeTime = 0;
                        //RecodeHotFixSize = 0;
                        CoroutineManager.Instance.StartHandler(DownloadAssets());
                        break;
                    case EHotFixPipeline.RecheckAssetList:
                        CoroutineManager.Instance.StartHandler(ReCheckAssetList());
                        break;
                    case EHotFixPipeline.Success:
                        AppManager.NextAppState = EAppState.Game;
                        break;
                    case EHotFixPipeline.WaitFail:

                        break;
                    case EHotFixPipeline.Fail:
                        AppManager.Quit();
                        break;
                    default:
                        break;
                }
            }

            if (HotFixPipeline == EHotFixPipeline.FixAssets || HotFixPipeline == EHotFixPipeline.AddressablesFixAssets)
            {
                UpdateDownloadSpeed();
            }


            m_UpdatingReceivers = true;
            for (int i = 0; i < m_UpdateReceivers.Count; i++)
                m_UpdateReceivers[i].Update(Time.unscaledDeltaTime);
            m_UpdatingReceivers = false;

            if (m_UpdateReceiversToRemove != null)
            {
                foreach (var r in m_UpdateReceiversToRemove)
                    m_UpdateReceivers.Remove(r);
                m_UpdateReceiversToRemove = null;
            }
        }

        /// <summary>
        /// 用户确认操作
        /// </summary>
        /// <param name="userConfirm"></param>
        public void SetUserConfirm(EUserConfirm userConfirm)
        {
            if (HotFixPipeline == EHotFixPipeline.WaitFixAssets)
            {
                if (userConfirm == EUserConfirm.Yes)
                {
                    if (ExcessAssets.Count > 0)
                    {
                        NextHotFixPipeline = EHotFixPipeline.DestroyAssets;
                    }
                    else
                    {
                        //NextHotFixPipeline = EHotFixPipeline.FixAssets;
                        //先下载bundle
                        //HitPointManager.HitPoint("game_hotfix_asset_start");
                        if (VersionHelper.mRemoteAaBundleList.Count > 0)
                        {
                            NextHotFixPipeline = EHotFixPipeline.AddressablesFixAssets;
                        }
                        else
                        {
                            NextHotFixPipeline = EHotFixPipeline.FixAssets;
                        }

                    }

                }
                else if (userConfirm == EUserConfirm.No)
                {
                    NextHotFixPipeline = EHotFixPipeline.Fail;
                }
            }
            else if (HotFixPipeline == EHotFixPipeline.WaitFail)
            {
                if (userConfirm == EUserConfirm.Yes)
                {
                    NextHotFixPipeline = EHotFixPipeline.CheckAssetList;
                }
                else if (userConfirm == EUserConfirm.No)
                {
                    NextHotFixPipeline = EHotFixPipeline.Fail;
                }
            }
        }

        private void UpdateDownloadSpeed()
        {
            RecodeTime += Time.unscaledDeltaTime;
            if (RecodeTime >= 0.5f)
            {
                DownloadSpeed = (ulong)((double)(HotFixSize - RecodeHotFixSize) / (double)RecodeTime);
                RecodeHotFixSize = HotFixSize;
                RecodeTime = 0;
            }
        }
        private void SetProgress(float min, float max, float p)
        {
            CheckVersionProgress = (max - min) * p + min;
        }

        public void SetHotFixAssetsList()
        {
            if (VersionHelper.RemoteConfigAndDllList == null)
                VersionHelper.RemoteConfigAndDllList = new List<AssetsInfo>();
            VersionHelper.RemoteConfigAndDllList.Clear();

            if (VersionHelper.NeedDeleteCacheAaAssets == null)
                VersionHelper.NeedDeleteCacheAaAssets = new List<AssetsInfo>();
            VersionHelper.NeedDeleteCacheAaAssets.Clear();

            if (VersionHelper.mRemoteAaBundleList == null)
                VersionHelper.mRemoteAaBundleList = new List<string>();
            VersionHelper.mRemoteAaBundleList.Clear();

            var remoteItor = VersionHelper.mRemoteAssetsList.Contents.GetEnumerator();
            AssetsInfo remoteInfo;
            while (remoteItor.MoveNext())
            {
                remoteInfo = remoteItor.Current.Value;
                if (remoteInfo.AssetType == 1)//bundle
                {
                    if (remoteInfo.State == 0)
                    {
                        string[] tempArr = remoteInfo.AssetName.Split('/');
                        string hashName = tempArr[1];
                        VersionHelper.mRemoteAaBundleList.Add(string.Format("{0}.bundle", hashName));
                    }
                    else
                    {
                        VersionHelper.NeedDeleteCacheAaAssets.Add(remoteInfo);
                    }
                }
                else
                {
                    VersionHelper.RemoteConfigAndDllList.Add(remoteInfo);
                }
            }
        }

        IEnumerator CheckHotFixSeriesList()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            //检查手机剩余内存大小; 调用android层
            uint persistDirleftMemorySize = (uint)SDKManager.SDKGetLeftMemorySize();

            if (persistDirleftMemorySize < (uint)NeedLoadCatalogSize)//最小20M
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("left_space", string.Format("{0}KB", persistDirleftMemorySize));
                dict.Add("need_space", string.Format("{0}KB", NeedLoadCatalogSize));
                HitPointManager.HitPoint("game_update_space", dict);

                CheckVersionProgress = 1f;
                AppManager.eAppError = EAppError.MemoryNotEnoughError;
                yield break;
            }
#endif
            CheckVersionProgress = 0.1f;
            yield return DownloadRemoteAssetList();

            if (AppManager.eAppError != EAppError.None)
            {
                CheckVersionProgress = 1f;
                yield break;
            }


            if (VersionHelper.mRemoteAssetsList == null)
            {
                AppManager.eAppError = EAppError.RemoteAssetListError;
                CheckVersionProgress = 1f;
                yield break;
            }

            if (VersionHelper.mRemoteAssetsList.VersionIdentifier == null)
            {
                AppManager.eAppError = EAppError.AssetVerifyError;
                CheckVersionProgress = 1f;
                yield break;
            }

            if (string.IsNullOrEmpty(VersionHelper.ResourceHotFixUniqueIdentifier))
                DebugUtil.LogError("本地不存在HotFixUniqueIdentifier!");

            if (!string.Equals(VersionHelper.ResourceHotFixUniqueIdentifier, VersionHelper.mRemoteAssetsList.VersionIdentifier, StringComparison.Ordinal))
            {
                //不是同一系列文件，资源
                CheckVersionProgress = 1.0f;
                AppManager.eAppError = EAppError.AssetVerifyError;
                yield break;
            }
            NextHotFixPipeline = EHotFixPipeline.AddressablesCheckCatalog;
        }


        //IEnumerator DownloadHotFixUniqueIdentifier()
        //{
        //    DebugUtil.LogFormat(ELogType.eNone, "下载Remote资源列表 {0}", RemoteHotFixUniqueIdentifierUrl);
        //    using (UnityWebRequest req = UnityWebRequest.Get(RemoteHotFixUniqueIdentifierUrl))
        //    {
        //        req.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
        //        req.SetRequestHeader("Pragma", "no-cache");
        //        req.timeout = 5;
        //        yield return req.SendWebRequest();

        //        AppManager.ResponseCode = req.responseCode;
        //        if (!req.isDone || req.isNetworkError || req.isHttpError || req.responseCode != 200)
        //        {
        //            if (req.isNetworkError)
        //                AppManager.eAppError = EAppError.NetworkError;
        //            if (req.isHttpError)
        //                AppManager.eAppError = EAppError.HttpError;
        //            AppManager.ResponseCode = req.responseCode;
        //        }
        //        else
        //        {
        //            MemoryStream ms = new MemoryStream(req.downloadHandler.data);
        //            VersionHelper.ReadRemoteHotFixUniqueIdentifier(ms);

        //            //下载下来，但是序列化没有成功（即资源列表的内容有问题）
        //            //if (VersionHelper.RemoteHotFixUniqueIdentifier == null)
        //            //{
        //            //    AppManager.eAppError = EAppError.RemoteAssetListError;
        //            //}
        //        }
        //    }
        //}
        IEnumerator CheckAssetList()
        {
            System.Diagnostics.Stopwatch swR = new System.Diagnostics.Stopwatch();
            swR.Start();

            CheckVersionProgress = 0.5f;
            yield return LoadPersistentAssetList();
            yield return LoadCacheAssetList();
            yield return CompareAssetsList(0.5f, 1f);
            AddDeleteToExcess();

            swR.Stop();
            DebugUtil.LogFormat(ELogType.eNone, "对比获取热更新资源(config、dll)：{0}ms", swR.ElapsedMilliseconds);

            if (HotFixFileCount > 0 || ExcessAssets.Count > 0 || HotFixTotalSize > 0)
            {
                //if (ResourceManager.HttpErrorOnComplete == null)
                //{
                //    ResourceManager.HttpErrorOnComplete = HotFixHttpErrorHitPoint;
                //}

                //HitPointManager.HitPoint("game_hotfix_size");

#if UNITY_ANDROID && !UNITY_EDITOR
                //检查手机剩余内存大小; 调用android层
                uint NeedTotalLeftMemorySize = (uint)((HotFixTotalSize * HotFixSizeRate) / KiloByte);
                uint persistDirleftMemorySize = (uint)SDKManager.SDKGetLeftMemorySize();

                if (persistDirleftMemorySize <=  NeedTotalLeftMemorySize)
                {
                    AppManager.eAppError = EAppError.MemoryNotEnoughError;

                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("left_space", string.Format("{0}KB", persistDirleftMemorySize));
                    dict.Add("need_space", string.Format("{0}KB", NeedTotalLeftMemorySize));
                    HitPointManager.HitPoint("game_update_space", dict);
                }
                else
#endif
                {
                    if (HotFixTotalSize > HotFixNeedTipSize && NetworkReachability.ReachableViaCarrierDataNetwork == Application.internetReachability) //--先关闭手机数据
                    {
                        NextHotFixPipeline = EHotFixPipeline.WaitFixAssets;//手机>100M,需等玩家同意，待下载资源状态
                    }
                    else
                    {   //wifi直接下载资源
                        if (ExcessAssets.Count > 0)
                        {
                            NextHotFixPipeline = EHotFixPipeline.DestroyAssets;
                        }
                        else
                        {
                            //HitPointManager.HitPoint("game_hotfix_asset_start");
                            if (VersionHelper.mRemoteAaBundleList.Count > 0)
                            {
                                NextHotFixPipeline = EHotFixPipeline.AddressablesFixAssets;
                            }
                            else if (NewAssets.Count > 0)
                            {
                                NextHotFixPipeline = EHotFixPipeline.FixAssets;
                            }
                            else
                            {
                                NextHotFixPipeline = EHotFixPipeline.RecheckAssetList;
                            }
                        }
                    }
                }

                CheckVersionProgress = 1f;
                yield break;
            }

            //删除更新列表
            //string persistentAssetsListPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sHotFixListName);
            //if (File.Exists(persistentAssetsListPath))
            //{
            //    File.Delete(persistentAssetsListPath);
            //}

            //当资源版本号标记不一致 但又没有资源更新的时候 仅将资源版本号更新下
            string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
            VersionHelper.SetPersistentVersion(VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion);
            VersionHelper.WritePersistentVersion(persistentVersionPath);

            NextHotFixPipeline = EHotFixPipeline.Success;
            yield break;
        }
        IEnumerator ReCheckAssetList()
        {
            yield return CompareAssetsList();
            if (NewAssets.Count > 0)
            {
                AppManager.eAppError = EAppError.AssetVerifyError;
                yield break;
            }

            #region Aa资源的 资源校验检查
            //int curAssetVersion = 0;
            //int.TryParse(VersionHelper.PersistentAssetVersion, out curAssetVersion);
            //string persistentAssetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);
            //var remoteItor = VersionHelper.mRemoteAssetsList.Contents.GetEnumerator();
            //List<AssetsInfo> excedInfo = new List<AssetsInfo>();

            //AssetsInfo remoteInfo;
            //while (remoteItor.MoveNext())
            //{
            //    remoteInfo = remoteItor.Current.Value;

            //    if (remoteInfo.AssetType == 0 || remoteInfo.State == 1)
            //        continue;

            //    if (remoteInfo.Version <= curAssetVersion)
            //        continue;

            //    string bundlePath = string.Format("{0}/{1}/__data", Caching.defaultCache.path, remoteInfo.AssetName);
            //    if (File.Exists(bundlePath))
            //    {
            //        string md5 = FrameworkTool.GetFileMD5(bundlePath);

            //        if ((!string.IsNullOrEmpty(remoteInfo.AssetMD52) && !string.Equals(remoteInfo.AssetMD52, md5) && !string.Equals(remoteInfo.AssetMD5, md5))
            //            || (string.IsNullOrEmpty(remoteInfo.AssetMD52) && !string.Equals(remoteInfo.AssetMD5, md5)))
            //        {
            //            excedInfo.Add(remoteInfo);

            //            //删除BundleName/HashName
            //            string hashPath = string.Format("{0}/{1}", Caching.defaultCache.path, remoteInfo.AssetName);
            //            FileOperationHelpter.DeleteDirctory(hashPath);


            //            //上报md5校验失败
            //            string bundleName = remoteInfo.AssetName.Split('/')[1];
            //            string url = string.Format("{0}/{1}/{2}/{3}.bunlde", DownloadABUrl, AssetPath.sAddressableDir, AssetPath.sPlatformName, bundleName);
            //            Dictionary<string, string> dict = new Dictionary<string, string>();
            //            dict.Add("file_url", url);
            //            dict.Add("file_md5_correct", remoteInfo.AssetMD5);
            //            dict.Add("file_md5_error", md5);
            //            HitPointManager.HitPoint("game_update_md5error", dict);

            //            DebugUtil.LogErrorFormat("校验失败：{0}，CorrectMD5:{1}  ErrorMD5:{2}", bundlePath, remoteInfo.AssetMD5, md5);


            //            //校验失败的特殊处理,直接标注清除标记
            //            if (RetryDownloadCountDict.ContainsKey(remoteInfo.AssetName))
            //            {
            //                RetryDownloadCountDict[remoteInfo.AssetName]++;
            //            }
            //            else
            //            {
            //                RetryDownloadCountDict.Add(remoteInfo.AssetName, 1);
            //            }

            //            if (RetryDownloadCountDict[remoteInfo.AssetName] >= RetryCount)
            //            {
            //                AppManager.eAppError = EAppError.RemoteAssetCompleteFailure;
            //                DebugUtil.LogErrorFormat("校验尝试失败{0}次，{1}", RetryCount, url);
            //                yield break;
            //            }
            //        }
            //        else
            //        {
            //            VersionHelper.AppendAssetInfo(persistentAssetsListPath, remoteInfo);
            //            if (RetryDownloadCountDict.ContainsKey(remoteInfo.AssetName))
            //            {
            //                RetryDownloadCountDict.Remove(remoteInfo.AssetName);
            //            }
            //        }
            //    }
            //}

            //if (excedInfo.Count > 0)
            //{
            //    AppManager.eAppError = EAppError.AssetVerifyError;
            //    yield break;
            //}


            #endregion



            //删除更新列表
            //         string persistentAssetsListPath = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sHotFixListName);
            //         if (File.Exists(persistentAssetsListPath))
            //         {
            //             File.Delete(persistentAssetsListPath);
            //         }
            //
            string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
            VersionHelper.SetPersistentVersion(VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion);
            VersionHelper.WritePersistentVersion(persistentVersionPath);

            NextHotFixPipeline = EHotFixPipeline.Success;
            //HitPointManager.HitPoint("game_hotfix_asset_success");

        }

        IEnumerator LoadPersistentAssetList()
        {
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);
            if (File.Exists(assetsListPath))
            {
                assetsListPath = string.Format("{0}{1}", AssetPath.sPersistentPrefix, assetsListPath);
                DebugUtil.LogFormat(ELogType.eNone, "加载Persistent资源列表 {0}", assetsListPath);

                using (UnityWebRequest request = UnityWebRequest.Get(new System.Uri(assetsListPath)))
                {
                    yield return request.SendWebRequest();
                    AppManager.ResponseCode = request.responseCode;
                    if (!request.isDone || request.isNetworkError || request.isHttpError)// || request.responseCode != 200)
                    {
                        Debug.LogFormat("加载Persistent资源列表失败 {0}", request.error);
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(request.downloadHandler.data);
                        VersionHelper.SetPersistentAssetsList(ms);
                    }
                }
            }
        }
        IEnumerator LoadCacheAssetList()
        {
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);
            if (File.Exists(assetsListPath))
            {
                assetsListPath = string.Format("{0}{1}", AssetPath.sPersistentPrefix, assetsListPath);
                Debug.LogFormat("加载Persistent资源列表 {0}", assetsListPath);
                using (UnityWebRequest request = UnityWebRequest.Get(new System.Uri(assetsListPath)))
                {
                    yield return request.SendWebRequest();
                    AppManager.ResponseCode = request.responseCode;
                    if (!request.isDone || request.isNetworkError || request.isHttpError)// || request.responseCode != 200)
                    {
                        Debug.LogFormat("加载Persistent资源列表失败 {0}", request.error);
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(request.downloadHandler.data);
                        mCacheAssetList = AssetList.Deserialize(ms);
                        ms.Close();
                        ms.Dispose();
                    }
                }
            }
        }
        IEnumerator DownloadRemoteAssetList()
        {
            //第一次尝试下载：
            DebugUtil.LogFormat(ELogType.eNone, "下载Remote资源列表 {0}", RemoteHotFixAssetsListUrl);
            string url = string.Format("{0}?ts={1}", RemoteHotFixAssetsListUrl, TimeManager.ClientNowMillisecond());

            string filePath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixTempListName);
            if (File.Exists(filePath))
                File.Delete(filePath);

            //HitPointManager.HitPoint("game_hotfix_assetlist_start");

            UnityWebRequest HttpReq = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            HttpReq.downloadHandler = new DownloadHandlerFile(filePath);
            //HttpReq.timeout = 20;
            yield return HttpReq.SendWebRequest();

            if (HttpReq.isNetworkError || HttpReq.isHttpError)
            {
                HttpReq.Dispose();
                yield return StartOneOpRemoteList();
            }
            else
            {
                HttpReq.Dispose();
                /* if (VersionHelper.hotfixListMD5 != FrameworkTool.GetFileMD5(filePath))
                 {
                     AppManager.eAppError = EAppError.HotFixAssetListMD5Error;
                 }
                 else*/
                {
                    //HitPointManager.HitPoint("game_hotfix_assetlist_success");

                    CheckVersionProgress = 0.3f;
                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    VersionHelper.SetRemoteAssetsList(fileStream);
                    fileStream.Close();
                    fileStream.Dispose();

                    SetHotFixAssetsList();

                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }
        }


        IEnumerator StartOneOpRemoteList()
        {
            ++RecordDownloadCount;
            int m_TimeoutCount = 0;
            int m_tempCount = RecordDownloadCount / RetryCount;
            //这里需要好好考虑下怎么写
            //if (ResourceManager.HotFixUrlArr.Length == 2)
            //{
            //    if (RecordDownloadCount == 3)
            //        m_TimeoutCount = 0;
            //    else if (RecordDownloadCount > 3)
            //        m_TimeoutCount = 1;
            //}

            string url = string.Format("{0}/{1}/{2}.{3}/{4}?ts={5}", VersionHelper.HotFixUrlArr[m_TimeoutCount],
                   AssetPath.sPlatformName,
                   VersionHelper.RemoteBuildVersion,
                   VersionHelper.RemoteAssetVersion,
                   AssetPath.sHotFixListName,
                   TimeManager.ClientNowMillisecond());

            DebugUtil.LogFormat(ELogType.eNone, string.Format("尝试下载HotFixList：retry={0},{1}", RecordDownloadCount, url));

            string filePath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixTempListName);
            if (File.Exists(filePath))
                File.Delete(filePath);

            UnityWebRequest HttpReq = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            HttpReq.downloadHandler = new DownloadHandlerFile(filePath);
            //HttpReq.timeout = 20;
            yield return HttpReq.SendWebRequest();

            if (HttpReq.isNetworkError || HttpReq.isHttpError)
            {
                //if (RecordDownloadCount < RetryCount || (RecordDownloadCount / RetryCount < ResourceManager.HotFixUrlArr.Length))
                //{
                //    HttpReq.downloadHandler.Dispose();
                //    HttpReq.Dispose();
                //    yield return StartOneOpRemoteList();
                //}
                //else
                //{
                //    if (HttpReq.isNetworkError) AppManager.eAppError = EAppError.NetworkError;
                //    if (HttpReq.isHttpError) AppManager.eAppError = EAppError.HttpError;
                //    AppManager.ResponseCode = HttpReq.responseCode;
                //    HttpReq.downloadHandler.Dispose();
                //    HttpReq.Dispose();
                //}
            }
            else
            {
                HttpReq.Dispose();
                /* if (VersionHelper.hotfixListMD5 != FrameworkTool.GetFileMD5(filePath))
                 {
                     AppManager.eAppError = EAppError.HotFixAssetListMD5Error;
                 }
                 else*/
                {
                    CheckVersionProgress = 0.3f;

                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    VersionHelper.SetRemoteAssetsList(fileStream);
                    fileStream.Close();
                    fileStream.Dispose();

                    SetHotFixAssetsList();

                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }
        }





        IEnumerator CompareAssetsList(float startProgress = 0f, float endProgress = 1f)
        {
            DebugUtil.LogFormat(ELogType.eNone, "对比资源列表");
            float halfProgress = (endProgress - startProgress) / 2f + startProgress;

            //HotFixSize = 0L;
            //HotFixTotalSize = 0L;
            HotFixFileCount = 0;

            NewAssets.Clear();
            ExcessAssets.Clear();

            AssetList newCacheList = new AssetList();

            AssetsInfo remoteInfo = null;
            AssetsInfo localInfo = null;
            int index = 0;
            int count = 0;

            index = 0;
            count = VersionHelper.RemoteConfigAndDllList.Count;
            var remoteItor = VersionHelper.RemoteConfigAndDllList.GetEnumerator();//VersionHelper.mRemoteAssetsList.Contents.GetEnumerator();

            int curAssetVersion = 0;
            int.TryParse(VersionHelper.PersistentAssetVersion, out curAssetVersion);

            while (remoteItor.MoveNext())
            {
                remoteInfo = remoteItor.Current;//remoteItor.Current.Value;
                if (remoteInfo.Version <= curAssetVersion)
                {
                    continue;
                }
                if (remoteInfo.AssetType == 1)
                {
                    continue;
                }

                if (VersionHelper.mPersistentAssetsList == null //没缓存 (本地记录已经下载的HotFixList.txt)
                    || !VersionHelper.mPersistentAssetsList.Contents.TryGetValue(remoteInfo.AssetName, out localInfo) //缓存了AssetsList ,新增的资源
                    || !remoteInfo.AssetMD5.Equals(localInfo.AssetMD5) // 缓存了AssetsList, 本地资源被修改
                    || remoteInfo.State != localInfo.State   // 缓存了AssetsList, 状态不一样
                    || remoteInfo.Version != localInfo.Version) // 缓存了AssetsList, 版本不一致
                {
                    //0 = 修改或者新增 1 = 删除
                    if (remoteInfo.State == 0)
                    {
                        AssetsInfo cacheInfo = null;
                        if (mCacheAssetList == null
                            || !mCacheAssetList.Contents.TryGetValue(remoteInfo.AssetName, out cacheInfo)
                            || cacheInfo.AssetMD5 != remoteInfo.AssetMD5
                            || cacheInfo.Version != remoteInfo.Version)
                        {
                            NewAssets.Add(remoteInfo);
                            //计算更新（配置文件，dll文件）资源大小
                            HotFixTotalSize += remoteInfo.Size;
                            CSVAndDllTotalSize += remoteInfo.Size;
                            ++HotFixFileCount;
                        }
                        else
                        {
                            //下载到一半的文件                
                            NewAssets.Insert(0, remoteInfo);
                            newCacheList.Contents.Add(cacheInfo.AssetName, cacheInfo);
                            //下载到一半的文件 计算剩余需要下载的资源大小
                            string path = cacheInfo.AssetName.Replace("\\", "/");
                            string filePath = AssetPath.GetPersistentFullPath(path);
                            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                            ulong needLoadcacheLength = remoteInfo.Size - (ulong)fileStream.Length;
                            fileStream.Close();
                            fileStream.Dispose();

                            HotFixTotalSize += needLoadcacheLength;
                            CSVAndDllTotalSize += needLoadcacheLength;
                            ++HotFixFileCount;
                        }
                    }
                    else
                    {
                        ExcessAssets.Enqueue(remoteInfo);
                    }
                }

                ++index;
                if (index % 80 == 0 || index == count)
                {
                    SetProgress(startProgress, halfProgress, (float)index / (float)count);
                    yield return null;
                }
            }

            if (mCacheAssetList != null)
            {
                mCacheAssetList.Contents.Clear();
            }

            mCacheAssetList = newCacheList;
            //将新的Cache存到本地
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);
            VersionHelper.WriteAssetList(assetsListPath, mCacheAssetList);

            DebugUtil.LogFormat(ELogType.eNone, "需要更新的资源(配置，dll) {0}", HotFixFileCount);
            DebugUtil.LogFormat(ELogType.eNone, "需要删除的资源(配置，dll) {0}", ExcessAssets.Count);
            DebugUtil.LogFormat(ELogType.eNone, "已经有部分下载的资源(配置，dll) {0}", mCacheAssetList.Contents.Count);
        }
        IEnumerator DestroyAssets()
        {
            float i = 0;
            float count = (float)ExcessAssets.Count;
            int num = 0;
            int numMax = ExcessAssets.Count / 30;
            numMax = numMax < 10 ? 10 : numMax;
           

            while (ExcessAssets.Count > 0)
            {
                SetProgress(0f, 1f, i / count);
                i = i + 1;

                AssetsInfo info = ExcessAssets.Dequeue();

                if (info.AssetType == 0)
                {
                    //删除缓存不使用的 config/dll
                    string fileNamePath = AssetPath.GetPersistentAssetFullPath(info.AssetName);
                    if (File.Exists(fileNamePath))
                    {
                        File.Delete(fileNamePath);
                    }

                    VersionHelper.AppendAssetInfoToPersistent(info);
                }
                else
                {
                    //删除缓存不使用的bundle
                    string hashPathDir = string.Format("{0}/{1}", Caching.defaultCache.path, info.AssetName);
                    FileOperationHelpter.DeleteDirctory(hashPathDir);
                }

                ++num;
                if (num > numMax)
                {
                    num = 0;
                    yield return null;
                }
            }


            if (VersionHelper.mRemoteAaBundleList.Count > 0)
            {
                NextHotFixPipeline = EHotFixPipeline.AddressablesFixAssets;
            }
            else
            {
                NextHotFixPipeline = EHotFixPipeline.FixAssets;
            }
        }
        IEnumerator DownloadAssets()
        {
            System.Diagnostics.Stopwatch swR = new System.Diagnostics.Stopwatch();
            swR.Start();
            string cacheAssetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);
            string persistentAssetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);
            LoadedHotFixFileCount = 0;

            while (NewAssets.Count > 0)
            {
                if (NextHotFixPipeline == EHotFixPipeline.WaitFail)
                    break;
                yield return null;

                AssetsInfo assetsInfo = NewAssets[0];
                NewAssets.RemoveAt(0);

                string path = assetsInfo.AssetName.Replace("\\", "/");
                string filePath = AssetPath.GetPersistentFullPath(path);
                string dir = Path.GetDirectoryName(filePath);

                //如果不是缓存则先删除
                if (mCacheAssetList == null || !mCacheAssetList.Contents.ContainsKey(assetsInfo.AssetName))
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                //校验失败的 需要删除再次下载
                if (RetryDownloadCountDict.ContainsKey(assetsInfo.AssetName))
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                //获取下载文件长度  大于1M放入缓存
                long totalLength = (long)assetsInfo.Size;
                if ((ulong)totalLength >= CacheFileMinSize)
                {
                    if (mCacheAssetList == null)
                        mCacheAssetList = new AssetList();
                    mCacheAssetList.Contents[assetsInfo.AssetName] = assetsInfo;
                    VersionHelper.WriteAssetList(cacheAssetsListPath, mCacheAssetList);
                }

                //new ConfigAssetProvider().Start(assetsInfo);
            }

            while (LoadedHotFixFileCount < HotFixFileCount && AppManager.eAppError == EAppError.None)
            {
                yield return null;
            }

            if (AppManager.eAppError != EAppError.None)
            {
                yield break;
            }

            #region 单个资源下载方式

            /*  while (NewAssets.Count > 0)
              {
                  if (NextHotFixPipeline == EHotFixPipeline.WaitFail)
                      break;

                  AssetsInfo assetsInfo = NewAssets[0];
                  NewAssets.RemoveAt(0);

                  string path = assetsInfo.AssetName.Replace("\\", "/");
                  //url后面追加的时间随机数，只是用来避免缓存旧版本同名文件
                  string url = string.Format("{0}/{1}", DownloadABUrl, path);
                  string filePath = AssetPath.GetPersistentFullPath(path);
                  string dir = Path.GetDirectoryName(filePath);

                  DebugUtil.LogFormat(ELogType.eNone, "尝试下载资源{0} 到 {1}", url, filePath);

                  //如果不是缓存则先删除
                  if (mCacheAssetList == null || !mCacheAssetList.Contents.ContainsKey(assetsInfo.AssetName))
                  {
                      if (File.Exists(filePath))
                      {
                          File.Delete(filePath);
                      }
                  }

                  if (!Directory.Exists(dir))
                  {
                      Directory.CreateDirectory(dir);
                  }

                  //校验失败的 需要删除再次下载
                  if (RetryDownloadCountDict.ContainsKey(assetsInfo.AssetName))
                  {
                      if (File.Exists(filePath))
                      {
                          File.Delete(filePath);
                      }
                  }

                  //获取下载文件长度 
                  long totalLength = (long)assetsInfo.Size;
                  //大于1M放入缓存
                  if ((ulong)totalLength >= CacheFileMinSize)
                  {
                      if (mCacheAssetList == null)
                          mCacheAssetList = new AssetList();
                      mCacheAssetList.Contents[assetsInfo.AssetName] = assetsInfo;
                      VersionHelper.WriteAssetList(cacheAssetsListPath, mCacheAssetList);
                  }

                  yield return BeginOperationConfig(assetsInfo);

                  if (AppManager.eAppError != EAppError.None)
                  {
                      yield break;
                  }

#region 老的热更新资源下载
                  //HttpReq = UnityWebRequest.Head(url);
                  //yield return HttpReq.SendWebRequest();
                  //if (HttpReq.isNetworkError || HttpReq.isHttpError || HttpReq.responseCode >= 400)
                  //{
                  //    if (HttpReq.isNetworkError)
                  //        AppManager.eAppError = EAppError.NetworkError;
                  //    if (HttpReq.isHttpError)
                  //        AppManager.eAppError = EAppError.HttpError;
                  //    AppManager.ResponseCode = HttpReq.responseCode;
                  //    Debug.LogErrorFormat("ResponseCode = {0}", HttpReq.responseCode);
                  //    HttpReq.Dispose();
                  //    yield break;
                  //}

                  //long totalLength = 0L;
                  //long.TryParse(HttpReq.GetResponseHeader("Content-Length"), out totalLength);
                  //HttpReq.Dispose();
                  ////获取已经缓存的长度
                  //FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                  //long cacheLength = fileStream.Length;

                  ////已经缓存完成的直接标记完成
                  //if (cacheLength < totalLength)
                  //{
                  //    HotFixSize += (ulong)cacheLength;

                  //    fileStream.Seek(cacheLength, SeekOrigin.Begin);
                  //    //获取文件内容
                  //    HttpReq = UnityWebRequest.Get(url);
                  //    HttpReq.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
                  //    HttpReq.SetRequestHeader("Pragma", "no-cache");
                  //    HttpReq.SetRequestHeader("Range", "bytes=" + cacheLength + "-" + totalLength);
                  //    //HttpReq.timeout = 5;

                  //    HttpReq.SendWebRequest();

                  //    if ((ulong)totalLength >= CacheFileMinSize)
                  //    {
                  //        if (mCacheAssetList == null)
                  //        {
                  //            mCacheAssetList = new AssetList();
                  //        }
                  //        mCacheAssetList.Contents[assetsInfo.AssetName] = assetsInfo;
                  //        VersionHelper.WriteAssetList(cacheAssetsListPath, mCacheAssetList);
                  //    }

                  //    if (HttpReq.isNetworkError || HttpReq.isHttpError || HttpReq.responseCode >= 400)
                  //    {
                  //        if (HttpReq.isNetworkError)
                  //            AppManager.eAppError = EAppError.NetworkError;
                  //        if (HttpReq.isHttpError)
                  //            AppManager.eAppError = EAppError.HttpError;
                  //        AppManager.ResponseCode = HttpReq.responseCode;
                  //        Debug.LogErrorFormat("ResponseCode = {0}", HttpReq.responseCode);
                  //        fileStream.Close();
                  //        fileStream.Dispose();
                  //        HttpReq.Dispose();
                  //        HttpReq = null;
                  //        yield break;
                  //    }


                  //    int readIndex = 0;
                  //    int readLength = 0;
                  //    do
                  //    {
                  //        yield return null;
                  //        byte[] buff = HttpReq.downloadHandler.data;
                  //        readLength = buff.Length - readIndex;
                  //        if (readLength > 0)
                  //        {
                  //            fileStream.Write(buff, readIndex, readLength);

                  //            readIndex += readLength;
                  //            cacheLength += readLength;

                  //            DebugUtil.LogFormat(ELogType.eNone, "下载资源大小{0} 已缓存{1}", totalLength, cacheLength);

                  //            HotFixSize += (ulong)readLength;
                  //        }
                  //        else
                  //        {
                  //            if (HttpReq.isNetworkError || HttpReq.isHttpError || HttpReq.responseCode >= 400)
                  //            {
                  //                if (HttpReq.isNetworkError)
                  //                    AppManager.eAppError = EAppError.NetworkError;
                  //                if (HttpReq.isHttpError)
                  //                    AppManager.eAppError = EAppError.HttpError;

                  //                AppManager.ResponseCode = HttpReq.responseCode;
                  //                Debug.LogErrorFormat("ResponseCode = {0}", HttpReq.responseCode);
                  //                fileStream.Close();
                  //                fileStream.Dispose();
                  //                HttpReq.Dispose();
                  //                HttpReq = null;
                  //                yield break;
                  //            }
                  //        }
                  //    } while (!HttpReq.isDone || cacheLength < totalLength);

                  //}

                  //fileStream.Close();
                  //fileStream.Dispose();
#endregion

                  string md5 = FrameworkTool.GetFileMD5(filePath);

                  if (string.Equals(assetsInfo.AssetMD5, md5))
                  {
                      VersionHelper.AppendAssetInfo(persistentAssetsListPath, assetsInfo);

                      //假如缓存里面有正在下载的  那么在下载完成后要删除缓存里正在下载的记录
                      if (mCacheAssetList != null && mCacheAssetList.Contents != null && mCacheAssetList.Contents.ContainsKey(assetsInfo.AssetName))
                      {
                          mCacheAssetList.Contents.Remove(assetsInfo.AssetName);
                          VersionHelper.WriteAssetList(cacheAssetsListPath, mCacheAssetList);
                      }


                      if (RetryDownloadCountDict.ContainsKey(assetsInfo.AssetName))
                      {
                          RetryDownloadCountDict.Remove(assetsInfo.AssetName);
                      }
                  }
                  else
                  {
                      Dictionary<string, string> dict = new Dictionary<string, string>();
                      dict.Add("file_url", url);
                      dict.Add("file_md5_correct", assetsInfo.AssetMD5);
                      dict.Add("file_md5_error", md5);
                      HitPointManager.HitPoint("game_update_md5error", dict);

                      Debug.LogErrorFormat("MD5 验证失败 {0} {1}/{2}", assetsInfo.AssetName, assetsInfo.AssetMD5, md5);

                      //校验失败的特殊处理,直接标注清除标记
                      if (RetryDownloadCountDict.ContainsKey(assetsInfo.AssetName))
                      {
                          RetryDownloadCountDict[assetsInfo.AssetName]++;
                      }
                      else
                      {
                          RetryDownloadCountDict.Add(assetsInfo.AssetName, 1);
                      }
                  }
              }*/
            #endregion


            swR.Stop();
            Debug.LogFormat("Get DownloadAssets Need Time Total:{0}ms", swR.ElapsedMilliseconds);

            File.Delete(cacheAssetsListPath);
            NextHotFixPipeline = EHotFixPipeline.RecheckAssetList;
        }


        public IEnumerator CheckAssetListAndDelete()
        {
            yield return LoadPersistentAssetList();

            ExcessAssets.Clear();

            DebugUtil.LogFormat(ELogType.eNone, "对比资源列表");
            //int count = VersionHelper.mPersistentAssetsList.Contents.Count;
            var persistItor = VersionHelper.mPersistentAssetsList.Contents.GetEnumerator();
            AssetsInfo perisistInfo;

            while (persistItor.MoveNext())
            {
                perisistInfo = persistItor.Current.Value;

                if (perisistInfo.State == 1)
                    continue;

                if (perisistInfo.AssetType == 1)
                {
                    string bundlePath = string.Format("{0}/{1}/__data", Caching.defaultCache.path, perisistInfo.AssetName);
                    if (File.Exists(bundlePath))
                    {
                        string md5 = FrameworkTool.GetFileMD5(bundlePath);
                        if (!string.Equals(perisistInfo.AssetMD5, md5, StringComparison.Ordinal))
                        {
                            ExcessAssets.Enqueue(perisistInfo);
                            string hashPath = string.Format("{0}/{1}", Caching.defaultCache.path, perisistInfo.AssetName);
                            FileOperationHelpter.DeleteDirctory(hashPath);
                            DebugUtil.LogFormat(ELogType.eNone, string.Format("资源md5出错，删除：{0}", hashPath));
                        }
                    }
                }
                else
                {
                    string path = perisistInfo.AssetName.Replace("\\", "/");
                    string filePath = AssetPath.GetPersistentFullPath(path);
                    if (File.Exists(filePath))
                    {
                        string md5 = FrameworkTool.GetFileMD5(filePath);
                        if (!string.Equals(perisistInfo.AssetMD5, md5, StringComparison.Ordinal))
                        {
                            ExcessAssets.Enqueue(perisistInfo);
                            File.Delete(filePath);
                            DebugUtil.LogFormat(ELogType.eNone, string.Format("资源md5出错，删除：{0}", filePath));
                        }
                    }
                }
            }

            if (ExcessAssets.Count > 0)
            {
                //删除HotFixList种的记录
                foreach (var item in ExcessAssets)
                {
                    VersionHelper.mPersistentAssetsList.Contents.Remove(item.AssetName);
                }

                string HotFixListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);
                if (File.Exists(HotFixListPath))
                    File.Delete(HotFixListPath);
                VersionHelper.WriteAssetList(HotFixListPath, VersionHelper.mPersistentAssetsList);

                string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
                VersionHelper.SetPersistentVersion(VersionHelper.StreamingBuildVersion, VersionHelper.StreamingAssetVersion);
                VersionHelper.WritePersistentVersion(persistentVersionPath);
            }

            if (CheckMD5AssetList != null)
            {
                CheckMD5AssetList.Invoke();
            }
        }


        public void DeleteErrorAssetList()
        {
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);

            while (ExcessAssets.Count > 0)
            {
                AssetsInfo info = ExcessAssets.Dequeue();

                if (info.AssetType == 1)
                {
                    string bundlePath = string.Format("{0}/{1}/__data", Caching.defaultCache.path, info.AssetName);
                    if (File.Exists(bundlePath))
                    {
                        //删除该标签名下 缓存的所有bundle
                        string laybelPath = info.AssetName.Split('/')[0];
                        laybelPath = string.Format("{0}/{1}", Caching.defaultCache.path, laybelPath);
                        FileOperationHelpter.DeleteFiles(laybelPath);
                    }
                }
                else
                {
                    string path = info.AssetName.Replace("\\", "/");
                    string filePath = AssetPath.GetPersistentFullPath(path);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        VersionHelper.mPersistentAssetsList.Contents.Remove(info.AssetName);
                        VersionHelper.WriteAssetList(assetsListPath, VersionHelper.mPersistentAssetsList);
                    }
                }
            }

            VersionHelper.Clear();
        }






        /* IEnumerator BeginOperationConfig(AssetsInfo assetsInfo)
         {
             string path = assetsInfo.AssetName.Replace("\\", "/");
             string url = string.Format("{0}/{1}", DownloadABUrl, path);
             string filePath = AssetPath.GetPersistentFullPath(path);
             string tsUrl = string.Format("{0}?ts={1}", url, TimeManager.ClientNowMillisecond());

             long totalLength = (long)assetsInfo.Size;
             bool isRetry = false;
             int retryCount = 0;
             int index = 0;

             while (isRetry || retryCount <= 0)
             {
                 //如果尝试下载的次数超过3次，主cdn域名下载并重试失败后自动使用备用域名下载
                 tsUrl = string.Format("{0}/{1}/{2}.{3}/{4}?ts={5}", VersionHelper.HotFixUrlArr[index], AssetPath.sPlatformName, VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion, path, TimeManager.ClientNowMillisecond());

 #region 根据文件头获取文件总长度 -- 使用 AssetsInfo 
                 //UnityWebRequest headRequest = UnityWebRequest.Head(url);
                 //yield return headRequest.SendWebRequest();
                 //if (headRequest.isNetworkError || headRequest.isHttpError || headRequest.responseCode >= 400)
                 //{
                 //    if (headRequest.isNetworkError)
                 //        AppManager.eAppError = EAppError.NetworkError;
                 //    if (headRequest.isHttpError)
                 //        AppManager.eAppError = EAppError.HttpError;
                 //    AppManager.ResponseCode = headRequest.responseCode;
                 //    Debug.LogErrorFormat("ResponseCode = {0}", headRequest.responseCode);
                 //    headRequest.Dispose();
                 //    yield break;
                 //}

                 //long totalLength = 0L;
                 //long.TryParse(headRequest.GetResponseHeader("Content-Length"), out totalLength);
                 //headRequest.Dispose();
 #endregion

                 //append设置为true文件写入方式为接续写入，不覆盖原文件。
                 UnityWebRequest HttpReq = new UnityWebRequest(tsUrl, UnityWebRequest.kHttpVerbGET);
                 HttpReq.downloadHandler = new DownloadHandlerFile(filePath, true);
                 HttpReq.timeout = 10;

                 //获取已经缓存的长度
                 FileInfo file = new FileInfo(filePath);
                 long cacheLength = file.Length;

                 //设置文件从什么位置开始下载和写入
                 HttpReq.SetRequestHeader("Range", "bytes=" + cacheLength + "-");

                 retryCount++;

                 //已经缓存完成的直接标记完成
                 if (cacheLength < totalLength)
                 {

                     HttpReq.SendWebRequest();

 #region 优化
                     //int readIndex = 0;
                     //int readLength = 0;
                     //bool httpError = false;
                     //do
                     //{
                     //    yield return null;
                     //    readLength = (int)HttpReq.downloadedBytes - readIndex;
                     //    if (readLength > 0)
                     //    {
                     //        //fileStream.Write(buff, readIndex, readLength);

                     //        readIndex += readLength;
                     //        cacheLength += readLength;

                     //        DebugUtil.LogFormat(ELogType.eNone, "下载资源大小{0} 已缓存{1}", totalLength, cacheLength);

                     //        HotFixSize += (ulong)readLength;
                     //    }
                     //    else
                     //    {
                     //        if (!string.IsNullOrEmpty(HttpReq.error))
                     //        {
                     //            #region 错误类型上报
                     //            //设置超时,会返回且isNetworkError为true
                     //            if (HttpReq.responseCode == 408 || HttpReq.error.Equals("Request timeout"))//XXXX这个貌似不准
                     //            {
                     //                Dictionary<string, string> dict = new Dictionary<string, string>();
                     //                dict.Add("file_url", url);
                     //                dict.Add("wait_time", HttpReq.timeout.ToString());
                     //                dict.Add("speed", DownloadSpeed.ToString()); //DownloadSpeed 不准
                     //                HitPointManager.HitPoint("game_update_httptimeout", dict);
                     //            }
                     //            else if (HttpReq.responseCode == 404)//文件丢失
                     //            {
                     //                Dictionary<string, string> dict = new Dictionary<string, string>();
                     //                dict.Add("file_url", url);
                     //                HitPointManager.HitPoint("game_update_missing", dict);
                     //            }
                     //            else//其他原因
                     //            {
                     //                Dictionary<string, string> dict = new Dictionary<string, string>();
                     //                dict.Add("file_url", url);
                     //                dict.Add("reason", string.Format("{0}:{1}", HttpReq.responseCode, HttpReq.error));
                     //                HitPointManager.HitPoint("game_update_othercase", dict);
                     //            }

                     //            Debug.LogErrorFormat("UnityWebRequest Fail= {0} EAppError = {1} ,ResponseCode = {2}, {3}", url, AppManager.eAppError, HttpReq.responseCode, HttpReq.error);
                     //            #endregion


                     //            #region 尝试次数

                     //            if (retryCount >= ReallyRetryCount)
                     //            {
                     //                isRetry = false;
                     //                AppManager.eAppError = EAppError.RemoteAssetCompleteFailure;
                     //            }
                     //            else if (retryCount >= RetryCount)
                     //            {
                     //                if (!string.IsNullOrEmpty(DownloadABUrlBackUp))
                     //                {
                     //                    isRetry = true;
                     //                }
                     //                else
                     //                {
                     //                    isRetry = false;
                     //                    if (HttpReq.isNetworkError)
                     //                        AppManager.eAppError = EAppError.NetworkError;
                     //                    if (HttpReq.isHttpError)
                     //                        AppManager.eAppError = EAppError.HttpError;
                     //                    AppManager.ResponseCode = HttpReq.responseCode;
                     //                    Debug.LogErrorFormat("BeginOperationConfig failed :{0}", HttpReq.error);
                     //                }
                     //            }
                     //            else
                     //            {
                     //                isRetry = true;
                     //            }

                     //            #endregion

                     //            httpError = true;
                     //        }
                     //    }
                     //} while (!HttpReq.isDone || (cacheLength < totalLength && !httpError));
 #endregion

                     while (!HttpReq.isDone)
                     {
                         yield return null;
                     }


                     if (!string.IsNullOrEmpty(HttpReq.error))
                     {
 #region 错误类型上报
                         //设置超时,会返回且isNetworkError为true
                         if (HttpReq.responseCode == 408 || HttpReq.error.Equals("Request timeout"))//XXXX这个貌似不准
                         {
                             Dictionary<string, string> dict = new Dictionary<string, string>();
                             dict.Add("file_url", url);
                             dict.Add("wait_time", HttpReq.timeout.ToString());
                             dict.Add("speed", DownloadSpeed.ToString()); //DownloadSpeed 不准
                             HitPointManager.HitPoint("game_update_httptimeout", dict);
                         }
                         else if (HttpReq.responseCode == 404)//文件丢失
                         {
                             Dictionary<string, string> dict = new Dictionary<string, string>();
                             dict.Add("file_url", url);
                             HitPointManager.HitPoint("game_update_missing", dict);
                         }
                         else//其他原因
                         {
                             Dictionary<string, string> dict = new Dictionary<string, string>();
                             dict.Add("file_url", url);
                             dict.Add("reason", string.Format("{0}:{1}", HttpReq.responseCode, HttpReq.error));
                             HitPointManager.HitPoint("game_update_othercase", dict);
                         }

                         Debug.LogErrorFormat("UnityWebRequest Fail= {0} EAppError = {1} ,ResponseCode = {2}, {3}", url, AppManager.eAppError, HttpReq.responseCode, HttpReq.error);
 #endregion

 #region 尝试次数

                         int count = retryCount % RetryCount;
                         if (count >= VersionHelper.HotFixUrlArr.Length)
                         {
                             isRetry = false;
                             if (HttpReq.isNetworkError)
                                 AppManager.eAppError = EAppError.NetworkError;
                             if (HttpReq.isHttpError)
                                 AppManager.eAppError = EAppError.HttpError;
                             AppManager.ResponseCode = HttpReq.responseCode;
                             DebugUtil.LogFormat(ELogType.eHotFix, "BeginOperationConfig failed :{0}", HttpReq.error);
                         }
                         else
                         {
                             index = count;
                             isRetry = true;
                         }
 #endregion
                     }
                     else
                     {
                         HotFixSize += (ulong)HttpReq.downloadedBytes;
                         DebugUtil.LogFormat(ELogType.eHotFix, "下载资源保存到路径{0} 下载文件长度{1}", path, totalLength);
                     }
                 }

                 HttpReq.Dispose();
             }
         }*/



        #region Addressables热更系统
        //用于记录keys -- 这样不知道会不会占用内存，毕竟内存也不小
        private void InitHotFixData()
        {
            HotFixSize = 0L;
            HotFixTotalSize = 0L;
            AssetBundleTotalSize = 0L;
            CSVAndDllTotalSize = 0L;

        }
        private void AddDeleteToExcess()
        {
            //只能在这里 把需要删除的aa加进来
            if (VersionHelper.NeedDeleteCacheAaAssets != null)
            {
                foreach (var item in VersionHelper.NeedDeleteCacheAaAssets)
                {
                    ExcessAssets.Enqueue(item);
                }
            }
        }
        IEnumerator AddressablesCheckCatalog()
        {
            InitHotFixData();
            //1.初始化
            var init = Addressables.InitializeAsync();
            yield return init;

            //2.检查目录更新
            AsyncOperationHandle<List<string>> checkhandle = Addressables.CheckForCatalogUpdates(false);
            yield return checkhandle;

            if (checkhandle.IsDone && checkhandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (checkhandle.Result.Count > 0)
                {
                    //3.更新目录 每次都更新
                    var updateHandle = Addressables.UpdateCatalogs(checkhandle.Result, false);
                    yield return updateHandle;
                    if (updateHandle.IsDone && updateHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        CheckVersionProgress = 0.4f;
                        AddressablesUtil.Release(ref updateHandle, null);
                        yield return AddressablesCheckDownloadAssetsSize();
                    }
                    else
                    {
                        AddressablesUtil.Release(ref updateHandle, null);
                        AppManager.eAppError = EAppError.HttpError;
                        DebugUtil.LogError("Addressable UpdateCatalogs failed !!!");
                    }
                }
                else
                {
                    CheckVersionProgress = 0.4f;
                    yield return AddressablesCheckDownloadAssetsSize();
                }
                AddressablesUtil.Release(ref checkhandle, null);
            }
            else
            {
                AddressablesUtil.Release(ref checkhandle, null);
                AppManager.eAppError = EAppError.HttpError;
                DebugUtil.LogError("Addressable CheckForCatalogUpdates failed !!!");
            }
        }
        IEnumerator AddressablesCheckDownloadAssetsSize()
        {
            System.Diagnostics.Stopwatch swR = new System.Diagnostics.Stopwatch();
            swR.Start();
            if (VersionHelper.mRemoteAaBundleList.Count > 0)
            {
                var sizeHandle = Addressables.GetDownloadSizeAsync(VersionHelper.mRemoteAaBundleList);
                yield return sizeHandle;

                if (sizeHandle.IsDone && sizeHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    CheckVersionProgress = 0.45f;
                    HotFixTotalSize = (ulong)sizeHandle.Result;
                    AssetBundleTotalSize = (ulong)sizeHandle.Result;
                    NextHotFixPipeline = EHotFixPipeline.CheckAssetList;
                    DebugUtil.LogFormat(ELogType.eNone, string.Format("Load Addressable Bundle Size:{0}", HotFixTotalSize));
                }
                else
                {
                    AppManager.eAppError = EAppError.HttpError;
                    Debug.LogError("Load Addressable Bundle Size failed !!!");
                }
                AddressablesUtil.Release(ref sizeHandle, null);
            }
            else
            {
                CheckVersionProgress = 0.45f;
                NextHotFixPipeline = EHotFixPipeline.CheckAssetList;
            }

            swR.Stop();
            DebugUtil.LogFormat(ELogType.eNone, "Get Download Addressable Bundle Size Need Time Total:{0}ms", swR.ElapsedMilliseconds);
        }
        IEnumerator DownloadRemoteAaBundleList()
        {
            System.Diagnostics.Stopwatch swR = new System.Diagnostics.Stopwatch();
            swR.Start();

            string sRemoteAaHotFixList = string.Format("{0}/{1}/{2}", DownloadABUrl, "aa", AssetPath.sAaHotFixBundleList);
            DebugUtil.LogFormat(ELogType.eNone, "下载Remote Addressable 资源列表 {0}", sRemoteAaHotFixList);
            using (UnityWebRequest req = UnityWebRequest.Get(sRemoteAaHotFixList))
            {
                req.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
                req.SetRequestHeader("Pragma", "no-cache");
                req.timeout = 10;
                yield return req.SendWebRequest();

                AppManager.ResponseCode = req.responseCode;
                if (!req.isDone || req.isNetworkError || req.isHttpError || req.responseCode != 200)
                {
                    if (req.isNetworkError)
                        AppManager.eAppError = EAppError.NetworkError;
                    if (req.isHttpError)
                        AppManager.eAppError = EAppError.HttpError;
                    AppManager.ResponseCode = req.responseCode;
                }
                else
                {
                    MemoryStream ms = new MemoryStream(req.downloadHandler.data);
                    VersionHelper.ReadRemoteAaBundleList(ms);

                    //下载下来，但是序列化没有成功（即资源列表的内容有问题）
                    //if (VersionHelper.mRemoteAaBundleList == null)
                    //{
                    //    AppManager.eAppError = EAppError.RemoteAssetListError;
                    //}
                }
            }

            swR.Stop();
            Debug.LogFormat("Get Download AaBundleList Need Time Total:{0}ms", swR.ElapsedMilliseconds);
        }
        IEnumerator AddressablesDownloadAssets()
        {
            if (VersionHelper.mRemoteAaBundleList != null && AssetBundleTotalSize > 0)
            {
                System.Diagnostics.Stopwatch swR = new System.Diagnostics.Stopwatch();
                swR.Start();
                DebugUtil.LogFormat(ELogType.eNone, "需要更新的资源(Bundle) Size:{0}", HotFixTotalSize);
                DebugUtil.LogFormat(ELogType.eNone, "需要更新的资源(Bundle) Count:{0}", VersionHelper.mRemoteAaBundleList.Count);

                AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(VersionHelper.mRemoteAaBundleList, Addressables.MergeMode.Union);

                while (!downloadHandle.IsDone)
                {
                    DownloadStatus downloadStatus = downloadHandle.GetDownloadStatus();
                    if (downloadStatus.DownloadedBytes > 0)
                        HotFixSize = (ulong)downloadStatus.DownloadedBytes;
                    yield return null;
                }


                if (downloadHandle.IsDone && downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    swR.Stop();
                    DebugUtil.LogFormat(ELogType.eNone, "Download Addressable Bundle total:{0}ms", swR.ElapsedMilliseconds);

                    DownloadStatus downloadStatus = downloadHandle.GetDownloadStatus();
                    DebugUtil.LogFormat(ELogType.eNone, string.Format("已经下载Bundle Size:{0} byte", downloadStatus.TotalBytes));

                    AddressablesUtil.Release(ref downloadHandle, null);
                    if (NewAssets.Count > 0)
                    {
                        NextHotFixPipeline = EHotFixPipeline.FixAssets;
                    }
                    else
                    {
                        NextHotFixPipeline = EHotFixPipeline.RecheckAssetList;
                    }
                    yield break;
                }
                else
                {
                    Debug.LogError("Load Addressable Bundle Asset failed !!!");
                    if (downloadHandle.OperationException.InnerException != null &&
                           downloadHandle.OperationException.InnerException.InnerException != null &&
                           downloadHandle.OperationException.InnerException.InnerException.InnerException != null)
                    {
                        //RemoteProviderException -> ProviderException -> OperationException -> Exception
                        RemoteProviderException providerException = downloadHandle.OperationException.InnerException.InnerException.InnerException as RemoteProviderException;
                        if (providerException != null && providerException.WebRequestResult != null)
                        {
                            UnityWebRequestResult webRequestResult = providerException.WebRequestResult;
                            AppManager.ResponseCode = webRequestResult.ResponseCode;//404
                        }
                    }

                    AppManager.eAppError = EAppError.HttpError;
                }
                AddressablesUtil.Release(ref downloadHandle, null);

                swR.Stop();
                DebugUtil.LogFormat(ELogType.eNone, "Download Addressable Bundle total:{0}ms", swR.ElapsedMilliseconds);
                DebugUtil.LogFormat(ELogType.eNone, "End Download Addressable Bundle :{0}", HotFixTotalSize);
            }
            else
            {
                if (NewAssets.Count > 0)
                {
                    NextHotFixPipeline = EHotFixPipeline.FixAssets;
                }
                else
                {
                    NextHotFixPipeline = EHotFixPipeline.RecheckAssetList;
                }
            }
        }
        public void HotFixHttpErrorHitPoint(UnityWebRequestResult webRequestResult)
        {
            //设置超时,会返回且isNetworkError为true
            if (webRequestResult.Error == "Request timeout")//== 408
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("file_url", webRequestResult.Url);
                dict.Add("wait_time", "10");
                //dict.Add("speed", string.Format("{0}/S", CountSize(DownloadSpeed))); //DownloadSpeed 不准
                //HitPointManager.HitPoint("game_update_httptimeout", dict);
            }
            else if (webRequestResult.ResponseCode == 404)//文件丢失
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("file_url", webRequestResult.Url);
                //HitPointManager.HitPoint("game_update_missing", dict);
            }
            else//其他原因
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("file_url", webRequestResult.Url);
                dict.Add("reason", webRequestResult.Error);
               // HitPointManager.HitPoint("game_update_othercase", dict);
            }

            DebugUtil.LogFormat(ELogType.eNone, string.Format("error:{0}, url={1}", webRequestResult.Error, webRequestResult.Url));
        }


        #endregion


        ListConfigWithEvents<IUpdateReceiver> m_UpdateReceivers = new ListConfigWithEvents<IUpdateReceiver>();
        List<IUpdateReceiver> m_UpdateReceiversToRemove = null;
        bool m_UpdatingReceivers = false;

        /// <summary>
        /// Add an update reveiver.
        /// </summary>
        /// <param name="receiver">The object to add. The Update method will be called until the object is removed. </param>
        public void AddUpdateReceiver(IUpdateReceiver receiver)
        {
            if (receiver == null)
                return;
            m_UpdateReceivers.Add(receiver);
        }

        /// <summary>
        /// Remove update receiver.
        /// </summary>
        /// <param name="receiver">The object to remove.</param>
        public void RemoveUpdateReciever(IUpdateReceiver receiver)
        {
            if (receiver == null)
                return;

            if (m_UpdatingReceivers)
            {
                if (m_UpdateReceiversToRemove == null)
                    m_UpdateReceiversToRemove = new List<IUpdateReceiver>();
                m_UpdateReceiversToRemove.Add(receiver);
            }
            else
            {
                m_UpdateReceivers.Remove(receiver);
            }
        }

    }

}
