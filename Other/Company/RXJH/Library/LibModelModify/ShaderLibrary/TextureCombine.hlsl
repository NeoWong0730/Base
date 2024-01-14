#ifndef TEXTURE_COMBINE_INCLUDED
#define TEXTURE_COMBINE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#define COMBINE_TEXTURE2D(textureName)\
Texture2D textureName;\
float4 textureName##_T_M;\
float4 textureName##_R_S;\
half4 textureName##_C

#define SAMPLE_COMBINE_TEXTURE2D(textureName, samplerState, uv) SampleCombineTexture2D(textureName, samplerState, uv, textureName##_T_M, textureName##_R_S, textureName##_C)

half4 SampleCombineTexture2D(Texture2D tex, SamplerState samplerState, float2 uv, float4 translateAndMirror, float4 matrixRS_I, half4 color)
{        
    float2 mirror = translateAndMirror.zw;
    float2 translate = translateAndMirror.xy;    
    float2 c0 = matrixRS_I.xy;
    float2 c1 = matrixRS_I.zw;

    //mirror = 0 uv 0 ~ 1 => 0 ~ 1
    //mirror = 1 uv 0 ~ 1 => 0.5 ~ 0 ~ 0.5
    float2 uv0 = abs(saturate(uv) - 0.5 * mirror);

    //translate
    uv0 = uv0 - translate;

    //RotateScale
    uv0 = (c0 * uv0.x + c1 * uv0.y) + 0.5;

    return SAMPLE_TEXTURE2D_X(tex, samplerState, uv0) * color;
}

float3x3 TRS(float2 translate, float angle, float2 scale)
{
    float s, c;
    sincos(angle,  s,  c);

    return float3x3(c * scale.x, -s * scale.x, 0.0,
                    s * scale.y, c * scale.y, 0.0,
                    translate.x, translate.y, 1.0);
}

float2x2 RS(float angle, float2 scale)
{    
    float s, c;
    sincos(angle,  s,  c);
    return float2x2(c * scale.x, -s * scale.x,
                    s * scale.y, c * scale.y);
}

float2x2 inverse(float2x2 m)
{
    float a = m._m00;
    float b = m._m01;
    float c = m._m10;
    float d = m._m11;

    float det = a * d - b * c;

    return float2x2(d, -b, -c, a) * (1.0 / det);
}

float2 UVTransformNoRotate(float2 uv, float2 translate, float2 scale, float2 mirror)
{
    //mirror = 0 uv 0 ~ 1 => 0 ~ 1
    //mirror = 1 uv 0 ~ 1 => 0.5 ~ 0 ~ 0.5
    float2 uv0 = abs(saturate(uv) - 0.5 * mirror);
    uv0 = (uv0 - translate) / scale + 0.5;

    return uv0;
}

float2 UVTransform(float2 uv, float2 translate, float2 scale, float rotation, float2 mirror)
{
    //mirror = 0 uv 0 ~ 1 => 0 ~ 1
    //mirror = 1 uv 0 ~ 1 => 0.5 ~ 0 ~ 0.5
    float2 uv0 = abs(saturate(uv) - 0.5 * mirror);
    uv0 = uv0 - translate;

    float2x2 m = RS(rotation, scale);
    m = inverse(m);

    float2 c0 = m._m00_m01;
    float2 c1 = m._m10_m11;

    uv0 = (c0 * uv0.x + c1 * uv0.y) + 0.5;

    return uv0;
}

#endif