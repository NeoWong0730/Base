using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AssetImporters;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEditor.AssetImporter;

public class MyAssetProcessor : AssetPostprocessor
{
    private static string sDefaultMat = "Lit";
  
    private static string sSpriteDir = "Assets/Projects/Image/";
    private static string sAtlasSpriteGroup = "AtlasSprite";

    private static string sDefaultMaterialPath = "Assets/ResourcesAB/Material/Common/MDefault.mat";

    public static bool stopTextureImporter = false;

    private static string[] extraExposedTransformPaths = new string[] {
        "Bip01LHand_01",
        "Bip01RHand_01",
        "Bip01/Bip01 Pelvis/Bip01 Spine/Waistdress_01",
        "Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Wing_01",
        "Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Facedress_01",
        "Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Headdress_01",

        "Bip001/Bip001 Pelvis/Bip001 Spine/Waistdress_01",
        "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Wing_01",
        "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/Facedress_01",
        "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/Headdress_01" };

    private static string sCharactorModelPath = "Assets/Arts/Charactor";
    private static string sFxModelPath = "Assets/Arts/Fx";
    private static string sSceneModelPath = "Assets/Arts/Scene";

    private static string sCharModelPath = "Assets/Arts/Charactor/Char";
    private static string sResourceABDir = "Assets/ResourcesAB/";
    public override int GetPostprocessOrder()
    {
        return 2;
    }

    static void OnPostprocessAllAssets(string[] imported, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (int i = 0; i < imported.Length; ++i)
        {
            AbbressableAssetEntrySetRule.SetAddressableAssetEntryName(imported[i]);
        }

        for (int i = 0; i < movedAssets.Length; ++i)
        {
            AbbressableAssetEntrySetRule.SetAddressableAssetEntryName(imported[i]);
        }
    }



    //private Material OnAssignMaterialModel(Material material, Renderer renderer)
    //{    
    //}
    //private void OnPreprocessMaterialDescription(MaterialDescription description, Material material, AnimationClip[] clips)
    //{
    //    Debug.LogErrorFormat("{0} 未指定材质, 将自动指定默认材质", assetPath);
    //    Shader shader = Shader.Find("Hidden/InternalErrorShader");
    //    material.shader = shader;
    //    //Material errorMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Arts/ErrorMaterial.mat");
    //    //if (errorMaterial)
    //    //{
    //    //    SourceAssetIdentifier identifier = new SourceAssetIdentifier(typeof(Material), description.materialName);
    //    //    ModelImporter modelImporter = assetImporter as ModelImporter;
    //    //    modelImporter.AddRemap(identifier, errorMaterial);            
    //    //}
    //    //else
    //    //{
    //    //    Debug.LogErrorFormat("{0} 默认材质无法加载", assetImporter.name);
    //    //}
    //}

    private void OnPostprocessModel(GameObject model)
    {
        if (model != null)
        {
            if (assetPath.StartsWith(sCharModelPath, System.StringComparison.Ordinal))
            {
                //if (!model.TryGetComponent<Framework.SimpleAnimation>(out Framework.SimpleAnimation simpleAnimation))
                //{
                //    simpleAnimation = model.AddComponent<Framework.SimpleAnimation>();
                //}
            }
        }
    }
    //private void OnPostprocessAnimation(GameObject go, AnimationClip clip)
    //{
    //    OptimalAnimationClip(clip);
    //}
    private void OnPreprocessTexture()
    {
        if (stopTextureImporter)
            return;

        TextureImporter importer = assetImporter as TextureImporter;
        // TextureTool.SetTextureImporter(importer, context.assetPath);
    }

    //private void OnPreprocessModel()
    //{
    //    ModelImporter importer = assetImporter as ModelImporter;
    //    SetModelImporter(importer, context.assetPath);
    //}


