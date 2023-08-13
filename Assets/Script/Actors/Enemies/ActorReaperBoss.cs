using System.Collections;
using UnityEngine;

public class ActorReaperBoss : MonoBehaviour
{
    public float FloatSpeed = 4;
    public float FloatScale = 0.1f;
    public float FloatOffset = 0.1f;

    public float SpriteMinScaleY = 0.9f;
    public float SpriteMaxScaleY = 1.0f;
    public float SpriteScaleSpeed = 4;

    public SpriteRenderer BodyRenderer;
    public Transform BodyTransform;

    Transform trans_;

    void Awake()
    {
        trans_ = transform;
    }

    void OnEnable()
    {
        trans_.position = Vector2.zero + Vector2.up * 2 + Vector2.right * 2;
        gameObject.SetActive(true);

        StartCoroutine(Think());
    }

    IEnumerator Think()
    {
        yield return null;
    }

    private void Update()
    {
        ActorBase.PlayerClosestEnemy = trans_;
        ActorBase.PlayerDistanceToClosestEnemy = Vector2.Distance(trans_.position, G.D.PlayerPos);

        float y = (Mathf.Sin(Time.time * FloatSpeed) + 1); // 0 - 2
        y *= FloatScale;
        y -= -FloatOffset;
        var pos = BodyTransform.position;
        pos.y = -y;
        BodyTransform.position = pos;

        var scale = BodyTransform.localScale;
        float scaleY = (Mathf.Sin(Time.time * SpriteScaleSpeed) + 1) * 0.5f; // 0 - 1
        float range = SpriteMaxScaleY - SpriteMinScaleY;
        scale.y = SpriteMinScaleY + scaleY * range;
        BodyTransform.localScale = scale;
    }
}
