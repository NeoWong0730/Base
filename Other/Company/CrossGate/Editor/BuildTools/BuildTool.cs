
using Framework;
using Lib.AssetLoader;
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
    public class AssetBundleBuildComparer : IEqualityComparer<AssetBundleBuild>
    {
        public bool Equals(AssetBundleBuild x, AssetBundleBuild y)
        {
            return string.Equals(x.assetBundleName, y.assetBundleName, StringComparison.Ordinal);
        }

        public int GetHashCode(AssetBundleBuild obj)
        {
            return obj.assetBundleName.GetHashCode();
        }
    }

    private class BuildArgs
    {
        //public string BuildSettingPath;
        //public string OutputPath; 已定义
        public string Vision;
        //public bool DebugMode;
        public string buildType;
        public string revision;
    }

    public enum EAssetProcessing : int
    {
        eSetAssetName = 1,
        eGetAssetList = 2,
        eGetAssetAddress = 4,
    }

    public enum ECopyAssetMode : int
    {
        eCopyAB = 1,
        eCopyConfig = 2,
        eCopyDll = 3,
        eCopyABToAssetBundleBuildPath = 4,
        eCopyCatalog = 5,
        eCopyBundle = 6
    }

    public enum EAssetBundleMode : int
    {
        eConfigDll = 1,
        eAaBundle = 2
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


    public static bool SetFORCE_AB = false;


    public static string AssetBundleEncryptPath = "../AssetEncrypt";
    public static string AssetBundleBuildPath = "../AssetBundle";
    public static string AssetBundlePublishPath = "../AssetPublish";
    public static string AddressableOriPath = "../Library/com.unity.addressables";
    public static string AddressableServerDataPath = "../ServerData";
    public static string HotFixBinaryFilePath = "../BinaryFile";

    public static string AtlasRoot = "Assets/Projects/Image";
    public static string ScriptRoot = "Assets/Scripts";
    public static string DependsRoot = "Depends";

    public static string AddressableBin = "AddressableBin";
    public static string AaPublishDirName = "AssetPublish";


    public static bool bStopBuild = false;

    private static Dictionary<string, AssetBundleBuild> mAssetBundleBuilds;
    private static List<string> TmpBuilds = new List<string>();

    public static bool USE_PROFILE_MODE = false;

    public static int WebRequestRetryCount = 3;
    public static int WebRequestTimeOut = 10;


    #region 路径获取
    static string hotfixBinaryFile;
    public static String HotFixBinaryFile
    {
        get
        {
            if (String.IsNullOrEmpty(hotfixBinaryFile))
                hotfixBinaryFile = String.Format("{0}/{1}", Application.dataPath, HotFixBinaryFilePath);
            return hotfixBinaryFile;
        }
    }
    #endregion


    #region 生成资源
    public static List<AssetBundleSettingCell> AddAssetBundleSettingCell()
    {
        //if (mAssetBundleSettingCells != null && mAssetBundleSettingCells.Count > 0)
        //{
        //    return;
        //}
        List<AssetBundleSettingCell> mAssetBundleSettingCells = new List<AssetBundleSettingCell>();

        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "AnimationClip", "*.anim", "*.*"));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Animator", "*.controller", "*.*"));
