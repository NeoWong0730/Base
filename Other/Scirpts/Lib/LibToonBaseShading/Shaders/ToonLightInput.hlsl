#ifndef TOON_LIGHT_INPUT_INCLUDED
#define TOON_LIGHT_INPUT_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct TBSSurfaceData
{
    half3 albedo;
    half3 specular;
    half  metallic;
    half  smoothness;
    half3 normalTS;
    half3 emission;
    half  occlusion;
    half  alpha;
    half4  colorStep;
    half4  colorBias;
    half4  colorBias1;
    half4  specularStep;
};

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half4 _SkinColor;
half4 _EmissionColor;
half4 _RimColor;
//half4 _OutlineColor;
half4 _ColorStep;
half4 _ColorBias;
half4 _ColorBias1;
half4 _SpecularStep;
half4 _Forward;
half4 _Right;
half _Cutoff;
half _Smoothness;
half _Metallic;
half _BumpScale;    
half _RimPower;   
half _EmissionUSEBaseMap;
//half _OutlineWidth;
//half _OffsetZ;
CBUFFER_END

// NOTE: Do not ifdef the properties for dots instancing, but ifdef the actual usage.
// Otherwise you might break CPU-side as property constant-buffer offsets change per variant.
// NOTE: Dots instancing is orthogonal to the constant buffer above.
#ifdef UNITY_DOTS_INSTANCING_ENABLED
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)    
    UNITY_DOTS_INSTANCED_PROP(float4, _EmissionColor)
    UNITY_DOTS_INSTANCED_PROP(float4, _RimColor)
    //UNITY_DOTS_INSTANCED_PROP(float4, _OutlineColor)
    UNITY_DOTS_INSTANCED_PROP(float4 , _ColorStep)
    UNITY_DOTS_INSTANCED_PROP(float4 , _ColorBias)
    UNITY_DOTS_INSTANCED_PROP(float , _Cutoff)
    UNITY_DOTS_INSTANCED_PROP(float , _Smoothness)
    UNITY_DOTS_INSTANCED_PROP(float , _Metallic)
    UNITY_DOTS_INSTANCED_PROP(float , _BumpScale)
    UNITY_DOTS_INSTANCED_PROP(float , _RimPower)
    //UNITY_DOTS_INSTANCED_PROP(float , _OutlineWidth)
    //UNITY_DOTS_INSTANCED_PROP(float , _OffsetZ)
    
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

#define _BaseColor              UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata_BaseColor)
#define _EmissionColor          UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata_EmissionColor)
#define _RimColor               UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata_RimColor)
//#define _OutlineColor           UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata_OutlineColor)
#define _ColorStep              UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_ColorStep)
#define _ColorBias              UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_ColorBias)
#define _Cutoff                 UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Cutoff)
#define _Smoothness             UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Smoothness)
#define _Metallic               UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Metallic)
#define _BumpScale              UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_BumpScale)
#define _RimPower               UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_RimPower)
//#define _OutlineWidth           UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_OutlineWidth)
//#define _OffsetZ                UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_OffsetZ)
#endif

TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
TEXTURE2D(_BumpMap);            SAMPLER(sampler_BumpMap);
TEXTURE2D(_EmissionMap);        SAMPLER(sampler_EmissionMap);
TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);

half Alpha(half albedoAlpha, half4 color, half cutoff)
{
#if !defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A) && !defined(_GLOSSINESS_FROM_BASE_ALPHA)
    half alpha = albedoAlpha * color.a;
#else
    half alpha = color.a;
#endif

#if defined(_ALPHATEST_ON)
    clip(alpha - cutoff);
#endif

    return alpha;
}

half4 SampleAlbedoAlpha(float2 uv, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap))
{
    return half4(SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, uv));
}

half3 SampleNormal(float2 uv, TEXTURE2D_PARAM(bumpMap, sampler_bumpMap), half scale = half(1.0))
{
#ifdef _NORMALMAP
    half4 n = SAMPLE_TEXTURE2D(bumpMap, sampler_bumpMap, uv);
    #if BUMP_SCALE_NOT_SUPPORTED
        return UnpackNormal(n);
    #else
        return UnpackNormalScale(n, scale);
    #endif
#else
    return half3(0.0h, 0.0h, 1.0h);
#endif
}

half3 SampleEmission(float2 uv, half3 emissionColor, half3 albedoColor, half useAlbedo, TEXTURE2D_PARAM(emissionMap, sampler_emissionMap))
{
#ifndef _EMISSION
    return lerp(1.0, albedoColor, useAlbedo) * emissionColor;
#else
    return SAMPLE_TEXTURE2D(emissionMap, sampler_emissionMap, uv).rgb * emissionColor;
#endif
}

inline void InitializeTBSLitSurfaceData(float2 uv, out TBSSurfaceData outSurfaceData)
{    
    outSurfaceData = (TBSSurfaceData)0;

#if defined(_METALLICSPECGLOSSMAP)
    half4 metallicGlossMap = SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv);
    outSurfaceData.metallic = metallicGlossMap.r;
    outSurfaceData.occlusion = metallicGlossMap.g;
    outSurfaceData.smoothness = metallicGlossMap.b * _Smoothness;
    half useBaseMapColor = metallicGlossMap.a;
#elif defined(_SDFSHADOWMAP)
    half4 metallicGlossMap = SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv);
    outSurfaceData.metallic = metallicGlossMap.r;
    outSurfaceData.occlusion = metallicGlossMap.g;
    half useBaseMapColor = metallicGlossMap.b;
#else
    outSurfaceData.metallic = _Metallic;
    outSurfaceData.occlusion = 0.5;
    outSurfaceData.smoothness = _Smoothness;
    half useBaseMapColor = 1;
#endif
    
    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    albedoAlpha = lerp(_SkinColor, half4(1, 1, 1, 1), useBaseMapColor) * albedoAlpha;
    outSurfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);

    outSurfaceData.albedo = _BaseColor.rgb * albedoAlpha.rgb;
    outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);    
    
    outSurfaceData.colorStep = _ColorStep;
    outSurfaceData.colorBias = _ColorBias;
    outSurfaceData.colorBias1 = _ColorBias1;
    outSurfaceData.specularStep = _SpecularStep;

    outSurfaceData.emission = SampleEmission(uv, _EmissionColor.rgb, albedoAlpha.rgb, _EmissionUSEBaseMap, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
}

#endif