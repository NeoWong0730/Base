using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class UVAnimGUI : ShaderCommonGUI
{
    public override void MaterialChanged(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        SetMaterialKeywords_AddtiveUVAnim(material);
    }

    public static void SetMaterialKeywords_AddtiveUVAnim(Material material)
    {
        material.shaderKeywords = null;

        SetMaterialKeywords(material);

        material.enableInstancing = false;

        if (material.HasProperty("_DISSOLUTION"))
        {
            CoreUtils.SetKeyword(material, "_DISSOLUTION_ON", material.GetFloat("_DISSOLUTION") == 1.0 ? true : false);
        }
    }
}