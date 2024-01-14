#ifndef TOON_LIGHTING_INCLUDED
#define TOON_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
#include "AdditionLighting.hlsl"

half3 RGBToNormalHSV(half3 albedo)
{
    half3 hsv = RgbToHsv(albedo);    

    //先映射到0-0.5 0.5-1
    //h = h - 0.1667;
    //if(h2 <= 0) 0 和 1 是一样的
    hsv.x += step(hsv.x - 0.1667, 0);

    return hsv;
}

half3 NormalHSVToRGB(half3 hsv)
{    
    hsv.x -= step(1, hsv.x + 0.1667);
    return HsvToRgb(hsv);
}

half3 GetVertexFacialShadow()
{
    Light mainLight = GetMainLight();

    half3 lightDir = mainLight.direction;
    half3 forward = TransformObjectToWorldDir(_Forward.xyz);
    half3 right = TransformObjectToWorldDir(_Right.xyz);
    //half3 forward = TransformObjectToWorldNormal(_Forward.xyz);
    //half3 right = TransformObjectToWorldNormal(_Right.xyz);

    lightDir.y = 0;
    forward.y = 0;
    right.y = 0;

    lightDir = NormalizeNormalPerVertex(lightDir);
    forward = NormalizeNormalPerVertex(forward);
    right = NormalizeNormalPerVertex(right);

    half halfLambert = dot(lightDir, forward) * 0.5 + 0.5; //saturate();
    half s = step(0, dot(lightDir, right));

    return half3(halfLambert, s, 1);
}

half3 PixelFacialShadow(half3 lightColor, half3 facialShadow, TBSSurfaceData surfaceData)
{
    half3 color0 = surfaceData.albedo;
    half3 normalHSV = RGBToNormalHSV(color0);
    half3 colorBias0 = surfaceData.colorBias.xyz;
    //根据饱和度来调整明度的偏移强度
    colorBias0.z = lerp(surfaceData.colorBias.z, surfaceData.colorBias.w, normalHSV.y);

    half3 color1 = NormalHSVToRGB(lerp(normalHSV, half3(0.5, 1, 0), colorBias0));    

    half v = 1 - lerp(surfaceData.occlusion, surfaceData.metallic, facialShadow.y); //1-亮部 = 阴影的阈值
    half3 color = lerp(color1, color0, smoothstep(v, v + surfaceData.colorStep.y, facialShadow.x)) * lightColor;//smoothstep(v, v + surfaceData.colorStep.y, facialShadow.x)

    return color;
    //return lightColor * facialShadow.x;
}

half3 CalculateDiffuse(half3 lightColor, half lightAttenuation, half3 lightDir, half3 normal, TBSSurfaceData surfaceData)
{ 
    half3 color0 = surfaceData.albedo;
    half3 normalHSV = RGBToNormalHSV(color0);
    half3 colorBias0 = surfaceData.colorBias.xyz;
    half3 colorBias1 = surfaceData.colorBias1.xyz;

    //根据饱和度来调整明度的偏移强度
    colorBias0.z = lerp(surfaceData.colorBias.z, surfaceData.colorBias.w, normalHSV.y);
    colorBias1.z = lerp(surfaceData.colorBias1.z, surfaceData.colorBias1.w, normalHSV.y);

    half3 color1 = NormalHSVToRGB(lerp(normalHSV, half3(0.5, 1, 0), colorBias0));
    half3 color2 = NormalHSVToRGB(lerp(normalHSV, half3(0.5, 1, 0), colorBias1));

    half4 colorStep = surfaceData.colorStep;
    half step0 = colorStep.x;
    half feather0 = lerp(step0, 1, colorStep.y);
    half step1 = colorStep.z;
    half feather1 = lerp(step1, step0, colorStep.w);

    //Half Lambert
    //half halfLambert = dot(lightDir, normal) * 0.5 + 0.5;
    //halfLambert = saturate(halfLambert + surfaceData.occlusion - 0.5);
    half halfLambert = saturate(dot(lightDir, normal) * 0.5 * lightAttenuation + surfaceData.occlusion);
    
    half3 color = lerp(color1, color0, smoothstep(step0, feather0, halfLambert));
    color = lerp(color2, color, smoothstep(step1, feather1, halfLambert)) * lightColor;

    return color;
}

