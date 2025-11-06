Shader "Custom/UIScreen"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.15
        _ScanlineFrequency ("Scanline Frequency", Range(20, 400)) = 120.0
        _ScanlineSpeed ("Scanline Speed", Float) = 2.0
        _Curvature ("Curvature", Range(0, 0.1)) = 0.01
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0.2
        _ChromaAberration ("Chroma Aberration", Range(0, 0.02)) = 0.003
        _ChromaSpeed ("Chroma Speed", Float) = 1.0
        _ChromaRange ("Chroma Range", Float) = 0.004
        _Brightness ("Brightness", Range(0, 2)) = 1.0
        _RGBMaskIntensity ("RGB Mask Intensity", Range(0, 1)) = 0.2

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 screenPos : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            
            float _ScanlineIntensity;
            float _ScanlineFrequency;
            float _ScanlineSpeed;
            float _Curvature;
            float _VignetteIntensity;
            float _ChromaAberration;
            float _ChromaSpeed;
            float _ChromaRange;
            float _Brightness;
            float _RGBMaskIntensity;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                o.screenPos = ComputeScreenPos(o.vertex).xy;
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
                    return fixed4(0, 0, 0, 0);
                
                float oscillatingChroma = _ChromaAberration + (sin(_Time.y * _ChromaSpeed) * 0.5 + 0.5) * _ChromaRange;

                float2 offset = float2(oscillatingChroma, oscillatingChroma);
                float r = tex2D(_MainTex, curvedUV - offset).r;
                float g = tex2D(_MainTex, curvedUV).g;
                float b = tex2D(_MainTex, curvedUV + offset).b;
                float a = tex2D(_MainTex, curvedUV).a;

                float3 rgbMask = RGBMask(i.uv);
                float3 color = float3(r, g, b) * rgbMask;

                color *= Scanlines(i.uv, _Time.y);
                color *= Vignette(i.uv);
                color *= _Brightness;

                color *= i.color.rgb;
                a *= i.color.a;
                
                return fixed4(color, a);
            }
            ENDCG
        }
    }
}
