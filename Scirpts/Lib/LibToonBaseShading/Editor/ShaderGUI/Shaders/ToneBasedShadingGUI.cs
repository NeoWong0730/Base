using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEditor.Rendering.ToneBasedShading.ShaderGUI.StencilGUI;

public static class ToneBasedShadingGUI
{
    public enum WorkflowMode
    {
        SDFFacialShadow = 0,
        Metallic = 1,
    }

    public static class Styles
    {
        public static readonly GUIContent Title = EditorGUIUtility.TrTextContent("Tone Based Shading");

        public static readonly GUIContent ColorStepLabel = EditorGUIUtility.TrTextContent("Color Step");
        public static readonly GUIContent ColorBiasLabel = EditorGUIUtility.TrTextContent("Color Bias");
        public static readonly GUIContent SpecularStepLabel = EditorGUIUtility.TrTextContent("Specular Step");

        public static readonly GUIContent StepLabel = EditorGUIUtility.TrTextContent("漫反射色阶");
        public static readonly GUIContent FeatherLabel = EditorGUIUtility.TrTextContent("色阶过渡");
        public static readonly GUIContent HBiasLabel = EditorGUIUtility.TrTextContent("色相(H)偏移");
        public static readonly GUIContent SBiasLabel = EditorGUIUtility.TrTextContent("饱和度(S)偏移");
        public static readonly GUIContent VBiasLabel = EditorGUIUtility.TrTextContent("亮度(V)偏移");

        public static readonly GUIContent DarkLabel = EditorGUIUtility.TrTextContent("暗部");
        public static readonly GUIContent ShadowLabel = EditorGUIUtility.TrTextContent("阴影");
        public static readonly GUIContent SpecularLabel = EditorGUIUtility.TrTextContent("高光色阶");

        public static readonly GUIContent DefaultValueLabel = EditorGUIUtility.TrTextContent("默认值");

        public static GUIContent SDFMapText =
    EditorGUIUtility.TrTextContent("SDF Map", "Sets and configures the map for the SDFFacialShadow workflow.");
    }

    public struct TBSProperties
    {
        public MaterialProperty colorStep;
        public MaterialProperty colorBias;
        public MaterialProperty colorBias1;
        public MaterialProperty specularStep;
        public MaterialProperty metallicGlossMap;

        public TBSProperties(MaterialProperty[] properties)
        {
            colorStep = BaseShaderGUI.FindProperty("_ColorStep", properties, false);
            colorBias = BaseShaderGUI.FindProperty("_ColorBias", properties, false);
            colorBias1 = BaseShaderGUI.FindProperty("_ColorBias1", properties, false);
            specularStep = BaseShaderGUI.FindProperty("_SpecularStep", properties, false);
            metallicGlossMap = BaseShaderGUI.FindProperty("_MetallicGlossMap", properties);
        }
    }

    public static readonly string sWorkflowMode = "_WorkflowMode";
    public static readonly string sSpecularGlossMap = "_MetallicGlossMap";

