using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// NB! this script is set to run after all other scripts, so we can be sure closestEnemy was updated
public class PlayerScript : MonoBehaviour
{
    bool chase_ = false;
    float chaseSwitch_ = 0;
    public Sprite[] RunSprites;
    public Sprite[] IdleSprites;

    public Vector3 LatestLeftRight { get { return flipX_ < 0 ? Vector3.left : Vector3.right; } }

    [System.NonSerialized] public Vector3 CursorPos;
    [System.NonSerialized] public bool IsInRound;
    Vector3 lookDir_;
    Vector3 moveVec_;
    bool isMoving_;
    Vector3 force_;
    Transform trans_;
    SpriteRenderer renderer_;
    Vector3 playerPos_;
    float flipX_;
    float playerScale_ = 2.0f;
    float nextFire_;
    int flashParamId_;
    int flashColorParamId_;
    float flashEndTime_;
    bool flashActive_;
    Material material_;
    bool immortal_;

    public bool RoundComplete;
    public bool UpgradesActive = false;
    public long DaggersThrown = 0;

    [System.NonSerialized] public WeaponBase Weapon;
    int daggersPerSalvo = 2;
    float salvoInterval = 0.1f;
    bool isFiringSalvo = false;

    SpriteRenderer shadowRenderer_;
    AnimationController animationController_ = new ();

    public void ResetPlayerPos()
    {
        var bounds = GameManager.ArenaBounds;
        var mid = bounds.center;
        playerPos_ = new Vector2(bounds.xMin + 2, mid.y);
        //playerPos_ = bounds.center;
        trans_.position = playerPos_;
    }

    public void ResetAll()
    {
        StopAllCoroutines();

        IsInRound = false;

        DisableToggledEffects();

        nextFire_ = 0;
        UpgradesActive = false;
        shadowRenderer_.enabled = true;
        isMoving_ = false;
        force_ = Vector3.zero;
        moveVec_ = Vector3.zero;
        ResetPlayerPos();
        lookDir_ = lookDir_.x < 0.0f ? Vector3.left : Vector3.right;
        Weapon = WeaponBase.GetWeapon(WeaponType.None);
        DaggersThrown = 0;
        isFiringSalvo = false;
    }

    public void StartGame()
    {
        ResetAll();
        StartCoroutine(Think());
        BeginShooting(WeaponType.Machinegun);
    }

    List<IPlayerToggleEfffect> toggles_;

    public void DisableToggledEffects()
    {
        if (toggles_ == null)
            toggles_ = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerToggleEfffect>().ToList();

        foreach (var toggleEffect in toggles_)
            toggleEffect.Disable();
    }

    public void TryEnableToggledEffects()
    {
        if (toggles_ == null)
            toggles_ = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerToggleEfffect>().ToList();

        foreach (var toggleEffect in toggles_)
            toggleEffect.TryEnable();
    }

    private void BeginShooting(WeaponType type)
    {
        Weapon = WeaponBase.GetWeapon(type);
        if (type != WeaponType.None)
            SetNextFire();

        IsInRound = true;

        TryEnableToggledEffects();
    }

    void SetNextFire()
    {
        double FireCd = PlayerUpgrades.Data.MagicMissileEffectiveCd;
        nextFire_ = G.D.GameTime + (float)FireCd;
    }

    IEnumerator Think()
    {
        var necromancer = GetComponent<NecromancerScript>();
        StartCoroutine(necromancer.Think());

        while (true)
        {
            yield return null;

            if (GameManager.Instance.GameState != GameManager.State.Idle_Fighting)
                continue;

            bool hasCloseEnemy = false;
            Vector2 vecToClosestEnemy = Vector2.right;

            if (ActorBase.PlayerClosestEnemy is not null)
            {
                vecToClosestEnemy = ActorBase.PlayerClosestEnemy.transform.position - trans_.position;
                float distanceClosest = vecToClosestEnemy.magnitude;
                hasCloseEnemy = distanceClosest <= PlayerUpgrades.Data.MagicMissileEffectiveRange;
            }

            if (!isFiringSalvo && G.D.GameTime > nextFire_ && hasCloseEnemy)
            {
                if (G.D.GameTime > nextFire_ && hasCloseEnemy)
                {
                    StartCoroutine(FireSalvo(vecToClosestEnemy.normalized));
                }
            }
        }
    }

