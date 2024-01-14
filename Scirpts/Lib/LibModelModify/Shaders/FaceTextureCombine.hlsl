#ifndef FACE_TEXTURE_COMBINE
#define FACE_TEXTURE_COMBINE

#include "../ShaderLibrary/TextureCombine.hlsl"

SAMPLER(sampler_LinearClamp);

COMBINE_TEXTURE2D(_EyebrowTex);
COMBINE_TEXTURE2D(_BlushTex);
COMBINE_TEXTURE2D(_EyeshadowTex);
COMBINE_TEXTURE2D(_DetailTex);

half3 BlendColor(half3 baseColor, half4 color)
{
    return baseColor * (1 - color.a) + color.rgb * color.a;
}

half3 FaceTextureCombine(half3 baseColor, float2 uv)
{
    baseColor = BlendColor(baseColor, SAMPLE_COMBINE_TEXTURE2D(_EyebrowTex, sampler_LinearClamp, uv));
    baseColor = BlendColor(baseColor, SAMPLE_COMBINE_TEXTURE2D(_BlushTex, sampler_LinearClamp, uv));
    baseColor = BlendColor(baseColor, SAMPLE_COMBINE_TEXTURE2D(_EyeshadowTex, sampler_LinearClamp, uv));
    baseColor = BlendColor(baseColor, SAMPLE_COMBINE_TEXTURE2D(_DetailTex, sampler_LinearClamp, uv));

    return baseColor;
}

#endif