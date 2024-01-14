Shader "Toon/Lit"
{
    Properties {
        [HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0

        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        _SkinColor("Skin Color", Color) = (1,1,1,1)

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _ColorStep ("Color Step", Vector) = (0.7, 0.1, 0.5, 0.1)
        _ColorBias ("Color Bias Dark", Vector) = (0.02, 0.1, 0.1, 0.2)
        _ColorBias1 ("Color Bias Shadow", Vector) = (0.03, 0.2, 0.1, 0.2)
        _SpecularStep ("Specular Step", Vector) = (0.9, 1.0, 0, 0)

        _Forward ("Forward", Vector) = (0, 0, 1, 0)
        _Right ("Right", Vector) = (1, 0, 0, 0)

        [Space(5)]
        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        [Space(5)]
        _MetallicGlossMap ("Metallic(R) Occlusion(G) Gloss(B) ID(A)", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.5
        [Gamma] _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0

        [Space(5)]
        [HDR]_EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}
        _EmissionUSEBaseMap("Emission USE BaseMap", Float) = 1.0

        [Space(5)]
        [Toggle] _Rim ("Rim", Float ) = 0
        [HDR] _RimColor ("Rim Color", Color) = (0,0,0)
        _RimPower ("Rim Power", Range(0, 1)) = 0.1

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0

        [Space(20)]
        [Toggle] _Outline ("Outline", Float ) = 0
        _OutlineWidth ("Outline Width", Float ) = 0
        _OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1)
        _OffsetZ ("Outline Offset (Z)", Float ) = 0

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        [HideInInspector] _ZTest("__zt", Float) = 4.0

        _ReceiveShadows("Receive Shadows", Float) = 1.0
        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0

        [Space(5)]
        _Stencil("Stencil ID", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _PassStencilOp("Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _FailStencilOp("Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _ZFailStencilOp("ZFail", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
    }

    SubShader {
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "ShaderModel"="4.5"}
        LOD 300

        Pass {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_PassStencilOp]
                Fail[_FailStencilOp]
                ZFail[_ZFailStencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]
            ZTest[_ZTest]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords            
            #pragma shader_feature_local _SDFSHADOWMAP
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _RIM_ON
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF            
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            //#pragma multi_compile_fragment _ DEBUG_DISPLAY

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            //#pragma instancing_options renderinglayer
            //#pragma multi_compile _ DOTS_INSTANCING_ON
            
            #define _REAL_BACKEDGI
            #define _USE_VERTEX_COLOR_MASK
            #define _MATERIALMAP_ON         //只根据是否开启高光确定是否开启 材质纹理 这个宏默认开启（后续可以去掉）

            #pragma vertex vert
            #pragma fragment frag

            #include "ToonLightInput.hlsl"
            #include "ToonLitForwardPasses.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            //#pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "ToonLightInput.hlsl"
            #include "ToonShadowCaster.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "OUTLINE"
            Tags{"LightMode" = "Outline"}

            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_PassStencilOp]
                Fail [_FailStencilOp]
                ZFail [_ZFailStencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }

            Cull Front
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5        

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            #pragma vertex vert
            #pragma fragment frag

            #include "ToonOutline.hlsl"
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.Rendering.ToneBasedShading.ShaderGUI.ToonLitShader"
}