#if USE_ATLAS_COMBINE
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "AtlasExport", "*.png|.asset", null));
#else
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Atlas", "*.spriteatlas", null));
#endif
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Audio", "*.mp3|*.wav", null));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Emoji", "*.asset", null));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Font", "*.fontsettings|*.ttf", null));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Material", "*.mat", "*.*"));
#if USE_ATLAS_COMBINE
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "PrefabExport", "*.prefab", "*.*"));
#else
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Prefab", "*.prefab", "*.*"));
#endif
        //mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Scene", "*.unity", "*.*"));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "SceneBase", "*.unity", "*.*"));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "SceneExport", "*.*", "*.*"));
        //mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Settings", "*.asset", "*.*"));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Shader", "*.shader|*.shadervariants|*.shadergraph", "*.shader|*.shadergraph", "shader"));//.shader | *.cginc |
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Texture", "*.png", null));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "UI", "*.prefab", null));

        //TODO:后期去除
        //mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Test", "*.prefab", null));

        return mAssetBundleSettingCells;
    }

    public static AddressableAssetSettings ClearAddressableGroup()
    {
        EditorUtility.DisplayCancelableProgressBar("GenAssetBundle", "清理Asset Address", 0);

        AddressableAssetSettings assetSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
        for (int i = assetSettings.groups.Count - 1; i >= 0; --i)
        {
            AddressableAssetGroup assetGroup = assetSettings.groups[i];

            if (assetGroup.HasSchema(typeof(PlayerDataGroupSchema)))
            {
                //排除内置的 Build In Data 
            }
            else if (!assetGroup.Default)
            {
                //删除自建创建的组
                assetSettings.RemoveGroup(assetGroup);
            }
            else
            {
                //去除默认本地组下的可寻址资源
                AddressableAssetEntry[] entries = new AddressableAssetEntry[assetGroup.entries.Count];
                assetGroup.entries.CopyTo(entries, 0);

                for (int j = entries.Length - 1; j >= 0; --j)
                {
                    assetGroup.RemoveAssetEntry(entries[j]);
                }
            }
        }

        //清除所有的label
        var labeList = assetSettings.GetLabels();
        foreach (var item in labeList)
        {
            assetSettings.RemoveLabel(item);
        }
        assetSettings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        EditorUtility.DisplayCancelableProgressBar("GenAssetBundle", "清理Asset Address", 1);

        return assetSettings;
    }

    public static void GenAddressableGroup_New()
    {
        List<AssetBundleSettingCell> AssetBundleSetting = AddAssetBundleSettingCell();
        AddressableAssetSettings assetSettings = ClearAddressableGroup();
        RebuildAddressableGroup(AssetBundleSetting, EAssetProcessing.eGetAssetAddress, assetSettings, true, false);
    }

    public static Dictionary<string, string> CollectResourceABSceneCopy()
    {
        Dictionary<string, string> fileInfos = new Dictionary<string, string>();
        string TopPath = string.Format("{0}/{1}/{2}", "Assets", AssetPath.sResourcesRootDir, "SceneExport");
        string SceneExportPath = string.Format("{0}/{1}/{2}", Application.dataPath, "ResourcesAB", "SceneExport");
        if (Directory.Exists(SceneExportPath))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(SceneExportPath);
            FileInfo[] fs = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            for (int i = 0; i < fs.Length; ++i)
            {
                FileInfo fileInfo = fs[i];
                if (string.Equals(fileInfo.Extension, ".meta", StringComparison.Ordinal))
                {
                    continue;
                }
                else
                {
                    if (!fileInfo.FullName.EndsWith(".unity", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    //"Assets/ResourcesAB/SceneExport/Scene1004.unity"
                    string fullName = fileInfo.FullName.Replace('\\', '/');
                    string assetPath = "Assets" + fullName.Replace(Application.dataPath, "");
                    string keyPath = assetPath.Replace(TopPath, "Scene");
                    fileInfos.Add(keyPath, assetPath);
                }
            }
        }

        return fileInfos;

    }
    private static int RebuildAddressableGroup(List<AssetBundleSettingCell> bundleSettingCells, EAssetProcessing assetProcessing, AddressableAssetSettings settings = null, bool enableTestSence = false, bool getDependencies = true)
    {
        Debug.LogFormat("Build -> Start CollectAssetBundles()");
        ShaderRecoder.Clear();
        FBXRecoder.Clear();
        MaterialRecoder.Clear();
        ControllerRecoder.Clear();
        PlayableRecoder.Clear();
        PrefabRecoder.Clear();
        AssetRecoder.Clear();
        TerrainLayerRecoder.Clear();
        SceneRecoder.Clear();
        TextureRecoder.Clear();
        AniRecoder.Clear();
        SignalRecoder.Clear();

        _outAssetPaths.Clear();
        _mainAssets.Clear();

        int count = 0;

        int rlt = 0;
        for (int i = 0; i < bundleSettingCells.Count; ++i)
        {
            AssetBundleSettingCell cell = bundleSettingCells[i];
            if (cell.bSelected)
            {
                rlt = CollectAssetBundle(cell, settings, enableTestSence);
            }

            if (rlt != 0)
            {
                break;
            }
        }

        if (rlt != 0)
            Debug.LogErrorFormat("搜集 CollectAssetBundle 出错:{0}", rlt);

        count = _mainAssets.Count;

        if (getDependencies)
        {
            int j = 0;
            for (; j < 10 && _outAssetPaths.Count > 0; ++j)
            {
                Debug.LogFormat("次数{0} 处理{1}", j.ToString(), _outAssetPaths.Count.ToString());
                string[] outAssetPathsArray = _outAssetPaths.ToArray();
                _outAssetPaths.Clear();
                rlt = GetDependencies(outAssetPathsArray, _outAssetPaths);
                if (rlt != 0)
                {
                    break;
                }
            }

            if (rlt != 0)
                Debug.LogErrorFormat("分析主资源获取依赖关系 GetDependencies 出错:{0}", rlt);


            Debug.LogFormat("次数{0} 剩余{1}", j.ToString(), _outAssetPaths.Count.ToString());
            _outAssetPaths.Clear();
            _mainAssets.Clear();
            //1

            count += GenerateLevel1Group(ShaderRecoder, "Shader", 1, "shader");
            count += GenerateLevel1Group(SceneRecoder, "_Scene", 1);
            count += GenerateLevel1Group(TerrainLayerRecoder, "_TerrainLayer");
            count += GenerateLevel1Group(PlayableRecoder, "_Playable");
            count += GenerateLevel1Group(FBXRecoder, "_FBX");
            count += GenerateLevel1Group(MaterialRecoder, "_Material");
            count += GenerateLevel1Group(ControllerRecoder, "_Controller");
            count += GenerateLevel1Group(PrefabRecoder, "_Prefab");
            count += GenerateLevel1Group(AssetRecoder, "_Asset");
            count += GenerateLevel1Group(TextureRecoder, "_Texture");
            count += GenerateLevel1Group(AniRecoder, "_Anim");
            count += GenerateLevel1Group(SignalRecoder, "_Signal");


            SceneRecoder.Clear();
            ShaderRecoder.Clear();
            FBXRecoder.Clear();
            MaterialRecoder.Clear();
            ControllerRecoder.Clear();
            PlayableRecoder.Clear();
            PrefabRecoder.Clear();
            AssetRecoder.Clear();
            TerrainLayerRecoder.Clear();
            TextureRecoder.Clear();
            AniRecoder.Clear();
            SignalRecoder.Clear();
        }

        Debug.LogFormat("Build -> End CollectAssetBundles() Count = {0}", count);
        return rlt;
    }

    public static Dictionary<string, int> SignalRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> SceneRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> ShaderRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> PlayableRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> FBXRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> MaterialRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> ControllerRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> PrefabRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> AssetRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> TerrainLayerRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> TextureRecoder = new Dictionary<string, int>();
    public static Dictionary<string, int> AniRecoder = new Dictionary<string, int>();
    public static List<string> _outAssetPaths = new List<string>(4096);
    public static HashSet<string> _mainAssets = new HashSet<string>();

    public static string AnimationClip_Monster_Dir = "Assets/ResourcesAB/AnimationClip/Monster";
    public static string AnimationClip_NPC_Dir = "Assets/ResourcesAB/AnimationClip/NPC";
    public static string MainAsset_Dir = "Assets/ResourcesAB";
    public static bool JudageAddressable(string assetPath, string groupName)
    {
        bool addressable = false;
        string groupPath;
        string secondPath;
        switch (groupName)
        {
            case "Shader":
                groupPath = string.Format("Assets/{0}/{1}", AssetPath.sResourcesRootDir, groupName);
                if (assetPath.Contains(groupPath) && assetPath.EndsWith(".shader"))
                    addressable = true;
                else
                    addressable = false;
                break;
            case "Emoji":
                groupPath = string.Format("Assets/{0}/{1}", AssetPath.sResourcesRootDir, groupName);
                if (assetPath.Contains(groupPath) && assetPath.EndsWith(".asset"))
                    addressable = true;
                else
                    addressable = false;
                break;
            case "Prefab":
                groupPath = string.Format("Assets/{0}/{1}", AssetPath.sResourcesRootDir, groupName);
                if (assetPath.Contains(groupPath) && assetPath.EndsWith(".prefab"))
                    addressable = true;
                else
                    addressable = false;
                break;
            case "Scene":
                groupPath = string.Format("Assets/{0}/{1}", AssetPath.sResourcesRootDir, "SceneBase");
                secondPath = string.Format("Assets/{0}/{1}", AssetPath.sResourcesRootDir, "SceneExport");
                if ((assetPath.Contains(groupPath) || assetPath.Contains(secondPath)) && assetPath.EndsWith(".unity"))
                    addressable = true;
                else
                    addressable = false;
                break;
            case "SceneExport":
                groupPath = string.Format("Assets/{0}/{1}", AssetPath.sResourcesRootDir, groupName);
                if (assetPath.Contains(groupPath) || assetPath.EndsWith(".asset"))
                    addressable = true;
                else
                    addressable = false;
                break;
            case "Material":
                groupPath = string.Format("Assets/{0}/{1}", AssetPath.sResourcesRootDir, groupName);
                if (assetPath.Contains(groupPath) || assetPath.EndsWith(".mat"))
                    addressable = true;
                else
                    addressable = false;
                break;
        }

        return addressable;
    }

    public static int CollectAssetBundle(AssetBundleSettingCell cell, AddressableAssetSettings settings = null, bool enableTestSence = false)
    {
        int rlt = 0;

        string relatedDirPath = cell.sRelativePath;
        string searthPattern = cell.sFilter;
        string overrideName = cell.sOverrideName;
        bool useOverrideName = !string.IsNullOrWhiteSpace(overrideName);
        SearchOption searchOption = cell.eSearchOption;
        string[] filters = searthPattern.Split('|');

        //显示用
        string titleName = string.Format("收集资源{0}", relatedDirPath);

        //要处理文件夹路径
        string folderPath;
        if (string.IsNullOrWhiteSpace(relatedDirPath))
        {
            //如果是ResourcesAB目录
            folderPath = string.Format("{0}/{1}", Application.dataPath, AssetPath.sResourcesRootDir);
            searchOption = SearchOption.TopDirectoryOnly;
        }
        else
        {
            folderPath = string.Format("{0}/{1}/{2}", Application.dataPath, AssetPath.sResourcesRootDir, relatedDirPath);
            //searchOption = SearchOption.AllDirectories;
        }

        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("不存在 folderPath：" + folderPath);
            return rlt;
        }

        DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
        List<FileInfo> fileInfos = new List<FileInfo>();

        for (int i = 0; i < filters.Length; ++i)
        {
            string filter = filters[i];
            FileInfo[] fileInfoArr = dirInfo.GetFiles(filter, searchOption);
            fileInfos.AddRange(fileInfoArr);
        }

        for (int i = 0; i < fileInfos.Count; ++i)
        {
            FileInfo fileInfo = fileInfos[i];
            string extension = fileInfo.Extension;

            if (string.Equals(extension, ".meta", StringComparison.Ordinal))
            {
                continue;
            }

            string fullName = fileInfo.FullName.Replace('\\', '/');
            string assetPath = "Assets" + fullName.Replace(Application.dataPath, "");

            //取消按钮事件
            if (EditorUtility.DisplayCancelableProgressBar(titleName, string.Format("({0}/{1}){2}", i.ToString(), fileInfos.Count.ToString(), assetPath), (float)i / (float)fileInfos.Count))
            {
                rlt = 1;
                break;
            }

            //1. 设置 组 和 地址 ======》
            string groupName = relatedDirPath;
            string address = assetPath.Remove(0, MainAsset_Dir.Length + 1);

            if (string.Equals(relatedDirPath, "AnimationClip", System.StringComparison.Ordinal))
            {
                //if (assetPath.StartsWith("Assets/Arts/Charactor/Accessories")) --到底 还要不要？
                //    continue;

                if (assetPath.StartsWith(AnimationClip_Monster_Dir, StringComparison.Ordinal))
                {
                    groupName = "AniMonsterLabel";
                    string filePath = assetPath.Substring(0, assetPath.LastIndexOf("/", System.StringComparison.Ordinal));
                    overrideName = filePath.Remove(0, AnimationClip_Monster_Dir.Length + 1);
                    useOverrideName = true;
                }
                else if (assetPath.StartsWith(AnimationClip_NPC_Dir, StringComparison.Ordinal))
                {
                    groupName = "AniNPCLabel";
                    string filePath = assetPath.Substring(0, assetPath.LastIndexOf("/", System.StringComparison.Ordinal));
                    overrideName = filePath.Remove(0, AnimationClip_NPC_Dir.Length + 1);
                    useOverrideName = true;
                }
                else
                {
                    overrideName = cell.sOverrideName;
                    useOverrideName =  !string.IsNullOrWhiteSpace(overrideName);
                }
            }
            else if (string.Equals(relatedDirPath, "Texture", System.StringComparison.Ordinal))
            {
                //策划表格不想改
                if (address.StartsWith("Texture/Big/", System.StringComparison.Ordinal))
                {
                    string filePath = address.Substring(0, address.LastIndexOf("/", System.StringComparison.Ordinal) + 1);
                    address = address.Replace(filePath, "Texture/Big/");
                }
            }
            else if (string.Equals(relatedDirPath, "SceneBase", System.StringComparison.Ordinal))
            {
                if (assetPath.EndsWith(".unity", System.StringComparison.Ordinal))
                {
                    address = address.Replace(groupName, "Scene");
                    groupName = "Scene";
                }
                else
                {
                    continue;
                }
            }
            else if (string.Equals(relatedDirPath, "SceneExport", System.StringComparison.Ordinal))
            {
                if (assetPath.EndsWith(".unity", System.StringComparison.Ordinal))
                {
                    //正式出包需要过滤测试场景
                    if (!enableTestSence && assetPath.IndexOf("_cutscene_", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        continue;
                    }

                    address = address.Replace(groupName, "Scene");
                    groupName = "Scene";
                }
                else if (assetPath.EndsWith("_InstanceData.asset", StringComparison.Ordinal) ||
                            assetPath.EndsWith("_LightMapData.asset", StringComparison.Ordinal))
                {
                    address = Path.GetFileNameWithoutExtension(assetPath);
                    groupName = "SceneData";
                }
                else
                {
                    continue;
                }
            }

#if USE_ATLAS_COMBINE
            else if (string.Equals(relatedDirPath, "AtlasExport", System.StringComparison.Ordinal))
            {
                address = address.Replace("AtlasExport/", "Atlas/");
            }
            else if (string.Equals(relatedDirPath, "UIExport", System.StringComparison.Ordinal))
            {
                address = address.Replace("UIExport/", "UI/");
            }
#endif

            //2. 检查地址合法性 ====》
            if (!FrameworkTool.IsAllUTF8(address))
            {
                //TODO 中止
                Debug.LogErrorFormat("文件路径包含不合理的字符 {0}", address);
                rlt = 2;
                break;
            }

            //3. 获取Group ====》
            AddressableAssetGroup assetGroup = settings.FindGroup(groupName);
            if (assetGroup == null)
            {
                assetGroup = settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
                BundledAssetGroupSchema bundledAssetGroupSchema = assetGroup.GetSchema<BundledAssetGroupSchema>();
                bundledAssetGroupSchema.UseAssetBundleCrc = false;//关闭bundle Crc 校验
                bundledAssetGroupSchema.UseAssetBundleCache = true;
                bundledAssetGroupSchema.UseAssetBundleCrcForCachedBundles = true;
                bundledAssetGroupSchema.IncludeGUIDInCatalog = false;
                bundledAssetGroupSchema.RetryCount = WebRequestRetryCount;
                bundledAssetGroupSchema.Timeout = WebRequestTimeOut;
                bundledAssetGroupSchema.AssetBundledCacheClearBehavior = BundledAssetGroupSchema.CacheClearBehavior.ClearWhenSpaceIsNeededInCache;

                //if (relatedDirPath == "Scene")
                bundledAssetGroupSchema.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.FullPath;
                //else
                //  bundledAssetGroupSchema.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.GUID;

                assetGroup.GetSchema<ContentUpdateGroupSchema>().StaticContent = false;//(重定向StreamingAsset)--by yd
                bundledAssetGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.FileNameHash; //使用hash作为 bundle名
                bundledAssetGroupSchema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
                bundledAssetGroupSchema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);

                if (useOverrideName)
                {
                    bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
                }
                else
                {
                    bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
                }
            }

            //4. 加入AssetEntry ===》
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetEntry assetEntry = settings.CreateOrMoveEntry(guid, assetGroup);
            assetEntry.address = address;
            if (useOverrideName)
            {
                assetEntry.SetLabel(overrideName, true, true);
            }

            //5. 收集主资源 ====》
            if (!_mainAssets.Contains(assetPath))
            {
                _mainAssets.Add(assetPath);
            }
            else
            {
                Debug.LogErrorFormat("存在相同的主资源 ：{0}", assetPath);
            }

            //6. 收集需要检查依赖的资源 ====》
            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            bool noDependencies =
                assetType == typeof(Texture2D) ||
                assetType == typeof(UnityEngine.AnimationClip) ||
                assetType == typeof(UnityEngine.U2D.SpriteAtlas) ||
                assetType == typeof(UnityEngine.AudioClip) ||
                assetType == typeof(UnityEngine.Font) ||
                assetType == typeof(UnityEngine.Shader) ||
                assetType == typeof(Framework.EmojiAsset);

            if (!noDependencies)
            {
                _outAssetPaths.Add(assetPath);
            }
        }

        EditorUtility.ClearProgressBar();
        return rlt;
    }

    private static int GetDependencies(string[] assetPaths, List<string> outAssetPaths)
    {
        int rlt = 0;
        for (int i = 0; i < assetPaths.Length; ++i)
        {
            string assetPath = assetPaths[i];

            string[] dependenPaths = AssetDatabase.GetDependencies(assetPath, false);

            for (int j = 0; j < dependenPaths.Length; ++j)
            {
                string dependenPath = dependenPaths[j];

                if (string.Equals(assetPath, dependenPath, StringComparison.Ordinal))
                    continue;

                if (dependenPath.EndsWith(".prefab", StringComparison.Ordinal))
                {
                    outAssetPaths.Add(dependenPath);
                    continue;
                }

                if (_mainAssets.Contains(dependenPath))
                    continue;

                //TODO 需要中止打包流程 提示先修复错误
                if (dependenPath.StartsWith("Assets/GameToolEditor"))
                {
                    rlt = 3;
                    Debug.LogErrorFormat("来自 Assets/GameToolEditor被引用到, {0} => {1}", assetPath, dependenPath);
                }

                string guid = AssetDatabase.AssetPathToGUID(dependenPath);

                if (dependenPath.EndsWith(".shader", StringComparison.Ordinal)
                    || dependenPath.EndsWith(".shadergraph", StringComparison.Ordinal))
                {
                    if (!ShaderRecoder.TryGetValue(guid, out int count))
                    {
                        //outAssetPaths.Add(dependenPath);
                    }
                    ShaderRecoder[guid] = count + 1;
                }
                else if (dependenPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)
                    || dependenPath.EndsWith(".obj", StringComparison.OrdinalIgnoreCase))
                {
                    if (!FBXRecoder.TryGetValue(guid, out int count))
                    {
                        outAssetPaths.Add(dependenPath);
                    }
                    FBXRecoder[guid] = count + 1;
                }
                else if (AssetDatabase.GetMainAssetTypeAtPath(dependenPath) == typeof(Texture2D))
                {
                    if (!dependenPath.StartsWith("Assets/Projects/Image", StringComparison.Ordinal))
                    {
                        if (!TextureRecoder.TryGetValue(guid, out int count))
                        {
                            //outAssetPaths.Add(dependenPath);
                        }
                        TextureRecoder[guid] = count + 1;
                    }
                }
                else if (dependenPath.EndsWith(".anim", StringComparison.Ordinal))
                {
                    if (!AniRecoder.TryGetValue(guid, out int count))
                    {
                        //outAssetPaths.Add(dependenPath);
                    }
                    AniRecoder[guid] = count + 1;
                }
                else if (dependenPath.EndsWith(".mat", StringComparison.Ordinal))
                {
                    if (!MaterialRecoder.TryGetValue(guid, out int count))
                    {
                        outAssetPaths.Add(dependenPath);
                    }
                    MaterialRecoder[guid] = count + 1;
                }
                else if (dependenPath.EndsWith(".controller", StringComparison.Ordinal))
                {
                    if (!ControllerRecoder.TryGetValue(guid, out int count))
                    {
                        outAssetPaths.Add(dependenPath);
                    }
                    ControllerRecoder[guid] = count + 1;
                }
                else if (dependenPath.EndsWith(".playable", StringComparison.Ordinal))
                {
                    if (!PlayableRecoder.TryGetValue(guid, out int count))
                    {
                        outAssetPaths.Add(dependenPath);
                    }
                    PlayableRecoder[guid] = count + 1;
                }
                else if (dependenPath.EndsWith(".asset", StringComparison.Ordinal))
                {
                    if (!AssetRecoder.TryGetValue(guid, out int count))
                    {
                        outAssetPaths.Add(dependenPath);
                    }
                    AssetRecoder[guid] = count + 1;
                }
                else if (dependenPath.EndsWith(".terrainlayer", StringComparison.Ordinal))
                {
                    if (!TerrainLayerRecoder.TryGetValue(guid, out int count))
                    {
                        outAssetPaths.Add(dependenPath);
                    }
                    TerrainLayerRecoder[guid] = count + 1;
                }
                else if (dependenPath.EndsWith(".signal", StringComparison.Ordinal))
                {
                    if (!SignalRecoder.TryGetValue(guid, out int count))
                    {
                        outAssetPaths.Add(dependenPath);
                    }
                    SignalRecoder[guid] = count + 1;
                }
                else if (dependenPath.EndsWith(".unity", StringComparison.Ordinal))
                {
                    //TODO 需要中止打包流程 提示先修复错误
                    rlt = 4;
                    Debug.LogErrorFormat("{0} => {1}", assetPath, dependenPath);

                    if (!SceneRecoder.TryGetValue(guid, out int count))
                    {
                        outAssetPaths.Add(dependenPath);
                    }
                    SceneRecoder[guid] = count + 1;
                }
                else
                {
                    if (!dependenPath.EndsWith(".cs", StringComparison.Ordinal))
                    {
                        Debug.LogFormat("Other {0} => {1}", assetPath, dependenPath);
                    }
                }
            }
        }

        return rlt;
    }

    private static int GenerateLevel1Group(Dictionary<string, int> TempHashSet, string groupName, int miniCount = 2, string LabelName = null)
    {
        if (TempHashSet.Count < 1)
            return 0;

        AddressableAssetSettings assetSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);

        bool useOverrideName = !string.IsNullOrEmpty(LabelName);
        AddressableAssetGroup assetGroup = assetSettings.FindGroup(groupName);
        if (assetGroup == null)
        {
            assetGroup = assetSettings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            BundledAssetGroupSchema bundledAssetGroupSchema = assetGroup.GetSchema<BundledAssetGroupSchema>();
            bundledAssetGroupSchema.UseAssetBundleCrc = false;//关闭bundle Crc 校验
            bundledAssetGroupSchema.UseAssetBundleCache = true;
            bundledAssetGroupSchema.UseAssetBundleCrcForCachedBundles = true;
            bundledAssetGroupSchema.IncludeGUIDInCatalog = false;
            bundledAssetGroupSchema.RetryCount = WebRequestRetryCount;
            bundledAssetGroupSchema.Timeout = WebRequestTimeOut;
            bundledAssetGroupSchema.AssetBundledCacheClearBehavior = BundledAssetGroupSchema.CacheClearBehavior.ClearWhenSpaceIsNeededInCache;
            bundledAssetGroupSchema.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.FullPath;


            assetGroup.GetSchema<ContentUpdateGroupSchema>().StaticContent = false;//(重定向StreamingAsset)--by yd
            bundledAssetGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.FileNameHash; //使用hash作为 bundle名
            bundledAssetGroupSchema.BuildPath.SetVariableByName(assetSettings, AddressableAssetSettings.kLocalBuildPath);
            bundledAssetGroupSchema.LoadPath.SetVariableByName(assetSettings, AddressableAssetSettings.kLocalLoadPath);

            if (useOverrideName)
            {
                bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
            }
            else
            {
                bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
            }
        }

        int count = 0;

        foreach (var item in TempHashSet)
        {
            if (item.Value < miniCount)
                continue;

            string guid = item.Key;
            //string path = AssetDatabase.GUIDToAssetPath(guid);

            AddressableAssetEntry assetEntry = assetSettings.CreateOrMoveEntry(guid, assetGroup);
            assetEntry.address = guid;
            if (useOverrideName)
            {
                assetEntry.SetLabel(LabelName, true, true);
            }

            ++count;
        }

        if (count < 1)
        {
            assetSettings.RemoveGroup(assetGroup);
        }

        Debug.LogFormat("{0} share = {1} dependence = {2}", groupName, count.ToString(), (TempHashSet.Count - count).ToString());
        return count;
    }

    private static int BuildAssetBundle(BuildTarget target, BuildAssetBundleOptions options)
    {
        Debug.LogFormat("Build -> Start BuildAssetBundle(bRebuild = {0}, options = [{1}])", target, options);

        string platformName = Enum.GetName(typeof(BuildTarget), target);
        string assetGenDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAssetBundleDir);

        if (!Directory.Exists(assetGenDir))
        {
            Directory.CreateDirectory(assetGenDir);
        }

        if (Directory.Exists(assetGenDir))
        {
            if ((options & BuildAssetBundleOptions.ForceRebuildAssetBundle) != 0)
            {
                // 强制把AssetBundle目录删除一下
                if (Directory.Exists(assetGenDir))
                {
                    Directory.Delete(assetGenDir, true);
                    Directory.CreateDirectory(assetGenDir);
                }
            }
        }
        else
        {
            Directory.CreateDirectory(assetGenDir);
        }

        //BuildPipeline.BuildAssetBundles(buildDir, options, target);
        AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[mAssetBundleBuilds.Count];
        mAssetBundleBuilds.Values.CopyTo(assetBundleBuilds, 0);
        mAssetBundleBuilds.Clear();

        BuildPipeline.BuildAssetBundles(assetGenDir, assetBundleBuilds, options, target);
        EditorUtility.ClearProgressBar();

        Debug.Log("Build -> End BuildAssetBundle()");
        return 0;
    }
    private static int LibraryAssetCopyToAddressableAsset(string platformName, string minimalVersion)
    {
        //文件夹的copy  【Library\com.unity.addressables\StreamingAssetsCopy】 拷贝到 【AssetPublish/对应版本文件下】
        int rst = 0;

        string sourceDirAddressable = string.Format("{0}/{1}", Application.dataPath, AddressableOriPath);
        //string SourceBundlePath = string.Format("{0}/{1}", Application.dataPath, AddressableServerDataPath);
        if (!Directory.Exists(sourceDirAddressable))
        {
            Debug.LogErrorFormat("不存在源目录 {0}", sourceDirAddressable);
            rst = -5;
            return rst;
        }

        string DestionDirAddressable = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion, AddressableBin);
        FileTool.CreateFolder(DestionDirAddressable);

        string platMapPath = PlatformMappingService.GetPlatformPathSubFolder().ToString();
        //string sourceDirNameaa = string.Format("{0}/{1}/{2}", sourceDirAddressable, AssetPath.sAddressableDir, platMapPath);
        string sourceDirNamebin = string.Format("{0}/{1}", sourceDirAddressable, platMapPath);
        string destDirNamebin = string.Format("{0}/{1}", sourceDirAddressable, platMapPath);
        //string sourceServerData = string.Format("{0}/{1}", SourceBundlePath, platformName);

        //拷贝aa 包含bundle
        //CopyAsset(sourceDirNameaa, string.Format("{0}/{1}/{2}", DestionDirAddressable, AssetPath.sAddressableDir, platMapPath), "拷贝Library Addressable Asset aa -> 资源原始目录 0", true, null);

        //拷贝bin
        //CopyAsset(sourceDirNamebin, Path.Combine(DestionDirAddressable, platMapPath), "拷贝Library Addressable Asset bin -> 资源原始目录 0", true, null);

        //拷贝bin单个文件
        File.Copy(sourceDirNamebin, destDirNamebin);



        //拷贝Layout
        string layoutSrc = string.Format("{0}/{1}", sourceDirAddressable, "buildlayout.txt");
        string layoutDes = string.Format("{0}/{1}/{2}/{3}/buildlayout.txt", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion);
        File.Copy(layoutSrc, layoutDes);
        AssetDatabase.Refresh();
        return rst;
    }
    private static int GenAssetBundle(BuildSetting buildSetting, BuildTarget target, string version, EHotFixMode eHotFixMode, VersionSetting versionSetting)
    {
        int rlt = 0;

        List<AssetBundleSettingCell> bundleSettingCells = buildSetting.mAssetBundleSettingCells;

        //string maximalVersion = VersionHelper.GetMaximalVersion(version);
        string minimalVersion = VersionHelper.GetMinimalVersion(version);

        int curVersion = 0;
        if (!int.TryParse(minimalVersion, out curVersion))
        {
            Debug.LogErrorFormat("版本格式错误 version = {0}", version);
            return 5;
        }
        string platformName = Enum.GetName(typeof(BuildTarget), target);

        //打整包(资源版本<=0)
        if (curVersion <= 0)//首包
        {
            //每次打包都需要清除缓存,因为多次缓存没清 会导致打包资源问题
            if (buildSetting.bClearCache)
            {
                AddressableAssetSettings.CleanPlayerContent(null);
                BuildCache.PurgeCache(false);
            }

            SetAddressableAssetSettings(version);
            rlt = GenLocalAsset_Addressable(buildSetting);
            if (rlt == 0)
                rlt = ReadLolcalHash(versionSetting, target);
        }
        else
        {
            GenHotFixAsset_Addressable(buildSetting, platformName);
            //rlt = DeleteNoUseCacheBundleAsset(target);
        }

        //copy一份到AssetBundle （主要用于android平台带sdk 导出到sdk project，减少打包时间）
        rlt = CopyAssetBundleToStream(target, ECopyAssetMode.eCopyABToAssetBundleBuildPath);

        return rlt;
    }
    public static int DeleteNoUseCacheBundleAsset(BuildTarget target)
    {
        int rlt = 0;
        string platformName = Enum.GetName(typeof(BuildTarget), target);
        //获取当前生成的bunde列表
        string filePath = string.Format("{0}/{1}", HotFixBinaryFile, "NewBundleList.bytes");
        if (!File.Exists(filePath))
        {
            rlt = -2;
            return rlt;
        }

        List<string> NewBundleList = ReadBinary(filePath) as List<string>;
        if (NewBundleList != null && NewBundleList.Count > 0)
        {
            //过滤一下
            List<string> tempBundleFiles = new List<string>();
            foreach (var item in NewBundleList)
            {
                int index = item.LastIndexOf("/") + 1;
                string tempPath = item.Substring(index);
                tempBundleFiles.Add(tempPath);
            }

            //删除缓存不使用的资源
            string cacheBundlePath = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AddressableOriPath, AssetPath.sAddressableDir, PlatformMappingService.GetPlatformPathSubFolder(), platformName);
            DirectoryInfo dirInfo = new DirectoryInfo(cacheBundlePath);
            FileInfo[] fileInfoArr = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            List<string> needDeleteBundleList = new List<string>();
            for (int i = 0; i < fileInfoArr.Length; ++i)
            {
                FileInfo fileInfo = fileInfoArr[i];
                string fileName = fileInfo.Name;
                if (!tempBundleFiles.Contains(fileName))
                {
                    string fileFullPath = fileInfo.FullName;
                    needDeleteBundleList.Add(fileFullPath);
                }
            }
            foreach (var item in needDeleteBundleList)
            {
                Debug.Log("删除Library下不使用的资源：" + item);
                File.Delete(item);
            }
        }

        return rlt;
    }
    public static void SetAddressableAssetSettings(string version)
    {
        //2.设置 启动时不启用目录更新，以及设置相关版本号
        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        m_Settings.DisableCatalogUpdateOnStartup = true;
        m_Settings.BuildRemoteCatalog = true;
        m_Settings.RemoteCatalogBuildPath.SetVariableByName(m_Settings, AddressableAssetSettings.kRemoteBuildPath);
        m_Settings.RemoteCatalogLoadPath.SetVariableByName(m_Settings, AddressableAssetSettings.kRemoteLoadPath);

        //3.设置首包的构建远端资源路径（catalog.json and catalog.hash）
        string defaultRemoteBuildPathStr = "ServerData/[BuildTarget]";
        m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kRemoteBuildPath, defaultRemoteBuildPathStr);
        if (Directory.Exists("ServerData"))
        {
            Directory.Delete("ServerData", true);
        }

        //4.设置远端加载路径 字符串
        string defaultRemoteLoadPathStr = "{UnityEngine.AddressableAssets.PathRebuild.RemoteLoadPath}";
        m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kRemoteLoadPath, defaultRemoteLoadPathStr);

        //5.只需要打首包设置版本号
        m_Settings.OverridePlayerVersion = version;

        //6.设置同时开启的网络请求 
        m_Settings.MaxConcurrentWebRequests = 3;

        //7.如果设置，则根据源资产的顺序将资产连续打包成bundle，从而提高资产加载时间。
        m_Settings.ContiguousBundles = true;

        //8.catalog 下载超时时间
        m_Settings.CatalogRequestsTimeout = 10;

        //9.设置静态受限资源，热更的生成方式（在这里没用到 关闭）
        m_Settings.CheckForContentUpdateRestrictionsOption =  CheckForContentUpdateRestrictionsOptions.Disabled;

        //10.默认设置 layout的生成文件格式为 TXT
        ProjectConfigData.BuildLayoutReportFileFormat = ProjectConfigData.ReportFileFormat.TXT;

        //8.存储内存初始化设置  手动加入
        //{UnityEngine.Application.persistentDataPath}/Caching
        //var cacheInitSetting = new CacheInitializationSettings();
        //cacheInitSetting.CreateObjectInitializationData();

    }

    public static int GenLocalAsset_Addressable(BuildSetting buildSetting)
    {
        int rlt = 0;
        //1.生成本地Group
        if (buildSetting.bRebuildGroup)
        {
            rlt = GenLocalAssetGroup_Addressable(buildSetting, true);
        }

        if (rlt != 0)
            return rlt;

        //2.打包 Bundle
        //AddressableAssetSettings.BuildPlayerContent();
        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult rst);
        if (!string.IsNullOrEmpty(rst.Error))
        {
            Debug.LogFormat("bundle 生成失败,原因:{0}",rst.Error);
            rlt = -5;
        }

        //3.刷新一下
        AssetDatabase.Refresh();

        return rlt;
    }
    public static int GenLocalAssetGroup_Addressable(BuildSetting buildSetting, bool FixIssues = true)
    {
        System.Diagnostics.Stopwatch swP = new System.Diagnostics.Stopwatch();
        swP.Start();

        int rlt = 0;

        List<AssetBundleSettingCell> bundleSettingCells = buildSetting.mAssetBundleSettingCells;

        //清理group
        AddressableAssetSettings assetSettings = ClearAddressableGroup();

        //重新构建group
        rlt = RebuildAddressableGroup(bundleSettingCells, EAssetProcessing.eGetAssetAddress, assetSettings, false);
        if (rlt == 0)
        {
            if (FixIssues)
            {
                MyCheckBundleDupeDependencies myCheckBundle = new MyCheckBundleDupeDependencies();
                myCheckBundle.RefreshAnalysis(assetSettings);
                myCheckBundle.FixIssues(assetSettings);
            }

            //使用自己的分析方法
            //ABDependsAnalyze.Analyze(false);

            AssetDatabase.Refresh();
        }

        swP.Stop();
        TimeSpan ts = swP.Elapsed;
        if (FixIssues)
        {
            Debug.LogError(string.Format("资源重新收集 + 分析 + 修正 时间总和:{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds));
        }
        else
        {
            Debug.LogError(string.Format("资源重新收集 时间总和:{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds));
        }
        return rlt;
    }
    private static int CopyAsset(string srcDir, string desDir, string title, bool clearOld, List<string> ignore = null)
    {
        srcDir = Path.GetFullPath(srcDir);
        desDir = Path.GetFullPath(desDir);
        int l = srcDir.Length;
        int rlt = 0;

        if (!Directory.Exists(srcDir))
        {
            Debug.LogErrorFormat("不存在源目录 {0}", srcDir);
            return 2;
        }

        if (clearOld && Directory.Exists(desDir))
        {
            Directory.Delete(desDir, true);
        }

        if (!Directory.Exists(desDir))
        {
            Directory.CreateDirectory(desDir);
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(srcDir);
        DirectoryInfo[] infos = directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);

        List<DirectoryInfo> directoryInfos = new List<DirectoryInfo>(infos.Length + 1);
        directoryInfos.Add(directoryInfo);
        directoryInfos.AddRange(infos);

        for (int dirIndex = 0; dirIndex < directoryInfos.Count; ++dirIndex)
        {
            FileInfo[] fileInfos = directoryInfos[dirIndex].GetFiles("*.*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < fileInfos.Length; ++i)
            {
                FileInfo fileInfo = fileInfos[i];
                if (ignore != null && ignore.Contains(fileInfo.Extension))
                {
                    continue;
                }

                string srcPath = fileInfo.FullName;
                if (EditorUtility.DisplayCancelableProgressBar(title, srcPath, (float)i / fileInfos.Length))
                {
                    rlt = 1;
                    break;
                }
                string desPath = desDir + srcPath.Remove(0, l);

                string dir = Path.GetDirectoryName(desPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.Copy(srcPath, desPath);
            }

            if (rlt != 0)
            {
                break;
            }
        }
        EditorUtility.ClearProgressBar();
        return rlt;
    }
    private static int EncryptAsset(string srcDir, string desDir, string title, bool clearOld, List<string> ignore = null)
    {
        srcDir = Path.GetFullPath(srcDir);
        desDir = Path.GetFullPath(desDir);
        int l = srcDir.Length;
        int rlt = 0;

        if (!Directory.Exists(srcDir))
        {
            Debug.LogErrorFormat("不存在源目录 {0}", srcDir);
            return 2;
        }

        if (clearOld && Directory.Exists(desDir))
        {
            Directory.Delete(desDir, true);
        }

        if (!Directory.Exists(desDir))
        {
            Directory.CreateDirectory(desDir);
        }

        //优先判断有没有对应平台的目录
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        string platformName = Enum.GetName(typeof(BuildTarget), buildTarget);
        string videoPath = string.Format("{0}/{1}/Video/{2}", Application.dataPath, AssetPath.sAssetCsvDir, platformName);
        bool videoHaveDir = Directory.Exists(videoPath);
        string[] folders = null;
        if (!videoHaveDir)
        {
            folders = Directory.GetDirectories(string.Format("{0}/{1}/Video", Application.dataPath, AssetPath.sAssetCsvDir));
            for (int i = 0; i < folders.Length; i++)
            {
                string tempStr = folders[i];
                folders[i] = tempStr.Replace("\\", "/");
            }
        }


        DirectoryInfo directoryInfo = new DirectoryInfo(srcDir);
        FileInfo[] fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

        for (int i = 0; i < fileInfos.Length; ++i)
        {
            FileInfo fileInfo = fileInfos[i];
            if (ignore.Contains(fileInfo.Extension))
            {
                continue;
            }

            string srcPath = fileInfo.FullName.Replace("\\", "/");
            if (EditorUtility.DisplayCancelableProgressBar(title, srcPath, (float)i / fileInfos.Length))
            {
                rlt = 1;
                break;
            }
            string desPath = desDir + srcPath.Remove(0, l);

            //（Config/Video）目录下的资源不加密
            string videoFile = "Assets/Config/Video/"; //@"Assets\Config\Video\";
            if (srcPath.Contains(videoFile))
            {
                //D:\Projects\crossgate\client\CrossGate\Assets\Config\Video\StandaloneWindows64\cutscene_1000_cin.mp4
                //特殊处理，如果Video有对应平台目录，则优先copy目录里的，没有对应平台目录在copy平台外的video
                if (videoHaveDir)
                {
                    string fileName = srcPath.Remove(0, l);
                    if (fileName.Contains(platformName))
                    {
                        fileName = fileName.Replace(string.Format("/{0}", platformName), "");
                        desPath = desDir + fileName;

                        string dir = Path.GetDirectoryName(desPath);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        File.Copy(srcPath, desPath);
                    }
                    else
                        continue;
                }
                else
                {
                    //只copy文件夹外部的，其他平台要过滤掉 这里只有一个 StandaloneWindows64 文件夹 -- 特殊处理
                    if (folders != null && folders.Length >= 0 && srcPath.Contains(folders[0]))
                    {
                        continue;
                    }
                    else
                    {
                        string dir = Path.GetDirectoryName(desPath);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        File.Copy(srcPath, desPath);
                    }
                }
            }
            else
            {
                _EncryptAsset(srcPath, desPath);
            }
        }
        EditorUtility.ClearProgressBar();
        return rlt;
    }
    private static void _EncryptAsset(string src, string des, string password = null)
    {
        byte[] data = File.ReadAllBytes(src);
        string dir = Path.GetDirectoryName(des);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        //using (EncryptFileStream encryptStream = new EncryptFileStream(des, FileMode.Create))
        //{
        //    encryptStream.Write(data, 0, data.Length);
        //}

        Unity.SharpZipLib.Checksum.Crc32 crc32 = new Unity.SharpZipLib.Checksum.Crc32();
        crc32.Reset();
        crc32.Update(data);

        using (FileStream fileStream = File.Create(des))
        using (Unity.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new Unity.SharpZipLib.Zip.ZipOutputStream(fileStream))
        {
            zipOutputStream.SetLevel(6);

            if (!string.IsNullOrWhiteSpace(password))
            {
                zipOutputStream.Password = password;
            }
            
            Unity.SharpZipLib.Zip.ZipEntry zipEntry = new Unity.SharpZipLib.Zip.ZipEntry(Path.GetFileName(des));

            zipEntry.DateTime = Consts.sConfigDate;//DateTime.Now;
            zipEntry.Size = data.Length;
            zipEntry.Crc = crc32.Value;

            zipOutputStream.PutNextEntry(zipEntry);
            zipOutputStream.Write(data, 0, data.Length);
            zipOutputStream.CloseEntry();
        }            
    }
    private static int GenConfig(BuildTarget target)
    {

        int rlt = 0;
        string platformName = Enum.GetName(typeof(BuildTarget), target);

        string resourcesABDir = string.Format("{0}/{1}", Application.dataPath, AssetPath.sAssetCsvDir);
        string assetGenDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAssetCsvDir);

        rlt = EncryptAsset(resourcesABDir, assetGenDir, "生成加密config", true, new List<string> { ".meta" });


        return rlt;
    }
    private static int GenLogic(BuildTarget target, BuildOptions buildOptions)
    {
        int rlt = 0;

        string platformName = Enum.GetName(typeof(BuildTarget), target);

        string resourcesABDir = string.Format("{0}/{1}", Application.dataPath, AssetPath.sAssetCodeDir);
        string assetGenDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAssetLogicDir);

        // 强制把AssetBundlePublish目录删除一下
        if (Directory.Exists(assetGenDir))
        {
            Directory.Delete(assetGenDir, true);
        }
        Directory.CreateDirectory(assetGenDir);

        if (Directory.Exists(resourcesABDir))
        {
            int count = Application.dataPath.Length + 1;

            string[] files = Directory.GetFiles(resourcesABDir, "*.cs", SearchOption.AllDirectories);

            Assembly[] assemblys = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();
            Assembly assembly = null;
            for (int i = 0; i < assemblys.Length; ++i)
            {
                if (string.Equals(assemblys[i].name, Path.GetFileNameWithoutExtension(AssetPath.sLogicDllName), StringComparison.Ordinal))
                {
                    assembly = assemblys[i];
                    break;
                }
            }

            if (assembly == null)
            {
                Debug.LogError("未找到Logic 程序集");
                return 3;
            }

            UnityEditor.Compilation.AssemblyBuilder assemblyBuilder = new UnityEditor.Compilation.AssemblyBuilder(assetGenDir + "/" + AssetPath.sLogicDllName, assembly.sourceFiles);
            assemblyBuilder.buildTarget = target;
            assemblyBuilder.buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            assemblyBuilder.compilerOptions = assembly.compilerOptions;
            assemblyBuilder.excludeReferences = new string[] { Application.dataPath + "/Plugins/ThirdLib/Runtime/ThirdLib.asmdef", Application.dataPath + "/Scripts/Framework/Framework.asmdef" };
            assemblyBuilder.additionalReferences = assembly.compiledAssemblyReferences;
            assemblyBuilder.flags = (buildOptions & BuildOptions.Development) != BuildOptions.None ? AssemblyBuilderFlags.DevelopmentBuild : AssemblyBuilderFlags.None;

            Debug.Log(string.Join(";", assemblyBuilder.additionalReferences));
            Debug.Log(string.Join(";", assemblyBuilder.defaultDefines));

            assemblyBuilder.buildStarted += delegate (string assemblyPath)
            {
                Debug.LogFormat("Assembly build start for {0}", assemblyPath);
            };

            assemblyBuilder.buildFinished += delegate (string assemblyPath, UnityEditor.Compilation.CompilerMessage[] compilerMessages)
            {
                foreach (UnityEditor.Compilation.CompilerMessage compilerMessage in compilerMessages)
                {
                    if (compilerMessage.type == UnityEditor.Compilation.CompilerMessageType.Error)
                    {
                        Debug.LogError(compilerMessage.message);
                    }
                }
            };

            EditorUtility.DisplayCancelableProgressBar("assemblyBuilder.Build", assetGenDir, 0.3f);
            if (!assemblyBuilder.Build())
            {
                Debug.LogErrorFormat("Faild to Build {0}", assemblyBuilder.assemblyPath);
                EditorUtility.DisplayDialog("错误", string.Format("Logic 编译失败", AssetPath.sLogicDllName), "确定");
                rlt = 4;
            }
            else
            {
                float p = 0.3f;
                while (assemblyBuilder.status != UnityEditor.Compilation.AssemblyBuilderStatus.Finished)
                {
                    if (p <= 0.8f)
                        p += 0.01f;
                    EditorUtility.DisplayCancelableProgressBar("AssemblyBuilder.Build() Logic.dll", assetGenDir, p);
                }

                //需要在copy一份放到Assets/LogicDll 文件下供生成桥接代码用。
                Startup.CopyDllFromLogicDll(platformName);

                //不需要调试信息
                string mdbPath = assetGenDir + "/" + AssetPath.sLogicPdbName;
                File.Delete(mdbPath);

                string dllPath = assetGenDir + "/" + AssetPath.sLogicDllName;
                if (File.Exists(dllPath))
                {
                    EditorUtility.DisplayCancelableProgressBar("加密Logic.dll", AssetPath.sLogicDllName, 0.8f);
                    _EncryptAsset(dllPath, dllPath + ".bytes", Consts.sLogicPassword);
                    File.Delete(dllPath);
                }
            }
        }

        EditorUtility.ClearProgressBar();
        return rlt;
    }
    public static int GenLogic2(BuildTarget target, BuildOptions buildOptions, bool useDebugMode)
    {
        int rlt = 0;
        string platformName = Enum.GetName(typeof(BuildTarget), target);
        string assetGenDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAssetLogicDir);
        if (Directory.Exists(assetGenDir))
            Directory.Delete(assetGenDir, true);
        Directory.CreateDirectory(assetGenDir);

        //copy logic
        string dllOriPath = string.Format("Assets/{0}/{1}.bytes", AssetPath.sAssetLogicDir, AssetPath.sLogicDllName);
        string dllDestPath = Path.Combine(assetGenDir, AssetPath.sLogicDllName);
        File.Copy(dllOriPath, dllDestPath, true);
        AssetDatabase.Refresh();
        // Copy to AssetBundle文件夹加密
        if (File.Exists(dllDestPath))
        {
            File.Move(dllDestPath, dllDestPath + ".bytes");
            AssetDatabase.Refresh();
            EditorUtility.DisplayCancelableProgressBar("加密Logic.dll", AssetPath.sLogicDllName, 0.8f);
            _EncryptAsset(dllDestPath + ".bytes", dllDestPath + ".bytes", Consts.sLogicPassword);
        }
        else
        {
            Debug.LogErrorFormat("File is not Exists {0}", dllDestPath);
        }


        //debug 模式才会 开启远程pdb调试模式
        if (useDebugMode)
        {
            //copy pdb
            string pdbOriPath = string.Format("Assets/{0}/{1}.bytes", AssetPath.sAssetLogicDir, AssetPath.sLogicPdbName);
            string pdbDestPath = Path.Combine(assetGenDir, AssetPath.sLogicPdbName);
            File.Copy(pdbOriPath, pdbDestPath, true);
            AssetDatabase.Refresh();
            // Copy to AssetBundle文件夹加密
            if (File.Exists(pdbDestPath))
            {
                File.Move(pdbDestPath, pdbDestPath + ".bytes");
                AssetDatabase.Refresh();
                EditorUtility.DisplayCancelableProgressBar("加密pdb", AssetPath.sLogicPdbName, 0.8f);
                _EncryptAsset(pdbDestPath + ".bytes", pdbDestPath + ".bytes", Consts.sLogicPassword);
            }
            else
            {
                Debug.LogErrorFormat("File is not Exists {0}", pdbDestPath);
            }
        }


        EditorUtility.ClearProgressBar();

        return rlt;
    }
    #endregion


    #region 生成热更新资源
    private static int GenHotFixAsset(BuildTarget target, string version, VersionSetting versionSetting, BuildSetting buildsettings)
    {
        int rlt = 0;
        string title = string.Format("生成热更新资源 {0} {1}", target.ToString(), version);
        string maximalVersion = VersionHelper.GetMaximalVersion(version);
        string minimalVersion = VersionHelper.GetMinimalVersion(version);

        if (string.IsNullOrWhiteSpace(minimalVersion))
        {
            Debug.LogErrorFormat("版本格式错误 version = {0}", version);
            return 5;
        }

        int curVersion = 0;
        if (!int.TryParse(minimalVersion, out curVersion))
        {
            Debug.LogErrorFormat("版本格式错误 version = {0}", version);
            return 5;
        }

        //写入同一版本系列热更文件
        string HotFixUniqueIdentifierPath = string.Format("{0}/{1}", HotFixBinaryFile, "HotFixUniqueIdentifier.txt");
        if (!File.Exists(HotFixUniqueIdentifierPath))
        {
            Debug.LogError("不存在同一版本系列热更文件:" + HotFixUniqueIdentifierPath);
            return 5;
        }


        string platformName = Enum.GetName(typeof(BuildTarget), target);
        string bundleDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundleBuildPath, platformName);
        string curDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion);

        AssetList curConfigList = GenAssetsList(target, bundleDir);
        AssetList curAaList = GenAaAssetList(platformName);

        curAaList.Version = curConfigList.Version = string.Format("{0}.{1}", maximalVersion, minimalVersion);
        curConfigList.VersionIdentifier = File.ReadAllText(HotFixUniqueIdentifierPath);
        curAaList.VersionIdentifier = curConfigList.VersionIdentifier; 

        if (curVersion <= 0)
        {
            //直接拷贝到0
            rlt = CopyAssetTo0(platformName, version);
        }
        else
        {
            //热更(Config/Dll)文件生成
            GetHotFixAssetListByCompare(out AssetList curHotFixConfigList, curConfigList, EAssetBundleMode.eConfigDll, platformName, curVersion);
            curHotFixConfigList.Version = string.Format("{0}.{1}", maximalVersion, minimalVersion);
            curHotFixConfigList.VersionIdentifier = curConfigList.VersionIdentifier;
            string curHotFixPath1 = string.Format("{0}/{1}/{2}/{3}/HotFixConfigList.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion.ToString());
            string hotfixcontent1 = AssetList.Serialize(curHotFixConfigList);
            WriteFile(curHotFixPath1, hotfixcontent1);

            //热更(AaBundle)文件生成
            GetHotFixAssetListByCompare(out AssetList curAaUpdateList, curAaList, EAssetBundleMode.eAaBundle, platformName, curVersion);
            curAaUpdateList.Version = string.Format("{0}.{1}", maximalVersion, minimalVersion);
            curAaUpdateList.VersionIdentifier = curAaList.VersionIdentifier;
            string curHotFixPath2 = string.Format("{0}/{1}/{2}/{3}/HotFixAaList.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion.ToString());
            string hotfixcontent2 = AssetList.Serialize(curAaUpdateList);
            WriteFile(curHotFixPath2, hotfixcontent2);

            //合并成一个HotFixList写入
            AssetList curHotFixList = new AssetList();
            curHotFixList.Contents = curHotFixConfigList.Contents.Concat(curAaUpdateList.Contents).ToDictionary(k => k.Key, v => v.Value);
            curHotFixList.Version = string.Format("{0}.{1}", maximalVersion, minimalVersion);
            curHotFixList.VersionIdentifier = curConfigList.VersionIdentifier;
            string curHotFixPath = string.Format("{0}/{1}/{2}/{3}/HotFixList.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion.ToString());
            string hotfixcontent = AssetList.Serialize(curHotFixList);
            WriteFile(curHotFixPath, hotfixcontent);
        }

        //写入当前的AssetList(Config、dll)文件
        string curAssetListPath = string.Format("{0}/{1}/{2}/{3}/ConfigAssetList.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion.ToString());
        WriteFile(curAssetListPath, AssetList.Serialize(curConfigList));

        //写入当前的AssetList(AaBundle)文件
        string curAaAssetListPath = string.Format("{0}/{1}/{2}/{3}/AaAssetList.txt", Application.dataPath, AssetBundlePublishPath, platformName, curVersion.ToString());
        WriteFile(curAaAssetListPath, AssetList.Serialize(curAaList));

        //写入自己记录的Bundle列表NewBundleList
        string fileOriPath = string.Format("{0}/{1}", HotFixBinaryFile, "NewBundleList.json");
        string fileDesPath = string.Format("{0}/{1}/{2}/{3}/NewBundleList.json", Application.dataPath, AssetBundlePublishPath, platformName, curVersion);
        File.Copy(fileOriPath, fileDesPath, true);

        //记录 热更版本文件（标志着热更新资源成功与否）
        string curVersionPath = string.Format("{0}/{1}/{2}/Version.txt", Application.dataPath, AssetBundlePublishPath, platformName);
        File.WriteAllText(curVersionPath, minimalVersion);

        return rlt;
    }

    public static long GetHotFixAssetListByCompare(out AssetList curHotFixList, AssetList curAssetList, EAssetBundleMode eAssetBundleMode, string platformName, int curVersion)
    {
        int rlt = 0;
        curHotFixList = curAssetList;

        if (eAssetBundleMode == EAssetBundleMode.eConfigDll)
        {
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
                    curHotFixList = CompareAssetsList(curAssetList, befAssetList, curVersion, null, ref versionIdentifierIsSame, "Config/Dll");
                    break;
                }
            }

            if (versionIdentifierIsSame == false)
                return -5;

            string befHotFixPath = string.Format("{0}/{1}/{2}/{3}/HotFixConfigList.txt", Application.dataPath, AssetBundlePublishPath, platformName, befVersion.ToString());
            AssetList oldHotFixList;
            if (File.Exists(befHotFixPath))
            {
                string befHotFixContent = File.ReadAllText(befHotFixPath);
                oldHotFixList = AssetList.Deserialize(befHotFixContent);
                curHotFixList = MergeChangeList(curHotFixList, oldHotFixList);
            }

            int i = 0;
            int count = curHotFixList.Contents.Count;
            foreach (AssetsInfo info in curHotFixList.Contents.Values)
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
        }
        else
        {
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
                    curHotFixList = CompareAssetsList(curAssetList, befAssetList, curVersion, null, ref versionIdentifierIsSame, "Bundle");
                    break;
                }
            }

            if (versionIdentifierIsSame == false)
                return -5;


            string befHotFixPath = string.Format("{0}/{1}/{2}/{3}/HotFixAaList.txt", Application.dataPath, AssetBundlePublishPath, platformName, befVersion.ToString());
            AssetList oldHotFixList = null;
            if (File.Exists(befHotFixPath))
            {
                string befHotFixContent = File.ReadAllText(befHotFixPath);
                oldHotFixList = AssetList.Deserialize(befHotFixContent);
                curHotFixList = MergeChangeList(curHotFixList, oldHotFixList);
            }
            #endregion

            //bundle没有改变
            if (curHotFixList.Contents.Count <= 0)
                return -5;

            //获取Addresable新增 热更资源
            string needUpdateListOriPath = string.Format("{0}/{1}", HotFixBinaryFile, "NeedUpdateList.json");
            if (!File.Exists(needUpdateListOriPath))
            {
                Debug.LogError("不存在文件:{0}" + needUpdateListOriPath);
                return -5;
            }
            BundleListRes newAddBundleListRes = JsonUtility.FromJson<BundleListRes>(File.ReadAllText(needUpdateListOriPath));
            if (newAddBundleListRes == null || newAddBundleListRes.bundleListRes == null)
            {
                Debug.LogError("NeedUpdateList.json 内容为空");
                return -5;
            }
            //获取Addresable当前的热更资源列表
            string NewBundleListPath = string.Format("{0}/{1}", HotFixBinaryFile, "NewBundleList.json");
            if (!File.Exists(NewBundleListPath))
            {
                Debug.LogError("不存在文件:{0}" + NewBundleListPath);
                return -5;
            }
            BundleListRes curentBundleListRes = JsonUtility.FromJson<BundleListRes>(File.ReadAllText(NewBundleListPath));
            if (curentBundleListRes == null || curentBundleListRes.bundleListRes == null)
            {
                Debug.LogError("NewBundleList.json 内容为空");
                return -5;
            }


            //创建bundle目录文件
            string DestionFile = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundlePublishPath, platformName, curVersion, AssetPath.sAddressableDir);
            FileTool.CreateFolder(DestionFile);
            string DestBundleFile = string.Format("{0}/{1}", DestionFile, platformName);
            FileTool.CreateFolder(DestBundleFile);

            //copy NeedUpdateList.json
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
            if (contentCatalogData == null || contentCatalogData.InternalIds == null || contentCatalogData.InternalIds.Length <= 0)
                return -5;


            //copy 热更资源bundle
            string catalogPersistName = string.Format("{0}/catalog_{1}.json", AssetPath.sAddressableCatalogDir, m_Settings.PlayerBuildVersion);
            string orignalBundlePath = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AddressableOriPath, AssetPath.sAddressableDir, PlatformMappingService.GetPlatformPathSubFolder(), platformName);

            int i = 0;
            string internalId;
            int count = curHotFixList.Contents.Count;
            int deleteAndAddResCount = 0;
            List<string> bundleChangeList = new List<string>();
            Dictionary<string, BundleRes> updateBundleResDict = newAddBundleListRes.bundleListRes.ToDictionary(key => key.hashOfFileName, value => value);
            Dictionary<string, BundleRes> currentBundleResDict = curentBundleListRes.bundleListRes.ToDictionary(key => key.hashOfFileName, value => value);


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
                            Debug.Log(info.AssetName + "此资源在某个过程被删除后又新增");
                        }
                        else
                        {
                            rlt = -5;
                            Debug.LogError("NeedUpdateList NewBundleList 中都不存在 hashOfFileName：" + info.AssetName);
                            break;
                        }
                    }

                    for (int j = 0; j < contentCatalogData.InternalIds.Length; j++)
                    {
                        if (contentCatalogData.InternalIds[j].Equals(bundleRes.bundleFileId, StringComparison.Ordinal))
                        {
                            internalId = contentCatalogData.InternalIds[j];
                            contentCatalogData.InternalIds[j] = internalId.Replace("{UnityEngine.AddressableAssets.Addressables.RuntimePath}", "{UnityEngine.AddressableAssets.PathRebuild.RemoteLoadPath}");
                            break;
                        }
                    }

                    string srcPath = string.Format("{0}/{1}.bundle", orignalBundlePath, bundleRes.hashOfFileName);
                    string desPath = string.Format("{0}/{1}/{2}.bundle", DestionFile, platformName, bundleRes.hashOfFileName);
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

            string DesCatalogFile = string.Format("{0}/catalog_{1}.json", DestionFile, m_Settings.PlayerBuildVersion);
            File.WriteAllText(DesCatalogFile, JsonUtility.ToJson(contentCatalogData));
            string catalogHash = FrameworkTool.GetFileMD5(DesCatalogFile);
            FileInfo catalogInfo = new FileInfo(DesCatalogFile);
            ulong catalogSize = (ulong)catalogInfo.Length;
            string DesCatalogHashFile = string.Format("{0}/catalog_{1}.hash", DestionFile, m_Settings.PlayerBuildVersion);
            File.WriteAllText(DesCatalogHashFile, catalogHash);
            AssetDatabase.Refresh();

            curHotFixList.Contents.TryGetValue(catalogPersistName, out AssetsInfo assetsInfo1);
            assetsInfo1.AssetMD5 = catalogHash;
            assetsInfo1.Size = catalogSize;

            curAssetList.Contents.TryGetValue(catalogPersistName, out AssetsInfo assetsInfo2);
            assetsInfo2.AssetMD5 = catalogHash;
            assetsInfo2.Size = catalogSize;

            #region 主要用于测试 Addressable 和 列表记录 热更资源存在差异

            foreach (var item in newAddBundleListRes.bundleListRes)
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

            //有可能某个资源在0保存有，1删除，2又加进来，那么在统计数量上面 就会不一样 （已排除这种情况）
            if (bundleChangeList.Count != (newAddBundleListRes.bundleListRes.Count + deleteAndAddResCount))
                Debug.LogError(string.Format("Addressable计算热更：{0},热更配置计算热更：{1}, 生成的热更新资源数量不一样，！！！", newAddBundleListRes.bundleListRes.Count, bundleChangeList.Count));
            else
                Debug.Log(string.Format("0->{0}，需要更新Bundle资源 {1}", curVersion, bundleChangeList.Count + 1));



            //if (!File.Exists(needUpdatefile)) return 0;

            //Dictionary<string, List<string>> needMoveFiles = ReadBinary(needUpdatefile) as Dictionary<string, List<string>>;
            //if (needMoveFiles != null && needMoveFiles.Count > 0)
            //{
            //    DebugBundleChangeAssetEntryLogFile(needMoveFiles, savePath);

            //    needRemovecount = needMoveFiles.Count + 1;//catalog.json
            //    Debug.Log("Aa 需要移动的热更新文件资源Count:" + needRemovecount);

            //    List<string> needmoveList = new List<string>(needMoveFiles.Count);
            //    string bundleName;
            //    int firstIndex;
            //    foreach (var item in needMoveFiles.Keys)
            //    {
            //        bundleName = item.Split('-')[1];
            //        firstIndex = bundleName.LastIndexOf("/");
            //        bundleName = bundleName.Remove(0, firstIndex + 1);
            //        bundleName = bundleName.Replace(".bundle", "");
            //        needmoveList.Add(bundleName);
            //        if (!bundleChangeList.Contains(bundleName))
            //        {
            //            Debug.Log("Addressable.bin 相比 HotFixAaList.txt 热更新增:" + item);
            //        }
            //    }

            //    List<string> difList = null;
            //    foreach (var item in bundleChangeList)
            //    {
            //        if (!needmoveList.Contains(item))
            //        {
            //            if (difList == null)
            //                difList = new List<string>();
            //            difList.Add(item);
            //            Debug.Log("HotFixAaList.txt 相比 Addressable.bin 新增:" + item);
            //        }
            //    }


            //    if (bundleChangeList.Count != needMoveFiles.Count)
            //    {
            //        //有可能某个资源在0保存有，1删除，2又加进来，那么在统计数量上面 就会不一样
            //        if (difList != null && difList.Count > 0)
            //        {
            //            Debug.Log(string.Format("热更配置计算热更, 删除又增加的资源:{0}", difList.Count));
            //        }
            //        else
            //        {
            //            Debug.LogError(string.Format("Addressable计算热更：{0},热更配置计算热更：{1}, 生成的热更新资源数量不一样，！！！", needMoveFiles.Count, bundleChangeList.Count));
            //        }
            //    }
            //    else
            //        Debug.Log(string.Format("0->{0}，需要更新Bundle资源 {1}", curVersion, bundleChangeList.Count + 1));
            //}
            #endregion


        }

        return rlt;
    }





    /// <summary>
    /// 得到一个对象的克隆(二进制的序列化和反序列化)--需要标记可序列化
    /// </summary>
    public static object Clone(object obj)
    {
        MemoryStream memoryStream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(memoryStream, obj);
        memoryStream.Position = 0;
        return formatter.Deserialize(memoryStream);
    }
    //private static int CopyHotFixResToStream(BuildTarget target, BuildSetting buildsettings)
    //{
    //    int rlt = 0;
    //    //需要把热更新的资源Copy到StreamingAsset(1.0.0首包资源，老的删除，新的加入，节省时间成本)
    //    string platformName = Enum.GetName(typeof(BuildTarget), target);

    //    if (buildsettings.bGenAssetBundle)
    //    {
    //        string newListStr = string.Format("{0}/{1}", HotFixBinaryFile, "NewBundleList.bytes");
    //        List<string> NewBundleList = ReadBinary(newListStr) as List<string>;
    //        if (NewBundleList != null && NewBundleList.Count > 0)
    //        {
    //            //过滤一下
    //            List<string> tempBundleFiles = new List<string>();
    //            foreach (var item in NewBundleList)
    //            {
    //                int index = item.LastIndexOf("/") + 1;
    //                string tempPath = item.Substring(index);
    //                tempBundleFiles.Add(tempPath);
    //            }

    //            //删除StreamingAsset/aa/android下的不使用的资源
    //            EditorUtility.DisplayCancelableProgressBar("拷贝热更新资源到StreamingAsset", "删除StreamingAsset下不使用的资源", 0.2f);
    //            string sBundlePath = string.Format("{0}/{1}", Addressables.PlayerBuildDataPath, platformName);
    //            DirectoryInfo dirInfo = new DirectoryInfo(sBundlePath);
    //            FileInfo[] fileInfoArr = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
    //            List<string> needDeleteBundleList = new List<string>();
    //            for (int i = 0; i < fileInfoArr.Length; ++i)
    //            {
    //                FileInfo fileInfo = fileInfoArr[i];
    //                if (fileInfo.Extension == ".meta")
    //                    continue;
    //                string fileName = fileInfo.Name;

    //                if (!tempBundleFiles.Contains(fileName))
    //                {
    //                    string fileFullPath = fileInfo.FullName;
    //                    needDeleteBundleList.Add(fileFullPath);
    //                    needDeleteBundleList.Add(string.Format("{0}.meta", fileFullPath));
    //                }
    //            }
    //            foreach (var item in needDeleteBundleList)
    //            {
    //                File.Delete(item);
    //            }

    //            EditorUtility.DisplayCancelableProgressBar("拷贝热更新资源到StreamingAsset", "添加新bundle", 0.8f);
    //            //添加新文件
    //            foreach (var file in tempBundleFiles)
    //            {
    //                string SrcPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", Application.dataPath, AddressableOriPath, AssetPath.sAddressableDir, PlatformMappingService.GetPlatformPathSubFolder(), platformName, file);
    //                string DesPath = string.Format("{0}/{1}/{2}/{3}", Application.streamingAssetsPath, AssetPath.sAddressableDir, platformName, file);

    //                if (!File.Exists(DesPath))
    //                {
    //                    File.Copy(SrcPath, DesPath, true);
    //                }
    //            }

    //            EditorUtility.ClearProgressBar();
    //            AssetDatabase.Refresh();

    //            //需要替换本地的Catalog.json里的远端路径为本地路径 --前提是Catalog.json 不能打成bundle
    //            string catalogPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, AssetPath.sAddressableDir, "catalog.json");
    //            if (File.Exists(catalogPath))
    //            {
    //                string catalogText = File.ReadAllText(catalogPath);
    //                string RemoteLoadPathStr = "{UnityEngine.AddressableAssets.PathRebuild.RemoteLoadPath}";
    //                string OriLocalLoadPath = "{UnityEngine.AddressableAssets.Addressables.RuntimePath}" + "/" + target;
    //                catalogText = catalogText.Replace(RemoteLoadPathStr, OriLocalLoadPath);
    //                File.WriteAllText(catalogPath, catalogText);
    //                AssetDatabase.Refresh();
    //            }
    //            else
    //            {
    //                Debug.LogError("不存在文件:" + catalogPath);
    //                rlt = -1;
    //                return rlt;
    //            }
    //        }
    //    }



    //    if (buildsettings.bGenCsv)
    //    {
    //        string assetCsvPublishDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAssetCsvDir);
    //        string streamingCsvDir = string.Format("{0}/{1}", Application.streamingAssetsPath, AssetPath.sAssetCsvDir);
    //        CopyAsset(assetCsvPublishDir, streamingCsvDir, "拷贝Config -> StreamingAssets\\config", true, new List<string> { ".manifest" });
    //    }

    //    if (buildsettings.bGenLogic)
    //    {
    //        string assetLogicPublishDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAssetLogicDir);
    //        string streamingLogicDir = string.Format("{0}/{1}", Application.streamingAssetsPath, AssetPath.sAssetLogicDir);
    //        CopyAsset(assetLogicPublishDir, streamingLogicDir, "拷贝Logic -> StreamingAssets\\logic", true, new List<string> { ".manifest", ".dll" });
    //    }

    //    return rlt;
    //}
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
    private static int CopyAssetTo0(string platformName, string version)
    {
        int rlt = 0;
        string maximalVersion = VersionHelper.GetMaximalVersion(version);
        string minimalVersion = "0";// VersionHelper.GetMinimalVersion(version);

        string bundleDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundleBuildPath, platformName);
        string publishDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion);
        FileTool.CreateFolder(publishDir);

        //拷贝配置config，dll文件 --> 热更资源版本0文件夹下
        rlt = CopyAsset(bundleDir, publishDir, "拷贝资源到初始资源目录 0 ", true, new List<string> { ".manifest", ".dll" });

        //拷贝Library下的Addressable bin文件 --> 资源版本0文件夹下
        //rlt = LibraryAssetCopyToAddressableAsset(platformName, minimalVersion);

        string sourceDirAddressable = string.Format("{0}/{1}", Application.dataPath, AddressableOriPath);
        if (!Directory.Exists(sourceDirAddressable))
        {
            Debug.LogErrorFormat("不存在源目录 {0}", sourceDirAddressable);
            rlt = -5;
            return rlt;
        }

        string DestionDirAddressable = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion, AddressableBin);
        FileTool.CreateFolder(DestionDirAddressable);

        string platMapPath = PlatformMappingService.GetPlatformPathSubFolder().ToString();
        string sourceDirNamebin = string.Format("{0}/{1}/addressables_content_state.bin", sourceDirAddressable, platMapPath);
        string destDirNamebin = string.Format("{0}/addressables_content_state.bin", DestionDirAddressable);

        //拷贝bin单个文件
        File.Copy(sourceDirNamebin, destDirNamebin, true);

        //拷贝Layout
        string layoutSrc = string.Format("{0}/{1}", sourceDirAddressable, "buildlayout.txt");
        string layoutDes = string.Format("{0}/{1}/{2}/{3}/buildlayout.txt", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion);
        File.Copy(layoutSrc, layoutDes);

        //copy0 需要删除之前打热更包生成的 NeedUpdateList.json 更新列表
        string needUpdateListOriPath = string.Format("{0}/{1}", HotFixBinaryFile, "NeedUpdateList.json");
        if (File.Exists(needUpdateListOriPath))
            File.Delete(needUpdateListOriPath);

        AssetDatabase.Refresh();

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
        string rootDir = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AddressableOriPath, AssetPath.sAddressableDir, PlatformMappingService.GetPlatformPathSubFolder(), platformName);
        Debug.LogFormat("Build -> Start GenAaAssetsList platformName:{0} rootDir:{1}", platformName, rootDir);

        //获取当前生成的bunde列表
        AssetList assetList = null;
        string fileOriPath = string.Format("{0}/{1}", HotFixBinaryFile, "NewBundleList.json");
        if (!File.Exists(fileOriPath))
        {
            Debug.LogError("不存在文件：" + fileOriPath);
            return assetList;
        }

        BundleListRes newAddBundleListRes = JsonUtility.FromJson<BundleListRes>(File.ReadAllText(fileOriPath));
        if (newAddBundleListRes == null || newAddBundleListRes.bundleListRes == null)
            return assetList;

        assetList = new AssetList();
        Dictionary<string, AssetsInfo> assetListMD5 = new Dictionary<string, AssetsInfo>(newAddBundleListRes.bundleListRes.Count);

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
    //private static AssetList GenAaUpdateAssetList(string platformName)
    //{
    //    Debug.LogFormat("Build -> Start GenAaAssetsList platformName:{0} rootDir:{1}", platformName);

    //    string rootDir = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AddressableOriPath, AssetPath.sAddressableDir, PlatformMappingService.GetPlatformPathSubFolder(), platformName);

    //    //获取热更文件
    //    AssetList assetList = null;
    //    string needUpdatefile = string.Format("{0}/{1}", HotFixBinaryFile, "NeedUpdateList.bytes");

    //    if (!File.Exists(needUpdatefile))
    //        return assetList;

    //    Dictionary<string, List<string>> needMoveFiles = ReadBinary(needUpdatefile) as Dictionary<string, List<string>>;

    //    assetList = new AssetList();
    //    Dictionary<string, AssetsInfo> assetListMD5 = new Dictionary<string, AssetsInfo>(needMoveFiles.Count);

    //    int i = 0;
    //    foreach (var item in needMoveFiles.Keys)
    //    {
    //        string[] tempStr = item.Split('-');
    //        string bundleName = tempStr[0];
    //        int startIndex = tempStr[1].LastIndexOf("/");
    //        int endIndex = tempStr[1].LastIndexOf(".");
    //        string hashName = tempStr[1].Substring(startIndex + 1, endIndex - startIndex - 1);

    //        string path = string.Format("{0}/{1}.bundle", rootDir, hashName);
    //        if (File.Exists(path))
    //        {
    //            FileInfo info = new FileInfo(path);
    //            AssetsInfo asset = new AssetsInfo();
    //            asset.AssetMD5 = FrameworkTool.GetFileMD5(path);
    //            asset.AssetName = string.Format("{0}/{1}", bundleName, hashName);
    //            asset.Size = (ulong)info.Length;
    //            asset.AssetType = 1;

    //            AssetsInfo tmp = null;
    //            if (assetListMD5.TryGetValue(asset.AssetMD5, out tmp))
    //            {
    //                Debug.LogErrorFormat("{0} 和 {1}拥有相同的MD5 ", tmp.AssetName, asset.AssetName);
    //            }
    //            else
    //            {
    //                assetListMD5.Add(asset.AssetMD5, asset);
    //            }

    //            assetList.Contents[asset.AssetName] = asset;
    //        }
    //        else
    //        {
    //            Debug.LogError("不存在bundle" + path);
    //        }

    //        ++i;

    //        if (EditorUtility.DisplayCancelableProgressBar("生成资源列表", string.Format("({0}/{1}){2}", i, needMoveFiles.Count, path), (float)i / (float)needMoveFiles.Count))
    //        {
    //            bStopBuild = true;
    //            break;
    //        }
    //    }

    //    EditorUtility.ClearProgressBar();
    //    return assetList;
    //}
    private static int GenHotFixAsset_Addressable(BuildSetting buildSetting, string platformName)
    {
        int rst = 0;

        //1.先回滚，重新生成 LocalGroup 
        if (buildSetting.bRebuildGroup)
        {
            rst = GenLocalAssetGroup_Addressable(buildSetting, true);
        }

        //2.每次跟第一次的进行比较资源,需要把热更0记录的 copy到Library下
        string cacheBinPath = String.Format("{0}/{1}/{2}/{3}/{4}/addressables_content_state.bin", Application.dataPath, AssetBundlePublishPath, platformName, 0, AddressableBin);
        if (!File.Exists(cacheBinPath))
        {
            Debug.LogError("不存在文件：" + cacheBinPath);
            return -5;
        }
        string desPath = ContentUpdateScript.GetContentStateDataPath(false);// 修改编辑器环境下 window（平台下）的bin文件
        File.Copy(cacheBinPath, desPath, true);
        AddressablesPlayerBuildResult result = ContentUpdateScript.BuildContentUpdate(AddressableAssetSettingsDefaultObject.Settings, desPath);
        //AddressableAssetSettings.BuildPlayerContent();

        //3.刷新一下
        AssetDatabase.Refresh();

        return rst;
    }

    public static int GenHotFixBundleRes(string platformName, string minimalVersion)
    {
        int rst = 0;

        //AssetList curAaUpdateList = new AssetList();

        //string remoteBuildCatalog = string.Format("{0}/{1}/{2}", Application.dataPath, AddressableServerDataPath, platformName);
        //string orignalBundlePath = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AddressableOriPath, AssetPath.sAddressableDir, PlatformMappingService.GetPlatformPathSubFolder(), platformName);

        //string DestionFile = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion, AssetPath.sAddressableDir);
        //if (Directory.Exists(DestionFile))
        //{
        //    Directory.Delete(DestionFile, true);
        //}
        //Directory.CreateDirectory(DestionFile);
        //AssetDatabase.Refresh();

        int needRemovecount = 0;

        string savePath = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion.ToString());

        //获取热更文件
        string needUpdatefile = string.Format("{0}/{1}", HotFixBinaryFile, "NeedUpdateList.bytes");

        Dictionary<string, List<string>> needMoveFiles = null;

        if (File.Exists(needUpdatefile))
            needMoveFiles = ReadBinary(needUpdatefile) as Dictionary<string, List<string>>;
        if (needMoveFiles != null && needMoveFiles.Count > 0)
        {
            needRemovecount = needMoveFiles.Count + 1;//catalog.json
            Debug.Log("Aa 需要移动的热更新文件资源Count:" + needRemovecount);
            //打印日志
            //DebugBundleChangeAssetEntryLogFile(needMoveFiles, savePath);

            ////copy catalog 和 hash
            //var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
            //string catalogName = "catalog_" + m_Settings.PlayerBuildVersion;

            //string OriCatalogFile = string.Format("{0}/{1}{2}", remoteBuildCatalog, catalogName, ".json");
            //string DesCatalogFile = string.Format("{0}/{1}{2}", DestionFile, catalogName, ".json");
            //File.Copy(OriCatalogFile, DesCatalogFile);

            //string OriCatalogHashFile = string.Format("{0}/{1}{2}", remoteBuildCatalog, catalogName, ".hash");
            //string DesCatalogHashFile = string.Format("{0}/{1}{2}", DestionFile, catalogName, ".hash");
            //File.Copy(OriCatalogHashFile, DesCatalogHashFile);

            ////只修改热更资源里的Catalog.json里的本地为远端路径 --前提是Catalog.json 不能打成bundle
            //string OriLocalLoadPath;
            //string RemoteLoadPathStr;

            //ContentCatalogData contentCatalogData = JsonUtility.FromJson<ContentCatalogData>(File.ReadAllText(DesCatalogFile));
            //if (contentCatalogData != null && contentCatalogData.InternalIds != null && contentCatalogData.InternalIds.Length > 0)
            //{
            //    foreach (var item in needMoveFiles.Keys)
            //    {
            //        string key = item;
            //        if (item.Contains("-"))
            //            key = item.Split('-')[1];

            //        OriLocalLoadPath = key;
            //        for (int j = 0; j < contentCatalogData.InternalIds.Length; j++)
            //        {
            //            if (contentCatalogData.InternalIds[j] == OriLocalLoadPath)
            //            {
            //                int firstIndex = key.IndexOf("/");
            //                string backPath = OriLocalLoadPath.Substring(firstIndex + 1);
            //                RemoteLoadPathStr = string.Format("{0}/{1}", "{UnityEngine.AddressableAssets.PathRebuild.RemoteLoadPath}", backPath);
            //                contentCatalogData.InternalIds[j] = RemoteLoadPathStr;
            //                break;
            //            }
            //        }
            //    }
            //}
            //File.WriteAllText(DesCatalogFile, JsonUtility.ToJson(contentCatalogData));
            //AssetDatabase.Refresh();

            ////copy 热更资源bundle
            //string DestBundleFile = string.Format("{0}/{1}", DestionFile, platformName);
            //if (Directory.Exists(DestBundleFile))
            //{
            //    Directory.Delete(DestBundleFile, true);
            //}
            //Directory.CreateDirectory(DestBundleFile);
            //AssetDatabase.Refresh();

            //int i = 0;
            //int count = needMoveFiles.Count;
            //foreach (var item in needMoveFiles)
            //{
            //    ++i;

            //    string item_key = item.Key.Replace("\\", "/");
            //    string[] item_keyArr = item_key.Split('-');
            //    string bundleName = item_keyArr[0];
            //    item_key = item_keyArr[1];

            //    int index = item_key.LastIndexOf("/");
            //    string hashName = item_key.Substring(index + 1);
            //    int dotIndex = hashName.IndexOf(".");
            //    string SrcPath = string.Format("{0}/{1}", orignalBundlePath, hashName);
            //    string DesPath = string.Format("{0}/{1}/{2}", DestionFile, platformName, hashName);
            //    File.Copy(SrcPath, DesPath);

            //    FileInfo info = new FileInfo(DesPath);
            //    AssetsInfo asset = new AssetsInfo();
            //    asset.AssetMD5 = FrameworkTool.GetFileMD5(DesPath);
            //    asset.Size = (ulong)info.Length;
            //    asset.AssetType = 1;
            //    asset.AssetName = string.Format("{0}/{1}", bundleName, hashName.Substring(0, dotIndex));
            //    asset.Version = minimalVersion.ToString();

            //    //特殊处理---by yd
            //    //if (minimalVersion == "1")
            //    //{
            //    //    asset.Version = 1;
            //    //}

            //    curAaUpdateList.Contents.Add(asset.AssetName, asset);


            //    if (EditorUtility.DisplayCancelableProgressBar("拷贝热更新资源（Bundle）", string.Format("({0}/{1})\n{2}\n{3}", i.ToString(), count.ToString(), SrcPath, DesPath), (float)i / (float)count))
            //    {
            //        rst = 1;
            //        break;
            //    }
            //}
            //EditorUtility.ClearProgressBar();
        }

        //copy 热更资源列表
        //System.IO.File.WriteAllText(string.Format("{0}/{1}", DestionFile, AssetPath.sAaHotFixBundleList), stringBuilder.ToString());


        //return curAaUpdateList;

        return needRemovecount;
    }


    /// <summary>
    /// 本地下载校验MD5 -- 由于pc下载的不适用Android平台
    /// </summary>
    /// <param name="AaUpdateContent"></param>
    /// <param name="platformName"></param>
    /// <param name="minimalVersion"></param>
    /// <returns></returns>
    public static Dictionary<string, AssetsInfo> GetCacheMD5DownloadHandlerAssetBundle(Dictionary<string, AssetsInfo> AaUpdateContent, string platformName, string minimalVersion)
    {
        if (Directory.Exists(Caching.defaultCache.path))
            FileOperationHelpter.DeleteDirctory(Caching.defaultCache.path);

        int i = 0;
        int count = AaUpdateContent.Count;
        var remoteItor = AaUpdateContent.GetEnumerator();

        string hashName;
        string DesPath;
        AssetsInfo newInfo;
        string DestionFile = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundlePublishPath, platformName, minimalVersion, AssetPath.sAddressableDir);
        Dictionary<string, AssetsInfo> MD52Dict = new Dictionary<string, AssetsInfo>();

        while (remoteItor.MoveNext())
        {
            newInfo = remoteItor.Current.Value;
            hashName = newInfo.AssetName.Split('/')[1];
            DesPath = string.Format("{0}/{1}/{2}.bundle", DestionFile, platformName, hashName);

            Hash128 hash128 = Hash128.Parse(hashName);
            string filepath = "file:///" + Path.GetFullPath(DesPath);
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(filepath, hash128))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {

                }

                if (request.isDone)
                {
                    string cachePath = string.Format("{0}/{1}/{2}/__data", Caching.defaultCache.path, hash128, hash128);
                    if (File.Exists(cachePath))
                    {
                        string cacheMd5 = FrameworkTool.GetFileMD5(cachePath);
                        if (!newInfo.AssetMD5.Equals(cacheMd5))
                        {
                            AssetsInfo tempInfo = new AssetsInfo();
                            tempInfo.AssetName = newInfo.AssetName;
                            /*** 原本是生成的bundle md5和下载下来的bundle md5不一致，所以先尝试下载一遍，做为第二个md5验证
                                 但是由于下载是从pc上模拟下载，并不能代表实际的平台android，所以第2个md5并不准确.
                                 Addressable能做校验的是:crc将与下载后的资产包数据的校验和进行比较,如果crc不匹配，则会记录一个错误，并且不会加载资产包。
                                 由于记录的错误在系统内部无法捕捉，同时快手方需要在md5不一致时进行打点，所以热更下载不在使用Addressable  **/

                            //tempInfo.AssetMD52 = cacheMd5;
                            MD52Dict.Add(newInfo.AssetName, tempInfo);

                            Debug.LogError(string.Format("md5 不一致,bundle md5:{0}  cache md5:{1}", newInfo.AssetMD5, cacheMd5));
                        }
                    }
                    else
                    {
                        Debug.LogError("Caching 里不存在文件：" + cachePath);
                    }
                }
            }

            ++i;

            if (EditorUtility.DisplayCancelableProgressBar("本地下载校验热更新资源", string.Format("({0}/{1})\n{2}", i.ToString(), count.ToString(), DesPath), (float)i / (float)count))
            {
                //rst = 1;
                break;
            }
        }

        EditorUtility.ClearProgressBar();

        return MD52Dict;
    }
    public static void CreateContentUpdateGroupByBundleMode(AddressableAssetSettings settings, List<AddressableAssetEntry> items, string groupName, BundledAssetGroupSchema.BundlePackingMode bundleMode)
    {
        var contentGroup = settings.CreateGroup(groupName, false, false, true, null);
        var schema = contentGroup.AddSchema<BundledAssetGroupSchema>();
        schema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteBuildPath);
        schema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteLoadPath);
        schema.BundleMode = bundleMode;
        contentGroup.AddSchema<ContentUpdateGroupSchema>().StaticContent = false;
        settings.MoveEntries(items, contentGroup);
    }
    public static object ReadBinary(string path)
    {
        Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryFormatter binFormat = new BinaryFormatter();
        return binFormat.Deserialize(fStream);
    }
    public static void WriteBinary(string path, object graph)
    {
        Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binFormat = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        binFormat.Serialize(fStream, graph);
        fStream.Close();
        AssetDatabase.Refresh();
    }
    public static int ReadLolcalHash(VersionSetting versionSetting, BuildTarget target)
    {
        string platformName = Enum.GetName(typeof(BuildTarget), target);
        //保存本地的hash，设置同一版本的热更系列 (通过第一次构建远端目录拿hash)
        int result = 0;
        string localHashPath = HotFixBinaryFile + "/LolcalHash.txt";
        string lolcalHash;
        if (File.Exists(localHashPath))
        {
            lolcalHash = File.ReadAllText(localHashPath);
            if (string.IsNullOrEmpty(lolcalHash))
            {
                Debug.LogError("记录首包hash为空，因为bundle未正常生成");
                return -5;
            }
            else
            {
                //设置包的唯一热更标识符
                string HotFixUniqueIdentifierPath = string.Format("{0}/{1}", HotFixBinaryFile, "HotFixUniqueIdentifier.txt");
                File.WriteAllText(HotFixUniqueIdentifierPath, lolcalHash);

                versionSetting.HotFixUniqueIdentifier = lolcalHash;
                EditorUtility.SetDirty(versionSetting);
            }
        }
        else
        {
            Debug.LogError("生成文件LolcalHash.txt失败，因为bundle未正常生成");
            return -5;
        }
        return result;
    }
    public static string GetMinimalVersionByVersion(string sVersion, BuildTarget mBuildTarget, VersionSetting mVersionSetting, BuildSetting mBuildSetting)
    {
        //1.0.0
        string maximalVersion = VersionHelper.GetMaximalVersion(sVersion);
        string minimalVersion = VersionHelper.GetMinimalVersion(sVersion);

        //自动设置热更的版本号 +1
        string fileDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundlePublishPath, mBuildTarget);
        if (!Directory.Exists(fileDir))
        {
            return sVersion;
        }
        DirectoryInfo rootInfo = new DirectoryInfo(fileDir);
        DirectoryInfo[] dicts = rootInfo.GetDirectories();
        int lastSmallVersion = 0;
        foreach (var item in dicts)
        {
            int smallVersion = 0;
            if (int.TryParse(item.Name, out smallVersion))
            {
                if (smallVersion > lastSmallVersion)
                    lastSmallVersion = smallVersion;
            }
        }

        string version = string.Format("{0}.{1}", maximalVersion, lastSmallVersion + 1);
        mVersionSetting.AssetVersion = mBuildSetting.sVersion = version;

        return version;
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
            versionSetting.AssetVersion = buildSetting.sVersion;
            versionSetting.VersionUrl = buildSetting.sVersionUrl;
            versionSetting.AppVersion = buildSetting.sAppVersion;
            AssetDatabase.CreateAsset(versionSetting, "Assets/Resources/VersionSetting.asset");
        }

        AssetDatabase.SaveAssets();

        return versionSetting;
    }
    public static void StartBuild(BuildTarget target, bool genAssetBundle, bool AtlasInclude)
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
            if (Directory.Exists(cacheDir))
                Directory.Delete(cacheDir, true);

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

        string atlasDir = "ResourcesAB/Atlas";

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
    public static void EndBuild(BuildTarget target,bool genAssetBundle, bool AtlasInclude)
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
    public static int CopyAssetBundleToStream(BuildTarget target, ECopyAssetMode eCopyAssetMode)
    {
        int rlt = 0;
        string platformName = Enum.GetName(typeof(BuildTarget), target);

        switch (eCopyAssetMode)
        {
            case ECopyAssetMode.eCopyAB:
                rlt = CopyAsset(Addressables.BuildPath, Addressables.PlayerBuildDataPath, "拷贝Library aa  -> StreamingAssets/aa", true, null);
                break;
            case ECopyAssetMode.eCopyConfig:
                string assetCsvPublishDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAssetCsvDir);
                string streamingCsvDir = string.Format("{0}/{1}", Application.streamingAssetsPath, AssetPath.sAssetCsvDir);
                rlt = CopyAsset(assetCsvPublishDir, streamingCsvDir, "拷贝Config -> StreamingAssets/config", true, new List<string> { ".manifest" });
                break;
            case ECopyAssetMode.eCopyDll:
                string assetLogicPublishDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAssetLogicDir);
                string streamingLogicDir = string.Format("{0}/{1}", Application.streamingAssetsPath, AssetPath.sAssetLogicDir);
                rlt = CopyAsset(assetLogicPublishDir, streamingLogicDir, "拷贝Logic -> StreamingAssets/logic", true, new List<string> { ".manifest", ".dll" });
                break;
            case ECopyAssetMode.eCopyABToAssetBundleBuildPath:
                string tempDesABDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAddressableDir);
                rlt = CopyAsset(Addressables.BuildPath, tempDesABDir, "拷贝Library aa  -> AssetBundle/aa", true, null);
                break;
            case ECopyAssetMode.eCopyCatalog:
                rlt = CopyAsset(Addressables.BuildPath + "/AddressablesLink", Addressables.PlayerBuildDataPath + "/AddressablesLink", "拷贝Library aa  -> StreamingAsset/aa", true, null);
                File.Copy(Addressables.BuildPath + "/catalog.json", Addressables.PlayerBuildDataPath + "/catalog.json", true);
                File.Copy(Addressables.BuildPath + "/settings.json", Addressables.PlayerBuildDataPath + "/settings.json", true);
                break;
            case ECopyAssetMode.eCopyBundle:
                string assetBundlePublishDir = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAddressableDir, platformName);
                string streamingBundleDir = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, AssetPath.sAddressableDir, platformName);
                rlt = CopyAsset(assetBundlePublishDir, streamingBundleDir, "拷贝Bundle -> StreamingAssets/aa/Android", true, new List<string> { ".manifest" });
                break;
        }

        AssetDatabase.Refresh();
        return rlt;
    }

    private static void WriteFile(string path, string content)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(path, content);
    }
    //     private static void WriteVersionInfo(string path, VersionSetting versionInfo)
    //     {
    //         string versionPath = string.Format("{0}/Version.txt", path);
    //         string json = string.Format("{0}\n{1}\n{2}", versionInfo.eChannelType.ToString(), versionInfo.Version, versionInfo.VersionUrl);
    //         File.WriteAllText(versionPath, json);
    //     }

    private static int CheckVersionFirstPackage(string version)
    {
        string maximalVersion = VersionHelper.GetMaximalVersion(version);
        string minimalVersion = VersionHelper.GetMinimalVersion(version);

        int result = 0;
        int curVersion = 0;
        if (!int.TryParse(minimalVersion, out curVersion))
        {
            Debug.LogErrorFormat("版本格式错误 version = {0}", version);
            result = 5;
        }

        if (curVersion > 0)
        {
            string ContentStr = string.Format("首包资源版本号设置有误,请检查版本号{0}!", version);
            EditorUtility.DisplayDialog("提示", ContentStr, "确定");
            Debug.LogErrorFormat("首包资源版本号设置错误 version = {0}", version);
            result = 5;
        }
        return result;
    }
    private static string FixVersionAndBackup(VersionSetting versionSetting, BuildSetting buildSetting)
    {
        //管总要求，直接打热更新资源  根据文件夹来确定版本号，即使打包工具填的不对，最终到这里会更正的 (根据大版本生成热更的)

        string version = buildSetting.sVersion;
        string maximalVersion = VersionHelper.GetMaximalVersion(version);
        string minimalVersion = VersionHelper.GetMinimalVersion(version);

        int curVersion = 0;
        if (!int.TryParse(minimalVersion, out curVersion))
        {
            Debug.LogErrorFormat("版本格式错误 version = {0}", version);
        }

        string fileDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundlePublishPath, buildSetting.mBuildTarget);
        if (!Directory.Exists(fileDir))
        {
            Directory.CreateDirectory(fileDir);
            AssetDatabase.Refresh();
        }

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

        if (lastSmallVersion <= -1)
        {
            //打首包资源
        }
        else
        {
            //打热更新资源
        }

        version = string.Format("{0}.{1}", maximalVersion, lastSmallVersion + 1);
        versionSetting.AssetVersion = buildSetting.sVersion = version;
        EditorUtility.SetDirty(versionSetting);
        EditorUtility.SetDirty(buildSetting);
        AssetDatabase.SaveAssets();
        //if (curVersion <= 0)
        //{
        ////只有重新生成1.0.0的首包 才需要备份大版本的一系列热更文件
        //string platformName = Enum.GetName(typeof(BuildTarget), buildSetting.mBuildTarget);
        //string curDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundlePublishPath, platformName);
        //if (buildSetting.bBackUpdate && Directory.Exists(curDir))
        //{
        //    DirectoryInfo dirInfo = new DirectoryInfo(curDir);
        //    string crateInfoTime = string.Format("{0:yyyy.MM.dd.HH.mm}", dirInfo.CreationTime);
        //    string saveOldAssetDirName = string.Format("{0}_bankup_{1}", platformName, crateInfoTime);
        //    string saveOldAssetDirPath = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundlePublishPath, saveOldAssetDirName);
        //    Directory.Move(curDir, saveOldAssetDirPath);
        //    AssetDatabase.Refresh();
        //}

        //if (Directory.Exists(curDir))
        //    Directory.Delete(curDir, true);
        //}


        return version;
    }
    public static void SetScriptingDefine(BuildSetting buildSetting)
    {
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildSetting.mBuildTarget);

        List<string> adds = new List<string>();
        List<string> removes = new List<string>();
        CheckScriptingDefine(adds, removes, "DEBUG_MODE", buildSetting.bUseDebugMode);
        CheckScriptingDefine(adds, removes, "ILRUNTIME_MODE", buildSetting.eRuntimeMode == ERuntimeMode.ILRuntime);
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
        //CheckScriptingDefine(adds, removes, "USE_MTP", buildSetting.eApkMode == ApkMode.UseSDK);
