using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public class MyCheckBundleDupeDependencies : CheckBundleDupeDependencies
{

    public enum FilePathType
    {
        None,//复杂的文件
        Scene,
        Charactor,
        Fx
    }
    public override void FixIssues(AddressableAssetSettings settings)
    {
        //为什么要分析2次，修正问题2次才能最终解决问题 ？？？？？

        HashSet<GUID> implicitAssets = new HashSet<GUID>();

        bool hashasImplicitShader = false;
        foreach (var asset in m_ImplicitAssets)
        {
            string assetGuid = asset.ToString();
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

            //剔除属于图集的,不解决图集的重复依赖问题
            if (assetPath.StartsWith("Assets/Projects/Image", System.StringComparison.Ordinal))
                continue;

            //shader 特殊处理，合并到一起
            if (assetPath.EndsWith(".shader", System.StringComparison.Ordinal)
                || assetPath.EndsWith(".shadergraph", System.StringComparison.Ordinal)
                || assetPath.EndsWith(".shadervariants", System.StringComparison.Ordinal))
            {
                var groupShader = CreateOrGetNonStaticGroup(settings, "Shader");
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(assetGuid, groupShader, false, false);
                entry.address = assetGuid;
                entry.SetLabel("shader", true, true);

                hashasImplicitShader = true;
                Debug.LogFormat("有Shader未收集全 {0}", assetPath);
                continue;
            }

            //标记有重复依赖的资源 排除图集引用
            //将其他重复依赖资源 加入hashset 交由基类原有逻辑处理
            implicitAssets.Add(asset);
        }

        m_ImplicitAssets.Clear();

        //当implicitAssets.Count > 0 会调用 settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        if (implicitAssets.Count > 0)
        {
            Debug.LogError("有未收集全的资源");

            m_ImplicitAssets = implicitAssets;
            base.FixIssues(settings);
        }
        else if (hashasImplicitShader)
        {
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        }

        //需要重新设置(默认BundleName格式需要更改) 
        var groupDefault = CreateOrGetNonStaticGroup(settings, "Default Local Group");
#if false
        #region 创建Group

        var groupAnimation = CreateOrGetNonStaticGroup(settings, "__Animation");

        var groupShader = CreateOrGetNonStaticGroup(settings, "Shader");
 

        var groupAsset = CreateOrGetNonStaticGroup(settings, "__Asset");

        var groupPlayable = CreateOrGetNonStaticGroup(settings, "__Playable");

        var groupExrLabel = CreateOrGetNonStaticGroup(settings, "__ExrLabel");

        var groupTexture = CreateOrGetNonStaticGroup(settings, "__Texture");

        var group = CreateOrGetNonStaticGroup(settings, "__AssetIsolation");

        //var groupLabel = CreateOrGetNonStaticGroup(settings, "Duplicate Asset Isolation Label");
        //var groupSceneLabel = CreateOrGetNonStaticGroup(settings, "Duplicate Scene Label");
        //var groupCharactorLabel = CreateOrGetNonStaticGroup(settings, "Duplicate Charactor Label");
        //var groupFXSeparately = CreateOrGetNonStaticGroup(settings, "Duplicate Fx Separately");

        #endregion





        foreach (var asset in m_ImplicitAssets)
        {
            string path = AssetDatabase.GUIDToAssetPath(asset.ToString());
            if (path.StartsWith("Assets/Projects/Image", System.StringComparison.Ordinal))//剔除属于图集的,不解决图集的重复依赖问题
                continue;
            //if (path.EndsWith("/LightingData.asset", System.StringComparison.Ordinal))//剔除场景，因为有个别场景会出现循环引用
            //    continue;
            //if (path.EndsWith(".terrainlayer", System.StringComparison.Ordinal))//过滤笔刷
            //    continue;
            //if (path.EndsWith(".signal", System.StringComparison.Ordinal))//过滤动画帧
            //    continue;

            if (path.EndsWith(".shader", System.StringComparison.Ordinal) 
                || path.EndsWith(".shadergraph", System.StringComparison.Ordinal)
                || path.EndsWith(".shadervariants", System.StringComparison.Ordinal))//Shader合并到一起
            {
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(asset.ToString(), groupShader, false, false);
                entry.SetLabel("shader", true, true);
                continue;
            }
            else if (path.EndsWith(".anim", System.StringComparison.Ordinal))
            {
                settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupAnimation, false, false);
                continue;
            }
            else if (path.EndsWith(".asset", System.StringComparison.Ordinal))
            {
                settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupAsset, false, false);
                continue;
            }
            else if (path.EndsWith(".playable", System.StringComparison.Ordinal))
            {
                settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupPlayable, false, false);
                continue;
            }
            else if (path.EndsWith(".exr", System.StringComparison.Ordinal))
            {
                settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupExrLabel, false, false);
                continue;
            }
            else if (AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(Texture2D))
            {
                settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupTexture, false, false);
                continue;
            }
            else
            {
                settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
            }
            
            //特殊处理的
            //if (path.StartsWith("Packages/com.unity.render-pipelines.universal/Runtime/Materials/", System.StringComparison.Ordinal) ||
            //    path.StartsWith("Assets/Projects/ColorChunk/", System.StringComparison.Ordinal))
            //{
            //    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupLabel, false, false);
            //    continue;
            //}


            //FilePathType filePathType = GetFilePathType(path);
            //switch (filePathType)
            //{
                //case FilePathType.Scene:
                //    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupSceneLabel, false, false);
                //    break;
                //case FilePathType.Charactor:
                //    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupCharactorLabel, false, false);
                //    break;
                //case FilePathType.Fx:
                //    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupFXSeparately, false, false);
                //    break;
                //case FilePathType.None:
                //    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
                //    break;
            //}
            //settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
        }

        //对Group里面的资源进行Label管理
        {
            SetAnimationGroupLabel(settings, "AniMonsterLabel");
            SetAnimationGroupLabel(settings, "AniNPCLabel");
            SetDuplicateExrGroupLabel(settings);
           // SetDuplicateCharactorGroupLabel(settings);
           // SetDuplicateSceneGroupLabel(settings);
           // SetDuplicateAssetIsolationLabel(settings);
        }

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
#endif

    }


    private FilePathType GetFilePathType(string path)
    {
        FilePathType filePathType = FilePathType.None;

        //if (path.StartsWith("Assets/Arts/Scene/"))
        //    filePathType = FilePathType.Scene;

        //if (path.StartsWith("Assets/Arts/Charactor/"))
        //    filePathType = FilePathType.Charactor;
        
        //if (path.StartsWith("Assets/Arts/Fx/"))
        //    filePathType = FilePathType.Fx;
        return filePathType;
    }





    public static void SetAnimationGroupLabel(AddressableAssetSettings settings,string groupName)
    {
        //key:label  value:paths
        Dictionary<string, List<string>> labelPathsDic = new Dictionary<string, List<string>>();
        string labelName = string.Empty;

        AddressableAssetGroup AnimationClipGroup = settings.FindGroup(groupName);
        if (AnimationClipGroup != null)
        {
            //根据标签名进行打包
            BundledAssetGroupSchema bundledAssetGroupSchema = AnimationClipGroup.GetSchema<BundledAssetGroupSchema>();
            if(bundledAssetGroupSchema != null)
                bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;

            foreach (AddressableAssetEntry entry in AnimationClipGroup.entries)
            {
                //ResourcesAB/AnimationClip/Char/Cin_Show/Cin_avatar_01_show/Cin_avatar_01_show_Action_01.anim

                string filePath = entry.AssetPath;
                filePath = filePath.Substring(0, entry.AssetPath.LastIndexOf("/"));
                labelName = filePath.Replace("Assets/ResourcesAB/", "");
               
                if (!labelPathsDic.ContainsKey(labelName))
                {
                    List<string> paths = new List<string>();
                    labelPathsDic.Add(labelName, paths);
                }

                labelPathsDic[labelName].Add(entry.AssetPath);
                entry.SetLabel(labelName, true, true);
            }
            
        }
    }

    public static void SetDuplicateAssetIsolationLabel(AddressableAssetSettings settings)
    {
        //key:label  value:paths
        Dictionary<string, List<string>> labelPathsDic = new Dictionary<string, List<string>>();
        string labelName = string.Empty;

        AddressableAssetGroup IsolationLabelGroup = settings.FindGroup("Duplicate Asset Isolation Label");
        if (IsolationLabelGroup != null)
        {
            //根据标签名进行打包
            BundledAssetGroupSchema bundledAssetGroupSchema = IsolationLabelGroup.GetSchema<BundledAssetGroupSchema>();
            if (bundledAssetGroupSchema != null)
                bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;

            foreach (AddressableAssetEntry entry in IsolationLabelGroup.entries)
            {
                string filePath = entry.AssetPath;
                labelName = filePath.Substring(0, entry.AssetPath.LastIndexOf("/"));

                if (!labelPathsDic.ContainsKey(labelName))
                {
                    List<string> paths = new List<string>();
                    labelPathsDic.Add(labelName, paths);
                }

                labelPathsDic[labelName].Add(entry.AssetPath);
                entry.SetLabel(labelName, true, true);
            }
        }
    }

    public static void SetDuplicateCharactorGroupLabel(AddressableAssetSettings settings)
    {
        //根据标签名进行打包
        AddressableAssetGroup DuplicateCharactorGroup = settings.FindGroup("Duplicate Charactor Label");
        BundledAssetGroupSchema bundledAssetGroupSchema = DuplicateCharactorGroup.GetSchema<BundledAssetGroupSchema>();
        bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;

        //key:label  value:paths
        Dictionary<string, List<string>> labelPathsDic = new Dictionary<string, List<string>>();
        string labelName = string.Empty;

        if (DuplicateCharactorGroup != null)
        {
            //优先统计相同文件下的资源个数
            foreach (AddressableAssetEntry entry in DuplicateCharactorGroup.entries)
            {
                string filePath = entry.AssetPath;
                string[] pathList = filePath.Split('/');
                //Char 和 Accessories 特殊处理
                if (filePath.StartsWith("Assets/Arts/Charactor/Char/") || filePath.StartsWith("Assets/Arts/Charactor/Accessories/")) 
                {
                    labelName = string.Format("{0}/{1}/{2}/{3}", pathList[2], pathList[3], pathList[4], pathList[5]);
                }
                else
                {
                    labelName = string.Format("{0}/{1}/{2}", pathList[2], pathList[3], pathList[4]);
                }
               

                if (!labelPathsDic.ContainsKey(labelName))
                {
                    List<string> paths = new List<string>();
                    labelPathsDic.Add(labelName, paths);
                }

                labelPathsDic[labelName].Add(entry.AssetPath);
                entry.SetLabel(labelName, true, true);
            }
        }
    }


    public static void SetDuplicateSceneGroupLabel(AddressableAssetSettings settings)
    {
        //根据标签名进行打包
        AddressableAssetGroup DuplicateSceneGroup = settings.FindGroup("Duplicate Scene Label");
        BundledAssetGroupSchema bundledAssetGroupSchema = DuplicateSceneGroup.GetSchema<BundledAssetGroupSchema>();
        bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;

        //key:label  value:paths
        Dictionary<string, List<string>> labelPathsDic = new Dictionary<string, List<string>>();
        string labelName = string.Empty;
 
        if (DuplicateSceneGroup != null)
        {
            //优先统计相同文件下的资源个数
            foreach (AddressableAssetEntry entry in DuplicateSceneGroup.entries)
            {
                string filePath = entry.AssetPath;
                string[] pathList = filePath.Split('/');
                string fileName = pathList[3];//第4个 = 文件夹名 分类
                labelName = pathList[2] + "/"+ fileName;
                
                if (!labelPathsDic.ContainsKey(labelName))
                {
                    List<string> paths = new List<string>();
                    labelPathsDic.Add(labelName, paths);
                }

                labelPathsDic[labelName].Add(entry.AssetPath);
                entry.SetLabel(labelName, true, true);
            }
        }
    }





    private void SetDuplicateExrGroupLabel(AddressableAssetSettings settings)
    {
        //根据标签名进行打包
        AddressableAssetGroup ExrGroup = settings.FindGroup("__ExrLabel");
        BundledAssetGroupSchema bundledAssetGroupSchema = ExrGroup.GetSchema<BundledAssetGroupSchema>();
        if (bundledAssetGroupSchema != null)
            bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
        
        //key:label  value:paths
        Dictionary<string, List<string>> labelPathsDic = new Dictionary<string, List<string>>();
        string labelName = string.Empty;
        
        if (ExrGroup != null)
        {
            foreach (AddressableAssetEntry entry in ExrGroup.entries)
            {
                string filePath = entry.AssetPath;
                filePath = filePath.Substring(0, entry.AssetPath.LastIndexOf("/"));
                labelName = filePath.Replace("Assets/ResourcesAB/", "");

                if (!labelPathsDic.ContainsKey(labelName))
                {
                    List<string> paths = new List<string>();
                    labelPathsDic.Add(labelName, paths);
                }

                labelPathsDic[labelName].Add(entry.AssetPath);
                entry.SetLabel(labelName, true, true);
            }
        }
    }

    private AddressableAssetGroup CreateOrGetNonStaticGroup(AddressableAssetSettings settings, string groupName)
    {
        var group = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == groupName);
        if (group == null || groupName == "Default Local Group")
        {
            if(group ==null)
                group = settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));

            //非静态
            group.GetSchema<ContentUpdateGroupSchema>().StaticContent = false;
            //设置远端模
            BundledAssetGroupSchema groupSchema = group.GetSchema<BundledAssetGroupSchema>();
            groupSchema.UseAssetBundleCrc = false;
            groupSchema.UseAssetBundleCache = true;
            groupSchema.UseAssetBundleCrcForCachedBundles = true;
            groupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.FileNameHash;
            groupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
            groupSchema.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.FullPath;
            groupSchema.IncludeGUIDInCatalog = false;
            groupSchema.RetryCount = BuildTool.WebRequestRetryCount;
            groupSchema.Timeout = BuildTool.WebRequestTimeOut;
            groupSchema.AssetBundledCacheClearBehavior = BundledAssetGroupSchema.CacheClearBehavior.ClearWhenSpaceIsNeededInCache;
            groupSchema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
            groupSchema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);
        }



        return group;
    }




    private Dictionary<string, List<string>> AnalysisOfAssetDependency()
    {
        string rootPath = Application.dataPath;
        DirectoryInfo dirInfo = new DirectoryInfo(rootPath);
        string bytesPath = Application.dataPath + "/AppScriptDll/AssetRelationShipByte.bytes";
        Dictionary<string, List<string>> tempDic = new Dictionary<string, List<string>>();
        if (!File.Exists(bytesPath))
        {
            FileInfo[] fs = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            List<string> extionList = new List<string>() { ".anim", ".prefab", ".unity", ".mat", ".fbx", ".controller", ".rendertexture", ".cubemap", ".modulemap", ".asset" };//一级资源后缀
            for (int i = 0; i < fs.Length; i++)
            {
                FileInfo fileInfo = fs[i];
                string fullName = fileInfo.FullName;
                string ext = Path.GetExtension(fullName).ToLower();
                if (ext == ".meta")
                {
                    continue;
                }
                if (!extionList.Contains(ext))
                {
                    continue;
                }
                fullName = fullName.Replace("\\", "/");
                int index = fullName.IndexOf("Assets");
                string assetPath = fullName.Substring(index);

                if (assetPath.StartsWith("Assets/Projects/Image") ||
                  assetPath.StartsWith("Assets/Resources/") ||
                  assetPath.StartsWith("Assets/ResourcesAB/AnimationClip") ||
                  assetPath.StartsWith("Assets/ResourcesAB/Atlas") ||
                  assetPath.StartsWith("Assets/ResourcesAB/Audio") ||
                  assetPath.StartsWith("Assets/ResourcesAB/Font") ||
                  assetPath.StartsWith("Assets/ResourcesAB/Material") ||
                  assetPath.StartsWith("Assets/ResourcesAB/Shader") ||
                  assetPath.StartsWith("Assets/ResourcesAB/Texture"))
                    continue;


                EditorUtility.DisplayProgressBar(i + "+构建关系中+" + fs.Length, fileInfo.Name, (float)i / fs.Length);
                string[] deps = AssetDatabase.GetDependencies(assetPath, false);

                for (int j = 0; j < deps.Length; j++)
                {
                    if (deps[j].StartsWith("Assets/Projects/Image", System.StringComparison.Ordinal) ||
                        deps[j].StartsWith("Assets/Resources/", System.StringComparison.Ordinal) ||
                        deps[j].StartsWith("Assets/ResourcesAB/AnimationClip", System.StringComparison.Ordinal) ||
                        deps[j].StartsWith("Assets/ResourcesAB/Atlas", System.StringComparison.Ordinal) ||
                        deps[j].StartsWith("Assets/ResourcesAB/Audio", System.StringComparison.Ordinal) ||
                        deps[j].StartsWith("Assets/ResourcesAB/Font", System.StringComparison.Ordinal) ||
                        deps[j].StartsWith("Assets/ResourcesAB/Material", System.StringComparison.Ordinal) ||
                        deps[j].StartsWith("Assets/ResourcesAB/Shader", System.StringComparison.Ordinal) ||
                        deps[j].StartsWith("Assets/ResourcesAB/Texture", System.StringComparison.Ordinal))
                        continue;
                    if (deps[j].EndsWith(".cs", System.StringComparison.Ordinal))
                        continue;
                    if (deps[j].EndsWith(".shader", System.StringComparison.Ordinal))
                        continue;
                    if (!tempDic.ContainsKey(deps[j]))
                    {
                        List<string> tempList = new List<string>();
                        tempDic[deps[j]] = tempList;
                    }
                    tempDic[deps[j]].Add(assetPath);
                }
            }

            WriteBinary(bytesPath, tempDic);
            EditorUtility.ClearProgressBar();
        }
        else
        {
            var binary = ReadBinary(bytesPath);
            tempDic = binary as Dictionary<string, List<string>>;
        }
        return tempDic;
    }

    public static void WriteBinary(string path, object graph)
    {
        Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryFormatter binFormat = new BinaryFormatter();
        binFormat.Serialize(fStream, graph);
        fStream.Close();
        AssetDatabase.Refresh();
    }

    static object ReadBinary(string path)
    {
        Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryFormatter binFormat = new BinaryFormatter();
        return binFormat.Deserialize(fStream);
    }
}
