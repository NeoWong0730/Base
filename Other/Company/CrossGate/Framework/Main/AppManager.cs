using Lib.AssetLoader;
using System;
using System.IO;
using UnityEngine;

namespace Framework
{
    public enum EAppError
    {
        None,
        NetworkError,//网络中断
        HttpError,//1.网络出错（中断） 2.文件不存在请求不到   
        RemoteVersionNetError,
        RemoteVersionInfoError,
        RemoteAssetListError,
        HotFixAssetListMD5Error,
        StreamingVersionError,
        VersionVerifyError,
        AssetVerifyError,
        HotFixSeriesError,
        MemoryNotEnoughError,
        RemoteAssetCompleteFailure,
        RemoteAssetMissError,
    }

    //HotFixError == EHotFixError.StreamingVersionError || HotFixError == EHotFixError.VersionVerifyError

    /// <summary>
    /// 用于APP的数据管理
    /// 包含APP状态
    /// APP设置
    /// </summary>
    public static class AppManager
    {
        public static EAppState eAppState;
        public static EAppState NextAppState;
        public static bool UseILRuntime;

        public static EAppError eAppError = EAppError.None;
        public static long ResponseCode = 200;

        public static float fInitProgress = 0;

        public static bool bHotFix = false;

        public static Camera mUICamera;

        public static int ProcessID;

        public static int nPerformanceScore;        

        #region 加载进度条
        public static float InitGameProgress { get; set; }
        private static GameObject uiProgressBar;
        public static void LoadUIProgressBar()
        {
            if (uiProgressBar)
                return;

            uiProgressBar = GameObject.Instantiate(Resources.Load("UI_ProgressBar") as GameObject);
            uiProgressBar.transform.localPosition = Vector3.zero;
            uiProgressBar.transform.localScale = Vector3.one;
            uiProgressBar.gameObject.SetActive(true);
            GameObject.DontDestroyOnLoad(uiProgressBar);
        }

        public static void DestroyUIProgressBar()
        {
            if (uiProgressBar)
            {                
                GameObject.DestroyImmediate(uiProgressBar);
                uiProgressBar = null;
            }
        }


        #endregion
        
        //开场UI
        //public static AssetRequest mAssetRequest = null;
        //public static GameObject mLaunchUI = null;

        public static bool bLevelDebug;

        public static UnityEngine.EventSystems.EventSystem mEventSystem;
        
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        
    #if UNITY_ANDROID
               SDKManager.SDKExitGame();
    #else
               Application.Quit();
    #endif

#endif
        }

        //     public static void OpenLaunchUI()
        //     {
        //         if(mLaunchUI == null)
        //         {            
        //             AssetMananger.Instance.LoadAssetAsyn("UI/Launch/UI_Launch.prefab", ref mAssetRequest, OnLoaded);
        //         }        
        //     }
        // 
        //     public static void CloseLaunchUI()
        //     {
        //         AssetMananger.Instance.UnloadAsset(ref mAssetRequest, OnLoaded);
        //         if(mLaunchUI != null)
        //         {
        //             GameObject.DestroyImmediate(mLaunchUI);
        //             mLaunchUI = null;
        //         }        
        //     }
        // 
        //     private static void OnLoaded(AssetRequest request)
        //     {
        //         mLaunchUI = request.Instantiate<GameObject>();
        //         mLaunchUI.transform.localPosition = Vector3.zero;
        //         mLaunchUI.transform.localScale = Vector3.one;
        //     }


       

        public static void ClearCachingData()
        {
            //安装包覆盖安装，需要清除之前缓存的资源，为了防止从缓存拿，导致资源错误
            //1.清除临时缓存文件夹
            string tempCachePath = Application.temporaryCachePath;
            FileOperationHelpter.DeleteDirctory(tempCachePath);

            string aaCatalogPath = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sAddressableCatalogDir);
            FileOperationHelpter.DeleteDirctory(aaCatalogPath);

            Caching.ClearCache();
            //DeleteFiles(Caching.defaultCache.path);
            
            string configPath = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sAssetCsvDir);
            FileOperationHelpter.DeleteDirctory(configPath);

            string logicPath = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sAssetLogicDir);
            FileOperationHelpter.DeleteDirctory(logicPath);

            string hotfixCacheTxt = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sHotFixCacheListName);
            if (File.Exists(hotfixCacheTxt))
                File.Delete(hotfixCacheTxt);

            string hotfixTxt = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sHotFixListName);
            if (File.Exists(hotfixTxt))
                File.Delete(hotfixTxt);

            string versionTxt = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sVersionName);
            if (File.Exists(versionTxt))
                File.Delete(versionTxt);
        }
        
    }
}
