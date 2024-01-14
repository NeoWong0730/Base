using System.IO;
using UnityEngine;

namespace Lib.AssetLoader
{
    public static class AssetPath
    {
        //WWW加载需要的前缀
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static readonly string sStreamPrefix = "file://";
        public static readonly string sPersistentPrefix = "file:///";
        public static readonly string sPlatformName = "StandaloneWindows";
#elif UNITY_IOS
    public static readonly string sStreamPrefix = "file://";
    public static readonly string sPersistentPrefix = "file://";
    public static readonly string sPlatformName = "iOS";
#elif UNITY_ANDROID
    public static readonly string sStreamPrefix = "";
    public static readonly string sPersistentPrefix = "file://";
    public static readonly string sPlatformName = "Android";
#endif
        //资源根目录以及拓展名
        public static readonly string sVersionName = "Version.txt";
        public static readonly string sHotFixListName = "HotFixList.txt";
        public static readonly string sHotFixTempListName = "HotFixTempList.txt";
        public static readonly string sHotFixCacheListName = "HotFixCacheList.txt";
        public static readonly string sAaHotFixBundleList = "AaHotFixBundleList.txt";

        public static readonly string sResourcesRootDir = "ResourcesAB";
        public static readonly string sTestDir = "Assets/Test";
        public static readonly string sGizmosDir = "Assets/Gizmos";
        public static readonly string sDesigner_EditorDir = "Assets/Designer_Editor";
        public static readonly string sGameToolEditorDir = "Assets/GameToolEditor";

        public static readonly string sAddressableBundle = "AddressableBundle";
        public static readonly string sSplitPackName = "S0";
        public static readonly string sSplitPackVersion = "SplitPackVersion.txt";
        public static readonly string sFirstPackListName = "FirstPackList.txt";
        public static readonly string sFirstPackTempListName = "FirstPackTempList.txt";
        public static readonly string sFirstPackCacheListName = "FirstPackCacheList.txt";

        public static readonly string sAddressableDir = "aa";
        public static readonly string sAssetBundleDir = "ab";
        public static readonly string sAssetCsvDir = "Config";
        public static readonly string sAssetLogicDir = "LogicDll";
        public static readonly string sAssetCodeDir = "Scripts/Logic/";
        public static readonly string sAddressableCatalogDir = "com.unity.addressables";

        public static readonly string sAssetExtension = ".ab";

        public const string sLogicDllName = "Logic.dll";
        public const string sLogicPdbName = "Logic.pdb";

        public static readonly string persistentDataPath = Application.persistentDataPath;
#if UNITY_EDITOR
        public static readonly string dataPath = Application.dataPath;

        public static readonly string streamingAssetsPath = Path.GetFullPath(Application.dataPath + "/../AssetBundle/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString());
#else
    public static readonly string streamingAssetsPath = Application.streamingAssetsPath;
#endif

        //PC热更状态标识
        public static readonly string sHotFixFlag = "HotFixFlag.log";
        public static readonly string sProcessDir = "AppProcess";

        public static string GetPersistentAssetFullPath(string relativePath, string ext = null)
        {
            if (string.IsNullOrWhiteSpace(ext))
            {
                return string.Format("{0}/{1}/{2}", persistentDataPath, sAssetBundleDir, relativePath);
            }
            else
            {
                return string.Format("{0}/{1}/{2}{3}", persistentDataPath, sAssetBundleDir, relativePath, ext);
            }
        }

        public static string GetStreamingAssetFullPath(string relativePath, string ext = null)
        {
            if (string.IsNullOrWhiteSpace(ext))
            {
                return string.Format("{0}/{1}/{2}", streamingAssetsPath, sAssetBundleDir, relativePath);
            }
            else
            {
                return string.Format("{0}/{1}/{2}{3}", streamingAssetsPath, sAssetBundleDir, relativePath, ext);
            }
        }

        public static string GetPersistentFullPath(string relativePath)
        {
            return string.Format("{0}/{1}", persistentDataPath, relativePath);
        }

        public static string GetStreamingFullPath(string relativePath)
        {
            return string.Format("{0}/{1}", streamingAssetsPath, relativePath);
        }

        public static string GetPersistentFullPathHasPrefix(string relativePath)
        {
            return string.Format("{0}{1}/{2}", sPersistentPrefix, persistentDataPath, relativePath);
        }

        public static string GetStreamingFullPathHasPrefix(string relativePath)
        {
            return string.Format("{0}{1}/{2}", sStreamPrefix, streamingAssetsPath, relativePath);
        }


        public static string GetConfigFullPath(string relativePath)
        {
            string path = string.Empty;
#if UNITY_EDITOR
            path = string.Format("{0}/{1}", Application.dataPath, relativePath);
#else                       
            if (VersionHelper.eHotFixMode != EHotFixMode.Close)
            {
                path = AssetPath.GetPersistentFullPath(relativePath);
                if (!File.Exists(path))
                {
                    path = AssetPath.GetStreamingFullPath(relativePath);
                }
            }
            else
            {
                path = AssetPath.GetStreamingFullPath(relativePath);
            }
#endif
            return path;
        }

        public static string GetConfigFullPathHasPrefix(string relativePath)
        {
            string path = string.Empty;
#if UNITY_EDITOR
            path = string.Format("{0}/{1}", Application.dataPath, relativePath);
#else
            if (VersionHelper.eHotFixMode != EHotFixMode.Close)
            {
                path = AssetPath.GetPersistentFullPath(relativePath);
                if (!File.Exists(path))
                {
                    path = AssetPath.GetStreamingFullPathHasPrefix(relativePath);
                }
                else
                {
                    path = sPersistentPrefix + path;
                }
            }
            else
            {
                path = AssetPath.GetStreamingFullPathHasPrefix(relativePath);
            }
#endif
            return path;
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public static string GetVideoFullUrl(string relativePath)
        {
#if UNITY_ANDROID
            return GetConfigFullPath(relativePath);
#else
            return GetConfigFullPathHasPrefix(relativePath);
#endif
        }
    }
}
