using UnityEngine;

// Boss look for the final enemy (The Night Creeper): a breathing pulse, a violet void aura behind the body, and a hit
// reaction (aura flare + violet sparks) instead of the usual white flash/circle, so he stays visible under fire.
// Added at runtime by ActorBase.Awake, so the prefab doesn't need it.
public class BossLook : MonoBehaviour
{
    const float BreathSpeed = 2.2f;
    const float BreathAmount = 0.07f;
    const float AuraScale = 1.25f;
    const float AuraAlphaMin = 0.45f;
    const float AuraAlphaMax = 0.75f;
    const float HitFlareTime = 0.15f;
    const float HitFlareScale = 0.12f;   // extra aura scale at the start of a flare
    const float HitSparkInterval = 0.06f;
    const int HitSparks = 4;
    static readonly Color AuraColor = new Color(0.65f, 0.25f, 1f, 1f);
    static readonly Color SparkA = new Color(0.75f, 0.35f, 1f, 1f);
    static readonly Color SparkB = new Color(1f, 0.55f, 1f, 1f);

    ActorBase _actor;
    SpriteRenderer _body;
    SpriteRenderer _aura;
    float _baseScaleY;
    float _phase;
    float _flare;          // 1 right after a hit, fades to 0
    float _nextSparks;

    void Awake()
    {
        _actor = GetComponent<ActorBase>();
        _body = GetComponent<SpriteRenderer>();
        _baseScaleY = transform.localScale.y;

        var go = new GameObject("VoidAura");
        go.transform.SetParent(transform, false);
        _aura = go.AddComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        _phase = Random.value * Mathf.PI * 2;
        _flare = 0f;
    }

    public void OnHit()
    {
        _flare = 1f;

        float now = Time.time;
        if (now < _nextSparks)
            return;
        _nextSparks = now + HitSparkInterval;

        var trail = Particles.I.ClickTrail;
        float radius = 0.4f * Mathf.Abs(transform.localScale.x);
        for (int i = 0; i < HitSparks; ++i)
        {
            trail.transform.position = (Vector2)transform.position + Random.insideUnitCircle * radius;
            trail.Emit(new ParticleSystem.EmitParams { startColor = Color.Lerp(SparkA, SparkB, Random.value) }, 1);
        }
    }

    void LateUpdate()
    {
        if (_actor.IsDead)
        {
            _aura.enabled = false;
            return;
        }

        float s = Mathf.Sin(Time.time * BreathSpeed + _phase);
        _flare = Mathf.Max(0f, _flare - Time.deltaTime / HitFlareTime);

        var scale = transform.localScale;
        scale.y = _baseScaleY * (1f + BreathAmount * s);
        transform.localScale = scale;

        // Child of the body, so it already follows position and flip; just mirror the current animation frame.
        _aura.enabled = true;
        _aura.sprite = _body.sprite;
        _aura.flipX = _body.flipX;
        _aura.sortingLayerID = _body.sortingLayerID;
        _aura.sortingOrder = _body.sortingOrder - 1;
        _aura.transform.localScale = Vector3.one * (AuraScale + HitFlareScale * _flare);
        var c = Color.Lerp(AuraColor, Color.white, 0.35f * _flare);
        c.a = Mathf.Lerp(Mathf.Lerp(AuraAlphaMin, AuraAlphaMax, 0.5f + 0.5f * s), 1f, _flare);
        _aura.color = c;
    }
}
