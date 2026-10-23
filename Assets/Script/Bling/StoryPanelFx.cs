using UnityEngine;

// Drives the metaball backdrop of the Scary Earl story text under the arena HP bar (Custom/SpriteMetaballs): sinister
// violet ooze while he's undefeated, calm warm brown with gold comets once he's beaten (victory banner), muted dark
// green for the every-beast-decimated banner; it fades between them. Keeps the blobs round on the wide panel
// (_Aspect from the scale). Its own material instance, unscaled time for the palette blend.
[RequireComponent(typeof(SpriteRenderer))]
public class StoryPanelFx : MonoBehaviour
{
    const float BlendTime = 1.5f;

    struct Palette
    {
        public Color Bg, Ball, Rim, Border;
        public float Speed, BallSize;
        public float BorderStrength, BorderLapSeconds; // comets running around the panel border (shader)
    }

    static readonly Palette Fighting = new()
    {
        // Cool indigo/violet: the complement of the gold name and the warm dusty arena, so the name pops and the panel
        // reads as "not from here". A violet-tinted dark base instead of pure black (black over brown looks muddy);
        // opaque enough behind the very light grey text. The rim is a soft lavender, kept thin so it never fights the text.
        Bg = new Color(0.04f, 0.02f, 0.07f, 0.45f),
        Ball = new Color(0.13f, 0.06f, 0.28f, 0.6f),
        Rim = new Color(0.45f, 0.25f, 0.8f, 0.5f),
        Speed = 0.35f,
        BallSize = 0.28f,
        // Discreet while fighting: dim lavender comets, slow.
        Border = new Color(0.55f, 0.4f, 0.95f, 1f),
        BorderStrength = 0.5f,
        BorderLapSeconds = 12f,
    };

    static readonly Palette Defeated = new()
    {
        // Warm and settled, in the arena's own dusty browns: deep umber blobs on a dark brown base, a muted copper rim
        // (redder and darker than the gold name, so the name still stands out, and less blood-red than before).
        Bg = new Color(0.07f, 0.04f, 0.02f, 0.6f),
        Ball = new Color(0.16f, 0.09f, 0.05f, 0.65f),
        Rim = new Color(0.55f, 0.28f, 0.1f, 0.45f),
        Speed = 0.18f,
        BallSize = 0.31f,
        // Victory lap: gold comets (toned down - they drew too much attention from the text).
        Border = new Color(1f, 0.84f, 0.35f, 1f),
        BorderStrength = 0.75f,
        BorderLapSeconds = 8f,
    };

    static readonly Palette Decimated = new()
    {
        // Muted dark green: moss-green blobs on a near-black green base, a soft green rim and light green comets. Calm
        // and clearly different from the brown/gold victory banner; the gold "decimated!" reads well on it.
        Bg = new Color(0.02f, 0.06f, 0.03f, 0.6f),
        Ball = new Color(0.06f, 0.17f, 0.08f, 0.65f),
        Rim = new Color(0.3f, 0.62f, 0.3f, 0.45f),
        Speed = 0.25f,
        BallSize = 0.3f,
        Border = new Color(0.55f, 0.95f, 0.45f, 1f),
        BorderStrength = 0.75f,
        BorderLapSeconds = 7f,
    };

    static readonly int BgId = Shader.PropertyToID("_BgColor");
    static readonly int BallId = Shader.PropertyToID("_BallColor");
    static readonly int RimId = Shader.PropertyToID("_RimColor");
    static readonly int PhaseId = Shader.PropertyToID("_Phase");
    static readonly int BallSizeId = Shader.PropertyToID("_BallSize");
    static readonly int AspectId = Shader.PropertyToID("_Aspect");
    static readonly int BorderStrengthId = Shader.PropertyToID("_BorderStrength");
    static readonly int BorderPhaseId = Shader.PropertyToID("_BorderPhase");
    static readonly int BorderColorId = Shader.PropertyToID("_BorderColor");

