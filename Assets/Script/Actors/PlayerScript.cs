using Assets.Script;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// NB! this script is set to run after all other scripts, so we can be sure closestEnemy was updated
public class PlayerScript : MonoBehaviour
{
    public Sprite[] RunSprites;
    public Sprite[] IdleSprites;

    public float Hp;
    public float MaxHp;
    public Vector3 LatestLeftRight { get { return flipX_ < 0 ? Vector3.left : Vector3.right; } }
    //[System.NonSerialized] public Vector3 LatestLeftRightUpDown;

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
    bool isDead_ = true;
    float playerScale_ = 2.0f;
    bool isShooting_;
    float nextFire_ = float.MaxValue;
    float shotsLeft_ = 0;
    int flashParamId_;
    int flashColorParamId_;
    float flashEndTime_;
    float lastRegenTick_;
    bool flashActive_;
    Material material_;
    float immunityEnd_;
    bool immortal_;

    public bool RoundComplete;
    public bool UpgradesActive = false;

    public Text OverheadText;

    Vector3 basePos_;
    RectTransform overheadTextTrans_;

    [System.NonSerialized] public WeaponBase Weapon;

    SpriteRenderer shadowRenderer_;

    AnimationController animationController_ = new AnimationController();

    private void Awake()
    {
        trans_ = transform;
        playerScale_ = trans_.localScale.x; // Assume uniform scale
        renderer_ = GetComponent<SpriteRenderer>();
        playerPos_ = trans_.position;
        basePos_ = trans_.position;

        flashParamId_ = Shader.PropertyToID("_FlashAmount");
        flashColorParamId_ = Shader.PropertyToID("_FlashColor");
        material_ = renderer_.material;

        overheadTextTrans_ = OverheadText.GetComponent<RectTransform>();

        shadowRenderer_ = trans_.Find("BlobShadow").GetComponent<SpriteRenderer>();

        BlackboardScript.PlayerScript = this;
        BlackboardScript.PlayerTrans = trans_;
    }

    public void SetPlayerPos(Vector3 pos)
    {
        playerPos_ = pos;
        trans_.position = playerPos_;
    }

    public void UpdateMaxHp()
    {
        float newMaxHp = PlayerUpgrades.Data.BaseHealth * PlayerUpgrades.Data.HealthMul;
        float hpAdded = newMaxHp - MaxHp;
        Hp += hpAdded;
        MaxHp = newMaxHp;
        if (Hp > MaxHp)
            Hp = MaxHp;
    }

    public void ResetAll()
    {
        Hp = PlayerUpgrades.Data.BaseHealth * PlayerUpgrades.Data.HealthMul;
        UpdateMaxHp();

        IsInRound = false;

        var toggleEffects = GetComponentsInChildren<IPlayerToggleEfffect>();
        foreach (var toggleEffect in toggleEffects)
            toggleEffect.Disable();

        immunityEnd_ = 0;
        shotsLeft_ = 0;
        lastRegenTick_ = 0;
        UpgradesActive = false;
        shadowRenderer_.enabled = true;
        isDead_ = false;
        isMoving_ = false;
        force_ = Vector3.zero;
        moveVec_ = Vector3.zero;
        SetPlayerPos(basePos_);
        lookDir_ = lookDir_.x < 0.0f ? Vector3.left : Vector3.right;
        OverheadText.enabled = false;
        Weapon = WeaponBase.GetWeapon(WeaponType.None);

        nextPoof_ = 0;
        accumulated_ = 0;
    }

    private void Start()
    {
        ResetAll();
        StartCoroutine(Think());
    }

    public void OnInitialPickup(WeaponType type)
    {
        Weapon = WeaponBase.GetWeapon(type);
        if (type != WeaponType.None)
            SetNextFire();

        IsInRound = true;

        var toggleEffects = GetComponentsInChildren<IPlayerToggleEfffect>();
        foreach (var toggleEffect in toggleEffects)
            toggleEffect.TryEnable();
    }

    void RefreshBulletCount()
    {
        shotsLeft_ += PlayerUpgrades.Data.MagicMissileBaseBullets + PlayerUpgrades.Data.MagicMissileBulletsAdd;
    }

