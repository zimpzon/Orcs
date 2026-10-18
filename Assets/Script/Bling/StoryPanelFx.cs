using UnityEngine;

// Drives the metaball backdrop of the Scary Earl story text under the arena HP bar (Custom/SpriteMetaballs): sinister
// violet ooze while he's undefeated, easing to calm warm gold once he's beaten. Keeps the blobs round on the wide
// panel (_Aspect from the scale). Its own material instance, unscaled time for the palette blend.
[RequireComponent(typeof(SpriteRenderer))]
public class StoryPanelFx : MonoBehaviour
{
    const float BlendTime = 1.5f;

    struct Palette
    {
        public Color Bg, Ball, Rim;
        public float Speed, BallSize;
    }

    static readonly Palette Fighting = new()
    {
        Bg = new Color(0f, 0f, 0f, 0.35f),
        Ball = new Color(0.25f, 0.05f, 0.35f, 0.55f),
        Rim = new Color(0.75f, 0.3f, 1f, 0.6f),
        Speed = 0.35f,
        BallSize = 0.28f,
    };

    static readonly Palette Defeated = new()
    {
        Bg = new Color(0.05f, 0.03f, 0.01f, 0.65f),
        Ball = new Color(0.14f, 0.08f, 0.06f, 0.65f),
        Rim = new Color(0.6f, 0.2f, 0.1f, 0.55f),
        Speed = 0.18f,
        BallSize = 0.31f,
    };

    //Bg = new Color(0.05f, 0.03f, 0.01f, 0.45f),
    //    Ball = new Color(0.34f, 0.24f, 0.06f, 0.5f),
    //    Rim = new Color(0.85f, 0.68f, 0.25f, 0.4f),

    static readonly int BgId = Shader.PropertyToID("_BgColor");
    static readonly int BallId = Shader.PropertyToID("_BallColor");
    static readonly int RimId = Shader.PropertyToID("_RimColor");
    static readonly int PhaseId = Shader.PropertyToID("_Phase");
    static readonly int BallSizeId = Shader.PropertyToID("_BallSize");
    static readonly int AspectId = Shader.PropertyToID("_Aspect");

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
        _mat.SetFloat(BallSizeId, Mathf.Lerp(Fighting.BallSize, Defeated.BallSize, t));
    }
}
