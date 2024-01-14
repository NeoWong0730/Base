Shader "SDF/Combine"
{
    Properties
    {
        _MainTex0 ("Texture", 2D) = "white" {}
        _MainTex1 ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "white" {}
        _MainTex3 ("Texture", 2D) = "white" {}
        _MainTex4 ("Texture", 2D) = "white" {}
        _MainTex5 ("Texture", 2D) = "white" {}
        _MainTex6 ("Texture", 2D) = "white" {}
        _MainTex7 ("Texture", 2D) = "white" {}
        _MainTex8 ("Texture", 2D) = "white" {}
        //_thread ("Thread", Range(0,1)) = 0.5
	    _delta("delta", Range(0,0.05)) = 0.01
    }

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex0;
            sampler2D _MainTex1;
            sampler2D _MainTex2;
            sampler2D _MainTex3;
            sampler2D _MainTex4;
            sampler2D _MainTex5;
            sampler2D _MainTex6;
            sampler2D _MainTex7;
            sampler2D _MainTex8;
			//float _thread;
			float _delta;

            fixed4 frag (v2f i) : SV_Target
            {
                const int textureCount = 9;
                const int stepCount = 8;

				fixed4 col0 = tex2D(_MainTex0, i.uv);
				fixed4 col1 = tex2D(_MainTex1, i.uv);
                fixed4 col2 = tex2D(_MainTex2, i.uv); 
                fixed4 col3 = tex2D(_MainTex3, i.uv); 
                fixed4 col4 = tex2D(_MainTex4, i.uv); 
                fixed4 col5 = tex2D(_MainTex5, i.uv); 
                fixed4 col6 = tex2D(_MainTex6, i.uv); 
                fixed4 col7 = tex2D(_MainTex7, i.uv); 
                fixed4 col8 = tex2D(_MainTex8, i.uv);

                float4 color = float4(0, 0, 0, 1);

                float cols[textureCount];

                cols[0] = col0.r;
                cols[1] = col1.r;
                cols[2] = col2.r;
                cols[3] = col3.r;
                cols[4] = col4.r;
                cols[5] = col5.r;
                cols[6] = col6.r;
                cols[7] = col7.r;
                cols[8] = col8.r;

                float4 color2 = float4(0, 0, 0, 1);

            for (float j = 1; j <= 256.0; j++)
            {            
                float _thread = j / 256.0;
                for (int i = 0; i < stepCount; i++)
                {
                    if (i / float(stepCount) < _thread && _thread <= (i + 1) / float(stepCount))
                    {
                        fixed r = lerp(cols[i], cols[i+1], _thread * stepCount - i);
                        r = smoothstep(0.5 - _delta, 0.5 + _delta, r);
                        fixed4 tmp_color = fixed4(r, r, r, 1);
                        color = fixed4(r, r, r, 1);
                        color2 = ((j-1) * color2 + tmp_color) / j;
                        break;
                    }
                }
            }

                return color2;
            }
            ENDCG
        }
    }
}
