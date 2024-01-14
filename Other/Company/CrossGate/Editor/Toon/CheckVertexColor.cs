using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CheckVertexColor
{
    //[MenuItem("__Tool__/CheckMeshVertexColor")]
    public static void CheckMeshVertexColor()
    {
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ResourcesAB/Prefab/Char", "Assets/ResourcesAB/Prefab/Monster", "Assets/ResourcesAB/Prefab/Npc", "Assets/ResourcesAB/Prefab/Weapons" });
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar("AnalysisMaterial", path, (float)i / (float)ids.Length))
            {
                break;
            }

            _CheckMeshVertexColor(path);
        }
        EditorUtility.ClearProgressBar();
    }

    public static void _CheckMeshVertexColor(string path)
    {
        GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        SkinnedMeshRenderer[] renderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            SkinnedMeshRenderer renderer = renderers[i];
            float v = renderer.sharedMesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Color) ? 1 : 0;
            for (int j = 0; j < renderer.sharedMaterials.Length; ++j)
            {
                if (renderer.sharedMaterials[j].HasProperty("_Use_VertexColor"))
                {
                    renderer.sharedMaterials[j].SetFloat("_Use_VertexColor", v);
                    Debug.LogFormat("set {0} _Use_VertexColor = {1}", renderer.sharedMaterials[j].name, v);
                }
            }
        }
    }
}
