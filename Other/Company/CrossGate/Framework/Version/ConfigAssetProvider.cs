using Framework;
using Lib.AssetLoader;
using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Util;


//public struct ConfigProvideHandle
//{
    //private Func<DownloadStatus> m_GetDownloadProgressCallback;
    //private DownloadStatus m_DownloadStatus;

    //int m_LoadedCount;
    //public List<ConfigAssetProvider> Result = new List<ConfigAssetProvider>();

    //public void SetProgressCallback(Func<float> callback)
    //{
    //    //InternalOp.SetProgressCallback(callback);
    //}

    //public void SetDownloadProgressCallbacks(Func<DownloadStatus> callback)
    //{
    //   // InternalOp.SetDownloadProgressCallback(callback);
    //}

    //public void Execute()
    //{
    //    m_LoadedCount = 0;
    //    for (int i = 0; i < Result.Count; i++)
    //    {
    //        if (Result[i].IsDone)
    //            m_LoadedCount++;
    //        else
    //            Result[i].Completed += m_InternalOnComplete;
    //    }
    //}


    //public void AddRange(ConfigAssetProvider configAssetProvider)
    //{
    //    Result.Add(configAssetProvider);
    //}


    //public DownloadStatus GetDownloadStatus(HashSet<object> visited)
    //{
    //    if (m_GetDownloadProgressCallback != null)
    //        m_DownloadStatus = m_GetDownloadProgressCallback();

    //    if (Status == AsyncOperationStatus.Succeeded)
    //        m_DownloadStatus.DownloadedBytes = m_DownloadStatus.TotalBytes;

    //    return new DownloadStatus() { DownloadedBytes = m_DownloadStatus.DownloadedBytes, TotalBytes = m_DownloadStatus.TotalBytes, IsDone = IsDone };
    //}

//}

//public static class ConfigResourceManager
//{
//    public static int LoadedHotFixFileCount = 0;
//    public static AsyncOperationHandle DownloadConfigAsync(List<AssetsInfo> NewAssets, bool autoReleaseHandle = false)
//    {
//        string cacheAssetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);
//        string persistentAssetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);
//        LoadedHotFixFileCount = 0;


//        while (NewAssets.Count > 0)
//        {
//            //if (NextHotFixPipeline == EHotFixPipeline.WaitFail)
//            //    break;
//            AssetsInfo assetsInfo = NewAssets[0];
//            NewAssets.RemoveAt(0);

//            string path = assetsInfo.AssetName.Replace("\\", "/");
//            string filePath = AssetPath.GetPersistentFullPath(path);
//            string dir = Path.GetDirectoryName(filePath);

//            //如果不是缓存则先删除
//            if (HotFixStateManager.Instance.CacheAssetList == null || !HotFixStateManager.Instance.CacheAssetList.Contents.ContainsKey(assetsInfo.AssetName))
//            {
//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//            }

//            if (!Directory.Exists(dir))
//            {
//                Directory.CreateDirectory(dir);
//            }

//            //校验失败的 需要删除再次下载
//            if (HotFixStateManager.Instance.RetryDownloadCountDict.ContainsKey(assetsInfo.AssetName))
//            {
//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//            }

//            //获取下载文件长度 
//            long totalLength = (long)assetsInfo.Size;
//            //大于1M放入缓存
//            if ((ulong)totalLength >= HotFixStateManager.Instance.CacheFileMinSize)
//            {
//                if (HotFixStateManager.Instance.CacheAssetList == null)
//                    HotFixStateManager.Instance.CacheAssetList = new AssetList();
//                HotFixStateManager.Instance.CacheAssetList.Contents[assetsInfo.AssetName] = assetsInfo;
//                VersionHelper.WriteAssetList(cacheAssetsListPath, HotFixStateManager.Instance.CacheAssetList);
//            }

//            new ConfigAssetProvider().Start(assetsInfo);
//        }

//        var handle = new AsyncOperationHandle();
//        return handle;

//    }
//}


//public class ConfigAssetProvider : IUpdateReceiver
//{
//    AssetsInfo assetInfo;
//    ConfigWebRequestQueueOperation m_WebRequestQueueOperation;
//    AsyncOperation m_RequestOperation;
//    int m_Retries;
//    FileStream fileStream;

