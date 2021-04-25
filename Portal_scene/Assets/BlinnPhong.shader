Shader "Unlit/BlinnPhong"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        
        LOD 100

        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members normalDir)
            #pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            // #pragma multi_compile_fog
            #include "UnityLightingCommon.cginc" // for _LightColor0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
                float4 vertex : SV_POSITION;
                
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
                fixed4 col = 0;
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);

                // WorldSpaceViewDir (float4 v)
                // WorldSpaceLightDir (float4 v)
                //_WorldSpaceLightPos0.xyz
                
                float k_a = 0.1;
                float k_s = 0.5;
                float p = 50;
                fixed4 I_a = fixed4(1.0, 1.0, 1.0, 1.0);
                
                // (Placeholder code. You will want to replace it.)
                float3 radius = WorldSpaceLightDir(i.vertex);
                float3 lightDirection;

                if (0.0 == _WorldSpaceLightPos0.w) // directional light?
                {
                    // attenuation = 1.0; // no attenuation
                    lightDirection = 
                    normalize(_WorldSpaceLightPos0.xyz);
                } 
                else // point or spot light
                {
                    float3 vertexToLightSource = 
                    _WorldSpaceLightPos0.xyz - i.vertex;
                    float distance = length(vertexToLightSource);
                    // attenuation = 1.0 / distance; // linear attenuation 
                    lightDirection = normalize(vertexToLightSource);
                    radius = vertexToLightSource;
                }

                // float3 radius = -i.vertex + _WorldSpaceLightPos0.xyz;
                float radius_squared = length(radius) * length(radius);
                fixed4 falloff = _LightColor0 / radius_squared; // / radius_squared;
                float3 v_norm = normalize(i.worldNormal);
                // Calculate norms
                fixed4 a_term = k_a * I_a;
                fixed4 d_term = falloff * max(0, dot(v_norm, lightDirection));
                float3 v = normalize(-WorldSpaceViewDir(i.vertex));
                float3 bisector = normalize(v + lightDirection);
                fixed4 s_term = k_s * falloff * pow(max(0, dot(v_norm, bisector)), p);
                col = a_term + d_term + s_term;

                // unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0[2]
                // for(int j = 0; j < 4; j++){
                //     float3 radius = float3(unity_4LightPosX0[j], unity_4LightPosY0[j], unity_4LightPosZ0[j]) - i.vertex;
                //     float radius_squared = length(radius) * length(radius);
                //     fixed4 falloff = unity_LightColor[j] / radius_squared; // / radius_squared;
                //     float3 v_norm = normalize(i.worldNormal);
                //     // Calculate norms
                //     fixed4 a_term = k_a * I_a;
                //     fixed4 d_term = falloff * max(0, dot(v_norm, normalize(radius)));
                //     float3 v = normalize(-WorldSpaceViewDir(i.vertex));
                //     float3 bisector = normalize(v + radius);
                //     fixed4 s_term = k_s * falloff * pow(max(0, dot(v_norm, bisector)), p);
                //     col += a_term + d_term + s_term;
                // }

                return col;
            }
            ENDCG
        }


        Pass
        {
            Tags { "LightMode"="ForwardAdd" }

            Blend One One
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members normalDir)
            #pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            // #pragma multi_compile_fog
            #include "UnityLightingCommon.cginc" // for _LightColor0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
                float4 vertex : SV_POSITION;
                
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
                fixed4 col = 0;
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);

                // WorldSpaceViewDir (float4 v)
                // WorldSpaceLightDir (float4 v)
                //_WorldSpaceLightPos0.xyz
                
                float k_a = 0.1;
                float k_s = 0.5;
                float p = 50;
                fixed4 I_a = fixed4(1.0, 1.0, 1.0, 1.0);
                
                // (Placeholder code. You will want to replace it.)
                float3 radius = WorldSpaceLightDir(i.vertex);
                float3 lightDirection;

                if (0.0 == _WorldSpaceLightPos0.w) // directional light?
                {
                    // attenuation = 1.0; // no attenuation
                    lightDirection = 
                    normalize(_WorldSpaceLightPos0.xyz);
                } 
                else // point or spot light
                {
                    float3 vertexToLightSource = 
                    _WorldSpaceLightPos0.xyz - i.vertex;
                    float distance = length(vertexToLightSource);
                    // attenuation = 1.0 / distance; // linear attenuation 
                    lightDirection = normalize(vertexToLightSource);
                    radius = vertexToLightSource;
                }

                // float3 radius = -i.vertex + _WorldSpaceLightPos0.xyz;
                float radius_squared = length(radius) * length(radius);
                fixed4 falloff = _LightColor0 / radius_squared; // / radius_squared;
                float3 v_norm = normalize(i.worldNormal);
                // Calculate norms
                fixed4 a_term = k_a * I_a;
                fixed4 d_term = falloff * max(0, dot(v_norm, lightDirection));
                float3 v = normalize(-WorldSpaceViewDir(i.vertex));
                float3 bisector = normalize(v + lightDirection);
                fixed4 s_term = k_s * falloff * pow(max(0, dot(v_norm, bisector)), p);
                col = a_term + d_term + s_term;

                // unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0[2]
                // for(int j = 0; j < 4; j++){
                //     float3 radius = float3(unity_4LightPosX0[j], unity_4LightPosY0[j], unity_4LightPosZ0[j]) - i.vertex;
                //     float radius_squared = length(radius) * length(radius);
                //     fixed4 falloff = unity_LightColor[j] / radius_squared; // / radius_squared;
                //     float3 v_norm = normalize(i.worldNormal);
                //     // Calculate norms
                //     fixed4 a_term = k_a * I_a;
                //     fixed4 d_term = falloff * max(0, dot(v_norm, normalize(radius)));
                //     float3 v = normalize(-WorldSpaceViewDir(i.vertex));
                //     float3 bisector = normalize(v + radius);
                //     fixed4 s_term = k_s * falloff * pow(max(0, dot(v_norm, bisector)), p);
                //     col += a_term + d_term + s_term;
                // }

                return col;
            }
            ENDCG
        }
    }
}