    [MenuItem("__Tools__/设置ResourceAB资源可寻址")]
    public static void ReportAssetsProcessor()
    {
        if (Application.isPlaying || EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            EditorUtility.DisplayDialog("错误", "游戏正在运行或者暂停，请不要操作！", "确定");
        }

        if (EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("错误", "游戏脚本正在编译，请不要操作！", "确定");
            return;
        }

        if (EditorUtility.DisplayDialog("提示", "该过程大概需要1-2分钟,继续请按确定", "确定", "取消"))
        {
            AddressableBundleGroupSetting.GenAddressableGroup_New();
            EditorUtility.DisplayDialog("生成完成", "AddressableAssetGroup 已重新生成", "确定");
        }
    }
    /*
    [MenuItem("__Tools__/ClearFBX")]
    public static void ReimportAllFBX()
    {
        int rlt = EditorUtility.DisplayDialogComplex("设置模型导入", "该过程需要较长时间", "设置", "取消", "取消");
        if (rlt != 0)
            return;

        string[] ids = AssetDatabase.FindAssets("t:Model");
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar("设置模型导入", string.Format("({0}/{1}){2}", i.ToString(), ids.Length.ToString(), path), (float)i / (float)ids.Length))
            {
                break;
            }

            ModelImporter importer = ModelImporter.GetAtPath(path) as ModelImporter;

            if (importer == null)
            {
                Debug.LogErrorFormat("{0} 获取ModelImporter失败", path);
                return;
            }

            SetModelImporter(importer, path);
            AssetDatabase.WriteImportSettingsIfDirty(path);
        }

        EditorUtility.ClearProgressBar();
        System.GC.Collect();

        AssetDatabase.Refresh();

        System.GC.Collect();
    }
    */
    public static void SetModelImporter(ModelImporter importer, string path)
    {
        importer.meshOptimizationFlags = MeshOptimizationFlags.Everything;

        importer.isReadable = false;

        importer.importBlendShapes = false;
        importer.importBlendShapeNormals = ModelImporterNormals.None;
        importer.importCameras = false;
        importer.importLights = false;
        importer.importConstraints = false;

        importer.materialImportMode = ModelImporterMaterialImportMode.None;

        if (path.StartsWith(sCharactorModelPath, System.StringComparison.Ordinal))
        {
            SetCharactorModelImporter(importer, path);
        }
        else if (path.StartsWith(sSceneModelPath, System.StringComparison.Ordinal))
        {
            SetSceneModelImporter(importer, path);
        }
        else if (path.StartsWith(sFxModelPath, System.StringComparison.Ordinal))
        {
            SetFxModelImporter(importer, path);
        }
        else
        {
            SetOtherModelImporter(importer, path);
        }
    }

    private static void SetCharactorModelImporter(ModelImporter importer, string path)
    {
        string filename = Path.GetFileNameWithoutExtension(path);

        bool isMesh = filename.LastIndexOf("_mesh", StringComparison.OrdinalIgnoreCase) >= 0;
        bool isHighModel = filename.IndexOf("_show_", StringComparison.OrdinalIgnoreCase) >= 0 || filename.StartsWith("show_", System.StringComparison.OrdinalIgnoreCase);
        bool isChar = path.StartsWith(sCharModelPath, System.StringComparison.Ordinal);

        if (isMesh)
        {
            importer.importAnimation = false;
            if (importer.animationType == ModelImporterAnimationType.Generic)
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                if (isHighModel && isChar)
                {
                    //importer.optimizeGameObjects = false;
                }
                else
                {
                    //importer.optimizeGameObjects = true;
                    if (isChar && importer.optimizeGameObjects)
                    {
                        if (importer.extraExposedTransformPaths.Length == importer.transformPaths.Length)
                        {
                            //暴露挂点
                            importer.extraExposedTransformPaths = extraExposedTransformPaths;
                        }
                    }
                }
            }

            importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            importer.materialSearch = ModelImporterMaterialSearch.Local;
            importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            importer.materialName = ModelImporterMaterialName.BasedOnMaterialName;
            importer.materialSearch = ModelImporterMaterialSearch.Local;
        }
        else
        {
            //importer.importVisibility = false;

            if (isHighModel || isChar)
            {
                importer.animationCompression = ModelImporterAnimationCompression.KeyframeReduction;
            }
            else
            {
                importer.animationCompression = ModelImporterAnimationCompression.Optimal;
                importer.animationRotationError = 1;
                importer.animationPositionError = 1;
                importer.animationScaleError = 1;
            }
        }

