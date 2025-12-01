Shader "Custom/Matrix"
{
    Properties
    {
        _Speed ("Velocity", Range(0, 10)) = 3
        _Density ("Density", Range(0, 1)) = 0.6
        [HDR] _Color ("Color", Color) = (0, 2, 0, 1)
        _CharSize ("Character size", Range(0.001, 0.2)) = 0.08
        _Flicker ("Flicker", Range(0, 1)) = 0.2
    }
    
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
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
                UNITY_FOG_COORDS(1)
            };
            
            float _Speed;
            float _Density;
            float4 _Color;
            float _CharSize;
            float _Flicker;
            
            // Función hash para números pseudoaleatorios
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }
            
            // Determinar si es 0 o 1 basado en posición y tiempo
            float getBinaryDigit(float2 gridPos, float time)
            {
                // Usar posición y tiempo para generar 0 o 1
                float seed = hash(float2(gridPos.x * 0.1, floor(time * 2 + gridPos.y * 0.1)));
                return floor(seed + 0.5); // 0 o 1
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _Speed;
                
                // Crear grid para caracteres
                float2 gridSize = float2(1.0 / _CharSize, 1.0 / (_CharSize * 2));
                float2 gridPos = floor(i.uv * gridSize);
                float2 cellUV = frac(i.uv * gridSize);
                
                // Centro del carácter
                float2 charCenter = abs(cellUV - 0.5);
                float inChar = step(max(charCenter.x, charCenter.y * 1.5), 0.45);
                
                // Desfase por columna para efecto de lluvia
                float columnSeed = hash(float2(gridPos.x, 0));
                float dropSpeed = columnSeed * 0.5 + 0.5;
                float dropOffset = columnSeed * 100.0;
                
                // Posición en la corriente de lluvia
                float rainStream = frac(gridPos.y / 30.0 - time * dropSpeed + dropOffset);
                
                // Brillar solo si está en la corriente activa
                float inStream = step(rainStream, 0.3);
                
                // Obtener dígito binario (0 o 1)
                float binaryDigit = getBinaryDigit(gridPos, time + dropOffset);
                
                // Crear el carácter "1" o "0"
                float charShape = 0;
                
                if (binaryDigit > 0.5) // Es un "1"
                {
                    // Un "1" es una línea vertical
                    charShape = step(abs(cellUV.x - 0.5), 0.15);
                }
                else // Es un "0"
                {
                    // Un "0" es un círculo o anillo
                    float radius = length(cellUV - 0.5);
                    charShape = smoothstep(0.4, 0.35, radius) - 
                               smoothstep(0.25, 0.2, radius);
                }
                
                // Aplicar densidad
                float densityCheck = hash(float2(gridPos.x, 123.456));
                if (densityCheck > _Density) inStream = 0;
                
                // Efecto de parpadeo aleatorio
                float flicker = hash(float2(gridPos.x + time * 0.1, gridPos.y));
                float flickerEffect = 1.0 - _Flicker + flicker * _Flicker;
                
                // Intensidad final (alpha)
                float alpha = charShape * inChar * inStream * flickerEffect;
                
                // Color con brillo variable en el rastro
                float trailBrightness = 1.0 - rainStream * 2.0;
                
                // Color final usando HDR
                fixed4 col = _Color;
                col.rgb *= alpha * trailBrightness; // Aplicar intensidad al color HDR
                col.a = alpha; // Alpha controla la transparencia
                
                // Aplicar fog si está habilitado
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