half3 CalculateSpecular(half3 lightColor, float3 lightDir, InputData inputData, TBSSurfaceData surfaceData)
{   
    float3 halfDir = SafeNormalize(lightDir + inputData.viewDirectionWS);
    float NoH = saturate(dot(inputData.normalWS, halfDir));
            
    half LoH = saturate(dot(lightDir, halfDir));
    half perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(surfaceData.smoothness);
    half roughness = max(PerceptualRoughnessToRoughness(perceptualRoughness), HALF_MIN);
    half roughness2 = roughness * roughness;
    half normalizationTerm = roughness * 4.0h + 2.0h;
    half roughness2MinusOne = roughness2 - 1.0h;
            
    float d = NoH * NoH * roughness2MinusOne + 1.00001f;
            

    half LoH2 = LoH * LoH;
    half specularTerm = roughness2 / ((d * d) * max(0.1h, LoH2) * normalizationTerm);            

#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
    specularTerm = specularTerm - HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif

    specularTerm = smoothstep(surfaceData.specularStep.x, surfaceData.specularStep.y, specularTerm);
    return surfaceData.metallic * lightColor *  specularTerm;// * surfaceData.albedo;
}

half3 CalculateSpecularSample(half3 lightColor, float3 lightDir, InputData inputData, TBSSurfaceData surfaceData)
{
    float3 halfDir = SafeNormalize(lightDir + inputData.viewDirectionWS);
    float NoH = saturate(dot(inputData.normalWS, halfDir));

    float specularTerm = pow(NoH, surfaceData.smoothness);

    specularTerm = smoothstep(surfaceData.specularStep.x, surfaceData.specularStep.y, specularTerm);
    return surfaceData.metallic * lightColor *  specularTerm * surfaceData.albedo;
}

half4 ToneBasedShadingFragmentLit(InputData inputData, TBSSurfaceData surfaceData)
{
    Light mainLight = GetMainLight(inputData.shadowCoord);
#if !defined(_SDFSHADOWMAP)  
    half3 diffuseColor = CalculateDiffuse(mainLight.color, mainLight.shadowAttenuation * mainLight.distanceAttenuation, mainLight.direction, inputData.normalWS, surfaceData);
#else
    half3 diffuseColor = PixelFacialShadow(mainLight.color, inputData.vertexLighting, surfaceData);
#endif

#ifdef _SPECULARHIGHLIGHTS_OFF
    half3 specularColor = half3(0, 0, 0);
#else
    half3 specularColor = CalculateSpecular(mainLight.color, mainLight.direction, inputData, surfaceData);
    //half3 specularColor = CalculateSpecularSample(mainLight.color, mainLight.direction, inputData, surfaceData);
#endif

#ifdef _ADDITIONAL_LIGHTS
    diffuseColor += CalculateAdditionLights(inputData.positionWS, inputData.normalWS, inputData.vertexLighting.xy / inputData.vertexLighting.z) * surfaceData.albedo;
#endif

    half3 emissiveColor = surfaceData.emission * surfaceData.albedo;
    half3 giColor = surfaceData.albedo * inputData.bakedGI;

    half3 rimColor = half3(0, 0, 0);
#ifdef _RIM_ON
    half NdotV = saturate(dot(inputData.normalWS, inputData.viewDirectionWS));
    half rimPower =  pow((1 - NdotV), exp2(lerp(3, 0, _RimPower)));
    rimColor = _RimColor.rgb * rimPower;
#endif

    half3 rgb = diffuseColor + emissiveColor + giColor + specularColor * mainLight.shadowAttenuation * mainLight.distanceAttenuation + rimColor;

    half4 color = half4(rgb, surfaceData.alpha);
    return color;
}

#endif