//    long m_BytesToDownload;
//    long m_DownloadedBytes;
//    ulong m_LastDownloadedByteCount = 0;
//    float m_TimeoutTimer = 0;
//    int m_TimeoutOverFrames = 0;
//    int m_TimeoutCount = 0;//请求超时的次数
//    long m_totalDownloadedBytes = 0;

//    bool m_WebRequestCompletedCallbackCalled = false;

//    public string persistFilePath;
//    public string relativeFilePath;

//    public string HotFixConfigUrl;

//    int Timeout = 10;
//    int RetryCount = 3;

//    private bool HasTimedOut => m_TimeoutTimer >= Timeout && m_TimeoutOverFrames > 5;

//    internal long BytesToDownload
//    {
//        get
//        {
//            if (m_BytesToDownload == -1)
//            {
//                if (assetInfo != null)
//                {
//                    //下载到一半的文件 计算剩余需要下载的资源大小
//                    FileInfo file = new FileInfo(persistFilePath);
//                    m_BytesToDownload = (long)assetInfo.Size - file.Length;
//                }
//                else
//                    m_BytesToDownload = 0;
//            }
//            return m_BytesToDownload;
//        }
//    }



//    public bool IsDone
//    {
//        get { return m_RequestOperation != null || m_RequestOperation.isDone; }
//    }


//    float PercentComplete() { return m_RequestOperation != null ? m_RequestOperation.progress : 0.0f; }
//    DownloadStatus GetDownloadStatus()
//    {
//        var status = new DownloadStatus() { TotalBytes = BytesToDownload, IsDone = PercentComplete() >= 1f };
//        if (BytesToDownload > 0)
//        {
//            if (m_WebRequestQueueOperation != null && string.IsNullOrEmpty(m_WebRequestQueueOperation.m_WebRequest.error))
//                m_DownloadedBytes = m_totalDownloadedBytes + (long)(m_WebRequestQueueOperation.m_WebRequest.downloadedBytes);
//            else if (m_RequestOperation != null && m_RequestOperation is UnityWebRequestAsyncOperation operation && string.IsNullOrEmpty(operation.webRequest.error))
//                m_DownloadedBytes = m_totalDownloadedBytes + (long)operation.webRequest.downloadedBytes;
//        }

//        status.DownloadedBytes = m_DownloadedBytes;
//        return status;
//    }

//    public void Start(AssetsInfo info)
//    {
//        m_Retries = 0;
//        m_TimeoutCount = 0;
//        m_DownloadedBytes = 0;
//        m_BytesToDownload = -1;
//        m_totalDownloadedBytes = 0;
//        assetInfo = info;
//        m_WebRequestCompletedCallbackCalled = false;
//        relativeFilePath = assetInfo.AssetName.Replace("\\", "/");
//        persistFilePath = AssetPath.GetPersistentFullPath(relativeFilePath);
//        HotFixConfigUrl = HotFixStateManager.Instance.DownloadABUrl;
//        BeginOperation();
//    }

//    public void EXit()
//    {
//        if(fileStream != null)
//        {
//            fileStream.Close();
//            fileStream.Dispose();
//        }

//        if (m_WebRequestQueueOperation != null)
//        {
//            if (m_WebRequestQueueOperation.IsDone)
//            {
//                if (m_RequestOperation != null && m_RequestOperation is UnityWebRequestAsyncOperation operation)
//                {
//                    operation.webRequest.Abort();

//                    m_RequestOperation = null;
//                    UnityWebRequest webRequest = operation.webRequest;
//                    FilesDownloadHandler filesDownloadHandler = webRequest?.downloadHandler as FilesDownloadHandler;
//                    if (filesDownloadHandler != null)
//                    {
//                        filesDownloadHandler.ErrorDispose(false);
//                        filesDownloadHandler.Dispose();
//                    }

//                    if (webRequest != null)
//                    {
//                        webRequest.Dispose();
//                    }
//                }
//            }
//            else
//            {
//                //还在队列中，需要从队列中 移除
//                m_WebRequestQueueOperation.Exit(); 
//            }
//        }
//    }


