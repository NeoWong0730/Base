using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering.Universal.ShaderGUI;

namespace UnityEditor.Rendering.ToneBasedShading.ShaderGUI
{
    internal class ToonLitShader : BaseShaderGUI
    {
        public static readonly GUIContent HelpSDFLabel = EditorGUIUtility.TrTextContent(
@"R = sdf
G = sdf mirror
B = ID图，目前用于皮肤染色 0 = 染色 1 = 不染色");

        public static readonly GUIContent HelpMetallicLabel = EditorGUIUtility.TrTextContent(
@"R = 金属度，控制高光强弱
G = 暗部调整值，0.5为中间值，值越小越容易进入暗部
B = 光滑度，控制高光聚散
A = ID图，目前用于皮肤染色 0 = 染色 1 = 不染色");

        public static readonly GUIContent HelpEmissionLabel = EditorGUIUtility.TrTextContent(
@"1.设置EmissionMap = 自发光纹理和自发光颜色乘积
2.不设置EmissionMap 勾选UseBaseMap = 基础纹理和自发光颜色乘积
3.不设置EmissionMap 不勾选UseBaseMap = 自发光颜色
* 不使用自发光,直接将自发光颜色置为黑色并且将HDR设置为0");
        public static readonly GUIContent SkinColorLabel = EditorGUIUtility.TrTextContent("Skin Color");
        public static readonly GUIContent EmissionUSEBaseMapLabel = EditorGUIUtility.TrTextContent("USE BaseMap");
        public static readonly GUIContent ForwardLabel = EditorGUIUtility.TrTextContent("Forward");
        public static readonly GUIContent RightLabel = EditorGUIUtility.TrTextContent("Right");

        static readonly string[] workflowModeNames = Enum.GetNames(typeof(ToneBasedShadingGUI.WorkflowMode));

        private LitGUI.LitProperties litProperties;
        private StencilGUI.StencilProperties stencilProperties;
        private ToneBasedShadingGUI.TBSProperties tbsProperties;
        private OutlineGUI.OutlineProperties outlineProperties;
        private RimGUI.RimProperties rimProperties;
        private MaterialProperty emissionUSEBaseMapProp;
        private MaterialProperty skinColorProp;

        private MaterialProperty forwardProp;
        private MaterialProperty rightProp;

        public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
        {
            materialScopesList.RegisterHeaderScope(ToneBasedShadingGUI.Styles.Title, 1 << 5, DrawToneBasedShading);
            materialScopesList.RegisterHeaderScope(RimGUI.Styles.Title, 1 << 6, DrawRim);
            materialScopesList.RegisterHeaderScope(OutlineGUI.Styles.Title, 1 << 7, DrawOutline);

            materialScopesList.RegisterHeaderScope(StencilGUI.Styles.Title, 1 << 4, DrawStencilOptions);
        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            litProperties = new LitGUI.LitProperties(properties);
            stencilProperties = new StencilGUI.StencilProperties(properties);
            tbsProperties = new ToneBasedShadingGUI.TBSProperties(properties);
            outlineProperties = new OutlineGUI.OutlineProperties(properties);
            rimProperties = new RimGUI.RimProperties(properties);

            emissionUSEBaseMapProp = FindProperty("_EmissionUSEBaseMap", properties, false);
            skinColorProp = FindProperty("_SkinColor", properties, false);
            forwardProp = FindProperty("_Forward", properties, false);
            rightProp = FindProperty("_Right", properties, false);
        }

        // material changed check
        public override void ValidateMaterial(Material material)
        {
            SetMaterialKeywords(material, ToneBasedShadingGUI.SetMaterialKeywords, null);
        }

        public void DrawStencilOptions(Material material)
        {
            StencilGUI.Inputs(stencilProperties, materialEditor, material);
        }

        public void DrawToneBasedShading(Material material)
        {
            ToneBasedShadingGUI.ColorInputs(tbsProperties, materialEditor, material);
        }

        public void DrawOutline(Material material)
        {
            OutlineGUI.Inputs(outlineProperties, materialEditor, material);
        }

        public void DrawRim(Material material)
        {
            RimGUI.Inputs(rimProperties, materialEditor, material);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            //if (litProperties.workflowMode != null)
            //    DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, workflowModeNames);
            if (litProperties.workflowMode != null)
                DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, workflowModeNames);

            base.DrawSurfaceOptions(material);
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            using (new EditorGUI.IndentLevelScope(2))
            {
                materialEditor.ShaderProperty(skinColorProp, SkinColorLabel);
            }

            if (litProperties.workflowMode != null &&
    (ToneBasedShadingGUI.WorkflowMode)litProperties.workflowMode.floatValue == ToneBasedShadingGUI.WorkflowMode.SDFFacialShadow)
            {
                materialEditor.ShaderProperty(forwardProp, ForwardLabel);
                materialEditor.ShaderProperty(rightProp, RightLabel);
                EditorGUILayout.HelpBox(HelpSDFLabel);
                ToneBasedShadingGUI.Inputs(tbsProperties, materialEditor, material);
            }
            else
            {
                EditorGUILayout.HelpBox(HelpMetallicLabel);
                LitGUI.Inputs(litProperties, materialEditor, material);
            }

            //materialEditor.ShaderProperty(emissionColor, EmissionColorLabel);
            DrawEmissionProperties(material, true);

            DrawTileOffset(materialEditor, baseMapProp);
        }

        protected override void DrawEmissionProperties(Material material, bool keyword)
        {
            EditorGUILayout.HelpBox(HelpEmissionLabel);

            if ((emissionMapProp != null) && (emissionColorProp != null))
            {
                materialEditor.TexturePropertySingleLine(Styles.emissionMap, emissionMapProp, emissionColorProp);
            }

            if (emissionUSEBaseMapProp != null)
            {
                if (emissionMapProp.textureValue == null)
                {
                    material.DisableKeyword("_EMISSION");
                    using (new EditorGUI.IndentLevelScope(2))
                    {
                        emissionUSEBaseMapProp.floatValue = EditorGUILayout.Toggle(EmissionUSEBaseMapLabel, emissionUSEBaseMapProp.floatValue == 1.0f) ? 1.0f : 0.0f;
                    }
                }
                else
                {
                    material.EnableKeyword("_EMISSION");
                }
            }
        }

        // material main advanced options
        public override void DrawAdvancedOptions(Material material)
        {
            if (litProperties.highlights != null)
            {
                materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
            }

            base.DrawAdvancedOptions(material);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Blend", (float)blendMode);

            material.SetFloat("_Surface", (float)surfaceType);
            if (surfaceType == SurfaceType.Opaque)
            {
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
            else
            {
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
        }
    }
}
