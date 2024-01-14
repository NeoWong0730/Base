using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Rendering.ToneBasedShading.ShaderGUI.StencilGUI;

public static class RimGUI
{
    public static class Styles
    {
        public static readonly GUIContent Title = EditorGUIUtility.TrTextContent("Rim");

        public static readonly GUIContent RimOnLabel = EditorGUIUtility.TrTextContent("Rim On");
        public static readonly GUIContent RimColorLabel = EditorGUIUtility.TrTextContent("Rim Color");
        public static readonly GUIContent RimPowerLabel = EditorGUIUtility.TrTextContent("Rim Power");
    }

    public struct RimProperties
    {
        public MaterialProperty rim;
        public MaterialProperty rimColor;
        public MaterialProperty rimPower;

        public RimProperties(MaterialProperty[] properties)
        {
            rim = BaseShaderGUI.FindProperty("_Rim", properties, false);
            rimColor = BaseShaderGUI.FindProperty("_RimColor", properties, false);
            rimPower = BaseShaderGUI.FindProperty("_RimPower", properties, false);
        }
    }

    public static void Inputs(RimProperties properties, MaterialEditor materialEditor, Material material)
    {
        materialEditor.ShaderProperty(properties.rim, Styles.RimOnLabel);
        materialEditor.ShaderProperty(properties.rimColor, Styles.RimColorLabel);
        materialEditor.ShaderProperty(properties.rimPower, Styles.RimPowerLabel);
    }
}