    IEnumerator FireSalvo(Vector2 fireDir)
    {
        double baseDamage = PlayerUpgrades.Data.MagicMissileEffectiveDamage;
        float mainScale = 1.5f;
        float multiDaggerScale = 1.0f;
        float anglePerShot = 10f;
        float totalRecoil = 0;

        isFiringSalvo = true;

        for (int i = 0; i < daggersPerSalvo; i++)
        {
            if (ActorBase.PlayerClosestEnemy == null)
                break;

            float recoil;
            Weapon.FireFromPoint(trans_.position, fireDir, baseDamage, mainScale, GameManager.Instance.SortLayerTopEffects, out recoil);
            DaggersThrown++;
            totalRecoil += recoil;

            // Multishot (same frame)
            double sideDamage = baseDamage * 0.5f;
            for (int j = 1; j <= PlayerUpgrades.Data.MagicMissileMultiShots; ++j)
            {
                var dir1 = Quaternion.AngleAxis(-j * anglePerShot, Vector3.forward) * fireDir;
                var dir2 = Quaternion.AngleAxis(+j * anglePerShot, Vector3.forward) * fireDir;

                Weapon.FireFromPoint(trans_.position, dir1, sideDamage, multiDaggerScale, GameManager.Instance.SortLayerTopEffects, out _);
                Weapon.FireFromPoint(trans_.position, dir2, sideDamage, multiDaggerScale, GameManager.Instance.SortLayerTopEffects, out _);
            }

            if (i < daggersPerSalvo - 1)
                yield return new WaitForSeconds(salvoInterval);
        }

        isFiringSalvo = false;

        AddForce(lookDir_ * totalRecoil);
        GameManager.Instance.ShakeCamera(totalRecoil * 2.0f);
        SetNextFire();
    }

    void SetFlash(bool setActive)
    {
        if (setActive && !flashActive_)
        {
            material_.SetFloat(flashParamId_, 1.0f);
            material_.SetColor(flashColorParamId_, new Color(1.0f, 0.6f, 0.6f));
            flashEndTime_ = GameManager.Instance.GameTime + PlayerUpgrades.Data.OnDamageTimeImmune;
        }
        else if (flashActive_)
        {
            material_.SetFloat(flashParamId_, 0.0f);
            flashEndTime_ = float.MaxValue;
        }

        flashActive_ = setActive;
    }

    public void Victory()
    {
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        CheckCollision(col);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        CheckCollision(col);
    }

    void CheckCollision(Collision2D col)
    {
        int layer = col.gameObject.layer;
        var actor = col.gameObject.GetComponent<ActorBase>();
        if (actor != null && !actor.IsBoss)
        {
            // Thorns
            //GameManager.Instance.DamageEnemy(actor, 20.0f, (actor.transform.position + trans_.position).normalized, 2.0f);
        }
    }

