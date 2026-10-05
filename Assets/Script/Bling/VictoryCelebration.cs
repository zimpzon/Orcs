using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Celebration on the victory dialog (PopupProgress) once the game is 100% complete (Completion100), or while the
// RightShift+G preview is on (CrownPulse.CheatForceShow):
// - slowly rotating golden light rays behind the "GLORIOUS VICTORY" header,
// - a shine sweeping across the header, which also breathes slightly,
// - crown fireworks: spark bursts with a popping crown, near the top,
// - confetti drifting down over the dialog.
// Everything is built in code on first use, ignores clicks, and runs on unscaled time. Tune in the Inspector.
[RequireComponent(typeof(ProgressPopupScript))]
public class VictoryCelebration : MonoBehaviour
{
    [Header("Light rays")]
    public bool Rays = true;
    public float RaySpeed = 12f;               // degrees per second
    [Range(0, 1)] public float RayAlpha = 0.25f;
    public float RaySizeMul = 1.3f;            // x header width

    [Header("Header shimmer")]
    public bool Shimmer = true;
    public float ShimmerInterval = 2.5f;
    public float ShimmerDuration = 0.6f;
    public float ShimmerWidth = 0.18f;         // fraction of the header width
    public float BreatheAmount = 0.03f;

    [Header("Crown fireworks")]
    public bool Fireworks = true;
    public float FireworkInterval = 2.2f;
    public int SparkCount = 16;
    public float SparkSpeed = 90f;
    public float SparkLife = 0.9f;
    public float FireworkCrownSize = 22f;

    [Header("Confetti")]
    public bool Confetti = true;
    public int ConfettiCount = 35;
    public Vector2 ConfettiFallSpeed = new(25f, 55f);
    [Range(0, 1)] public float ConfettiAlpha = 0.85f;
    public Color[] ConfettiColors =
    {
        new(1f, 0.835f, 0.29f),       // gold  #FFD54A
        new(0.553f, 0.745f, 0.298f),  // green #8DBE4C
        Color.white,
    };

    static readonly Color Gold = new(1f, 0.835f, 0.29f);

    ProgressPopupScript _popup;
    TextMeshProUGUI _text;
    RectTransform _rect, _back, _front, _rays;
    bool _built, _active;
    Vector3 _headerCenter;   // in popup-local space
    float _headerWidth;
    float _nextFirework;
    static Sprite _raySprite;

    class Confetto { public RectTransform Rt; public float Speed, SwayPhase, SwayAmount, Spin; }
    readonly List<Confetto> _confetti = new();

    class Spark { public RectTransform Rt; public Image Img; public Vector2 Pos, Vel; public float Age = 999f; }
    readonly List<Spark> _sparks = new();
    Image _fwCrown;
    float _fwCrownAge = 999f;

    void Awake()
    {
        _popup = GetComponent<ProgressPopupScript>();
        _text = _popup.ProgressText;
        _rect = (RectTransform)transform;
    }

    static bool ShouldShow()
        => CrownPulse.CheatForceShow || SaveGame.Members.Achieved.Contains(Achieved.Completion100);

    void LateUpdate()
    {
        bool show = ShouldShow();
        if (show && !_built)
            Build();
        if (show != _active && _built)
        {
            _active = show;
            _back.gameObject.SetActive(show);
            _front.gameObject.SetActive(show);
        }
        if (!show)
            return;

        float dt = Time.unscaledDeltaTime;
        float t = Time.unscaledTime;
        UpdateHeaderAndShimmer(t);
        UpdateRays(t);
        UpdateFireworks(t, dt);
        UpdateConfetti(t, dt);
    }

    // ---------------------------------------------------------------- build

    RectTransform Container(string name, int siblingIndex, bool clip)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(transform, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        if (clip)
            go.AddComponent<RectMask2D>();
        if (siblingIndex >= 0)
            go.transform.SetSiblingIndex(siblingIndex);
        else
            go.transform.SetAsLastSibling();
        return rt;
    }

    static Image NewImage(Transform parent, string name, Sprite sprite, Color color, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.raycastTarget = false;
        var rt = img.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        return img;
    }

