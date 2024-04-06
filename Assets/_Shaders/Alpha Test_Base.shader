Shader "Shader Demo/Alpha Test/Base" {
    Properties {
        _MainTex    ("主要材質 Main Texture", 2D) = "white"{}
        [HDR] _Col        ("Color", Color) = (1, 1, 1, 1)
        _Cutoff     ("透切閾值 Cutoff", Range(0, 1)) = 0.5
    }

    SubShader {
        Tags {
            //需要調整渲染順序
            "Queue" = "AlphaTest"
            //如果需要使用 Alpha Test，渲染種類需要改為 TransparentCutout
            "RenderType"= "TransparentCutout"
            //為了實現透明效果，需要不響應投射器
            "IgnoreProjector" = "True"
            //關閉陰影投射
            "ForceNoShadowCasting" = "True"     
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma target 3.0

            //輸入参數
            uniform sampler2D _MainTex; uniform half4 _MainTex_ST;
            uniform half4 _Col;
            uniform half _Cutoff;

            //輸入結構
            struct VertexInput {
                //輸入模型的頂點信息
                float4 vertex : POSITION;
                //輸入模型的紋理坐標
                float2 uv : TEXCOORD0;
            };

            //輸出結構
            struct VertexOutput {
                //頂點位置(屏幕空間)：由模型的頂點信息換算而來
                float4 pos : SV_POSITION;
                //UV：用於存儲紋理坐標
                float2 uv : TEXCOORD2;
            };

            //頂點着色器
            VertexOutput vert (VertexInput v) {
                //新建一个輸出結構
                VertexOutput o = (VertexOutput)0;
                //變換頂點信息，OS > CS
                o.pos = UnityObjectToClipPos( v.vertex );
                //提取UV信息，並支持Tilling Offset
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //將輸出結構輸出
                return o;
            }

            //像素着色器
            float4 frag(VertexOutput i) : COLOR {
                //採樣貼圖
                half4 var_MainTex = tex2D(_MainTex, i.uv) * _Col;
                //進行透明剪切
                clip(var_MainTex.a - _Cutoff);

                //輸出最終顏色
                return var_MainTex;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
