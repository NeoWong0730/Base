//�߹ⷴ�� ������ɫ������
Shader "NW/Lit/SpecularVertex"{
	//����
	Properties{
		_Diffuse("Diffuse Color", Color) = (1, 1, 1, 1)
		_Specular("Specular Color", Color) = (1, 1, 1, 1)
		_Gloss("Gloss", Range(8, 200)) = 10
	}

	SubShader{
		Pass{
			Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" "Queue"="Geometry"}

			CGPROGRAM

			#include "Lighting.cginc"
			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Diffuse;
			fixed4 _Specular;
			half _Gloss;

			struct a2v{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f{
				float4 position : SV_POSITION;
				fixed3 color : COLOR;
			};

			v2f vert(a2v v){
				v2f f;
				f.position = UnityObjectToClipPos(v.vertex);
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				fixed3 normalDir = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir, lightDir), 0) * _Diffuse.rgb;

				//�߹ⷴ��
				fixed3 reflectDir = reflect(-lightDir, normalDir); //�����
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
				fixed3 specular = _LightColor0.rgb * pow(max(0, dot(viewDir, reflectDir)), _Gloss)
				  * _Specular;
				f.color = diffuse + ambient + specular;

				return f;
			}

			fixed4 frag(v2f f):SV_TARGET
			{
				return fixed4(f.color, 1);
			};

			ENDCG
		}
	}
	FallBack "VertexLit"
}
