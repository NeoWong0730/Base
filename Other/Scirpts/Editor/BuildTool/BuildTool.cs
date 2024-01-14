
using Framework;
using Lib;
using Lib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Player;
using UnityEditor.Compilation;
//using UnityEditor.Rendering;//ToDo 后期优化
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.U2D;
using static AppSetting;

public static class BuildTool
{
    private class BuildArgs
    {
        //public string BuildSettingPath;
        //public string OutputPath; 已定义
        public string Vision;
        //public bool DebugMode;
        public string buildType;
        public string revision;
    }


    public enum EAssetBundleMode : int
    {
        eConfigDll = 1,
        eAaBundle = 2
    }

    public enum EBundleRecordTxt : int
    {
        eNewGenBundleTxt,
        eNeedUpdateBundleTxt,
    }


    [Serializable]
    class BundleListRes
    {
        [SerializeField]
        public List<BundleRes> bundleListRes = null;
    }

    [Serializable]
    class BundleRes
    {
        [SerializeField]
        public string bundleFileId;

        [SerializeField]
        public string hashOfFileName;

        [SerializeField]
        public string hashOfBundle;

        [SerializeField]
        public string bundleName;

        [SerializeField]
        public List<string> mainAssets = null;
    }

    public static string AssetBundleEncryptPath = "../AssetEncrypt";
    public static string AssetBundleBuildPath = "../AssetBundle";
    public static string AssetBundleS0Name = "S0";
    public static string AssetBundlePublishPath = "../AssetPublish";
    public static string AddressableOriPath = "../Library/com.unity.addressables";
    public static string AddressableServerDataPath = "../ServerData";
    public static string HotFixBinaryFilePath = "../BinaryFile";

    public static string AtlasRoot = "Assets/Projects/Image";
    public static string ScriptRoot = "Assets/Scripts";
    public static string DependsRoot = "Depends";

    public static string AddressableBin = "AddressableBin";
    public static string AaPublishDirName = "AssetPublish";


    public static bool SetFORCE_AB = false;
    public static bool bStopBuild = false;


    public static bool USE_PROFILE_MODE = false;

    public static int Build(BuildSetting buildSetting)
    {
        System.Diagnostics.Stopwatch swP = new System.Diagnostics.Stopwatch();
        swP.Start();

        Debug.ClearDeveloperConsole();
        VersionSetting versionSetting = SetVersion(buildSetting);
        EditorUtility.SetDirty(versionSetting);
        EditorUtility.SetDirty(buildSetting);
        AssetDatabase.SaveAssets();

        PrintBuildSetting(buildSetting);

        BuildTarget buildTarget = buildSetting.mBuildTarget;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildSetting.mBuildTarget);


        bool genAssetBundle = buildSetting.bGenAssetBundle;
        bool genCsv = buildSetting.bGenCsv;
        bool genLogic = buildSetting.bGenLogic;
        bool genApp = buildSetting.bBuildApp;
        bool genHotFix = buildSetting.bGenHotFix;

        bool useBuildOption = buildSetting.bBuildOptions;
        bool useDebugMode = buildSetting.bUseDebugMode;
        bool useProfileEvent = buildSetting.bUseProfileEvent;
        BuildOptions buildOptions = buildSetting.mBuildOptions;
        bool useSplitPack = buildSetting.bUseSplitPack;


        string version = buildSetting.sAssetVersion;
        string maximalVersion = VersionHelper.GetMaximalVersion(version);
        string minimalVersion = VersionHelper.GetMinimalVersion(version);

        bool clearAssetStreamPath = genCsv && genLogic && genAssetBundle && genApp;

        ERuntimeMode eRuntimeMode = buildSetting.eRuntimeMode;
        


        if (string.IsNullOrWhiteSpace(maximalVersion) || string.IsNullOrWhiteSpace(minimalVersion))
        {
            Debug.LogErrorFormat("打包失败：版本格式错误 version = {0}", version);
            return 5;
        }

        if (!int.TryParse(minimalVersion, out int curVersion))
        {
            Debug.LogErrorFormat("打包失败：小版本格式错误 version = {0}", version);
            return 5;
        }

        if (CompilePlayerScripts(buildSetting) != 0)
        {
            Debug.LogError("打包失败：脚本编译不成功，请检查代码！");
            return 5;
        }

        version = FixVersionAndBackup(versionSetting, buildSetting, genHotFix);

        if (GetAppOutPath(buildSetting, out string appFilePath) != 0)
        {
            Debug.LogError("打包失败: 获取App的输出路径出错！");
            return 5;
        }

#if UNITY_ANDROID
        PlayerSettings.bundleVersion = buildSetting.sAppVersion;
        int.TryParse(buildSetting.sVersionCode, out int versioncode);
        PlayerSettings.Android.bundleVersionCode = versioncode;
#elif UNITY_IOS
        PlayerSettings.bundleVersion = buildSetting.sAppVersion;
        PlayerSettings.iOS.buildNumber = buildSetting.sVersionCode;
#endif

        int rlt = 0;

        string defineStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        BuildTool.StartBuild(buildTarget, genAssetBundle);

        if (clearAssetStreamPath && rlt == 0)
        {
            Debug.LogFormat("Build -> Start ClearAssetSteamPath()");
            rlt = ClearAssetSteamPath();
            Debug.LogFormat("Build -> End ClearAssetSteamPath()");
        }

        if (genCsv && rlt == 0)
        {
            Debug.LogFormat("Build -> Start GenConfig({0})", buildTarget.ToString());
            rlt = Build_Config.GenConfig(buildTarget);
            Debug.LogFormat("Build -> End GenConfig({0})", buildTarget.ToString());
        }

        if (genLogic && rlt == 0)
        {
            Debug.LogFormat("Build -> Start GenLogic({0}, {1})", buildTarget.ToString(), buildOptions.ToString());
            rlt = Build_Logic.GenLogic(buildTarget, buildOptions, useDebugMode);
            Debug.LogFormat("Build -> End GenLogic({0}, {1})", buildTarget.ToString(), buildOptions.ToString());
        }

        if (genAssetBundle && rlt == 0)
        {
            Debug.LogFormat("Build -> Start AssetBundle({0})", buildTarget.ToString());
            rlt = Build_AssetBundle.GenAssetBundle(buildSetting, versionSetting);
            Debug.LogFormat("Build -> End AssetBundle({0})", buildTarget.ToString());
        }

        if (genApp && rlt == 0)
        {
            Debug.LogFormat("Build -> Start BuildPlayer({0}, {1}, {2})", appFilePath, buildTarget.ToString(), buildOptions.ToString());
            rlt = SetPackageIdentifier(versionSetting, buildTarget);
            PlayerSettings.SetScriptingBackend(BuildPipeline.GetBuildTargetGroup(buildTarget), buildSetting.eScriptingImplementation);
            SetStackTraceLogType(buildSetting.bUseDebugMode);
            FirstPackSplitAssetHandle(buildSetting);

#if UNITY_ANDROID
            //android 平台只有正式版生成符号表
            EditorUserBuildSettings.androidCreateSymbols = buildSetting.bUseDebugMode ? AndroidCreateSymbols.Disabled: AndroidCreateSymbols.Public;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = (buildSetting.eApkMode == ApkMode.UseSDK) ? true : false;
#endif
            BuildPipeline.BuildPlayer(new string[] { "Assets/ResourcesAB/GameStart.unity" }, appFilePath, buildTarget, buildOptions);
            Debug.LogFormat("Build -> End BuildPlayer({0}, {1}, {2})", appFilePath, buildTarget.ToString(), buildOptions.ToString());
        }

        if (genHotFix && rlt == 0)
        {
            Debug.LogFormat("Build -> Start GenHotFixAsset({0}, {1})", buildTarget.ToString(), version);
            rlt = GenHotFixAsset(buildSetting);
            Debug.LogFormat("Build -> End GenHotFixAsset({0}, {1})", buildTarget.ToString(), version);
        }

        BuildTool.EndBuild(EditorUserBuildSettings.activeBuildTarget, genAssetBundle, false);

        SetStackTraceLogType(true);
        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();

