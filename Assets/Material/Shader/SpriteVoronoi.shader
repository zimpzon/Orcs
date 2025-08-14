Shader "Custom/SpritePlasma" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorA ("Color A", Color) = (1,0,0,1)
        _ColorB ("Color B", Color) = (0,0,1,1)
        _Scale ("Scale", Float) = 5
        _AnimSpeed ("Animation Speed", Float) = 1
        _AnimScale ("Animation Amount", Float) = 1
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
            fixed4 _ColorA;
            fixed4 _ColorB;
            fixed _Scale;
            fixed _AnimSpeed;
            fixed _AnimScale;

            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos  : SV_POSITION;
                fixed4 col  : COLOR;
                float2 uv   : TEXCOORD0;
                float2 worldUV : TEXCOORD1;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.col = v.color;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldUV = v.texcoord * _Scale;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                float t = _Time.y * _AnimSpeed;

                // Base coordinates
                float2 p = i.worldUV;

                // Classic plasma function
                float v = 0.0;
                v += sin(p.x + t);
                v += sin(p.y + t);
                v += sin(p.x + p.y + t);
                v += sin(sqrt(p.x * p.x + p.y * p.y) + t);

                // Normalize to 0..1
                v = (v / 4.0 + 1.0) * 0.5;

                // Add animation scale as contrast
                v = pow(v, 1.0 - _AnimScale * 0.9);

                fixed4 plasmaColor = lerp(_ColorA, _ColorB, v);

                return fixed4(plasmaColor.rgb, tex.a) * i.col;
            }
            ENDCG
        }
    }
}
