using Assets.Script;
using Assets.Script.Enemies;
using System;
using System.Collections;
using UnityEngine;

public enum ActorTypeEnum
{
    None, Any, Ogre, OgreBandana, OgreBandanaGun, OgreLarge, OgreShaman, OgreShamanStaff, OgreShamanStaffLarge,
    OgreSmall, OrcBronze, OrcBronzeShield, OrcIron, OrcIronCyclops, OrcIronCyclopsShield, OrcIronShield, OrcPlain,
    OrcPlainShield, OrcWhiteMask, OrcWhiteMaskShield, PirateBandana, PirateBandanaGun, PirateDuck, PirateFancyGun,
    PirateNoShirt, PirateNoShirtGun, PirateRedBeard, PirateRedBeardGun, Skeleton, OgreEdgy, ReaperBoss, BatWhite, BatRed,
    SpikyBall, OgreIcyShamanStaff,
};

public enum ActorForcedTargetType { Absolute, Direction };

public class ActorBase : MonoBehaviour
{
    const float IgnoreCrowdsWhenCloseToPlayer = 2.0f;
    const float PaintBallTickTime = 1.0f;

    public Sprite[] Animations;
    public bool CanBeFrozen = true;
    public bool CanBePoisened = true;
    public float Speed = 1.0f;
    public bool AvoidCrowds = true;
    public float CrowdScanRadius = 1.0f;
    public int CrowdMaxNearby = 3;
    public float CrowdOutOfTheWayRange = 2.5f;

    public int CacheReviveCount = 0;
    public bool IsBoss = false;
    public float Mass = 1.0f;
    public int XpValue = 1;
    public int XpCount = 1;
    public int GoldCount = 0;
    public ActorTypeEnum ActorType;
    public float BaseHp;
    [System.NonSerialized] public float TimeBorn;
    [System.NonSerialized] public float TimeDied;
    [System.NonSerialized] public float Hp = 50;

    public static void ResetClosestEnemy()
    {
        PlayerClosestEnemy = null;
        PlayerDistanceToClosestEnemy = float.MaxValue;
    }

    [System.NonSerialized] public static float Damage = 25.0f;
    [System.NonSerialized] public bool IsFullyReady = false;
    [System.NonSerialized] public bool UseSpawnParticles = false;

    [System.NonSerialized] public static Transform PlayerClosestEnemy;
    [System.NonSerialized] public static float PlayerDistanceToClosestEnemy;

    [System.NonSerialized] public float RadiusBody = 0.4f;
    [System.NonSerialized] public Vector3 BodyOffset = new Vector3(0.0f, -0.25f, 0.0f);

    [System.NonSerialized] public bool IsCorpse;
    [System.NonSerialized] public bool IsDead;

    private bool hasForcedDestination_;
    private ActorForcedTargetType forcedTargetType_;
    private bool forcedDestinationBreakAtDamage_;
    private Vector3 forcedDestination_;
    protected bool despawnAtForcedDestination_;

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
    protected float distanceToPlayer_;

    protected float slowmotionModifier_ = 1.0f;

    protected bool isPainted_;
    private float nextPaintDamage_;
    private float crowdedWaveRandom_;
    protected bool isFrozen_;
    protected bool isLivingBomb_;
    [NonSerialized] public bool IsSpawning = true;
    [NonSerialized] public int UniqueId;
    static int UniqueIdCounter = 1;

    float paintEnd_;
    Color paintColor_;
    float frozenEnd_;
    Color frozenColor_;
    float livingBombDamage_;
    float livingBombEnd_;
    Vector3 scale_;

    // TODO PE: Hmm easy to forget something here. Subclasses may also need a reset. Hopefully it will be easily detectable. Or not.
    public void Reset(bool init)
    {
        if (!init)
        {
            StopAllCoroutines();
            transform_.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            UseSpawnParticles = false;
        }

        Hp = BaseHp;
        distanceToPlayer_ = float.MaxValue;
        position_ = Vector3.zero;
        IsCorpse = false;
        IsDead = false;
        isPainted_ = false;
        nextPaintDamage_ = float.MaxValue;
        isFrozen_ = false;
        isLivingBomb_ = false;
        IsSpawning = true;
        IsFullyReady = false;
        slowmotionModifier_ = 1.0f;
        flashEndTime_ = 0.0f;
        force_ = Vector3.zero;
        hasForcedDestination_ = false;
        forcedDestination_ = Vector3.zero;
        despawnAtForcedDestination_ = false;
        forcedDestinationBreakAtDamage_ = true;
        nextCheckForCrowded_ = 0;
        material_.color = Color.white;
    }

