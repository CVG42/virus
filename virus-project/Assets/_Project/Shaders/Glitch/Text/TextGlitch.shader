Shader "Custom/TMP/TextGlitch"
{
    Properties
    {
        _MainTex ("Font Atlas", 2D) = "white" {}
        _FaceColor ("Face Color", Color) = (1,1,1,1)

        _Softness ("Softness", Range(0.5, 4)) = 1

        _GlitchAmount ("Glitch Amount", Range(0, 40)) = 0
        _SliceAmount ("Slice Chance", Range(0, 1)) = 0.25
        _SliceFrequency ("Slice Frequency", Float) = 0.05
        _SliceSpeed ("Slice Speed", Float) = 25
        _Jitter ("Jitter", Range(0, 1)) = 1
        _ChromaticAmount ("Chromatic Amount", Range(0, 8)) = 0

        // UI Mask support
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float glitchDirection : TEXCOORD2;
            };

            sampler2D _MainTex;
            fixed4 _FaceColor;

            float _Softness;

            float _GlitchAmount;
            float _SliceAmount;
            float _SliceFrequency;
            float _SliceSpeed;
            float _Jitter;
            float _ChromaticAmount;

            float4 _ClipRect;

            float Hash(float n)
            {
                return frac(sin(n) * 43758.5453123);
            }

            v2f vert(appdata v)
            {
                v2f o;

                float time = _Time.y * _SliceSpeed;

                float row = floor((v.vertex.y + time) * _SliceFrequency);
                float rowNoise = Hash(row + floor(_Time.y * 16.0));

                float sliceGate = step(1.0 - _SliceAmount, rowNoise);

                float direction = Hash(row + 3.17) * 2.0 - 1.0;
                float offset = direction * _GlitchAmount * sliceGate;

                float jitterRow = floor(v.vertex.y * (_SliceFrequency * 0.35) + floor(_Time.y * 20.0));
                float jitter = (Hash(jitterRow) * 2.0 - 1.0) * _GlitchAmount * 0.25 * _Jitter;

                offset += jitter * sliceGate;

                v.vertex.x += offset;

                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _FaceColor;
                o.glitchDirection = direction * sliceGate;

                return o;
            }

            float GetSDFAlpha(float2 uv)
            {
                float distance = tex2D(_MainTex, uv).a;

                float width = max(fwidth(distance), 0.0001) * _Softness;

                return smoothstep(
                    0.5 - width,
                    0.5 + width,
                    distance
                );
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float chromaticOffset = _ChromaticAmount * 0.0015;
                float2 rgbOffset = float2(chromaticOffset * i.glitchDirection, 0);

                float alphaR = GetSDFAlpha(i.uv + rgbOffset);
                float alphaG = GetSDFAlpha(i.uv);
                float alphaB = GetSDFAlpha(i.uv - rgbOffset);

                fixed4 color = i.color;

                float finalAlpha = max(max(alphaR, alphaG), alphaB) * color.a;

                float3 finalRGB = color.rgb * float3(alphaR, alphaG, alphaB);

                #ifdef UNITY_UI_CLIP_RECT
                finalAlpha *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(finalAlpha - 0.001);
                #endif

                return fixed4(finalRGB, finalAlpha);
            }

            ENDCG
        }
    }
}
