using TMPro;
using UnityEngine;

public class HpBarScript : MonoBehaviour
{
    public Transform ForegroundSprite;
    public Transform ScaleRoot;
    public TextMeshPro HpText;
    public long CurrentHp = 0;
    public long MaxHp = 100;
    public Transform FillTransform;

    // Damage trail: a darker shade of the fill behind it that stays at the pre-hit value for a moment and then drains
    // down to the fill, so chunks of lost health are visible. Big chunks shake the bar a bit.
    const float HoldTime = 0.35f;        // trail waits this long after the last hit before draining
    const float MaxHold = 0.8f;          // ...but never longer than this after it last caught up (sustained damage)
    const float DrainRate = 6f;          // ease-out drain speed (exponential)
    const float MinDrainSpeed = 0.15f;   // bar fractions per second, so the tail never crawls
    const float ChunkForShake = 0.04f;   // fraction of max HP lost in one hit to shake the bar
    const float ShakeTime = 0.15f;
    const float ShakeMax = 0.04f;        // world units
    const float TrailDarken = 0.55f;     // trail color = fill color * this

    Transform _trail;
    SpriteRenderer _trailRenderer;
    float _fillFraction;
    float _trailFraction;
    float _holdUntil;
    float _caughtUpTime;
    float _shakeT;
    float _shakeAmp;
    Vector3 _basePosition;

    void Awake()
    {
        _basePosition = transform.localPosition;

        var trailGo = Instantiate(FillTransform.gameObject, FillTransform.parent, false);
        trailGo.name = "Trail";
        _trail = trailGo.transform;
        _trail.localPosition = FillTransform.localPosition;
        _trail.localScale = FillTransform.localScale;

        var fillRenderer = FillTransform.GetComponent<SpriteRenderer>();
        _trailRenderer = trailGo.GetComponent<SpriteRenderer>();
        _trailRenderer.sortingOrder = fillRenderer.sortingOrder - 1;
        var fillColor = fillRenderer.color;
        _trailRenderer.color = new Color(fillColor.r * TrailDarken, fillColor.g * TrailDarken, fillColor.b * TrailDarken, fillColor.a);

        _fillFraction = _trailFraction = FillTransform.localScale.x;
    }

    public long AddHp(long amount)
    {
        SetHp(CurrentHp + amount, MaxHp);
        return CurrentHp;
    }

    public void SetHp(long current, long max)
    {
        if (current < 0) current = 0;

        CurrentHp = current;
        MaxHp = max;
        var scale = FillTransform.localScale;
        scale.x = max == 0 ? 0 : (float)current / max;
        FillTransform.localScale = scale;
        HpText.text = $"{Format512.FormatWithDecimals(current, alwaysThreeDecimalsForLargeNumbers: true)}/{Format512.FormatWithDecimals(max, alwaysThreeDecimalsForLargeNumbers: true)}";
        //HpText.text = $"{(long)current}/{(long)max}";

        UpdateTrailTarget(scale.x);
    }

    void UpdateTrailTarget(float newFraction)
    {
        float oldFraction = _fillFraction;
        _fillFraction = newFraction;
        if (_trail == null)
            return;

        float now = Time.unscaledTime;
        if (newFraction >= oldFraction)
        {
            // Healed or reset (new round): no trail.
            _trailFraction = newFraction;
            _caughtUpTime = now;
            ApplyTrail();
            return;
        }

        _trailFraction = Mathf.Max(_trailFraction, oldFraction);
        _holdUntil = Mathf.Min(now + HoldTime, _caughtUpTime + MaxHold);

        float chunk = oldFraction - newFraction;
        if (chunk >= ChunkForShake)
        {
            _shakeT = ShakeTime;
            _shakeAmp = Mathf.Max(_shakeAmp * (_shakeT / ShakeTime), Mathf.Min(ShakeMax, ShakeMax * chunk / 0.2f));
        }
    }

    void ApplyTrail()
    {
        var scale = _trail.localScale;
        scale.x = _trailFraction;
        _trail.localScale = scale;
    }

    void Update()
    {
        if (_trail == null)
            return;

        float dt = Time.unscaledDeltaTime;
        float now = Time.unscaledTime;

        if (_trailFraction > _fillFraction)
        {
            if (now >= _holdUntil)
            {
                float gap = _trailFraction - _fillFraction;
                float step = Mathf.Max(MinDrainSpeed * dt, gap * (1f - Mathf.Exp(-DrainRate * dt)));
                _trailFraction = Mathf.MoveTowards(_trailFraction, _fillFraction, step);
                if (_trailFraction <= _fillFraction)
                    _caughtUpTime = now;
            }
        }
        else
        {
            _trailFraction = _fillFraction;
            _caughtUpTime = now;
        }
        ApplyTrail();

        // Small decaying vertical shake for big chunks.
        if (_shakeT > 0f)
        {
            _shakeT = Mathf.Max(0f, _shakeT - dt);
            float k = _shakeT / ShakeTime;
            float offset = Mathf.Sin(now * 90f) * _shakeAmp * k;
            transform.localPosition = _basePosition + new Vector3(0f, offset, 0f);
            if (_shakeT <= 0f)
            {
                _shakeAmp = 0f;
                transform.localPosition = _basePosition;
            }
        }
    }
}
