Shader "UI/PixelatedTransition"
{
    Properties
    {
        [HDR] _MainColor ("Main Color", Color) = (0, 5, 2, 1)
        _PixelSize ("Pixel Size", Float) = 15
        _Progress ("Progress", Range(0, 1)) = 0
        _GlitchAmount ("Glitch Amount", Range(0, 1)) = 0.3
        _TrailLength ("Trail Length", Range(0, 1)) = 0.4
        _ColorRandomness ("Color Randomness", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        BlendOp Add
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
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
            
            float4 _MainColor;
            float _PixelSize;
            float _Progress;
            float _GlitchAmount;
            float _TrailLength;
            float _ColorRandomness;
            
            float noise(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y;
                float2 uv = i.uv;

                float glitch = noise(float2(time * 10.0, uv.y * 100.0)) * _GlitchAmount;
                uv.x += glitch * 0.02 * _Progress;

                float pixelation = _Progress * _PixelSize;
                
                if (pixelation > 1.0)
                {
                    float2 pixelCoord = floor(uv * pixelation);
                    float2 pixelUV = pixelCoord / pixelation;

                    float columnID = noise(float2(pixelCoord.x * 0.1, 0));
                    float rowTime = time * 2.0 + columnID * 10.0;
                    
                    float dropPos = fmod(pixelCoord.y / 10.0 - rowTime, 1.0);
                    float inTrail = step(dropPos, _TrailLength);

                    float pixelActive = step(noise(pixelCoord * 0.3 + time), _Progress * 1.5) * inTrail;
                    
                    if (pixelActive < 0.1)
                    {
                        discard;
                    }

                    float3 baseColor = _MainColor.rgb;
                    
                    // Variación aleatoria de color
                    float hueShift = noise(pixelCoord * 0.5 + time * 0.1) * _ColorRandomness * 0.5;
                    float3 variedColor = float3(
                        baseColor.r * (0.8 + hueShift),
                        baseColor.g * (1.0 + hueShift * 0.5),
                        baseColor.b * (1.2 - hueShift)
                    );
                    
                    float headBrightness = smoothstep(0.0, 0.1, dropPos) * 2.0;
                    float trailBrightness = smoothstep(_TrailLength, 0.0, dropPos);
                    float brightness = max(headBrightness, trailBrightness);
                    
                    float3 finalColor = variedColor * brightness;
                    
                    float flicker = 0.8 + noise(float2(time * 3.0, pixelCoord.x * 0.7)) * 0.4;
                    finalColor *= flicker;

                    float alpha = _Progress * pixelActive * (0.5 + brightness * 0.5);
                    
                    return fixed4(finalColor, alpha);
                }
                else
                {
                    return fixed4(_MainColor.rgb, _Progress);
                }
            }
            ENDCG
        }
    }
}
