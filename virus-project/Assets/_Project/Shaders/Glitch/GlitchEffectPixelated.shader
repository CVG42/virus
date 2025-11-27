Shader "Custom/GlitchEffectPixelated"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.3
        _GlitchSpeed ("Glitch Speed", Range(0, 10)) = 2.0
        _PixelSize ("Pixel Size", Int) = 10
        _ColorShift ("Color Shift", Range(0, 0.2)) = 0.05
        _BlockGlitch ("Block Glitch", Range(0, 1)) = 0.5
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
            int _PixelSize;
            float _ColorShift;
            float _BlockGlitch;
            float4 _BaseColor;

            // Pixelar la textura
            float4 pixelate(sampler2D tex, float2 uv, int pixelSize)
            {
                float2 pixelUV = floor(uv * pixelSize) / pixelSize;
                return tex2D(tex, pixelUV);
            }

            // Ruido de bloques cuadrados
            float blockNoise(float2 blockCoord, float time)
            {
                float2 gridPos = floor(blockCoord);
                return frac(sin(dot(gridPos + time, float2(12.9898, 78.233))) * 43758.5453);
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
                
                // Coordenadas de bloque para efectos
                float2 blockCoord = uv * _PixelSize;
                float2 pixelBlock = floor(blockCoord);
                
                // Efecto de glitch por bloques
                float glitchBlock = blockNoise(pixelBlock + float2(0, time * 3.0), time);
                float shiftBlock = blockNoise(pixelBlock + float2(100.0, time * 5.0), time);
                
                // Aplicar desplazamiento solo a bloques específicos
                float2 glitchedUV = uv;
                if (glitchBlock > 0.7 && _BlockGlitch > 0.1)
                {
                    float shiftAmount = (shiftBlock - 0.5) * _GlitchIntensity * 0.1;
                    glitchedUV.x += shiftAmount;
                    
                    // Hacer que el desplazamiento sea en pasos de píxel
                    glitchedUV = floor(glitchedUV * _PixelSize) / _PixelSize;
                }
                
                // Pixelado base
                float2 finalUV = floor(glitchedUV * _PixelSize) / _PixelSize;
                float4 originalColor = pixelate(_MainTex, finalUV, _PixelSize);
                float originalAlpha = originalColor.a;
                
                // Efecto RGB split por bloques
                float4 finalColor = originalColor;
                if (shiftBlock > 0.6)
                {
                    float2 redUV = floor((glitchedUV + float2(_ColorShift, 0.0)) * _PixelSize) / _PixelSize;
                    float2 blueUV = floor((glitchedUV - float2(_ColorShift, 0.0)) * _PixelSize) / _PixelSize;
                    
                    float red = tex2D(_MainTex, redUV).r;
                    float green = originalColor.g;
                    float blue = tex2D(_MainTex, blueUV).b;
                    
                    finalColor = float4(red, green, blue, originalAlpha);
                }
                
                // Efecto de bloques que desaparecen
                if (glitchBlock > 0.9)
                {
                    finalColor.rgb = float3(0, 0, 0); // Bloque negro
                }
                else if (glitchBlock > 0.85)
                {
                    finalColor.rgb = float3(1, 1, 1); // Bloque blanco
                }
                
                // Aplicar color base
                finalColor.rgb *= _BaseColor.rgb;
                
                return finalColor;
            }
            ENDCG
        }
    }
}
