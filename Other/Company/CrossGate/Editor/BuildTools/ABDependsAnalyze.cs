using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public static class ABDependsAnalyze {

    public class ABNode
    {
        public string _name;
        public string _path;
        public bool _isRoot; //是否是root节点

        public bool _isCombine; //是否被依赖项相同，合并成一个ab
        public List<string> _combineList = new List<string>();

        public Dictionary<string, ABNode> _depends = new Dictionary<string, ABNode>();
        public Dictionary<string, ABNode> _beDepends = new Dictionary<string, ABNode>();
    }

    //private static ABDependsAnalyze _Instance = null;
    //public static ABDependsAnalyze I
    //{
    //    get
    //    {
    //        if (_Instance == null) _Instance = new ABDependsAnalyze();
    //        return _Instance;
    //    }
    //}

    public static Dictionary<string, ABNode> _ABNodeList = new Dictionary<string, ABNode>(); //<path, node>
    //private static List<AssetBundleBuild> _ABBuildList = new List<AssetBundleBuild>();

    //private  void ABDependsAnalyze()
    //{
    //    _ABNodeList = new Dictionary<string, ABNode>();
    //    _ABBuildList = new List<AssetBundleBuild>();
    //}

    private static void Clear()
    {
        _ABNodeList.Clear();
        //_ABBuildList.Clear();
    }

    [MenuItem("__Tools__/打包相关/分析依赖关系(还需优化)")]
    static void SetAllBundleName()
    {
        Analyze(true);
    } 
    

    //这里传入的路径最好是全路径, 返回需要打包的文件列表
    //然后调用BuildPipeline.BuildAssetBundles(exportPath, AssetBundleBuild[], xxx,xxx);就可以了
    public static void Analyze(bool ClearAllGroup)
    {
        //分组全部
        //if(ClearAllGroup)
        //TRAssetsProcessor.GenAddressableGroup_New();
        
        //获得所有的asset 
        List<string> fs = new List<string>();
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(false);
        var allEntries = new List<AddressableAssetEntry>();
        settings.GetAllAssets(allEntries, false, g =>
        {
            if (g == null)
                return false;

            if (!g.HasSchema<ContentUpdateGroupSchema>() || !g.GetSchema<ContentUpdateGroupSchema>().StaticContent)
                return false;
            if (!g.HasSchema<BundledAssetGroupSchema>() || !g.GetSchema<BundledAssetGroupSchema>().IncludeInBuild)
                return false;

            //过滤组
            //if (g.Name == "AnimationClip" || g.Name == "Atlas" || g.Name == "Audio" || g.Name == "Emoji" || g.Name  == "Font" || g.Name == "Texture")
            //    return false;
            if (g.Name == "Material" || g.Name == "Prefab" || g.Name == "Scene" || g.Name == "UI")
                return true;

            return false;
        });

        
        foreach (var item in allEntries)
        {
            fs.Add(item.AssetPath);
        }
        

        AnalyzeComplex(fs);
    }

    private static bool IsFileVaild(string filePath)
    {//读者可以自己实现文件有效性
     //filePath = filePath.ToLower();
        string ext = Path.GetExtension(filePath).ToLower();

        if (ext == ".meta") return false;
        if (ext == ".cs") return false;
        if (ext == ".dll") return false;
        if (filePath.EndsWith("LightingData.asset")) return false; //即使没改，还是会发生变化
        //if (ext == ".shader") return false;

        if (filePath.StartsWith("Assets/Projects/Image") ||
            filePath.StartsWith("Assets/Resources/") ||
            filePath.StartsWith("Assets/ResourcesAB/Font") ||
            filePath.StartsWith("Assets/ResourcesAB/Material") ||
            filePath.StartsWith("Assets/ResourcesAB/Shader") ||
            filePath.StartsWith("Assets/ResourcesAB/Texture") ||
            filePath.StartsWith("Assets/ResourcesAB/Emoji") ||
            filePath.StartsWith("Assets/ResourcesAB/AnimationClip") ||
            filePath.StartsWith("Assets/ResourcesAB/Scene") ||//忽略场景会被LightingData给引用
            filePath.StartsWith("Assets/GameToolEditor/"))
            return false;

        return true;
    }

    private static string GetAbName(ABNode abNode)
    {//读者可以自己实现路径转换方式，这边提供一种方法
        if (abNode._isRoot)
        {//root节点以所在路径为加载路径
            return abNode._name.Replace("Assets/", "") + ".ab";
        }
        else
        {//依赖节点以guid作为路径
            return "depends/" + AssetDatabase.AssetPathToGUID(abNode._path) + ".ab";
        }
    }
    private static string curRootAsset = string.Empty;
    private static float curProgress = 0f;
    private static void AnalyzeComplex(List<string> files) 
    {
        Clear();

        //建立依赖节点，生成一张有向无环图
        int ind = 0;
        for (int i = 0; i < files.Count; ++i)
        {
            string unityPath = files[i];
            curProgress = (float)ind / (float)files.Count;
            curRootAsset = "正在分析依赖：" + unityPath;
            EditorUtility.DisplayProgressBar(curRootAsset, curRootAsset, curProgress);
            ind++;

            var abNode = CreateNode(unityPath, true);
            if (!_ABNodeList.ContainsKey(unityPath))
            {
                _ABNodeList.Add(unityPath, abNode);
                AnalyzeNode(abNode);
            }
            else abNode._isRoot = true;
        }

        //打印一下
        Debug.LogError("_ABNodeList.Count:" + _ABNodeList.Count);



        //操作得到实际的引用关系  别人  P->M   M->T   不再是  P引用了M和T了   破除由环形依赖照成的问题
        foreach (var item in _ABNodeList.Values)
        {
            if (item._isRoot)//根资源本身独立打包
                continue;

            foreach (var cell in item._beDepends)
            {
                if (_ABNodeList.ContainsKey(cell.Key))
                {
                    foreach (var Depcell in item._depends)
                    {
                        if (Depcell.Value._beDepends.ContainsKey(cell.Key))// && !Depcell.Value._isRoot
                        {
                            Depcell.Value._beDepends.Remove(cell.Key);
                            cell.Value._depends.Remove(Depcell.Key);
                        }
                    }
                }
            }
        }

        //操作得到是去掉引用为1的并且不是root的节点   同时将他的引用记录到他父节点上
        bool flag = true;
        while (flag)
        {
            flag = false;
            var abNodes = _ABNodeList.Values.ToArray();
            foreach (var abNode in abNodes)
            {
                if (abNode._isRoot) continue;
                if (abNode._beDepends.Count != 1) continue;

                var beDepend = abNode._beDepends.Values.ToArray()[0];

                beDepend._depends.Remove(abNode._path);
                foreach (var depend in abNode._depends.Values)
                {
                    depend._beDepends.Remove(abNode._path);

                    if (!depend._beDepends.ContainsKey(beDepend._path))
                        depend._beDepends.Add(beDepend._path, beDepend);
                    if (!beDepend._depends.ContainsKey(depend._path))
                        beDepend._depends.Add(depend._path, depend);
                }

                _ABNodeList.Remove(abNode._path);
                flag = true;
            }
        }


        //合点，减少总数量  ----暂时先关闭
        //相同依赖合并成1个节点，比如A->B,A->C,D->B,D->C,那么BC可以打到一个ab里面，减少ab数量
        flag = true;
        while (flag)
        {
            flag = false;
            var abNodes = _ABNodeList.Values.OrderBy(a => a._path).ToArray();
            for (int i = 0; i < abNodes.Length; i++)
            {
                if (abNodes[i]._isRoot) continue;
                if (abNodes[i]._isCombine) continue;
                if (abNodes[i]._path.ToLower().EndsWith(".shader")) continue;

                for (int j = i + 1; j < abNodes.Length; j++)
                {
                    if (abNodes[j]._isRoot) continue;
                    if (abNodes[j]._isCombine) continue;
                    if (abNodes[j]._path.ToLower().EndsWith(".shader")) continue;

                    if (!IsBeDependsEqual(abNodes[i], abNodes[j])) continue;

                    abNodes[i]._combineList.Add(abNodes[j]._path);
                    abNodes[j]._isCombine = true;
                    flag = true;
                }
            }
        }

        EditorUtility.ClearProgressBar();

        DebugLogFile();
        DebugcombineList();
        RefreshDuplicateGroup();
    }


    //分析复杂分组的资源依赖情况 --打印给策划看

    public static void RefreshDuplicateGroup()
    {
        //Duplicate Scene Material
        CheckAssetGroupReference("Duplicate Scene Material");
        //Duplicate Charactor Material
        CheckAssetGroupReference("Duplicate Charactor Material");


        //Duplicate Scene Tga
        CheckAssetGroupReference("Duplicate Scene Tga");
        //Duplicate Charactor Tga
        CheckAssetGroupReference("Duplicate Charactor Tga");

        
        //Duplicate Scene Psd
        CheckAssetGroupReference("Duplicate Scene Psd");
        //Duplicate Charactor Psd
        CheckAssetGroupReference("Duplicate Charactor Psd");


        //Duplicate Scene Png
        CheckAssetGroupReference("Duplicate Scene Png");
       //Duplicate Charactor Png
        CheckAssetGroupReference("Duplicate Charactor Png");
        
    }


    private static void CheckAssetGroupReference(string groupName)
    {
        string filePath = string.Empty;
        Dictionary<string, List<string>> labelPathsDic = new Dictionary<string, List<string>>();

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(false);
        AddressableAssetGroup DuplicateGroup = settings.FindGroup(groupName);
        if (DuplicateGroup != null)
        {
            //优先统计相同文件下的资源个数
            foreach (AddressableAssetEntry entry in DuplicateGroup.entries)
            {
                filePath = entry.AssetPath.Substring(0, entry.AssetPath.LastIndexOf("/"));
                if (!labelPathsDic.ContainsKey(filePath))
                {
                    List<string> paths = new List<string>();
                    labelPathsDic.Add(filePath, paths);
                }
                labelPathsDic[filePath].Add(entry.AssetPath);
            }
        }

        if (labelPathsDic == null || labelPathsDic.Count <= 0)
            return;


        //对于同一份文件夹下的资源，这些资源的被依赖列表是否一样，不一样，把地址打印出来(证明资源引用错乱)
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        foreach (var item in labelPathsDic)
        {
            List<string> tempList = item.Value;
            if (tempList.Count < 2)
                continue;

            //统计同一文件夹下，是不是有资源可合并
            stringBuilder.AppendLine("同一文件夹下的资源如下：");
            string tempPath = string.Empty;
            foreach (var cell in tempList)
            {
                stringBuilder.AppendFormat("\t{0}\n", cell);
            }

            stringBuilder.AppendLine();
            #region 折叠代码
            //foreach (var cell in tempList)
            //{
            //    if (_ABNodeList.ContainsKey(cell))
            //    {
            //        if (_ABNodeList[cell]._combineList.Count > 1 && _ABNodeList[cell]._isCombine == false)
            //        {
            //            tempPath = cell;
            //        }
            //    }
            //    else
            //    {
            //        //Debug.LogErrorFormat("_ABNodeList不包含key：{0}", cell);
            //    }
            //}


            //List<string> canCombine = new List<string>();
            //List<string> noCanCombine = new List<string>();

            //if (!string.IsNullOrEmpty(tempPath))
            //{
            //    var combineList = _ABNodeList[tempPath]._combineList;
            //    stringBuilder.AppendLine("可合并,被依赖列表可相同：");
            //    foreach (var cell in tempList)
            //    {
            //        if (!combineList.Contains(cell))
            //        {
            //            noCanCombine.Add(cell);
            //        }
            //        else
            //        {
            //            canCombine.Add(cell);
            //        }
            //    }


            //    //打印可以合并 和 不可合并的列表
            //    string depPath = string.Empty;
            //    foreach (var cell in canCombine)
            //    {
            //        if (_ABNodeList.ContainsKey(cell))
            //        {
            //            stringBuilder.AppendFormat("\t{0}{1}\n", "可合：", cell);
            //            depPath = cell;
            //        }
            //        else
            //        {
            //            stringBuilder.AppendFormat("\t{0}{1}\t{2}\n", "可合：", cell, "ABNodeList不存在");
            //        }
            //    }

            //    if (!string.IsNullOrEmpty(depPath) &&_ABNodeList[depPath]._beDepends != null)
            //    {
            //        foreach (var dep in _ABNodeList[depPath]._beDepends)
            //        {
            //            stringBuilder.AppendFormat("\t\t{0}\n", dep.Key);
            //        }
            //    }


            //    foreach (var cell in noCanCombine)
            //    {
            //        if (_ABNodeList.ContainsKey(cell))
            //        {
            //            stringBuilder.AppendFormat("\t{0}\t{1}\t被依赖的个数:{1}\n", "不可合：", cell, _ABNodeList[cell]._beDepends.Count);
            //            if (_ABNodeList[cell]._beDepends != null)
            //            {
            //                foreach (var dep in _ABNodeList[cell]._beDepends)
            //                {
            //                    stringBuilder.AppendFormat("\t\t{0}\n", dep.Key);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            stringBuilder.AppendFormat("\t{0}\t{1}\n", cell, "ABNodeList不存在");
            //        }
            //    }
            //}
            //else
            //{
            //    stringBuilder.AppendLine("不可合并,被依赖列表不同：");
            //    foreach (var cell in tempList)
            //    {
            //        if (_ABNodeList.ContainsKey(cell))
            //        {
            //            stringBuilder.AppendFormat("\t{0}\t被依赖的个数:{1}\n", cell, _ABNodeList[cell]._beDepends.Count);

            //            if (_ABNodeList[cell]._beDepends != null)
            //            {
            //                foreach (var dep in _ABNodeList[cell]._beDepends)
            //                {
            //                    stringBuilder.AppendFormat("\t\t{0}\n", dep.Key);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            stringBuilder.AppendFormat("\t{0}{1}\n", "_ABNodeList不包含key：",cell);
            //        }
            //    }
            //}
            #endregion

            foreach (var cell in tempList)
            {
                if (_ABNodeList.ContainsKey(cell))
                {
                    if (_ABNodeList[cell]._beDepends != null)
                    {
                        stringBuilder.AppendFormat("\t{0}\t被依赖的个数：{1}\n", cell, _ABNodeList[cell]._beDepends.Count);
                        foreach (var dep in _ABNodeList[cell]._beDepends)
                        {
                            stringBuilder.AppendFormat("\t\t{0}\n", dep.Key);
                        }
                    }
                }
                else
                {
                    stringBuilder.AppendFormat("\t{0}\t{1}\n", cell,"ABNodeList不包含");
                }
            }

            stringBuilder.AppendLine("------------------------------------------------------------------------------------");
   
            //var beDepList = _ABNodeList[item.Value[0]]._beDepends.Keys.ToArray();//标准依据
            //for (int i = item.Value.Count - 1; i > 0; i--)
            //{
            //    var comparerList = _ABNodeList[item.Value[i]]._beDepends.Keys.ToArray();
            //    if (!beDepList.Equals(comparerList))
            //        Debug.LogErrorFormat("{0} != {1} 被依赖列表不同", item.Value[0], item.Value[i]);
            //}
        }

        string LogFileDir = Path.Combine(Application.dataPath, "LogABDep").Replace("\\", "/");
        if (!Directory.Exists(LogFileDir))
        {
            Directory.CreateDirectory(LogFileDir);
        }
        AssetDatabase.Refresh();
        string path = string.Format("{0}/{1}{2}", LogFileDir, groupName.Trim(), " Group.txt");
        System.IO.File.WriteAllText(path, stringBuilder.ToString());
        AssetDatabase.Refresh();

    }

    
    private static bool IsBeDependsEqual(ABNode a, ABNode b)
    {
        if (a._beDepends.Count != b._beDepends.Count) return false;
        if (a._beDepends.Count == 0) return false;

        foreach (var beDepend in a._beDepends.Values)
        {
            if (!b._beDepends.ContainsKey(beDepend._path)) return false;
        }

        return true;
    }

    private static ABNode CreateNode(string filePath, bool isRoot)
    {
        ABNode abNode = null;
        if (_ABNodeList.ContainsKey(filePath))
        {
            abNode = _ABNodeList[filePath];
            return abNode;
        }

        abNode = new ABNode();
        abNode._name = Path.GetFileNameWithoutExtension(filePath);
        abNode._path = filePath;
        abNode._isRoot = isRoot;
        abNode._combineList.Add(filePath);

        return abNode;
    }

    private static void AnalyzeNode(ABNode abNode)
    {
        string[] depend_paths = AssetDatabase.GetDependencies(abNode._path,true);//所有依赖，以及依赖的依赖
        foreach (var tempDependPath in depend_paths)
        {
            string dependPath = tempDependPath;
            if (dependPath == abNode._path) continue;

            if (!IsFileVaild(dependPath)) continue;

            ABNode abDependNode = CreateNode(dependPath, false);
            abNode._depends.Add(dependPath, abDependNode);
            abDependNode._beDepends.Add(abNode._path, abNode);

            if (!_ABNodeList.ContainsKey(dependPath))
            {
                _ABNodeList.Add(dependPath, abDependNode);
                EditorUtility.DisplayProgressBar(curRootAsset, dependPath, curProgress);
                AnalyzeNode(abDependNode);
            }
        }

    }

    //这样本身并没有错，错在循环依赖就陷入死循环了
    private static bool DeleteRepeatNode(ABNode abNode, ABNode deleteNode)
    {
        bool flag = false;
        if (abNode._depends.ContainsKey(deleteNode._path))
        {
            abNode._depends.Remove(deleteNode._path);
            deleteNode._beDepends.Remove(abNode._path);

            //Repo.Log(ELogType.Load, "delete " + abNode._path + "\n" + deleteNode._path);
            flag = true;
        }

        var beDepends = abNode._beDepends.Values.ToArray();
        foreach (var beDependNode in beDepends)
        {
            if (beDependNode._beDepends.ContainsKey(abNode._path))
            {
                flag |= false;
                continue;
            }

            flag |= DeleteRepeatNode(beDependNode, deleteNode);
        }

        return flag;
    }



    public static void FixIssues()
    {
        #region 创建Group
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(false);

        #region FBX Group
        var groupSceneFBX = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Scene FBX");
        if (groupSceneFBX == null)
        {
            groupSceneFBX = settings.CreateGroup("Duplicate Scene FBX", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupSceneFBX.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupFBXGroupSchema = groupSceneFBX.GetSchema<BundledAssetGroupSchema>();
            groupFBXGroupSchema.UseAssetBundleCrc = false;
            groupFBXGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupFBXGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }

        var groupCharactorFBX = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Charactor FBX");
        if (groupCharactorFBX == null)
        {
            groupCharactorFBX = settings.CreateGroup("Duplicate Charactor FBX", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupCharactorFBX.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupFBXGroupSchema = groupCharactorFBX.GetSchema<BundledAssetGroupSchema>();
            groupFBXGroupSchema.UseAssetBundleCrc = false;
            groupFBXGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupFBXGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }

        var groupFxFBX = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Fx FBX");
        if (groupFxFBX == null)
        {
            groupFxFBX = settings.CreateGroup("Duplicate Fx FBX", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupFxFBX.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupFBXGroupSchema = groupFxFBX.GetSchema<BundledAssetGroupSchema>();
            groupFBXGroupSchema.UseAssetBundleCrc = false;
            groupFBXGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupFBXGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }
        #endregion

        #region Mat Group
        var groupSceneMat = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Scene Material");
        if (groupSceneMat == null)
        {
            groupSceneMat = settings.CreateGroup("Duplicate Scene Material", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupSceneMat.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupMatGroupSchema = groupSceneMat.GetSchema<BundledAssetGroupSchema>();
            groupMatGroupSchema.UseAssetBundleCrc = false;
            groupMatGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupMatGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }


        var groupCharactorMat = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Charactor Material");
        if (groupCharactorMat == null)
        {
            groupCharactorMat = settings.CreateGroup("Duplicate Charactor Material", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupCharactorMat.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupMatGroupSchema = groupCharactorMat.GetSchema<BundledAssetGroupSchema>();
            groupMatGroupSchema.UseAssetBundleCrc = false;
            groupMatGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupMatGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }

        var groupFxMat = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Fx Material");
        if (groupFxMat == null)
        {
            groupFxMat = settings.CreateGroup("Duplicate Fx Material", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupFxMat.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupMatGroupSchema = groupFxMat.GetSchema<BundledAssetGroupSchema>();
            groupMatGroupSchema.UseAssetBundleCrc = false;
            groupMatGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupMatGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }
        #endregion


        var groupAnimation = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Animation");
        if (groupAnimation == null)
        {
            groupAnimation = settings.CreateGroup("Duplicate Animation", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupAnimation.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupAnimationGroupSchema = groupAnimation.GetSchema<BundledAssetGroupSchema>();
            groupAnimationGroupSchema.UseAssetBundleCrc = false;
            groupAnimationGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupAnimationGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }


        var groupShader = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Shader");
        if (groupShader == null)
        {
            groupShader = settings.CreateGroup("Shader", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupShader.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupShaderGroupSchema = groupShader.GetSchema<BundledAssetGroupSchema>();
            groupShaderGroupSchema.UseAssetBundleCrc = false;
            groupShaderGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupShaderGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
        }

        //以下是新增的group
        var groupController = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Controller");
        if (groupController == null)
        {
            groupController = settings.CreateGroup("Duplicate Controller", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupController.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupControllerGroupSchema = groupController.GetSchema<BundledAssetGroupSchema>();
            groupControllerGroupSchema.UseAssetBundleCrc = false;
            groupControllerGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupControllerGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }


        var groupAsset = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Asset");
        if (groupAsset == null)
        {
            groupAsset = settings.CreateGroup("Duplicate Asset", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupAsset.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupAssetGroupSchema = groupAsset.GetSchema<BundledAssetGroupSchema>();
            groupAssetGroupSchema.UseAssetBundleCrc = false;
            groupAssetGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }


        var groupPlayable = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Playable");
        if (groupPlayable == null)
        {
            groupPlayable = settings.CreateGroup("Duplicate Playable", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupPlayable.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupPlayableGroupSchema = groupPlayable.GetSchema<BundledAssetGroupSchema>();
            groupPlayableGroupSchema.UseAssetBundleCrc = false;
            groupPlayableGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupPlayableGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }


        var groupObj = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Obj");
        if (groupObj == null)
        {
            groupObj = settings.CreateGroup("Duplicate Obj", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupObj.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupObjGroupSchema = groupObj.GetSchema<BundledAssetGroupSchema>();
            groupObjGroupSchema.UseAssetBundleCrc = false;
            groupObjGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupObjGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }

        
        var groupTga = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Tga");
        if (groupTga == null)
        {
            groupTga = settings.CreateGroup("Duplicate Tga", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupTga.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupTgaGroupSchema = groupTga.GetSchema<BundledAssetGroupSchema>();
            groupTgaGroupSchema.UseAssetBundleCrc = false;
            groupTgaGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupTgaGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }


        var groupPsd = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Psd");
        if (groupPsd == null)
        {
            groupPsd = settings.CreateGroup("Duplicate Psd", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupPsd.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupPsdGroupSchema = groupPsd.GetSchema<BundledAssetGroupSchema>();
            groupPsdGroupSchema.UseAssetBundleCrc = false;
            groupPsdGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupPsdGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }



        var groupPng = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Png");
        if (groupPng == null)
        {
            groupPng = settings.CreateGroup("Duplicate Png", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            groupPng.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupPngGroupSchema = groupPng.GetSchema<BundledAssetGroupSchema>();
            groupPngGroupSchema.UseAssetBundleCrc = false;
            groupPngGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupPngGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }


        var group = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == "Duplicate Asset Isolation");
        if (group == null)
        {
            group = settings.CreateGroup("Duplicate Asset Isolation", false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            group.GetSchema<ContentUpdateGroupSchema>().StaticContent = true;
            BundledAssetGroupSchema groupGroupSchema = group.GetSchema<BundledAssetGroupSchema>();
            groupGroupSchema.UseAssetBundleCrc = false;
            groupGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            groupGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }
        #endregion

        //被依赖列表一样，则合并
        var ABValueList =_ABNodeList.Values.ToArray();
        foreach (var item in ABValueList)
        {
            if (item._isCombine)
            {
                //可以合并的
                Debug.LogErrorFormat("{0}  count:{2}",item._path,item._combineList.Count);
            }
        }
        

        foreach (var item in _ABNodeList)
        {
            if (item.Value._isRoot == true)
                continue;
            if (item.Value._beDepends.Count == 1)
                continue;
            if (item.Key.EndsWith(".terrainlayer"))//过滤笔刷
                continue;
            if (item.Key.EndsWith(".signal"))//过滤TimeLine里面的动画帧
                continue;
            if (item.Key.EndsWith(".prefab")&&item.Key.StartsWith("Assets/Arts/Scene"))//过滤prefab Scene里面的预制体是直接写入到Scene
                continue;


            //针对被重复引用的资源，需要单独打包
            if (item.Value._beDepends.Count > 1)
            {
                string path = item.Key;

                if (path.EndsWith(".shader"))//Shader合并到一起
                {
                    AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupShader, false, false);
                    entry.SetLabel("shader", true, true);
                    continue;
                }

                if (path.EndsWith(".FBX") || path.EndsWith(".fbx"))
                {
                    if (path.StartsWith("Assets/Arts/Charactor"))
                    {
                        settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupCharactorFBX, false, false);
                    }
                    else if (path.StartsWith("Assets/Arts/Scene"))
                    {
                        settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupSceneFBX, false, false);
                    }
                    else //StartsWith("Assets/Arts/Fx")
                    {
                        settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupFxFBX, false, false);
                    }
                    continue;
                }

                if (path.EndsWith(".mat"))
                {
                    if (path.StartsWith("Assets/Arts/Charactor"))
                    {
                        settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupCharactorMat, false, false);
                    }
                    else if (path.StartsWith("Assets/Arts/Scene"))
                    {
                        settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupSceneMat, false, false);
                    }
                    else //StartsWith("Assets/Arts/Fx")
                    {
                        settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupFxMat, false, false);
                    }
                    continue;
                }

                if (path.EndsWith(".anim"))
                {
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupAnimation, false, false);
                    continue;
                }

                //以下是新增
                if (path.EndsWith(".tga") || path.EndsWith(".TGA"))
                {
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupTga, false, false);
                    continue;
                }
                if (path.EndsWith(".psd") || path.EndsWith(".PSD"))
                {
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupPsd, false, false);
                    continue;
                }
                if (path.EndsWith(".png") || path.EndsWith(".PNG"))
                {
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupPng, false, false);
                    continue;
                }
                if (path.EndsWith(".controller"))
                {
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupController, false, false);
                    continue;
                }
                if (path.EndsWith(".asset"))//LightingData 也在里面
                {
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupAsset, false, false);
                    continue;
                }
                if (path.EndsWith(".playable"))
                {
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupPlayable, false, false);
                    continue;
                }
                if (path.EndsWith(".obj"))
                {
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), groupObj, false, false);
                    continue;
                }
                settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
            }
        }

        //对Group里面的资源进行Label管理   只暂时对这些进行合并
        {
            SetAnimationGroupLabel(settings);
            SetDuplicateCharactorMatGroupLabel(settings);
            SetDuplicateSceneMatGroupLabel(settings);
        }

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        
    }





    public static void SetAnimationGroupLabel(AddressableAssetSettings settings)
    {
        Dictionary<string, int> labelPathsDic = new Dictionary<string, int>();
        string filePath = string.Empty;
        AddressableAssetGroup AnimationClipGroup = settings.FindGroup("AnimationClip");
        if (AnimationClipGroup != null)
        {
            foreach (AddressableAssetEntry entry in AnimationClipGroup.entries)
            {
                filePath = entry.AssetPath.Substring(0, entry.AssetPath.LastIndexOf("/"));
                filePath = filePath.Replace("Assets/ResourcesAB/", "");
                if (!labelPathsDic.ContainsKey(filePath))
                {
                    labelPathsDic.Add(filePath, 1);
                }
                else
                {
                    labelPathsDic[filePath]++;
                }
            }


            foreach (AddressableAssetEntry entry in AnimationClipGroup.entries)
            {
                filePath = entry.AssetPath.Substring(0, entry.AssetPath.LastIndexOf("/"));
                filePath = filePath.Replace("Assets/ResourcesAB/", "");
                if (labelPathsDic.ContainsKey(filePath))
                {
                    entry.labels.Clear();
                    if (!entry.labels.Contains(filePath))
                        entry.labels.Add(filePath);
                    entry.SetLabel(filePath, true, true);
                }
            }

            //根据标签名进行打包
            BundledAssetGroupSchema bundledAssetGroupSchema = AnimationClipGroup.GetSchema<BundledAssetGroupSchema>();
            bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
        }
    }
    public static void SetDuplicateCharactorMatGroupLabel(AddressableAssetSettings settings)
    {
        //key:label  value:paths
        Dictionary<string, List<string>> labelPathsDic = new Dictionary<string, List<string>>();
        string filePath = string.Empty;
        AddressableAssetGroup DuplicateCharactorMaterialGroup = settings.FindGroup("Duplicate Charactor Material");
        if (DuplicateCharactorMaterialGroup != null)
        {
            //优先统计相同文件下的资源个数
            foreach (AddressableAssetEntry entry in DuplicateCharactorMaterialGroup.entries)
            {
                filePath = entry.AssetPath.Substring(0, entry.AssetPath.LastIndexOf("/"));

                if (filePath.StartsWith("Assets/Arts/Charactor"))
                {
                    filePath = filePath.Replace("Assets/Arts/Charactor", "Charactor");
                }

                if (!labelPathsDic.ContainsKey(filePath))
                {
                    List<string> paths = new List<string>();
                    labelPathsDic.Add(filePath, paths);
                }
                labelPathsDic[filePath].Add(entry.AssetPath);
            }


            //对于同一份文件夹下的资源，这些资源的被依赖列表是否一样，不一样，把地址打印出来(证明资源引用错乱)
            //foreach (var item in labelPathsDic)
            //{
            //    if (item.Value.Count > 1)
            //    {
            //        var beDepList = _ABNodeList[item.Value[0]]._beDepends.Keys.ToArray();//标准依据
            //        for (int i = item.Value.Count-1; i > 0; i--)
            //        {
            //            var comparerList = _ABNodeList[item.Value[i]]._beDepends.Keys.ToArray();
            //            if (!beDepList.Equals(comparerList))
            //                Debug.LogErrorFormat("{0} != {1} 被依赖列表不同", item.Value[0], item.Value[i]);
            //        }
            //    }
            //}

            
            //设置标签名
            foreach (AddressableAssetEntry entry in DuplicateCharactorMaterialGroup.entries)
            {
                filePath = entry.AssetPath.Substring(0, entry.AssetPath.LastIndexOf("/"));

                if (filePath.StartsWith("Assets/Arts/Charactor"))//简编标签名
                {
                    filePath = filePath.Replace("Assets/Arts/Charactor", "Charactor");
                }

                if (labelPathsDic.ContainsKey(filePath))
                {
                    entry.labels.Clear();
                    if (!entry.labels.Contains(filePath))
                        entry.labels.Add(filePath);
                    entry.SetLabel(filePath, true, true);
                }
            }

            //根据标签名进行打包
            BundledAssetGroupSchema bundledAssetGroupSchema = DuplicateCharactorMaterialGroup.GetSchema<BundledAssetGroupSchema>();
            bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
        }
    }

    public static void SetDuplicateSceneMatGroupLabel(AddressableAssetSettings settings)
    {
        //key:label  value:paths
        Dictionary<string, List<string>> labelPathsDic = new Dictionary<string, List<string>>();
        string filePath = string.Empty;
        AddressableAssetGroup DuplicateCharactorMaterialGroup = settings.FindGroup("Duplicate Scene Material");
        if (DuplicateCharactorMaterialGroup != null)
        {
            //优先统计相同文件下的资源个数
            foreach (AddressableAssetEntry entry in DuplicateCharactorMaterialGroup.entries)
            {
                filePath = entry.AssetPath.Substring(0, entry.AssetPath.LastIndexOf("/"));

                if (filePath.StartsWith("Assets/Arts/Scene"))
                {
                    filePath = filePath.Replace("Assets/Arts/Scene", "Scene");
                }

                if (!labelPathsDic.ContainsKey(filePath))
                {
                    List<string> paths = new List<string>();
                    labelPathsDic.Add(filePath, paths);
                }
                labelPathsDic[filePath].Add(entry.AssetPath);
            }


            //对于同一份文件夹下的资源，这些资源的被依赖列表是否一样，不一样，把地址打印出来(证明资源引用错误)
            //foreach (var item in labelPathsDic)
            //{
            //    if (item.Value.Count > 1)
            //    {
            //        var beDepList = _ABNodeList[item.Value[0]]._beDepends.Keys.ToArray();//标准依据
            //        for (int i = item.Value.Count - 1; i > 0; i--)
            //        {
            //            var comparerList = _ABNodeList[item.Value[i]]._beDepends.Keys.ToArray();
            //            if (!beDepList.Equals(comparerList))
            //                Debug.LogErrorFormat("{0} != {1} 被依赖列表不同", item.Value[0], item.Value[i]);
            //        }
            //    }
            //}


            //设置标签名
            foreach (AddressableAssetEntry entry in DuplicateCharactorMaterialGroup.entries)
            {
                filePath = entry.AssetPath.Substring(0, entry.AssetPath.LastIndexOf("/"));

                if (filePath.StartsWith("Assets/Arts/Scene"))//简编标签名
                {
                    filePath = filePath.Replace("Assets/Arts/Scene", "Scene");
                }

                if (labelPathsDic.ContainsKey(filePath))
                {
                    entry.labels.Clear();
                    if (!entry.labels.Contains(filePath))
                        entry.labels.Add(filePath);
                    entry.SetLabel(filePath, true, true);
                }
            }

            //根据标签名进行打包
            BundledAssetGroupSchema bundledAssetGroupSchema = DuplicateCharactorMaterialGroup.GetSchema<BundledAssetGroupSchema>();
            bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
        }
    }



    private static void DebugLogFile()
    {
        System.Text.StringBuilder str = new System.Text.StringBuilder();
        foreach (var item in _ABNodeList)
        {
            str.AppendFormat("{0}\t依赖的个数:{1}\n", item.Key, item.Value._depends.Count);
            if (item.Value._depends != null)
            {
                foreach (var dep in item.Value._depends)
                {
                    str.AppendFormat("\t{0} \n", dep.Key);
                }
            }

            str.AppendFormat("{0}\t被依赖的个数:{1}\n", item.Key, item.Value._beDepends.Count);

            if (item.Value._beDepends != null)
            {
                foreach (var dep in item.Value._beDepends)
                {
                    str.AppendFormat("\t{0} \n", dep.Key);
                }
            }

            str.AppendLine("------------------------------------------------------------------------------------");
        }

        string LogFileDir = Path.Combine(Application.dataPath, "LogABDep").Replace("\\", "/");
        if (!Directory.Exists(LogFileDir))
        {
            Directory.CreateDirectory(LogFileDir);
        }
        AssetDatabase.Refresh();
        System.IO.File.WriteAllText(LogFileDir + "/TotalABDependsAnalyzeDep.txt", str.ToString());
    }

    private static void DebugcombineList()
    {
        System.Text.StringBuilder str = new System.Text.StringBuilder();

 
        foreach (var item in _ABNodeList)
        {
            if (item.Value._combineList.Count >1)
            {
                str.AppendFormat("{0}\n", "以下被依赖的个数相同：");
                foreach (var cell in item.Value._combineList)
                {
                    str.AppendFormat("\t{0}\n", cell);
                }

                ABNode node = _ABNodeList[item.Value._combineList[0]];
                str.AppendFormat("{0}\t个数:{1}\n", "共同被依赖项", node._beDepends.Count);
                foreach (var dep in node._beDepends)
                {
                    str.AppendFormat("\t{0} \n", dep.Key);
                }
                str.AppendLine("------------------------------------------------------------------------------------");
            }
        }

        string LogFileDir = Path.Combine(Application.dataPath, "LogABDep").Replace("\\", "/");
        if (!Directory.Exists(LogFileDir))
        {
            Directory.CreateDirectory(LogFileDir);
        }
        AssetDatabase.Refresh();
        System.IO.File.WriteAllText(LogFileDir + "/SameBeDepends.txt", str.ToString());
    }



}
