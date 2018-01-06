using Assets.Script;
using System.Collections;
using UnityEngine;
using System;

public enum ActorTypeEnum { None, Any, SmallWalker, SmallCharger, LargeWalker, Caster };

public class ActorBase : MonoBehaviour
{
    [System.NonSerialized] public float RadiusFirstCheck = 0.6f;
    [System.NonSerialized] public float RadiusBody = 0.3f;
    [System.NonSerialized] public Vector3 BodyOffset = new Vector3(0.0f, -0.25f, 0.0f);
    [System.NonSerialized] public float RadiusHead = 0.2f;
    [System.NonSerialized] public Vector3 HeadOffset = new Vector3(0.0f, 0.15f, 0.0f);

    [System.NonSerialized] public bool IsCorpse;

    [System.NonSerialized] public bool IsInsideVisibleArea;
    [System.NonSerialized] public ActorTypeEnum ActorType;
    [System.NonSerialized] public float Hp;
    public Transform Transform;

    protected GameModeData GameMode;
    protected float DecayTime = 20.0f;
    protected AnimationController animationController_ = new AnimationController();
    protected Sprite[] currentAnimations_;

    protected Vector3 position_;
    protected float mass_ = 1.0f;
    protected float massInverse_;
    protected Vector3 force_;
    protected Transform transform_;
    protected SpriteRenderer renderer_;
    protected Material material_;
    protected int flashParamId_;
    protected int flashColorParamId_;
    protected float flashEndTime_;

    protected virtual void PreAwake() { }
    protected virtual void PostAwake() { }

    protected virtual void PreStart() { }
    protected virtual void PostStart() { }

    protected virtual void PreUpdate() { }
    protected virtual void PostUpdate() { }

    protected virtual void OnDeath() { }

    protected float slowmotionModifier_ = 1.0f;

    protected bool isPainted_;
    float paintEnd_;
    protected bool isLivingBomb_;
    float livingBombDamage_;
    float livingBombEnd_;
    protected bool isSpawning_ = true;
    protected bool isFullyReady_ = false;

    Color outsideArenaColor = new Color(0.0f, 0.0f, 0.0f);

    public void Awake()
    {
        GameMode = GameManager.Instance.CurrentGameModeData;
        PreAwake();

        currentAnimations_ = SpriteData.Instance.SkellieWalkSprites; // Set default animations

        transform_ = this.transform;
        Transform = transform_;
        renderer_ = GetComponent<SpriteRenderer>();
        material_ = renderer_.material;
        flashParamId_ = Shader.PropertyToID("_FlashAmount");
        flashColorParamId_ = Shader.PropertyToID("_FlashColor");

        // Assume uniform scale
        float scale = transform_.localScale.x;
        RadiusFirstCheck *= scale;
        RadiusHead *= scale;
        RadiusBody *= scale;
        HeadOffset *= scale;
        BodyOffset *= scale;

        PostAwake();
    }

    public void Start()
    {
        PreStart();

        this.gameObject.layer = GameManager.Instance.LayerNeutral;
        GameManager.Instance.RegisterEnemy(this);
        massInverse_ = 1.0f / mass_;

        material_.color *= GameMode.AmbientColor;

        PostStart();
        StartCoroutine(SpawnAnimCo());
    }

    public bool OnPaintballHit()
    {
        if (isPainted_)
            return false;

        isPainted_ = true;
        paintEnd_ = Time.time + 8.0f;
        material_.color = Color.HSVToRGB(UnityEngine.Random.value, 1.0f, 1.0f);
        return true;
    }

