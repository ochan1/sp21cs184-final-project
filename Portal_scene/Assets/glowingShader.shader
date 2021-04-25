Shader "Unlit/glowingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [PowerSlider(4)] _Power ("Power", Range(0, 100)) = 1
        [PowerSlider(4)] _Scale ("Scale", Range(0, 100)) = 1
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            float _Scale;
            float _Power;
            fixed4 _FresnelColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldNormal: NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 viewDir = normalize(WorldSpaceViewDir(i.vertex));
                float3 currNormal = normalize(i.worldNormal);
                float R = _Scale * pow(1.0 + dot(currNormal, viewDir), _Power);
                return lerp(col, _FresnelColor, R);   // cyan
            }
            ENDCG
        }
    }
}