    // Two comets run around the panel border (shader): discreet lavender while fighting, bright gold once he's beaten.
    float _borderPhase;
    float _borderLapSeconds = 10f;

    Material _mat;
    Palette _from, _to, _current; // fading from _from to _to (_blend 0..1); _current = what's applied now
    float _blend = 1f;
    float _lastAspect = -1f;
    float _phase;        // blob animation time x speed, so changing the speed doesn't make them jump
    float _speed;

    void Awake()
    {
        _mat = GetComponent<SpriteRenderer>().material; // instance
        CreateFrame();
    }

    // ---- Pixel frame ----
    // A thin pixel-art border around the panel, so it sits in the pixely arena like a little sign instead of floating
    // over it. Its own sliced sprite made in code (the dialog / bestiary panel sprites have a filled center that would
    // cover the metaballs): transparent inside, at the arena floor's 20 pixels per unit. A sibling of this backdrop
    // (whose scale is stretched), fitted to its bounds every frame; shown and hidden with it.
    const float FramePixelsPerUnit = 20f;
    [Tooltip("Frame edge inside the backdrop's edges (world units): the backdrop fades out at its edges.")]
    public float FrameInset = 0.06f;
    [Tooltip("Room between the text and the frame, left/right and top/bottom (world units).")]
    public Vector2 TextPadding = new(0.45f, 0.3f);
    SpriteRenderer _body, _frame;
    TMPro.TMP_Text _text; // the banner text (parent): the frame and backdrop are sized around it
    static Sprite _frameSprite;

    void CreateFrame()
    {
        _body = GetComponent<SpriteRenderer>();
        _text = transform.parent != null ? transform.parent.GetComponent<TMPro.TMP_Text>() : null;
        var go = new GameObject("PixelFrame");
        go.transform.SetParent(transform.parent, false);
        _frame = go.AddComponent<SpriteRenderer>();
        _frame.sprite = FrameSprite();
        _frame.drawMode = SpriteDrawMode.Sliced;
        _frame.sortingLayerID = _body.sortingLayerID;
        _frame.sortingOrder = _body.sortingOrder + 1; // over the metaballs, its center is see-through
        go.SetActive(gameObject.activeInHierarchy);
    }

    void OnDisable()
    {
        if (_frame != null)
            _frame.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (_frame == null)
            return;
        if (!_frame.gameObject.activeSelf)
            _frame.gameObject.SetActive(true);
        FitToText();
        var b = _body.bounds;
        var parentScale = _frame.transform.parent != null ? _frame.transform.parent.lossyScale : Vector3.one;
        _frame.transform.position = new Vector3(b.center.x, b.center.y, transform.position.z);
        _frame.size = new Vector2(
            Mathf.Max(0.1f, (b.size.x - FrameInset * 2) / Mathf.Abs(parentScale.x)),
            Mathf.Max(0.1f, (b.size.y - FrameInset * 2) / Mathf.Abs(parentScale.y)));
    }

    // The backdrop (this object, a stretched sprite) covers the text's drawn bounds + TextPadding + FrameInset, centered
    // on the text; the frame then sits FrameInset inside it. Text space = the parent's local space.
    void FitToText()
    {
        if (_text == null || _body.sprite == null || string.IsNullOrEmpty(_text.text))
            return;
        var tb = _text.textBounds;
        if (tb.size.x <= 0f || tb.size.y <= 0f)
            return;
        Vector2 want = new Vector2(tb.size.x + (TextPadding.x + FrameInset) * 2, tb.size.y + (TextPadding.y + FrameInset) * 2);
        Vector2 spriteSize = _body.sprite.bounds.size;
        transform.localPosition = new Vector3(tb.center.x, tb.center.y, transform.localPosition.z);
        transform.localScale = new Vector3(want.x / spriteSize.x, want.y / spriteSize.y, 1f);
    }

