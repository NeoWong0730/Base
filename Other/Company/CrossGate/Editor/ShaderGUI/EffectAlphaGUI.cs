using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class EffectAlphaGUI : ShaderCommonGUI
{
    public override void MaterialChanged(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        SetMaterialKeywords(material);

        if (material.HasProperty("_Mask"))
            CoreUtils.SetKeyword(material, "_MASKMAP_ON", material.GetTexture("_Mask"));
    }

    public static void SetMaterialKeywords_AddtiveUVAnim(Material material)
    {
        material.shaderKeywords = null;
        //material.enableInstancing = false;

        if (material.HasProperty("_Mask"))
            CoreUtils.SetKeyword(material, "_MASKMAP_ON", material.GetTexture("_Mask"));
    }
}