        if (!isHighModel)
        {
            importer.importTangents = ModelImporterTangents.None;
        }
        else
        {
            importer.importTangents = ModelImporterTangents.CalculateMikk;
        }
    }

    private static void SetSceneModelImporter(ModelImporter importer, string path)
    {
    }

    private static void SetFxModelImporter(ModelImporter importer, string path)
    {
    }

    private static void SetOtherModelImporter(ModelImporter importer, string path)
    {
        importer.importTangents = ModelImporterTangents.None;
        importer.animationCompression = ModelImporterAnimationCompression.Optimal;
        importer.animationRotationError = 1;
        importer.animationPositionError = 1;
        importer.animationScaleError = 1;
    }

    //[MenuItem("Assets/AnimationClip/修改Animation压缩为Optimal")]
    //static void ModifyModel()
    //{
    //    foreach (UnityEngine.Object o in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets))
    //    {
    //        if (!(o is GameObject))
    //            continue;
    //        GameObject mod = o as GameObject;
    //        string path = AssetDatabase.GetAssetPath(mod);
    //        ModelImporter modelimporter = ModelImporter.GetAtPath(path) as ModelImporter;
    //        SetModelImporter(modelimporter, path);
    //    }
    //    AssetDatabase.Refresh();
    //}

    static string _Name_Scale = "m_LocalScale";
    public static void OptimalAnimationClip(AnimationClip animationClip)
    {
        //Debug.LogFormat("优化{0}的缩放曲线", animationClip.name);

        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(animationClip);
        for (int i = 0; i < curveBindings.Length; ++i)
        {
            EditorCurveBinding curveBinding = curveBindings[i];
            AnimationCurve curveData = AnimationUtility.GetEditorCurve(animationClip, curveBinding);

            // Debug.LogFormat("曲线{1}", animationClip.name, curveBinding.propertyName);

            if (curveBinding.propertyName.StartsWith(_Name_Scale, StringComparison.Ordinal))
            {
                bool vaild = false;
                for (int j = 0; j < curveData.length; ++j)
                {
                    Keyframe keyframe = curveData[j];
                    if (keyframe.value >= 0.99999 && keyframe.value <= 1.00001f)
                    {
                    }
                    else
                    {
                        vaild = true;
                        break;
                    }
                }

                if (!vaild)
                {
                    //remove
                    AnimationUtility.SetEditorCurve(animationClip, curveBinding, null);
                    Debug.LogFormat("删除了{0}的曲线{1} {2}", animationClip.name, i.ToString(), curveBinding.propertyName);
                }
            }

            //处理过后必须要设置一次SetCurve 否则文件会变大（随便设置一次就行）
            AnimationClipCurveData[] tCurveArr = AnimationUtility.GetAllCurves(animationClip);
            if (tCurveArr.Length > 0)
            {
                AnimationClipCurveData tCurveData = tCurveArr[0];
                animationClip.SetCurve(tCurveData.path, tCurveData.type, tCurveData.propertyName, tCurveData.curve);
            }
        }
    }

    //[MenuItem("Assets/AnimationClip/优化")]
    //static void OptimalOneAnimationClip()
    //{
    //    var arr = Selection.assetGUIDs;
    //    for(int i = 0; i < arr.Length; ++i)
    //    {
    //        string assetPath = AssetDatabase.GUIDToAssetPath(arr[i]);
    //        if (EditorUtility.DisplayCancelableProgressBar("AnimationClip优化", string.Format("({0}/{1}){2}", i.ToString(), arr.Length.ToString(), assetPath), (float)i / (float)arr.Length))
    //        {
    //            break;
    //        }
    //
    //        AnimationClip asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
    //        if (asset == null)
    //            continue;
    //
    //        OptimalAnimationClip(asset);
    //        EditorUtility.SetDirty(asset);
    //    }
    //    AssetDatabase.SaveAssets();
    //    EditorUtility.ClearProgressBar();
    //}
}