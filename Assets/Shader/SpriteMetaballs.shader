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
        _BorderColor ("Border comets", Color) = (1,0.84,0.35,1)
        _BorderStrength ("Border comets strength (0 = off)", Float) = 0
        _BorderPhase ("Border comets position (animated from code)", Float) = 0
        _BorderInset ("Border inset", Float) = 0.1
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
            fixed4 _BorderColor;
            float _BorderStrength;
            float _BorderPhase;
            float _BorderInset;

            // Two comets running around a rectangle inset from the panel edge, with fading tails and a faint glow line
            // along the whole border (StoryPanelFx turns it on once Scary Earl is defeated). Returns the brightness.
            float BorderComets(float2 p, float aspect)
            {
                float hw = max(aspect * 0.5 - _BorderInset, 0.05);
                float hh = max(0.5 - _BorderInset, 0.05);
                // Distance to the rectangle outline.
                float2 q = abs(p) - float2(hw, hh);
                float dist = abs(length(max(q, 0)) + min(max(q.x, q.y), 0));
                float outline = exp(-dist * dist / 0.0006);

                // Position along the outline, counter-clockwise from the bottom-left corner.
                float W = hw * 2, H = hh * 2, L = 2 * (W + H);
                float dx = hw - abs(p.x), dy = hh - abs(p.y);
                float s;
                if (dy <= dx)
                    s = p.y < 0 ? (p.x + hw) : (W + H + (hw - p.x));        // bottom / top edge
                else
                    s = p.x > 0 ? (W + (p.y + hh)) : (2 * W + H + (hh - p.y)); // right / left edge

                float comet = 0;
                [unroll]
                for (int c = 0; c < 2; c++) {
                    float head = frac(_BorderPhase + c * 0.5) * L;
                    float behind = fmod(head - s + L, L);                   // how far behind the head
                    comet += exp(-behind / (L * 0.06)) * (behind < L * 0.5); // tail
                    comet += exp(-(L - behind) * (L - behind) / 0.0004);     // a little bright tip just ahead
                }
                return outline * (0.15 + comet);
            }

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

                col.a *= edge;

                // Border comets on top (not faded by the soft edges, they sit inside them).
                float glow = BorderComets(p, _Aspect) * _BorderStrength;
                col.rgb += _BorderColor.rgb * glow;
                col.a = saturate(col.a + glow * _BorderColor.a);

                col.a *= i.col.a * tex2D(_MainTex, i.uv).a;
                col.rgb *= i.col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
