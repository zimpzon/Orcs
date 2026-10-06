using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Celebration on the generic (blue) message popup for rewards (chest, mystery): the popup springs in and golden god
// rays turn slowly behind the header line, same look as the victory dialog's (VictoryCelebration): inside the panel
// (clipped), behind the text. Added in code by GameCanvasScript.ShowPopup(..., celebrate: true). Unscaled time.
public class RewardPopupFx : MonoBehaviour
{
    // Pop-in
    const float PopDuration = 0.35f;
    const float PopFromScale = 0.6f;

    // Rays: same values as VictoryCelebration.
    const float RaySpeed = 12f;          // degrees per second
    const float RayAlpha = 0.25f;
    const float RaySizeMul = 1.3f;       // x header width

    static readonly Color Gold = new(1f, 0.835f, 0.29f);
    static Sprite _raySprite;

    RectTransform _popup, _panel, _back, _rays;
    TextMeshProUGUI _text;
    float _start;
    bool _logged;

    void Awake()
    {
        _popup = (RectTransform)transform;
        _popup.localScale = Vector3.one * PopFromScale; // right away, so it never shows a frame at full size
    }

    void Start()
    {
        _start = Time.unscaledTime;
        _text = GetComponentInChildren<TextMeshProUGUI>();

        // The prefab root is a fixed box; the visible panel is "Background" (sized to the text).
        _panel = _popup;
        foreach (var rt in GetComponentsInChildren<RectTransform>(true))
        {
            if (rt.name == "Background")
            {
                _panel = rt;
                break;
            }
        }

        // Clipping layer over the panel, first child: on the panel's background, behind the text. The panel has a
        // layout group, so the layer opts out of layout.
        var go = new GameObject("RewardFxBack", typeof(RectTransform));
        go.transform.SetParent(_panel, false);
        go.transform.SetAsFirstSibling();
        go.AddComponent<LayoutElement>().ignoreLayout = true;
        go.AddComponent<RectMask2D>();
        _back = (RectTransform)go.transform;
        _back.anchorMin = Vector2.zero;
        _back.anchorMax = Vector2.one;
        _back.offsetMin = _back.offsetMax = Vector2.zero;

        var rays = new GameObject("Rays", typeof(RectTransform));
        rays.transform.SetParent(_back, false);
        var img = rays.AddComponent<Image>();
        img.sprite = RaySprite();
        img.color = new Color(Gold.r, Gold.g, Gold.b, RayAlpha);
        img.raycastTarget = false;
        _rays = img.rectTransform;
        _rays.anchorMin = _rays.anchorMax = _rays.pivot = new Vector2(0.5f, 0.5f);
        _rays.sizeDelta = Vector2.one * 100;
    }

    void LateUpdate()
    {
        float t = Time.unscaledTime - _start;

        // Spring in (ease out back).
        float p = Mathf.Clamp01(t / PopDuration);
        const float c1 = 1.9f, c3 = c1 + 1f;
        float ease = 1f + c3 * Mathf.Pow(p - 1f, 3) + c1 * Mathf.Pow(p - 1f, 2);
        _popup.localScale = Vector3.one * Mathf.LerpUnclamped(PopFromScale, 1f, ease);

        if (_rays == null)
            return;

        // Centered on the header (first line), sized from its width, like the victory dialog.
        Vector3 center = Vector3.zero;
        float headerWidth = 0f;
        if (_text != null && HeaderBounds(out var min, out var max))
        {
            center = _back.InverseTransformPoint(_text.rectTransform.TransformPoint((min + max) * 0.5f));
            headerWidth = max.x - min.x;
        }

        float size = Mathf.Max(60f, headerWidth * RaySizeMul) * (1f + 0.05f * Mathf.Sin(t * 1.3f));
        _rays.sizeDelta = new Vector2(size, size);
        _rays.localPosition = center;
        _rays.localRotation = Quaternion.Euler(0, 0, -t * RaySpeed);

        // Temporary diagnostics: the effect didn't show up in testing; this tells us whether it runs and where.
        if (!_logged && t > 0.5f)
        {
            _logged = true;
            Debug.Log($"RewardPopupFx: panel '{_panel.name}' rect {_panel.rect}, rays at {_rays.localPosition} size {size:0}, " +
                      $"header width {headerWidth:0}, back rect {_back.rect}, popup scale {_popup.localScale.x:0.00}");
        }
    }

    bool HeaderBounds(out Vector3 min, out Vector3 max)
    {
        min = max = Vector3.zero;
        var info = _text.textInfo;
        if (info == null || info.lineCount == 0)
            return false;

        var line = info.lineInfo[0];
        bool any = false;
        for (int c = line.firstCharacterIndex; c <= line.lastCharacterIndex && c < info.characterCount; ++c)
        {
            var ch = info.characterInfo[c];
            if (!ch.isVisible)
                continue;
            min = any ? Vector3.Min(min, ch.bottomLeft) : ch.bottomLeft;
            max = any ? Vector3.Max(max, ch.topRight) : ch.topRight;
            any = true;
        }
        return any;
    }

    // Soft 12-wedge sunburst, alpha fading outward (same as VictoryCelebration's). Unity null check: the runtime
    // texture is destroyed when play mode ends.
    static Sprite RaySprite()
    {
        if (_raySprite != null)
            return _raySprite;

        const int size = 128;
        const int wedges = 12;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        var px = new Color32[size * size];
        for (int y = 0; y < size; ++y)
        {
            for (int x = 0; x < size; ++x)
            {
                float dx = (x + 0.5f) / size * 2f - 1f, dy = (y + 0.5f) / size * 2f - 1f;
                float r = Mathf.Sqrt(dx * dx + dy * dy);
                float ang = Mathf.Atan2(dy, dx);
                float wedge = 0.5f + 0.5f * Mathf.Cos(ang * wedges);
                float ray = Mathf.SmoothStep(0.35f, 1f, wedge);
                float fade = Mathf.Clamp01(1f - r) * Mathf.Clamp01(r * 4f);
                px[y * size + x] = new Color32(255, 255, 255, (byte)(255 * ray * fade));
            }
        }
        tex.SetPixels32(px);
        tex.Apply();
        _raySprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        return _raySprite;
    }
}
