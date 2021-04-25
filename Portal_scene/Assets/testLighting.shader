Shader "Custom/testLighting"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float3 worldPos;
            float2 uv_MainTex;
            float3 worldNormal; // : NORMAL;
            float3 viewDir;
            fixed4 _LightColor0;
            float4 _WorldSpaceLightPos0;
            float4 unity_4LightPosX0;
            float4 unity_4LightPosY0;
            float4 unity_4LightPosZ0;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            // o.Albedo = c.rgb;
            // // Metallic and smoothness come from slider variables
            // o.Metallic = _Metallic;
            // o.Smoothness = _Glossiness;
            //o.Alpha = c.a;

            float k_a = .1;    // 0.1 - ambient
            float k_s = .5;        // 0.5 - specular
            float p = 100.0;    // 100 - specular
            float3 I_a = float3(1.0, 1.0, 1.0);     // 1.0 - ambient
            // (Placeholder code. You will want to replace it.)
            // float3 radius = float3(-IN.worldPos.x, IN.worldPos.y, -IN.worldPos.z) - float3(-IN.unity_4LightPosX0[0], IN.unity_4LightPosY0[0], -IN.unity_4LightPosZ0[0]);
            float3 radius = float3(IN.unity_4LightPosX0[0], IN.unity_4LightPosY0[0], IN.unity_4LightPosZ0[0]) - float3(IN.worldPos.x, IN.worldPos.y, IN.worldPos.z);
            float radius_squared = length(radius) * length(radius);
            float3 falloff = c.rgb;
            radius = normalize(radius);
            float3 v_norm = normalize(IN.worldNormal);
            float3 a_term = k_a * normalize(I_a);
            float3 d_term = falloff * max(0.0, dot(v_norm, normalize(radius)));
            float3 v = normalize(IN.viewDir);
            float3 bisector = normalize(v + normalize(radius));
            float3 s_term = k_s * falloff * pow(max(0.0, dot(v_norm, bisector)), p);
            // o.Albedo = falloff;
            // o.Albedo = float3(IN.unity_4LightPosX0[0], IN.unity_4LightPosY0[0], IN.unity_4LightPosZ0[0]);
            o.Albedo = a_term + s_term + d_term;
            // o.Albedo = falloff;
            // o.Albedo = float3(dot(v_norm, normalize(radius)), dot(v_norm, normalize(radius)), dot(v_norm, normalize(radius)));
            // o.Albedo = d_term;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
