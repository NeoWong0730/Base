using Framework;
using Lib.AssetLoader;
using Lib.Core;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class AppState_CheckVersion : IAppState
{
    //private bool _waitFirstSceneLoad = false;
    
    //private static bool _isDone = false;
    //private static int _retry = 0;
    public EAppState State
    {
        get
        {
            return EAppState.CheckVersion;
        }
    }

    public void OnEnter()
    {
        //当第一次进入的时候 等待视频准备完成后再加载空场景
        //其他情况下 场景应该就是空场景
        VideoManager.Play(Lib.AssetLoader.AssetPath.GetVideoFullUrl("Config/Video/cutscene_login_01.mp4"), true);
       //if (SceneManager.mMainScene == null)
       //{
       //    _waitFirstSceneLoad = true;            
       //}
       //else if (!string.Equals(SceneManager.mMainScene.sName, "Empty"))
       //{
       //    SceneManager.LoadSceneAsync("Empty", UnityEngine.SceneManagement.LoadSceneMode.Single);
       //}

        //HitPointManager.HitPoint("game_hotfix_start");
        //CoroutineManager.Instance.StartHandler(CheckVersion());
    }

    public void OnExit()
    {
        //if (_waitFirstSceneLoad)
        //{
        //    _waitFirstSceneLoad = false;
        //    SceneManager.LoadSceneAsync("Empty", UnityEngine.SceneManagement.LoadSceneMode.Single);
        //}

        if (AppManager.NextAppState != EAppState.HotFix)
        {
            //UIHotFix.Destroy();
        }
    }

    public void OnFixedUpdate() { }

    public float OnLoding() { return 1f; }

    public void OnUpdate()
    {
       //if (_waitFirstSceneLoad)
       //{
       //    if (VideoManager.isPlaying && _isDone)
       //    {
       //        _waitFirstSceneLoad = false;
       //        SceneManager.LoadSceneAsync("Empty", UnityEngine.SceneManagement.LoadSceneMode.Single);
       //    }
       //}
    }

    static IEnumerator LoadPersistentVersion()
    {
        string persistentVersionPath = string.Format("{0}{1}/{2}", AssetPath.sPersistentPrefix, AssetPath.persistentDataPath, AssetPath.sVersionName);
        DebugUtil.LogFormat(ELogType.eNone, "LoadPersistentVersion {0}", persistentVersionPath);
        string persistentPath = AssetPath.GetPersistentFullPath(AssetPath.sVersionName);
        if (File.Exists(persistentPath))
        {
            using (UnityWebRequest req = UnityWebRequest.Get(persistentVersionPath))
            {
                //req.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
                //req.SetRequestHeader("Pragma", "no-cache");
                //req.timeout = 5;
                yield return req.SendWebRequest();

                if (!req.isDone || req.isNetworkError || req.isHttpError)//|| req.responseCode != 200)
                {
                    DebugUtil.LogWarningFormat("LoadPersistentVersion Fail {0}", req.error);
                }
                else
                {
                    MemoryStream ms = new MemoryStream(req.downloadHandler.data);
                    //VersionHelper.ReadPersistentVersion(ms);
                }
            }
        }
    }
    static IEnumerator DownloadRemoteVersion(string remoteVersionUrl)
    {
        DebugUtil.LogFormat(ELogType.eNone, "DownloadRemoteVersion {0}", remoteVersionUrl);
        //HitPointManager.SDKHitPointBaseData pointBaseData = HitPointManager.GetSDKHitPointBaseData();
        //HitPointManager.HitPoint("game_checkversion_start");

        using (UnityWebRequest req = UnityWebRequest.Get(remoteVersionUrl))
        {
            req.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
            req.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
            req.SetRequestHeader("Pragma", "no-cache");
            //req.SetRequestHeader("device-id", pointBaseData.device_id); 
            //req.SetRequestHeader("os", AssetPath.sPlatformName);
            //req.SetRequestHeader("app-version", pointBaseData.app_version);
            //req.SetRequestHeader("channel", pointBaseData.channel);
            //req.SetRequestHeader("channel-id", pointBaseData.markert_channel);

            string headerStr = string.Format("Content-Type:{0},Cache-Control:{1},Pragma:{2},device-id:{3},os:{4},app-version:{5},channel:{6},channel-id:{7}",
                req.GetRequestHeader("Content-Type"), 
                req.GetRequestHeader("Cache-Control"),
                req.GetRequestHeader("Pragma"),
                req.GetRequestHeader("device-id"),
                req.GetRequestHeader("os"),
                req.GetRequestHeader("app-version"),
                req.GetRequestHeader("channel"),
                req.GetRequestHeader("channel-id")
                );

            DebugUtil.LogFormat(ELogType.eNone, "设置Header\n {0}", headerStr);
            
            req.timeout = 10;
            yield return req.SendWebRequest();

            AppManager.ResponseCode = req.responseCode;
            if (!req.isDone || req.isNetworkError || req.isHttpError || req.responseCode != 200)
            {
                //UIHotFix.Create();
                Hashtable hashtable = new Hashtable();
                hashtable.Add("file_url", remoteVersionUrl);
                hashtable.Add("reason", req.error);
                //HitPointManager.HitPoint("game_checkversion_fail", hashtable);

                AppManager.eAppError = EAppError.RemoteVersionNetError;
                yield break;
            }
            else
            {
                MemoryStream ms = new MemoryStream(req.downloadHandler.data);
                //VersionHelper.ReadRemoteVersion(ms);
                //if (string.IsNullOrWhiteSpace(VersionHelper.RemoteBuildVersion) || string.IsNullOrWhiteSpace(VersionHelper.RemoteAssetVersion) || string.IsNullOrWhiteSpace(VersionHelper.HotFixUrl))
                //{
                //    AppManager.eAppError = EAppError.RemoteVersionInfoError;
                //}
                //else
                //{
                //    UnityEngine.AddressableAssets.PathRebuild.LoadRemoteVersionFinish = true;
                //}
            }
        }
      
    }

    static IEnumerator InitAssets()
    {
        string persistentABPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sAssetBundleDir);

        string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
        string persistentAssetsListPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sHotFixListName);

        string streamingVersionPath = string.Format("{0}{1}/{2}", AssetPath.sStreamPrefix, AssetPath.streamingAssetsPath, AssetPath.sVersionName);
        string streamingAssetsListPath = string.Format("{0}{1}/{2}", AssetPath.sStreamPrefix, AssetPath.streamingAssetsPath, AssetPath.sHotFixListName);

        DebugUtil.LogFormat(ELogType.eNone, "删除沙盒目录资源 {0}", persistentABPath);
        if (Directory.Exists(persistentABPath))
        {
            DirectoryInfo info = new DirectoryInfo(persistentABPath);
            info.Delete(true);
            info = null;
        }
        yield return null;

        DebugUtil.LogFormat(ELogType.eNone, "删除 Persistent Version {0}", persistentVersionPath);
        if (File.Exists(persistentAssetsListPath))
        {
            File.Delete(persistentAssetsListPath);
        }
        yield return null;

        DebugUtil.LogFormat(ELogType.eNone, "删除 Persistent Assets List {0}", persistentAssetsListPath);
        if (File.Exists(persistentAssetsListPath))
        {
            File.Delete(persistentAssetsListPath);
        }
        yield return null;

        DebugUtil.LogFormat(ELogType.eNone, "拷贝 Streaming Version to Persistent {0}", persistentVersionPath);
        //VersionHelper.SetPersistentVersion(VersionHelper.StreamingBuildVersion, VersionHelper.StreamingAssetVersion);
        //VersionHelper.WritePersistentVersion(persistentVersionPath);
        yield return null;
    }



    private void LoadHotFixUI()
    {
        GameObject uiroot = new GameObject("HotFixUIRoot");
        GameObject uiHotFix = GameObject.Instantiate(Resources.Load("UI_HotFix") as GameObject);
        uiHotFix.transform.SetParent(uiroot.transform);
        uiHotFix.transform.localPosition = Vector3.zero;
        uiHotFix.transform.localScale = Vector3.one;
        uiHotFix.gameObject.SetActive(true);
    }


