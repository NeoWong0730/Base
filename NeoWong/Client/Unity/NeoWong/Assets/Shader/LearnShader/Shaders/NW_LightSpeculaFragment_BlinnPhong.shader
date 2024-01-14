//BlinnPhong 片元着色器计算
Shader "NW/Lit/SpecularFragment_BlinnPhong"{
	//属性
	Properties
	{
		_Diffuse("Diffuse Color", Color) = (1, 1, 1, 1)
		_Specular("Specular Color", Color) = (1, 1, 1, 1)
		_Gloss("Gloss", Range(8, 200)) = 10
		_Factor("Factor", Range(0, 5)) = 1
	}

	SubShader
	{
		Pass
		{
			Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" "Queue"="Geometry"}

			CGPROGRAM

			#include "Lighting.cginc"

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Diffuse;
			fixed4 _Specular;
			half _Gloss;
			float _Factor;

			struct a2v
			{
				fixed4 vertex : Position;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldVertex : TEXCOORD1;
			};

			v2f vert(a2v v)
			{
				v2f f;
				f.position = UnityObjectToClipPos(v.vertex);
				f.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				f.worldVertex = mul(unity_ObjectToWorld, v.vertex).xyz;

				return f;
			};

			fixed4 frag(v2f f) : SV_TARGET
			{
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				fixed normalDir = normalize(f.worldNormal);
				fixed lightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir, lightDir), 0) * _Diffuse.rgb;

				//BlinnPhong
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - f.worldVertex);
				fixed3 halfDir = normalize(lightDir + viewDir);
				fixed3 specular = _Factor * _LightColor0.rgb * pow(max(0, dot(normalDir, halfDir)), _Gloss) * _Specular;

				fixed3 tempColor = diffuse + ambient + specular;

				return fixed4(tempColor, 1);
			};

			ENDCG
		}
	}

	
	FallBack "VertexLit"
}
