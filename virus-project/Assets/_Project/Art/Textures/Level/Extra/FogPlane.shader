Shader "URP/FogPlane"
{
    Properties
    {
        [HDR]_FogColor ("Fog Color", Color) = (0.8, 0.9, 1.0, 0.15)
        _FogDensity ("Fog Density", Range(0, 1)) = 0.2
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 3.0
        _NoiseSpeed ("Noise Speed", Range(0, 1)) = 0.1
        _LayerOffset ("Layer Offset", Range(0, 2)) = 0.5
        _HeightFalloff ("Height Falloff", Range(0.1, 10)) = 2.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent+100"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float worldHeight : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _FogColor;
                half _FogDensity;
                half _NoiseScale;
                half _NoiseSpeed;
                half _LayerOffset;
                half _HeightFalloff;
            CBUFFER_END

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float fbm(float2 uv)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                
                for (int i = 0; i < 4; i++)
                {
                    value += amplitude * noise(uv * frequency);
                    frequency *= 2.0;
                    amplitude *= 0.5;
                }
                
                return value;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 posOS = IN.positionOS.xyz;
                posOS.y += sin(IN.positionOS.x * 2.0 + _Time.y) * 0.02;
                posOS.y += cos(IN.positionOS.z * 2.0 + _Time.y) * 0.02;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(posOS);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.positionWS = vertexInput.positionWS;
                OUT.uv = IN.uv * _NoiseScale;
                OUT.worldHeight = OUT.positionWS.y;
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 noiseUV1 = IN.uv + _Time.y * _NoiseSpeed;
                float2 noiseUV2 = IN.uv * 0.7 - _Time.y * _NoiseSpeed * 0.5;
                
                float fogNoise1 = fbm(noiseUV1);
                float fogNoise2 = fbm(noiseUV2);
                float fogNoise = (fogNoise1 + fogNoise2) * 0.5;

                float heightFactor = 1.0 - smoothstep(0.0, _HeightFalloff, IN.worldHeight);
                heightFactor = pow(heightFactor, 2.0);
                
                half alpha = _FogDensity * heightFactor * (fogNoise * 0.4 + 0.6);
                
                half4 finalColor = _FogColor;
                finalColor.a *= alpha;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}