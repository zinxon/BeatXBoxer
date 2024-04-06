Shader "Shader Demo/PhaseInAndOut" {
    Properties {
        _MainTex    ("主要材質 Main Texture", 2D) = "white"{}
        _Cutoff     ("透切閾值 Cutoff", Range(0, 1)) = 0.5
        
        _EdgeSpread ("Edge Spread", Range(0, 0.5)) = 0.1
        _SplitValue ("Split Value", Range(-1, 1)) = 0.1
        [HDR] _BaseCol    ("Base Color", Color) = (1, 1, 1, 1)
        [HDR] _Col ("Color", Color) = (1, 1, 1, 1)
        _Opacity    ("透明度 Opacity", range(0, 1)) = 0.5
    }

    SubShader {
        Tags {
            //需要調整渲染順序
            "Queue" = "AlphaTest"
            //如果需要使用 Alpha Test，渲染種類需要改為 TransparentCutout
            "RenderType"= "Transparent"
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

            Cull off
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma target 3.0

            //輸入参數
            uniform sampler2D _MainTex; uniform half4 _MainTex_ST;
            uniform half _Cutoff;

            uniform half _EdgeSpread;
            uniform half _SplitValue;
            uniform half4 _BaseCol;
            uniform half4 _Col;
            uniform half _Opacity;

            //輸入結構
            struct VertexInput {
                //輸入模型的頂點信息
                float4 vertex : POSITION;
                //輸入模型的紋理坐標
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            //輸出結構
            struct VertexOutput {
                //頂點位置(屏幕空間)：由模型的頂點信息換算而來
                float4 pos : SV_POSITION;
                float4 posWS : TEXCOORD0;
                float3 nDir : TEXCOORD1;
                //UV：用於存儲紋理坐標
                float2 uv : TEXCOORD2;
            };

            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }

            void Unity_Step_float(float Edge, float In, out float Out)
            {
                Out = step(Edge, In);
            }

            void Unity_InvertColors_float(float In, float InvertColors, out float Out)
            {
                Out = abs(InvertColors - In);
            }

            void Unity_OneMinus_float(float In, out float Out)
            {
                Out = 1 - In;
            }


            //頂點着色器
            VertexOutput vert (VertexInput v) {
                //新建一个輸出結構
                VertexOutput o = (VertexOutput)0;
                //變換頂點信息，OS > CS
                o.pos = UnityObjectToClipPos( v.vertex );
                o.posWS = mul(unity_ObjectToWorld, v.vertex);
                o.nDir = v.vertex;
                //提取UV信息，並支持Tilling Offset
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //將輸出結構輸出
                return o;
            }

            //像素着色器
            float4 frag(VertexOutput i) : COLOR {
                //float posY = i.nDir.y;
                float posY = i.nDir.y;
                float remapResult = 0;
                Unity_Remap_float(_EdgeSpread, float2(0, 1), float2(0, -0.5), remapResult);

                remapResult += _SplitValue;

                float smoothStepResult = 0;
                Unity_Smoothstep_float(posY, _SplitValue, remapResult, smoothStepResult);

                float result = 0;
                Unity_OneMinus_float(smoothStepResult, result);

                float4 col = _BaseCol * result;

                float stepResult = 0;
                Unity_Step_float(posY, _SplitValue, stepResult);

                float invertColResult = 0;
                Unity_InvertColors_float(stepResult, 1, invertColResult);


                //half4 var_MainTex = tex2D(_MainTex, i.uv) + col;

                half4 finalCol = col + _Col;
                half opacity = finalCol.a * _Opacity;

                clip(col.r - invertColResult);

                //輸出最終顏色
                return float4(finalCol.rgb *opacity, opacity);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
