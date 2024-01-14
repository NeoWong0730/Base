using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine.AddressableAssets;
using System;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

public static class TestCompileForBeforeBuild
{
#if !(UNITY_ANDROID || UNITY_IPHONE)  
    private const string selectABModeMenuPath = "__Tools__/打包相关/加载资源(使用AB)";
    [MenuItem(selectABModeMenuPath, priority =0 )]
    internal static void SetLoadLocalBundleMode()
    {
        bool bSelected = Menu.GetChecked(selectABModeMenuPath);
        Menu.SetChecked(selectABModeMenuPath,!bSelected);
        EditorPrefs.SetBool(StartupUseLocalBundle.bUseLocalGenBundle, !bSelected);
        StartupUseLocalBundle.AddSharePlayModeToDataBuilder(EditorPrefs.GetBool(StartupUseLocalBundle.bUseLocalGenBundle));
    }

    [MenuItem(selectABModeMenuPath, true)]
    public static bool SetLoadLocalBundleMode_Check_Menu()
    {
        Menu.SetChecked(selectABModeMenuPath, EditorPrefs.GetBool(StartupUseLocalBundle.bUseLocalGenBundle,true));
        return true;
    }
#endif



    [MenuItem("__Tools__/打包相关/打包之前测试脚本是否能编译成功")]
    internal static void BuildPlayerScriptsOnly()
    {
        BuildTarget targetPlatform =  EditorUserBuildSettings.activeBuildTarget;
        var targetGroup = BuildPipeline.GetBuildTargetGroup(targetPlatform);
        ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings()
        {
            target = targetPlatform,
            group = targetGroup,
            options = ScriptCompilationOptions.None
        };

        string TempOutputFolder = Path.GetDirectoryName(Application.dataPath) + "/AppScriptDll";
        if (Directory.Exists(TempOutputFolder))
        {
            Directory.Delete(TempOutputFolder, true);
        }
        Directory.CreateDirectory(TempOutputFolder);


        ScriptCompilationResult scriptCompilationResult = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, TempOutputFolder);

        if (scriptCompilationResult.assemblies.Count == 0)
        {
            Debug.LogError("脚本编译失败");
        }
        else
        {
            Debug.Log("脚本编译成功:"+ scriptCompilationResult.assemblies.Count);
        }