#if USE_ADDRESSABLE_ASSET
    //private void AddressableInternalIdTransformFunc()
    //{
    //    //检查版本的时候 需要先地址重定向
    //    Addressables.InternalIdTransformFunc += location =>
    //    {
    //        //string originalRemoteLoadPath = "http://192.168.1.224:8008/hotres/StandaloneWindows";
    //        //string modifyRemoteLoadPath = "http://192.168.1.224:8008/hotres/StandaloneWindows/1.0/3/aa";

    //        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
    //        string originalRemoteLoadPath = m_Settings.RemoteCatalogLoadPath.GetValue(m_Settings);
    //        string modifyRemoteLoadPath = "{UnityEngine.AddressableAssets.PathRebuild.RemoteLoadPath}";//string.Format("{0}/{1}/{2}/{3}/{4}", VersionHelper.HotFixUrl, AssetPath.sPlatformName, VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion, AssetPath.sAddressableDir);

    //        string transPath = location.InternalId;
    //        if (location.InternalId.Contains(originalRemoteLoadPath))
    //        {
    //            transPath = location.InternalId.Replace(originalRemoteLoadPath, modifyRemoteLoadPath);
    //        }
    //        return transPath;
    //    };
    //}

#endif

    public void OnLateUpdate() { }
    public void OnLowMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
    public void OnGUI() { }    

    public void OnApplicationPause(bool pause) { }
}
