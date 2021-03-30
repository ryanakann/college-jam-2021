Shader "Custom/Node_shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _Highlight("Highlight Color", Color) = (1,1,1,1)
        _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        [PowerSlider(8)] _FresnelExponent("Fresnel Exponent", Range(0.25, 8)) = 1
        _FresnelExponentFreq("Fresnel Frequency", Range(0, 16)) = 1
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
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
            INTERNAL_DATA
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        fixed4 _Highlight;
        float3 _FresnelColor;
        float _FresnelExponent;
        float _FresnelExponentFreq;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color * _Highlight;
            o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // Fresnel effect
            float fresnel = dot(IN.worldNormal, IN.viewDir);
            fresnel = saturate(1 - fresnel);
            float fresnelExponent = _FresnelExponent;
            fresnel = pow(fresnel, fresnelExponent);
            float fresnelTime = (cos(_Time * _FresnelExponentFreq * 4 * 3.1415926535) + 1) / 2; // Cycle between 0 and 1
            float3 fresnelColor = lerp((0.0, 0.0, 0.0), fresnel * _FresnelColor, fresnelTime);
            o.Emission = fresnelColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
