Shader "Unlit/InvertColorArea"
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Threshold ("Threshold", Range(0., 1.)) = 0
        _ThresholdDiscard ("ThresholdDiscard", Range(0., 1.)) = 0
    }
    SubShader 
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        //Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Cull off
        ZWrite Off
        Blend OneMinusDstColor Zero
        BlendOp Add
        //AlphaToMask On // Needs anti aliasing...

        PASS
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0 Alpha:Blend

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
            fixed4 _Color;
            float _Threshold;
            float _ThresholdDiscard;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
        
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = abs(_Threshold - col.rgb);
                if(col.a < _ThresholdDiscard)
                    discard;
                return col;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}