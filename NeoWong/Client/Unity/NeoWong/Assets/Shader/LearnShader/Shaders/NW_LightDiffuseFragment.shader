//漫反射 片元着色器计算
Shader "NW/Lit/DiffuseFragment"{
	//属性
	Properties{
		_Diffuse("Diffuse Color", Color) = (1, 1, 1, 1)
	}

	SubShader{
		Pass{
			Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" "Queue"="Geometry"}

			CGPROGRAM

			#include "Lighting.cginc"
			#pragma vertex vert
			#pragma fragment frag

			struct a2v{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f{
				float4 position : SV_POSITION;
				float3 worldNormalDir : COLOR0;
			};

			float4 _Diffuse;

			v2f vert(a2v v){
				v2f f;
				f.position = UnityObjectToClipPos(v.vertex);
				f.worldNormalDir = mul(v.normal, (float3x3)unity_WorldToObject);
				return f;
			}

			fixed4 frag(v2f f):SV_TARGET{
				//环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				//法线
				fixed3 normalDir = normalize(f.worldNormalDir);

				//灯光
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

				//漫反射计算
				fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir, lightDir), 0);
				fixed3 resultColor = (ambient + diffuse) * _Diffuse;

				return fixed4(resultColor, 1);
			}

			ENDCG
		}
	}
	FallBack "VertexLit"
}
