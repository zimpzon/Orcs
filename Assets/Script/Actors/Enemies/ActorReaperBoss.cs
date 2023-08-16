using System.Collections;
using TMPro;
using UnityEngine;

public class ActorReaperBoss : MonoBehaviour, IKillableObject
{
    public TextMeshPro OverheadText;

    public float FloatSpeed = 4;
    public float FloatScale = 0.1f;

    public float SpriteMinScaleY = 0.9f;
    public float SpriteMaxScaleY = 1.0f;
    public float SpriteScaleSpeed = 4;

    public SpriteRenderer BodyRenderer;
    public Transform BodyTransform;

    Transform trans_;
    const float FlyMaxHeight = 1.5f;
    const float FlySpeed = 2;
    float flyOffset_;
    bool isFlying_;

    public void Fly()
    {
        isFlying_ = true;
        GameManager.Instance.MakePoof(trans_.position, 4, 0.5f);
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerThrowBomb);
    }

    public void Land()
    {
        isFlying_ = false;
    }

    public void Kill()
    {
        Reset();
        StopAllCoroutines();
    }

    void Reset()
    {
        flyOffset_ = 0;
        isFlying_ = false;
    }

    void OnDisable()
    {
        Kill();
    }

    void Awake()
    {
        trans_ = transform;
    }
    void OnEnable()
    {
        Reset();
        OverheadText.text = string.Empty;
        StartCoroutine(Think());
    }

    IEnumerator Think()
    {
        while (true)
        {
            if (isFlying_ && flyOffset_ < FlyMaxHeight)
            {
                flyOffset_ += FlySpeed * G.D.GameDeltaTime;
            }

            GameManager.SetDebugOutput("flyOffset_", flyOffset_);

            if (!isFlying_ && flyOffset_ > 0)
            {
                flyOffset_ -= FlySpeed * G.D.GameDeltaTime;
                flyOffset_ = Mathf.Max(0, flyOffset_);

                if (flyOffset_ == 0)
                {
                    GameManager.Instance.MakePoof(trans_.position, 4, 1.0f);
                    GameManager.Instance.MakeFlash(trans_.position, 4.0f);
                    Explosions.Push(trans_.position, radius: 1.5f, force: 2.0f);
                }
            }

            yield return null;
        }
    }

    public IEnumerator Speak(string text, float pause, bool sound = true)
    {
        OverheadText.text = text;
        OverheadText.maxVisibleCharacters = 0;

        var letterDelay = new WaitForSeconds(0.05f);
        while (OverheadText.maxVisibleCharacters < text.Length)
        {
            if (OverheadText.maxVisibleCharacters++ % 2 == 0)
                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.ShortDeepBump, volumeScale: 0.5f, pitch: 1.2f + Random.value * 1.0f);

            yield return letterDelay;
        }

        // stop clip early
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.Ackack, volumeScale: 0.05f, pitch: 0.05f);

        yield return new WaitForSeconds(pause);
    }

    private void Update()
    {
        ActorBase.PlayerClosestEnemy = trans_;
        ActorBase.PlayerDistanceToClosestEnemy = Vector2.Distance(trans_.position, G.D.PlayerPos);

        float y = (Mathf.Sin(Time.time * FloatSpeed) + 1); // 0 - 2
        y *= FloatScale;
        y -= 0.07f;
        y += flyOffset_;

        var pos = BodyTransform.localPosition;
        pos.y = y;
        BodyTransform.localPosition = pos;

        var scale = BodyTransform.localScale;
        float scaleY = (Mathf.Sin(Time.time * SpriteScaleSpeed) + 1) * 0.5f; // 0 - 1
        float range = SpriteMaxScaleY - SpriteMinScaleY;
        scale.y = SpriteMinScaleY + scaleY * range;
        BodyTransform.localScale = scale;
    }
}
