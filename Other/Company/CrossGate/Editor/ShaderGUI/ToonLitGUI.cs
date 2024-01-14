using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toon
{
    public class ToonLitGUI : BaseShaderGUI
    {
        [FlagsAttribute]
        public enum EKeyFlags
        {
            None = 0,

            Front = 0,  //前渲染默认 正常管线

            _SPECULARHIGHLIGHTS_OFF = 1,            
            _NORMALMAP_ON = 2,
            _TINTMASK_ON = 4,
            _RIM_ON = 8,

            
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

        bool m_ColorStepFoldout;
        bool m_EmissionColorFoldout;
        bool m_TintMaskFoldout;
        bool m_OutlineFoldout;

        private readonly GUIContent colorStep = new GUIContent("卡通色阶", "");
        private readonly GUIContent emissionColor = new GUIContent("自发光", "");
        //private readonly GUIContent colorTranslate = new GUIContent("颜色偏移", "");
        private readonly GUIContent translateColor1 = new GUIContent("暗部颜色 HSV", "颜色偏移 H(X) S(Y) V(Z)");
        private readonly GUIContent translateColor2 = new GUIContent("阴影颜色 HSV", "颜色偏移 H(X) S(Y) V(Z)");

        private readonly GUIContent highlightsText = new GUIContent("Specular Highlights",
            "When enabled, the Material reflects the shine from direct lighting.");

        private readonly GUIContent tintMask = new GUIContent("染色", "");
        
        private readonly GUIContent colorMaskMap = new GUIContent("Mask Map", "");
        private readonly GUIContent emissiveMaskFrom = new GUIContent("Mask Map", "");
        //private readonly GUIContent colorMaskR = new GUIContent("R", "");
        //private readonly GUIContent colorMaskG = new GUIContent("G", "");
        //private readonly GUIContent colorMaskB = new GUIContent("B", "");
        //private readonly GUIContent colorMaskA = new GUIContent("A", "");

        private readonly GUIContent outline = new GUIContent("描边", "");
        private readonly GUIContent use = new GUIContent("开启", "");
        private readonly GUIContent width = new GUIContent("宽度", "");
        private readonly GUIContent color = new GUIContent("颜色", "");

        private GUIContent stencil = new GUIContent("Stencil ID", "");
        private GUIContent stencilComp = new GUIContent("CompareFunction", "");
        private GUIContent passStencilOp = new GUIContent("Pass", "");
        private GUIContent failStencilComp = new GUIContent("Fail", "");
        private GUIContent zFailStencilOp = new GUIContent("ZFail", "");
        private GUIContent stencilWriteMask = new GUIContent("Write Mask", "");
        private GUIContent stencilReadMask = new GUIContent("Read Mask", "");
        private GUIContent rim = new GUIContent("边缘发光(Rim)", "");
        //private GUIContent rimColor = new GUIContent("Read Mask", "");
        private GUIContent power = new GUIContent("强度", "");

        //Hair
        private GUIContent primarySpecularColor = new GUIContent("primary color", "");
        private GUIContent primarySpecSmooth = new GUIContent("primary smooth", "");
        private GUIContent primaryShift = new GUIContent("primary shift", "");
        
        private GUIContent secondarySpecColor = new GUIContent("secondary color", "");
        private GUIContent secondarySpecSmooth = new GUIContent("secondary smooth", "");
        private GUIContent secondaryShift = new GUIContent("secondary shift", "");
        private GUIContent USE_UV2 = new GUIContent("UV2", "");

        private MaterialProperty highlights;

        //private MaterialProperty colorTranslateProp;
        private MaterialProperty translateColorProp1;
        private MaterialProperty translateColorProp2;

        private MaterialProperty useTintMaskProp;
        private MaterialProperty colorMaskMapProp;
        private MaterialProperty colorMaskRProp;
        private MaterialProperty colorMaskGProp;
        private MaterialProperty colorMaskBProp;
        private MaterialProperty colorMaskAProp;

        private MaterialProperty emissiveMaskFromProp;

        private MaterialProperty outlineOffsetProp;
        private MaterialProperty outlineWidthProp;
        private MaterialProperty outlineColorProp;

        private MaterialProperty rimProp;
        private MaterialProperty rimColorProp;
        private MaterialProperty rimPowerProp;

        private MaterialProperty srcBlend;
        private MaterialProperty dstBlend;

        private MaterialProperty _Stencil;
        private MaterialProperty _StencilComp;
        private MaterialProperty _PassStencilOp;
        private MaterialProperty _FailStencilComp;
        private MaterialProperty _ZFailStencilOp;
        private MaterialProperty _StencilWriteMask;
        private MaterialProperty _StencilReadMask;

        //Hair
        private MaterialProperty _PrimarySpecularColor;
        private MaterialProperty _PrimarySpecSmooth;
        private MaterialProperty _PrimaryShift;

        private MaterialProperty _SecondarySpecColor;
        private MaterialProperty _SecondarySpecSmooth;
        private MaterialProperty _SecondaryShift;

        private MaterialProperty _USE_UV2;        

        public readonly string[] emissiveMaskFromNames = { "None", "Base Map (A)", "Material Map (A)" };

        public static int GetRenderQueueOffset(EKeyFlags keyFlags)
        {
            //switch (keyFlags)
            //{
            //    case EKeyFlags.Front:
            //        break;
            //    case EKeyFlags.Front| EKeyFlags._RIM_ON:
            //        break;
            //    case EKeyFlags.Front:
            //        break;
            //    case EKeyFlags.Front:
            //        break;
            //
            //    case EKeyFlags.Both:
            //        break;
            //    case EKeyFlags.Back:
            //        break;
            //    case EKeyFlags._SPECULARHIGHLIGHTS_OFF:
            //        break;
            //    case EKeyFlags._RIM_ON:
            //        break;
            //    case EKeyFlags._NORMALMAP_ON:
            //        break;
            //    case EKeyFlags._TINTMASK_ON:
            //        break;
            //    default:
            //        break;
            //}

            return (int)keyFlags;
        }

        public static void SetMaterialKeywords_ToonLit(Material material)
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
            material.enableInstancing = false;

            bool _NORMALMAP_ON = material.HasProperty("_NormalMap") && material.GetTexture("_NormalMap");
            CoreUtils.SetKeyword(material, "_NORMALMAP_ON", _NORMALMAP_ON);            

            bool _MATERIALMAP_ON = false;
            //使用了各项异性的 强制开启材质贴图 不需要设置宏
            if (material.HasProperty("_MaterialMap") && !material.HasProperty("_PrimarySpecularColor"))
            {
                _MATERIALMAP_ON = material.GetTexture("_MaterialMap");
                //只根据是否开启高光确定是否开启 材质纹理 这个宏默认开启（后续可以去掉）
                //CoreUtils.SetKeyword(material, "_MATERIALMAP_ON", _MATERIALMAP_ON);
            }

            bool _SPECULARHIGHLIGHTS_OFF = true;
            if (material.HasProperty("_SpecularHighlights"))
            {
                //_MATERIALMAP_ON || 
                bool useHighlights = (material.GetFloat("_Metallic") > 0.01f && material.GetFloat("_SpecularHighlights") == 1.0f);
                useHighlights |= _MATERIALMAP_ON;

                _SPECULARHIGHLIGHTS_OFF = !useHighlights;
            }
            CoreUtils.SetKeyword(material, "_SPECULARHIGHLIGHTS_OFF", _SPECULARHIGHLIGHTS_OFF);

            bool hasTintMaskMap = material.HasProperty("_TintMaskMap") && material.GetTexture("_TintMaskMap");
            bool _TINTMASK_ON = hasTintMaskMap && material.HasProperty("_TINTMASK") && material.GetFloat("_TINTMASK") == 1.0f;
            CoreUtils.SetKeyword(material, "_TINTMASK_ON", _TINTMASK_ON);

            if (material.HasProperty("_SHADE_COLOR_TRANSLATE"))
                CoreUtils.SetKeyword(material, "_SHADE_COLOR_TRANSLATE_ON", material.GetFloat("_SHADE_COLOR_TRANSLATE") == 1.0f);

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

            //存在染色系统时 关闭边缘发光
            bool _RIM_ON = !_TINTMASK_ON && material.HasProperty("_Rim") && (material.GetFloat("_Rim") == 1.0f);
            CoreUtils.SetKeyword(material, "_RIM_ON", _RIM_ON);

            CoreUtils.SetKeyword(material, "_USE_UV2_ON", material.HasProperty("_USE_UV2") && (material.GetFloat("_USE_UV2") == 1.0f));

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

            if (_NORMALMAP_ON)
                keyFlags |= EKeyFlags._NORMALMAP_ON;

            if (_SPECULARHIGHLIGHTS_OFF)
                keyFlags |= EKeyFlags._SPECULARHIGHLIGHTS_OFF;

            if (_TINTMASK_ON)
                keyFlags |= EKeyFlags._TINTMASK_ON;
            
            if (_RIM_ON)
                keyFlags |= EKeyFlags._RIM_ON;

            if (material.renderQueue == (int)RenderQueue.AlphaTest)
            {
                material.renderQueue -= GetRenderQueueOffset(keyFlags);
            }
            else
            {
                material.renderQueue += GetRenderQueueOffset(keyFlags);
            }                
        }

        public override void MaterialChanged(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            SetMaterialKeywords_ToonLit(material);
        }

        public override void FindProperties(MaterialProperty[] properties)
        {
            surfaceTypeProp = FindProperty("_Surface", properties);
            blendModeProp = FindProperty("_Blend", properties);
            cullingProp = FindProperty("_Cull", properties);
            alphaClipProp = FindProperty("_ALPHATEST", properties);
            alphaCutoffProp = FindProperty("_Cutoff", properties);
            receiveShadowsProp = FindProperty("_Receive_Shadows", properties, false);
            baseMapProp = FindProperty("_MainTex", properties, false);
            baseColorProp = FindProperty("_BaseColor", properties, false);
            //emissionMapProp = FindProperty("_EmissionMap", properties, false);
            //emissionColorProp = FindProperty("_EmissionColor", properties, false);
            //queueOffsetProp = FindProperty("_QueueOffset", properties, false);    
            queueOffsetProp = FindProperty("_QueueOffset", properties, false);

            litProperties = new LitProperties(properties);

            //colorTranslateProp = FindProperty("_SHADE_COLOR_TRANSLATE", properties, false);
            translateColorProp1 = FindProperty("_ShadeColorTranslate", properties, false);
            translateColorProp2 = FindProperty("_ShadeColorTranslate2", properties, false);

            highlights = FindProperty("_SpecularHighlights", properties, false);

            useTintMaskProp = FindProperty("_TINTMASK", properties, false);
            colorMaskMapProp = FindProperty("_TintMaskMap", properties, false);
            colorMaskRProp = FindProperty("_ColorR", properties, false);
            colorMaskGProp = FindProperty("_ColorG", properties, false);
            colorMaskBProp = FindProperty("_ColorB", properties, false);
            colorMaskAProp = FindProperty("_ColorA", properties, false);

            emissiveMaskFromProp = FindProperty("_EMISSIVE_MASK_IN", properties, false);

            outlineWidthProp = FindProperty("_Outline_Width", properties, false);
            outlineOffsetProp = FindProperty("_OffsetZ", properties, false);
            outlineColorProp = FindProperty("_Outline_Color", properties, false);

            srcBlend = FindProperty("_SrcBlend", properties, false);
            dstBlend = FindProperty("_DstBlend", properties, false);

            _Stencil = FindProperty("_Stencil", properties, false);
            _StencilComp = FindProperty("_StencilComp", properties, false);
            _PassStencilOp = FindProperty("_PassStencilOp", properties, false);
            _FailStencilComp = FindProperty("_FailStencilComp", properties, false);
            _ZFailStencilOp = FindProperty("_ZFailStencilOp", properties, false);
            _StencilWriteMask = FindProperty("_StencilWriteMask", properties, false);
            _StencilReadMask = FindProperty("_StencilReadMask", properties, false);

            rimProp = FindProperty("_Rim", properties, false);
            rimColorProp = FindProperty("_RimLightColor", properties, false);
            rimPowerProp = FindProperty("_RimLight_Power", properties, false);

            _PrimarySpecularColor = FindProperty("_PrimarySpecularColor", properties, false);
            _PrimarySpecSmooth = FindProperty("_PrimarySpecSmooth", properties, false);
            _PrimaryShift = FindProperty("_PrimaryShift", properties, false);

            _SecondarySpecColor = FindProperty("_SecondarySpecColor", properties, false);
            _SecondarySpecSmooth = FindProperty("_SecondarySpecSmooth", properties, false);
            _SecondaryShift = FindProperty("_SecondaryShift", properties, false);

            _USE_UV2 = FindProperty("_USE_UV2", properties, false);
        }

        public override void DrawBaseProperties(Material material)
        {
            base.DrawBaseProperties(material);            
            DoMetallicSpecularArea(litProperties, materialEditor, material);
        }

        protected override void DrawEmissionProperties(Material material, bool keyword)
        {

        }

        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            BaseShaderGUI.DrawNormalArea(materialEditor, litProperties.bumpMapProp, litProperties.bumpScaleProp);

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
            //EditorGUI.BeginChangeCheck();
            //var colorTranslateValue = EditorGUILayout.Toggle(colorTranslate, colorTranslateProp.floatValue == 1f);
            //if (EditorGUI.EndChangeCheck())
            //{
            //    colorTranslateProp.floatValue = colorTranslateValue ? 1f : 0f;
            //}

            if (useTintMaskProp == null || useTintMaskProp.floatValue == 0f)
            {
                materialEditor.ColorProperty(litProperties.baseColor1, "暗部颜色");
                materialEditor.ColorProperty(litProperties.baseColor2, "阴影颜色");
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var translateColor1Value = EditorGUILayout.Vector3Field(translateColor1, translateColorProp1.vectorValue);
                if (EditorGUI.EndChangeCheck())
                {
                    translateColorProp1.vectorValue = new Vector4(translateColor1Value.x, translateColor1Value.y, translateColor1Value.z, 1);
                }

                if (_PrimarySpecularColor != null)
                {
                    EditorGUI.BeginChangeCheck();
                    var translateColor2Vector3 = EditorGUILayout.Vector3Field(translateColor2, translateColorProp2.vectorValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        translateColorProp2.vectorValue = translateColor2Vector3;
                    }
                }
                else
                {                    
                    materialEditor.ShaderProperty(translateColorProp2, translateColor2);

                }                
            }

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
            EditorGUI.BeginChangeCheck();
            var useTintMaskValue = EditorGUILayout.ToggleLeft(tintMask, useTintMaskProp.floatValue == 1f);
            if (EditorGUI.EndChangeCheck())
            {
                useTintMaskProp.floatValue = useTintMaskValue ? 1f : 0f;
            }

            if(useTintMaskValue)
            {
                ++EditorGUI.indentLevel;

                if (colorMaskMapProp != null)
                {
                    materialEditor.TexturePropertySingleLine(colorMaskMap, colorMaskMapProp);
                    materialEditor.ColorProperty(colorMaskRProp, "R");
                    materialEditor.ColorProperty(colorMaskGProp, "G");
                    materialEditor.ColorProperty(colorMaskBProp, "B");
                    materialEditor.ColorProperty(colorMaskAProp, "A");
                }
                else
                {
                    materialEditor.ColorProperty(colorMaskAProp, "A");
                }

                --EditorGUI.indentLevel;
                EditorGUILayout.Space();
            }

            if(rimProp != null)
            {
                EditorGUI.BeginChangeCheck();
                var useRim = EditorGUILayout.ToggleLeft(rim, rimProp.floatValue == 1f);
                if (EditorGUI.EndChangeCheck())
                {
                    rimProp.floatValue = useRim ? 1f : 0f;
                }

                if(useRim)
                {
                    ++EditorGUI.indentLevel;

                    materialEditor.ShaderProperty(rimColorProp, color);
                    materialEditor.ShaderProperty(rimPowerProp, power);

                    --EditorGUI.indentLevel;
                    EditorGUILayout.Space();
                }
            }

            if (outlineWidthProp != null)
            {
                bool useOutline = material.GetShaderPassEnabled("Outline");
                EditorGUI.BeginChangeCheck();
                useOutline = EditorGUILayout.ToggleLeft(outline, useOutline);
                if (EditorGUI.EndChangeCheck())
                {
                    material.SetShaderPassEnabled("Outline", useOutline);
                }

                if (useOutline)
                {
                    ++EditorGUI.indentLevel;
                    materialEditor.FloatProperty(outlineWidthProp, "宽度");
                    materialEditor.FloatProperty(outlineOffsetProp, "Z轴偏移");
                    materialEditor.ColorProperty(outlineColorProp, "颜色");
                    --EditorGUI.indentLevel;
                }
            }
        }

        public void DoMetallicSpecularArea(LitProperties properties, MaterialEditor materialEditor, Material material)
        {
            bool hasGlossMap = false;
            hasGlossMap = properties.metallicGlossMap.textureValue != null;
            //smoothnessChannelNames = Styles.metallicSmoothnessChannelNames;
            materialEditor.TexturePropertySingleLine(LitProperties.metallicMapText, properties.metallicGlossMap, properties.metallic);

            if (_PrimarySpecularColor == null)
            {
                //string[] smoothnessChannelNames;

                EditorGUI.indentLevel += 2;
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = properties.smoothness.hasMixedValue;
                var smoothness = EditorGUILayout.Slider(LitProperties.smoothnessText, properties.smoothness.floatValue, 0f, 1f);
                if (EditorGUI.EndChangeCheck())
                    properties.smoothness.floatValue = smoothness;
                EditorGUI.indentLevel -= 2;
            }
            else
            {
                materialEditor.ShaderProperty(_PrimarySpecularColor, primarySpecularColor);
                materialEditor.ShaderProperty(_PrimarySpecSmooth, primarySpecSmooth);
                materialEditor.ShaderProperty(_PrimaryShift, primaryShift);
                materialEditor.ShaderProperty(_SecondarySpecColor, secondarySpecColor);
                materialEditor.ShaderProperty(_SecondarySpecSmooth, secondarySpecSmooth);
                materialEditor.ShaderProperty(_SecondaryShift, secondaryShift);
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

            if(_USE_UV2 != null)
            {
                EditorGUILayout.Space();
                materialEditor.ShaderProperty(_USE_UV2, USE_UV2);
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
