using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework;
using System.IO;

public class ExportHumanPrefab
{
    public static string sHumanFamaleFile = "Assets/Art/Human/Basemodel/famale/famale.prefab";
    public static string sHumanMaleFile = "Assets/Art/Human/Basemodel/male/male.prefab";
    public static string sHumanSkeletonFile = "Assets/Art/Human/BaseModel/Skeleton/skeleton_common.prefab";

    public static string sClothPath = "Assets/Art/Human/Clothes";
    public static string sHairPath = "Assets/Art/Human/Hair";

    public static string sHumanOutputPath = "Assets/ResourcesAB/Prefab/Human";
    public static string sClothOutputPath = "Assets/ResourcesAB/Prefab/Cloth";
    public static string sHairOutputPath = "Assets/ResourcesAB/Prefab/Hair";

    public static void ExportHumanSkeletonPrefab(string inputFile, string outputPath)
    {
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(inputFile);
        GameObject goClone = Object.Instantiate<GameObject>(go);
        goClone.name = go.name;

        PrefabUtility.SaveAsPrefabAsset(goClone, outputPath);

        //TODO 生成插槽信息

        Object.DestroyImmediate(goClone);
    }

    public static void ExportHumanFacePrefab(string inputFile, string outputPath, Dictionary<string, Transform> skeletonBoneMap, Dictionary<string, string> skeletonBonePathMap)
    {
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(inputFile);
        GameObject goClone = Object.Instantiate<GameObject>(go);
        goClone.name = go.name;

        SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        GameObject faceSkin = null;
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            if (skinnedMeshRenderer.gameObject.name.EndsWith("_face", System.StringComparison.OrdinalIgnoreCase))
            {
                faceSkin = Object.Instantiate(skinnedMeshRenderer.gameObject);
                break;
            }
        }

        if(faceSkin != null)
        {
            FaceModify faceModify = faceSkin.GetOrAddComponent<FaceModify>();
            faceModify.mModifyBone = faceSkin.GetOrAddComponent<ModifyBone>();
            faceModify.mModifyMaterial = faceSkin.GetOrAddComponent<ModifyMaterial>();
            faceModify.mModifyTexture = faceSkin.GetOrAddComponent<ModifyTexture>();

            faceModify.mModifyTexture.mTargetMaterial = null;

            ModelPart modelPart = faceSkin.GetOrAddComponent<ModelPart>();
            ConfigModelPart(modelPart, outputPath, 1, skeletonBoneMap, skeletonBonePathMap);

            string dir = Path.GetDirectoryName(outputPath);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            PrefabUtility.SaveAsPrefabAsset(faceSkin, outputPath);

            Object.DestroyImmediate(faceSkin);
        }

