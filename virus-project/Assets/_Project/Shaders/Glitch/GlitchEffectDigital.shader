Shader "Custom/GlitchEffectDigital"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.3
        _GlitchSpeed ("Glitch Speed", Range(0, 10)) = 2.0
        _PixelSize ("Pixel Size", Range(2, 100)) = 20
        _DigitalNoise ("Digital Noise", Range(0, 1)) = 0.5
        _ColorBleed ("Color Bleed", Range(0, 0.2)) = 0.05
        [HDR]_BaseColor ("Base Color", Color) = (1,1,1,1)
        [HDR]_GlitchColor ("Glitch Color", Color) = (0,1,1,1)
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
            float _PixelSize;
            float _DigitalNoise;
            float _ColorBleed;
            float4 _BaseColor;
            float4 _GlitchColor;

            float digitalNoise(float2 uv, float speed)
            {
                float2 pixelUV = floor(uv * _PixelSize) / _PixelSize;
                float noise = frac(sin(dot(pixelUV + _Time.y * speed, float2(12.9898, 78.233))) * 43758.5453);
                return step(0.5, noise);
            }

            float2 blockShift(float2 uv, float time)
            {
                float2 shiftedUV = uv;

                float blockPattern = digitalNoise(float2(uv.y * 3.0, time * 2.0), 1.0);
                float shiftAmount = digitalNoise(float2(time * 5.0, uv.y * 10.0), 2.0) * _GlitchIntensity;
                
                if (blockPattern > 0.5)
                {
                    shiftedUV.x += shiftAmount * 0.1;
                }
                
                return shiftedUV;
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
                
                float2 pixelUV = floor(uv * _PixelSize) / _PixelSize;

                float digitalGlitch = digitalNoise(float2(time * 8.0, uv.y * 5.0), 3.0);
                float colorGlitch = digitalNoise(float2(time * 12.0, uv.x * 4.0), 4.0);
 
                float2 glitchedUV = blockShift(pixelUV, time);

                float4 originalColor = tex2D(_MainTex, glitchedUV);
                float originalAlpha = originalColor.a;

                float2 redUV = glitchedUV + float2(_ColorBleed * digitalGlitch, 0.0);
                float2 blueUV = glitchedUV - float2(_ColorBleed * colorGlitch, 0.0);
                
                float red = tex2D(_MainTex, redUV).r;
                float green = tex2D(_MainTex, glitchedUV).g;
                float blue = tex2D(_MainTex, blueUV).b;

                float4 finalColor = float4(red, green, blue, originalAlpha);
                
                finalColor.rgb *= _BaseColor.rgb;

                if (digitalGlitch > 0.5 && _DigitalNoise > 0.1)
                {
                    float noiseVal = digitalNoise(float2(uv.x * 10.0, time * 20.0), 10.0);
                    if (noiseVal > 0.5)
                    {
                        finalColor.rgb = _GlitchColor.rgb;
                    }
                }

                float scanLine = digitalNoise(float2(0.0, uv.y * 50.0 + time * 10.0), 5.0);
                if (scanLine > 0.5)
                {
                    finalColor.rgb *= 1.5;
                }

                float flicker = digitalNoise(float2(time * 0.5, 0.0), 2.0);
                finalColor.rgb *= (flicker * 0.3 + 0.7);
                
                return finalColor;
            }
            ENDCG
        }
    }
}