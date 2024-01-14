//������ ������ɫ������
Shader "NW/Lit/DiffuseVertex"{
	//����
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
				fixed3 color : COLOR;
			};

			float4 _Diffuse;

			//������ɫ�� 
			v2f vert(a2v v){
				v2f f;

				//ת�����ü��ռ�
				f.position = UnityObjectToClipPos(v.vertex);

				//������
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				//���߷���
				fixed3 normalDir = normalize(mul(v.normal, (float3x3)unity_WorldToObject));

				//�ƹⷽ��
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

				//���������
				fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir, lightDir), 0);

				f.color = (diffuse + ambient) * _Diffuse;

				return f;
			};

			fixed4 frag(v2f f):SV_TARGET
			{
				return fixed4(f.color, 1);
			};

			ENDCG
		}
	}
	FallBack "VertexLit"
}