    public static void ColorInputs(TBSProperties properties, MaterialEditor materialEditor, Material material)
    {
        Vector4 colorStepValue = properties.colorStep.vectorValue;
        Vector4 colorBiasValue = properties.colorBias.vectorValue;
        Vector4 colorBiasValue1 = properties.colorBias1.vectorValue;
        Vector4 specularStepValue = properties.specularStep.vectorValue;

        if (colorStepValue.z > colorStepValue.x)
            colorStepValue.z = colorStepValue.x;

        EditorGUILayout.LabelField(Styles.StepLabel);       
        colorStepValue.x = EditorGUILayout.Slider(Styles.DarkLabel, colorStepValue.x, 0, 1);
        colorStepValue.z = EditorGUILayout.Slider(Styles.ShadowLabel, colorStepValue.z, 0, 1);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(Styles.FeatherLabel);
        colorStepValue.y = EditorGUILayout.Slider(Styles.DarkLabel, colorStepValue.y, 0, 1);
        colorStepValue.w = EditorGUILayout.Slider(Styles.ShadowLabel, colorStepValue.w, 0, 1);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(Styles.HBiasLabel);
        colorBiasValue.x = EditorGUILayout.Slider(Styles.DarkLabel, colorBiasValue.x, 0, 1);
        colorBiasValue1.x = EditorGUILayout.Slider(Styles.ShadowLabel, colorBiasValue1.x, 0, 1);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(Styles.SBiasLabel);
        colorBiasValue.y = EditorGUILayout.Slider(Styles.DarkLabel, colorBiasValue.y, 0, 1);
        colorBiasValue1.y = EditorGUILayout.Slider(Styles.ShadowLabel, colorBiasValue1.y, 0, 1);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(Styles.VBiasLabel);
        float v0z = colorBiasValue.z;
        float v0w = colorBiasValue.w;
        float v1z = colorBiasValue1.z;
        float v1w = colorBiasValue1.w;
        EditorGUILayout.BeginHorizontal();
        colorBiasValue.z = EditorGUILayout.FloatField(Styles.DarkLabel, colorBiasValue.z);
        EditorGUILayout.MinMaxSlider(ref colorBiasValue.z, ref colorBiasValue.w, 0, 1);
        colorBiasValue.w = EditorGUILayout.FloatField(colorBiasValue.w, GUILayout.Width(65));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        colorBiasValue1.z = EditorGUILayout.FloatField(Styles.ShadowLabel, colorBiasValue1.z);
        EditorGUILayout.MinMaxSlider(ref colorBiasValue1.z, ref colorBiasValue1.w, 0, 1);
        colorBiasValue1.w = EditorGUILayout.FloatField(colorBiasValue1.w, GUILayout.Width(65));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        specularStepValue.x = EditorGUILayout.FloatField(Styles.SpecularLabel, specularStepValue.x);
        EditorGUILayout.MinMaxSlider(ref specularStepValue.x, ref specularStepValue.y, 0, 1);
        specularStepValue.y = EditorGUILayout.FloatField(specularStepValue.y, GUILayout.Width(65));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        if (GUILayout.Button(Styles.DefaultValueLabel))
        {
            if (EditorUtility.DisplayDialog("提示", "将恢复到默认值", "确定", "取消"))
            {
                colorStepValue = new Vector4(0.7f, 0.1f, 0.5f, 0.1f);
                colorBiasValue = new Vector4(0.02f, 0.1f, 0.1f, 0.2f);
                colorBiasValue1 = new Vector4(0.03f, 0.2f, 0.2f, 0.3f);
                specularStepValue = new Vector4(0f, 1f, 0f, 0f);
            }
        }

        properties.colorStep.vectorValue = colorStepValue;
        properties.colorBias.vectorValue = colorBiasValue;
        properties.colorBias1.vectorValue = colorBiasValue1;
        properties.specularStep.vectorValue = specularStepValue;
    }

    internal static void SetupSDFShadowWorkflowKeyword(Material material, out bool isSDFShadowWorkflow)
    {
        isSDFShadowWorkflow = false;     // default is metallic workflow
        if (material.HasProperty(sWorkflowMode))
            isSDFShadowWorkflow = ((WorkflowMode)material.GetFloat(sWorkflowMode)) == WorkflowMode.SDFFacialShadow;
    }

    // setup keywords for Lit.shader
    public static void SetMaterialKeywords(Material material)
    {
        SetupSDFShadowWorkflowKeyword(material, out bool isSDFShadowWorkFlow);
        
        var hasGlossMap = material.GetTexture(sSpecularGlossMap) != null;
        CoreUtils.SetKeyword(material, "_METALLICSPECGLOSSMAP", hasGlossMap && !isSDFShadowWorkFlow);
        CoreUtils.SetKeyword(material, "_SDFSHADOWMAP", hasGlossMap && isSDFShadowWorkFlow);

        if (material.HasProperty("_SpecularHighlights"))
            CoreUtils.SetKeyword(material, "_SPECULARHIGHLIGHTS_OFF",
                material.GetFloat("_SpecularHighlights") == 0.0f || isSDFShadowWorkFlow);

        if (material.HasProperty("_EnvironmentReflections"))
            CoreUtils.SetKeyword(material, "_ENVIRONMENTREFLECTIONS_OFF",
                material.GetFloat("_EnvironmentReflections") == 0.0f);

        if (material.HasProperty("_BumpMap"))
            CoreUtils.SetKeyword(material, ShaderKeywordStrings._NORMALMAP, !isSDFShadowWorkFlow && material.GetTexture("_BumpMap"));
    }

    public static void Inputs(TBSProperties properties, MaterialEditor materialEditor, Material material)
    {
        materialEditor.TexturePropertySingleLine(Styles.SDFMapText, properties.metallicGlossMap);
    }
}