        swP.Stop();
        TimeSpan ts = swP.Elapsed;
        Debug.LogError(string.Format("打包一共消耗时间:{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds));

        return rlt;
    }


    #region 生成资源

    public static void FirstPackSplitAssetHandle(BuildSetting buildSetting)
    {
        if (!buildSetting.bUseSplitPack)
            return;

        string platform = Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget);
        
        //获取最新的 CatalogData
        string oriCatalogPath = string.Format("{0}/{1}/{2}/{3}/catalog.json", Application.dataPath, BuildTool.AssetBundleBuildPath, platform, "aa");
        ContentCatalogData contentCatalogData = JsonUtility.FromJson<ContentCatalogData>(File.ReadAllText(oriCatalogPath));
        if (contentCatalogData == null || contentCatalogData.InternalIds == null || contentCatalogData.InternalIds.Length <= 0)
        {
            Debug.LogError("contentCatalogData == null");
            return;
        }


        AssetList assetList = GenS0AssetList();
        if (assetList == null)
        {
            Debug.LogError("S0AssetList == null");
            return;
        }


        AssetList s0AssetList = new AssetList();
        s0AssetList.Version = buildSetting.sAppVersion;


        long TotalSize = 0;
        long MegaByte900 = 1048576 * 900;
        string tempFileName;
        string orignalInternalName;
        string tempInternalName;
        
        string srcBundlePath = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, BuildTool.AssetBundleBuildPath, platform, "aa", "Android");
        string dstS0AssetPath = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, BuildTool.AssetBundleBuildPath, BuildTool.AssetBundleS0Name, "aa", "Android");
        FileTool.CreateFolder(dstS0AssetPath);

        //以900M 为界限进行资源划分，目前主要是用于测试
        DirectoryInfo dir = new DirectoryInfo(srcBundlePath);
        FileInfo[] fileInfos = dir.GetFiles();
        for (int i = 0; i < fileInfos.Length; ++i)
        {
            if (TotalSize > MegaByte900)
            {
                //1.1 copy to splitpack(剩余资源)
                FileInfo fileInfo = fileInfos[i];
                string desTemp = string.Format("{0}/{1}", dstS0AssetPath, fileInfo.Name);
                File.Copy(fileInfo.FullName, desTemp, true);

                //1.2 记录列表
                string assetName = fileInfo.Name.Replace(".bundle", "");
                if (assetList.Contents.ContainsKey(assetName))
                    s0AssetList.Contents.Add(assetName, assetList.Contents[assetName]);

                //1.3 修改catalog
                tempFileName = string.Format("{0}/{1}/{2}", "{UnityEngine.AddressableAssets.Addressables.RuntimePath}", platform, fileInfo.Name);
                for (int j = 0; j < contentCatalogData.InternalIds.Length; j++)
                {
                    orignalInternalName = contentCatalogData.InternalIds[j];
                    if (!orignalInternalName.EndsWith(".bundle"))
                        continue;
                    tempInternalName = orignalInternalName.Replace("\\", "/");
                    if (tempInternalName.Equals(tempFileName, StringComparison.Ordinal))
                    {
                        contentCatalogData.InternalIds[j] = orignalInternalName.Replace("{UnityEngine.AddressableAssets.Addressables.RuntimePath}", "{UnityEngine.AddressableAssets.PathRebuild.SplitPackLoadPath}");
                        break;
                    }
                }
            }
            else
            {
                FileInfo fileInfo = fileInfos[i];
                //string desTemp = string.Format("{0}/{1}", dstAaAndroidPath, fileInfo.Name);
                //File.Copy(fileInfo.FullName, desTemp, true);
                TotalSize += fileInfo.Length;
            }
        }

        //写回纪录列表
        string curS0Path = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, BuildTool.AssetBundleBuildPath, BuildTool.AssetBundleS0Name, AssetPath.sFirstPackListName);
        string s0Content = AssetList.Serialize(s0AssetList);
        BuildTool.WriteFile(curS0Path, s0Content);


        //回写 catalog.json
        string desS0CatalogPath = string.Format("{0}/{1}/{2}/aa/catalog.json", Application.dataPath, BuildTool.AssetBundleBuildPath, BuildTool.AssetBundleS0Name);
        File.WriteAllText(desS0CatalogPath, JsonUtility.ToJson(contentCatalogData));
    }

    #endregion


    #region 生成热更新资源
    private static int GenHotFixAsset(BuildSetting buildsettings)
    {
        int rlt = 0;
        string title = string.Format("生成热更新资源 {0} {1}", buildsettings.mBuildTarget.ToString(), buildsettings.sAssetVersion);
        string maximalVersion = VersionHelper.GetMaximalVersion(buildsettings.sAssetVersion);
        string minimalVersion = VersionHelper.GetMinimalVersion(buildsettings.sAssetVersion);

        int curVersion = 0;
        int.TryParse(minimalVersion, out curVersion);

        //写入同一版本系列热更文件
        string HotFixUniqueIdentifierPath = string.Format("{0}/{1}", Build_AssetBundle.HotFixBinaryFile, "HotFixUniqueIdentifier.txt");
        if (!File.Exists(HotFixUniqueIdentifierPath))
        {
            Debug.LogError("不存在热更标识符文件:" + HotFixUniqueIdentifierPath);
            return 5;
        }


        string platformName = Enum.GetName(typeof(BuildTarget), buildsettings.mBuildTarget);
        string bundleDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundleBuildPath, platformName);
        string curDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion);

        AssetList curConfigList = GenAssetsList(buildsettings.mBuildTarget, bundleDir);
        AssetList curAaList = GenAaAssetList(platformName);

        curAaList.Version = curConfigList.Version = string.Format("{0}.{1}", maximalVersion, minimalVersion);
        curAaList.VersionIdentifier = curConfigList.VersionIdentifier = File.ReadAllText(HotFixUniqueIdentifierPath);

        if (curVersion <= 0)
        {
            //直接拷贝到0
            rlt = CopyAssetTo0(buildsettings);
        }
        else
        {
            //热更(Config/Dll)文件生成
            AssetList curHotFixConfigList = GenHotFixConfigDll(buildsettings, curConfigList);

            //热更(AaBundle)文件生成
            AssetList curAaUpdateList = GenHotFixBundle(buildsettings, curAaList);

            //合并成一个HotFixList写入
            AssetList HotFixList = CombineHotFixToOne(buildsettings, curHotFixConfigList, curAaUpdateList);
        }

        //写入当前的AssetList(Config、dll)文件
        string curAssetListPath = string.Format("{0}/{1}/{2}/{3}/ConfigAssetList.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion.ToString());
        WriteFile(curAssetListPath, AssetList.Serialize(curConfigList));


        //写入当前的AssetList(AaBundle)文件
        string curAaAssetListPath = string.Format("{0}/{1}/{2}/{3}/AaAssetList.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion.ToString());
        WriteFile(curAaAssetListPath, AssetList.Serialize(curAaList));


        //写入自己记录的Bundle列表NewBundleList
        string fileOriPath = string.Format("{0}/{1}", Build_AssetBundle.HotFixBinaryFile, "NewBundleList.json");
        string fileDesPath = string.Format("{0}/{1}/{2}/{3}/NewBundleList.json", Application.dataPath, AssetBundlePublishPath, platformName, curVersion);
        File.Copy(fileOriPath, fileDesPath, true);


        //记录 热更版本文件（标志着热更新资源成功与否:管哥要求）
        string curVersionPath = string.Format("{0}/{1}/{2}/Version.txt", Application.dataPath, AssetBundlePublishPath, platformName);
        File.WriteAllText(curVersionPath, minimalVersion);

        return rlt;
    }

    public static AssetList GenHotFixConfigDll(BuildSetting buildSetting, AssetList curConfigList)
    {
        int rlt = 0;

        AssetList curHotFixConfigList = null;

        //热更(Config/Dll)文件生成
        string maximalVersion = VersionHelper.GetMaximalVersion(buildSetting.sAssetVersion);
        string minimalVersion = VersionHelper.GetMinimalVersion(buildSetting.sAssetVersion);
        int.TryParse(minimalVersion, out int curVersion);
        string platformName = Enum.GetName(typeof(BuildTarget), buildSetting.mBuildTarget);


        string bundleDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundleBuildPath, platformName);
        string curDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundlePublishPath, platformName, curVersion);

        bool versionIdentifierIsSame = true;
        int befVersion = curVersion - 1;
        for (; befVersion >= 0; --befVersion)
        {
            string befAssetListPath = string.Format("{0}/{1}/{2}/{3}/ConfigAssetList.txt", Application.dataPath, AssetBundlePublishPath, platformName, befVersion.ToString());
            if (File.Exists(befAssetListPath))
            {
                string befAssetListContent = File.ReadAllText(befAssetListPath);
                AssetList befAssetList = AssetList.Deserialize(befAssetListContent);
                curHotFixConfigList = CompareAssetsList(curConfigList, befAssetList, curVersion, null, ref versionIdentifierIsSame, "Config/Dll");
                break;
            }
        }

        if (versionIdentifierIsSame == false)
        {
            //return -5;
        }


        string befHotFixPath = string.Format("{0}/{1}/{2}/{3}/HotFixConfigList.txt", Application.dataPath, AssetBundlePublishPath, platformName, befVersion.ToString());
        AssetList oldHotFixList;
        if (File.Exists(befHotFixPath))
        {
            string befHotFixContent = File.ReadAllText(befHotFixPath);
            oldHotFixList = AssetList.Deserialize(befHotFixContent);
            curHotFixConfigList = MergeChangeList(curHotFixConfigList, oldHotFixList);
        }

        int i = 0;
        int count = curHotFixConfigList.Contents.Count;
        foreach (AssetsInfo info in curHotFixConfigList.Contents.Values)
        {
            if (info.State == 0)
            {
                ++i;
                string srcPath = string.Format("{0}/{1}", bundleDir, info.AssetName);
                string desPath = string.Format("{0}/{1}", curDir, info.AssetName);
                FrameworkTool.CopyFile(srcPath, desPath, true);

                if (EditorUtility.DisplayCancelableProgressBar("拷贝热更新资源 (config、dll)", string.Format("({0}/{1})\n{2}\n{3}", i.ToString(), count.ToString(), srcPath, desPath), (float)i / (float)count))
                {
                    rlt = 1;
                    break;
                }
            }
        }

        curHotFixConfigList.Version = string.Format("{0}.{1}", maximalVersion, minimalVersion);
        curHotFixConfigList.VersionIdentifier = curConfigList.VersionIdentifier;
        string curHotFixPath1 = string.Format("{0}/{1}/{2}/{3}/HotFixConfigList.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion.ToString());
        string hotfixcontent1 = AssetList.Serialize(curHotFixConfigList);
        WriteFile(curHotFixPath1, hotfixcontent1);
        return curHotFixConfigList;
    }

    public static AssetList GenHotFixBundle(BuildSetting buildSetting, AssetList curBundleList)
    {
        //热更(AaBundle)文件生成
        int rlt = 0;

        AssetList curHotFixList = null;

        //热更Bundle文件生成
        string maximalVersion = VersionHelper.GetMaximalVersion(buildSetting.sAssetVersion);
        string minimalVersion = VersionHelper.GetMinimalVersion(buildSetting.sAssetVersion);
        int.TryParse(minimalVersion, out int curVersion);
        string platformName = Enum.GetName(typeof(BuildTarget), buildSetting.mBuildTarget);


        #region 通过原始比较方法获取热更列表
        bool versionIdentifierIsSame = true;
        int befVersion = curVersion - 1;
        for (; befVersion >= 0; --befVersion)
        {
            string befAssetListPath = string.Format("{0}/{1}/{2}/{3}/AaAssetList.txt", Application.dataPath, AssetBundlePublishPath, platformName, befVersion.ToString());
            if (File.Exists(befAssetListPath))
            {
                string befAssetListContent = File.ReadAllText(befAssetListPath);
                AssetList befAssetList = AssetList.Deserialize(befAssetListContent);
                curHotFixList = CompareAssetsList(curBundleList, befAssetList, curVersion, null, ref versionIdentifierIsSame, "Bundle");
                break;
            }
        }

        if (versionIdentifierIsSame == false)
        {
            //return -5;
        }



        string befHotFixPath = string.Format("{0}/{1}/{2}/{3}/HotFixAaList.txt", Application.dataPath, AssetBundlePublishPath, platformName, befVersion.ToString());
        AssetList oldHotFixList = null;
        if (File.Exists(befHotFixPath))
        {
            string befHotFixContent = File.ReadAllText(befHotFixPath);
            oldHotFixList = AssetList.Deserialize(befHotFixContent);
            curHotFixList = MergeChangeList(curHotFixList, oldHotFixList);
        }

        #endregion

        //bundle没有改变,不会生成热更资源
        if (curHotFixList.Contents.Count <= 0)
        {
            Debug.Log("bundle没有改变,不会生成热更资源");
            //return -5;
        }


        //获取Addresable新增热更资源
        BundleListRes needUpdateListRes = GetNewBundleListRes(EBundleRecordTxt.eNeedUpdateBundleTxt);
        if (needUpdateListRes == null || needUpdateListRes.bundleListRes == null)
        {
            Debug.LogError("NeedUpdateList.json 内容为空");
            //return -5;
        }


        //获取Addresable当前的热更资源列表
        BundleListRes curentBundleListRes = GetNewBundleListRes(EBundleRecordTxt.eNewGenBundleTxt);
        if (curentBundleListRes == null || curentBundleListRes.bundleListRes == null)
        {
            Debug.LogError("NewBundleList.json 内容为空");
            //return -5;
        }


        //创建bundle目录文件
        string DestionAaDir = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundlePublishPath, platformName, curVersion, AssetPath.sAddressableDir);
        FileTool.CreateFolder(DestionAaDir);
        string DestBundleFile = string.Format("{0}/{1}", DestionAaDir, platformName);
        FileTool.CreateFolder(DestBundleFile);

        //copy NeedUpdateList.json
        string needUpdateListOriPath = string.Format("{0}/{1}", Build_AssetBundle.HotFixBinaryFile, "NeedUpdateList.json");
        string needUpdateListDesPath = string.Format("{0}/{1}/{2}/{3}/NeedUpdateList.json", Application.dataPath, AssetBundlePublishPath, platformName, curVersion);
        File.Copy(needUpdateListOriPath, needUpdateListDesPath, true);
        //copy layout
        string layoutSrc = string.Format("{0}/{1}/buildlayout.txt", Application.dataPath, AddressableOriPath);
        string layoutDes = string.Format("{0}/{1}/{2}/{3}/buildlayout.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion);
        File.Copy(layoutSrc, layoutDes);



        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        //只修改热更资源里的Catalog.json里的本地为远端路径 --前提是Catalog.json 不能打成bundle(Catalog不会被打成Bundle)
        string remoteBuildCatalog = string.Format("{0}/{1}/{2}/catalog_{3}.json", Application.dataPath, AddressableServerDataPath, platformName, m_Settings.PlayerBuildVersion);
        ContentCatalogData contentCatalogData = JsonUtility.FromJson<ContentCatalogData>(File.ReadAllText(remoteBuildCatalog));


        //copy 热更资源bundle
        string catalogPersistName = string.Format("{0}/catalog_{1}.json", AssetPath.sAddressableCatalogDir, m_Settings.PlayerBuildVersion);
        string orignalBundlePath = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AddressableOriPath, AssetPath.sAddressableDir, PlatformMappingService.GetPlatformPathSubFolder(), platformName);

        int i = 0;
        string internalId;
        int count = curHotFixList.Contents.Count;
        int deleteAndAddResCount = 0;
        List<string> bundleChangeList = new List<string>();
        Dictionary<string, BundleRes> updateBundleResDict = needUpdateListRes.bundleListRes.ToDictionary(key => key.hashOfFileName, value => value);
        Dictionary<string, BundleRes> currentBundleResDict = curentBundleListRes.bundleListRes.ToDictionary(key => key.hashOfFileName, value => value);

        #region 最新Catalog重定向 =》分包资源

        if (buildSetting.bUseSplitPack)
        {
            //首包分离资源的列表获取
            string S0Path = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundlePublishPath, platformName, AssetBundleS0Name);
            String S0FileName = string.Format("{0}/{1}", S0Path, AssetPath.sFirstPackListName);
            AssetList S0AssetList = null;
            if (Directory.Exists(S0Path) && File.Exists(S0FileName))
            {
                string s0AssetListContent = File.ReadAllText(S0FileName);
                S0AssetList = AssetList.Deserialize(s0AssetListContent);
            }

            //重定义分包Catalog.json 首包分出去的资源又没被热更，这些需要Catalog重定向
            if (S0AssetList != null && S0AssetList.Contents != null)
            {
                string tempFileName;
                string tempOriInternalId;
                string tempInternalId;
                foreach (AssetsInfo info in S0AssetList.Contents.Values)
                {
                    tempFileName = string.Format("{0}/{1}/{2}.bundle", "{UnityEngine.AddressableAssets.Addressables.RuntimePath}", platformName, info.AssetName);
                    for (int j = 0; j < contentCatalogData.InternalIds.Length; j++)
                    {
                        tempOriInternalId = contentCatalogData.InternalIds[j];
                        if (!tempOriInternalId.EndsWith(".bundle"))
                            continue;
                        tempInternalId = tempOriInternalId.Replace("\\", "/");
                        if (tempInternalId.Equals(tempFileName, StringComparison.Ordinal))
                        {
                            contentCatalogData.InternalIds[j] = tempOriInternalId.Replace("{UnityEngine.AddressableAssets.Addressables.RuntimePath}", "{UnityEngine.AddressableAssets.PathRebuild.SplitPackLoadPath}");
                            break;
                        }
                    }
                }
            }
        }
        #endregion


        foreach (AssetsInfo info in curHotFixList.Contents.Values)
        {
            if (info.State == 0)
            {
                ++i;

                if (info.AssetName.Equals(catalogPersistName))
                    continue;

                bundleChangeList.Add(info.AssetName);
                if (!updateBundleResDict.TryGetValue(info.AssetName, out BundleRes bundleRes))
                {
                    if (currentBundleResDict.TryGetValue(info.AssetName, out BundleRes bundleTemp))
                    {
                        bundleRes = bundleTemp;
                        deleteAndAddResCount++;
                        Debug.Log(info.AssetName + "此资源出现原因：首包已有，经修改后又还原(或删除后又原样加入)");
                    }
                    else
                    {
                        rlt = -5;
                        Debug.LogError("NeedUpdateList NewBundleList 中都不存在 hashOfFileName：" + info.AssetName);
                        break;
                    }
                }

                //重定义热更 Catalog.json 热更加载的路径
                for (int j = 0; j < contentCatalogData.InternalIds.Length; j++)
                {
                    if (!contentCatalogData.InternalIds[j].EndsWith(".bundle"))
                        continue;
                    if (contentCatalogData.InternalIds[j].Equals(bundleRes.bundleFileId, StringComparison.Ordinal))
                    {
                        internalId = contentCatalogData.InternalIds[j];
                        contentCatalogData.InternalIds[j] = internalId.Replace("{UnityEngine.AddressableAssets.Addressables.RuntimePath}", "{UnityEngine.AddressableAssets.PathRebuild.RemoteLoadPath}");
                        break;
                    }
                }


                string srcPath = string.Format("{0}/{1}.bundle", orignalBundlePath, bundleRes.hashOfFileName);
                string desPath = string.Format("{0}/{1}/{2}.bundle", DestionAaDir, platformName, bundleRes.hashOfFileName);
                File.Copy(srcPath, desPath);

                if (EditorUtility.DisplayCancelableProgressBar("拷贝热更新资源 (Bundle)", string.Format("({0}/{1})\n{2}\n{3}", i.ToString(), count.ToString(), srcPath, desPath), (float)i / (float)count))
                {
                    rlt = 1;
                    break;
                }
            }
        }


        //回写 catalog.json  catalog.hash 更换列表中记录的md5
        File.WriteAllText(remoteBuildCatalog, JsonUtility.ToJson(contentCatalogData));

        string DesCatalogFile = string.Format("{0}/catalog_{1}.json", DestionAaDir, m_Settings.PlayerBuildVersion);
        File.WriteAllText(DesCatalogFile, JsonUtility.ToJson(contentCatalogData));
        string catalogHash = FrameworkTool.GetFileMD5(DesCatalogFile);
        FileInfo catalogInfo = new FileInfo(DesCatalogFile);
        ulong catalogSize = (ulong)catalogInfo.Length;
        string DesCatalogHashFile = string.Format("{0}/catalog_{1}.hash", DestionAaDir, m_Settings.PlayerBuildVersion);
        File.WriteAllText(DesCatalogHashFile, catalogHash);
        AssetDatabase.Refresh();

        curHotFixList.Contents.TryGetValue(catalogPersistName, out AssetsInfo assetsInfo1);
        assetsInfo1.AssetMD5 = catalogHash;
        assetsInfo1.Size = catalogSize;

        curBundleList.Contents.TryGetValue(catalogPersistName, out AssetsInfo assetsInfo2);
        assetsInfo2.AssetMD5 = catalogHash;
        assetsInfo2.Size = catalogSize;


        #region 主要用于测试 Addressable 和 列表记录 热更资源存在差异

        foreach (var item in needUpdateListRes.bundleListRes)
        {
            if (!bundleChangeList.Contains(item.hashOfFileName))
            {
                Debug.Log("Addressable.bin 相比 HotFixAaList.txt 热更新增:" + item);
            }
        }

        foreach (var item in bundleChangeList)
        {
            if (!updateBundleResDict.TryGetValue(item, out BundleRes bundleRes))
            {
                Debug.Log("HotFixAaList.txt 相比 Addressable.bin 新增:" + item);
            }
        }

        //第一种情况：某个资源在0保存有，1修改，2还原，那么在统计数量上面 就会不一样 （已排除这种情况）
        //第二种情况：某个资源在0保存有，1删除，2加入，那么在统计数量上面 就会不一样 （已排除这种情况）
        if (bundleChangeList.Count != (needUpdateListRes.bundleListRes.Count + deleteAndAddResCount))
            Debug.LogError(string.Format("Addressable计算热更：{0},热更配置计算热更：{1}, 生成的热更新资源数量不一样，！！！", needUpdateListRes.bundleListRes.Count, bundleChangeList.Count));
        else
            Debug.Log(string.Format("0->{0}，需要更新Bundle资源 {1}", curVersion, bundleChangeList.Count + 1));

        #endregion


        curHotFixList.Version = string.Format("{0}.{1}", maximalVersion, minimalVersion);
        curHotFixList.VersionIdentifier = curBundleList.VersionIdentifier;
        string curHotFixPath2 = string.Format("{0}/{1}/{2}/{3}/HotFixAaList.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion.ToString());
        string hotfixcontent2 = AssetList.Serialize(curHotFixList);
        WriteFile(curHotFixPath2, hotfixcontent2);

        return curHotFixList;
    }

    public static AssetList CombineHotFixToOne(BuildSetting buildSetting, AssetList curHotFixConfigList,AssetList curAaUpdateList)
    {
        //热更Bundle文件生成
        string maximalVersion = VersionHelper.GetMaximalVersion(buildSetting.sAssetVersion);
        string minimalVersion = VersionHelper.GetMinimalVersion(buildSetting.sAssetVersion);
        int.TryParse(minimalVersion, out int curVersion);
        string platformName = Enum.GetName(typeof(BuildTarget), buildSetting.mBuildTarget);


        AssetList curHotFixList = new AssetList();
        curHotFixList.Contents = curHotFixConfigList.Contents.Concat(curAaUpdateList.Contents).ToDictionary(k => k.Key, v => v.Value);
        curHotFixList.Version = curHotFixConfigList.Version;
        curHotFixList.VersionIdentifier = curHotFixConfigList.VersionIdentifier;
        string curHotFixPath = string.Format("{0}/{1}/{2}/{3}/HotFixList.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion.ToString());
        string hotfixcontent = AssetList.Serialize(curHotFixList);
        WriteFile(curHotFixPath, hotfixcontent);
        return curHotFixList;
    }

    private static AssetList MergeChangeList(AssetList newList, AssetList oldList)
    {
        AssetList assetList = new AssetList();

        foreach (AssetsInfo info in oldList.Contents.Values)
        {
            assetList.Contents[info.AssetName] = info;
        }

        foreach (AssetsInfo info in newList.Contents.Values)
        {
            assetList.Contents[info.AssetName] = info;
        }

        return assetList;
    }

    private static AssetList CompareAssetsList(AssetList newList, AssetList oldList, int minimalVersion, Action<float> action, ref bool versionIdentifierIsSame, string assetTypeName)
    {
        if (newList.VersionIdentifier != oldList.VersionIdentifier)
        {
            Debug.LogErrorFormat("热更系列标识符记录错误 oldList:{0}  newList{1}", oldList.VersionIdentifier, newList.VersionIdentifier);
            versionIdentifierIsSame = false;
        }

        Debug.LogFormat("对比资源列表");
        ulong HotFixTotalSize = 0L;

        int newAssetsCount = 0;
        int excessAssetsCount = 0;

        AssetList changeList = new AssetList();

        AssetsInfo newInfo = null;
        AssetsInfo oldInfo = null;
        int index = 0;
        int count = 0;

        #region 需要下载的内容
        index = 0;
        count = newList.Contents.Count;
        var remoteItor = newList.Contents.GetEnumerator();

        while (remoteItor.MoveNext())
        {
            newInfo = remoteItor.Current.Value;
            if (!oldList.Contents.TryGetValue(newInfo.AssetName, out oldInfo) || !newInfo.AssetMD5.Equals(oldInfo.AssetMD5))
            {
                //0 = 修改或者新增
                newInfo.State = 0;
                newInfo.Version = minimalVersion;
                changeList.Contents[newInfo.AssetName] = newInfo;
                ++newAssetsCount;

                HotFixTotalSize += newInfo.Size;
            }

            ++index;
            action?.Invoke((float)index / (float)count * 0.5f);
        }
        #endregion

        #region 需要删除的内容
        index = 0;
        count = oldList.Contents.Count;
        var localItor = oldList.Contents.GetEnumerator();

        while (localItor.MoveNext())
        {
            oldInfo = localItor.Current.Value;
            if (!newList.Contents.TryGetValue(oldInfo.AssetName, out newInfo))
            {
                //1 = 删除
                oldInfo.State = 1;
                oldInfo.Version = minimalVersion;
                changeList.Contents[oldInfo.AssetName] = oldInfo;
                ++excessAssetsCount;
            }

            ++index;
            action?.Invoke((float)index / (float)count * 0.5f + 0.5f);
        }
        #endregion

        Debug.LogFormat("{0}->{1},{2}需要更新的资源 {3}", minimalVersion - 1, minimalVersion, assetTypeName, newAssetsCount);
        Debug.LogFormat("{0}->{1},{2}需要删除的资源 {3}", minimalVersion - 1, minimalVersion, assetTypeName, excessAssetsCount);

        return changeList;
    }

    private static int CopyAssetTo0(BuildSetting buildSetting)
    {
        int rlt = 0;

        string sourceDirAddressable = string.Format("{0}/{1}", Application.dataPath, AddressableOriPath);
        if (!Directory.Exists(sourceDirAddressable))
        {
            Debug.LogErrorFormat("不存在源目录 {0}", sourceDirAddressable);
            rlt = -5;
            return rlt;
        }


        string maximalVersion = VersionHelper.GetMaximalVersion(buildSetting.sAssetVersion);
        string minimalVersion = "0";
        string platformName = Enum.GetName(typeof(BuildTarget), buildSetting.mBuildTarget);

        //拷贝config + dll + bundle
        string bundleDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundleBuildPath, platformName);
        string publishDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion);
        FileTool.CreateFolder(publishDir);
        rlt = FileTool.CopyAsset(bundleDir, publishDir, "拷贝资源(config + dll + bundle)到初始资源目录 0 ", true, new List<string> { ".manifest", ".dll" });


        //拷贝bin单个文件
        string DestionDirAddressable = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion, AddressableBin);
        FileTool.CreateFolder(DestionDirAddressable);
        string platMapPath = PlatformMappingService.GetPlatformPathSubFolder().ToString();
        string sourceDirNamebin = string.Format("{0}/{1}/addressables_content_state.bin", sourceDirAddressable, platMapPath);
        string destDirNamebin = string.Format("{0}/addressables_content_state.bin", DestionDirAddressable);
        File.Copy(sourceDirNamebin, destDirNamebin, true);


        //拷贝Layout
        string layoutSrc = string.Format("{0}/{1}", sourceDirAddressable, "buildlayout.txt");
        string layoutDes = string.Format("{0}/{1}/{2}/{3}/buildlayout.txt", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion);
        File.Copy(layoutSrc, layoutDes);


        //copy0 需要删除之前打热更包生成的 NeedUpdateList.json 更新列表
        string needUpdateListOriPath = string.Format("{0}/{1}", Build_AssetBundle.HotFixBinaryFile, "NeedUpdateList.json");
        if (File.Exists(needUpdateListOriPath))
            File.Delete(needUpdateListOriPath);
        AssetDatabase.Refresh();

        //分包需要Copy S0 ---（需要上传Bundle资源到S0）
        if (buildSetting.bUseSplitPack)
        {
            string s0OriDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundleBuildPath, AssetBundleS0Name);
            string s0DesDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundlePublishPath, platformName, AssetBundleS0Name);
            FileTool.CreateFolder(s0DesDir);
            rlt = FileTool.CopyAsset(s0OriDir, s0DesDir, "拷贝S0资源到热更S0", true);
        }

        return rlt;
    }

    private static AssetList GenAssetsList(BuildTarget target, string rootDir)
    {
        Debug.LogFormat("Build -> Start GenAssetsList (target = {0})", target);

        string platformName = Enum.GetName(typeof(BuildTarget), target);

        string assetCsvGenDir = string.Format("{0}/{1}", rootDir, AssetPath.sAssetCsvDir);
        string assetCodeGenDir = string.Format("{0}/{1}", rootDir, AssetPath.sAssetLogicDir);

        List<string> paths = new List<string>();

        if (Directory.Exists(assetCsvGenDir))
        {
            string[] csvPaths = Directory.GetFiles(assetCsvGenDir, "*.*", SearchOption.AllDirectories);
            paths.AddRange(csvPaths);
        }

        if (Directory.Exists(assetCodeGenDir))
        {
            string[] logicPaths = Directory.GetFiles(assetCodeGenDir, "*.*", SearchOption.AllDirectories);
            paths.AddRange(logicPaths);
        }

        AssetList assetList = new AssetList();
        Dictionary<string, AssetsInfo> assetListMD5 = new Dictionary<string, AssetsInfo>(paths.Count);

        if (paths != null)
        {
            for (int i = 0; i < paths.Count; ++i)
            {
                FileInfo info = new FileInfo(paths[i]);

                if (!info.Extension.Equals(".manifest"))
                {
                    AssetsInfo asset = new AssetsInfo();
                    asset.AssetMD5 = FrameworkTool.GetFileMD5(paths[i]);
                    asset.AssetName = paths[i].Substring(rootDir.Length + 1);
                    asset.Size = (ulong)info.Length;
                    asset.AssetType = 0;

                    AssetsInfo tmp = null;
                    if (assetListMD5.TryGetValue(asset.AssetMD5, out tmp))
                    {
                        Debug.LogErrorFormat("{0} 和 {1}拥有相同的MD5 ", tmp.AssetName, asset.AssetName);
                    }
                    else
                    {
                        assetListMD5.Add(asset.AssetMD5, asset);
                    }

                    assetList.Contents[asset.AssetName] = asset;

                    if (EditorUtility.DisplayCancelableProgressBar("生成该包的(Config/Dll)唯一标识符", string.Format("({0}/{1}){2}", i, paths.Count, paths[i]), (float)i / (float)paths.Count))
                    {
                        bStopBuild = true;
                        break;
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
        return assetList;
    }

    private static AssetList GenAaAssetList(string platformName)
    {
        string rootDir = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAddressableDir, platformName);
        Debug.LogFormat("Build -> Start GenAaAssetsList platformName:{0} rootDir:{1}", platformName, rootDir);


        //获取当前最新生成的bunde列表
        AssetList assetList = null;
        BundleListRes newBundleListRes = GetNewBundleListRes(EBundleRecordTxt.eNewGenBundleTxt);
        if (newBundleListRes == null || newBundleListRes.bundleListRes == null)
            return assetList;


        assetList = new AssetList();
        Dictionary<string, AssetsInfo> assetListMD5 = new Dictionary<string, AssetsInfo>(newBundleListRes.bundleListRes.Count);

        //catalog.json需要放入
        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        string catalogPath = string.Format("{0}/{1}/{2}/catalog_{3}.json", Application.dataPath, AddressableServerDataPath, platformName, m_Settings.PlayerBuildVersion);
        if (File.Exists(catalogPath))
        {
            string catalogName = string.Format("{0}/catalog_{1}.json", AssetPath.sAddressableCatalogDir, m_Settings.PlayerBuildVersion);
            FileInfo info = new FileInfo(catalogPath);
            AssetsInfo asset = new AssetsInfo();
            asset.AssetMD5 = FrameworkTool.GetFileMD5(catalogPath);
            asset.AssetName = catalogName;
            asset.Size = (ulong)info.Length;
            asset.AssetType = 2;
            assetList.Contents[asset.AssetName] = asset;
        }
        else
        {
            Debug.LogError(catalogPath + " :不存在catalog.json");
        }


        int i = 0;
        int Lenth = newBundleListRes.bundleListRes.Count;
        foreach (var item in newBundleListRes.bundleListRes)
        {
            string path = string.Format("{0}/{1}.bundle", rootDir, item.hashOfFileName);
            if (File.Exists(path))
            {
                FileInfo info = new FileInfo(path);
                AssetsInfo asset = new AssetsInfo();
                asset.AssetMD5 = FrameworkTool.GetFileMD5(path);
                asset.AssetName = item.hashOfFileName;
                asset.BundleName = item.bundleName;
                asset.HashOfBundle = item.hashOfBundle;
                asset.Size = (ulong)info.Length;
                asset.AssetType = 1;

                AssetsInfo tmp = null;
                if (assetListMD5.TryGetValue(asset.AssetMD5, out tmp))
                {
                    Debug.LogErrorFormat("{0} 和 {1}拥有相同的MD5 ", tmp.AssetName, asset.AssetName);
                }
                else
                {
                    assetListMD5.Add(asset.AssetMD5, asset);
                }

                assetList.Contents[asset.AssetName] = asset;
            }
            else
            {
                Debug.LogError("不存在bundle" + path);
            }

            ++i;

            if (EditorUtility.DisplayCancelableProgressBar("生成资源列表(Bundle + Catalog)", string.Format("({0}/{1}){2}", i, Lenth, path), (float)i / (float)Lenth))
            {
                bStopBuild = true;
                break;
            }
        }

        EditorUtility.ClearProgressBar();
        return assetList;
    }

    private static BundleListRes GetNewBundleListRes(EBundleRecordTxt eBundleRecordTxt)
    {
        BundleListRes newBundleListRes = null;
        string bundleRecordTxtName = "";
        switch (eBundleRecordTxt)
        {
            case EBundleRecordTxt.eNewGenBundleTxt:
                bundleRecordTxtName = "NewBundleList.json";
                break;
            case EBundleRecordTxt.eNeedUpdateBundleTxt:
                bundleRecordTxtName = "NeedUpdateList.json";
                break;
        }
        string fileOriPath = string.Format("{0}/{1}", Build_AssetBundle.HotFixBinaryFile, bundleRecordTxtName);
        if (!File.Exists(fileOriPath))
        {
            Debug.LogError("不存在文件：" + fileOriPath);
            return newBundleListRes;
        }

        newBundleListRes = JsonUtility.FromJson<BundleListRes>(File.ReadAllText(fileOriPath));
        return newBundleListRes;
    }

    public static AssetList GenS0AssetList()
    {
        string platformName = Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget);
        string rootDir = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAddressableDir, platformName);
        Debug.LogFormat("Build -> Start GenAaAssetsList platformName:{0} rootDir:{1}", platformName, rootDir);


        AssetList assetList = null;
        BundleListRes newAddBundleListRes = GetNewBundleListRes(EBundleRecordTxt.eNewGenBundleTxt);
        if (newAddBundleListRes == null || newAddBundleListRes.bundleListRes == null)
        {
            Debug.LogError("NewBundleList.json 内容为空");
            return assetList;
        }


        assetList = new AssetList();
        Dictionary<string, AssetsInfo> assetListMD5 = new Dictionary<string, AssetsInfo>(newAddBundleListRes.bundleListRes.Count);

        ////catalog.json需要放入
        //var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        //string catalogPath = string.Format("{0}/{1}/{2}/catalog_{3}.json", Application.dataPath, AddressableServerDataPath, platformName, m_Settings.PlayerBuildVersion);
        //if (File.Exists(catalogPath))
        //{
        //    string catalogName = string.Format("{0}/catalog_{1}.json", AssetPath.sAddressableCatalogDir, m_Settings.PlayerBuildVersion);
        //    FileInfo info = new FileInfo(catalogPath);
        //    AssetsInfo asset = new AssetsInfo();
        //    asset.AssetMD5 = FrameworkTool.GetFileMD5(catalogPath);
        //    asset.AssetName = catalogName;
        //    asset.Size = (ulong)info.Length;
        //    asset.AssetType = 2;
        //    assetList.Contents[asset.AssetName] = asset;
        //}
        //else
        //{
        //    Debug.LogError(catalogPath + " :不存在catalog.json");
        //}


        int i = 0;
        int Lenth = newAddBundleListRes.bundleListRes.Count;
        foreach (var item in newAddBundleListRes.bundleListRes)
        {
            string path = string.Format("{0}/{1}.bundle", rootDir, item.hashOfFileName);
            if (File.Exists(path))
            {
                FileInfo info = new FileInfo(path);
                AssetsInfo asset = new AssetsInfo();
                asset.AssetMD5 = FrameworkTool.GetFileMD5(path);
                asset.AssetName = item.hashOfFileName;
                asset.BundleName = item.bundleName;
                asset.HashOfBundle = item.hashOfBundle;
                asset.Size = (ulong)info.Length;
                asset.AssetType = 1;

                AssetsInfo tmp = null;
                if (assetListMD5.TryGetValue(asset.AssetMD5, out tmp))
                {
                    Debug.LogErrorFormat("{0} 和 {1}拥有相同的MD5 ", tmp.AssetName, asset.AssetName);
                }
                else
                {
                    assetListMD5.Add(asset.AssetMD5, asset);
                }

                assetList.Contents[asset.AssetName] = asset;
            }
            else
            {
                Debug.LogError("不存在bundle" + path);
            }

            ++i;

            if (EditorUtility.DisplayCancelableProgressBar("生成资源列表(Bundle)", string.Format("({0}/{1}){2}", i, Lenth, path), (float)i / (float)Lenth))
            {
                bStopBuild = true;
                break;
            }
        }

        EditorUtility.ClearProgressBar();
        return assetList;
    }


    #endregion


    #region 打包APP
    public static VersionSetting SetVersion(BuildSetting buildSetting)
    {
        VersionSetting versionSetting = null;
        string versionPath = string.Format("{0}/Resources/{1}", Application.dataPath, "VersionSetting.asset");
        if (File.Exists(versionPath))
        {
            versionSetting = AssetDatabase.LoadAssetAtPath<VersionSetting>("Assets/Resources/VersionSetting.asset");
        }
        if (versionSetting == null)
        {
            versionSetting = new VersionSetting();
            //versionSetting.eChannelType = buildSetting.eChannelType;
            versionSetting.eHotFixType = buildSetting.eHotFixMode;
            versionSetting.AssetVersion = buildSetting.sAssetVersion;
            versionSetting.VersionUrl = buildSetting.sVersionUrl;
            versionSetting.AppVersion = buildSetting.sAppVersion;
            AssetDatabase.CreateAsset(versionSetting, "Assets/Resources/VersionSetting.asset");
        }

        AssetDatabase.SaveAssets();

        return versionSetting;
    }
    public static void SetStackTraceLogType(bool IsDebugMode)
    {
        if (!IsDebugMode)
        {
            PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
        }
        else
        {
            PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
        }
    }
    public static void StartBuild(BuildTarget target, bool genAssetBundle, bool AtlasInclude = true)
    {
        bStopBuild = false;

        if (genAssetBundle)
        {
            //analysisMaterial分析材质球
            AnalysisMaterial.DoClearMaterialProperty();
            //TODO:先不收集变体
            //AnalysisMaterial.DoAnalysisMaterial();
            //AnalysisMaterial.DoAnalysisVariants();

            //清除图集缓存
            string cacheDir = $"{Application.dataPath}/../Library/AtlasCache";
            FileTool.DeleteFolder(cacheDir);

            SetAtlasIncludeInBuild(AtlasInclude);
            AssetDatabase.SaveAssets();

            //Addressable构建过程也有打图集 后续优化下
            SpriteAtlasUtility.PackAllAtlases(target);
            AssetDatabase.Refresh();
        }

    }
    public static void SetAtlasIncludeInBuild(bool value)
    {
        /****************** 设置图集状态  ***************/
        if (bStopBuild)
            return;

        string atlasDir = string.Format("{0}/{1}", Application.dataPath, "ResourcesAB/Atlas");
        if (!Directory.Exists(atlasDir))
            return;

        DirectoryInfo dirInfo = new DirectoryInfo(string.Format("{0}/{1}", Application.dataPath, atlasDir));
        FileInfo[] fileInfoArr = dirInfo.GetFiles("*.spriteatlas", SearchOption.AllDirectories);

        for (int i = 0; i < fileInfoArr.Length; ++i)
        {
            FileInfo fileInfo = fileInfoArr[i];

            string fullName = fileInfo.FullName.Replace('\\', '/');
            string assetPath = "Assets" + fullName.Replace(Application.dataPath, "");

            if (EditorUtility.DisplayCancelableProgressBar("设置图集状态", string.Format("({0}/{1}){2}", i.ToString(), fileInfoArr.Length.ToString(), assetPath), (float)i / (float)fileInfoArr.Length))
            {
                bStopBuild = true;
                break;
            }

            SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
            spriteAtlas.SetIncludeInBuild(value);

            SpriteAtlasPackingSettings spriteAtlasPackingSettings = spriteAtlas.GetPackingSettings();
            spriteAtlasPackingSettings.enableRotation = false;
            spriteAtlasPackingSettings.enableTightPacking = false;
            spriteAtlasPackingSettings.padding = 2;
            spriteAtlas.SetPackingSettings(spriteAtlasPackingSettings);

            var atlasSetting = spriteAtlas.GetPlatformSettings("Android");
            atlasSetting.overridden = true;
            atlasSetting.format = TextureImporterFormat.ETC2_RGBA8;
            spriteAtlas.SetPlatformSettings(atlasSetting);

            atlasSetting = spriteAtlas.GetPlatformSettings("iPhone");
            atlasSetting.overridden = true;
            atlasSetting.format = TextureImporterFormat.ASTC_4x4;
            spriteAtlas.SetPlatformSettings(atlasSetting);
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
    public static void EndBuild(BuildTarget target, bool genAssetBundle, bool AtlasInclude)
    {
        bStopBuild = false;
        if (genAssetBundle)
        {
            if (AtlasInclude)
            {
                SetAtlasIncludeInBuild(true);
                AssetDatabase.SaveAssets();
            }

            SpriteAtlasUtility.PackAllAtlases(target);
            AssetDatabase.Refresh();
        }
    }
    public static int ClearAssetSteamPath()
    {
        if (Directory.Exists(Application.streamingAssetsPath))
        {
            DirectoryInfo info = new DirectoryInfo(Application.streamingAssetsPath);
            info.Delete(true);
            info = null;
        }
        return 0;
    }
   
    public static void WriteFile(string path, string content)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(path, content);
    }

    public static int GetAppOutPath(BuildSetting buildSetting, out string appFilePath)
    {
        string AppOutPath = buildSetting.sAppPublishPath;
        string currentTime = string.Format("{0:yyyy.MMdd.HHmm}", System.DateTime.Now);
        string productName = "JiangHu";
        appFilePath = null;

        if (string.IsNullOrWhiteSpace(AppOutPath))
        {
            Debug.LogError("未选择输出目录");
            return 1;
        }
        else
        {
            switch (buildSetting.mBuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    appFilePath = string.Format("{0}/{1}_{2}_{3}_{4}_{5}_{6}.exe", AppOutPath, productName, currentTime, buildSetting.sSvnRevision, buildSetting.eScriptingImplementation.ToString(), buildSetting.eRuntimeMode.ToString(), buildSetting.sAppVersion);
                    break;
                case BuildTarget.iOS:
                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.crossgate.kuaishou.ios");
                    appFilePath = string.Format("{0}/{1}_{2}_{3}_{4}_{5}_{6}.ipa", AppOutPath, productName, currentTime, buildSetting.sSvnRevision, buildSetting.eScriptingImplementation.ToString(), buildSetting.eRuntimeMode.ToString(), buildSetting.sAppVersion);
                    break;
                case BuildTarget.Android:
                    appFilePath = string.Format("{0}/{1}_{2}_{3}_{4}_{5}_{6}.apk", AppOutPath, productName, currentTime, buildSetting.sSvnRevision, buildSetting.eScriptingImplementation.ToString(), buildSetting.eRuntimeMode.ToString(), buildSetting.sAppVersion);
                    break;
                default:
                    {
                        Debug.LogErrorFormat("未支持该平台打包 {0}", buildSetting.mBuildTarget.ToString());
                        break;
                    }
            }

            if (string.IsNullOrWhiteSpace(appFilePath))
            {
                return 1;
            }
        }

        return 0;
    }
    private static string FixVersionAndBackup(VersionSetting versionSetting, BuildSetting buildSetting, bool genHotFix)
    {
        if (genHotFix)
            return buildSetting.sAssetVersion;

        //管总要求，直接打热更新资源  根据文件夹来确定版本号，即使打包工具填的不对，最终到这里会更正的 (根据大版本生成热更的)
        string version = buildSetting.sAssetVersion;
        string maximalVersion = VersionHelper.GetMaximalVersion(version);
        string minimalVersion = VersionHelper.GetMinimalVersion(version);


        //根据热更文件夹名称，自动+1生成正确的热更版本号，无需手动设置
        string fileDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundlePublishPath, buildSetting.mBuildTarget);
        FileTool.CreateFolder(fileDir, false);

        DirectoryInfo rootInfo = new DirectoryInfo(fileDir);
        DirectoryInfo[] dicts = rootInfo.GetDirectories();
        int lastSmallVersion = -1;
        foreach (var item in dicts)
        {
            int smallVersion = 0;
            if (int.TryParse(item.Name, out smallVersion))
            {
                if (smallVersion > lastSmallVersion)
                    lastSmallVersion = smallVersion;
            }
        }
        version = string.Format("{0}.{1}", maximalVersion, lastSmallVersion + 1);
        versionSetting.AssetVersion = buildSetting.sAssetVersion = version;
        EditorUtility.SetDirty(versionSetting);
        EditorUtility.SetDirty(buildSetting);
        AssetDatabase.SaveAssets();
        return version;
    }
    private static int CompilePlayerScripts(BuildSetting buildSetting)
    {
        BuildTarget target = buildSetting.mBuildTarget;
        string platformName = Enum.GetName(typeof(BuildTarget), target);
        var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
        ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings()
        {
            target = target,
            group = targetGroup,
            options = ScriptCompilationOptions.None
        };

        //创建临时输出程序集路径
        string TempOutputFolder = Path.GetDirectoryName(Application.dataPath) + "/AppScriptDll";
        FileTool.CreateFolder(TempOutputFolder);

        //编译脚本
        ScriptCompilationResult scriptCompilationResult = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, TempOutputFolder);
        if (scriptCompilationResult.assemblies.Count > 0)
        {
            Debug.LogFormat("success to BuildAssemblies count {0}", scriptCompilationResult.assemblies.Count);
        }
        else
        {
            Debug.LogErrorFormat("failed to BuildAssemblies count {0}", scriptCompilationResult.assemblies.Count);
            return -1;
        }
        AssetDatabase.Refresh();

        //删除零时文件
        FileTool.DeleteFolder(TempOutputFolder);
        return 0;
    }
    private static int SetPackageIdentifier(VersionSetting versionSetting, BuildTarget buildTarget)
    {
        int rlt = 0;

        //设置apk的唯一性，避免1.0.0热更关闭apk 和1.0.3热更apk 在【覆盖安装时】需要清除手机本地缓存

        //获取需要的 LocalHash 
        string platformName = Enum.GetName(typeof(BuildTarget), buildTarget);
        string localHashPath = string.Format("{0}/{1}", Build_AssetBundle.HotFixBinaryFile, "LolcalHash.txt");
        string lolcalAaHash = File.ReadAllText(localHashPath);
        if (!File.Exists(localHashPath) || string.IsNullOrEmpty(lolcalAaHash))
        {
            Debug.LogError(string.Format("不存在文件:{0},或文件内容为空：{1}", localHashPath, lolcalAaHash));
            return 5;
        }

        //需要获取 HotFixUniqueIdentifier
        string HotFixUniqueIdentifierPath = string.Format("{0}/{1}", Build_AssetBundle.HotFixBinaryFile, "HotFixUniqueIdentifier.txt");
        string hotFixUniqueIdentifierStr = File.ReadAllText(HotFixUniqueIdentifierPath);
        if (!File.Exists(HotFixUniqueIdentifierPath) || string.IsNullOrEmpty(hotFixUniqueIdentifierStr))
        {
            Debug.LogError(string.Format("不存在文件:{0},或文件内容为空：{1}", HotFixUniqueIdentifierPath, hotFixUniqueIdentifierStr));
            return 5;
        }

        //根据生成的AssetList内容 生成hash值
        //string bundleDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundleBuildPath, platformName);
        //AssetList curAssetList = GenAssetsList(buildTarget, bundleDir);
        //string assetListContent = AssetList.Serialize(curAssetList);
        //string localConfigHash = HashingMethods.Calculate(assetListContent).ToString();
        //string combineHash = HashingMethods.Calculate(string.Format("{0}|{1}", lolcalAaHash, localConfigHash)).ToString();


        //整包的唯一标识符 不需要这么费劲的计算生成，可以给个时间所有的包都具有唯一性了，那么每次覆盖安装就删除缓存
        versionSetting.PackageIdentifier = TimeManager.ClientNowMillisecond().ToString();
        versionSetting.HotFixUniqueIdentifier = hotFixUniqueIdentifierStr;
        EditorUtility.SetDirty(versionSetting);

        return rlt;
    }

    #endregion


    #region 编译指令打包
    private static BuildArgs ParseCommandArgs(string[] args)
    {
        BuildArgs buildArgs = new BuildArgs();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-buildType")
            {
                buildArgs.buildType = args[i + 1];
            }

            if (args[i] == "-vision")
            {
                buildArgs.Vision = args[i + 1];
            }

            if (args[i] == "-revision")
            {
                buildArgs.revision = args[i + 1];
            }

            //if (args[i] == "-outputPath")
            //{
            //    buildArgs.OutputPath = args[i + 1];
            //}
            //if (args[i] == "-buildSettingPath")
            //{
            //    buildArgs.BuildSettingPath = args[i + 1];
            //}
            //if (args[i] == "-debugMode")
            //{
            //    buildArgs.DebugMode = string.Equals(args[i + 1], "1");
            //}
        }

        return buildArgs;
    }

    //编译指令 调用
    public static void BuildFromCmd()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        BuildArgs buildArgs = ParseCommandArgs(args);
        int defaultBuildType = 0;//默认打的是Release
        int.TryParse(buildArgs.buildType, out defaultBuildType);

        BuildSetting buildSetting = null;
        string buildsettingPath = string.Format("{0}/{1}", Application.dataPath, "DefaultBuildSetting.asset");
        if (File.Exists(buildsettingPath))
        {
            buildSetting = AssetDatabase.LoadAssetAtPath<BuildSetting>("Assets/DefaultBuildSetting.asset");
        }
        if (buildSetting == null)
        {
            buildSetting = BuildSetting.CreateBuildSetting("Assets", "DefaultBuildSetting");
        }

        Debug.LogFormat("MyLog buildType {0}", buildArgs.buildType);
        Debug.LogFormat("MyLog Vision {0}", buildArgs.Vision);
        Debug.LogFormat("MyLog revision {0}", buildArgs.revision);
        switch (defaultBuildType)
        {
            case 0:
                Debug.LogFormat("发布版 {0}", defaultBuildType);
                buildSetting.sAssetVersion = buildArgs.Vision;
                buildSetting.Release();
                break;
            case 1:
                Debug.LogFormat("测试版 {0}", defaultBuildType);
                buildSetting.sAssetVersion = buildArgs.Vision;
                buildSetting.Debug();
                break;
            case 2:
                Debug.LogFormat("热更版 {0}", defaultBuildType);
                buildSetting.HotFix();
                break;
        }

        buildSetting.sSvnRevision = buildArgs.revision;

        //生成app
        if (buildSetting.bBuildApp)
        {
            if (string.IsNullOrWhiteSpace(buildSetting.sAppPublishPath))
                buildSetting.sAppPublishPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "APPPublish");
            if (!Directory.Exists(Path.GetFullPath(buildSetting.sAppPublishPath)))
            {
                Directory.CreateDirectory(buildSetting.sAppPublishPath);
            }
        }

        Debug.LogFormat("开始构建！！！");
        Build(buildSetting);
        Debug.LogFormat("构建结束！！！");
    }

    #endregion


    #region 检查脚本宏设置
    public static void CheckScriptingDefine(List<string> add, List<string> remove, string symbol, bool bAdd)
    {
        if (bAdd)
        {
            add.Add(symbol);
        }
        else
        {
            remove.Add(symbol);
        }
    }

    public static void SetScriptingDefine(BuildSetting buildSetting)
    {
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildSetting.mBuildTarget);

        List<string> adds = new List<string>();
        List<string> removes = new List<string>();
        CheckScriptingDefine(adds, removes, "DEBUG_MODE", buildSetting.bUseDebugMode);
        //CheckScriptingDefine(adds, removes, "ILRUNTIME_MODE", buildSetting.eRuntimeMode == ERuntimeMode.ILRuntime);
        CheckScriptingDefine(adds, removes, "MONO_REFLECT_MODE", buildSetting.eRuntimeMode == ERuntimeMode.MonoReflect);
        CheckScriptingDefine(adds, removes, "DISABLE_ILRUNTIME_DEBUG", !buildSetting.bUseDebugMode);
        CheckScriptingDefine(adds, removes, "BEHAVIAC_RELEASE", !buildSetting.bUseDebugMode);
        CheckScriptingDefine(adds, removes, "ADDRESSABLES_LOG_ALL", buildSetting.bUseDebugMode);
        CheckScriptingDefine(adds, removes, "NO_PROFILER", !buildSetting.bBuildOptions);
        //7：快手不加密的包，此id不能变
        CheckScriptingDefine(adds, removes, "PROTOCOL_NOENCRYPT", buildSetting.sChannelId == 7 || buildSetting.bUseProtocal_noEncrypt ? true : false);
        //8：快手扫码包
        CheckScriptingDefine(adds, removes, "USE_SDK_QRCODE", buildSetting.sChannelId == 8 || buildSetting.bUseQRcode ? true : false);


