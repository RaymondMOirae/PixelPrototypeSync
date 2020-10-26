Shader "Prototype/Editor/CanvasGridCode" {
    Properties {}

    HLSLINCLUDE
    
    #include "UnityCG.cginc"

    struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float2 pixelCoord : TEXCOORD1;
        float4 color : TEXCOORD2;
    };

    

    ENDHLSL

    SubShader {

        // #0 Render grid
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha, One One
            BlendOp Add, Max
            HLSLPROGRAM


            #pragma vertex vert
            #pragma fragment frag



            float4 RenderSize; // (w, h, 1/w, 1/h)
            float2 GridSize;
            float4 Color;

            v2f vert(appdata_full i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = i.texcoord;
                o.pixelCoord = i.texcoord * RenderSize.xy;
                o.color = i.color;
                return o;
            }

            float sampleAt(float2 pixelCoord)
            {
                float2 nearestGridPos = round(pixelCoord / GridSize) * GridSize;
                float2 delta = abs(pixelCoord - nearestGridPos);
                return saturate(1 - step(.5, min(delta.x, delta.y)));
            }

            float4 frag(v2f i) : SV_TARGET
            {
                float2 delta = float2(-0.25, 0.25);
                float samples = sampleAt(i.pixelCoord.xy + delta.xx)
                    + sampleAt(i.pixelCoord.xy + delta.yx)
                    + sampleAt(i.pixelCoord.xy + delta.yy)
                    + sampleAt(i.pixelCoord.xy + delta.xy);

                float4 color = Color;
                color.a = samples / 4;
                return color;
            }

            ENDHLSL
        }

        // #1 Pixel image rendering
        Pass {
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            
            float4 RenderSize; // (w, h, 1/w, 1/h)

            v2f vert(appdata_full i)
            {
                v2f o;
                o.pos = float4(i.vertex.xyz * 2, 1);
                o.pos.y *= _ProjectionParams.x;
                o.uv = i.texcoord;
                o.pixelCoord = i.texcoord * RenderSize.xy;
                o.color = i.color;
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                return tex2D(_MainTex, i.uv) * i.color.a;
            }

            ENDHLSL
        }

        // #2 Damage Visualization
        Pass {
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            
            float4 RenderSize; // (w, h, 1/w, 1/h)

            v2f vert(appdata_full i)
            {
                v2f o;
                o.pos = float4(i.vertex.xyz * 2, 1);
                o.pos.y *= _ProjectionParams.x;
                o.uv = i.texcoord;
                o.pixelCoord = i.texcoord * RenderSize.xy;
                o.color = i.color;
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                float4 color = i.color;
                // color.rgb /= 8;
                color.a *= .3f;
                return color;
            }

            ENDHLSL

        }

    }
}
