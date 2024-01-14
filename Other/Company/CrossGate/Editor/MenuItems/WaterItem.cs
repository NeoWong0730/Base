using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using System.IO;

public class WaterItem
{
    [MenuItem("GameObject/Water/Water01")]
    public static void CreateWater01()
    {
        GameObject go = new GameObject("Water01", typeof(PlanarReflections));
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        GameObjectUtility.SetParentAndAlign(plane, go);
        plane.transform.localPosition = Vector3.zero;
        plane.transform.localRotation = Quaternion.identity;
        plane.transform.localScale = Vector3.one;

        MeshRenderer meshRenderer = plane.GetComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("FX/WaterA"));
        Texture texture = AssetDatabase.LoadAssetAtPath<Texture>("Assets/ResourcesAB/Texture/Water/WaterFresnel.psd");
        material.SetTexture("_Fresnel", texture);
        material.EnableKeyword("_REFRACTIVE_ON");
        material.EnableKeyword("_REFLECTIVE_ON");

        string matPath = Application.dataPath + "/ResourcesAB/Material/";
        string matNameFormat = "waterA{0}.mat";

        int i = 0;
        string matFinalName = "waterA.mat";
        string matFinalFullPath = matPath + matFinalName;
        while (File.Exists(matFinalFullPath))
        {
            ++i;            
            if(i > 20)
            {
                Debug.LogErrorFormat("请腾个名字出来");
                break;
            }
            matFinalName = string.Format(matNameFormat, i.ToString("D2"));
            matFinalFullPath = matPath + matFinalName;
        }

        if(i < 20)
        {
            AssetDatabase.CreateAsset(material, "Assets/ResourcesAB/Material/" + matFinalName);
        }
        

        meshRenderer.material = material;

        PlanarReflections planarReflections = go.GetComponent<PlanarReflections>();
        planarReflections.target = plane;
        planarReflections.m_settings.m_ResolutionMultiplier = PlanarReflections.ResolutionMulltiplier.Half;
    }
}