    void Build()
    {
        _built = true;
        _back = Container("VictoryFxBack", 0, clip: true);
        _front = Container("VictoryFxFront", -1, clip: false);

        _rays = NewImage(_back, "Rays", RaySprite(), new Color(Gold.r, Gold.g, Gold.b, RayAlpha), Vector2.one * 100).rectTransform;

        for (int i = 0; i < ConfettiCount; ++i)
        {
            var c = ConfettiColors.Length > 0 ? ConfettiColors[Random.Range(0, ConfettiColors.Length)] : Color.white;
            var img = NewImage(_back, "Confetto", null, new Color(c.r, c.g, c.b, ConfettiAlpha), new Vector2(4, 7));
            var conf = new Confetto
            {
                Rt = img.rectTransform,
                Speed = Random.Range(ConfettiFallSpeed.x, ConfettiFallSpeed.y),
                SwayPhase = Random.value * Mathf.PI * 2,
                SwayAmount = Random.Range(4f, 12f),
                Spin = Random.Range(-220f, 220f),
            };
            ResetConfetto(conf, randomY: true);
            _confetti.Add(conf);
        }

        for (int i = 0; i < SparkCount; ++i)
        {
            var img = NewImage(_front, "Spark", null, Gold, new Vector2(3, 3));
            img.gameObject.SetActive(false);
            _sparks.Add(new Spark { Rt = img.rectTransform, Img = img });
        }
        _fwCrown = NewImage(_front, "FireworkCrown", _popup.CrownSprite, Color.white, Vector2.one * FireworkCrownSize);
        _fwCrown.preserveAspect = true;
        _fwCrown.gameObject.SetActive(false);
        _nextFirework = Time.unscaledTime + 0.4f;
    }