    public void Awake()
    {
        UniqueId = UniqueIdCounter++;
        TimeBorn = GameManager.Instance.GameTime;
        transform_ = this.transform;

        crowdedWaveRandom_ = UnityEngine.Random.value * 100;

        renderer_ = GetComponent<SpriteRenderer>();
        material_ = renderer_.material;
        flashParamId_ = Shader.PropertyToID("_FlashAmount");
        flashColorParamId_ = Shader.PropertyToID("_FlashColor");

        Reset(init: true);

        // Assume uniform scale
        RadiusBody *= transform_.localScale.x;
        BodyOffset *= transform_.localScale.x;

        scale_ = transform_.localScale;
    }

    public void Start()
    {
        massInverse_ = 1.0f / Mass;
    }

    public void OnEnable()
    {
        //// This is annoying. OnEnable is called right after Awake (eg before Start) when the cache precreates the objects, if the prefab is enabled.
        //// On the other hand, if the prefab is NOT enabled, the first OnEnable call is called AFTER Start. So we don't know which is which here.
        //if (GameManager.Instance == null)
        //    return;

        position_ = transform_.position;
        GameMode = GameManager.Instance.CurrentGameModeData;

        if (UseSpawnParticles)
            StartCoroutine(SpawnAnimCo());
        else
            IsSpawning = false;
    }

    public void OnLivingBombHit(float damage, bool playSound = true)
    {
        if (isLivingBomb_)
            return;

        livingBombDamage_ = damage;
        isLivingBomb_ = true;

        const float BombTime = 1.0f;
        const float BombRnd = 0.1f;
        livingBombEnd_ = GameManager.Instance.GameTime + BombTime + UnityEngine.Random.value * BombRnd;

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

        if (GameManager.Instance.GameTime >= livingBombEnd_)
        {
            isLivingBomb_ = false;
            StartCoroutine(ExplodeCo(0.0f));
            return;
        }

        float flashAmount = ((int)(GameManager.Instance.GameTime * 6) & 1) == 0 ? 0.0f : 0.8f;
        material_.SetFloat(flashParamId_, flashAmount);
        if (UnityEngine.Random.value < 0.01f)
            GameManager.Instance.MakePoof(transform_.position, 1, 1.0f);
    }

    public void UpdatePosition(Vector3 moveVec, float speed)
    {
        if (!IsFullyReady)
        {
            // Force towards center
            Vector3 centerDir = (Vector3.zero - position_).normalized;
            moveVec = (moveVec + centerDir).normalized;
        }

        if (hasForcedDestination_)
        {
            if (forcedTargetType_ == ActorForcedTargetType.Absolute)
            {
                moveVec = (forcedDestination_ - position_).normalized;
            }
            else
            {
                moveVec = forcedDestination_;
            }
        }

        if (isLivingBomb_)
            speed *= 0.9f;
        else if (isFrozen_)
            speed *= 0.001f;
        else if (isPainted_)
        {
            // paintball slow is proportional to mass
            float slowEffect = Mass / 5.0f * PlayerUpgrades.Data.PaintballBaseSlowMul;
            speed *= Mathf.Clamp(slowEffect, 0.6f, 0.9f);
        }

        position_ += moveVec * speed * slowmotionModifier_ * GameManager.Instance.GameDeltaTime;

        Vector3 scale = scale_;
        scale.x = moveVec.x < 0 ? -scale.x : scale.x;

        transform_.localScale = scale;
    }

    public void SetForcedTarget(Vector2 target, bool despawnAtDestination = false, bool breakAtDamage = true, ActorForcedTargetType targetType = ActorForcedTargetType.Absolute)
    {
        hasForcedDestination_ = true;
        forcedTargetType_ = targetType;
        forcedDestination_ = targetType == ActorForcedTargetType.Absolute ? target : (target - (Vector2)position_).normalized;
        despawnAtForcedDestination_ = despawnAtDestination;
        forcedDestinationBreakAtDamage_ = breakAtDamage;
    }

