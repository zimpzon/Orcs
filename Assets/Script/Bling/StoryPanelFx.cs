using UnityEngine;

// Drives the metaball backdrop of the Scary Earl story text under the arena HP bar (Custom/SpriteMetaballs): sinister
// violet ooze while he's undefeated, easing to calm warm gold once he's beaten; comets circle the border in both. Keeps the blobs round on the wide
// panel (_Aspect from the scale). Its own material instance, unscaled time for the palette blend.
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
        BorderStrength = 0.8f,
        BorderLapSeconds = 10f,
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
        // Victory lap: bright gold comets.
        Border = new Color(1f, 0.84f, 0.35f, 1f),
        BorderStrength = 1.2f,
        BorderLapSeconds = 6f,
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
    float _blend;        // 0 = fighting, 1 = defeated
    float _lastAspect = -1f;
    float _phase;        // blob animation time x speed, so changing the speed doesn't make them jump
    float _speed;

    void Awake()
    {
        _mat = GetComponent<SpriteRenderer>().material; // instance
    }

    void OnEnable()
    {
        _blend = Target();
        Apply();
    }

    static float Target() => SaveGame.Members != null && SaveGame.Members.ScaryEarlDefeated ? 1f : 0f;

    void Update()
    {
        _phase += Time.unscaledDeltaTime * _speed;
        _mat.SetFloat(PhaseId, _phase);
        _borderPhase = Mathf.Repeat(_borderPhase + Time.unscaledDeltaTime / _borderLapSeconds, 1f);
        _mat.SetFloat(BorderPhaseId, _borderPhase);

        float target = Target();
        if (_blend != target)
        {
            _blend = Mathf.MoveTowards(_blend, target, Time.unscaledDeltaTime / BlendTime);
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
        _mat.SetColor(BgId, Color.Lerp(Fighting.Bg, Defeated.Bg, t));
        _mat.SetColor(BallId, Color.Lerp(Fighting.Ball, Defeated.Ball, t));
        _mat.SetColor(RimId, Color.Lerp(Fighting.Rim, Defeated.Rim, t));
        _speed = Mathf.Lerp(Fighting.Speed, Defeated.Speed, t);
        _mat.SetFloat(BorderStrengthId, Mathf.Lerp(Fighting.BorderStrength, Defeated.BorderStrength, t));
        _mat.SetColor(BorderColorId, Color.Lerp(Fighting.Border, Defeated.Border, t));
        _borderLapSeconds = Mathf.Lerp(Fighting.BorderLapSeconds, Defeated.BorderLapSeconds, t);
        _mat.SetFloat(BallSizeId, Mathf.Lerp(Fighting.BallSize, Defeated.BallSize, t));
    }
}