#elif UNITY_STANDALONE_WIN
        CheckScriptingDefine(adds, removes, "USE_PCSDK", buildSetting.eApkMode == ApkMode.UseSDK);
        CheckScriptingDefine(adds, removes, "GM_PROPAGATE_VERSION", buildSetting.bGmPropagateVersion);      
#endif

        ApplyScriptingDefine(adds, removes, buildTargetGroup);
        AssetDatabase.Refresh();
    }
    private static int CompilePlayerScripts(BuildSetting buildSetting)
    {
        //删除上次生成的绑定代码
        BuildTarget target = buildSetting.mBuildTarget;
        string CLRBindingCodePath = "Assets/Scripts/Framework/ILRuntime/Generated/CLRBindings";
        if (Directory.Exists(CLRBindingCodePath))
        {
            Directory.Delete(CLRBindingCodePath, true);
        }
        Directory.CreateDirectory(CLRBindingCodePath);
        AssetDatabase.Refresh();

        //设置脚本用到的宏
        //SetScriptingDefine(buildSetting);

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
        if (Directory.Exists(TempOutputFolder))
        {
            Directory.Delete(TempOutputFolder, true);
        }
        Directory.CreateDirectory(TempOutputFolder);
        AssetDatabase.Refresh();

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

        //需要copy 一份放到Assets/LogicDll 文件下供生成桥接代码用。
        Startup.CopyDllFromAddressableBuild(TempOutputFolder, platformName);
        AssetDatabase.Refresh();

        //删除零时文件
        if (Directory.Exists(TempOutputFolder))
        {
            Directory.Delete(TempOutputFolder, true);
        }
        return 0;
    }
    private static int GenerateCLRBindingByAnalysis(BuildSetting buildSetting)
    {
        int rlt = 0;
        //运行时选择ILRuntime 需要生成桥接代码
        if (buildSetting.eRuntimeMode == ERuntimeMode.ILRuntime)
        {
            try
            {
                ILRuntimeCLRBinding.GenerateCLRBindingByAnalysis();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                rlt = -5;
            }
        }

        return rlt;
    }
    private static int SetVersionAppFilePathByGenHotApk(BuildSetting buildSetting, BuildTarget target, VersionSetting versionSetting, ref string appFilePath)
    {
        int rlt = 0;

        String sVersion = buildSetting.sVersion;
        //1.0.0
        string maximalVersion = VersionHelper.GetMaximalVersion(sVersion);
        string minimalVersion = VersionHelper.GetMinimalVersion(sVersion);

        //查找打出热更资源的最大的版本号
        string platformName = Enum.GetName(typeof(BuildTarget), target);
        string fileDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, BuildTool.AssetBundlePublishPath, platformName, maximalVersion);
        DirectoryInfo rootInfo = new DirectoryInfo(fileDir);
        DirectoryInfo[] dicts = rootInfo.GetDirectories();
        int lastSmallVersion = 0;
        foreach (var item in dicts)
        {
            int smallVersion = 0;
            if (int.TryParse(item.Name, out smallVersion))
            {
                if (smallVersion > lastSmallVersion)
                    lastSmallVersion = smallVersion;
            }
        }

        sVersion = string.Format("{0}.{1}", maximalVersion, lastSmallVersion);


        string[] appPath = appFilePath.Split('_');
        appFilePath = string.Empty;
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < appPath.Length; i++)
        {
            if (i == appPath.Length - 1)
            {
                string appExpend = appPath[i];
                int index = appExpend.LastIndexOf(".");
                appExpend = appExpend.Substring(index);
                stringBuilder.Append(sVersion).Append(appExpend);
            }
            else
                stringBuilder.Append(appPath[i]).Append("_");
        }

        appFilePath = stringBuilder.ToString();
        //PlayerSettings.bundleVersion = sVersion;

        return rlt;

    }
    private static void SetAppFilePathByExportAS(ref string appFilePath)
    {
        //给导出的android项目进行设置包名
        appFilePath = appFilePath.Replace("魔力宝贝", "CrossGate");
        int lastIndex = appFilePath.LastIndexOf(".");
        appFilePath = appFilePath.Substring(0, lastIndex);
    }
    private static int SetPackageIdentifier(VersionSetting versionSetting, BuildTarget buildTarget)
    {
        int rlt = 0;

        //设置apk的唯一性，避免1.0.0热更关闭apk 和1.0.3热更apk 在【覆盖安装时】需要清除手机本地缓存


        //获取需要的 LocalHash 
        string platformName = Enum.GetName(typeof(BuildTarget), buildTarget);
        string localHashPath = string.Format("{0}/{1}", HotFixBinaryFile, "LolcalHash.txt");
        string lolcalAaHash = File.ReadAllText(localHashPath);
        if (!File.Exists(localHashPath) || string.IsNullOrEmpty(lolcalAaHash))
        {
            Debug.LogError(string.Format("不存在文件:{0},或文件内容为空：{1}", localHashPath, lolcalAaHash));
            return 5;
        }

        //需要获取 HotFixUniqueIdentifier
        string HotFixUniqueIdentifierPath = string.Format("{0}/{1}", HotFixBinaryFile, "HotFixUniqueIdentifier.txt");
        string hotFixUniqueIdentifierStr =  File.ReadAllText(HotFixUniqueIdentifierPath);
        if (!File.Exists(HotFixUniqueIdentifierPath) || string.IsNullOrEmpty(hotFixUniqueIdentifierStr))
        {
            Debug.LogError(string.Format("不存在文件:{0},或文件内容为空：{1}", HotFixUniqueIdentifierPath, hotFixUniqueIdentifierStr));
            return 5;
        }

        //根据生成的AssetList内容 生成hash值
        string bundleDir = string.Format("{0}/{1}/{2}", Application.dataPath, AssetBundleBuildPath, platformName);
        AssetList curAssetList = GenAssetsList(buildTarget, bundleDir);
        string assetListContent = AssetList.Serialize(curAssetList);
        string localConfigHash = HashingMethods.Calculate(assetListContent).ToString();

        string combineHash = string.Format("{0}|{1}", lolcalAaHash, localConfigHash);
        versionSetting.PackageIdentifier = combineHash;
        versionSetting.HotFixUniqueIdentifier = hotFixUniqueIdentifierStr;
        EditorUtility.SetDirty(versionSetting);

        return rlt;
    }

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
        //PrintGroupAssetEntryLogFile(buildSetting.sVersion);

        BuildTarget buildTarget = buildSetting.mBuildTarget;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildSetting.mBuildTarget);

        string productName = "CrossGate";//PlayerSettings.productName;
        string currentTime = string.Format("{0:yyyy.MMdd.HHmm}", System.DateTime.Now);


        string version = buildSetting.sVersion;
        //bool bIncreasePacking = buildSetting.bIncreasePacking;
        bool genAssetBundle = buildSetting.bGenAssetBundle;
        bool genCsv = buildSetting.bGenCsv;
        bool genLogic = buildSetting.bGenLogic;
        bool genHotFix = buildSetting.bGenHotFix;
        bool genApp = buildSetting.bBuildApp;


        //bool copyABToStreamPath = buildSetting.bCopyABToStreamPath && genApp;
        //bool copyConfigToStreamPath = buildSetting.bCopyConfigToStreamPath && genApp;
        //bool copyLogicToStreamPath = buildSetting.bCopyLogicToStreamPath && genApp;

        bool useBuildOption = buildSetting.bBuildOptions;
        bool useDebugMode = buildSetting.bUseDebugMode;
        bool useProfileEvent = buildSetting.bUseProfileEvent;
        ERuntimeMode eRuntimeMode = buildSetting.eRuntimeMode;

        string AppOutPath = buildSetting.sAppPublishPath;

        BuildOptions buildOptions = buildSetting.mBuildOptions;


        bool clearAssetStreamPath = genCsv && genLogic && genAssetBundle && genApp;
        //bool analysisMaterial = genAssetBundle;//|| genHotFix;

        string maximalVersion = VersionHelper.GetMaximalVersion(version);
        string minimalVersion = VersionHelper.GetMinimalVersion(version);


