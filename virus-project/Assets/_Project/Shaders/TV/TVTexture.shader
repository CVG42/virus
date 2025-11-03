Shader "Custom/TVTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineFrequency ("Scanline Frequency", Range(20, 400)) = 120.0
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.1
        _ScanlineSpeed ("Scanline Speed", Float) = 1.0
        _Curvature ("Curvature", Range(0, 0.1)) = 0.02
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0.3
        _ChromaAberration ("Chroma Aberration", Range(0, 0.02)) = 0.005
        _Brightness ("Brightness", Range(0, 2)) = 1.0
        _RGBMaskIntensity ("RGB Mask Intensity", Range(0, 1)) = 0.3
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
            float4 _MainTex_ST;
            float _ScanlineFrequency;
            float _ScanlineIntensity;
            float _ScanlineSpeed;
            float _Curvature;
            float _VignetteIntensity;
            float _ChromaAberration;
            float _Brightness;
            float _RGBMaskIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 ApplyCurvature(float2 uv)
            {
                uv = uv * 2.0 - 1.0;
                float2 offset = abs(uv.yx) * _Curvature;
                uv = uv + uv * offset * offset;
                uv = uv * 0.5 + 0.5;
                return uv;
            }

            float Scanlines(float2 uv, float time)
            {
                float scanlinePos = uv.y * _ScanlineFrequency + time * _ScanlineSpeed * 5.0;
                float scanline = frac(scanlinePos);
    
                scanline = smoothstep(0.0, 0.15, scanline) * smoothstep(1.0, 0.85, scanline);
    
                return lerp(1.0, 1.0 - _ScanlineIntensity, scanline);
            }

            float Vignette(float2 uv)
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(uv, center);
                return 1.0 - dist * _VignetteIntensity;
            }

            float3 RGBMask(float2 uv)
            {
                float3 mask = float3(1.0, 1.0, 1.0);
                
                float stripe = frac(uv.x * 3.0);
                if (stripe < 0.33)
                    mask.gb *= 1.0 - _RGBMaskIntensity;
                else if (stripe < 0.66)
                    mask.rb *= 1.0 - _RGBMaskIntensity;
                else
                    mask.rg *= 1.0 - _RGBMaskIntensity;
                    
                return mask;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 curvedUV = ApplyCurvature(i.uv);
                
                if (curvedUV.x < 0.0 || curvedUV.x > 1.0 || curvedUV.y < 0.0 || curvedUV.y > 1.0)
                    return fixed4(0, 0, 0, 1);

                float2 offset = float2(_ChromaAberration, _ChromaAberration);
                float r = tex2D(_MainTex, curvedUV - offset).r;
                float g = tex2D(_MainTex, curvedUV).g;
                float b = tex2D(_MainTex, curvedUV + offset).b;

                float3 rgbMask = RGBMask(i.uv);
                float3 color = float3(r, g, b) * rgbMask;

                float scanline = Scanlines(i.uv, _Time.y);
                color *= scanline;
                float vignette = Vignette(i.uv);
                color *= vignette;
                color *= _Brightness;
                
                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}