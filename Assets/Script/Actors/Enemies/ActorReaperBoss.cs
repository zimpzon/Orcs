using System;
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
    public Transform ScytheTransform;
    public CapsuleCollider2D Collider;
    public ActorBase Actor;

    Color bodyBaseColor_;
    Transform trans_;
    const float FlyMaxHeight = 1.5f;
    const float FlySpeed = 2;
    float flyOffset_;
    bool isFlying_;
    bool inDeathSequence_;
    [NonSerialized] public bool FightComplete;
    [NonSerialized] public float? ForcedScaleX;

    void Awake()
    {
        trans_ = transform;
        bodyBaseColor_ = BodyRenderer.color;
    }

    void OnEnable()
    {
        Reset();
        OverheadText.text = string.Empty;
        StartCoroutine(Think());
    }

    public void Fly()
    {
        isFlying_ = true;
        GameManager.Instance.MakePoof(trans_.position, 4, 0.5f);
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerThrowBomb);
        Collider.enabled = false;
    }

    public void Land()
    {
        isFlying_ = false;
    }

    public void Kill()
    {
        GameObject.Destroy(this.gameObject);
    }

    void Reset()
    {
        StopAllCoroutines();
        OverheadText.text = "";
        FightComplete = false;
        flyOffset_ = 0;
        isFlying_ = false;
        FightComplete = false;
        inDeathSequence_ = false;
        Collider.enabled = true;
        ForcedScaleX = null;
        BodyRenderer.color = bodyBaseColor_;
    }

    IEnumerator Think()
    {
        while (true)
        {
            if (isFlying_ && flyOffset_ < FlyMaxHeight)
            {
                flyOffset_ += FlySpeed * G.D.GameDeltaTime;
            }

            if (!isFlying_ && flyOffset_ > 0)
            {
                flyOffset_ -= FlySpeed * G.D.GameDeltaTime;
                flyOffset_ = Mathf.Max(0, flyOffset_);

                if (flyOffset_ == 0)
                {
                    Explosions.Push(trans_.position, radius: 3.5f, force: 2.0f);
                    Collider.enabled = true;
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
            if (OverheadText.maxVisibleCharacters++ % 2 == 0 && sound)
                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.ShortDeepBump, volumeScale: 0.8f, pitch: 1.2f + UnityEngine.Random.value * 1.0f);

            yield return letterDelay;
        }

        yield return new WaitForSeconds(pause);
    }

    Chapter1Controller C;

    public void StartFight(Chapter1Controller controller)
    {
        C = controller;
        StartCoroutine(Fight());
    }

    public IEnumerator Fight()
    {
        C.HpBar.Show();

        // enable player
        G.D.PlayerScript.StopPuppet(moveToBegin: true);

        while (G.D.PlayerScript.IsPuppet)
            yield return null;

        GameManager.Instance.Orc.SetPosition(PositionUtility.GetPointInsideArena(), startingGame: true);
        PlayerUpgrades.Data.OrcReviveTimeMul *= 3.0f;

        const float BossSpeed = 5.0f;
        G.D.PlayerScript.TryEnableToggledEffects();

        yield return Chapter1BossUtil.MoveBoss(transform, Vector2.zero + C.BossOffsetY, BossSpeed);

        StartCoroutine(Chapter1BossUtil.FollowPlayer(this));
        StartCoroutine(Chapter1BossUtil.ThrowFlasks(this, C.AcidFlaskProto));

        bool armySpawned = false;

        while (true)
        {
            GameManager.Instance.Orc.SetChasePlayer(chase: true);

            yield return Chapter1BossUtil.FireballSpiral(transform, 5.0f);
            yield return new WaitForSeconds(5.0f);
            yield return Chapter1BossUtil.Bombard(this, C.AcidFlaskProto);
            yield return new WaitForSeconds(5.0f);

            if (!armySpawned)
            {
                armySpawned = true;
                yield return Chapter1BossUtil.SpawnArmy(this);
            }
        }
    }

    IEnumerator DeathSequence()
    {
        Collider.enabled = false;
        GameManager.Instance.Orc.ResetAll();

        yield return Chapter1BossUtil.DeathSequence(this);

        FightComplete = true;
        inDeathSequence_ = false;
    }

    private void Update()
    {
        if (inDeathSequence_ || FightComplete)
            return;

        if (Actor.IsDead)
        {
            StopAllCoroutines();
            StartCoroutine(DeathSequence());
            var bodyPos = BodyTransform.localPosition;
            bodyPos.y = 0;
            BodyTransform.localPosition = bodyPos;
            inDeathSequence_ = true;
            return;
        }

        float y = (Mathf.Sin(Time.time * FloatSpeed) + 1); // 0 - 2,
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
        float playerDir = trans_.position.x - G.D.PlayerPos.x < 0 ? -1 : 1;
        scale.x = ForcedScaleX.HasValue ? ForcedScaleX.Value : playerDir;
        BodyTransform.localScale = scale;
    }
}
