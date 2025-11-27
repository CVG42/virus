Shader "Custom/GlitchEffectCompression"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.2
        _GlitchSpeed ("Glitch Speed", Range(0, 5)) = 1.0
        _BlockSize ("Block Size", Range(4, 64)) = 16
        _Compression ("Compression Artifacts", Range(0, 1)) = 0.3
        _ColorShift ("Color Shift", Range(0, 0.1)) = 0.02
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
            float _BlockSize;
            float _Compression;
            float _ColorShift;
            float4 _BaseColor;

            float2 compressionBlocks(float2 uv, float blockSize, float time)
            {
                float2 blockCoord = floor(uv * blockSize);
                float blockHash = frac(sin(dot(blockCoord, float2(12.9898, 78.233))) * 43758.5453);

                if (blockHash > 0.7)
                {
                    float moveAmount = sin(time * 3.0 + blockCoord.y) * _GlitchIntensity * 0.1;
                    uv.x += moveAmount;
                }
                
                return uv;
            }

            float macroBlockEffect(float2 uv, float time)
            {
                float2 macroCoord = floor(uv * (_BlockSize * 0.25));
                float macroHash = frac(sin(dot(macroCoord, float2(67.89, 45.67))) * 43758.5453);
                
                float pulse = sin(time * 5.0 + macroCoord.x * 2.0 + macroCoord.y * 3.0);
                
                if (macroHash > 0.8 && pulse > 0.0)
                {
                    return macroHash;
                }
                return 0.0;
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

                uv = compressionBlocks(uv, _BlockSize, time);

                float2 pixelUV = floor(uv * _BlockSize) / _BlockSize;

                float4 originalColor = tex2D(_MainTex, pixelUV);
                float originalAlpha = originalColor.a;
                
                float macroEffect = macroBlockEffect(uv, time);
                if (macroEffect > 0.0)
                {
                    originalColor.rgb = lerp(originalColor.rgb, float3(1,0,0), macroEffect * _Compression);
                }

                float blockShift = floor(sin(time * 2.0 + uv.y * 10.0) * 3.0) / _BlockSize;
                
                float2 redUV = pixelUV + float2(_ColorShift + blockShift, 0.0);
                float2 greenUV = pixelUV;
                float2 blueUV = pixelUV - float2(_ColorShift + blockShift * 0.5, 0.0);
                
                float red = tex2D(_MainTex, redUV).r;
                float green = tex2D(_MainTex, greenUV).g;
                float blue = tex2D(_MainTex, blueUV).b;

                float4 finalColor = float4(red, green, blue, originalAlpha);

                finalColor.rgb *= _BaseColor.rgb;

                float compressionNoise = frac(sin(uv.y * 100.0) * 43758.5453);
                if (compressionNoise < _Compression * 0.3)
                {
                    finalColor.rgb = floor(finalColor.rgb * 4.0) / 4.0;
                }
                
                return finalColor;
            }
            ENDCG
        }
    }
}