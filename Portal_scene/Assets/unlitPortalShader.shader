Shader "Unlit/unlitPortalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Center ("Center", Vector) = (0, 0, 0, 0)
        [PowerSlider(4)] _MaxDist ("Max Dist", Range(0, 7)) = 1
        _Color ("Color", Color) = (1,1,1,1)
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Center;
            float _MaxDist;
            fixed4 _Color;
            fixed4 _FresnelColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float3 hsb2rgb(float3 c ){
                float3 rgb = clamp( abs(fmod(c.x*6.0+float3(0.0,4.0,2.0),6)-3.0)-1.0, 0, 1);
                rgb = rgb*rgb*(3.0-2.0*rgb);
                return c.z * lerp( float3(1,1,1), rgb, c.y);
            }

            float3 RGB2HSV(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 tex_col = col;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                // float3 viewDir = normalize(IN.viewDir);
                // float3 currNormal = normalize(IN.worldNormal);
                // put center in world space
                // set max distance from edge, linearly interpolate from that point (IN.vertex) to the edge
                float4 center_world = mul(unity_ObjectToWorld, _Center);
                float4 vertex_world = mul(unity_ObjectToWorld, i.vertex);
                float dist = distance(center_world.xyz, vertex_world.xyz);     // how far from the center we are
                float3 hsv_color = RGB2HSV(_Color.rgb);
                float3 hsv_tex = RGB2HSV(tex_col.rgb);
                // float R = _Scale * pow(1.0 + dist, _Power);   // dot(currNormal, viewDir)
                float3 mix = lerp(hsv_color, hsv_tex, dist);
                // float3 rgb = hsb2rgb(mix);  // lerp(_FresnelColor, c, R) lerp(_FresnelColor, _Color, dist / _MaxDist)
                col = fixed4(mix.x, mix.y, mix.z, 1);
                return col;
            }
            ENDCG
        }
    }
}
