using UnityEngine;

// Drives the metaball backdrop of the Scary Earl story text under the arena HP bar (Custom/SpriteMetaballs): sinister
// violet ooze while he's undefeated, calm warm brown with gold comets once he's beaten (victory banner), dark crimson
// with red comets for the every-beast-decimated banner; it fades between them. Keeps the blobs round on the wide panel
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
        // Dark crimson: blood-red blobs and rim on a near-black maroon base, red-orange comets. The gold "decimated!"
        // reads well on it and it's clearly not the victory banner.
        Bg = new Color(0.08f, 0.01f, 0.02f, 0.6f),
        Ball = new Color(0.24f, 0.03f, 0.05f, 0.65f),
        Rim = new Color(0.8f, 0.14f, 0.14f, 0.45f),
        Speed = 0.25f,
        BallSize = 0.3f,
        Border = new Color(1f, 0.38f, 0.25f, 1f),
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
