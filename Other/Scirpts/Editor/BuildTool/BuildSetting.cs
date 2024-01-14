using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Lib;
using Framework;

public enum ERuntimeMode
{
    Normal = 0,
    MonoReflect = 1,
    HybridCLR =2,
    //ILRuntime = 2,
}


public enum ApkMode
{
    NoSDK = 0,
    UseSDK = 1,
}

public enum ApkProtectModel
{
    NoProtect = 0,
    UseProtect = 1,
}


[System.Serializable]
public class AssetBundleSettingCell
{
    public AssetBundleSettingCell(SearchOption searchOption, string relativePath, string filter, string dependencyFilter, string overrideName = null)
    {
        eSearchOption = searchOption;
        sRelativePath = relativePath;
        sFilter = filter;
        sDependencyFilter = dependencyFilter;
        sOverrideName = overrideName;
    }    

    public string sRelativePath;
    public string sFilter;
    public string sDependencyFilter;
    public string sOverrideName;
    public SearchOption eSearchOption;    

    public bool bSelected = true;
}

public class BuildSetting : ScriptableObject
{
    [MenuItem("Assets/BuildSetting")]
    public static void CreateBuildSetting()
    {
        DefaultAsset[] selects = Selection.GetFiltered<DefaultAsset>(SelectionMode.Assets);
        DefaultAsset floder = selects[0];
        string path = floder != null ? AssetDatabase.GetAssetPath(floder) : "Assets";
        CreateBuildSetting(path, "BuildSetting");
    }

    public static BuildSetting CreateBuildSetting(string path, string name)
    {
        UnityEngine.Debug.Log(path);
        BuildSetting buildSetting = ScriptableObject.CreateInstance<BuildSetting>();

        buildSetting.Reset();
        buildSetting.mBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        buildSetting.sAppPublishPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "APPPublish");

        AssetDatabase.CreateAsset(buildSetting, string.Format("{0}/{1}.asset", path, name));

        return buildSetting;
    }

    public AndroidBuildType mAndroidBuildType = AndroidBuildType.Release;
    public XcodeBuildConfig miOSBuildType = XcodeBuildConfig.Release;

    public EHotFixMode eHotFixMode = EHotFixMode.Close;
    public string sAssetVersion = "0.0.0";
    public string sVersionUrl = null;
    public string sSvnRevision = "0";

    public string sAppVersion = "0.0.0";
    public string sVersionCode = "0";
    public int sChannelId = 0;
    public int sLastChannelId = 0;

    public BuildTarget mBuildTarget = BuildTarget.StandaloneWindows;
    public BuildOptions mBuildOptions = BuildOptions.None;
    public bool bBuildOptions = false;
    public bool bDeepProfilerOption = false;

   // public BuildAssetBundleOptions mBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.DisableWriteTypeTree;
    public ScriptingImplementation eScriptingImplementation = ScriptingImplementation.IL2CPP;


    public bool bUseProfileEvent = false;
    public bool bUseDebugMode = false;
    public ERuntimeMode eRuntimeMode = ERuntimeMode.Normal;

    public bool bClearCache = true;//先设置默认勾选
    public bool bRebuildGroup = true;
    public bool bGenAssetBundle = false;
    public bool bGenCsv = false;
    public bool bGenLogic = false;
    public bool bUseLocalGenBundle = true;
    public bool bUseProtocal_noEncrypt = false;
    public bool bUseQRcode = false;
    public bool bGmPropagateVersion = false;
    public bool bUseSplitPack = false;

#if UNITY_ANDROID || UNITY_STANDALONE_WIN
    public ApkMode eApkMode = ApkMode.NoSDK;
#else
    public ApkMode eApkMode = ApkMode.NoSDK;
#endif
#if UNITY_STANDALONE_WIN
    public ApkProtectModel eUseACEAnti = ApkProtectModel.NoProtect;
#endif


    public bool bBuildApp = false;
    public string sAppPublishPath;

    public bool bGenHotFix = false;

    public List<AssetBundleSettingCell> mAssetBundleSettingCells;

    public void Reset()
    {
        bGenAssetBundle = true;
        bGenCsv = true;
        bGenLogic = true;

        bBuildApp = true;
        bGenHotFix = true;
        //bCopyABToStreamPath = true;
        //bCopyConfigToStreamPath = true;
        //bCopyLogicToStreamPath = true;
        bUseDebugMode = false;
        bBuildOptions = false;
        bDeepProfilerOption = false;
        bUseProfileEvent = false;
        //bIncreasePacking = false; 暂时先不设置
        //mBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.DisableWriteTypeTree;

        bUseProtocal_noEncrypt = false;
        bUseQRcode = false;

#if UNITY_ANDROID || UNITY_STANDALONE_WIN ||UNITY_IOS
        eApkMode = ApkMode.UseSDK;
#else
        eApkMode = ApkMode.NoSDK;
#endif


        //eRuntimeMode = ERuntimeMode.ILRuntime; todo -yd
        eScriptingImplementation = ScriptingImplementation.IL2CPP;

        mAssetBundleSettingCells = AddressableBundleGroupSetting.AddAssetBundleSettingCell();
    }

    public void Release()
    {
        //Reset();
        bUseDebugMode = false;

#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        eApkMode = ApkMode.UseSDK;
#else
        eApkMode = ApkMode.NoSDK;
#endif
        //eRuntimeMode = ERuntimeMode.ILRuntime; todo -yd
        mBuildOptions = BuildOptions.None;
        eScriptingImplementation = ScriptingImplementation.IL2CPP;
        if (eHotFixMode != EHotFixMode.Close)
            SaveHotFixBaseAsset();
    }

    public void Debug()
    {
        //Reset();
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        eApkMode = ApkMode.UseSDK;
#else
        eApkMode = ApkMode.NoSDK;
#endif
        //eRuntimeMode = ERuntimeMode.ILRuntime; todo -yd
        eScriptingImplementation = ScriptingImplementation.IL2CPP; 
        bBuildOptions = true;
        mBuildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler;
        bUseDebugMode = true;
        if (eHotFixMode != EHotFixMode.Close)
            SaveHotFixBaseAsset();
    }

    public void HotFix()
    {
        //Reset();
        bUseDebugMode = true;

        bGenCsv = true;
        bGenLogic = true;
        bGenAssetBundle = true;

        bBuildApp = false;
        bGenHotFix = true;
    }

    public void SaveHotFixBaseAsset()
    {
        string minimalVersion = VersionHelper.GetMinimalVersion(sAssetVersion);
        int curVersion = 0;
        if (int.TryParse(minimalVersion, out curVersion))
        {
            if (curVersion <= 0)
            {
                bGenHotFix = true;
            }
            else
            {
                bGenHotFix = false;
            }
        }
    }
}