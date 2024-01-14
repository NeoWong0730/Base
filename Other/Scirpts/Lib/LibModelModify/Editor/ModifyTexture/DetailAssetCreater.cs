using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Unity.Mathematics;

public static class DetailAssetCreater
{
    [MenuItem("Tools/CreateDetailAsset")]
    public static void CreateDetailAsset()
    {
        Shader shader = Shader.Find("Character Customize/Texture Combine/Face");

        string srcPath = "Assets/Art/FaceDetail";
        string dstPath = "Assets/ResourcesAB/FaceDetail";

        string[] srcAssets = AssetDatabase.FindAssets("t:Texture", new string[] { srcPath });
        for (int i = 0; i < srcAssets.Length; ++i)
        {            
            string srcAsset = AssetDatabase.GUIDToAssetPath(srcAssets[i]);

            if(EditorUtility.DisplayCancelableProgressBar("CreateDetailAsset", srcAsset, (float)i / srcAssets.Length))
            {
                break;
            }

            string dstAsset = srcAsset.Replace(srcPath, dstPath);

            string fileName = Path.GetFileNameWithoutExtension(srcAsset);
            string[] ss = fileName.Split('_');
            if (ss.Length < 2)
            {
                Debug.LogError($"name {srcAsset} error");
                continue;
            }

            string id = ss[ss.Length - 1];
            string s = ss[ss.Length - 2];
            if (!int.TryParse(s, out int areaID))
            {
                Debug.LogError($"name {srcAsset} area error");
                continue;
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(srcAsset);
            if(texture == null)
            {
                Debug.LogError($"name {srcAsset} is not Texture2D");
                continue;
            }

            dstAsset = Path.ChangeExtension(dstAsset, "asset");
            DetailAsset detailAsset = AssetDatabase.LoadAssetAtPath<DetailAsset>(dstAsset);
            if(detailAsset == null)
            {
                detailAsset = ScriptableObject.CreateInstance<DetailAsset>();
                Material material = new Material(shader);
                material.name = id.ToString();

                string dir = Application.dataPath + "/../" + Path.GetDirectoryName(dstAsset);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                AssetDatabase.CreateAsset(detailAsset, dstAsset);
                AssetDatabase.AddObjectToAsset(material, detailAsset);
                detailAsset.mMaterial = material;
            }

            detailAsset.AreaID = areaID;
            detailAsset.useColor = true;
            detailAsset.vTextureSize = new float2(texture.width, texture.height);
            detailAsset.mMaterial.SetTexture("_MainTex", texture);

            EditorUtility.SetDirty(detailAsset);
            AssetDatabase.SaveAssetIfDirty(detailAsset);
        }

        EditorUtility.ClearProgressBar();
    }
}
