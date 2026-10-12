using UnityEngine;

// Boss look for the final enemy (The Night Creeper): a slow breathing pulse and a purple void aura behind the body.
// Added at runtime by ActorBase.Awake, so the prefab doesn't need it.
public class BossLook : MonoBehaviour
{
    const float BreathSpeed = 2.2f;
    const float BreathAmount = 0.05f;
    const float AuraScale = 1.18f;
    const float AuraAlphaMin = 0.35f;
    const float AuraAlphaMax = 0.6f;
    static readonly Color AuraColor = new Color(0.45f, 0.1f, 0.8f, 1f);

    ActorBase _actor;
    SpriteRenderer _body;
    SpriteRenderer _aura;
    float _baseScaleY;
    float _phase;

    void Awake()
    {
        _actor = GetComponent<ActorBase>();
        _body = GetComponent<SpriteRenderer>();
        _baseScaleY = transform.localScale.y;

        var go = new GameObject("VoidAura");
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one * AuraScale;
        _aura = go.AddComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        _phase = Random.value * Mathf.PI * 2;
    }

    void LateUpdate()
    {
        if (_actor.IsDead)
        {
            _aura.enabled = false;
            return;
        }

        float s = Mathf.Sin(Time.time * BreathSpeed + _phase);

        var scale = transform.localScale;
        scale.y = _baseScaleY * (1f + BreathAmount * s);
        transform.localScale = scale;

        // Child of the body, so it already follows position and flip; just mirror the current animation frame.
        _aura.enabled = true;
        _aura.sprite = _body.sprite;
        _aura.flipX = _body.flipX;
        _aura.sortingLayerID = _body.sortingLayerID;
        _aura.sortingOrder = _body.sortingOrder - 1;
        var c = AuraColor;
        c.a = Mathf.Lerp(AuraAlphaMin, AuraAlphaMax, 0.5f + 0.5f * s);
        _aura.color = c;
    }
}
