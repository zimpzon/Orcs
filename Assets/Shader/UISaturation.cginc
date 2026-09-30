#ifndef UI_SATURATION_INCLUDED
#define UI_SATURATION_INCLUDED

// Global UI saturation, set from G.UISaturationBoost via Shader.SetGlobalFloat.
// 0 = unchanged (also what an unset global reads as, so edit mode looks normal), 0.25 = +25%, negative desaturates.
float _UISaturationBoost;

inline half3 UISaturateRgb(half3 rgb)
{
    half lum = dot(rgb, half3(0.299, 0.587, 0.114));
    return lerp(lum.xxx, rgb, 1.0 + _UISaturationBoost);
}

// For premultiplied-alpha output (Blend One OneMinusSrcAlpha): rgb must stay within [0, a].
inline half4 ApplyUISaturationPremul(half4 c)
{
    c.rgb = clamp(UISaturateRgb(c.rgb), 0, c.a);
    return c;
}

#endif