    // Soft 12-wedge sunburst, alpha fading outward. Generated once.
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
                float wedge = 0.5f + 0.5f * Mathf.Cos(ang * wedges);       // 1 in the middle of a ray
                float ray = Mathf.SmoothStep(0.35f, 1f, wedge);
                float fade = Mathf.Clamp01(1f - r) * Mathf.Clamp01(r * 4f);  // fade out to the edge, soft center
                byte a = (byte)(255 * ray * fade);
                px[y * size + x] = new Color32(255, 255, 255, a);
            }
        }
        tex.SetPixels32(px);
        tex.Apply();
        _raySprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        return _raySprite;
    }

    // ---------------------------------------------------------------- header + shimmer

    void UpdateHeaderAndShimmer(float t)
    {
        _text.ForceMeshUpdate();
        var info = _text.textInfo;
        if (info.lineCount == 0)
            return;

        var line = info.lineInfo[0];
        bool any = false;
        Vector3 min = Vector3.zero, max = Vector3.zero;
        for (int c = line.firstCharacterIndex; c <= line.lastCharacterIndex && c < info.characterCount; ++c)
        {
            var ch = info.characterInfo[c];
            if (!ch.isVisible)
                continue;
            min = any ? Vector3.Min(min, ch.bottomLeft) : ch.bottomLeft;
            max = any ? Vector3.Max(max, ch.topRight) : ch.topRight;
            any = true;
        }
        if (!any)
            return;

        Vector3 center = (min + max) * 0.5f;
        _headerWidth = max.x - min.x;
        _headerCenter = _rect.InverseTransformPoint(_text.rectTransform.TransformPoint(center));

        if (!Shimmer)
            return;

        float breathe = 1f + BreatheAmount * Mathf.Sin(t * 2.2f);
        float cycle = Mathf.Repeat(t, ShimmerInterval);
        float sweep = cycle < ShimmerDuration ? cycle / ShimmerDuration : -1f; // 0..1 across the header, -1 = off

        for (int c = line.firstCharacterIndex; c <= line.lastCharacterIndex && c < info.characterCount; ++c)
        {
            var ch = info.characterInfo[c];
            if (!ch.isVisible)
                continue;
            int mi = ch.materialReferenceIndex, vi = ch.vertexIndex;
            var verts = info.meshInfo[mi].vertices;
            var cols = info.meshInfo[mi].colors32;
            for (int k = 0; k < 4; ++k)
            {
                var v = verts[vi + k];
                verts[vi + k] = center + (v - center) * breathe;

                if (sweep >= 0f)
                {
                    float u = Mathf.InverseLerp(min.x, max.x, v.x);
                    float band = 1f - Mathf.Clamp01(Mathf.Abs(u - Mathf.Lerp(-ShimmerWidth, 1f + ShimmerWidth, sweep)) / ShimmerWidth);
                    cols[vi + k] = Color32.Lerp(cols[vi + k], new Color32(255, 255, 235, cols[vi + k].a), band * 0.85f);
                }
            }
        }
        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
    }

    // ---------------------------------------------------------------- rays

    void UpdateRays(float t)
    {
        _rays.gameObject.SetActive(Rays);
        if (!Rays)
            return;

        float size = Mathf.Max(60f, _headerWidth * RaySizeMul) * (1f + 0.05f * Mathf.Sin(t * 1.3f));
        _rays.sizeDelta = new Vector2(size, size);
        _rays.localPosition = _back.InverseTransformPoint(_rect.TransformPoint(_headerCenter));
        _rays.localRotation = Quaternion.Euler(0, 0, -t * RaySpeed);
        var img = _rays.GetComponent<Image>();
        img.color = new Color(Gold.r, Gold.g, Gold.b, RayAlpha);
    }

    // ---------------------------------------------------------------- fireworks

    void UpdateFireworks(float t, float dt)
    {
        if (Fireworks && t >= _nextFirework)
        {
            _nextFirework = t + FireworkInterval * Random.Range(0.8f, 1.2f);
            var r = _front.rect;
            var at = new Vector2(Random.Range(r.xMin + r.width * 0.15f, r.xMax - r.width * 0.15f),
                                 Random.Range(r.center.y + r.height * 0.05f, r.yMax - r.height * 0.12f));
            foreach (var s in _sparks)
            {
                float a = Random.value * Mathf.PI * 2;
                s.Pos = at;
                s.Vel = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * SparkSpeed * Random.Range(0.5f, 1f);
                s.Age = 0f;
                s.Img.color = Random.value < 0.7f ? Gold : Color.white;
                s.Rt.gameObject.SetActive(true);
            }
            _fwCrown.rectTransform.localPosition = at;
            _fwCrownAge = 0f;
            _fwCrown.gameObject.SetActive(_fwCrown.sprite != null);
        }

        foreach (var s in _sparks)
        {
            if (s.Age >= SparkLife)
            {
                if (s.Rt.gameObject.activeSelf)
                    s.Rt.gameObject.SetActive(false);
                continue;
            }
            s.Age += dt;
            s.Vel *= Mathf.Exp(-2.5f * dt);   // drag
            s.Vel += Vector2.down * 40f * dt; // a little gravity
            s.Pos += s.Vel * dt;
            s.Rt.localPosition = s.Pos;
            var c = s.Img.color;
            c.a = 1f - s.Age / SparkLife;
            s.Img.color = c;
        }

        if (_fwCrown.gameObject.activeSelf)
        {
            _fwCrownAge += dt;
            const float CrownLife = 1.1f;
            float k = _fwCrownAge / CrownLife;
            if (k >= 1f)
            {
                _fwCrown.gameObject.SetActive(false);
            }
            else
            {
                float pop = _fwCrownAge < 0.15f ? Mathf.Lerp(0f, 1.2f, _fwCrownAge / 0.15f)
                                                : Mathf.Lerp(1.2f, 1f, Mathf.Clamp01((_fwCrownAge - 0.15f) / 0.15f));
                _fwCrown.rectTransform.localScale = Vector3.one * pop;
                _fwCrown.rectTransform.sizeDelta = Vector2.one * FireworkCrownSize;
                _fwCrown.color = new Color(1, 1, 1, k < 0.5f ? 1f : 1f - (k - 0.5f) / 0.5f);
            }
        }
    }

    // ---------------------------------------------------------------- confetti

    void ResetConfetto(Confetto c, bool randomY)
    {
        var r = _back.rect;
        float y = randomY ? Random.Range(r.yMin, r.yMax) : r.yMax + Random.Range(5f, 40f);
        c.Rt.localPosition = new Vector3(Random.Range(r.xMin, r.xMax), y, 0);
        c.Rt.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
    }

    void UpdateConfetti(float t, float dt)
    {
        var r = _back.rect;
        foreach (var c in _confetti)
        {
            bool on = Confetti;
            if (c.Rt.gameObject.activeSelf != on)
                c.Rt.gameObject.SetActive(on);
            if (!on)
                continue;

            var p = c.Rt.localPosition;
            p.y -= c.Speed * dt;
            p.x += Mathf.Cos(t * 1.7f + c.SwayPhase) * c.SwayAmount * dt;
            c.Rt.localPosition = p;
            c.Rt.localRotation *= Quaternion.Euler(0, 0, c.Spin * dt);
            // Flip-like tumble: squash the width with the spin.
            float flip = Mathf.Abs(Mathf.Cos(t * 3f + c.SwayPhase));
            c.Rt.localScale = new Vector3(0.35f + 0.65f * flip, 1f, 1f);

            if (p.y < r.yMin - 10f)
                ResetConfetto(c, randomY: false);
        }
    }
}