#if UNITY_ANDROID
        
#elif UNITY_STANDALONE_WIN
        //CheckScriptingDefine(adds, removes, "USE_PCSDK", buildSetting.eApkMode == ApkMode.UseSDK);
        //CheckScriptingDefine(adds, removes, "GM_PROPAGATE_VERSION", buildSetting.bGmPropagateVersion);
#endif

        ApplyScriptingDefine(adds, removes, buildTargetGroup);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="adds"></param>
    /// <param name="removes"></param>
    /// <param name="buildTargetGroup"></param>
    /// <returns>原先的define</returns>
    public static string ApplyScriptingDefine(List<string> adds, List<string> removes, BuildTargetGroup buildTargetGroup)
    {
        string sDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        string[] defines = sDefines.Split(';');

        List<string> defs = new List<string>(defines.Length + adds.Count);

        for (int i = 0; i < defines.Length; ++i)
        {
            string item = defines[i];
            if (!removes.Contains(item))
            {
                defs.Add(item);
            }
        }

        for (int i = 0; i < adds.Count; ++i)
        {
            string item = adds[i];
            if (!defs.Contains(item))
            {
                defs.Add(item);
            }
        }

        string newDefineStr = string.Join(";", defs);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newDefineStr);
        Debug.LogFormat("宏 : {0}", newDefineStr);

        return sDefines;
    }
    #endregion


    #region 打印相关
    public static int PrintBuildSetting(BuildSetting buildSetting)
    {
        Debug.Log("==========Build Start==================");
        Debug.Log(buildSetting.mBuildTarget.ToString());
        Debug.LogFormat("加密生成配置 {0}", buildSetting.bGenCsv);
        Debug.LogFormat("加密生成代码 {0}", buildSetting.bGenLogic);
        Debug.LogFormat("打包资源 {0}", buildSetting.bGenAssetBundle);

        Debug.LogFormat("生成应用程序 {0}", buildSetting.bBuildApp);
        if (buildSetting.bBuildApp)
        {
            //Debug.LogFormat("  重新拷贝配置 {0}", buildSetting.bCopyConfigToStreamPath);
            //Debug.LogFormat("  重新拷贝代码 {0}", buildSetting.bCopyLogicToStreamPath);
            //Debug.LogFormat("  重新拷贝资源 {0}", buildSetting.bCopyABToStreamPath);
            Debug.LogFormat("  APP输出路径 {0}", buildSetting.sAppPublishPath);
        }
        Debug.Log("==========Build Finish==================");
        return 0;
    }

    public static void DebugBundleChangeAssetEntryLogFile(Dictionary<string, List<string>> needMoveFiles, string savePath)
    {
        System.Text.StringBuilder str = new System.Text.StringBuilder();
        foreach (var item in needMoveFiles)
        {
            if (item.Value != null)
            {
                string[] keylist = item.Key.Split('-');
                str.AppendFormat("{0}\t {1}\t Bundle包含AssetEntry的个数:{2}\n", keylist[1], keylist[0], item.Value.Count);
                foreach (var dep in item.Value)
                {
                    str.AppendFormat("\t{0} \n", dep);
                }
            }
            else
            {
                Debug.LogError("key:" + item.Key + " value == null");
                str.AppendFormat("{0}\tBundle包含AssetEntry的个数:{1}\n", item.Key, "null");
            }
            str.AppendLine("------------------------------------------------------------------------------------");
        }

        System.IO.File.WriteAllText(savePath + "/NeedUpdateListBundle.txt", str.ToString());
    }

    public static void PrintGroupAssetEntryLogFile(string version)
    {
        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        var groups = m_Settings.groups.Where(g => g != null);
        StringBuilder str = new StringBuilder();
        foreach (var assetGroup in groups)
        {
            str.AppendFormat("{0}\tGroup中AssetEntry的个数:{1}\n", assetGroup.name, assetGroup.entries.Count);
            foreach (var entry in assetGroup.entries)
            {
                string label = string.Empty;
                if (entry.labels.Count > 0)
                {
                    foreach (var item in entry.labels)
                    {
                        label = item + "\t";
                    }
                }
                str.AppendFormat("\t{0}\t\tLabelCount:{1}\t{2}\n", entry.AssetPath, entry.labels.Count, label);
            }
            str.AppendLine("------------------------------------------------------------------------------------");
        }


        if (!Directory.Exists(Build_AssetBundle.HotFixBinaryFile))
        {
            Directory.CreateDirectory(Build_AssetBundle.HotFixBinaryFile);
        }
        AssetDatabase.Refresh();
        System.IO.File.WriteAllText(string.Format("{0}/{1}{2}", Build_AssetBundle.HotFixBinaryFile, version, "GroupToAssetEntries.txt"), str.ToString());
    }
    #endregion

}

public class FilterBuildAssemblies : IFilterBuildAssemblies
{
    public int callbackOrder { get { return 0; } }

    public string[] OnFilterAssemblies(BuildOptions buildOptions, string[] assemblies)
    {
        if (BuildTool.USE_PROFILE_MODE)
        {
            List<string> all = new List<string>(assemblies);
            return all.ToArray();
        }
        else if ((buildOptions & BuildOptions.BuildScriptsOnly) != BuildOptions.None)
        {
            List<string> all = new List<string>(assemblies);
            return all.ToArray();
        }
        else
        {
            List<string> all = new List<string>(assemblies);
            all.Remove("Library/ScriptAssemblies/Logic.dll");
            Debug.Log("remove Library/ScriptAssemblies/Logic.dll");
            return all.ToArray();
        }
    }

    //ToDo 后期优化 IPreprocessShaders
    //public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    //{
    //    throw new NotImplementedException();
    //}
}