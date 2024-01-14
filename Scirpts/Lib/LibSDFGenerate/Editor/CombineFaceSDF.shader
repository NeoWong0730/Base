Shader "SDF/CombineFaceSDF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BTex ("B Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            sampler2D _BTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {                
                fixed r = tex2D(_MainTex, i.uv).r;
                fixed g = tex2D(_MainTex, float2(1 - i.uv.x, i.uv.y)).g;
                fixed b = tex2D(_BTex, i.uv).b;
                return fixed4(r, g, b, 1);
            }
            ENDCG
        }
    }
}
