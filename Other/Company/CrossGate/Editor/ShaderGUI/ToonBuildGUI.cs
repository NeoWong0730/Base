using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toon
{
    public class ToonBuildGUI : BaseShaderGUI
    {
        [FlagsAttribute]
        public enum EKeyFlags
        {
            None = 0,

            Front = 1,  //前渲染默认 正常管线
            
            _EMISSIVE_MASK_IN_BASE_MAP = 2,
            INSTANCING_ON = 4,

            Both = 16,
            Back = 32,
        }

        public enum BlendMode2
        {
            Alpha,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
            Premultiply, // Physically plausible transparency mode, implemented as alpha pre-multiply
            Additive,
            Multiply,
            Other,
        }

        public struct LitProperties
        {
            public static readonly GUIContent baseColorStepStyle = new GUIContent("色阶","阴影 - 暗部 - 亮部");
            public static readonly GUIContent baseColorFeatherStyle = new GUIContent("暗部过渡", "");
            public static readonly GUIContent baseColor1FeatherStyle = new GUIContent("阴影过渡", "");
            public static readonly GUIContent _Snow_HeightStyle = new GUIContent("积雪高度", "");
            public static readonly GUIContent _SnowStrengthFactorStyle = new GUIContent("纹理(A)控制的积雪强度系数 (A + x) * y + z", "");
            public static readonly GUIContent _SnowStrengthFactorStyleMode = new GUIContent("纹理(A)控制的积雪强度模式", "");
            public static readonly GUIContent _MaxOffsetStyle = new GUIContent("Max Offset", "");
            public static readonly GUIContent _SnowTilingStyle = new GUIContent("雪纹理的平铺次数", "");

            public static GUIContent metallicMapText = new GUIContent("Metallic Map", "Metallic(R) Smooth(G) Emissive(A)");
            public static GUIContent smoothnessText = new GUIContent("Smoothness", "Controls the spread of highlights and reflections on the surface.");

            public MaterialProperty baseColorStep;
            public MaterialProperty baseColorFeather;
            public MaterialProperty baseColor1;

            public MaterialProperty baseColor1Step;
            public MaterialProperty baseColor1Feather;
            public MaterialProperty baseColor2;

            // Surface Option Props
            public MaterialProperty workflowMode;

            // Surface Input Props
            public MaterialProperty metallic;            
            public MaterialProperty metallicGlossMap;
            
            public MaterialProperty smoothness;            
            public MaterialProperty bumpMapProp;
            public MaterialProperty bumpScaleProp;
            public MaterialProperty occlusionStrength;
            public MaterialProperty occlusionMap;

            public MaterialProperty emissionColorProp;
            public MaterialProperty emissionDarkColorProp;

            // Advanced Props
            public MaterialProperty highlights;
            public MaterialProperty reflections;

            public LitProperties(MaterialProperty[] properties)
            {
                baseColorStep = BaseShaderGUI.FindProperty("_BaseColor_Step", properties, false);
                baseColorFeather = BaseShaderGUI.FindProperty("_BaseShade_Feather", properties, false);
                baseColor1 = BaseShaderGUI.FindProperty("_1st_ShadeColor", properties, false);

                baseColor1Step = BaseShaderGUI.FindProperty("_ShadeColor_Step", properties, false);
                baseColor1Feather = BaseShaderGUI.FindProperty("_1st2nd_Shades_Feather", properties, false);
                baseColor2 = BaseShaderGUI.FindProperty("_2nd_ShadeColor", properties, false);

                // Surface Option Props
                workflowMode = null;// BaseShaderGUI.FindProperty("_WorkflowMode", properties, false);
                // Surface Input Props
                metallic = BaseShaderGUI.FindProperty("_Metallic", properties, false);
                smoothness = BaseShaderGUI.FindProperty("_Smoothness", properties, false);
                metallicGlossMap = BaseShaderGUI.FindProperty("_MaterialMap", properties, false);

                bumpMapProp = BaseShaderGUI.FindProperty("_NormalMap", properties, false);
                bumpScaleProp = BaseShaderGUI.FindProperty("_BumpScale", properties, false);

                occlusionStrength = null;//BaseShaderGUI.FindProperty("_OcclusionStrength", properties, false);
                occlusionMap = null;//BaseShaderGUI.FindProperty("_OcclusionMap", properties, false);
                // Advanced Props
                highlights = null;//BaseShaderGUI.FindProperty("_SpecularHighlights", properties, false);
                reflections = null;//BaseShaderGUI.FindProperty("_EnvironmentReflections", properties, false);

                emissionColorProp = BaseShaderGUI.FindProperty("_Emissive_Color", properties, false);
                emissionDarkColorProp = BaseShaderGUI.FindProperty("_Emissive_Color_Night", properties, false);
            }
        }

        private LitProperties litProperties;

        private readonly GUIContent colorStep = new GUIContent("卡通色阶", "");
        private readonly GUIContent emissionColor = new GUIContent("自发光", "");        

        private readonly GUIContent highlightsText = new GUIContent("Specular Highlights",
            "When enabled, the Material reflects the shine from direct lighting.");
        
        private readonly GUIContent emissiveMaskFrom = new GUIContent("Mask Map", "");

        private GUIContent stencil = new GUIContent("Stencil ID", "");
        private GUIContent stencilComp = new GUIContent("CompareFunction", "");
        private GUIContent passStencilOp = new GUIContent("Pass", "");
        private GUIContent failStencilComp = new GUIContent("Fail", "");
        private GUIContent zFailStencilOp = new GUIContent("ZFail", "");
        private GUIContent stencilWriteMask = new GUIContent("Write Mask", "");
        private GUIContent stencilReadMask = new GUIContent("Read Mask", "");                

        private MaterialProperty highlights;        
        private MaterialProperty emissiveMaskFromProp;

        private MaterialProperty srcBlend;
        private MaterialProperty dstBlend;

        private MaterialProperty _Stencil;
        private MaterialProperty _StencilComp;
        private MaterialProperty _PassStencilOp;
        private MaterialProperty _FailStencilComp;
        private MaterialProperty _ZFailStencilOp;
        private MaterialProperty _StencilWriteMask;
        private MaterialProperty _StencilReadMask;

        private MaterialProperty _Snow_Height;
        private MaterialProperty _SnowStrengthFactor;
        private MaterialProperty _SnowStrengthFactorMode;
        private MaterialProperty _MaxOffset;
        private MaterialProperty _SnowTiling;

        public readonly string[] emissiveMaskFromNames = { "None", "Base Map (A)"};

        public readonly string[] snowStrengthFactorModes = { "None", "加强(0~1)", "加减(-1~1)", "减弱(-1~0)",  "自定义"};

        public static int GetRenderQueueOffset(EKeyFlags keyFlags)
        {
            return (int)keyFlags;
        }

        public static void SetMaterialKeywords_ToonBuild(Material material)
        {            
            material.shaderKeywords = null;

            if (material.HasProperty("_ALPHATEST") && material.HasProperty("_AlphaClip"))
            {
                material.SetFloat("_AlphaClip", material.GetFloat("_ALPHATEST"));
            }

            // Setup blending - consistent across all Universal RP shaders
            SetupMaterialBlendMode(material);

            //默认都接受阴影
            //if (material.HasProperty("_ReceiveShadows"))
            //    CoreUtils.SetKeyword(material, "_RECEIVE_SHADOWS_OFF", _RECEIVE_SHADOWS_OFF);

            //if (material.HasProperty("_Receive_Shadows"))
            //    CoreUtils.SetKeyword(material, "_RECEIVE_SHADOWS_OFF", material.GetFloat("_Receive_Shadows") == 0.0f);

            //关闭实例化
            //material.enableInstancing = false;

            //去除自发光相关的宏 减少变体 改用数学计算
            int _EMISSIVE_MASK_IN = material.HasProperty("_EMISSIVE_MASK_IN") ? (int)material.GetFloat("_EMISSIVE_MASK_IN") : 0;
            switch (_EMISSIVE_MASK_IN)
            {
                case 1:
                    {
                        //CoreUtils.SetKeyword(material, "_EMISSIVE_MASK_IN_BASE_MAP", true);
                        material.SetFloat("_EmissiveStrength", 1f);
                        material.SetFloat("_EmissiveMaskInBaseMap", 1f);
                        material.SetFloat("_EmissiveMaskInMaterialMap", 0f);
                    }
                    break;
                case 2:
                    {
                        //CoreUtils.SetKeyword(material, "_EMISSIVE_MASK_IN_MATERIAL_MAP", true);
                        material.SetFloat("_EmissiveStrength", 1f);
                        material.SetFloat("_EmissiveMaskInBaseMap", 0f);
                        material.SetFloat("_EmissiveMaskInMaterialMap", 1f);
                    }
                    break;
                default:
                    {
                        material.SetFloat("_EmissiveStrength", 0f);
                        material.SetFloat("_EmissiveMaskInBaseMap", 0f);
                        material.SetFloat("_EmissiveMaskInMaterialMap", 0f);
                    }
                    break;
            }

            bool alphaClip = false;
            if (material.HasProperty("_ALPHATEST"))
                alphaClip = material.GetFloat("_ALPHATEST") >= 0.5;
            
            if (alphaClip)
            {
                material.EnableKeyword("_ALPHATEST_ON");
            }
            else
            {
                material.DisableKeyword("_ALPHATEST_ON");
            }
            
            BaseShaderGUI.RenderFace renderFace = (BaseShaderGUI.RenderFace)material.GetFloat("_Cull");          

            EKeyFlags keyFlags = EKeyFlags.None;
            switch (renderFace)
            {
                case RenderFace.Front:
                    keyFlags |= EKeyFlags.Front;
                    break;
                case RenderFace.Back:
                    keyFlags |= EKeyFlags.Back;
                    break;
                case RenderFace.Both:
                    keyFlags |= EKeyFlags.Both;
                    break;
                default:
                    break;
            }

            if(_EMISSIVE_MASK_IN == 1)
            {
                keyFlags |= EKeyFlags._EMISSIVE_MASK_IN_BASE_MAP;
            }
            //material.enableInstancing = true;
            if (material.enableInstancing)
            {
                keyFlags |= EKeyFlags.INSTANCING_ON;
            }

            if (material.renderQueue == (int)RenderQueue.AlphaTest)
            {
                if (string.Equals(material.shader.name, "Toon/ToonTree", StringComparison.Ordinal))
                {
                    material.renderQueue -= GetRenderQueueOffset(keyFlags);
                }
                else
                {
                    material.renderQueue -= (GetRenderQueueOffset(keyFlags) + 64);
                }
            }
            else
            {
                if (string.Equals(material.shader.name, "Toon/ToonTree", StringComparison.Ordinal))
                {
                    material.renderQueue += (GetRenderQueueOffset(keyFlags) + 64);
                }
                else
                {
                    material.renderQueue -= GetRenderQueueOffset(keyFlags);
                }
            }            
        }

        public override void MaterialChanged(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            SetMaterialKeywords_ToonBuild(material);
        }

        public override void FindProperties(MaterialProperty[] properties)
        {
            surfaceTypeProp = FindProperty("_Surface", properties);
            blendModeProp = FindProperty("_Blend", properties);
            alphaClipProp = FindProperty("_ALPHATEST", properties);
            alphaCutoffProp = FindProperty("_Cutoff", properties);
            srcBlend = FindProperty("_SrcBlend", properties, false);
            dstBlend = FindProperty("_DstBlend", properties, false);
            cullingProp = FindProperty("_Cull", properties);            
            
            receiveShadowsProp = FindProperty("_Receive_Shadows", properties, false);
            baseMapProp = FindProperty("_MainTex", properties, false);
            baseColorProp = FindProperty("_BaseColor", properties, false);                
            queueOffsetProp = FindProperty("_QueueOffset", properties, false);

            litProperties = new LitProperties(properties);

            highlights = FindProperty("_SpecularHighlights", properties, false);

            emissiveMaskFromProp = FindProperty("_EMISSIVE_MASK_IN", properties, false);                        

            _Stencil = FindProperty("_Stencil", properties, false);
            _StencilComp = FindProperty("_StencilComp", properties, false);
            _PassStencilOp = FindProperty("_PassStencilOp", properties, false);
            _FailStencilComp = FindProperty("_FailStencilComp", properties, false);
            _ZFailStencilOp = FindProperty("_ZFailStencilOp", properties, false);
            _StencilWriteMask = FindProperty("_StencilWriteMask", properties, false);
            _StencilReadMask = FindProperty("_StencilReadMask", properties, false);

            _SnowStrengthFactor = FindProperty("_SnowStrengthFactor", properties, false);
            _SnowStrengthFactorMode = FindProperty("_SnowStrengthFactorMode", properties, false);

            _Snow_Height = FindProperty("_Snow_Height", properties, false);
            _MaxOffset = FindProperty("_MaxOffset", properties, false);
            _SnowTiling = FindProperty("_SnowTiling", properties, false);
        }        

        protected override void DrawEmissionProperties(Material material, bool keyword)
        {

        }

        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);

            EditorGUILayout.Space();
            if(emissiveMaskFromProp != null)
            {
                EditorGUILayout.LabelField(emissionColor);
                ++EditorGUI.indentLevel;

                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = emissiveMaskFromProp.hasMixedValue;
                var emissiveMaskFromValue = (int)emissiveMaskFromProp.floatValue;
                emissiveMaskFromValue = EditorGUILayout.Popup(emissiveMaskFrom, emissiveMaskFromValue, emissiveMaskFromNames);
                if (EditorGUI.EndChangeCheck())
                    emissiveMaskFromProp.floatValue = emissiveMaskFromValue;
                EditorGUI.showMixedValue = false;

                materialEditor.ColorProperty(litProperties.emissionColorProp, "自发光白天");
                materialEditor.ColorProperty(litProperties.emissionDarkColorProp, "自发光夜晚");

                --EditorGUI.indentLevel;
                EditorGUILayout.Space();
            }            


            EditorGUILayout.LabelField(colorStep);
            ++EditorGUI.indentLevel;
            materialEditor.ColorProperty(litProperties.baseColor1, "暗部颜色");
            materialEditor.ColorProperty(litProperties.baseColor2, "阴影颜色");
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            var baseColorStep = litProperties.baseColorStep.floatValue;
            var baseColor1Step = litProperties.baseColor1Step.floatValue;

            Vector2 v = new Vector2(baseColor1Step, baseColorStep);
            v = EditorGUILayout.Vector2Field(LitProperties.baseColorStepStyle, v);

            baseColorStep = v.y;
            baseColor1Step = v.x;
            baseColor1Step = Mathf.Min(baseColorStep, baseColor1Step);
            
            EditorGUILayout.MinMaxSlider(LitProperties.baseColorStepStyle, ref baseColor1Step, ref baseColorStep, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                litProperties.baseColorStep.floatValue = baseColorStep;
                litProperties.baseColor1Step.floatValue = baseColor1Step;
            }

            EditorGUI.BeginChangeCheck();
            var baseColorFeather = EditorGUILayout.Slider(LitProperties.baseColorFeatherStyle, litProperties.baseColorFeather.floatValue, 0f, 1f);
            var baseColor1Feather = EditorGUILayout.Slider(LitProperties.baseColor1FeatherStyle, litProperties.baseColor1Feather.floatValue, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                litProperties.baseColorFeather.floatValue = baseColorFeather;
                litProperties.baseColor1Feather.floatValue = baseColor1Feather;
            }
            --EditorGUI.indentLevel;

            EditorGUILayout.Space();

            if (_Snow_Height != null)
            {
                _Snow_Height.floatValue = EditorGUILayout.Slider(LitProperties._Snow_HeightStyle, _Snow_Height.floatValue, 0f, 1f);
            }

            if(_SnowStrengthFactor != null)
            {
                if(_SnowStrengthFactorMode != null)
                {
                    _SnowStrengthFactorMode.floatValue = EditorGUILayout.Popup(LitProperties._SnowStrengthFactorStyleMode, (int)_SnowStrengthFactorMode.floatValue, snowStrengthFactorModes);

                    switch ((int)_SnowStrengthFactorMode.floatValue)
                    {
                        case 0:
                            _SnowStrengthFactor.vectorValue = new Vector4(0, 0, 0, 1);
                            break;
                        case 1:
                            _SnowStrengthFactor.vectorValue = new Vector4(0, 1, 0, 1);
                            break;
                        case 2:
                            _SnowStrengthFactor.vectorValue = new Vector4(-0.5f, 2, 0, 1);
                            break;
                        case 3:
                            _SnowStrengthFactor.vectorValue = new Vector4(0, 1, -1, 1);
                            break;                        
                        default:
                            break;
                    }
                }

                _SnowStrengthFactor.vectorValue = EditorGUILayout.Vector4Field(LitProperties._SnowStrengthFactorStyle, _SnowStrengthFactor.vectorValue);
            }

            if (_MaxOffset != null)
            {
                _MaxOffset.floatValue = EditorGUILayout.FloatField(LitProperties._MaxOffsetStyle, _MaxOffset.floatValue);
            }

            if (_SnowTiling != null)
            {
                _SnowTiling.floatValue = EditorGUILayout.FloatField(LitProperties._SnowTilingStyle, _SnowTiling.floatValue);
            }
        }       

        public override void DrawAdvancedOptions(Material material)
        {
            if (highlights != null)
            {
                EditorGUI.BeginChangeCheck();
                materialEditor.ShaderProperty(highlights, highlightsText);
                if (EditorGUI.EndChangeCheck())
                {
                    MaterialChanged(material);
                }
            }

            base.DrawAdvancedOptions(material);

            if(_Stencil != null)
            {
                materialEditor.ShaderProperty(_Stencil, stencil);
                materialEditor.ShaderProperty(_StencilComp, stencilComp);
                materialEditor.ShaderProperty(_PassStencilOp, passStencilOp);
                materialEditor.ShaderProperty(_FailStencilComp, failStencilComp);
                materialEditor.ShaderProperty(_ZFailStencilOp, zFailStencilOp);
                materialEditor.ShaderProperty(_StencilWriteMask, stencilWriteMask);
                materialEditor.ShaderProperty(_StencilReadMask, stencilReadMask);
            }

            EditorGUILayout.LabelField(material.renderQueue.ToString());
        }
        public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {            
            int _SrcBlend = material.GetInt("_SrcBlend");
            int _DstBlend = material.GetInt("_DstBlend");
            if (_SrcBlend == (int)UnityEngine.Rendering.BlendMode.One && _DstBlend == (int)UnityEngine.Rendering.BlendMode.Zero)
            {
                material.SetFloat("_Surface", 0);
            }
            else
            {
                material.SetFloat("_Surface", 1);

                if (_SrcBlend == (int)UnityEngine.Rendering.BlendMode.SrcAlpha && _DstBlend == (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha)
                {
                    material.SetFloat("_Blend", (int)BlendMode2.Alpha);
                }
                else if (_SrcBlend == (int)UnityEngine.Rendering.BlendMode.One && _DstBlend == (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha)
                {
                    material.SetFloat("_Blend", (int)BlendMode2.Premultiply);
                }
                else if (_SrcBlend == (int)UnityEngine.Rendering.BlendMode.SrcAlpha && _DstBlend == (int)UnityEngine.Rendering.BlendMode.One)
                {
                    material.SetFloat("_Blend", (int)BlendMode2.Additive);
                }
                else if (_SrcBlend == (int)UnityEngine.Rendering.BlendMode.DstColor && _DstBlend == (int)UnityEngine.Rendering.BlendMode.Zero)
                {
                    material.SetFloat("_Blend", (int)BlendMode2.Multiply);
                }
                else
                {
                    material.SetFloat("_Blend", (int)BlendMode2.Other);
                }
            }

            base.OnOpenGUI(material, materialEditor);
        }

        public override void DrawSurfaceOptions(Material material)
        {
            DoPopup(Styles.surfaceType, surfaceTypeProp, Enum.GetNames(typeof(SurfaceType)));
            if ((SurfaceType)material.GetFloat("_Surface") == SurfaceType.Transparent)
            {
                DoPopup(Styles.blendingMode, blendModeProp, Enum.GetNames(typeof(BlendMode2)));
                if (blendModeProp.floatValue == (int)BlendMode2.Other)
                {
                    materialEditor.ShaderProperty(srcBlend, "__src");
                    materialEditor.ShaderProperty(dstBlend, "__dst");
                }
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = cullingProp.hasMixedValue;
            var culling = (RenderFace)cullingProp.floatValue;
            culling = (RenderFace)EditorGUILayout.EnumPopup(Styles.cullingText, culling);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(Styles.cullingText.text);
                cullingProp.floatValue = (float)culling;
                material.doubleSidedGI = (RenderFace)cullingProp.floatValue != RenderFace.Front;
            }

            EditorGUI.showMixedValue = false;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = alphaClipProp.hasMixedValue;
            var alphaClipEnabled = EditorGUILayout.Toggle(Styles.alphaClipText, alphaClipProp.floatValue == 1);
            if (EditorGUI.EndChangeCheck())
                alphaClipProp.floatValue = alphaClipEnabled ? 1 : 0;
            EditorGUI.showMixedValue = false;

            if (alphaClipProp.floatValue == 1)
                materialEditor.ShaderProperty(alphaCutoffProp, Styles.alphaClipThresholdText, 1);

            if (receiveShadowsProp != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = receiveShadowsProp.hasMixedValue;
                var receiveShadows =
                    EditorGUILayout.Toggle(Styles.receiveShadowText, receiveShadowsProp.floatValue == 1.0f);
                if (EditorGUI.EndChangeCheck())
                    receiveShadowsProp.floatValue = receiveShadows ? 1.0f : 0.0f;
                EditorGUI.showMixedValue = false;
            }
        }
    }
}
