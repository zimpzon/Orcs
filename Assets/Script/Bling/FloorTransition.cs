using UnityEngine;

// Arena floor change (new round's random floor / color, Scary Earl's floor): instead of swapping at once, the new floor
// spreads out from the middle in a growing circle with a chunky pixel edge and a fading bright ring, then the floor itself
// takes it over. No shader: a copy of the floor (FloorNext) only shows inside a SpriteMask circle; the ring only
// inside the floor's rectangle (a second mask).
public static class FloorTransition
{
    const float Duration = 0.45f;
    const bool EdgeRing = true;
    const float RingAlpha = 0.55f;
    const int CirclePixels = 128;     // mask circle texture: its pixels are the chunky edge
    const float RingWidthShare = 0.06f;

    static SpriteRenderer _floor, _next, _ring;
    static SpriteMask _mask, _ringClip;
    static Sprite _targetSprite;
    static Color _targetColor;
    static bool _running;
    static int _tween = -1;

    public static void Change(SpriteRenderer floor, Sprite sprite, Color color)
    {
        if (floor == null)
            return;
        if (_running)
            Finish(); // a change while one runs: complete that one first

        if (floor.sprite == sprite && floor.color == color)
            return;

        Build(floor);
        _targetSprite = sprite;
        _targetColor = color;

        _next.sprite = sprite;
        _next.color = color;
        _next.size = floor.size;
        _next.gameObject.SetActive(true);

        // From the floor's middle until it covers the floor's corners - the whole floor sprite, which is bigger than
        // the arena area (ending at the arena's corners left the floor's edges to pop in at the end).
        var b = floor.bounds;
        Vector2 from = b.center;
        float radius = ((Vector2)b.extents).magnitude;
        float endScale = radius * 2f * 1.05f; // circle sprite is 1 unit across

        _mask.transform.position = new Vector3(from.x, from.y, floor.transform.position.z);
        _mask.transform.localScale = Vector3.zero;
        _mask.gameObject.SetActive(true);
        if (_ring != null)
        {
            _ring.transform.position = _mask.transform.position;
            _ring.transform.localScale = Vector3.zero;
            _ring.color = new Color(1f, 1f, 1f, RingAlpha);
            _ring.gameObject.SetActive(true);
            _ringClip.transform.position = new Vector3(from.x, from.y, floor.transform.position.z);
            SetWorldScale(_ringClip.transform, b.size);
            _ringClip.gameObject.SetActive(true);
        }

        _running = true;
        _tween = LeanTween.value(_mask.gameObject, 0f, 1f, Duration).setIgnoreTimeScale(true) // even speed: no slow end
            .setOnUpdate(t =>
            {
                float s = t * endScale;
                SetWorldScale(_mask.transform, new Vector2(s, s));
                if (_ring != null)
                {
                    SetWorldScale(_ring.transform, new Vector2(s, s));
                    _ring.color = new Color(1f, 1f, 1f, RingAlpha * (1f - t));
                }
            })
            .setOnComplete(Finish).id;
    }

    // Completes a running change at once (e.g. before reading the floor's current sprite / color).
    public static void Complete() => Finish();

    static void Finish()
    {
        if (!_running)
            return;
        _running = false;
        if (_tween >= 0)
            LeanTween.cancel(_tween);
        _tween = -1;

        if (_floor != null)
        {
            _floor.sprite = _targetSprite;
            _floor.color = _targetColor;
        }
        _next.gameObject.SetActive(false);
        _mask.gameObject.SetActive(false);
        if (_ring != null)
        {
            _ring.gameObject.SetActive(false);
            _ringClip.gameObject.SetActive(false);
        }
    }

    static void Build(SpriteRenderer floor)
    {
        if (_floor == floor && _next != null)
            return;
        _floor = floor;

        // The new floor: a copy of the floor renderer just above it, visible only inside the mask.
        var nextGo = new GameObject("FloorNext");
        nextGo.layer = floor.gameObject.layer;
        nextGo.transform.SetParent(floor.transform, false);
        _next = nextGo.AddComponent<SpriteRenderer>();
        _next.sharedMaterial = floor.sharedMaterial;
        _next.drawMode = floor.drawMode;
        _next.tileMode = floor.tileMode;
        _next.sortingLayerID = floor.sortingLayerID;
        _next.sortingOrder = floor.sortingOrder + 1;
        _next.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        nextGo.SetActive(false);

        // The mask: not under the floor (its scale would multiply in), only affecting the floor's own orders.
        var maskGo = new GameObject("FloorReveal");
        maskGo.layer = floor.gameObject.layer;
        maskGo.transform.SetParent(floor.transform.parent, false);
        _mask = maskGo.AddComponent<SpriteMask>();
        _mask.sprite = MakeCircle(filled: true);
        _mask.isCustomRangeActive = true;
        _mask.frontSortingLayerID = floor.sortingLayerID;
        _mask.frontSortingOrder = floor.sortingOrder + 1;
        _mask.backSortingLayerID = floor.sortingLayerID;
        _mask.backSortingOrder = floor.sortingOrder;
        maskGo.SetActive(false);

        if (EdgeRing)
        {
            var ringGo = new GameObject("FloorRevealRing");
            ringGo.layer = floor.gameObject.layer;
            ringGo.transform.SetParent(floor.transform.parent, false);
            _ring = ringGo.AddComponent<SpriteRenderer>();
            _ring.sprite = MakeCircle(filled: false);
            _ring.sortingLayerID = floor.sortingLayerID;
            _ring.sortingOrder = floor.sortingOrder + 2;
            _ring.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            ringGo.SetActive(false);

            // The ring only over the floor: a mask the floor's size, affecting only the ring's order.
            var clipGo = new GameObject("FloorRevealRingClip");
            clipGo.layer = floor.gameObject.layer;
            clipGo.transform.SetParent(floor.transform.parent, false);
            _ringClip = clipGo.AddComponent<SpriteMask>();
            _ringClip.sprite = MakeSquare();
            _ringClip.isCustomRangeActive = true;
            _ringClip.frontSortingLayerID = floor.sortingLayerID;
            _ringClip.frontSortingOrder = floor.sortingOrder + 2;
            _ringClip.backSortingLayerID = floor.sortingLayerID;
            _ringClip.backSortingOrder = floor.sortingOrder + 1;
            clipGo.SetActive(false);
        }
    }

    // Sizes in world units: the masks sit next to the floor, under a scaled parent (0.756 - with plain localScale the
    // circle stopped short of the corners and the ring clip was smaller than the floor).
    static void SetWorldScale(Transform t, Vector2 size)
    {
        var parent = t.parent != null ? t.parent.lossyScale : Vector3.one;
        t.localScale = new Vector3(size.x / parent.x, size.y / parent.y, 1f);
    }

    // A white square 1 world unit across.
    static Sprite MakeSquare()
    {
        var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
        var px = new Color32[16];
        for (int i = 0; i < px.Length; ++i)
            px[i] = new Color32(255, 255, 255, 255);
        tex.SetPixels32(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
    }

    // A pixel circle 1 world unit across (point filtered: the edge stays blocky when scaled up). Not filled: just the
    // outer ring, for the shockwave edge.
    static Sprite MakeCircle(bool filled)
    {
        int n = CirclePixels;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
        var px = new Color32[n * n];
        float r = n * 0.5f, inner = r * (1f - RingWidthShare * 2f);
        for (int y = 0; y < n; ++y)
        for (int x = 0; x < n; ++x)
        {
            float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
            bool on = d <= r && (filled || d >= inner);
            px[y * n + x] = on ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0);
        }
        tex.SetPixels32(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
    }
}