#if UNITY_ANDROID
        PlayerSettings.bundleVersion = buildSetting.sAppVersion;
        int.TryParse(buildSetting.sVersionCode, out int versioncode);
        PlayerSettings.Android.bundleVersionCode = versioncode;
#elif UNITY_IOS
        PlayerSettings.bundleVersion = buildSetting.sAppVersion;
        PlayerSettings.iOS.buildNumber = buildSetting.sVersionCode;
#endif

        if (string.IsNullOrWhiteSpace(maximalVersion) || string.IsNullOrWhiteSpace(minimalVersion))
        {
            Debug.LogErrorFormat("版本格式错误 version = {0}", version);
            return 5;
        }

        if (genHotFix)
        {
            version = FixVersionAndBackup(versionSetting, buildSetting);
        }

        string appFilePath = null;
        if (genApp)
        {
            if (string.IsNullOrWhiteSpace(AppOutPath))
            {
                Debug.LogError("未选择输出目录");
                return 1;
            }
            else
            {
                switch (buildTarget)
                {
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                        appFilePath = string.Format("{0}/{1}_{2}_{3}_{4}_{5}_{6}.exe", AppOutPath, productName, currentTime, buildSetting.sSvnRevision, buildSetting.eScriptingImplementation.ToString(), eRuntimeMode.ToString(), version);
                        break;
                    case BuildTarget.iOS:
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.crossgate.kuaishou.ios");
                        appFilePath = string.Format("{0}/{1}_{2}_{3}_{4}_{5}_{6}.ipa", AppOutPath, productName, currentTime, buildSetting.sSvnRevision, buildSetting.eScriptingImplementation.ToString(), eRuntimeMode.ToString(), version);
                        break;
                    case BuildTarget.Android:
                        appFilePath = string.Format("{0}/{1}_{2}_{3}_{4}_{5}_{6}.apk", AppOutPath, productName, currentTime, buildSetting.sSvnRevision, buildSetting.eScriptingImplementation.ToString(), eRuntimeMode.ToString(), version);
                        break;
                    default:
                        {
                            Debug.LogErrorFormat("未支持该平台打包 {0}", buildTarget.ToString());
                            break;
                        }
                }

                if (string.IsNullOrWhiteSpace(appFilePath))
                {
                    return 1;
                }
            }
        }


        int rlt = 0;

        //设置宏和编译脚本
        string defineStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        rlt = CompilePlayerScripts(buildSetting);
        if (rlt != 0)
        {
            Debug.LogError("打包失败：脚本编译不成功，请检查代码！");
            return rlt;
        }


        //Addressable 相关设置
        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        //m_Settings.EnableSendProfilerEvents = useProfileEvent;
        string LocalBuildPathStr = "[UnityEngine.AddressableAssets.Addressables.BuildPath]/[BuildTarget]";
        m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kLocalBuildPath, LocalBuildPathStr);
        string LocalLoadPathStr = "{UnityEngine.AddressableAssets.Addressables.RuntimePath}/[BuildTarget]";
        m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kLocalLoadPath, LocalLoadPathStr);

        BuildTool.StartBuild(buildTarget, genAssetBundle, true);

        if (clearAssetStreamPath && rlt == 0)
        {
            Debug.LogFormat("Build -> Start ClearAssetSteamPath()");
            rlt = ClearAssetSteamPath();
            Debug.LogFormat("Build -> End ClearAssetSteamPath()");
        }

        if (genCsv && rlt == 0)
        {
            Debug.LogFormat("Build -> Start GenConfig({0})", buildTarget.ToString());
            rlt = GenConfig(buildTarget);
            Debug.LogFormat("Build -> End GenConfig({0})", buildTarget.ToString());
            //Debug.LogFormat("Build -> Start CopyConfigToStream({0},CopyConfig)", buildTarget.ToString());
            //rlt = CopyAssetBundleToStream(buildTarget, ECopyAssetMode.eCopyConfig);
            //Debug.LogFormat("Build -> End CopyConfigToStream({0},CopyConfig)", buildTarget.ToString());
        }

        if (genLogic && rlt == 0)
        {
            Debug.LogFormat("Build -> Start GenLogic({0}, {1})", buildTarget.ToString(), buildOptions.ToString());
            rlt = GenLogic2(buildTarget, buildOptions, useDebugMode);
            Debug.LogFormat("Build -> End GenLogic({0}, {1})", buildTarget.ToString(), buildOptions.ToString());

            //Debug.LogFormat("Build -> Start CopyLogicToStream({0},CopyLogic)", buildTarget.ToString());
            //rlt = CopyAssetBundleToStream(buildTarget, ECopyAssetMode.eCopyDll);
            //Debug.LogFormat("Build -> End CopyLogicToStream({0},CopyLogic)", buildTarget.ToString());
        }

        if (genAssetBundle && rlt == 0)
        {
            Debug.LogFormat("Build -> Start AssetBundle({0})", buildTarget.ToString());
            rlt = GenAssetBundle(buildSetting, buildTarget, version, buildSetting.eHotFixMode, versionSetting);
            Debug.LogFormat("Build -> End AssetBundle({0})", buildTarget.ToString());

            //if (buildSetting.eApkMode == ApkMode.UseSDK && buildTarget == BuildTarget.Android)
            //{
            //    //android平台使用sdk不需要copy到Stream 减少资源导入时间
            //    Debug.LogFormat("Build -> Start CopyAssetBundleToStream({0},CopyAssetBundle)", buildTarget.ToString());
            //    rlt = CopyAssetBundleToStream(buildTarget, ECopyAssetMode.eCopyCatalog);
            //    Debug.LogFormat("Build -> End CopyAssetBundleToStream({0},CopyAssetBundle)", buildTarget.ToString());
            //}
            //else
            //{
            //    Debug.LogFormat("Build -> Start CopyAssetBundleToStream({0},CopyAssetBundle)", buildTarget.ToString());
            //    rlt = CopyAssetBundleToStream(buildTarget, ECopyAssetMode.eCopyAB);
            //    Debug.LogFormat("Build -> End CopyAssetBundleToStream({0},CopyAssetBundle)", buildTarget.ToString());
            //}
        }

        if (genApp && rlt == 0)
        {
            Debug.LogFormat("Build -> Start BuildPlayer({0}, {1}, {2})", appFilePath, buildTarget.ToString(), buildOptions.ToString());
#if UNITY_ANDROID
            //                 PlayerSettings.Android.keystoreName = Application.dataPath + "/../SlugPrj.keystore";
            //                 PlayerSettings.Android.keystorePass = "xxxx";
            //                 PlayerSettings.Android.keyaliasName = "蓝星使命";
            //                 PlayerSettings.Android.keyaliasPass = "xxxx";
#endif

            //对于打不带sdk的包，需要copy资源到StreamingAsset文件夹
            //if (buildSetting.eApkMode == ApkMode.NoSDK && buildTarget == BuildTarget.Android)
            //{
            //    string bundleDirName = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath,AssetPath.sAddressableDir, Enum.GetName(typeof(BuildTarget), buildTarget));
            //    if (!Directory.Exists(bundleDirName))
            //    {
            //        rlt = CopyAssetBundleToStream(buildTarget, ECopyAssetMode.eCopyBundle);
            //        AssetDatabase.Refresh();
            //    }
            //}
           

            rlt = SetPackageIdentifier(versionSetting, buildTarget);
            rlt = GenerateCLRBindingByAnalysis(buildSetting);
            if (rlt != 0)
            {
                Debug.LogError("打包失败：生成绑定代码失败，请考虑debug和release模式!");
                return rlt;
            }

            PlayerSettings.SetScriptingBackend(BuildPipeline.GetBuildTargetGroup(buildTarget), buildSetting.eScriptingImplementation);

            if (!buildSetting.bUseDebugMode)
            {
                PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
                PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
                PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
                PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
            }

            if (buildSetting.eApkMode == ApkMode.NoSDK)
            {
#if UNITY_ANDROID

                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
#endif
                BuildPipeline.BuildPlayer(new string[] { "Assets/ResourcesAB/GameStart.unity" }, appFilePath, buildTarget, buildOptions);
            }
            else if (buildSetting.eApkMode == ApkMode.UseSDK)
            {
#if UNITY_ANDROID
                EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
                SetAppFilePathByExportAS(ref appFilePath);
#endif

                BuildPipeline.BuildPlayer(new string[] { "Assets/ResourcesAB/GameStart.unity" }, appFilePath, buildTarget, buildOptions);
            }
            Debug.LogFormat("Build -> End BuildPlayer({0}, {1}, {2})", appFilePath, buildTarget.ToString(), buildOptions.ToString());
        }

        if (genHotFix && rlt == 0)
        {
            Debug.LogFormat("Build -> Start GenHotFixAsset({0}, {1})", buildTarget.ToString(), version);
            rlt = GenHotFixAsset(buildTarget, version, versionSetting, buildSetting);
            Debug.LogFormat("Build -> End GenHotFixAsset({0}, {1})", buildTarget.ToString(), version);
        }

        BuildTool.EndBuild(EditorUserBuildSettings.activeBuildTarget, genAssetBundle, false);

        //PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defineStr);
        AssetDatabase.Refresh();

        ILRuntimeMenu.ClearGenerateCLRBinding();
        PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
        PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
        PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
        PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
        PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);

        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();



        swP.Stop();
        TimeSpan ts = swP.Elapsed;
        Debug.LogError(string.Format("打包一共消耗时间:{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds));

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
                buildSetting.sVersion = buildArgs.Vision;
                buildSetting.Release();
                break;
            case 1:
                Debug.LogFormat("测试版 {0}", defaultBuildType);
                buildSetting.sVersion = buildArgs.Vision;
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


        if (!Directory.Exists(HotFixBinaryFile))
        {
            Directory.CreateDirectory(HotFixBinaryFile);
        }
        AssetDatabase.Refresh();
        System.IO.File.WriteAllText(string.Format("{0}/{1}{2}", HotFixBinaryFile, version, "GroupToAssetEntries.txt"), str.ToString());
    }
    #endregion


    #region 老的Bundle设置
    //public static void SetAssetBundleName(List<AssetBundleSettingCell> bundleSettingCells)
    //{
    //    CollectAssetBundles(bundleSettingCells, EAssetProcessing.eSetAssetName);
    //}
    //public static void ClearAssetBundleName()
    //{
    //    Debug.LogFormat("Build -> Start ClearAssetBundleName");
    //    string titleName = "清理资源包名";

    //    string[] names = AssetDatabase.GetAllAssetBundleNames();
    //    string name;
    //    for (int i = 0; i < names.Length; ++i)
    //    {
    //        name = names[i];

    //        if (EditorUtility.DisplayCancelableProgressBar(titleName, string.Format("({0}/{1}){2}", i.ToString(), names.Length.ToString(), name), (float)i / (float)names.Length))
    //        {
    //            bStopBuild = true;
    //            break;
    //        }

    //        AssetDatabase.RemoveAssetBundleName(name, true);
    //    }
    //    AssetDatabase.RemoveUnusedAssetBundleNames();

    //    EditorUtility.ClearProgressBar();

    //    Debug.LogFormat("Build -> End ClearAssetBundleName");
    //}
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