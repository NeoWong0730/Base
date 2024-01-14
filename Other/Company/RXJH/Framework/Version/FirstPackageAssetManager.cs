using Framework;
using Lib.AssetLoader;
using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Util;

public class FirstPackageAssetManager : TSingleton<FirstPackageAssetManager>
{
    public enum EFirstPackageLoadType
    {
        RemainLoad = 0,
        playLoad = 1,
        NeedLoad = 2,
    }


    public const ulong KiloByte = 1024UL;
    public const ulong MegaByte = 1048576UL;
    public const ulong GigaByte = 1073741824UL;

    public const string sByteFormat = "{0:F2}Byte";
    public const string sKiloByteFormat = "{0:F2}K";
    public const string sMegaByteFormat = "{0:F2}M";
    public const string sGigaByteFormat = "{0:F2}G";

    /// <summary>热更新文件的总数</summary>
    public int UpdateFileCount { get; private set; }

    /// <summary>更新总大小</summary>
    public ulong UpdateTotalSize { get; private set; }


    /// <summary>已经更新的大小</summary>
    public ulong UpdatedSize { get; set; }

    public string SplitUrl;
    public string DownloadABUrl;

    public List<AssetsInfo> NewAssetsInfoList = new List<AssetsInfo>();

    public List<string> NewAssetList = new List<string>();



    public EFirstPackageLoadType SelectedMode;

    int Timeout = 10;
    float m_TimeoutTimer = 0;
    int m_TimeoutOverFrames = 0;
    ulong m_LastDownloadedByteCount = 0;
    int m_Retries;
    int RetryCount = 3;
    int m_TimeoutCount = 0;//请求超时的次数
    string m_url;

    private bool HasTimedOut => m_TimeoutTimer >= Timeout && m_TimeoutOverFrames > 5;
    string RemoteHotFixAssetsListUrl;
    bool m_WebRequestCompletedCallbackCalled = false;//停止执行回调，或者回调完成执行
    private UnityWebRequestAsyncOperation webAsyncOperation;
    public AssetList mFirstPackageAssetsList { get; private set; }
    AsyncOperationHandle<long> sizeHandle;



    //1.获取首包剩余需要下载的资源列表
    //首包剩余资源Assetlist 对比热更后的Assetlist
    public void OnEnter()
    {
        if (FirstPackAssetIsLoadFinish())
            return;

        m_Retries = 0;
        m_TimeoutCount = 0;

        RemoteHotFixAssetsListUrl = string.Format("{0}/{1}/{2}.{3}/{4}?ts={5}",
            VersionHelper.HotFixUrl,
            AssetPath.sPlatformName,
            VersionHelper.RemoteBuildVersion,
            VersionHelper.RemoteAssetVersion,
            AssetPath.sFirstPackListName,
            TimeManager.ClientNowMillisecond());

        BeginOperation();
    }


    /// <summary>
    /// 首包 分包资源是否下载完成,如果标识被删除，则根据
    /// </summary>
    /// <returns></returns>
    public bool FirstPackAssetIsLoadFinish()
    {
        //这个条件还不是很充分
        //if(File.Exists("FirstPackList.txt"))

        //if (PersistentSplitPackAssetVersion.Equals("-1"))
        //    return false;
        //else
        //    return true;

        return false;
    }




