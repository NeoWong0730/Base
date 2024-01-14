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

    /// <summary>�ȸ����ļ�������</summary>
    public int UpdateFileCount { get; private set; }

    /// <summary>�����ܴ�С</summary>
    public ulong UpdateTotalSize { get; private set; }


    /// <summary>�Ѿ����µĴ�С</summary>
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
    int m_TimeoutCount = 0;//����ʱ�Ĵ���
    string m_url;

    private bool HasTimedOut => m_TimeoutTimer >= Timeout && m_TimeoutOverFrames > 5;
    string RemoteHotFixAssetsListUrl;
    bool m_WebRequestCompletedCallbackCalled = false;//ִֹͣ�лص������߻ص����ִ��
    private UnityWebRequestAsyncOperation webAsyncOperation;
    public AssetList mFirstPackageAssetsList { get; private set; }
    AsyncOperationHandle<long> sizeHandle;



    //1.��ȡ�װ�ʣ����Ҫ���ص���Դ�б�
    //�װ�ʣ����ԴAssetlist �Ա��ȸ����Assetlist
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
    /// �װ� �ְ���Դ�Ƿ��������,�����ʶ��ɾ���������
    /// </summary>
    /// <returns></returns>
    public bool FirstPackAssetIsLoadFinish()
    {
        //������������Ǻܳ��
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
            string filePath = AssetPath.GetPersistentFullPath(AssetPath.sFirstPackTempListName);
            string loadedMD5 = FrameworkTool.GetFileMD5(filePath);
            if (VersionHelper.FirstPackAssetListMD5 != loadedMD5)
            {
                AppManager.eAppError = EAppError.HotFixAssetListMD5Error;
                DebugUtil.LogErrorFormat("MD5��֤ʧ��:{0} Header��¼md5:{1}  ������ɺ�md5:{2}", url, VersionHelper.FirstPackAssetListMD5, loadedMD5);
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

        //�װ�ʣ����ԴAssetlist �Ա��ȸ���Assetlist,��ȡ����װ���Ҫ���µ���Դ�б�
        AssetsInfo remoteInfo = null;
        var remoteItor = mFirstPackageAssetsList.Contents.GetEnumerator();
        while (remoteItor.MoveNext())
        {
            remoteInfo = remoteItor.Current.Value;

            //mRemoteAssetsList ���ȸ���Ҫ��������޸ģ�
            if (VersionHelper.mRemoteAssetsList == null
                || VersionHelper.mRemoteAssetsList != null && !VersionHelper.mRemoteAssetsList.Contents.ContainsKey(remoteInfo.AssetName))
            {
                NewAssetsInfoList.Add(remoteInfo);
                NewAssetList.Add(remoteInfo.AssetName + ".bundle");
            }
        }


        //��ȡ���صĴ�С �͸���
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
                Debug.LogError("�װ�û��Ҫ���µ���Դ �����Ѿ�������� ��Ҫ�����");
                //�Ѿ�������� ���ܱ�Ǳ��ڲ�ɾ�����ڱ���¼���

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
        //չʾ3�������װ�ʣ����Դ��ʽ
        //1.һ��������ʣ����Դ������Ϸ
        //2.�������(����һ���߳�)
        //3.�����������
        int selectQ = 1;
        switch (selectQ)
        {
            case 1:
                Debug.Log("Ĭ��ʹ��һ��������");
                SelectedMode = EFirstPackageLoadType.RemainLoad;
                CoroutineManager.Instance.StartHandler(AddressablesDownloadAssets(3));
                break;
            case 2: 
                Debug.Log("�������");
                SelectedMode = EFirstPackageLoadType.playLoad;
                CoroutineManager.Instance.StartHandler(AddressablesDownloadAssets(1));
                break;
            case 3: 
                Debug.Log("�������أ��Ῠ�ȼ�����ɺ󣬣�������ֵȿ��ǻ���Ƿȱ��ĿǰAddressable�Ѿ����㰴������");
                break;

        }
    }

    IEnumerator AddressablesDownloadAssets(int maxRequest)
    {
        DebugUtil.LogFormat(ELogType.eNone, "��Ҫ���µ���Դ(Bundle) Size:{0}", UpdateTotalSize);

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
            DebugUtil.LogFormat(ELogType.eNone, string.Format("������ɣ��Ѿ�����Bundle Size:{0} byte", downloadStatus.TotalBytes));

            //��¼�װ�������ɵ��ļ� FirstPackList.txt
            
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
