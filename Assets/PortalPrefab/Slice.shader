Shader "Custom/Slice"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        sliceNormal("normal", Vector) = (0,0,0,0)
        sliceCentre("centre", Vector) = (0,0,0,0)
        sliceOffsetDst("offset", Float) = 0
        [MaterialToggle] _sliceEnabled("Slice Enabled", Float) = 0

    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  "RenderType" = "Geometry" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard addshadow

        #pragma target 3.0
        sampler2D _MainTex;

        struct Input
        {
            float3 worldPos;
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _sliceEnabled;

        // World space normal of slice, anything along this direction from centre will be invisible
        float3 sliceNormal;
        // World space centre of slice
        float3 sliceCentre;
        // Increasing makes more of the mesh visible, decreasing makes less of the mesh visible
        float sliceOffsetDst;



        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 adjustedCenter = sliceCentre + sliceNormal * sliceOffsetDst;
            if (_sliceEnabled==1) {
                float3 offsetToSliceCenter = IN.worldPos - adjustedCenter;
                clip(-dot(offsetToSliceCenter, sliceNormal));
            }
            float4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Smoothness = _Glossiness;
            o.Metallic = _Metallic;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
