Shader "Shader Demo/Fire/Noise" {
    Properties {
        _Mask           ("R:外焰 G:内焰 B:透貼", 2d) = "blue"{}
        _Noise          ("R:噪聲1 G:噪聲2", 2d) = "gray"{}
        _Noise1Params   ("噪聲1 X:大小 Y:流速 Z:强度", vector) = (1.0, 0.2, 0.2, 1.0)
        _Noise2Params   ("噪聲2 X:大小 Y:流速 Z:强度", vector) = (1.0, 0.2, 0.2, 1.0)
        [HDR]_Color1    ("Outside Flame Color", color) = (1,1,1,1)
        [HDR]_Color2    ("Inside Flame Color", color) = (1,1,1,1)
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
            uniform sampler2D _Mask;    uniform float4 _Mask_ST;
            uniform sampler2D _Noise;
            uniform half3 _Noise1Params;
            uniform half3 _Noise2Params;
            uniform half3 _Color1;
            uniform half3 _Color2;

            //輸入結構
            struct VertexInput {
                //輸入模型的頂點信息
                float4 vertex : POSITION;
                //輸入模型的紋理坐標
                float2 uv : TEXCOORD0;
            };


            // 輸出結構
            struct VertexOutput {
                //頂點位置(屏幕空間)：由模型的頂點信息換算而來
                float4 pos : SV_POSITION;
                //UV信息，採樣 Mask
                float2 uv0 : TEXCOORD0; 
                //UV信息，採樣 Noise1        
                float2 uv1 : TEXCOORD1;
                //UV信息，採樣 Noise2
                float2 uv2 : TEXCOORD2;
            };

            //頂點着色器
            VertexOutput vert (VertexInput v) {
                //新建一个輸出結構
                VertexOutput o = (VertexOutput)0;
                //變換頂點信息，OS > CS
                o.pos = UnityObjectToClipPos( v.vertex );
                 //提取UV信息
                o.uv0 = TRANSFORM_TEX(v.uv, _Mask);
                //提取UV信息，並控制 UV1 的噪聲速度
                o.uv1 = o.uv0 * _Noise1Params.x + float2(0.0, frac(-_Time.x * _Noise1Params.y));
                //提取UV信息，並控制 UV2 的噪聲速度
                o.uv2 = o.uv0 * _Noise2Params.x + float2(0.0, frac(-_Time.x * _Noise2Params.y));
                return o;
            }

            //像素着色器
            half4 frag(VertexOutput i) : COLOR {
                //採樣擾動遮罩
                half warpMask = tex2D(_Mask, i.uv0).b;

                //採樣噪聲1
                half var_Noise1 = tex2D(_Noise, i.uv1).r;
                //採樣噪聲2
                half var_Noise2 = tex2D(_Noise, i.uv2).g;
                //進行噪聲混合
                half noise = var_Noise1 * _Noise1Params.z + var_Noise2 * _Noise2Params.z;
                
                //擾動UV: 完整的uv - noise UV = mask UV
                float2 warpUV = i.uv0 - float2(0.0, noise) * warpMask;
                //採樣 Mask
                half3 var_Mask = tex2D(_Mask, warpUV);
                //設置 Opacity
                half opacity = var_Mask.r + var_Mask.g;
                //計算最終結果
                half3 finalRGB = _Color1 * var_Mask.r + _Color2 * var_Mask.g;
                
                //輸出最終顏色
                return half4(finalRGB, opacity);
            }
            ENDCG
        }
    }
}
