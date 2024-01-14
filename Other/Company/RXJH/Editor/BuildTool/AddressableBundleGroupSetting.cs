using Lib.AssetLoader;
using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

public class AddressableBundleGroupSetting
{
    public static int WebRequestRetryCount = 3;
    public static int WebRequestTimeOut = 10;

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



    public static List<AssetBundleSettingCell> AddAssetBundleSettingCell()
    {
        List<AssetBundleSettingCell> mAssetBundleSettingCells = new List<AssetBundleSettingCell>();

        //mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.AnimationClip.ToString(), "*.anim", "*.*"));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.AnimationData.ToString(), "*.asset", "*.*"));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.AnimatorController.ToString(), "*.overrideController|*.controller", "*.*"));
        //mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.Animator.ToString(), "*.controller", "*.*"));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.Atlas.ToString(), "*.spriteatlas", null));
        
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.AvatarMask.ToString(), "*.mask", null));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.BehaviorTree.ToString(), "*.asset", null)); 
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.FaceDetail.ToString(), "*.asset", null));

        //mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.Audio.ToString(), "*.mp3|*.wav", null));
        //mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.Emoji.ToString(), "*.asset", null));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.Font.ToString(), "*.fontsettings|*.ttf", null));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.Material.ToString(), "*.mat", "*.*"));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.Prefab.ToString(), "*.prefab", "*.*"));

        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.Scene.ToString(), "*.unity", "*.*"));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.SceneBase.ToString(), "*.unity", "*.*"));
        //mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.SceneExport.ToString(), "*.*", "*.*"));
        //mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, "Settings", "*.asset", "*.*"));
        //mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.Shader.ToString(), "*.shader|*.shadervariants|*.shadergraph", "*.shader|*.shadergraph", "shader"));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.Texture.ToString(), " *.png", null));
        mAssetBundleSettingCells.Add(new AssetBundleSettingCell(SearchOption.AllDirectories, AddressableGroupName.UI.ToString(), "*.prefab", null));

        return mAssetBundleSettingCells;
    }
    public static void CleanAddressableCache(bool clearCache)
    {
        if (!clearCache) return;

        //ÿ�δ��װ�����Ҫ�������,��Ϊ��λ���û�� �ᵼ�´����Դ����
        AddressableAssetSettings.CleanPlayerContent(null);
        BuildCache.PurgeCache(false);
    }
    public static AddressableAssetSettings ClearAddressableGroup()
    {
        EditorUtility.DisplayCancelableProgressBar("GenAssetBundle", "����Asset Address", 0);

        AddressableAssetSettings assetSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
        for (int i = assetSettings.groups.Count - 1; i >= 0; --i)
        {
            AddressableAssetGroup assetGroup = assetSettings.groups[i];

            if (assetGroup.HasSchema(typeof(PlayerDataGroupSchema)))
            {
                //�ų����õ� Build In Data 
            }
            else if (!assetGroup.Default)
            {
                //ɾ���Խ���������
                assetSettings.RemoveGroup(assetGroup);
            }
            else
            {
                //ȥ��Ĭ�ϱ������µĿ�Ѱַ��Դ
                AddressableAssetEntry[] entries = new AddressableAssetEntry[assetGroup.entries.Count];
                assetGroup.entries.CopyTo(entries, 0);

                for (int j = entries.Length - 1; j >= 0; --j)
                {
                    assetGroup.RemoveAssetEntry(entries[j]);
                }
            }
        }

        //������е�label
        var labeList = assetSettings.GetLabels();
        foreach (var item in labeList)
        {
            assetSettings.RemoveLabel(item);
        }
        assetSettings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        EditorUtility.DisplayCancelableProgressBar("GenAssetBundle", "����Asset Address", 1);

        return assetSettings;
    }
    public static void SetAddressableAssetSettings(string version)
    {
        //1.Addressable �������
        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        string LocalBuildPathStr = "[UnityEngine.AddressableAssets.Addressables.BuildPath]/[BuildTarget]";
        m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kLocalBuildPath, LocalBuildPathStr);
        string LocalLoadPathStr = "{UnityEngine.AddressableAssets.Addressables.RuntimePath}/[BuildTarget]";
        m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kLocalLoadPath, LocalLoadPathStr);

        //2.���� ����ʱ������Ŀ¼���£��Լ�������ذ汾��
        m_Settings.DisableCatalogUpdateOnStartup = true;
        m_Settings.BuildRemoteCatalog = true;
        m_Settings.RemoteCatalogBuildPath.SetVariableByName(m_Settings, AddressableAssetSettings.kRemoteBuildPath);
        m_Settings.RemoteCatalogLoadPath.SetVariableByName(m_Settings, AddressableAssetSettings.kRemoteLoadPath);

        //3.�����װ��Ĺ���Զ����Դ·����catalog.json and catalog.hash��
        string defaultRemoteBuildPathStr = "ServerData/[BuildTarget]";
        m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kRemoteBuildPath, defaultRemoteBuildPathStr);
        FileTool.DeleteFolder("ServerData");

        //4.����Զ�˼���·�� �ַ���
        string defaultRemoteLoadPathStr = "{UnityEngine.AddressableAssets.PathRebuild.RemoteLoadPath}";
        m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kRemoteLoadPath, defaultRemoteLoadPathStr);

        //5.ֻ��Ҫ���װ����ð汾��
        m_Settings.OverridePlayerVersion = version;

        //6.����ͬʱ�������������� 
        m_Settings.MaxConcurrentWebRequests = 3;

        //7.������ã������Դ�ʲ���˳���ʲ����������bundle���Ӷ�����ʲ�����ʱ�䡣
        m_Settings.ContiguousBundles = true;

        //8.catalog ���س�ʱʱ��
        m_Settings.CatalogRequestsTimeout = 10;

        //9.���þ�̬������Դ���ȸ������ɷ�ʽ��������û�õ� �رգ�
        m_Settings.CheckForContentUpdateRestrictionsOption = CheckForContentUpdateRestrictionsOptions.Disabled;

        //10.Ĭ������ layout�������ļ���ʽΪ TXT
        ProjectConfigData.BuildLayoutReportFileFormat = ProjectConfigData.ReportFileFormat.TXT;

        //8.�洢�ڴ��ʼ������  �ֶ�����
        //{UnityEngine.Application.persistentDataPath}/Caching
        //var cacheInitSetting = new CacheInitializationSettings();
        //cacheInitSetting.CreateObjectInitializationData();
    }
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

    private static int RebuildAddressableGroup(List<AssetBundleSettingCell> bundleSettingCells, AddressableAssetSettings settings = null, bool enableTestSence = false, bool getDependencies = true)
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
            Debug.LogErrorFormat("�Ѽ� CollectAssetBundle ����:{0}", rlt);

        count = _mainAssets.Count;

        if (getDependencies)
        {
            int j = 0;
            for (; j < 10 && _outAssetPaths.Count > 0; ++j)
            {
                Debug.LogFormat("����{0} ����{1}", j.ToString(), _outAssetPaths.Count.ToString());
                string[] outAssetPathsArray = _outAssetPaths.ToArray();
                _outAssetPaths.Clear();
                rlt = GetDependencies(outAssetPathsArray, _outAssetPaths);
                if (rlt != 0)
                {
                    break;
                }
            }

            if (rlt != 0)
                Debug.LogErrorFormat("��������Դ��ȡ������ϵ GetDependencies ����:{0}", rlt);


            Debug.LogFormat("����{0} ʣ��{1}", j.ToString(), _outAssetPaths.Count.ToString());
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

                //TODO ��Ҫ��ֹ������� ��ʾ���޸�����
                if (dependenPath.StartsWith("Assets/GameToolEditor"))
                {
                    rlt = 3;
                    Debug.LogErrorFormat("���� Assets/GameToolEditor�����õ�, {0} => {1}", assetPath, dependenPath);
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
                    //TODO ��Ҫ��ֹ������� ��ʾ���޸�����
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
            bundledAssetGroupSchema.UseAssetBundleCrc = false;//�ر�bundle Crc У��
            bundledAssetGroupSchema.UseAssetBundleCache = true;
            bundledAssetGroupSchema.UseAssetBundleCrcForCachedBundles = true;
            bundledAssetGroupSchema.IncludeGUIDInCatalog = false;
            bundledAssetGroupSchema.RetryCount = WebRequestRetryCount;
            bundledAssetGroupSchema.Timeout = WebRequestTimeOut;
            bundledAssetGroupSchema.AssetBundledCacheClearBehavior = BundledAssetGroupSchema.CacheClearBehavior.ClearWhenSpaceIsNeededInCache;
            bundledAssetGroupSchema.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.FullPath;


            assetGroup.GetSchema<ContentUpdateGroupSchema>().StaticContent = false;//(�ض���StreamingAsset)--by yd
            bundledAssetGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.FileNameHash; //ʹ��hash��Ϊ bundle��
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

    public static int CollectAssetBundle(AssetBundleSettingCell cell, AddressableAssetSettings settings = null, bool enableTestSence = false)
    {
        int rlt = 0;

        string relatedDirPath = cell.sRelativePath;
        string searthPattern = cell.sFilter;
    
        SearchOption searchOption = cell.eSearchOption;
        string[] filters = searthPattern.Split('|');

        //��ʾ��
        string titleName = string.Format("�ռ���Դ{0}", relatedDirPath);

        //Ҫ�����ļ���·��
        string folderPath;
        if (string.IsNullOrWhiteSpace(relatedDirPath))
        {
            //�����ResourcesABĿ¼
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
            Debug.LogError("������ folderPath��" + folderPath);
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

            //ȡ����ť�¼�
            if (EditorUtility.DisplayCancelableProgressBar(titleName, string.Format("({0}/{1}){2}", i.ToString(), fileInfos.Count.ToString(), assetPath), (float)i / (float)fileInfos.Count))
            {
                rlt = 1;
                break;
            }

            //1. ����Assetentry �����Ż�������Ҫ����ļ��Ƿ�ͬ����ֻҪƽʱ����λ
            AbbressableAssetEntrySetRule.SetAddressableAssetEntryName(assetPath);

            //2. �ռ�����Դ ====��
            if (!_mainAssets.Contains(assetPath))
            {
                _mainAssets.Add(assetPath);
            }
            else
            {
                Debug.LogErrorFormat("������ͬ������Դ ��{0}", assetPath);
            }

            //3. �ռ���Ҫ�����������Դ ====��
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

    public static int GenLocalAsset_Addressable(BuildSetting buildSetting)
    {
        int rlt = 0;
        //1.���ɱ���Group
        if (buildSetting.bRebuildGroup)
        {
            rlt = GenLocalAssetGroup_Addressable(buildSetting, true);
        }

        if (rlt != 0)
            return rlt;

        //2.��� Bundle
        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult rst);
        if (!string.IsNullOrEmpty(rst.Error))
        {
            Debug.LogFormat("bundle ����ʧ��,ԭ��:{0}", rst.Error);
            rlt = -5;
        }

        //3.ˢ��һ��
        AssetDatabase.Refresh();

        return rlt;
    }


    public static int GenLocalAssetGroup_Addressable(BuildSetting buildSetting, bool FixIssues = true)
    {
        System.Diagnostics.Stopwatch swP = new System.Diagnostics.Stopwatch();
        swP.Start();

        int rlt = 0;

        List<AssetBundleSettingCell> bundleSettingCells = buildSetting.mAssetBundleSettingCells;

        //����group
        AddressableAssetSettings assetSettings = ClearAddressableGroup();

        //���¹���group
        rlt = RebuildAddressableGroup(bundleSettingCells, assetSettings, false);
        if (rlt == 0)
        {
            if (FixIssues)
            {
                MyCheckBundleDupeDependencies myCheckBundle = new MyCheckBundleDupeDependencies();
                myCheckBundle.RefreshAnalysis(assetSettings);
                myCheckBundle.FixIssues(assetSettings);
            }

            //ʹ���Լ��ķ�������
            //ABDependsAnalyze.Analyze(false);

            AssetDatabase.Refresh();
        }

        swP.Stop();
        TimeSpan ts = swP.Elapsed;
        if (FixIssues)
        {
            Debug.LogError(string.Format("��Դ�����ռ� + ���� + ���� ʱ���ܺ�:{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds));
        }
        else
        {
            Debug.LogError(string.Format("��Դ�����ռ� ʱ���ܺ�:{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds));
        }
        return rlt;
    }

    public static int GenHotFixAsset_Addressable(BuildSetting buildSetting)
    {
        int rst = 0;

        //1.�Ȼع����������� LocalGroup 
        if (buildSetting.bRebuildGroup)
        {
            rst = GenLocalAssetGroup_Addressable(buildSetting, true);
        }

        //2.ÿ�θ���һ�εĽ��бȽ���Դ,��Ҫ���ȸ�0��¼�� copy��Library��
        string cacheBinPath = String.Format("{0}/{1}/{2}/{3}/{4}/addressables_content_state.bin", Application.dataPath, BuildTool.AssetBundlePublishPath, buildSetting.mBuildTarget, 0, BuildTool.AddressableBin);
        if (!File.Exists(cacheBinPath))
        {
            Debug.LogError("�������ļ���" + cacheBinPath);
            return -5;
        }

        //�޸ı༭�������� window��ƽ̨�£���bin�ļ���¼
        string desPath = ContentUpdateScript.GetContentStateDataPath(false);
        File.Copy(cacheBinPath, desPath, true);
        AddressablesPlayerBuildResult result = ContentUpdateScript.BuildContentUpdate(AddressableAssetSettingsDefaultObject.Settings, desPath);
        //AddressableAssetSettings.BuildPlayerContent();

        //3.ˢ��һ��
        AssetDatabase.Refresh();

        return rst;
    }


    public static void GenAddressableGroup_New()
    {
        List<AssetBundleSettingCell> AssetBundleSetting = AddAssetBundleSettingCell();
        AddressableAssetSettings assetSettings = ClearAddressableGroup();
        RebuildAddressableGroup(AssetBundleSetting, assetSettings, true, false);
    }
}
