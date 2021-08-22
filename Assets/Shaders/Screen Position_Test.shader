Shader "Shader Demo/Test" {
    Properties {
        _MainTex        ("主要材質 Main Texture", 2D) = "white"{}
        _MainCol        ("Main Color",  Color) = (1, 1, 1, 1)
        _Opacity        ("透明度 Opacity", range(0, 1)) = 0.5
        _WarpMidValue   ("擾動中間值 Warp Middle Value", Range(0, 1)) = 0.5
        _WarpInt        ("擾動強度 Warp Intensity", Range(0, 5)) = 1
    }

    SubShader {
        Tags {
            //需要調整渲染順序
            "Queue" = "Transparent"
            //為了實現透明效果，渲染種類需要改為 Transparent
            "RenderType"= "Transparent"
            //為了實現透明效果，需要不響應投射器
            "IgnoreProjector" = "True"
            //關閉陰影投射
            "ForceNoShadowCasting" = "True"   
        }

        //獲取背景紋理
        GrabPass{
            "_BGTex"
        }

        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }

            //使用混合效果
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma target 3.0

            //輸入参數
            uniform sampler2D _MainTex; uniform half4 _MainTex_ST;
            uniform half4 _MainCol;
            uniform half _Opacity;
            uniform half _WarpMidValue;
            uniform half _WarpInt;
            //獲取背景紋理
            uniform sampler2D _BGTex;

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
                float2 uv : TEXCOORD0;
                //背景紋理採樣坐標
                float4 grabPos : TEXCOORD1;
            };

            //頂點着色器
            VertexOutput vert (VertexInput v) {
                //新建一个輸出結構
                VertexOutput o = (VertexOutput)0;
                //變換頂點信息，OS > CS
                o.pos = UnityObjectToClipPos( v.vertex );
                //提取UV信息，並支持Tilling Offset
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //提取背景紋理UV坐標
                o.grabPos = ComputeGrabScreenPos(o.pos);
                //返回結構輸出
                return o;
            }

            //像素着色器
            float4 frag(VertexOutput i) : COLOR {
                //採樣貼圖紋理
                //half4 var_MainTex = tex2D(_MainTex, i.uv);
                half4 var_MainTex = _MainCol;

                //使用背景紋理UV坐標來調整擾動強度
                //i.grabPos.xy += (var_MainTex.b - _WarpMidValue) * _WarpInt * _Opacity;
                //使用背景紋理UV坐標來調整擾動強度
                i.grabPos.xy += (var_MainTex.b - _WarpMidValue) * _WarpInt;
                //採樣背景紋理
                half3 var_BGTex = tex2Dproj(_BGTex, i.grabPos).rgb;

                //設置 Opacity
                half opacity = var_MainTex.a;
                //計算最終結果
                half3 finalRGB = lerp(1.0, var_MainTex.rgb, _Opacity) * var_BGTex;
                
                //輸出最終顏色
                return half4(finalRGB * opacity, opacity);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}