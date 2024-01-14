using Lib;
using UnityEngine;
using System;
using System.IO;

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

    public static class AppManager
    {
        public static EAppState eAppState;
        public static EAppState NextAppState;

        public static EAppError eAppError = EAppError.None;
        public static long ResponseCode = 200;

        public static float fInitProgress = 0;

        public static bool bHotfix = false;

        public static Camera mUICamera;

        public static int ProcessID;

        public static int nPerformanceScore;

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
        }

        public static void DestoryUIProgressBar()
        {
            if (uiProgressBar)
            {
                GameObject.DestroyImmediate(uiProgressBar);
                uiProgressBar = null;
            }
        }

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

        public static void ClearCachingData()
        {
            string tempCachePath = Application.temporaryCachePath;
            FileOperationHelper.DeleteDirectory(tempCachePath);

            string aaCatalogPath = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sAddressableCatalogDir);
            FileOperationHelper.DeleteDirectory(aaCatalogPath);

            Caching.ClearCache();

            string configPath = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sAssetCsvDir);
            FileOperationHelper.DeleteDirectory(configPath);

            string logicPath = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sAssetLogicDir);
            FileOperationHelper.DeleteDirectory(logicPath); ;

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