    void SetNextFire()
    {
        float FireCd = PlayerUpgrades.Data.MagicMissileBaseCd * PlayerUpgrades.Data.MagicMissileCdMul;
        nextFire_ = GameManager.Instance.GameTime + FireCd;
    }

    IEnumerator Think()
    {
        while (true)
        {
            if (GameManager.Instance.GameState != GameManager.State.Playing)
            {
                yield return null;
                continue;
            }

            if (GameManager.Instance.GameTime > nextFire_)
            {
                RefreshBulletCount();
                nextFire_ = float.MaxValue;
            }

            bool hasBullets = (int)shotsLeft_ > 0;

            if (!hasBullets)
            {
                // Stop firing
                isShooting_ = false;
                Weapon.StopFire();
            }
            else if (hasBullets && Weapon.GetCdLeft() <= 0.0f)
            {
                shotsLeft_ -= 1;
                if (shotsLeft_ >= 1)
                    SetNextFire();

                isShooting_ = true;

                float recoil;
                var fireDir = lookDir_;
                if (ActorBase.PlayerClosestEnemy != null)
                    fireDir = (ActorBase.PlayerClosestEnemy.transform.position - trans_.position).normalized;

                float damage = PlayerUpgrades.Data.MagicMissileBaseDamage * PlayerUpgrades.Data.MagicMissileDamageMul;

                Weapon.FireFromPoint(trans_.position, fireDir, damage, scale: 1.0f, GameManager.Instance.SortLayerTopEffects, out recoil);

                const float anglePerShot = 20;
                const float multiDaggerScale = 0.8f;

                damage *= 0.20f;

                for (int i = 1; i < PlayerUpgrades.Data.MagicMissileMultiShots + 1; ++i)
                {
                    var dir1 = Quaternion.AngleAxis(-(i * anglePerShot), Vector3.forward) * fireDir;
                    Weapon.FireFromPoint(trans_.position, dir1, damage, multiDaggerScale, GameManager.Instance.SortLayerTopEffects, out _);

                    var dir2 = Quaternion.AngleAxis(+(i * anglePerShot), Vector3.forward) * fireDir;
                    Weapon.FireFromPoint(trans_.position, dir2, damage, multiDaggerScale, GameManager.Instance.SortLayerTopEffects, out _);
                }

                AddForce(lookDir_ * -recoil);
                const float RecoilScreenShakeFactor = 2.0f;
                GameManager.Instance.ShakeCamera(recoil * RecoilScreenShakeFactor);
            }

            if (!isMoving_)
            {
                animationController_.Tick(GameManager.Instance.GameDeltaTime, renderer_, IdleSprites);
            }
            else
            {
                animationController_.Tick(GameManager.Instance.GameDeltaTime, renderer_, RunSprites);
            }
            yield return null;
        }
    }

    public void DamagePlayer(float damage)
    {
        if (isDead_)
            return;

        if (GameManager.Instance.GameTime < immunityEnd_)
            return;

        damage = damage * PlayerUpgrades.Data.HealthDefenseMul;

        if (!immortal_)
            Hp -= damage;
        else
            FloatingTextSpawner.Instance.Spawn(trans_.position + Vector3.up * 1.0f, $"immortal", Color.white, speed: 0.1f, timeToLive: 0.2f, fontStyle: FontStyles.Normal);

        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerStaffHit, pitch: 4.0f);

        FloatingTextSpawner.Instance.Spawn(trans_.position + Vector3.up * 0.5f, $"-{(int)damage:0}", Color.blue, speed: 0.5f, timeToLive: 0.5f, fontStyle: FontStyles.Bold);
        if (Hp <= 0)
        {
            Hp = 0;
            isDead_ = true;
            StartCoroutine(EndGame(victory: false));
        }
        else
        {
            immunityEnd_ = GameManager.Instance.GameTime + PlayerUpgrades.Data.OnDamageTimeImmune;
            SetFlash(true);
        }
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
        StartCoroutine(EndGame(victory: true));
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
        if (isDead_ || layer == GameManager.Instance.LayerPlayerProjectile || layer == GameManager.Instance.LayerNeutral || layer == GameManager.Instance.LayerOrc)
            return;

