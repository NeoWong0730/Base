#ifndef ADDITION_LIGHTING_INCLUDED
#define ADDITION_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

TEXTURE2D(_AdditionLightMap); SAMPLER(sampler_AdditionLightMap);

half3 Shade4PointLights (
    float4 lightPosX, float4 lightPosY, float4 lightPosZ,
    half3 lightColor0, half3 lightColor1, half3 lightColor2, half3 lightColor3,
    half4 lightAttenX, half4 lightAttenY,
    float3 pos, float3 normal)
{
    // to light vectors
    float4 toLightX = lightPosX - pos.x;
    float4 toLightY = lightPosY - pos.y;
    float4 toLightZ = lightPosZ - pos.z;
    // squared lengths
    float4 distanceSqr = 0;
    distanceSqr += toLightX * toLightX;
    distanceSqr += toLightY * toLightY;
    distanceSqr += toLightZ * toLightZ;
    // don't produce NaNs if some vertex position overlaps with the light
    distanceSqr = max(distanceSqr, HALF_MIN);

    // NdotL
    float4 ndotl = 0;
    ndotl += toLightX * normal.x;
    ndotl += toLightY * normal.y;
    ndotl += toLightZ * normal.z;

    // correct NdotL
    float4 corr = rsqrt(distanceSqr);
    ndotl = saturate(ndotl * corr);
    
    // attenuation
#if SHADER_HINT_NICE_QUALITY
    float4 factor = distanceSqr * lightAttenX;
    float4 smoothFactor = saturate(1.0h - factor * factor);
    smoothFactor = smoothFactor * smoothFactor;
#else
    half4 smoothFactor = saturate(distanceSqr * lightAttenX + lightAttenY);
#endif
    float4 atten = rcp(distanceSqr) * smoothFactor;

    float4 diff = ndotl * atten;

    // final color
    half3 col = 0;
    col += lightColor0 * diff.x;
    col += lightColor1 * diff.y;
    col += lightColor2 * diff.z;
    col += lightColor3 * diff.w;
    return col;
}

float3 GetLightPosition(uint index)
{
#if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
    return _AdditionalLightsBuffer[index].position.xyz;        
#else
    return _AdditionalLightsPosition[index].xyz;
#endif
}

half3 GetLightColor(uint index)
{
#if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
    return _AdditionalLightsBuffer[index].color.rgb;       
#else
    return _AdditionalLightsColor[index].rgb;
#endif
}

half2 GetLightAtten(uint index)
{
#if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
    return _AdditionalLightsBuffer[index].attenuation.xy;
#else
    return _AdditionalLightsAttenuation[index].xy;
#endif
}

half3 CalculateAdditionLights(float3 positionWS, float3 normalWS, float2 uv)
{
    real2 rg = SAMPLE_TEXTURE2D_LOD(_AdditionLightMap, sampler_AdditionLightMap, uv, 0).rg;
                
    uint intR = UnpackByte(rg.r);
    uint intG = UnpackByte(rg.g);

    uint4 indexes = uint4(
        (intR & 3) | ((intG << 2) & 12),
        ((intR >> 2) & 3) | ((intG) & 12),
        ((intR >> 4) & 3) | ((intG >> 2) & 12),
        ((intR >> 6) & 3) | ((intG >> 4) & 12)
    );    

    uint4 indexVerify = step(1, indexes);
    //无效的下标取第0为的灯光
    indexes = (indexes - 1) * indexVerify;

    float3 lightPositionWS0 = GetLightPosition(indexes.x);
    float3 lightPositionWS1 = GetLightPosition(indexes.y);
    float3 lightPositionWS2 = GetLightPosition(indexes.z);
    float3 lightPositionWS3 = GetLightPosition(indexes.w);

    //无效的下标灯光颜色设置为黑色
    half3 lightColor0 = GetLightColor(indexes.x) * indexVerify.x;
    half3 lightColor1 = GetLightColor(indexes.y) * indexVerify.y;
    half3 lightColor2 = GetLightColor(indexes.z) * indexVerify.z;
    half3 lightColor3 = GetLightColor(indexes.w) * indexVerify.w;

    half2 lightAtten0 = GetLightAtten(indexes.x);
    half2 lightAtten1 = GetLightAtten(indexes.y);
    half2 lightAtten2 = GetLightAtten(indexes.z);
    half2 lightAtten3 = GetLightAtten(indexes.w);

    half4 lightAttenX = half4(lightAtten0.x, lightAtten1.x, lightAtten2.x, lightAtten3.x);
    half4 lightAttenY = half4(lightAtten0.y, lightAtten1.y, lightAtten2.y, lightAtten3.y);

    float4 lightPosX = float4(lightPositionWS0.x, lightPositionWS1.x, lightPositionWS2.x, lightPositionWS3.x);
    float4 lightPosY = float4(lightPositionWS0.y, lightPositionWS1.y, lightPositionWS2.y, lightPositionWS3.y);
    float4 lightPosZ = float4(lightPositionWS0.z, lightPositionWS1.z, lightPositionWS2.z, lightPositionWS3.z);

    return Shade4PointLights(lightPosX, lightPosY, lightPosZ,
    lightColor0, lightColor1, lightColor2, lightColor3,
    lightAttenX, lightAttenY,
    positionWS, normalWS);
}
#endif