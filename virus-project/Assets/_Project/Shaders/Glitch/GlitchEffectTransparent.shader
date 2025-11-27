Shader "Custom/GlitchEffectTransparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.1
        _GlitchSpeed ("Glitch Speed", Range(0, 10)) = 1.0
        _ColorShift ("Color Shift", Range(0, 0.1)) = 0.02
        _ScanLines ("Scan Lines", Range(0, 1)) = 0.5
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1.0
        [HDR]_BaseColor ("Base Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
            float _GlitchIntensity;
            float _GlitchSpeed;
            float _ColorShift;
            float _ScanLines;
            float _NoiseScale;
            float4 _BaseColor;

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                
                float a = rand(i);
                float b = rand(i + float2(1.0, 0.0));
                float c = rand(i + float2(0.0, 1.0));
                float d = rand(i + float2(1.0, 1.0));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(a, b, u.x) + 
                      (c - a) * u.y * (1.0 - u.x) + 
                      (d - b) * u.x * u.y;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y * _GlitchSpeed;
                
                float glitchNoise = noise(float2(time * 10.0, uv.y * _NoiseScale));
                float glitchThreshold = 0.7;
                
                float horizontalShift = 0.0;
                if (glitchNoise > glitchThreshold)
                {
                    horizontalShift = (glitchNoise - glitchThreshold) / (1.0 - glitchThreshold) * _GlitchIntensity;
                    horizontalShift *= (rand(float2(time, uv.y)) * 2.0 - 1.0);
                }

                float2 glitchedUV = uv + float2(horizontalShift, 0.0);

                float scanLine = sin(uv.y * 800.0 + time * 5.0) * 0.5 + 0.5;
                scanLine = lerp(1.0, scanLine, _ScanLines);
                
                float4 originalSample = tex2D(_MainTex, glitchedUV);
                float originalAlpha = originalSample.a;
                
                float2 redUV = glitchedUV + float2(_ColorShift * sin(time * 3.0), 0.0);
                float2 blueUV = glitchedUV - float2(_ColorShift * cos(time * 2.0), 0.0);
                
                float red = tex2D(_MainTex, redUV).r;
                float green = tex2D(_MainTex, glitchedUV).g;
                float blue = tex2D(_MainTex, blueUV).b;

                float4 finalColor = float4(red, green, blue, originalAlpha);

                finalColor.rgb *= _BaseColor.rgb;

                finalColor.rgb *= scanLine;
                
                float flicker = rand(float2(time * 0.5, 0.0)) * 0.1 + 0.9;
                finalColor.rgb *= flicker;
                
                return finalColor;
            }
            ENDCG
        }
    }
}
