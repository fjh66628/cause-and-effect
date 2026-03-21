Shader "Unlit/StarfieldBackground"
{
    Properties
    {
        _BackgroundColor ("Background", Color) = (0.05, 0.05, 0.1, 1)
        _Color1 ("Color1", Color) = (1, 1, 1, 1) 
        _Color2 ("Color2", Color) = (1, 0.9, 0.7, 1) 
        _Color3 ("Color3", Color) = (0.8, 0.9, 1.2, 1) 
        _StarDensity ("density", Range(0, 1)) = 0.6        // 每个格子出现星星的概率
        _GridSize ("size of grid", Range(5, 100)) = 20           // 网格数量，越大星星越多
        _BaseStarSize ("size", Range(0.01, 1)) = 0.8
        _TwinkleSpeed ("speed", Range(0.2, 5)) = 1.5
        _ColorVariation ("random range", Range(0, 1)) = 0.7 
        _MovingSpeed ("moving speed", Range(0, 0.01)) = 0.001 
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
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

            float4 _BackgroundColor;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float _StarDensity;
            float _GridSize;
            float _BaseStarSize;
            float _TwinkleSpeed;
            float _ColorVariation;
            float _MovingSpeed;

            // 伪随机函数（输入二维，输出一维）
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            // 二维随机（用于颜色等）
            float2 rand2(float2 co)
            {
                return float2(rand(co), rand(co + 123.456));
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
                i.uv += _MovingSpeed * _Time.y; // 让星空缓慢移动
                i.uv = frac(i.uv); // 保持在 [0,1] 范围内
                // 确定当前像素属于哪个网格
                float2 gridPos = i.uv * _GridSize;
                float2 cellIndex = floor(gridPos);           // 格子索引
                float2 cellLocal = frac(gridPos) - 0.5;       // 格子内坐标 [-0.5, 0.5]

                // 为每个格子生成随机种子
                float2 seed = cellIndex;
                float rnd = rand(seed);                       // 0~1 随机值

                // 根据密度决定该格子是否生成星星
                float hasStar = step(1.0 - _StarDensity, rnd); // 密度越大，星星越多

                // 随机星星大小（在基础大小附近浮动，且每颗星星不同）
                float sizeRand = rand(seed + 0.456);
                float starSize = _BaseStarSize * (0.7 + 0.6 * sizeRand);

                // 随机颜色
                float colorRnd = rand(seed + 1.234);
                float3 starColor;
                if (colorRnd < 0.4)
                    starColor = lerp(_Color1, _Color2, _ColorVariation);
                else if (colorRnd < 0.7)
                    starColor = lerp(_Color1, _Color3, _ColorVariation);
                else
                    starColor = _Color1;

                // 独立闪烁：每颗星星有自己的时间相位和频率
                float phase = rand(seed + 3.456) * 10.0;       // 相位偏移
                float freq = 0.8 + 0.5 * rand(seed + 7.890);   // 频率变化
                float twinkle = sin(_Time.y * _TwinkleSpeed * freq + phase);
                // 将正弦波映射到 0.3~1.0 之间（星星不会完全消失）
                float brightness = 0.5 + 0.5 * twinkle;        // 范围 0~1
                brightness = 0.3 + 0.7 * brightness;           // 范围 0.3~1.0

                // 大小随亮度轻微变化（亮时稍微大一点）
                float finalSize = starSize * (0.8 + 0.4 * brightness);

                // 绘制圆形（带抗锯齿）
                float dist = length(cellLocal);
                float circle = smoothstep(finalSize, finalSize * 0.5, dist); // 边缘柔和
                float alpha = circle * hasStar * brightness;   // 透明度随亮度变化

                // 最终颜色混合
                float3 finalRGB = lerp(_BackgroundColor.rgb, starColor, alpha);
                float finalAlpha = _BackgroundColor.a + alpha;

                return fixed4(finalRGB, finalAlpha);
            }
            ENDCG
        }
    }
}