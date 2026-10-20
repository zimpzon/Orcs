// Faint metaballs for UI backgrounds (credits dialog, UiMetaballs): blobs a touch lighter than the Image color, slowly
// merging and splitting, with a soft lighter rim. Transparent between the blobs, so the dialog panel shows through.
// UI-compatible: stencil (masks) and rect clipping like the default UI shader. _Aspect / _Phase come from UiMetaballs.
Shader "Custom/UIMetaballs" {
    Properties {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BallLift ("Ball brightness lift", Float) = 0.1
        _BallAlpha ("Ball opacity", Float) = 0.35
        _RimLift ("Rim brightness lift", Float) = 0.2
        _RimAlpha ("Rim opacity", Float) = 0.3
        _BallSize ("Ball size", Float) = 0.22
        _Threshold ("Threshold", Float) = 1.0
        _Aspect ("Aspect (width/height)", Float) = 1.5
        _Phase ("Phase (animated from code)", Float) = 0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _ClipRect;
            float _BallLift, _BallAlpha, _RimLift, _RimAlpha, _BallSize, _Threshold, _Aspect, _Phase;

            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos   : SV_POSITION;
                fixed4 col   : COLOR;
                float2 uv    : TEXCOORD0;
                float4 world : TEXCOORD1;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.world = v.vertex;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.col = v.color * _Color;
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 p = float2((i.uv.x - 0.5) * _Aspect, i.uv.y - 0.5);
                float halfW = max(_Aspect * 0.5 - 0.15, 0.1);
                float t = _Phase;

                float field = 0;
                float r2 = _BallSize * _BallSize;
                [unroll]
                for (int k = 0; k < 6; k++) {
                    float fk = k;
                    float2 c = float2(
                        sin(t * (0.53 + 0.11 * fk) + fk * 1.7) * halfW,
                        sin(t * (0.71 + 0.07 * fk) + fk * 2.3) * 0.38);
                    float2 d = p - c;
                    field += r2 / max(dot(d, d), 0.0001);
                }

                float w = 0.1;
                float inside = smoothstep(_Threshold - w, _Threshold + w, field);
                float rim = saturate(1 - abs(field - _Threshold) / (w * 3)) * (1 - inside * 0.6);

                // The dialog's own color, a bit lighter where the blobs are: same palette, just texture.
                fixed3 baseCol = i.col.rgb;
                fixed3 ballCol = saturate(baseCol + _BallLift);
                fixed3 rimCol = saturate(baseCol + _RimLift);
                fixed4 col;
                col.rgb = lerp(ballCol, rimCol, saturate(rim / max(inside + rim, 0.001)));
                col.a = (inside * _BallAlpha + rim * _RimAlpha) * i.col.a * tex2D(_MainTex, i.uv).a;

                #ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(i.world.xy, _ClipRect);
                #endif
                return col;
            }
            ENDCG
        }
    }
}
