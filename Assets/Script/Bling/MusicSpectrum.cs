using UnityEngine;
using UnityEngine.UI;

// Spectrum bars in the music dialog, dancing to the playing song (MusicManagerScript's AudioSource). Log-spaced
// frequency bands so bass and treble both get room; bars jump up fast and fall slowly. Built from code inside this
// RectTransform (a placeholder in the scene). WebGL: Unity can't read the spectrum there, so it hides itself.
public class MusicSpectrum : MonoBehaviour
{
    [Range(4, 64)] public int BarCount = 24;
    [Range(0f, 8f)] public float BarSpacing = 2f;
    public Color BarColor = new Color(0.96f, 0.77f, 0.19f, 0.85f); // TextColors.Gold-ish
    [Tooltip("Overall height scale of the bars.")]
    public float Gain = 40f;
    [Tooltip("How fast bars fall back (fraction of full height per second).")]
    public float FallSpeed = 1.6f;
    [Tooltip("Lowest bar height (fraction), so the row never disappears completely.")]
    [Range(0f, 0.3f)] public float MinHeight = 0.04f;

    const int Samples = 512;
    const float MinFreq = 60f;
    const float MaxFreq = 12000f;

    readonly float[] _spectrum = new float[Samples];
    RectTransform[] _bars;
    float[] _heights;

    void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        gameObject.SetActive(false);
        return;
#endif
    }

    void Build()
    {
        var area = (RectTransform)transform;
        float w = area.rect.width;
        float barW = Mathf.Max(1f, (w - BarSpacing * (BarCount - 1)) / BarCount);
        _bars = new RectTransform[BarCount];
        _heights = new float[BarCount];
        for (int i = 0; i < BarCount; ++i)
        {
            var go = new GameObject("Bar", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            var img = go.AddComponent<Image>();
            img.color = BarColor;
            img.raycastTarget = false;
            var rt = img.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0f, 0f);
            rt.anchoredPosition = new Vector2(i * (barW + BarSpacing), 0f);
            rt.sizeDelta = new Vector2(barW, 0f);
            _bars[i] = rt;
        }
    }

    void Update()
    {
        if (_bars == null)
        {
            if (((RectTransform)transform).rect.width <= 0f)
                return; // layout not ready yet
            Build();
        }

        var music = MusicManagerScript.Instance;
        var source = music != null ? music.Source : null;
        bool playing = source != null && source.isPlaying;
        if (playing)
            source.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

        float height = ((RectTransform)transform).rect.height;
        float binHz = AudioSettings.outputSampleRate * 0.5f / Samples;
        float dt = Time.unscaledDeltaTime;

        for (int i = 0; i < BarCount; ++i)
        {
            float target = 0f;
            if (playing)
            {
                // Band i covers a log-spaced slice of MinFreq..MaxFreq.
                float f0 = MinFreq * Mathf.Pow(MaxFreq / MinFreq, i / (float)BarCount);
                float f1 = MinFreq * Mathf.Pow(MaxFreq / MinFreq, (i + 1) / (float)BarCount);
                int b0 = Mathf.Clamp((int)(f0 / binHz), 0, Samples - 1);
                int b1 = Mathf.Clamp((int)(f1 / binHz), b0 + 1, Samples);
                float sum = 0f;
                for (int b = b0; b < b1; ++b)
                    sum += _spectrum[b];
                float avg = sum / (b1 - b0);
                // Higher bands are much quieter: lift them, then a soft log curve into 0..1.
                float lift = 1f + i * 0.35f;
                target = Mathf.Clamp01(Mathf.Log10(1f + avg * Gain * lift * 10f));
            }

            float h = _heights[i];
            h = target > h ? Mathf.Lerp(h, target, 0.6f) : Mathf.Max(target, h - FallSpeed * dt);
            _heights[i] = h;
            _bars[i].sizeDelta = new Vector2(_bars[i].sizeDelta.x, height * Mathf.Max(MinHeight, h));
        }
    }
}
