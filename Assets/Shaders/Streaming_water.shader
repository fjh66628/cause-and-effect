Shader "Unlit/Streaming_end_Transparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Range(0, 1)) = 0.5
        _Alpha ("Alpha", Range(0, 1)) = 1.0       // 可选：全局透明度控制
    }
    SubShader
    {
        // 将渲染队列改为 Transparent，并关闭深度写入，确保透明混合正确
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            // 标准 Alpha 混合：源颜色（当前片段）的 Alpha 与背景混合
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off   // 避免透明物体相互遮挡产生的深度问题

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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            float _Alpha;   // 可选属性

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 滚动 UV
                i.uv -= _Speed * _Time.y;
                i.uv = frac(i.uv);   // 实现无限循环滚动

                // 采样纹理
                fixed4 col = tex2D(_MainTex, i.uv);

                // 应用全局透明度（若不需要可删除此乘，直接使用 col.a）
                col.a *= _Alpha;

                // 应用雾效（雾效不影响透明度，但会与混合后的颜色叠加）
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;   // 输出带透明度的颜色
            }
            ENDCG
        }
    }
}