        //删除零时文件
        if (Directory.Exists(TempOutputFolder))
        {
            Directory.Delete(TempOutputFolder, true);
        }
    }

    [MenuItem("__Tools__/打包相关/打开Library下aa")]
    public static void openLibrary()
    {
        string path = Application.dataPath.Replace("/Assets", "/") + Addressables.RuntimePath;
        Application.OpenURL(path);
    }



    [MenuItem("__Tools__/打包相关/渠道包删除jaraar")]
    public static void QuDaoDeleteLib()
    {
        string libPath = string.Format("{0}/../../Android/{1}/{2}", Application.dataPath, "unityLibrary", "libs");
        string copyPath = string.Format("{0}/../../Android/tempFile", Application.dataPath);

        List<string> tempArr = new List<string>();
        //渠道包 基础sdk 需要删除以下lib库中的文件 
        tempArr.Add("/alipaySdk-20180601.jar");
        tempArr.Add("/basewx-1.31.5.aar");
        tempArr.Add("/kwaisdk-1.31.5.aar");
        tempArr.Add("/kwaisdk_qq-1.31.5.aar");
        tempArr.Add("/kwaisdk_wechat-1.31.5.aar");
        tempArr.Add("/open_sdk_lite-0.0.2.jar");
        tempArr.Add("/wechat-sdk-android-without-mta-6.7.9.jar");

        //删除文件
        foreach (var item in tempArr)
        {
            File.Delete(libPath + item);
        }
    }


    [MenuItem("__Tools__/打包相关/打开沙盒路径")]
    public static void OpenPersistentPath()
    {
        Application.OpenURL(Application.persistentDataPath);
    }


    [MenuItem("__Tools__/打包相关/打开缓存路径")]
    public static void OpenCachePath()
    {
        //Application.OpenURL(Application.temporaryCachePath);
        Application.OpenURL(Caching.defaultCache.path);
    }

    [MenuItem("__Tools__/打包相关/打开项目路径")]
    public static void OpenProjectPath()
    {
        Application.OpenURL(Application.dataPath.Replace("/Assets", "/"));
    }

    [MenuItem("__Tools__/打包相关/打印Group中的词条列表")]
    public static void PfrintGroupAssetEntry()
    {
        string OriCatalogFile = string.Format("{0}/{1}/{2}/{3}.json", Application.dataPath, BuildTool.AddressableServerDataPath, PlatformMappingService.GetPlatformPathSubFolder(), "catalog_1.8.0");
        //string OriCatalogFile = string.Format("{0}/{1}{2}", remoteBuildCatalog, catalogName, ".json");
        string DesCatalogFile = string.Format(@"D:\Projects\crossgate\client_android\CrossGate\AssetPublish\Android\1\aa\catalog_1.8.0.json");
        //File.Copy(OriCatalogFile, DesCatalogFile);
        File.Move(OriCatalogFile, DesCatalogFile);
        //BuildTool.PrintGroupAssetEntryLogFile("Current");
    }

    //[MenuItem("__Tools__/打包相关/打印需要更新的Bundle")]
    //public static void PfrintNewUpdateBundleAssetEntry()
    //{
    //    //获取热更文件
    //    string needUpdatefile = string.Format("{0}/{1}", BuildTool.HotFixBinaryFile, "NeedUpdateList.bytes");

    //    Dictionary<string, List<string>> needMoveFiles = null;
    //    System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

    //    if (File.Exists(needUpdatefile))
    //        needMoveFiles = BuildTool.ReadBinary(needUpdatefile) as Dictionary<string, List<string>>;
    //    if (needMoveFiles != null && needMoveFiles.Count > 0)
    //    {
    //        //打印日志
    //        BuildTool.DebugBundleChangeAssetEntryLogFile(needMoveFiles, BuildTool.HotFixBinaryFile);
    //    }

    //}



    [MenuItem("__Tools__/打包相关/打开输出日志路径")]
    public static void OpenConsoleLogPath()
    {
        string settingsPath = string.Format("{0}/{1}/settings.json", Application.streamingAssetsPath, Lib.AssetLoader.AssetPath.sAddressableDir);
        var Runtimedata = JsonUtility.FromJson<UnityEngine.AddressableAssets.Initialization.ResourceManagerRuntimeData>(File.ReadAllText(settingsPath));
        if (Runtimedata != null)
        {
            Debug.LogError(Runtimedata.BuildTarget);
        } 



        //Debug.Log(AssetDatabase.GetMainAssetTypeAtPath("Assets/ResourcesAB/AnimationClip/n_02/11011_show_Cutscene_000_01_01.anim"));
        //Debug.Log(AssetDatabase.GetMainAssetTypeAtPath("Assets/ResourcesAB/Atlas/BagUIAtlas.spriteatlas"));
        //Debug.Log(AssetDatabase.GetMainAssetTypeAtPath("Assets/ResourcesAB/Audio/UI/cgsys05.wav"));
        //Debug.Log(AssetDatabase.GetMainAssetTypeAtPath("Assets/ResourcesAB/Font/Special_font.fontsettings"));
        //Debug.Log(AssetDatabase.GetMainAssetTypeAtPath("Assets/ResourcesAB/Font/zhanghaishanruixian.ttf"));
        //Debug.Log(AssetDatabase.GetMainAssetTypeAtPath("Assets/ResourcesAB/Shader/CustomGhostTime.shader"));
        //Debug.Log(AssetDatabase.GetMainAssetTypeAtPath("Assets/ResourcesAB/Emoji/emoji.asset"));
        //Debug.Log(AssetDatabase.GetMainAssetTypeAtPath("Assets/Shader/ShaderGraph/DissolveSmooth.shadergraph"));


        //string[] ss = AssetDatabase.GetDependencies("Assets/GameObject 1.prefab", false);
        //for(int i = 0; i < ss.Length; ++i)
        //{
        //    if(ss[i].EndsWith(".FBX", StringComparison.OrdinalIgnoreCase))
        //    {
        //        Debug.LogError(ss[i]);
        //    }
        //    else
        //    {
        //        Debug.Log(ss[i]);

        //    }
        //}




        // ss = AssetDatabase.GetDependencies("Assets/ResourcesAB/Prefab/Char/Cin_01.prefab", false);
        // for (int i = 0; i < ss.Length; ++i)
        // {
        //     Debug.LogWarning(ss[i]);
        // }
        return;

        //Application.OpenURL(Application.consoleLogPath);
        // string prefabPath = @"D:\Projects\crossgate\client_android\CrossGate\Assets\ResourcesAB\Prefab\Char\Li_01.prefab";
        // string prefabPath = "Assets/Arts/Charactor/NPC/3901/Materials/3901_show_clothes_s_01.tga";
        // prefabPath = AssetDatabase.GetAssetPath();

        //string[] ss = AssetDatabase.GetDependencies(prefabPath);


        //for (int i = 0; i < ss.Length; i++)
        //{
        //    Type type = AssetDatabase.GetMainAssetTypeAtPath(ss[i]);
        //    Debug.LogError(ss[i]);
        //    Debug.LogError(type);
        //}

        //Type type = AssetDatabase.GetMainAssetTypeAtPath(prefabPath);
        //Debug.LogError(type);

        int totalBundleCount = 0;
        AddressableAssetSettings assetSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
        for (int i = 0; i < assetSettings.groups.Count; i++)
        {
            int bundleCount = 0;
            AddressableAssetGroup assetGroup = assetSettings.groups[i];
            BundledAssetGroupSchema bundledAssetGroupSchema = assetGroup.GetSchema<BundledAssetGroupSchema>();
            if (string.Equals(assetGroup.Name, "Default Local Group", StringComparison.Ordinal))
            {
                bundleCount = 1;
            }


            if (bundledAssetGroupSchema.BundleMode == BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel)
            {
//                bundledAssetGroupSchema.
                //Debug.LogFormat(string.Format("{0} PackSeparately AssetEntryCount:{1}  BundleCount:{2}", assetGroup.Name, assetGroup.entries.Count, bundleCount));
            }
            else
            {
                Debug.LogFormat(string.Format("{0} PackSeparately AssetEntryCount:{1}  BundleCount:{2}", assetGroup.Name, assetGroup.entries.Count , bundleCount));
            }

            totalBundleCount += bundleCount;

        }


    }
    //[MenuItem("__Tools__/打包相关/打开AddressableBundle资源下载目录")]
    //public static void OpenAddressableBundlePath()
    //{
    //    string AddressableBundlePath = "C:/Users/Taren/AppData/LocalLow/Unity/TaRenWang_魔力宝贝";
    //    Application.OpenURL(AddressableBundlePath);
    //}
}
