// Slowly oozing metaballs on a sprite (the Scary Earl story panel under the arena HP bar, StoryPanelFx sets the colors).
// Blobs merge and split on Lissajous paths; a soft glowing rim where the field crosses the threshold. The panel fades
// out toward its edges. _Aspect (width / height, set from code) keeps the blobs round on a wide sprite.
Shader "Custom/SpriteMetaballs" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _BgColor ("Background", Color) = (0,0,0,0.35)
        _BallColor ("Balls", Color) = (0.25,0.05,0.35,0.55)
        _RimColor ("Rim glow", Color) = (0.75,0.3,1,0.6)
        _Phase ("Phase (animated from code)", Float) = 0
        _Threshold ("Threshold", Float) = 1.0
        _BallSize ("Ball size", Float) = 0.28
        _Aspect ("Aspect (width/height)", Float) = 8
        _EdgeFade ("Edge fade", Float) = 0.12
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
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _BgColor;
            fixed4 _BallColor;
            fixed4 _RimColor;
            float _Phase;
            float _Threshold;
            float _BallSize;
            float _Aspect;
            float _EdgeFade;

            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 col : COLOR;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.col = v.color;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Panel space: x in [-aspect/2, aspect/2], y in [-0.5, 0.5].
                float2 p = float2((i.uv.x - 0.5) * _Aspect, i.uv.y - 0.5);
                float halfW = max(_Aspect * 0.5 - 0.3, 0.1);
                float t = _Phase; // time x speed, integrated in StoryPanelFx so speed changes don't jump

                float field = 0;
                float r2 = _BallSize * _BallSize;
                [unroll]
                for (int k = 0; k < 6; k++) {
                    float fk = k;
                    // Each ball: its own frequencies and phase, spread over the panel's width.
                    float2 c = float2(
                        sin(t * (0.53 + 0.11 * fk) + fk * 1.7) * halfW,
                        sin(t * (0.71 + 0.07 * fk) + fk * 2.3) * 0.28);
                    float2 d = p - c;
                    field += r2 / max(dot(d, d), 0.0001);
                }

                float w = 0.08;
                float inside = smoothstep(_Threshold - w, _Threshold + w, field);
                float rim = saturate(1 - abs(field - _Threshold) / (w * 3)) * (1 - inside * 0.6);

                fixed4 col = _BgColor;
                col.rgb = lerp(col.rgb, _BallColor.rgb, inside);
                col.a = lerp(col.a, _BallColor.a, inside);
                col.rgb += _RimColor.rgb * rim * _RimColor.a;
                col.a = saturate(col.a + rim * _RimColor.a * 0.5);

                // Soft edges all around, so the panel blends into the arena.
                float2 e = float2(_EdgeFade / max(_Aspect, 0.01), _EdgeFade);
                float edge = smoothstep(0, e.x, i.uv.x) * smoothstep(0, e.x, 1 - i.uv.x)
                           * smoothstep(0, e.y, i.uv.y) * smoothstep(0, e.y, 1 - i.uv.y);

                col.a *= edge * i.col.a * tex2D(_MainTex, i.uv).a;
                col.rgb *= i.col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
