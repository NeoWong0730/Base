#ifndef TOON_OUTLINE_2_INCLUDED
#define TOON_OUTLINE_2_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

CBUFFER_START(UnityPerMaterial)
half4 _OutlineColor;
half _OutlineWidth;
half _OffsetZ;
CBUFFER_END

#ifdef UNITY_DOTS_INSTANCING_ENABLED
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP(float4, _OutlineColor)    
    UNITY_DOTS_INSTANCED_PROP(float , _OutlineWidth)
    UNITY_DOTS_INSTANCED_PROP(float , _OffsetZ)    
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

#define _OutlineColor           UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata_OutlineColor)
#define _OutlineWidth           UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_OutlineWidth)
#define _OffsetZ                UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_OffsetZ)
#endif

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    //float4 color        : Color;

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

Varyings vert2 (Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

#ifdef _ALPHATEST_ON
	output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
#endif

    float outlineWidth = _OutlineWidth * 0.001;//(_ScreenParams.w - 1);
	float4 pos = TransformObjectToHClip(input.positionOS);
	float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, input.normalOS.xyz);
	float3 ndcNormal = normalize(TransformWViewToHClip(viewNormal.xyz)) * pos.w;
	float aspect = (_ScreenParams.y) / (_ScreenParams.x);

	ndcNormal.x *= aspect;
	pos.xy += outlineWidth * ndcNormal.xy;
	output.positionCS = pos;
   // output.positionCS = TransformObjectToHClip(float4(input.positionOS.xyz + input.normalOS * outlineWidth, input.positionOS.w));

#if defined(UNITY_REVERSED_Z)
    output.positionCS.z -= _OffsetZ * 0.01;
#else
    output.positionCS.z += _OffsetZ * 0.01;
#endif

    return output;
}

Varyings vert3 (Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

#ifdef _ALPHATEST_ON
	output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
#endif

    output.positionCS = TransformObjectToHClip(input.positionOS);
    float3 normalVS = mul((float3x3)UNITY_MATRIX_IT_MV, input.normalOS.xyz);
    float3 normalCS = normalize(TransformWViewToHClip(normalVS.xyz)) * output.positionCS.w;
    float2 outlineWidth = (_ScreenParams.wz - 1) * _OutlineWidth;
    output.positionCS.xy += normalCS.xy * outlineWidth;

#if defined(UNITY_REVERSED_Z)
    output.positionCS.z -= (_OffsetZ * (_ScreenParams.w - 1));
#else
    output.positionCS.z += (_OffsetZ * (_ScreenParams.w - 1));
#endif

    return output;
}

Varyings vert (Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

#ifdef _ALPHATEST_ON
	output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
#endif

    float outlineWidth = _OutlineWidth * 0.001;
    output.positionCS = TransformObjectToHClip(float4(input.positionOS.xyz + input.normalOS * outlineWidth, input.positionOS.w));

#if UNITY_REVERSED_Z
    #if SHADER_API_OPENGL || SHADER_API_GLES || SHADER_API_GLES3
        output.positionCS.z += _OffsetZ * 0.01;
    #else
        output.positionCS.z -= _OffsetZ * 0.01;
    #endif
#elif UNITY_UV_STARTS_AT_TOP
    output.positionCS.z += _OffsetZ * 0.01;
#else
    output.positionCS.z += _OffsetZ * 0.01;
#endif    

    return output;
}
/*
Varyings vertZBias (Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

#ifdef _ALPHATEST_ON
	output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
#endif
    float outlineWidth = _Outline_Width * 0.001;

    float4 positionVS = mul(UNITY_MATRIX_MV, input.positionOS);
    positionVS.z += outlineWidth;
    output.positionCS = TransformWViewToHClip(positionVS.rgb);
    return output;
}

Varyings vertShell (Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

#ifdef _ALPHATEST_ON
	output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
#endif
    float outlineWidth = _Outline_Width * 0.001;

    float4 positionVS = mul(UNITY_MATRIX_MV, input.positionOS);
    float3 normalVS = TransformWorldToViewDir(TransformObjectToWorldDir(input.normalOS));
    normalVS.z - 0.4;
    positionVS = positionVS + float4(normalize(normalVS), 0) * outlineWidth;    
    
    output.positionCS = TransformWViewToHClip(positionVS.rgb);

    return output;
}
*/
half4 frag(Varyings input) : SV_Target
{              
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

#ifdef _ALPHATEST_ON
    half albedoAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a;
    clip(albedoAlpha - _Cutoff);
#endif

    half3 color = _OutlineColor.rgb;

#ifdef _Hair
    color = lerp(color, _ColorA * 0.2, _UseTintColor);
#endif

    return half4(color, 1.0);//_Outline_Color;//half4(_Outline_Color.rgb, 1.0);
}
#endif