    void CheckForCrowded()
    {
        if (!AvoidCrowds || hasForcedDestination_ || GameManager.Instance.GameTime < nextCheckForCrowded_)
            return;

        // if distance to player is small ignore crowd rules and go for it!
        if (distanceToPlayer_ < IgnoreCrowdsWhenCloseToPlayer && !IsBoss)
            return;

        int crowdCount = BlackboardScript.CountEnemies(position_, radius: CrowdScanRadius);
        if (crowdCount > CrowdMaxNearby)
        {
            float chance = (float)(Math.Sin(GameManager.Instance.GameTime * 0.5f + crowdedWaveRandom_) + 1) * 0.5f;
            if (UnityEngine.Random.value > chance)
            {
                var forcedPos = position_ + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * CrowdOutOfTheWayRange;
                forcedPos = GameManager.Instance.ClampToBounds(forcedPos, renderer_.sprite);
                SetForcedTarget(forcedPos);

                //FloatingTextSpawner.Instance.Spawn(position_, "CROWD!", Color.yellow);
            }
        }
        nextCheckForCrowded_ = GameManager.Instance.GameTime + 1.0f + UnityEngine.Random.value * 0.5f;
    }

    public bool OnFreeze(Color color, float freezeTime)
    {
        if (!CanBeFrozen)
            return false;

        frozenColor_ = color;
        isFrozen_ = true;
        frozenEnd_ = GameManager.Instance.GameTime + freezeTime;
        return true;
    }

    public bool OnPaintballHit(Color color, float paintTime)
    {
        if (!CanBePoisened)
            return false;

        isPainted_ = true;

        // add a little rnd so aoe poisened enemies don't look so synchronized
        float rnd = UnityEngine.Random.value * 0.2f;
        nextPaintDamage_ = GameManager.Instance.GameTime + PaintBallTickTime + rnd;

        paintColor_ = color;
        paintEnd_ = GameManager.Instance.GameTime + paintTime + rnd;
        return true;
    }

    public void LateUpdate()
    {
        IsDead = Hp <= 0.0f;

        if (GameManager.Instance.GameTime > flashEndTime_)
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

        if (!IsDead)
        {
            distanceToPlayer_ = Vector2.Distance(transform_.position, G.D.PlayerPos);
            if (distanceToPlayer_ < PlayerDistanceToClosestEnemy)
            {
                PlayerClosestEnemy = transform_;
                PlayerDistanceToClosestEnemy = distanceToPlayer_;
            }
        }

        if (IsBoss)
        {
            CheckPainted();
            return;
        }

        if (!isFrozen_)
        {
            CheckForCrowded();

            bool forcedDistinationReached;
            if (forcedTargetType_ == ActorForcedTargetType.Absolute)
            {
                forcedDistinationReached = hasForcedDestination_ && Vector3.Distance(forcedDestination_, position_) < 0.2f;
            }
            else
            {
                forcedDistinationReached = IsFullyReady && !GameManager.Instance.IsOutsideBounds(position_);
            }

            if (forcedDistinationReached)
            {
                //FloatingTextSpawner.Instance.Spawn(position_, "reached", Color.cyan);
                hasForcedDestination_ = false;
                if (despawnAtForcedDestination_)
                {
                    GameManager.Instance.MakePoof(position_, 3);
                    GameManager.Instance.MakeFlash(position_, 1);
                    StopAllCoroutines();
                    ReturnToCache();
                    return;
                }
            }

            Vector3 diff = force_ * 25.0f;
            force_ -= diff * GameManager.Instance.GameDeltaTime;
            position_ += force_ * GameManager.Instance.GameDeltaTime * 60;
            transform_.position = position_;
        }

        if (IsSpawning)
            return;

        CheckPainted();
        CheckFrozen();
        UpdateLivingBomb();
        CheckFullyReady();

        position_.z = 0;

        if (IsFullyReady)
        {
            if (!hasForcedDestination_)
                position_ = GameManager.Instance.ClampToBounds(position_, renderer_.sprite);
        }

        if (!IsDead && !isFrozen_)
        {
            animationController_.Tick(GameManager.Instance.GameDeltaTime, renderer_, Animations);
        }

        renderer_.sortingOrder = (Mathf.RoundToInt(transform_.position.y * 100f) * -1) - (IsDead ? 10000 : 0);

        const float RegenTime = 1.0f;
        slowmotionModifier_ = Mathf.Clamp01(slowmotionModifier_ + GameManager.Instance.GameDeltaTime / RegenTime);
    }

    public void SetSlowmotion(float amount = 0.0f)
    {
        slowmotionModifier_ = amount;
    }

