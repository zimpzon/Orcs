using TMPro;
using UnityEngine;

// Rare shine on the "Idle Earl" logo text: about once a minute a soft, slightly slanted band of warm light sweeps across
// the letters, left to right. Tints the TMP vertex colors toward ShineColor and restores them afterwards. Unscaled time.
[RequireComponent(typeof(TMP_Text))]
public class LogoShine : MonoBehaviour
{
    public float MinInterval = 50f;
    public float MaxInterval = 70f;
    public float FirstDelay = 15f;
    public float Duration = 1.4f;
    public float BandWidth = 0.22f;   // fraction of the text width
    public float Slant = 0.35f;       // how much the band leans (x shifts with height)
    [Range(0, 1)] public float Strength = 0.75f;
    public Color ShineColor = new(1f, 0.97f, 0.85f, 1f);

    TMP_Text _text;
    float _nextShine;
    float _start = -1f;
    Color32[][] _baseColors;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _nextShine = Time.unscaledTime + FirstDelay;
    }

    void OnDisable() => Restore();

    void LateUpdate()
    {
        float now = Time.unscaledTime;
        if (_start < 0f)
        {
            if (now < _nextShine)
                return;
            _start = now;
            CaptureBase();
        }

        float t = (now - _start) / Duration;
        if (t >= 1f || _baseColors == null)
        {
            Restore();
            _start = -1f;
            _nextShine = now + Random.Range(MinInterval, MaxInterval);
            return;
        }

        Apply(Mathf.SmoothStep(0f, 1f, t));
    }

    void CaptureBase()
    {
        _text.ForceMeshUpdate();
        var info = _text.textInfo;
        _baseColors = new Color32[info.meshInfo.Length][];
        for (int m = 0; m < info.meshInfo.Length; ++m)
            _baseColors[m] = (Color32[])info.meshInfo[m].colors32.Clone();
    }

    void Apply(float progress)
    {
        var info = _text.textInfo;
        var bounds = _text.textBounds;
        float width = Mathf.Max(1f, bounds.size.x);
        float height = Mathf.Max(1f, bounds.size.y);

        // The band starts fully left of the text and ends fully right of it.
        float travel = 1f + 2f * BandWidth + Slant;
        float center = -BandWidth - Slant + progress * travel;

        for (int m = 0; m < info.meshInfo.Length && m < _baseColors.Length; ++m)
        {
            var mesh = info.meshInfo[m];
            var colors = mesh.colors32;
            var verts = mesh.vertices;
            var baseColors = _baseColors[m];
            int count = Mathf.Min(colors.Length, baseColors.Length);
            for (int v = 0; v < count; ++v)
            {
                float x = (verts[v].x - bounds.min.x) / width;
                float y = (verts[v].y - bounds.min.y) / height;
                float d = Mathf.Abs(x + y * Slant - center) / BandWidth;
                float shine = d >= 1f ? 0f : (1f - d * d) * Strength;
                Color c = baseColors[v];
                Color lit = Color.Lerp(c, ShineColor, shine);
                lit.a = c.a;
                colors[v] = lit;
            }
        }
        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    void Restore()
    {
        if (_baseColors == null || _text == null)
            return;

        var info = _text.textInfo;
        for (int m = 0; m < info.meshInfo.Length && m < _baseColors.Length; ++m)
        {
            var colors = info.meshInfo[m].colors32;
            int count = Mathf.Min(colors.Length, _baseColors[m].Length);
            System.Array.Copy(_baseColors[m], colors, count);
        }
        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        _baseColors = null;
    }
}
