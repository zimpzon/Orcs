using System.Collections;
using UnityEngine;

// Makes the clickable arena chest (PopupChestScript) look valuable: small star glints twinkle over it (on top of its
// original particle effect), and picking it up plays a little show - the glints burst and the chest pops and shrinks
// away. Built in code, added by PopupChestScript.Awake. Glints are children of the chest,
// so they follow it (it's parked far off-screen while hidden). (Tried god rays, a glow behind it and a coin fountain on
// pickup - didn't look right.)
public class ChestBling : MonoBehaviour
{
    static readonly Color GlintColor = new(1f, 0.97f, 0.8f, 1f);

    // Glints
    const int GlintCount = 8;
    const float GlintLife = 0.5f, GlintSize = 0.4f, GlintSpread = 0.55f, GlintMinGap = 0.1f, GlintMaxGap = 0.25f;

    // Pickup: pop up, then shrink away.
    const float PopScale = 1.25f, PopTime = 0.08f, ShrinkTime = 0.25f;

    readonly SpriteRenderer[] _glints = new SpriteRenderer[GlintCount];
    readonly float[] _glintAge = new float[GlintCount];
    float _nextGlint;
    float _time;
    float _worldToLocal = 1f; // 1 / chest's world scale, so sizes below are in world units
    SpriteRenderer _chest;
    static Sprite _starSprite;

    void Awake()
    {
        _chest = GetComponent<SpriteRenderer>();
        _worldToLocal = 1f / Mathf.Max(0.0001f, Mathf.Abs(transform.lossyScale.x));

        for (int i = 0; i < GlintCount; ++i)
        {
            _glints[i] = NewRenderer(transform, "ChestGlint", StarSprite(), _chest.sortingLayerID, _chest.sortingOrder + 1);
            _glints[i].enabled = false;
            _glintAge[i] = GlintLife;
        }
    }

    static SpriteRenderer NewRenderer(Transform parent, string name, Sprite sprite, int layer, int order)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerID = layer;
        sr.sortingOrder = order;
        return sr;
    }

    // Local scale that makes a sprite `worldSize` units wide.
    float ScaleFor(SpriteRenderer sr, float worldSize) => worldSize / sr.sprite.bounds.size.x * _worldToLocal;

    void Update()
    {
        float dt = Time.deltaTime;
        _time += dt;

        if (_time >= _nextGlint)
        {
            _nextGlint = _time + Random.Range(GlintMinGap, GlintMaxGap);
            StartGlint();
        }

        for (int i = 0; i < GlintCount; ++i)
        {
            if (_glintAge[i] >= GlintLife)
                continue;
            _glintAge[i] += dt;
            if (_glintAge[i] >= GlintLife)
            {
                _glints[i].enabled = false;
                continue;
            }
            float t = _glintAge[i] / GlintLife;
            float size = Mathf.Sin(t * Mathf.PI); // 0 -> 1 -> 0
            _glints[i].transform.localScale = Vector3.one * ScaleFor(_glints[i], GlintSize * size);
            _glints[i].transform.Rotate(0, 0, 90f * dt);
            _glints[i].color = GlintColor;
        }
    }

    void StartGlint()
    {
        for (int i = 0; i < GlintCount; ++i)
        {
            if (_glintAge[i] < GlintLife)
                continue;
            _glintAge[i] = 0f;
            _glints[i].enabled = true;
            _glints[i].transform.localPosition = Random.insideUnitCircle * GlintSpread * _worldToLocal;
            _glints[i].transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 90f));
            return;
        }
    }

    // Picked up: glint burst, the chest pops and shrinks to nothing. PopupChestScript waits for this
    // before hiding the chest (and restores its scale when it shows it again).
    public IEnumerator PickupCo()
    {
        for (int i = 0; i < GlintCount; ++i)
            StartGlint();


        Vector3 baseScale = transform.localScale;
        for (float t = 0; t < PopTime; t += Time.deltaTime)
        {
            transform.localScale = baseScale * Mathf.Lerp(1f, PopScale, t / PopTime);
            yield return null;
        }
        for (float t = 0; t < ShrinkTime; t += Time.deltaTime)
        {
            float k = t / ShrinkTime;
            transform.localScale = baseScale * PopScale * (1f - k * k); // ease in: slow start, fast finish
            yield return null;
        }
        transform.localScale = Vector3.zero;
    }

    // Four-pointed star, soft core: two thin crossed diamonds. Generated once (Unity null check: destroyed with play mode).
    static Sprite StarSprite()
    {
        if (_starSprite != null)
            return _starSprite;

        const int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        var px = new Color32[size * size];
        for (int y = 0; y < size; ++y)
        {
            for (int x = 0; x < size; ++x)
            {
                float dx = Mathf.Abs((x + 0.5f) / size * 2f - 1f), dy = Mathf.Abs((y + 0.5f) / size * 2f - 1f);
                // Thin arms along both axes, plus a small round glow in the middle.
                float arm = Mathf.Max(Mathf.Clamp01(1f - dx - dy * 6f), Mathf.Clamp01(1f - dy - dx * 6f));
                float core = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy) * 3f);
                float a = Mathf.Clamp01(arm + core);
                px[y * size + x] = new Color32(255, 255, 255, (byte)(255 * a));
            }
        }
        tex.SetPixels32(px);
        tex.Apply();
        _starSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        return _starSprite;
    }
}
