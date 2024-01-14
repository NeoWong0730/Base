using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ToneBasedShadingGUI;

public static class OutlineGUI 
{
    public static class Styles
    {
        public static readonly GUIContent Title = EditorGUIUtility.TrTextContent("Outline");

        public static readonly GUIContent OutlineOnLabel = EditorGUIUtility.TrTextContent("Outline On");
        public static readonly GUIContent OutlineColorLabel = EditorGUIUtility.TrTextContent("Outline Color");
        public static readonly GUIContent OutlineWidthLabel = EditorGUIUtility.TrTextContent("Outline Width");
        public static readonly GUIContent OffsetZLabel = EditorGUIUtility.TrTextContent("Offset Z ");
    }

    public struct OutlineProperties
    {
        public MaterialProperty outline;
        public MaterialProperty outlineWidth;
        public MaterialProperty outlineColor;
        public MaterialProperty offsetZ;

        public OutlineProperties(MaterialProperty[] properties)
        {
            outline = BaseShaderGUI.FindProperty("_Outline", properties, false);
            outlineColor = BaseShaderGUI.FindProperty("_OutlineColor", properties, false);
            outlineWidth = BaseShaderGUI.FindProperty("_OutlineWidth", properties, false);            
            offsetZ = BaseShaderGUI.FindProperty("_OffsetZ", properties, false);
        }
    }

    public static void Inputs(OutlineProperties properties, MaterialEditor materialEditor, Material material)
    {
        materialEditor.ShaderProperty(properties.outline, Styles.OutlineOnLabel);
        materialEditor.ShaderProperty(properties.outlineColor, Styles.OutlineColorLabel);
        materialEditor.ShaderProperty(properties.outlineWidth, Styles.OutlineWidthLabel);
        materialEditor.ShaderProperty(properties.offsetZ, Styles.OffsetZLabel);

        material.SetShaderPassEnabled("OUTLINE", properties.outline.floatValue == 1.0);
    }
}