//    public void Update(float unscaledDeltaTime)
//    {
//        if (m_RequestOperation != null && m_RequestOperation is UnityWebRequestAsyncOperation operation && !operation.isDone)
//        {
//            if (m_LastDownloadedByteCount != operation.webRequest.downloadedBytes)
//            {
//                m_TimeoutTimer = 0;
//                m_TimeoutOverFrames = 0;
//                m_LastDownloadedByteCount = operation.webRequest.downloadedBytes;
//            }
//            else
//            {
//                m_TimeoutTimer += unscaledDeltaTime;
//                if (HasTimedOut)
//                    operation.webRequest.Abort();

//                m_TimeoutOverFrames++;
//            }
//        }
//    }


//    private void BeginOperation()
//    {
//        m_WebRequestCompletedCallbackCalled = false;

//        string url = string.Format("{0}/{1}", HotFixConfigUrl, relativeFilePath);
//        string tsUrl = string.Format("{0}?ts={1}", url, TimeManager.ClientNowMillisecond());

//        //获取已下载的文件长度，没有则创建
//        fileStream = new FileStream(persistFilePath, FileMode.OpenOrCreate, FileAccess.Read);
//        long cacheLength = fileStream.Length;
//        fileStream.Close();
//        fileStream.Dispose();

//        long totalLength = (long)assetInfo.Size;
//        UnityWebRequest HttpReq = new UnityWebRequest(tsUrl, UnityWebRequest.kHttpVerbGET);
//        FilesDownloadHandler filesDownload = new FilesDownloadHandler(persistFilePath);
//        HttpReq.downloadHandler = filesDownload;        
//        HttpReq.disposeDownloadHandlerOnDispose = false;

//        HttpReq.SetRequestHeader("Range", "bytes=" + cacheLength + "-");
        
//        if (cacheLength < totalLength)
//        {
//            m_WebRequestQueueOperation = ConfigWebRequestQueue.QueueRequest(HttpReq);
//            if (m_WebRequestQueueOperation.IsDone)
//                BeginWebRequestOperation(m_WebRequestQueueOperation.Result);
//            else
//                m_WebRequestQueueOperation.OnComplete += asyncOp => BeginWebRequestOperation(asyncOp);
//        }
//    }

//    private void BeginWebRequestOperation(AsyncOperation asyncOp)
//    {
//        m_TimeoutTimer = 0;
//        m_TimeoutOverFrames = 0;
//        m_totalDownloadedBytes += (long)m_LastDownloadedByteCount;
//        m_LastDownloadedByteCount = 0;
   
//        m_RequestOperation = asyncOp;

//        if (m_RequestOperation == null || m_RequestOperation.isDone)
//            WebRequestOperationCompleted(m_RequestOperation);
//        else
//        {
//            if (Timeout > 0)
//                HotFixManager.Instance.AddUpdateReceiver(this);
//            m_RequestOperation.completed += WebRequestOperationCompleted;
//        }
//    }


//    private void WebRequestOperationCompleted(AsyncOperation op)
//    {
//        if (m_WebRequestCompletedCallbackCalled)
//            return;

//        if (Timeout > 0)
//            HotFixManager.Instance.RemoveUpdateReciever(this);

//        m_WebRequestCompletedCallbackCalled = true;
//        UnityWebRequestAsyncOperation remoteReq = op as UnityWebRequestAsyncOperation;
//        var webReq = remoteReq?.webRequest;
//        FilesDownloadHandler filesDownloadHandler = webReq?.downloadHandler as FilesDownloadHandler;

//        UnityWebRequestResult uwrResult = null;
//        if (webReq != null && !UnityWebRequestUtilities.RequestHasErrors(webReq, out uwrResult))
//        {
//            ++HotFixStateManager.Instance.LoadedHotFixFileCount;
//            //DebugUtil.LogFormat( ELogType.eNone,string.Format("下载资源完成 url = {0},loadedCount = {1}", webReq.url, HotFixManager.Instance.LoadedHotFixFileCount));
//            HotFixStateManager.Instance.HotFixSize += (ulong)webReq.downloadedBytes;
//            VertifyConfigAssetMD5(webReq.url);

//            filesDownloadHandler.ErrorDispose(false);
//            filesDownloadHandler.Dispose();
//            webReq.Dispose();
//        }
//        else
//        {
//            if (HasTimedOut)
//            {
//                uwrResult.Error = "Request timeout";
//            }

