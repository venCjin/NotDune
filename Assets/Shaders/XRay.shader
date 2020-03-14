Shader "Custom/XRay"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _FresnelColor ("Outline (Fresnel) Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [PowerSlider(4)] _FresnelExponent ("Fresnel Exponent", Range(0.25, 4)) = 1

    }
    SubShader
    {
        Tags { /*"RenderType"="Opaque"*/"Queue" = "Transparent"/* "RenderType"="Transparent"*/ }
        LOD 200

        ZTest [_ZTest]



        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows Lambert alpha:fade

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
        fixed4 _FresnelColor;
        float _FresnelExponent;
        int _OnSurface;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input i, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, i.uv_MainTex) * _Color;
            
            float fresnel = dot (i.worldNormal, normalize(i.viewDir));
            fresnel = pow((1 - fresnel), _FresnelExponent);

            o.Emission = (fresnel * _FresnelColor) * (1 - _OnSurface);
            o.Albedo = c * _OnSurface;

            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            o.Alpha = fresnel * (1 - _OnSurface) + c.a * _OnSurface;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
