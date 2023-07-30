﻿using Assets.Script;
using Assets.Script.Enemies;
using System.Collections;
using UnityEngine;

public enum ActorTypeEnum
{
    None, Any, Ogre, OgreBandana, OgreBandanaGun, OgreLarge, OgreShaman, OgreShamanStaff, OgreShamanStaffLarge,
    OgreSmall, OrcBronze, OrcBronzeShield, OrcIron, OrcIronCyclops, OrcIronCyclopsShield, OrcIronShield, OrcPlain,
    OrcPlainShield, OrcWhiteMask, OrcWhiteMaskShield, PirateBandana, PirateBandanaGun, PirateDuck, PirateFancyGun,
    PirateNoShirt, PirateNoShirtGun, PirateRedBeard, PirateRedBeardGun, Skeleton,
};

public class ActorBase : MonoBehaviour
{
    public Sprite[] Animations;
    public bool CanBeFrozen = true;
    public bool CanBePoisened = true;
    public float Speed = 1.0f;
    public bool AvoidCrowds = true;
    public bool IsBoss = false;
    public float Mass = 1.0f;
    public int XpValue = 1;
    public ActorTypeEnum ActorType;
    public float BaseHp;
    [System.NonSerialized] public float TimeBorn;
    [System.NonSerialized] public float TimeDied;
    [System.NonSerialized] public float Hp = 50;

    const float PaintBallTickTime = 1.0f;

    public static void ResetClosestEnemy()
    {
        PlayerClosestEnemy = null;
        PlayerDistanceToClosestEnemy = float.MaxValue;
    }

    [System.NonSerialized] public static float Damage = 25.0f;
    [System.NonSerialized] public bool IsFullyReady = false;
    [System.NonSerialized] public bool UseSpawnParticles = false;

    [System.NonSerialized] public static ActorBase PlayerClosestEnemy;
    [System.NonSerialized] public static float PlayerDistanceToClosestEnemy;

    [System.NonSerialized] public float RadiusBody = 0.4f;
    [System.NonSerialized] public Vector3 BodyOffset = new Vector3(0.0f, -0.25f, 0.0f);

    [System.NonSerialized] public bool IsCorpse;
    [System.NonSerialized] public bool IsDead;

    private bool hasForcedDestination_;
    private Vector3 forcedDestination_;
    protected bool despawnOnForcedDestinationReached_;

    protected GameModeData GameMode;
    protected float DecayTime = 10.0f;
    protected AnimationController animationController_ = new AnimationController();

    protected Vector3 position_;
    protected float massInverse_;
    protected Vector3 force_;
    protected Transform transform_;
    protected SpriteRenderer renderer_;
    protected Material material_;
    protected int flashParamId_;
    protected int flashColorParamId_;
    protected float flashEndTime_;
    protected float nextCheckForCrowded_;

    protected virtual void PreEnable() { }
    protected virtual void PostEnable() { }

    protected virtual void PreUpdate() { }
    protected virtual void PostUpdate() { }

    protected virtual void OnDeath() { }

    protected float slowmotionModifier_ = 1.0f;

    protected bool isPainted_;
    private float nextPaintDamage_;
    protected bool isFrozen_;
    protected bool isLivingBomb_;
    protected bool isSpawning_ = true;

    float paintEnd_;
    Color paintColor_;
    float frozenEnd_;
    Color frozenColor_;
    float livingBombDamage_;
    float livingBombEnd_;
    Vector3 scale_;

    // TODO PE: Hmm easy to forget something here. Subclasses may also need a reset. Hopefully it will be easily detectable. Or not.
    public void ResetForCaching()
    {
        StopAllCoroutines();
        Hp = BaseHp;
        transform_.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        IsCorpse = false;
        IsDead = false;
        UseSpawnParticles = false;
        isPainted_ = false;
        nextPaintDamage_ = float.MaxValue;
        isFrozen_ = false;
        isLivingBomb_ = false;
        isSpawning_ = true;
        IsFullyReady = false;
        slowmotionModifier_ = 1.0f;
        flashEndTime_ = 0.0f;
        force_ = Vector3.zero;
        hasForcedDestination_ = false;
        forcedDestination_ = Vector3.zero;
        despawnOnForcedDestinationReached_ = false;
        material_.color = Color.white;
    }

    public void Awake()
    {
        TimeBorn = Time.time;
        transform_ = this.transform;

        renderer_ = GetComponent<SpriteRenderer>();
        material_ = renderer_.material;
        flashParamId_ = Shader.PropertyToID("_FlashAmount");
        flashColorParamId_ = Shader.PropertyToID("_FlashColor");

        // Assume uniform scale
        RadiusBody *= transform_.localScale.x;
        BodyOffset *= transform_.localScale.x;

        scale_ = transform_.localScale;
    }

