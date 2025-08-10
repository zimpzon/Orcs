Shader "Custom/SpriteGradientVertical" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorTop ("Top Color", Color) = (1,1,1,1)
        _ColorBottom ("Bottom Color", Color) = (1,1,1,1)
        _Scale ("Scale", Float) = 1
    }

    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "CanUseSpriteAtlas"="True" 
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _ColorTop;
            fixed4 _ColorBottom;
            fixed _Scale;

            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos  : SV_POSITION;
                fixed4 col  : COLOR;
                float2 uv   : TEXCOORD0;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // UV.y = 1 at top, 0 at bottom
                float t = saturate((1.0 - v.texcoord.y) * _Scale);
                o.col = lerp(_ColorTop, _ColorBottom, t) * v.color;

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                return tex * i.col;
            }
            ENDCG
        }
    }
}
