using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DissolutionGUI : BaseShaderGUI
{
    private GUIContent maskMap = new GUIContent("Mask Map", "");
    private GUIContent edgeColor = new GUIContent("Edge Color", "");
    private GUIContent mask = new GUIContent("Mask Value", "");
    private GUIContent edge = new GUIContent("Edge Value", "");

    private MaterialProperty _MaskMap;
    private MaterialProperty _EdgeColor;
    private MaterialProperty _N_mask;
    private MaterialProperty _N_edge;


    public override void FindProperties(MaterialProperty[] properties)
    {
        base.FindProperties(properties);

        _MaskMap = FindProperty("_MaskMap", properties);
        _EdgeColor = FindProperty("_EdgeColor", properties);
        _N_mask = FindProperty("_N_mask", properties);
        _N_edge = FindProperty("_N_edge", properties);
    }

    public override void MaterialChanged(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        SetMaterialKeywords(material);
    }

    // material main surface options
    public override void DrawSurfaceOptions(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        // Use default labelWidth
        EditorGUIUtility.labelWidth = 0f;

        // Detect any changes to the material
        EditorGUI.BeginChangeCheck();
        {
            base.DrawSurfaceOptions(material);
        }
        if (EditorGUI.EndChangeCheck())
        {
            foreach (var obj in blendModeProp.targets)
                MaterialChanged((Material)obj);
        }
    }

    // material main surface inputs
    public override void DrawSurfaceInputs(Material material)
    {
        base.DrawSurfaceInputs(material);
        DrawTileOffset(materialEditor, baseMapProp);

        materialEditor.TexturePropertySingleLine(maskMap, _MaskMap);
        DrawTileOffset(materialEditor, _MaskMap);

        materialEditor.ShaderProperty(_EdgeColor, edgeColor);        
        materialEditor.ShaderProperty(_N_edge, edge);
        materialEditor.ShaderProperty(_N_mask, mask);
    }
}