    // 12x12, 4 px border: a near-black outer line, two mid brown, one light inner line (the dialog panels' browns).
    static Sprite FrameSprite()
    {
        if (_frameSprite != null)
            return _frameSprite;
        const int n = 12, border = 4;
        Color32[] ring =
        {
            new Color32(0x1E, 0x19, 0x14, 255), // outer
            new Color32(0x6E, 0x62, 0x53, 255),
            new Color32(0x6E, 0x62, 0x53, 255),
            new Color32(0xA3, 0x92, 0x7A, 255), // inner highlight
        };
        var px = new Color32[n * n];
        for (int y = 0; y < n; ++y)
        for (int x = 0; x < n; ++x)
        {
            int d = Mathf.Min(Mathf.Min(x, y), Mathf.Min(n - 1 - x, n - 1 - y)); // distance to the edge
            // Cut the outermost corner pixel: a slightly rounded pixel corner.
            bool corner = (x == 0 || x == n - 1) && (y == 0 || y == n - 1);
            px[y * n + x] = d < border && !corner ? ring[d] : new Color32(0, 0, 0, 0);
        }
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
        tex.SetPixels32(px);
        tex.Apply();
        _frameSprite = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), FramePixelsPerUnit, 0,
            SpriteMeshType.FullRect, new Vector4(border, border, border, border));
        return _frameSprite;
    }

    void OnEnable()
    {
        _from = _to = _current = Target();
        _blend = 1f;
        Apply();
    }

    static Palette Target()
    {
        if (SaveGame.Members == null || !SaveGame.Members.ScaryEarlDefeated)
            return Fighting;
        return ScaryEarlFight.ShowsDecimatedBanner ? Decimated : Defeated;
    }

    static bool Same(Palette a, Palette b) => a.Bg == b.Bg && a.Ball == b.Ball && a.Rim == b.Rim && a.Border == b.Border;

    void Update()
    {
        _phase += Time.unscaledDeltaTime * _speed;
        _mat.SetFloat(PhaseId, _phase);
        _borderPhase = Mathf.Repeat(_borderPhase + Time.unscaledDeltaTime / _borderLapSeconds, 1f);
        _mat.SetFloat(BorderPhaseId, _borderPhase);

        var target = Target();
        if (!Same(target, _to))
        {
            // New look: fade to it from wherever the colors are right now.
            _from = _current;
            _to = target;
            _blend = 0f;
        }
        if (_blend < 1f)
        {
            _blend = Mathf.MoveTowards(_blend, 1f, Time.unscaledDeltaTime / BlendTime);
            Apply();
        }

        Vector3 s = transform.lossyScale;
        float aspect = Mathf.Abs(s.y) > 0.0001f ? Mathf.Abs(s.x / s.y) : 1f;
        if (aspect != _lastAspect)
        {
            _lastAspect = aspect;
            _mat.SetFloat(AspectId, aspect);
        }
    }

    void Apply()
    {
        float t = Mathf.SmoothStep(0f, 1f, _blend);
        _current = new Palette
        {
            Bg = Color.Lerp(_from.Bg, _to.Bg, t),
            Ball = Color.Lerp(_from.Ball, _to.Ball, t),
            Rim = Color.Lerp(_from.Rim, _to.Rim, t),
            Border = Color.Lerp(_from.Border, _to.Border, t),
            Speed = Mathf.Lerp(_from.Speed, _to.Speed, t),
            BallSize = Mathf.Lerp(_from.BallSize, _to.BallSize, t),
            BorderStrength = Mathf.Lerp(_from.BorderStrength, _to.BorderStrength, t),
            BorderLapSeconds = Mathf.Lerp(_from.BorderLapSeconds, _to.BorderLapSeconds, t),
        };
        _mat.SetColor(BgId, _current.Bg);
        _mat.SetColor(BallId, _current.Ball);
        _mat.SetColor(RimId, _current.Rim);
        _speed = _current.Speed;
        _mat.SetFloat(BorderStrengthId, _current.BorderStrength);
        _mat.SetColor(BorderColorId, _current.Border);
        _borderLapSeconds = _current.BorderLapSeconds;
        _mat.SetFloat(BallSizeId, _current.BallSize);
    }
}
