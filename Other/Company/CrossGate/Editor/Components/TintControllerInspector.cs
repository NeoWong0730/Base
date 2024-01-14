using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(TintController))]
public class TintControllerInspector : Editor
{
    [MenuItem("__Tools__/批量自动添加染色脚本")]
    public static void AutoTintController()
    {
        string[] assetsGUIDs = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/ResourcesAB/Prefab/Char", "Assets/ResourcesAB/Prefab/Weapons" });
        _DoAutoTintController(assetsGUIDs, false);
    }

    [MenuItem("Assets/自动添加染色脚本")]
    public static void AutoTintControllerOne()
    {
        if (Selection.assetGUIDs == null)
            return;

        _DoAutoTintController(Selection.assetGUIDs, false);
    }

    private static void _DoAutoTintController(string[] assetsGUIDs, bool forceClear)
    {
        string title = "添加TintController";

        string highModelFlag = "_show.prefab";
        string highModelFlag2 = "_show_";

        string kClothesMesh = "show_clothes_mesh";
        string kHairMesh = "show_hair_mesh";

        string charDir = "Assets/ResourcesAB/Prefab/Char";
        //string weaponsDir = "Assets/ResourcesAB/Prefab/Weapons";

        HashSet<Renderer> needTintRenderers = new HashSet<Renderer>();
        
        for (int i = 0; i < assetsGUIDs.Length; ++i)
        {
            string assetsPath = AssetDatabase.GUIDToAssetPath(assetsGUIDs[i]);
            if (EditorUtility.DisplayCancelableProgressBar(title, assetsPath, (float)i / assetsGUIDs.Length))
                break;

            GameObject go = PrefabUtility.LoadPrefabContents(assetsPath);

            TintController tintController = null;

            //已有脚本的检测一次
            if (!forceClear
                && go.TryGetComponent<TintController>(out tintController)
                && tintController.mRenderers != null
                && tintController.mRenderers.Length > 0)
            {                
                for (int j = 0; j < tintController.mRenderers.Length; ++j)
                {
                    if (tintController.mRenderers[j] != null && !needTintRenderers.Contains(tintController.mRenderers[j]))
                    {
                        needTintRenderers.Add(tintController.mRenderers[j]);
                    }
                }

                if (needTintRenderers.Count > 0)
                {
                    if (tintController.mRenderers.Length != needTintRenderers.Count)
                    {
                        tintController.mRenderers = new Renderer[needTintRenderers.Count];
                        needTintRenderers.CopyTo(tintController.mRenderers, 0, needTintRenderers.Count);
                        PrefabUtility.SaveAsPrefabAsset(go, assetsPath);
                    }

                    needTintRenderers.Clear();
                    PrefabUtility.UnloadPrefabContents(go);
                    continue;
                }
            }


            bool isHighModel = assetsPath.EndsWith(highModelFlag, System.StringComparison.OrdinalIgnoreCase) || assetsPath.IndexOf(highModelFlag2, System.StringComparison.OrdinalIgnoreCase) >= 0;
            bool isChar = assetsPath.StartsWith(charDir, System.StringComparison.Ordinal);

            bool hasHair = false;
            bool hasCloth = false;

            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

            if (isHighModel && isChar)
            {
                for (int j = 0; j < renderers.Length; ++j)
                {
                    Renderer renderer = renderers[j];

                    if (renderer.gameObject.name.EndsWith(kClothesMesh, System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.Equals(renderer.sharedMaterial.shader.name, "Toon/ToonLit_OutLine", System.StringComparison.Ordinal))
                        {
                            if (renderer.sharedMaterial.IsKeywordEnabled("_TINTMASK_ON"))
                            {
                                hasCloth = true;
                                needTintRenderers.Add(renderer);
                            }
                        }
                    }
                    else if (renderer.gameObject.name.EndsWith(kHairMesh, System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.Equals(renderer.sharedMaterial.shader.name, "Toon/ToonHair_OutLine", System.StringComparison.Ordinal))
                        {
                            hasHair = true;
                            needTintRenderers.Add(renderer);
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < renderers.Length; ++j)
                {
                    Renderer renderer = renderers[j];
                    if (renderer is ParticleSystemRenderer)
                        continue;

                    if (string.Equals(renderer.sharedMaterial.shader.name, "Toon/ToonLit_OutLine"))
                    {
                        if (renderer.sharedMaterial.IsKeywordEnabled("_TINTMASK_ON"))
                        {
                            needTintRenderers.Add(renderer);
                        }
                    }
                }
            }

            if (needTintRenderers.Count > 0)
            {
                if (!go.TryGetComponent<TintController>(out tintController))
                {
                    tintController = go.AddComponent<TintController>();
                }
                tintController.mRenderers = new Renderer[needTintRenderers.Count];
                needTintRenderers.CopyTo(tintController.mRenderers, 0, needTintRenderers.Count);

                if(isHighModel && isChar)
                {
                    if (!hasHair)
                    {
                        Debug.LogErrorFormat("高模 没有找到头发 {0}", assetsPath);
                    }
                    if (!hasCloth)
                    {
                        Debug.LogErrorFormat("高模 没有找到衣服 {0}", assetsPath);
                    }
                }                
            }
            else
            {
                if (go.TryGetComponent<TintController>(out tintController))
                {
                    DestroyImmediate(tintController);
                }

                Debug.LogErrorFormat("没有找到可以染色的部位 {0}", assetsPath);
            }

            needTintRenderers.Clear();

            PrefabUtility.SaveAsPrefabAsset(go, assetsPath);
            PrefabUtility.UnloadPrefabContents(go);
        }
        
        EditorUtility.ClearProgressBar();
    }


    private readonly int _ColorR = Shader.PropertyToID("_ColorR");
    private readonly int _ColorG = Shader.PropertyToID("_ColorG");
    private readonly int _ColorB = Shader.PropertyToID("_ColorB");
    private readonly int _ColorA = Shader.PropertyToID("_ColorA");

    private readonly int _UseTintColor = Shader.PropertyToID("_UseTintColor");

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool hasChange = false;

        TintController tintController = target as TintController;
        MaterialPropertyBlock materialPropertyBlock = tintController.Get();

        Color colorR = materialPropertyBlock == null ? Color.clear : materialPropertyBlock.GetColor(_ColorR);
        Color colorG = materialPropertyBlock == null ? Color.clear : materialPropertyBlock.GetColor(_ColorG);
        Color colorB = materialPropertyBlock == null ? Color.clear : materialPropertyBlock.GetColor(_ColorB);
        Color colorA = materialPropertyBlock == null ? Color.clear : materialPropertyBlock.GetColor(_ColorA);

        Color newColorR = EditorGUILayout.ColorField("colorR", colorR);
        if (newColorR != colorR)
        {
            materialPropertyBlock = tintController.GetOrCreate();
            materialPropertyBlock.SetColor(_ColorR, newColorR);
            hasChange = true;
        }

        Color newColorG = EditorGUILayout.ColorField("colorG", colorG);
        if (newColorG != colorG)
        {
            materialPropertyBlock = tintController.GetOrCreate();
            materialPropertyBlock.SetColor(_ColorG, newColorG);
            hasChange = true;
        }

        Color newColorB = EditorGUILayout.ColorField("colorB", colorB);
        if (newColorB != colorB)
        {
            materialPropertyBlock = tintController.GetOrCreate();
            materialPropertyBlock.SetColor(_ColorB, newColorB);
            hasChange = true;
        }

        Color newColorA = EditorGUILayout.ColorField("colorA", colorA);
        if (newColorA != colorA)
        {
            materialPropertyBlock = tintController.GetOrCreate();
            materialPropertyBlock.SetColor(_ColorA, newColorA);
            materialPropertyBlock.SetFloat(_UseTintColor, 1);
            hasChange = true;
        }

        if(hasChange)
        {
            tintController.Apply();
        }        
    }
}
