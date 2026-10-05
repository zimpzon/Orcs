using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Effects on the rebirth dialog's "Rebirth" title (TextHeader): a rolling letter wave, a shine sweeping across it and
// a soft pulsing glow behind it. Built on first use; the glow lives in a sibling placed just before the title so it
// draws behind it. Runs on unscaled time, never blocks clicks.
[RequireComponent(typeof(TextMeshProUGUI))]
public class RebirthTitleFx : MonoBehaviour
{
    [Header("Letter wave")]
    public bool Wave = true;
    public float WaveAmount = 2f;        // px
    public float WaveSpeed = 3f;
    public float WavePhasePerLetter = 0.6f;

    [Header("Shine")]
    public bool Shine = true;
    public float ShineInterval = 3f;
    public float ShineDuration = 0.6f;
    public float ShineWidth = 0.2f;      // fraction of the title width

    [Header("Glow")]
    public bool Glow = true;
    public Color GlowColor = new(0.56f, 0.83f, 1f);   // diamond blue #8FD3FF
    [Range(0, 1)] public float GlowAlphaMin = 0.12f;
    [Range(0, 1)] public float GlowAlphaMax = 0.3f;
    public float GlowPulseSpeed = 2.5f;
    public Vector2 GlowSizeMul = new(1.6f, 2.5f);    // x title width, x title height

    TextMeshProUGUI _text;
    RectTransform _fx;
    Image _glow;
    bool _built;
    Vector3 _min, _max;
    static Sprite _glowSprite;

    void Awake()
    {
        // The title's bobbing diamonds are clones of this text (AscendDecisionScript): they must not run the effects.
        if (transform.parent != null && transform.parent.GetComponent<RebirthTitleFx>() != null)
        {
            Destroy(this);
            return;
        }
        _text = GetComponent<TextMeshProUGUI>();
    }

    void OnDisable()
    {
        if (_fx != null)
            _fx.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (_text == null || !MeasureAndAnimateText())
            return;

        if (!_built)
            Build();
        _fx.gameObject.SetActive(true);

        UpdateGlow(Time.unscaledTime);
    }

    // ------------------------------------------------------------------ text: measure, wave, shine

    bool MeasureAndAnimateText()
    {
        _text.ForceMeshUpdate();
        var info = _text.textInfo;
        bool any = false;
        for (int c = 0; c < info.characterCount; ++c)
        {
            var ch = info.characterInfo[c];
            if (!ch.isVisible)
                continue;
            _min = any ? Vector3.Min(_min, ch.bottomLeft) : ch.bottomLeft;
            _max = any ? Vector3.Max(_max, ch.topRight) : ch.topRight;
            any = true;
        }
        if (!any)
            return false;

        if (!Wave && !Shine)
            return true;

        float t = Time.unscaledTime;
        float cycle = Mathf.Repeat(t, ShineInterval);
        float sweep = Shine && cycle < ShineDuration ? cycle / ShineDuration : -1f;
        int letter = 0;
        for (int c = 0; c < info.characterCount; ++c)
        {
            var ch = info.characterInfo[c];
            if (!ch.isVisible)
                continue;
            var verts = info.meshInfo[ch.materialReferenceIndex].vertices;
            var cols = info.meshInfo[ch.materialReferenceIndex].colors32;
            float lift = Wave ? Mathf.Sin(t * WaveSpeed - letter * WavePhasePerLetter) * WaveAmount : 0f;
            for (int k = 0; k < 4; ++k)
            {
                int vi = ch.vertexIndex + k;
                verts[vi] += new Vector3(0, lift, 0);
                if (sweep >= 0f)
                {
                    float u = Mathf.InverseLerp(_min.x, _max.x, verts[vi].x);
                    float band = 1f - Mathf.Clamp01(Mathf.Abs(u - Mathf.Lerp(-ShineWidth, 1f + ShineWidth, sweep)) / ShineWidth);
                    cols[vi] = Color32.Lerp(cols[vi], new Color32(255, 255, 240, cols[vi].a), band * 0.85f);
                }
            }
            letter++;
        }
        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
        return true;
    }

    // ------------------------------------------------------------------ build glow

    void Build()
    {
        _built = true;
        var go = new GameObject("RebirthTitleFx", typeof(RectTransform));
        go.transform.SetParent(transform.parent, false);
        go.transform.SetSiblingIndex(transform.GetSiblingIndex()); // just before the title -> drawn behind it
        _fx = (RectTransform)go.transform;
        _fx.anchorMin = _fx.anchorMax = _fx.pivot = new Vector2(0.5f, 0.5f);

        _glow = NewImage("Glow", GlowSprite(), Color.clear);
    }

    Image NewImage(string name, Sprite sprite, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(_fx, false);
        var img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.raycastTarget = false;
        img.rectTransform.anchorMin = img.rectTransform.anchorMax = img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        return img;
    }

    // Soft round white blob fading to transparent. Generated once.
    static Sprite GlowSprite()
    {
        if (_glowSprite != null)
            return _glowSprite;

        const int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        var px = new Color32[size * size];
        for (int y = 0; y < size; ++y)
        {
            for (int x = 0; x < size; ++x)
            {
                float dx = (x + 0.5f) / size * 2f - 1f, dy = (y + 0.5f) / size * 2f - 1f;
                float r = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(1f - r);
                px[y * size + x] = new Color32(255, 255, 255, (byte)(255 * a * a));
            }
        }
        tex.SetPixels32(px);
        tex.Apply();
        _glowSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        return _glowSprite;
    }

    // Title rect (text-local glyph bounds) converted into the fx container's space.
    Vector3 ToFx(Vector3 textLocal) => _fx.InverseTransformPoint(transform.TransformPoint(textLocal));

    // ------------------------------------------------------------------ glow

    void UpdateGlow(float t)
    {
        _glow.gameObject.SetActive(Glow);
        if (!Glow)
            return;

        Vector3 lo = ToFx(_min), hi = ToFx(_max);
        _glow.rectTransform.localPosition = (lo + hi) * 0.5f;
        _glow.rectTransform.sizeDelta = new Vector2((hi.x - lo.x) * GlowSizeMul.x, (hi.y - lo.y) * GlowSizeMul.y);
        float k = 0.5f + 0.5f * Mathf.Sin(t * Mathf.PI * 2f / GlowPulseSpeed);
        _glow.color = new Color(GlowColor.r, GlowColor.g, GlowColor.b, Mathf.Lerp(GlowAlphaMin, GlowAlphaMax, k));
    }
}