    public void OnLivingBombHit(float damage, bool playSound = true)
    {
        if (isLivingBomb_)
            return;

        livingBombDamage_ = damage;
        isLivingBomb_ = true;

        const float BombTime = 1.0f;
        const float BombRnd = 0.1f;
        livingBombEnd_ = Time.time + BombTime + UnityEngine.Random.value * BombRnd;

        material_.SetColor(flashColorParamId_, new Color(1.0f, 0.6f, 0.6f));
        GameManager.Instance.MakeFlash(transform_.position);
        GameManager.Instance.MakePoof(transform_.position, 5, 1.0f);
        GameManager.Instance.ShakeCamera(0.1f);
        if (playSound)
            AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.MiscAudioSource, AudioManager.Instance.AudioData.PlayerStaffHit);
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
        if (UnityEngine.Random.value < 0.01f)
            GameManager.Instance.MakePoof(transform_.position, 1, 1.0f);
    }

    protected void UpdatePosition(Vector3 moveVec, float speed)
    {
        if (!isFullyReady_)
        {
            // Force towards center
            Vector3 centerDir = (Vector3.zero - position_).normalized;
            moveVec = (moveVec + centerDir).normalized;
        }

        if (isLivingBomb_)
            speed *= 0.9f;
        else if (isPainted_)
            speed *= 0.25f;

        position_ += moveVec * speed * slowmotionModifier_ * Timers.EnemyTimer * Time.deltaTime;
    }

    public void Update()
    {
        force_ *= 1.0f - (30.0f * Time.deltaTime);
        position_ += force_ * Time.deltaTime * 60 * Timers.EnemyTimer;
        transform_.position = position_;

        if (isSpawning_)
            return;

        PreUpdate();
        CheckPainted();
        UpdateLivingBomb();
        CheckFullyReady();

        position_.z = 0;

        if (isFullyReady_)
            position_ = GameManager.Instance.ClampToBounds(position_, renderer_.sprite);

        bool dead = Hp <= 0.0f;
        if (!dead)
            animationController_.Tick(Time.deltaTime * Timers.EnemyTimer, renderer_, currentAnimations_);

        renderer_.sortingOrder = (Mathf.RoundToInt(transform_.position.y * 100f) * -1) - (dead ? 10000 : 0);

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

    public void AddForce(Vector3 force)
    {
        force_ += force;
        //if (force_.magnitude > 0.5f)
        //    force_ = force_.normalized * 0.5f;
    }

    public void ApplyDamage(float amount, Vector3 direction, float forceModifier, bool headshot)
    {
        if (Hp <= 0.0f)
            return;

        // Sandbox mode is with default damage
        float damageModifier = GameManager.Instance.CurrentDeedData.Deed == DeedEnum.Sandbox ? 1.0f : Unlocks.DamageModifier;
        amount *= damageModifier;

        Hp -= amount;
        GameManager.Instance.TriggerBlood(transform_.position, 1.0f + (amount * 0.25f) * forceModifier);

        if (Hp <= 0.0f)
        {
            GameManager.Instance.OnEnemyKill(this);
            OnDeath();
            StopAllCoroutines();
            StartCoroutine(DieAnimation(direction, forceModifier));
        }
        else
        {
            float force = Mathf.Clamp(amount * 0.2f, 1.0f, 3.0f);
            AddForce(direction * (force * 0.2f * massInverse_ * forceModifier));
            material_.SetFloat(flashParamId_, 0.75f);
            flashEndTime_ = Time.time + 0.1f * forceModifier;
            if (headshot)
                slowmotionModifier_ = 0.0f;
        }
    }

    protected void EnableShadow(bool enable)
    {
        transform.Find("BlobShadow").GetComponent<SpriteRenderer>().enabled = enable;
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
        if (isFullyReady_)
            return;

        bool isFullyReady = !isSpawning_ && GameManager.Instance.IsInsideBounds(transform_.position, renderer_.sprite);
        if (!isFullyReady)
        {
            material_.SetColor(flashColorParamId_, outsideArenaColor);
            material_.SetFloat(flashParamId_, 0.4f);
            return;
        }

        material_.SetColor(flashColorParamId_, Color.white);
        material_.SetFloat(flashParamId_, 0.0f);

        isFullyReady_ = true;
        this.gameObject.layer = GameManager.Instance.LayerEnemy;
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
        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.MiscAudioSource, AudioManager.Instance.AudioData.LivingBombExplode);
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

        if (BlackboardScript.DistanceToPlayer(transform_.position) < ExplodeRadius * 0.5f)
        {
            GameManager.Instance.PlayerScript.KillPlayer();
        }

        ApplyDamage(livingBombDamage_, UnityEngine.Random.insideUnitCircle.normalized, 0.25f, true);
    }

    IEnumerator DieAnimation(Vector3 deathSourceDir, float forceModifier)
    {
        EnableShadow(false);
        this.gameObject.layer = GameManager.Instance.LayerEnemyCorpse;

        float direction = transform_.position.x < deathSourceDir.x ? -1 : 1;
        material_.SetColor(flashColorParamId_, Color.black);
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

        BlackboardScript.DeadEnemies.Remove(this);
        GameObject.Destroy(this.gameObject);
    }
}