    void DoMovement()
    {
        if (GameManager.Instance.GameState != GameManager.State.Idle_Fighting)
            return;

        float speed = PlayerUpgrades.Data.MoveSpeedEffective;

        speed *= GameManager.Instance.GameDeltaTime;

        Vector3 newMoveVec = Vector3.zero;
        if (ActorBase.PlayerClosestEnemy != null)
        {
            Vector3 toClosest = (ActorBase.PlayerClosestEnemy.transform.position - trans_.position);

            if (Time.time > chaseSwitch_)
            {
                chase_ = !chase_;
                chaseSwitch_ = chase_ ?
                    Time.time + 1.0f + UnityEngine.Random.value * 2.0f :
                    Time.time + 0.25f + UnityEngine.Random.value * 0.25f;
            }
            float distanceToClosest = toClosest.magnitude;

            if (distanceToClosest > 4)
                chase_ = true;

            if (distanceToClosest < 1.5f)
            {
                chase_ = false;
                chaseSwitch_ = Time.time + 0.5f + UnityEngine.Random.value * 0.5f;
            }

            if (chase_ && distanceToClosest > 1.0f)
            {
                if (distanceToClosest > 2.5f)
                {
                    newMoveVec = toClosest.normalized * speed;
                }
            }
            else
            {
                if (distanceToClosest < 2.0f)
                {
                    newMoveVec = -toClosest.normalized * speed;
                }
            }
        }

        if (newMoveVec == Vector3.zero)
        {
            const float Damp = 12.0f;
            moveVec_ *= 1.0f - (GameManager.Instance.GameDeltaTime * Damp);
            if (moveVec_.sqrMagnitude < 0.0001f)
                moveVec_ = Vector3.zero;
        }
        else
        {
            moveVec_ = newMoveVec;
        }

        isMoving_ = moveVec_ != Vector3.zero;

        lookDir_ = flipX_ < 0 ? Vector2.left : Vector2.right;

        if (moveVec_.x != 0.0f)
        {
            flipX_ = moveVec_.x < 0 ? -playerScale_ : playerScale_;
            renderer_.flipX = flipX_ < 0;
        }
    }

    public void AddForce(Vector3 f)
    {
        force_ += f;
    }

    long accumulatedCount = 0;
    double lastAccumulatedAdd;

    public void OnGoldPickedUp(bool isLargeCoin, long value)
    {
        float pitch = Math.Min(1.1f, 0.9f + accumulatedCount * 0.01f);
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.MoneyPickup, volumeScale: 0.7f, pitch: pitch);
        accumulatedCount++;

        if (G.D.GameTime > lastAccumulatedAdd + 0.25f)
            accumulatedCount = 0;

        lastAccumulatedAdd = G.D.GameTime;
        GameManager.Instance.AddGold(isLargeCoin, value);
    }

    private void Awake()
    {
        trans_ = transform;
        playerScale_ = trans_.localScale.x; // Assume uniform scale
        renderer_ = GetComponent<SpriteRenderer>();
        playerPos_ = trans_.position;

        flashParamId_ = Shader.PropertyToID("_FlashAmount");
        flashColorParamId_ = Shader.PropertyToID("_FlashColor");
        material_ = renderer_.material;

        shadowRenderer_ = trans_.Find("BlobShadow").GetComponent<SpriteRenderer>();
    }

    float _nextGrendade = -1;

    void Update()
    {
        bool isRunning = isMoving_;

        Sprite[] sprites;
        sprites = isRunning ? RunSprites : IdleSprites;

        //if (G.D.GameTime > _nextGrendade)
        //{
        //    var from = G.D.PlayerPos;
        //    var to = from + (Vector3)PositionUtility.RandomPointOnUnitCircle() * 5;
        //    GameManager.Instance.Grenade.Throw(from, to, PlayerUpgrades.Data.MagicMissileEffectiveDamage);
        //    _nextGrendade = G.D.GameTime + 6.0f;
        //}

        animationController_.Tick(GameManager.Instance.GameDeltaTime, renderer_, sprites);

        if (G.GetCheatKeyDown(KeyCode.X) && G.GetCheatKey(KeyCode.RightShift))
        {
            immortal_ = !immortal_;
            FloatingTextSpawner.Instance.Spawn(trans_.position + Vector3.up * 0.5f, $"Immortal: {immortal_}", Color.cyan, speed: 0.5f, timeToLive: 0.5f, fontStyle: FontStyles.Bold);
        }

        if (GameManager.Instance.PauseGameTime)
            return;

        if (flashActive_ && GameManager.Instance.GameTime > flashEndTime_)
            SetFlash(false);

        renderer_.sortingOrder = Mathf.RoundToInt(trans_.position.y * 100f) * -1;

        if (isMoving_)
            playerPos_ += moveVec_;

        playerPos_ += 60 * GameManager.Instance.GameDeltaTime * force_;
        playerPos_.z = 0;
        playerPos_ = GameManager.Instance.ClampToBounds(playerPos_, renderer_.sprite);
        trans_.position = playerPos_;
        force_ *= 1.0f - (20.0f * GameManager.Instance.GameDeltaTime);

        DoMovement();
    }
}
