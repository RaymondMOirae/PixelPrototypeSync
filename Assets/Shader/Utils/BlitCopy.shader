Shader "Prototype/Utils/BlitCopy" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
    }

    HLSLINCLUDE

    #include "UnityCG.cginc"

    struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    v2f vert(appdata_img i) {
        v2f o;
        o.pos = UnityObjectToClipPos(i.vertex);
        o.uv = i.texcoord;
        return o;
    }

    sampler2D _MainTex;

    float4 frag(v2f i) : SV_TARGET {
        return tex2D(_MainTex, i.uv.xy);
    }

    ENDHLSL

    SubShader {
        // #0 BlitCopy without alpha blending
        Pass {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            ENDHLSL
        }
        
        // #1 BlitCopy with alpha blending
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            ENDHLSL
        }
    }
}