        var actor = col.gameObject.GetComponent<ActorBase>();
        if (actor != null && !actor.IsBoss)
        {
            // Thorns
            GameManager.Instance.DamageEnemy(actor, 20.0f, (actor.transform.position + trans_.position).normalized, 2.0f);
        }

        // Projectiles gets same damage as actors, hackz
        DamagePlayer(ActorBase.Damage);
    }

    IEnumerator EndGame(bool victory)
    {
        yield return new WaitForEndOfFrame();

        isDead_ = true;
        SetFlash(false);
        shadowRenderer_.enabled = false;
        GameProgressScript.Instance.Stop();
        AudioManager.Instance.StopAllRepeating();
        OnInitialPickup(WeaponType.None);
        var clip = victory ? AudioManager.Instance.AudioData.Victory : AudioManager.Instance.AudioData.PlayerDie;
        AudioManager.Instance.PlayClip(clip);

        if (!victory)
        {
            GameManager.Instance.TriggerBlood(trans_.position, 1);
            MusicManagerScript.Instance.StopMusic();
        }

        Camera cam = Camera.main;
        Time.timeScale = 0.0f;
        Vector3 cameraTarget = trans_.localPosition;

        RoundComplete = false;
        float targetOrthoSize = 5.0f;
        float targetShowGameOverOrthoSize = 6.0f;
        bool quickDeath = true;
        while (true)
        {
            if (RoundComplete)
                break;

            float orthoDistance = cam.orthographicSize - targetOrthoSize;
            float amount = orthoDistance * 0.5f * Time.unscaledDeltaTime * 2;
            float multiplier = (1.0f / cam.orthographicSize * amount);
            cam.transform.parent.position += (cameraTarget - cam.transform.parent.position) * multiplier;
            cam.orthographicSize -= amount;

            // Show game over overlay
            if (GameManager.Instance.GameState != GameManager.State.Dead && (cam.orthographicSize < targetShowGameOverOrthoSize || quickDeath))
                GameManager.Instance.ShowGameOver();

            if (cam.orthographicSize < targetOrthoSize + 0.1f)
                break;

            yield return null;
        }
    }

    void CheckControls()
    {
        if (isDead_)
            return;

        if (GameManager.Instance.GameState != GameManager.State.Playing)
            return;

        float left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1.0f : 0.0f;
        float right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0.0f;
        float up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0.0f;
        float down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1.0f : 0.0f;

        float speed = PlayerUpgrades.Data.BaseMoveSpeed * PlayerUpgrades.Data.MoveSpeedMul;

        speed *= GameManager.Instance.GameDeltaTime;

        Vector3 newMoveVec_ = Vector3.left * (left * speed) + Vector3.right * (right * speed) + Vector3.up * (up * speed) + Vector3.down * (down * speed);
        if (newMoveVec_ == Vector3.zero)
        {
            const float Damp = 12.0f;
            moveVec_ *= 1.0f - (GameManager.Instance.GameDeltaTime * Damp);
            if (moveVec_.sqrMagnitude < 0.0001f)
                moveVec_ = Vector3.zero;
        }
        else
        {
            moveVec_ = newMoveVec_;
        }

        isMoving_ = moveVec_ != Vector3.zero;

        if (ActorBase.PlayerClosestEnemy != null)
            lookDir_ = (ActorBase.PlayerClosestEnemy.transform.position - trans_.position).normalized;
        else
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

    void UpdateOverheadText()
    {
        bool isRambo = Weapon != null && Weapon.Type == WeaponType.Rambo;
        if (isShooting_ && isRambo)
        {
            OverheadText.text = "AAAARGH!";
            OverheadText.enabled = true;

            Vector2 uiPos = GameManager.Instance.UiPositionFromWorld(trans_.position + Vector3.up * 0.5f + ((Vector3)RndUtil.RandomInsideUnitCircle() * 0.07f));
            overheadTextTrans_.anchoredPosition = uiPos;
            Color col = Color.HSVToRGB(Random.value * 0.3f, 1.0f, 1.0f);
            OverheadText.color = col;
        }
    }

    public void AddHp(float value, bool alwaysShow = false)
    {
        if (Hp >= MaxHp && !alwaysShow)
            return;

        if (alwaysShow)
            FloatingTextSpawner.Instance.Spawn(transform.position + Vector3.up * 0.5f, $"+{(int)value}", new Color(0, 1, 0, 1.0f), speed: 0.5f, timeToLive: 0.75f);

        float newHp = Hp + value;
        int visibleOld = (int)(Hp + 0.5f);
        int visibleNew = (int)(newHp + 0.5f);
        int visibleChange = visibleNew - visibleOld;
        Hp = newHp;

        if (visibleChange > 0 && !alwaysShow)
        {
            const float a = 0.3f;
            FloatingTextSpawner.Instance.Spawn(transform.position + Vector3.up * 0.5f, $"+{visibleChange}", new Color(0, 1, 0, a), speed: 0.75f, timeToLive: 0.5f);
        }

        if (Hp > MaxHp)
            Hp = MaxHp;
    }

    float nextPoof_;
    float accumulated_;

    void CheckDistanceToEdge()
    {
        var closestEdgePoint = (Vector3)BlackboardScript.ClosestPointOnEdge(trans_.position);
        // total hack for cloud being partially out of screen at top
        if (closestEdgePoint.y > 5)
            closestEdgePoint.y -= 0.5f;

        float distance = Vector2.Distance(closestEdgePoint, trans_.position);

        GameManager.SetDebugOutput("dist", distance);

        const float MinDistance = 1.2f;
        const float DischargeTime = 2.0f;

        if (distance < MinDistance)
        {
            accumulated_ += GameManager.Instance.GameDeltaTime * 1;
            if (accumulated_ > DischargeTime)
            {
                var force = (trans_.position - closestEdgePoint).normalized * (MinDistance - distance) * 0.4f;
                GameManager.SetDebugOutput("force", force);
                GameManager.Instance.MakeFlash(trans_.position, 1.0f);
                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerStaffHit, pitch: 4.0f);
                AddForce(force);
                accumulated_ = 0;
            }
        }
        else if (distance > MinDistance)
        {
            accumulated_ -= GameManager.Instance.GameDeltaTime;
            if (accumulated_ < 0)
                accumulated_ = 0;
        }

        if (accumulated_ > 0 && GameManager.Instance.GameTime > nextPoof_)
        {
            nextPoof_ = GameManager.Instance.GameTime + 0.2f;
            GameManager.Instance.MakeSpawnPoof(closestEdgePoint, 2);
        }

        GameManager.SetDebugOutput("dist", distance);
        GameManager.SetDebugOutput("acc", accumulated_);

        Debug.DrawLine(trans_.position, closestEdgePoint, Color.cyan);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            BlackboardScript.DestroyAllEnemies();

        if (Input.GetKeyDown(KeyCode.X) && Input.GetKey(KeyCode.RightShift))
        {
            immortal_ = !immortal_;
            FloatingTextSpawner.Instance.Spawn(trans_.position + Vector3.up * 0.5f, $"Immortal: {immortal_}", Color.cyan, speed: 0.5f, timeToLive: 0.5f, fontStyle: FontStyles.Bold);
        }

        if (GameManager.Instance.PauseGameTime)
            return;

        if (GameManager.Instance.GameTime > lastRegenTick_ + 1.0f)
        {
            lastRegenTick_ = GameManager.Instance.GameTime;

            AddHp(PlayerUpgrades.Data.BaseHealthRegenSec + PlayerUpgrades.Data.HealthRegenSecAdd);
        }

        if (flashActive_ && GameManager.Instance.GameTime > flashEndTime_)
            SetFlash(false);

        renderer_.sortingOrder = Mathf.RoundToInt(trans_.position.y * 100f) * -1;

        if (isMoving_ && !isDead_)
            playerPos_ += moveVec_;

        if (!isDead_)
            playerPos_ += force_ * GameManager.Instance.GameDeltaTime * 60;

        playerPos_.z = 0;
        playerPos_ = GameManager.Instance.ClampToBounds(playerPos_, renderer_.sprite);
        trans_.position = playerPos_;
        force_ *= 1.0f - (20.0f * GameManager.Instance.GameDeltaTime);

        //CheckDistanceToEdge();
        CheckControls();
        UpdateOverheadText();
    }
}