    public void Start()
    {
        gameObject.layer = GameManager.Instance.LayerNeutral;
        massInverse_ = 1.0f / Mass;
    }

    public void OnEnable()
    {
        // This is annoying. OnEnable is called right after Awake (eg before Start) when the cache precreates the objects, if the prefab is enabled.
        // On the other hand, if the prefab is NOT enabled, the first OnEnable call is called AFTER Start. So we don't know which is which here.
        if (GameManager.Instance == null)
            return;

        GameMode = GameManager.Instance.CurrentGameModeData;
        PreEnable();
        GameManager.Instance.RegisterEnemy(this);

        if (UseSpawnParticles)
        {
            StartCoroutine(SpawnAnimCo());
        }

        isSpawning_ = false;

        PostEnable();
    }

    public void OnLivingBombHit(float damage, bool playSound = true)
    {
        if (isLivingBomb_)
            return;

        livingBombDamage_ = damage;
        isLivingBomb_ = true;

        const float BombTime = 1.0f;
        const float BombRnd = 0.1f;
        livingBombEnd_ = Time.time + BombTime + Random.value * BombRnd;

        material_.SetColor(flashColorParamId_, new Color(1.0f, 0.6f, 0.6f));
        GameManager.Instance.MakeFlash(transform_.position);
        GameManager.Instance.MakePoof(transform_.position, 5, 1.0f);
        GameManager.Instance.ShakeCamera(0.1f);
        if (playSound)
            AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.AudioData.PlayerStaffHit);
    }

    private void UpdateLivingBomb()
    {
        if (!isLivingBomb_)
            return;

        if (Time.time >= livingBombEnd_)
        {
            isLivingBomb_ = false;
            StartCoroutine(ExplodeCo(0.0f));
            return;
        }

        float flashAmount = ((int)(Time.time * 6) & 1) == 0 ? 0.0f : 0.8f;
        material_.SetFloat(flashParamId_, flashAmount);
        if (Random.value < 0.01f)
            GameManager.Instance.MakePoof(transform_.position, 1, 1.0f);
    }

    protected void UpdatePosition(Vector3 moveVec, float speed)
    {
        if (!IsFullyReady)
        {
            // Force towards center
            Vector3 centerDir = (Vector3.zero - position_).normalized;
            moveVec = (moveVec + centerDir).normalized;
        }

        if (hasForcedDestination_)
        {
            moveVec = (forcedDestination_ - position_).normalized;
        }

        if (isLivingBomb_)
            speed *= 0.9f;
        else if (isFrozen_)
            speed *= 0.001f;
        else if (isPainted_)
        {
            // paintball slow is proportional to mass
            float slowEffect = Mass / 5.0f * PlayerUpgrades.Data.PaintballBaseSlowMul;
            speed *= Mathf.Clamp(slowEffect, 0.25f, 0.9f);
        }

        position_ += moveVec * speed * slowmotionModifier_ * Time.deltaTime;

        Vector3 scale = scale_;
        scale.x = moveVec.x < 0 ? -scale.x : scale.x;

        transform_.localScale = scale;
    }

    void CheckForCrowded()
    {
        if (!AvoidCrowds || Time.time < nextCheckForCrowded_)
            return;

        int crowdCount = BlackboardScript.CountEnemies(position_, radius: 1.0f);
        if (crowdCount > 2)
        {
            if (Random.value > 0.25f)
            {
                forcedDestination_ = position_ + (Vector3)Random.insideUnitCircle.normalized * 2.0f;
                forcedDestination_ = GameManager.Instance.ClampToBounds(forcedDestination_, renderer_.sprite);
                hasForcedDestination_ = true;
                //FloatingTextSpawner.Instance.Spawn(position_, "CROWD!", Color.yellow);
            }
        }
        nextCheckForCrowded_ = Time.time + 0.3f + Random.value * 0.5f;
    }

    public bool OnFreeze(Color color, float freezeTime)
    {
        frozenColor_ = color;
        isFrozen_ = true;
        frozenEnd_ = Time.time + freezeTime;
        return true;
    }

    public bool OnPaintballHit(Color color, float paintTime)
    {
        isPainted_ = true;
        nextPaintDamage_ = GameManager.Instance.GameTime + PaintBallTickTime;
        paintColor_ = color;
        paintEnd_ = GameManager.Instance.GameTime + paintTime;
        return true;
    }

    public void Update()
    {
        if (Time.time > flashEndTime_)
            material_.SetFloat(flashParamId_, 0.0f);

        if (isFrozen_)
            material_.color = Color.Lerp(Color.white, frozenColor_, 0.6f);
        else if (isPainted_)
        {
            material_.color = Color.Lerp(Color.white, paintColor_, 0.8f);
            if (GameManager.Instance.GameTime > nextPaintDamage_)
            {
                GameManager.Instance.DamageEnemy(this, PlayerUpgrades.Data.PaintballBaseDamagePerSec * PlayerUpgrades.Data.PaintballDamagePerSecMul, Vector3.zero, 0.01f);
                nextPaintDamage_ = GameManager.Instance.GameTime + PaintBallTickTime;
            }
        }

        if (!isFrozen_)
        {
            CheckForCrowded();

            bool forcedDistinationReached = hasForcedDestination_ && Vector3.Distance(forcedDestination_, position_) < 0.2f;
            if (forcedDistinationReached)
            {
                //FloatingTextSpawner.Instance.Spawn(position_, "Avoided", Color.cyan);
                hasForcedDestination_ = false;
                if (despawnOnForcedDestinationReached_)
                {
                    StopAllCoroutines();
                    ReturnToCache();
                }
            }

            force_ *= 1.0f - (30.0f * Time.deltaTime);
            position_ += force_ * Time.deltaTime * 60; 
            transform_.position = position_;
        }

        if (isSpawning_)
            return;

        PreUpdate();
        CheckPainted();
        CheckFrozen();
        UpdateLivingBomb();
        CheckFullyReady();

        position_.z = 0;

        IsDead = Hp <= 0.0f;

        if (IsFullyReady)
        {
            position_ = GameManager.Instance.ClampToBounds(position_, renderer_.sprite);

            if (!IsDead)
            {
                float distanceToPlayer = BlackboardScript.DistanceToPlayer(position_);
                if (distanceToPlayer < PlayerDistanceToClosestEnemy)
                {
                    PlayerClosestEnemy = this;
                    PlayerDistanceToClosestEnemy = distanceToPlayer;
                }
            }
        }

        if (!IsDead && !isFrozen_)
        {
            animationController_.Tick(Time.deltaTime, renderer_, Animations);
        }

        renderer_.sortingOrder = (Mathf.RoundToInt(transform_.position.y * 100f) * -1) - (IsDead ? 10000 : 0);

        const float RegenTime = 1.0f;
        slowmotionModifier_ = Mathf.Clamp01(slowmotionModifier_ + Time.deltaTime / RegenTime);

        PostUpdate();
    }

    public void SetSlowmotion(float amount = 0.0f)
    {
        slowmotionModifier_ = amount;
    }

    private void CheckPainted()
    {
        if (!isPainted_)
            return;

        if (Time.time > paintEnd_)
        {
            isPainted_ = false;
            material_.color = Color.white;
        }
    }

    private void CheckFrozen()
    {
        if (!isFrozen_)
            return;

        if (Time.time > frozenEnd_)
        {
            isFrozen_ = false;
            material_.color = Color.white;
        }
    }

    public void AddForce(Vector3 force)
    {
        force_ += force;
    }

    public void ApplyDamage(float amount, Vector3 direction, float forceModifier)
    {
        if (amount > 0)
        {
            Hp -= amount;
            GameManager.Instance.TriggerBlood(transform_.position, 1.0f + (amount * 0.25f) * forceModifier);
        }

        if (Hp <= 0.0f)
        {
            Hp = 0;
            GameManager.Instance.OnEnemyKill(this);
            OnDeath();
            StopAllCoroutines();
            StartCoroutine(DieAnimation(direction, forceModifier));
        }
        else
        {
            float force = Mathf.Clamp(amount * 0.2f, 0.1f, 3.0f);
            AddForce(direction * (force * 0.2f * massInverse_ * forceModifier));
            if (amount > 0.01f)
            {
                material_.SetFloat(flashParamId_, 0.75f);
                flashEndTime_ = Time.time + 0.03f;
            }
        }
    }

    public void Explode(float delay)
    {
        StartCoroutine(ExplodeCo(delay));
    }

    IEnumerator SpawnAnimCo()
    {
        isSpawning_ = true;
        float endTime = Time.time + 1.0f;
        material_.SetColor(flashColorParamId_, new Color(0.3f, 0.3f, 0.3f));
        float flashAmount = 1.0f;
        while (Time.time < endTime)
        {
            Vector3 basePos = transform_.position;
            Vector3 pos = basePos;
            Vector3 cloudPos = pos + Vector3.down * 0.25f;

            material_.SetFloat(flashParamId_, flashAmount);
            flashAmount += Time.deltaTime * 1.0f;
            GameManager.Instance.MakeFlash(pos, 2.0f);
            GameManager.Instance.MakeSpawnPoof(cloudPos, 1);
            yield return new WaitForSeconds(0.2f);
        }

        material_.SetFloat(flashParamId_, 0.0f);
        isSpawning_ = false;
    }

    void CheckFullyReady()
    {
        if (IsFullyReady)
            return;

        bool isFullyReady = !isSpawning_ && GameManager.Instance.IsInsideBounds(transform_.position, renderer_.sprite);
        if (!isFullyReady)
            return;

        IsFullyReady = true;
        gameObject.layer = GameManager.Instance.LayerEnemy;
    }

    IEnumerator ExplodeCo(float delay)
    {
        float explodeTime = Time.time + delay;
        float flashOffset = UnityEngine.Random.value;
        while (Time.time < explodeTime)
        {
            material_.SetFloat(flashParamId_, 0.6f + (Mathf.Sin(((Time.time + flashOffset) * 10) + 1.0f) * 0.5f) * 0.25f);
            yield return null;
        }

        const float ExplodeRadius = 3.0f;
        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.AudioData.LivingBombExplode);
        GameManager.Instance.MakeCircle(transform_.position, ExplodeRadius);
        GameManager.Instance.MakePoof(transform_.position, 2, ExplodeRadius * 0.2f);
        GameManager.Instance.ShakeCamera(0.4f);

        int aliveCount = BlackboardScript.GetEnemies(transform_.position, ExplodeRadius);
        bool isFirstHit = true;
        for (int i = 0; i < aliveCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.Enemies[idx];
            if (enemy != this)
            {
                enemy.OnLivingBombHit(livingBombDamage_ * 0.9f, isFirstHit);
                isFirstHit = false;
            }
        }

        ApplyDamage(livingBombDamage_, RndUtil.RandomInsideUnitCircle().normalized, forceModifier: 0.25f);
    }

    IEnumerator DieAnimation(Vector3 deathSourceDir, float forceModifier)
    {
        TimeDied = Time.time;

        gameObject.layer = GameManager.Instance.LayerEnemyCorpse;

        float direction = transform_.position.x < deathSourceDir.x ? -1 : 1;
        material_.SetColor(flashColorParamId_, new Color(0, 0, 0, 0.5f));
        flashEndTime_ = float.MaxValue;

        float flashAmount = 0.0f;

        Vector3 startPos = position_;
        Vector3 pos = startPos;
        Vector3 jumpOffset = Vector3.zero;
        float downForce = -20.0f;
        float velocityY = 4.0f * massInverse_ * forceModifier;
        float totalRotation = 0;
        Vector3 deathForce = deathSourceDir.normalized * 20 * forceModifier;

        float maxTime = Time.time + 2.0f;
        float bloodEndTime = Time.time + 0.5f;

        while (Time.time < maxTime)
        {
            float delta = Time.deltaTime;

            material_.SetFloat(flashParamId_, flashAmount);
            flashAmount += delta * 2;
            flashAmount = Mathf.Clamp(flashAmount, 0.0f, 0.6f);

            pos += deathForce * delta;

            jumpOffset.y += velocityY * delta;
            velocityY += downForce * delta;
            if (jumpOffset.y <= 0)
            {
                velocityY = -velocityY * 0.1f;
            }

            position_ = pos + jumpOffset;
            position_ = GameManager.Instance.ClampToBounds(position_, renderer_.sprite);

            if (totalRotation < 90)
            {
                float rotationAmount = direction * delta * 200;
                transform_.Rotate(0.0f, 0.0f, rotationAmount);
                totalRotation += Mathf.Abs(rotationAmount);
            }

            deathForce *= 1.0f - (5.0f * delta);

            bool stoppedMoving = deathForce.sqrMagnitude < 0.1f && totalRotation >= 90;
            if (stoppedMoving)
                break;

            if (Time.time < bloodEndTime)
                GameManager.Instance.TriggerBlood(pos, 0.1f);

            yield return null;
        }

        IsCorpse = true;

        StartCoroutine(Decay(DecayTime));
    }

    IEnumerator Decay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopAllCoroutines();
        BlackboardScript.DeadEnemies.Remove(this);
        ReturnToCache();
    }

    public void ReturnToCache()
    {
        ResetForCaching();
        ActorCache.Instance.ReturnObject(gameObject);
    }
}
