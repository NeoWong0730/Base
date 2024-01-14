#ifndef WEATHER_INCLUDED
#define WEATHER_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#if defined(_SNOW_ON) || defined(_RIPPLE_ON) || defined(_GRASS)
TEXTURE2D(_SnowNoise); SAMPLER(SnowNoise_linear_repeat_sampler);
#endif

CBUFFER_START(UnityPerCamera)
//float _SnowFade; x
//float _WaterHeight; y
//float _SnowRatio; z
//float _SnowHeight; w
float4 _SnowData;
half3 _SnowColor;
half3 _SnowColorDark;

//x = 涟漪的噪点缩放
//y = 雪的噪点的缩放
float4 _NoiseScale;
//xy = 涟漪范围控制
//zw = 水坑范围控制
float4 _RipperData;
CBUFFER_END

CBUFFER_START(UnityPerFrame)
    float4 _WindParam;  //xz风向 y风速 w强度
	float _WindDensity; //波的密度		
    float3 _LightDirection;
    float _NightSchedule;
	real4 _AmbientSH;
	float4 _CollideInfo;
CBUFFER_END

float ComputeWindStrength(float3 positionWS)
{
    float disH = dot(_WindParam.xz, positionWS.xz);                 		//当前位置相对风向水平方向的距离
    float disV = length(positionWS.xz - (_WindParam.xz * disH));    		//当前位置相对风向垂直方向的距离
    disH += sin(disV * 0.5);                          						//在风向垂直方向做波动

    disH = disH * _WindDensity + _WindParam.y * _Time.y;
                                        
    //计算从原始位置到最大弯曲位置的比例
    float3 strength4 = sin(float3(disH, disH * 2, disH * 4));
    return ((strength4.x + strength4.y + strength4.z) * 0.25 + 0.2) * _WindParam.w;	
}

#if defined(_RIPPLE_ON)
half ComputeRippleByTexture(half2 ripple, float t)
{	
	//获取波纹的时间,从A通道获取不同的波纹时间,
	float dropFrac = frac(ripple.y + t);
	//把时间限制在R通道内
	float timeFrac = dropFrac - 1.0 + ripple.x;
	//做淡出处理
	float dropFactor = 1 - dropFrac;
	//计算最终的高度，用一个sin计算出随时间的振幅，修改一下值就知道什么效果了
	float final = dropFactor * saturate(sin(clamp(timeFrac * 9.0, 0.0, 4.0) * PI));
	return final;
}

half ComputeRippleColor(float2 uv, float t)
{	
	half4 ripple = SAMPLE_TEXTURE2D(_SnowNoise, SnowNoise_linear_repeat_sampler, uv);
	return ComputeRippleByTexture(ripple.ba, t);
}
#endif

half3 MixWeather(half3 diffuse, float3 positionWS, float3 normalWS)
{
#if defined(_RIPPLE_ON)
	half4 noiseTex = SAMPLE_TEXTURE2D(_SnowNoise, SnowNoise_linear_repeat_sampler, positionWS.xz * _NoiseScale.x);
	//half ripple = ComputeRippleByTexture(noiseTex.ba, positionWS.x + _Time.y);

	half ripple = frac(noiseTex.r + frac(_Time.y));
	diffuse += ripple * step(0.8, normalWS.y);
#elif defined(_SNOW_ON)
	half4 noiseTex = SAMPLE_TEXTURE2D(_SnowNoise, SnowNoise_linear_repeat_sampler,(positionWS.xz + positionWS.y) * _NoiseScale.y);//0.15

	//面朝上导致的积雪强度
	float strength = saturate((normalWS.y - _SnowData.z) / (1 - _SnowData.z));
	//到水面高度的过渡
	float waterFade = positionWS.y - _SnowData.y;//saturate(positionWS.y - _SnowData.y);

    float ndotu = saturate((noiseTex.g * strength + _SnowData.x - 1) * 5 * waterFade);
    diffuse = lerp(diffuse, lerp(_SnowColorDark, _SnowColor, noiseTex.r), ndotu);
#endif
    return diffuse;
}

half3 MixWeather_Tree(half3 diffuse, float3 positionWS, half snowTiling)
{
#if defined(_SNOW_ON)
	half4 noiseTex = SAMPLE_TEXTURE2D(_SnowNoise, SnowNoise_linear_repeat_sampler, positionWS.xz * snowTiling);

	//面朝上导致的积雪强度
	float strength = 1;//saturate((1 - _SnowData.z) / (1 - _SnowData.z));
	//到水面高度的过渡
	//float waterFade = positionWS.y - _SnowData.y;//saturate(positionWS.y - _SnowData.y);

    float ndotu = saturate((noiseTex.g * saturate(0.5 + noiseTex.r)* strength + _SnowData.x - 1) * 2);
    diffuse = lerp(diffuse, lerp(_SnowColorDark, _SnowColor, noiseTex.r), ndotu);
#endif
    return diffuse;
}