    private void CheckPainted()
    {
        if (!isPainted_)
            return;

        if (GameManager.Instance.GameTime > paintEnd_)
        {
            isPainted_ = false;
            material_.color = Color.white;
        }
    }

    private void CheckFrozen()
    {
        if (!isFrozen_)
            return;

        if (GameManager.Instance.GameTime > frozenEnd_)
        {
            isFrozen_ = false;
            material_.color = Color.white;
        }
    }

    public void AddForce(Vector3 force)
    {
        if (IsBoss)
            return;

        if (hasForcedDestination_ && !forcedDestinationBreakAtDamage_)
            return;

        force_ += force;
    }

    public void ApplyDamage(float amount, Vector3 direction, float forceModifier)
    {
        if (amount > 0)
        {
            Hp -= amount;
            GameManager.Instance.TriggerBlood(transform_.position, 1.0f + (amount * 0.25f) * forceModifier);
        }

        if (hasForcedDestination_ && forcedDestinationBreakAtDamage_)
           hasForcedDestination_ = false;

        if (Hp <= 0.0f)
        {
            Hp = 0;
            GameManager.Instance.OnEnemyKill(this);
            StopAllCoroutines();
            if (!IsBoss)
                StartCoroutine(DieAnimation(direction, forceModifier));
        }
        else
        {
            float force = Mathf.Clamp(amount * 0.2f, 0.1f, 3.0f);
            AddForce(direction * (force * 0.2f * massInverse_ * forceModifier));

            if (amount > 0.01f)
            {
                material_.SetFloat(flashParamId_, 0.75f);
                flashEndTime_ = GameManager.Instance.GameTime + 0.03f;
            }
        }
    }

    public void Explode(float delay)
    {
        StartCoroutine(ExplodeCo(delay));
    }

    IEnumerator SpawnAnimCo()
    {
        IsSpawning = true;
        float endTime = GameManager.Instance.GameTime + 1.0f;
        material_.SetColor(flashColorParamId_, new Color(0.3f, 0.3f, 0.3f));
        float flashAmount = 1.0f;
        while (GameManager.Instance.GameTime < endTime)
        {
            Vector3 basePos = transform_.position;
            Vector3 pos = basePos;
            Vector3 cloudPos = pos + Vector3.down * 0.25f;

            material_.SetFloat(flashParamId_, flashAmount);
            flashAmount += GameManager.Instance.GameDeltaTime * 1.0f;
            GameManager.Instance.MakeFlash(pos, 2.0f);
            GameManager.Instance.MakeSpawnPoof(cloudPos, 1);
            yield return new WaitForSeconds(0.2f);
        }

        material_.SetFloat(flashParamId_, 0.0f);
        IsSpawning = false;
    }

    void CheckFullyReady()
    {
        if (IsFullyReady)
            return;

        bool isFullyReady = !IsSpawning && GameManager.Instance.IsInsideBounds(transform_.position, renderer_.sprite);
        if (!isFullyReady)
            return;

        IsFullyReady = true;
        gameObject.layer = GameManager.Instance.LayerEnemy;
    }

    IEnumerator ExplodeCo(float delay)
    {
        float explodeTime = GameManager.Instance.GameTime + delay;
        float flashOffset = UnityEngine.Random.value;
        while (GameManager.Instance.GameTime < explodeTime)
        {
            material_.SetFloat(flashParamId_, 0.6f + (Mathf.Sin(((GameManager.Instance.GameTime + flashOffset) * 10) + 1.0f) * 0.5f) * 0.25f);
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
            ActorBase enemy = BlackboardScript.EnemyOverlap[i];
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
        TimeDied = GameManager.Instance.GameTime;

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

        float maxTime = GameManager.Instance.GameTime + 2.0f;
        float bloodEndTime = GameManager.Instance.GameTime + 0.5f;

        while (GameManager.Instance.GameTime < maxTime)
        {
            while (GameManager.Instance.PauseGameTime)
                yield return null;

            float delta = GameManager.Instance.GameDeltaTime;

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

            if (GameManager.Instance.GameTime < bloodEndTime)
                GameManager.Instance.TriggerBlood(pos, 0.1f);

            yield return null;
        }

        isPainted_ = false;
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
        if (IsBoss)
            return;

        CacheReviveCount++;

        Reset(init: false);
        ActorCache.Instance.ReturnObject(gameObject);
    }
}