        Object.DestroyImmediate(goClone);
    }
   
    public static void ExportHumanClothPrefab(string inputFile, string outputPath, Dictionary<string, Transform> skeletonBoneMap, Dictionary<string, string> skeletonBonePathMap)
    {
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(inputFile);
        GameObject goClone = Object.Instantiate<GameObject>(go);
        goClone.name = go.name;

        SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        GameObject clothSkin = null;
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            if (skinnedMeshRenderer.gameObject.name.StartsWith("cloth", System.StringComparison.OrdinalIgnoreCase)
                || skinnedMeshRenderer.gameObject.name.EndsWith("fcloth", System.StringComparison.OrdinalIgnoreCase))
            {
                clothSkin = Object.Instantiate(skinnedMeshRenderer.gameObject);
                break;
            }
        }

        if (clothSkin != null)
        {
            ModelPart modelPart = clothSkin.GetOrAddComponent<ModelPart>();
            ConfigModelPart(modelPart, outputPath, 0, skeletonBoneMap, skeletonBonePathMap);

            string dir = Path.GetDirectoryName(outputPath);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            PrefabUtility.SaveAsPrefabAsset(clothSkin, outputPath);

            Object.DestroyImmediate(clothSkin);
        }

        Object.DestroyImmediate(goClone);
    }

    public static void ExportHumanHairPrefab(string inputFile, string outputPath, Dictionary<string, Transform> skeletonBoneMap, Dictionary<string, string> skeletonBonePathMap)
    {
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(inputFile);
        GameObject goClone = Object.Instantiate<GameObject>(go);
        goClone.name = go.name;

        SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        GameObject clothSkin = null;
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            if (skinnedMeshRenderer.gameObject.name.StartsWith("hair", System.StringComparison.OrdinalIgnoreCase)
                || skinnedMeshRenderer.gameObject.name.StartsWith("fhair", System.StringComparison.OrdinalIgnoreCase))
            {
                clothSkin = Object.Instantiate(skinnedMeshRenderer.gameObject);
                break;
            }
        }

        if (clothSkin != null)
        {
            ModelPart modelPart = clothSkin.GetOrAddComponent<ModelPart>();            
            ConfigModelPart(modelPart, outputPath, 0, skeletonBoneMap, skeletonBonePathMap);

            string dir = Path.GetDirectoryName(outputPath);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            PrefabUtility.SaveAsPrefabAsset(clothSkin, outputPath);

            Object.DestroyImmediate(clothSkin);
        }

        Object.DestroyImmediate(goClone);
    }

    public static string GetBonePath(Transform root, Transform bone)
    {
        string path = bone.name;
        while (bone.parent != null)
        {
            bone = bone.parent;
            if (bone == root)
                break;
            path = path.Insert(0, $"{bone.name}/");
        }
        return path;
    }

    private static Transform FindAdditionRoot(Transform node, Dictionary<string, Transform> skeletonBoneMap)
    {
        Transform parent = node.parent;
        if (!parent)
            return node;

        if (!skeletonBoneMap.ContainsKey(parent.name))
        {
            return FindAdditionRoot(parent, skeletonBoneMap);
        }
        return node;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="skinType">0 = Cloth 1 = face 2 = hair</param>
    public static void ConfigModelPart(ModelPart modelPart, string path, int skinType, Dictionary<string, Transform> skeletonBoneMap, Dictionary<string, string> skeletonBonePathMap)
    {
        string skinConfigFile = System.IO.Path.ChangeExtension(path, ".asset");
        ModelPartConfig skinConfig = AssetDatabase.LoadAssetAtPath<ModelPartConfig>(skinConfigFile);
        if (!skinConfig)
        {
            skinConfig = ScriptableObject.CreateInstance<ModelPartConfig>();
            AssetDatabase.CreateAsset(skinConfig, skinConfigFile);
        }

        SkinnedMeshRenderer skinnedMeshRenderer = modelPart.GetComponent<SkinnedMeshRenderer>();
        Transform rootBone = skinnedMeshRenderer.rootBone;
        Transform[] bones = skinnedMeshRenderer.bones;

        Dictionary<string, Transform> additionRootClones = new Dictionary<string, Transform>();
        Dictionary<string, Transform> additionBones = new Dictionary<string, Transform>();
        string[] bonePaths = new string[bones.Length];

        for (int i = 0; i < bones.Length; ++i)
        {
            string boneName = bones[i].name;

            if (skeletonBoneMap.ContainsKey(boneName))
            {
                bonePaths[i] = skeletonBonePathMap[boneName];
                bones[i] = null;
                continue;
            }

            if (additionBones.TryGetValue(boneName, out Transform bone))
            {
                bonePaths[i] = null;
                bones[i] = bone;
                continue;
            }

            Transform additionRoot = FindAdditionRoot(bones[i], skeletonBoneMap);
            string additionRootParentName = additionRoot.parent.name;

            if (!additionRootClones.TryGetValue(additionRootParentName, out Transform additionRootParentClone))
            {
                additionRootParentClone = (new GameObject(additionRootParentName)).transform;
                additionRootParentClone.SetParent(modelPart.transform);

                additionRootClones.Add(additionRootParentName, additionRootParentClone);
            }

            GameObject additionRootClone = GameObject.Instantiate(additionRoot.gameObject, additionRootParentClone, false);
            additionRootClone.name = additionRoot.gameObject.name;

            Transform[] newAdditionBones = additionRootClone.GetComponentsInChildren<Transform>();
            foreach (var newAdditionBone in newAdditionBones)
            {
                additionBones.TryAdd(newAdditionBone.name, newAdditionBone);
            }

            if (additionBones.TryGetValue(boneName, out Transform additionBone))
            {
                bones[i] = additionBone;
            }
            else
            {
                Debug.LogError($"查找附加骨骼 {boneName} 失败");
            }
        }

        if (!additionBones.TryGetValue(rootBone.name, out Transform selfRootBone))
        {
            skinConfig.mRootBoneName = skeletonBonePathMap[rootBone.name];
            skinnedMeshRenderer.rootBone = null;
        }
        else
        {
            skinConfig.mRootBoneName = null;
            skinnedMeshRenderer.rootBone = selfRootBone;            
        }

        skinConfig.mBoneNames = bonePaths;
        skinnedMeshRenderer.bones = bones;

        EditorUtility.SetDirty(skinConfig);
        AssetDatabase.SaveAssetIfDirty(skinConfig);

        modelPart.mSkinConfig = skinConfig;

        string[] boneRootNames = new string[additionRootClones.Count];
        Transform[] boneRoots = new Transform[additionRootClones.Count];

        additionRootClones.Values.CopyTo(boneRoots, 0);
        for (int i = 0; i < boneRootNames.Length; ++i)
        {
            boneRootNames[i] = skeletonBonePathMap[boneRoots[i].name];
        }

        List<AnimancerAdditionPlayable> animancerAdditionPlayables = new List<AnimancerAdditionPlayable>(4);

        switch (skinType)
        {
            case 0:
                SetSyncBones(modelPart, animancerAdditionPlayables, boneRoots, boneRootNames);
                //SetBoneDampingSolo(modelPart, animancerAdditionPlayables, boneRoots, boneRootNames);
                SetBoneDampingMulti(modelPart, animancerAdditionPlayables, boneRoots, boneRootNames);
                break;
            case 1:
                SetSyncBones(modelPart, animancerAdditionPlayables, boneRoots, boneRootNames);
                break;
            default:
                break;
        }

        modelPart.mAnimancerAdditionPlayables = animancerAdditionPlayables.ToArray();
    }

    private static void CreateBoneDampingSolo(ModelPart modelPart, Transform root, Transform node, int boneDepth, List<AnimancerAdditionPlayable> animancerAdditionPlayables, string boneRootName)
    {
        int count = node.childCount;
        if (count <= 0)
        {
            if (boneDepth > 1)
            {
                Framework.BoneDampingSolo damping = modelPart.gameObject.AddComponent<Framework.BoneDampingSolo>();
                damping._EndBone = node;
                damping._BoneCount = boneDepth;
                damping._RootBone = root;
                //damping._RootBonePath = boneRootName;

                animancerAdditionPlayables.Add(damping);
            }

            return;
        }

        ++boneDepth;

        for (int i = 0; i < count; ++i)
        {
            CreateBoneDampingSolo(modelPart, root, node.GetChild(i), boneDepth, animancerAdditionPlayables, boneRootName);
        }
    }

    private static void SetBoneDampingSolo(ModelPart modelPart, List<AnimancerAdditionPlayable> animancerAdditionPlayables, Transform[] boneRoots, string[] boneRootNames)
    {
        if (boneRoots == null || boneRoots.Length == 0)
            return;


        for (int i = 0; i < boneRoots.Length; ++i)
        {
            CreateBoneDampingSolo(modelPart, boneRoots[i], boneRoots[i], 1, animancerAdditionPlayables, boneRootNames[i]);
        }
    }

    private static void SetBoneDampingMulti(ModelPart modelPart, List<AnimancerAdditionPlayable> animancerAdditionPlayables, Transform[] boneRoots, string[] boneRootNames)
    {
        if (boneRoots == null || boneRoots.Length == 0)
            return;

        BoneDampingMulti bonesDampingMulti = modelPart.gameObject.AddComponent<BoneDampingMulti>();
        animancerAdditionPlayables.Add(bonesDampingMulti);

        bonesDampingMulti._RootBones = boneRoots;
    }

    private static void SetSyncBones(ModelPart modelPart, List<AnimancerAdditionPlayable> animancerAdditionPlayables, Transform[] boneRoots, string[] boneRootNames)
    {
        if (boneRoots == null || boneRoots.Length == 0)
            return;


        BoneSync syncBones = modelPart.gameObject.AddComponent<BoneSync>();
        animancerAdditionPlayables.Add(syncBones);

        syncBones.mRootBones = boneRoots;
        syncBones.mRootBonePaths = boneRootNames;
    }
}