    private void BeginOperation()
    {
        m_WebRequestCompletedCallbackCalled = false;
        string filePath = AssetPath.GetPersistentFullPath(AssetPath.sFirstPackTempListName);
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
            string filePath = AssetPath.GetPersistentFullPath(AssetPath.sFirstPackTempListName);
            string loadedMD5 = FrameworkTool.GetFileMD5(filePath);
            if (VersionHelper.FirstPackAssetListMD5 != loadedMD5)
            {
                AppManager.eAppError = EAppError.HotFixAssetListMD5Error;
                DebugUtil.LogErrorFormat("MD5验证失败:{0} Header记录md5:{1}  下载完成后md5:{2}", url, VersionHelper.FirstPackAssetListMD5, loadedMD5);
            }
            else
            {
                SetFirstPackageAssetList(filePath);
                GetFirstPackageAssetList();
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
                    AssetPath.sFirstPackListName);
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

    public void SetFirstPackageAssetList(string filePath)
    {
        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        mFirstPackageAssetsList = AssetList.Deserialize(fileStream);
        fileStream.Close();
        fileStream.Dispose();
    }


    public void GetFirstPackageAssetList()
    {
        UpdateTotalSize = 0;
        NewAssetsInfoList.Clear();

        //首包剩余资源Assetlist 对比热更的Assetlist,获取大概首包需要更新的资源列表
        AssetsInfo remoteInfo = null;
        var remoteItor = mFirstPackageAssetsList.Contents.GetEnumerator();
        while (remoteItor.MoveNext())
        {
            remoteInfo = remoteItor.Current.Value;

            //mRemoteAssetsList 在热更后不要清除（后修改）
            if (VersionHelper.mRemoteAssetsList == null
                || VersionHelper.mRemoteAssetsList != null && !VersionHelper.mRemoteAssetsList.Contents.ContainsKey(remoteInfo.AssetName))
            {
                NewAssetsInfoList.Add(remoteInfo);
                NewAssetList.Add(remoteInfo.AssetName + ".bundle");
            }
        }


        //获取下载的大小 和个数
        if (NewAssetList.Count > 0)
        {
            sizeHandle = Addressables.GetDownloadSizeAsync(NewAssetList);
            sizeHandle.Completed += SizeHandle_Completed;
        }
        else
        {
            Debug.Log("FirstPackageAssets.Count = 0");
        }
    }


    private void SizeHandle_Completed(AsyncOperationHandle<long> obj)
    {
        if (sizeHandle.IsDone && sizeHandle.Status == AsyncOperationStatus.Succeeded)
        {
            UpdateTotalSize = (ulong)sizeHandle.Result;
            DebugUtil.LogFormat(ELogType.eNone, string.Format("Load Addressable Bundle Size:{0}", UpdateTotalSize));

            if (UpdateTotalSize > 0)
            {
                SelectedFirstPackageUpdateMode();
            }
            else
            {
                Debug.LogError("首包没有要更新的资源 或者已经下载完成 需要做标记");
                //已经下载完成 可能标记被内部删除，在标记下即可

            }
        }
        else
        {
            Debug.LogError("Load Addressable Bundle Size failed !!!");
            AppManager.eAppError = EAppError.HttpError;
        }

        AddressablesUtil.Release(ref sizeHandle, SizeHandle_Completed);
    }


    public void SelectedFirstPackageUpdateMode()
    {
        //展示3种下载首包剩余资源形式
        //1.一次性下载剩余资源在完游戏
        //2.边玩边下(开启一个线程)
        //3.按需进行下载
        int selectQ = 1;
        switch (selectQ)
        {
            case 1:
                Debug.Log("默认使用一次性下载");
                SelectedMode = EFirstPackageLoadType.RemainLoad;
                CoroutineManager.Instance.StartHandler(AddressablesDownloadAssets(3));
                break;
            case 2: 
                Debug.Log("边玩边下");
                SelectedMode = EFirstPackageLoadType.playLoad;
                CoroutineManager.Instance.StartHandler(AddressablesDownloadAssets(1));
                break;
            case 3: 
                Debug.Log("按需下载，会卡等加载完成后，，这个表现等考虑会有欠缺，目前Addressable已经满足按需下载");
                break;

        }
    }

    IEnumerator AddressablesDownloadAssets(int maxRequest)
    {
        DebugUtil.LogFormat(ELogType.eNone, "需要更新的资源(Bundle) Size:{0}", UpdateTotalSize);

        WebRequestQueue.SetMaxConcurrentRequests(maxRequest);


        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(NewAssetList, Addressables.MergeMode.Union);

        while (!downloadHandle.IsDone)
        {
            DownloadStatus downloadStatus = downloadHandle.GetDownloadStatus();
            if (downloadStatus.DownloadedBytes > 0)
                UpdatedSize = (ulong)downloadStatus.DownloadedBytes;
            yield return null;
        }


        if (downloadHandle.IsDone && downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            DownloadStatus downloadStatus = downloadHandle.GetDownloadStatus();
            DebugUtil.LogFormat(ELogType.eNone, string.Format("下载完成，已经下载Bundle Size:{0} byte", downloadStatus.TotalBytes));

            //记录首包下载完成的文件 FirstPackList.txt
            
        }
        else
        {
            Debug.LogError("Load Addressable Bundle Asset failed !!!");
            AppManager.eAppError = EAppError.HttpError;
        }

        AddressablesUtil.Release(ref downloadHandle, null);

    }


    public void OnExit()
    {
        VersionHelper.Clear();

    }
}