//            webReq = m_WebRequestQueueOperation.m_WebRequest;
//            if (uwrResult == null)
//                uwrResult = new UnityWebRequestResult(m_WebRequestQueueOperation.m_WebRequest);

//            filesDownloadHandler.ErrorDispose(true);
//            filesDownloadHandler.Dispose();
//            webReq.Dispose();

//            bool forcedRetry = false;
//            //string message = string.Format("Web request failed, retrying {0}/{1} url:{2}}", m_Retries, RetryCount, uwrResult.Url);

//            if (!forcedRetry)
//            {
//                //回调这里执行打点的逻辑
//                ResourceManager.HttpErrorOnComplete?.Invoke(uwrResult);

//                if (m_Retries < RetryCount && uwrResult.Error != "Request aborted")
//                {
//                    m_Retries++;
//                    DebugUtil.LogFormat(ELogType.eNone, string.Format("Web request failed url:{0}, retrying {1}/{2}", uwrResult.Url, m_Retries, RetryCount));
//                    BeginOperation();
//                }
//                else if (m_TimeoutCount < ResourceManager.HotFixUrlArr.Length - 1)
//                {
//                    m_TimeoutCount++;
//                    m_Retries = 0;
//                    //这里在换链接后，要把前一个连接保存的删除
//                    if (File.Exists(persistFilePath))
//                        File.Delete(persistFilePath);

//                    HotFixConfigUrl = string.Format("{0}/{1}/{2}.{3}", VersionHelper.HotFixUrlArr[m_TimeoutCount], 
//                        AssetPath.sPlatformName, 
//                        VersionHelper.RemoteBuildVersion, 
//                        VersionHelper.RemoteAssetVersion);
//                    DebugUtil.LogFormat(ELogType.eNone, string.Format("Web request replace cdn:{0}, retrying {1}/{2}", HotFixConfigUrl, m_Retries, RetryCount));
//                    BeginOperation();
//                }
//                else
//                {
//                    AppManager.ResponseCode = uwrResult.ResponseCode;
//                    AppManager.eAppError = EAppError.HttpError;
//                }
//            }
//        }
//        //webReq.Dispose();
//    }


//    private void VertifyConfigAssetMD5(string url)
//    {
//        string md5 = FrameworkTool.GetFileMD5(persistFilePath);

//        if (string.Equals(assetInfo.AssetMD5, md5, StringComparison.Ordinal))
//        {
//            VersionHelper.AppendAssetInfoToPersistent(assetInfo);

//            //假如缓存里面有正在下载的  那么在下载完成后要删除缓存里正在下载的记录
//            if (HotFixStateManager.Instance.CacheAssetList != null && HotFixStateManager.Instance.CacheAssetList.Contents != null 
//                && HotFixStateManager.Instance.CacheAssetList.Contents.ContainsKey(assetInfo.AssetName))
//            {
//                HotFixStateManager.Instance.CacheAssetList.Contents.Remove(assetInfo.AssetName);
//                string cacheAssetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixCacheListName);
//                VersionHelper.WriteAssetList(cacheAssetsListPath, HotFixStateManager.Instance.CacheAssetList);
//            }

//            if (HotFixStateManager.Instance.RetryDownloadCountDict.ContainsKey(assetInfo.AssetName))
//            {
//                HotFixStateManager.Instance.RetryDownloadCountDict.Remove(assetInfo.AssetName);
//            }
//        }
//        else
//        {
//            Dictionary<string, string> dict = new Dictionary<string, string>();
//            dict.Add("file_url", url);
//            dict.Add("file_md5_correct", assetInfo.AssetMD5);
//            dict.Add("file_md5_error", md5);
//           // HitPointManager.HitPoint("game_update_md5error", dict);

//            Debug.LogErrorFormat("MD5验证失败:{0} 记录md5:{1}  下载md5:{2}", assetInfo.AssetName, assetInfo.AssetMD5, md5);

//            //校验失败的特殊处理,直接标注清除标记
//            if (HotFixStateManager.Instance.RetryDownloadCountDict.ContainsKey(assetInfo.AssetName))
//            {
//                HotFixStateManager.Instance.RetryDownloadCountDict[assetInfo.AssetName]++;
//            }
//            else
//            {
//                HotFixStateManager.Instance.RetryDownloadCountDict.Add(assetInfo.AssetName, 1);
//            }
//        }
//    }





//}
