Shader "Loki/LokiPortal"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _TotallyTransparentColor("Totally Transparent Color", Color) = (1, 1, 1, 0.0)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump"{}
        _RimPower("Rim Power", Range(0.5, 8.0)) = 3.0
        _Brightness("Brightness", Range(1, 50)) = 10
        _NormalIntensity("Normal Intensity", Range(0, 50)) = 1
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0.0
        _Outline("Outline width", Range(0, 10)) = 0.1
        _OutlineColor("Outline color", Color) = (1, 1, 1, 1)
        _OutlineSharpness("Outline Sharpness", Range(-100, 0)) = 0
        _OutlineTint("Outline Tint", Color) = (1, 1, 1, 1)
        _StandardDeviation("Standard deviation", Range(1, 10))=1.0
        _FadeLength("Fade Length", Range(0, 2)) = 0.15

        _RainTex("Rain Texture", 2D) = "black"{}
        _RainBumpMap("Rain Normal Map", 2D) = "bump"{}

    }
    SubShader
    {
        Tags{
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
}

        Pass{
            ZWrite Off
            ColorMask 0
}

        Cull Off


        CGPROGRAM
        #pragma surface surf Standard alpha:fade

        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;
        };

        fixed4 _Color;
        half _RimPower;
        half _Brightness;
        half _NormalIntensity;
        half _Glossiness;
        half _Metallic;
        sampler2D _BumpMap;
        half _Outline;
        half _OutlineSharpness;
        fixed4 _OutlineTint;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            if (IN.uv_MainTex.x < 0.5) {
                c += (_Outline * (1 / (pow(1 - IN.uv_MainTex.x, _OutlineSharpness))) * _OutlineTint);
                // c += (_Outline * exp( -pow(IN.uv_MainTex.x, _OutlineSharpness)) * _OutlineTint);
            }
            else {
                c += (_Outline * (1 / pow(IN.uv_MainTex.x, _OutlineSharpness)) * _OutlineTint);
                // c += (_Outline * exp( pow(1 - IN.uv_MainTex.x, _OutlineSharpness)) * _OutlineTint);
            }
            if (IN.uv_MainTex.y < 0.5) {
                c += (_Outline * (1 / pow(1 - IN.uv_MainTex.y, _OutlineSharpness)) * _OutlineTint);
                // c += (_Outline * exp( -pow(IN.uv_MainTex.y, _OutlineSharpness)) * _OutlineTint);
            }
            else {
                c += (_Outline * (1 / pow(IN.uv_MainTex.y, _OutlineSharpness)) * _OutlineTint);
                // c += (_Outline * exp( pow(1 - IN.uv_MainTex.y, _OutlineSharpness)) * _OutlineTint);
            }

            o.Emission = c.rgb;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Normal *= float3(_NormalIntensity, _NormalIntensity, 1);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG

            GrabPass{}

            Pass{
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                struct appdata {
                    float4 vertex : POSITION;
                    float4 uv : TEXCOORD0;
    };
            struct v2f {
                float uv : TEXCOORD0;
                float4 uvgrab : TEXCOORD1;
                float4 uvbump : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            fixed4 _TotallyTransparentColor;
            fixed4 _OutlineColor;
            sampler2D _CameraDepthTexture;
            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            half _StandardDeviation;
            float _FadeLength;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uvgrab.xy = (float2(o.vertex.x, -o.vertex.y) + o.vertex.w) * 0.5;
                o.uvgrab.zw = o.vertex.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET{
                //fixed4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                fixed4 col = fixed4(0, 0, 0, 0);
                half xMultiplier = -1;
                float totalWeight = 0;
                for (float x = -3; x < 4; x++) {
                    half yMultiplier = -1;
                    for (float y = -3; y < 4; y++) {
                        // float kernelVal = (exp(-(x * x)) + exp(-(y * y))) % 1;
                        float kernelVal = ((exp(-( ( (x * x) + (y * y) ) / (2 * (_StandardDeviation * _StandardDeviation) ) ))) / (2 * 3.14 * _StandardDeviation * _StandardDeviation)) % 1;
                        totalWeight += kernelVal;
                        float xBlurOffset = 0.01 * xMultiplier;
                        float yBlurOffset = 0.01 * yMultiplier;
                        float4 newUVForBlur = i.uvgrab;
                        newUVForBlur.x += xBlurOffset;
                        newUVForBlur.y += yBlurOffset;
                        //if (newUVForBlur.x > 1) {
                        //    newUVForBlur.x = newUVForBlur.x - 1;
                        //}
                        //if (newUVForBlur.y > 1) {
                        //    newUVForBlur.y = newUVForBlur.y - 1;
                        //}
                        //if (newUVForBlur.x < 0) {
                        //    newUVForBlur.x = 1 + newUVForBlur.x;
                        //}
                        //if (newUVForBlur.y < 0) {
                        //    newUVForBlur.y = 1 + newUVForBlur.y;
                        //}
                        col += (tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(newUVForBlur)) * kernelVal);
                        yMultiplier++;
                    }
                    xMultiplier++;
                    col /= (totalWeight * 9.8);
                }
                float2 screenuv = i.vertex.xy / _ScreenParams.xy;
                float screenDepth = Linear01Depth(tex2D(_CameraDepthTexture, screenuv));
                float diff = screenDepth - Linear01Depth(i.vertex.z);
                float intersect = 0;

                if (diff > 0)
                    intersect = 1 - smoothstep(0, _ProjectionParams.w * _FadeLength, diff);
                fixed4 glowColor = fixed4(lerp(_TotallyTransparentColor.rgb * _TotallyTransparentColor.a, _OutlineColor, pow(intersect, 8)), 1);

                col += (_TotallyTransparentColor * _TotallyTransparentColor.a + glowColor);
                col.a = _TotallyTransparentColor.a;
                return col;
            }
                ENDCG
        }


            CGPROGRAM
#pragma surface surf Standard alpha:fade

#pragma target 3.0

            sampler2D _MainTex;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_BumpMap;
            };

            fixed4 _Color;
            half _RimPower;
            half _Brightness;
            half _NormalIntensity;
            half _Glossiness;
            half _Metallic;
            sampler2D _BumpMap;
            half _Outline;
            half _OutlineSharpness;
            fixed4 _OutlineTint;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

                o.Emission = c.rgb;
                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
                o.Normal *= float3(_NormalIntensity, _NormalIntensity, 1) * 100;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
            ENDCG

                //CGPROGRAM
                //#pragma surface surf Standard alpha:fade
                //#pragma target 3.0
                //struct Input {
                //    float2 uv_RainTex;
                //    float2 uv_RainBumpMap;
                //};

                //sampler2D _RainTex;
                //sampler2D _RainBumpMap;

                //void surf(Input IN, inout SurfaceOutputStandard o) {
                //    fixed4 c = tex2D(_RainTex, IN.uv_RainTex);
                //    o.Albedo = c.rgb;
                //    o.Alpha = c.a;
                //}

                //ENDCG

    }
    FallBack "Diffuse"
}