half3 MixWeather_Build(half3 diffuse, float3 positionWS, float3 normalWS, half hControl, half texMask, half4 snowStrengthFactor)
{
#if defined(_RIPPLE_ON)
	half4 noiseTex = SAMPLE_TEXTURE2D(_SnowNoise, SnowNoise_linear_repeat_sampler, positionWS.xz * _NoiseScale.x);
	half ripple = frac(frac(_Time.y) + noiseTex.b + noiseTex.a);
	ripple = step(0.9, ripple) * noiseTex.b;

	half2 v2 = saturate((noiseTex.gg - _RipperData.xz) / (_RipperData.yw -  _RipperData.xz));	

	diffuse += ripple * normalWS.y * (1 - v2.x);
	diffuse *= v2.y;
#elif defined(_SNOW_ON)
	half4 noiseTex = SAMPLE_TEXTURE2D(_SnowNoise, SnowNoise_linear_repeat_sampler, (positionWS.xz + positionWS.y) * _NoiseScale.y);
	//厚度导致的积雪强度
	float hStrength = saturate((_SnowData.w - abs(positionWS.y)) / _SnowData.w) * hControl;
	//面朝上导致的积雪强度
	float nStrength = (normalWS.y - _SnowData.z) / (1 - _SnowData.z);
	float strength = max(hStrength, nStrength);

	//到水面高度的过渡
	float waterFade = saturate(positionWS.y - _SnowData.y);

	float ndotu = saturate((noiseTex.g * saturate(0.5 + noiseTex.r) * strength + _SnowData.x - 1) * 5 * waterFade);
#ifdef _BUILD	
	half finalV = saturate(ndotu + (texMask + snowStrengthFactor.x) * snowStrengthFactor.y + snowStrengthFactor.z);
	diffuse = lerp(diffuse, lerp(_SnowColorDark, _SnowColor, noiseTex.r), finalV);
#else
	diffuse = lerp(diffuse, lerp(_SnowColorDark, _SnowColor, noiseTex.r), ndotu);
#endif
    
#endif
    return diffuse;
}

#ifdef _GRASS
real2 MixWeather_Grass(float3 positionWS)
{
	half4 noiseTex = SAMPLE_TEXTURE2D(_SnowNoise, SnowNoise_linear_repeat_sampler, positionWS.xz * 0.15);	
#if defined(_SNOW_ON)	
	real ndotu = saturate((noiseTex.g + _SnowData.x - 1) * 5);	
	return real2(noiseTex.r, ndotu);
#endif
    return real2(noiseTex.r, 0);
}
#endif

half3 MixWeather_Terrain(half3 diffuse, float3 positionWS, float3 normalWS, half snowStrength)
{
#if defined(_RIPPLE_ON)
	half4 noiseTex = SAMPLE_TEXTURE2D(_SnowNoise, SnowNoise_linear_repeat_sampler, positionWS.xz * _NoiseScale.x);
	half ripple = frac(frac(_Time.y) + noiseTex.b + noiseTex.a);
	ripple = step(0.9, ripple) * noiseTex.b;

	half2 v2 = saturate((noiseTex.gg - _RipperData.xz) / (_RipperData.yw -  _RipperData.xz));	

	diffuse += ripple * normalWS.y * (1 - v2.x);
	diffuse *= v2.y;
#elif defined(_SNOW_ON)
	half4 noiseTex = SAMPLE_TEXTURE2D(_SnowNoise, SnowNoise_linear_repeat_sampler,(positionWS.xz + positionWS.y) * _NoiseScale.y);

	//面朝上导致的积雪强度
	float strength = saturate((normalWS.y - _SnowData.z) / (1 - _SnowData.z)) * snowStrength;
	//到水面高度的过渡
	float waterFade = saturate(positionWS.y - _SnowData.y);

    float ndotu = saturate((noiseTex.g * saturate(0.5 + noiseTex.r) * strength + _SnowData.x - 1) * 5 * waterFade);
    diffuse = lerp(diffuse, lerp(_SnowColorDark, _SnowColor, noiseTex.r), ndotu);		
#endif
    return diffuse;
}

#endif