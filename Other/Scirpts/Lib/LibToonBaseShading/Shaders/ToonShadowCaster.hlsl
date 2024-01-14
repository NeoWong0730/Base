#ifndef TOON_SHADOW_CASTER_PASS_2_INCLUDED
#define TOON_SHADOW_CASTER_PASS_2_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "ToonLightInput.hlsl"
#include "Weather.hlsl"

//float4 _ShadowBias; // x: depth bias, y: normal bias

struct Attributes
{
#ifdef _TREE
    half4 color : COLOR;
#endif
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
#ifdef _ALPHATEST_ON
    float2 texcoord     : TEXCOORD0;
#endif                
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
#ifdef _ALPHATEST_ON
    float2 uv           : TEXCOORD0;
#endif
    float4 positionCS   : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

float4x4 Rotation(float4 rotation)
{
    float radx = radians(rotation.x);
    float rady = radians(rotation.y);
    float radz = radians(rotation.z);

    float sinx = sin(radx);
    float cosx = cos(radx);
    float siny = sin(rady);
    float cosy = cos(rady);
    float sinz = sin(radz);
    float cosz = cos(radz);

    float4x4 matrixRot = float4x4(
        cosy * cosz, -cosy * sinz, siny, 0,
        cosx * sinz + sinx * siny * cosz, cosx * cosz - sinx * siny * sinz, -sinx * cosy, 0,
        sinx * sinz - cosx * siny * cosz, sinx * cosz + cosx * siny * sinz, cosx * cosy, 0,
        0, 0, 0, 1
    );
    return matrixRot;
}
	
Varyings ShadowPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

#ifdef _ALPHATEST_ON
	output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
#endif

    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);

#ifdef _TREE        
    float strength = ComputeWindStrength(positionWS.xyz) * clamp(input.positionOS.y, 0, 2);
    float offset = strength * _MaxOffset * (input.color.a - 0.5) * 2;
    positionWS.xz += (_WindParam.xz * offset);
#endif
    
	float3 normalWS = TransformObjectToWorldNormal(input.normalOS);			
	float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
			
#if UNITY_REVERSED_Z
	positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#else
	positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#endif
	output.positionCS = positionCS;

	return output;
}

half4 ShadowPassFragment(Varyings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
#ifdef _ALPHATEST_ON
    half albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a;
    clip(albedoAlpha - _Cutoff);
#endif
    return 0;
}
#endif