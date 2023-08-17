using System;
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
    float nextFire_;
    float shotsLeft_ = 0;
    int flashParamId_;
    int flashColorParamId_;
    float flashEndTime_;
    float lastRegenTick_;
    bool flashActive_;
    Material material_;
    float immunityEnd_;
    bool immortal_;
    float timeNextRound_;

    public bool RoundComplete;
    public bool UpgradesActive = false;

    public Text OverheadText;

    Vector3 basePos_;
    RectTransform overheadTextTrans_;

    [System.NonSerialized] public WeaponBase Weapon;

    SpriteRenderer shadowRenderer_;

    AnimationController animationController_ = new ();

    [NonSerialized] public bool IsPuppet;
    bool puppetMoveToBegin_;
    Vector3 puppetDst_;
    Vector2 puppetLookDir_;
    [NonSerialized] public bool isAtPuppetTarget;

    public void SetPuppet(Vector3 destination, Vector2 lookDir)
    {
        IsPuppet = true;
        puppetDst_ = destination;
        puppetLookDir_ = lookDir;
        isAtPuppetTarget = false;
    }

    void SetOverheadText(string text, Color col)
    {
        Vector2 uiPos = GameManager.Instance.UiPositionFromWorld(trans_.position + Vector3.up * 1.2f);
        overheadTextTrans_.anchoredPosition = uiPos;
        OverheadText.text = text;
        OverheadText.color = col;
        OverheadText.enabled = true;
    }

    public void StopPuppet(bool moveToBegin = false)
    {
        if (moveToBegin)
        {
            puppetMoveToBegin_ = true;

            SetOverheadText("move to begin", Color.white);
            return;
        }

        OverheadText.enabled = false;
        puppetMoveToBegin_ = false;
        IsPuppet = false;
        isAtPuppetTarget = false;
        puppetDst_ = Vector3.zero;
    }

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
    }

    public void SetPlayerPos(Vector3 pos)
    {
        playerPos_ = pos;
        trans_.position = playerPos_;
    }

    public void UpdateMaxHp(bool isReset = false)
    {
        float newMaxHp = PlayerUpgrades.Data.BaseHealth * PlayerUpgrades.Data.HealthMul;
        float hpAdded = newMaxHp - MaxHp;
        MaxHp = newMaxHp;
        AddHp(hpAdded, alwaysShow: !isReset);
    }

    public void ResetAll()
    {
        StopAllCoroutines();

        IsPuppet = false;

        Hp = PlayerUpgrades.Data.BaseHealth * PlayerUpgrades.Data.HealthMul;
        UpdateMaxHp(isReset: true);

        IsInRound = false;

        DisableToggledEffects();

        nextBlood_ = 0;
        timeNextRound_ = 0;
        nextFire_ = 0;
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
    }

    public void StartGame()
    {
        ResetAll();
        StartCoroutine(Think());
    }

    public void DisableToggledEffects()
    {
        var toggleEffects = GetComponentsInChildren<IPlayerToggleEfffect>();
        foreach (var toggleEffect in toggleEffects)
            toggleEffect.Disable();
    }

    public void TryEnableToggledEffects()
    {
        var toggleEffects = GetComponentsInChildren<IPlayerToggleEfffect>();
        foreach (var toggleEffect in toggleEffects)
            toggleEffect.TryEnable();
    }

    public void OnInitialPickup(WeaponType type)
    {
        Weapon = WeaponBase.GetWeapon(type);
        if (type != WeaponType.None)
            SetNextFire();

        IsInRound = true;

        TryEnableToggledEffects();
    }

    void RefreshBulletCount()
    {
        shotsLeft_ += PlayerUpgrades.Data.MagicMissileBaseBullets + PlayerUpgrades.Data.MagicMissileBulletsAdd;
    }

    void SetNextFire()
    {
        if (isDead_ || SaveGame.RoundScore == 0)
            return;

        float FireCd = PlayerUpgrades.Data.IsRambo ? 0.15f : PlayerUpgrades.Data.MagicMissileBaseCd * PlayerUpgrades.Data.MagicMissileCdMul;
        nextFire_ = G.D.GameTime + FireCd;
    }

    IEnumerator Think()
    {
        while (true)
        {
            yield return null;

            if (GameManager.Instance.GameState != GameManager.State.Playing || isDead_ || IsPuppet || SaveGame.RoundScore == 0)
                continue;

            if (G.D.GameTime > nextFire_)
            {
                RefreshBulletCount();
                nextFire_ = float.MaxValue;
            }

            bool hasBullets = (int)shotsLeft_ > 0;

            if (!hasBullets)
            {
                // Stop firing
                Weapon.StopFire();
            }
            else if (hasBullets && G.D.GameTime > timeNextRound_)
            {
                float roundCd = PlayerUpgrades.Data.IsRambo ? 0.1f : PlayerUpgrades.Data.MagicMissileBaseBulletCd * PlayerUpgrades.Data.MagicMissileCdMul;
                timeNextRound_ = G.D.GameTime + roundCd;

                shotsLeft_ -= 1;
                if (shotsLeft_ >= 1)
                    SetNextFire();

                float recoil;
                var fireDir = lookDir_;
                if (ActorBase.PlayerClosestEnemy != null)
                    fireDir = (ActorBase.PlayerClosestEnemy.transform.position - trans_.position).normalized;

                float damage = PlayerUpgrades.Data.MagicMissileBaseDamage * PlayerUpgrades.Data.MagicMissileDamageMul;

                Weapon.FireFromPoint(trans_.position, fireDir, damage, scale: 1.0f, GameManager.Instance.SortLayerTopEffects, out recoil);

                const float anglePerShot = 20;
                const float multiDaggerScale = 0.5f;

                if (!PlayerUpgrades.Data.IsRambo)
                {
                    damage *= 0.05f;

                    for (int i = 1; i < PlayerUpgrades.Data.MagicMissileMultiShots + 1; ++i)
                    {
                        var dir1 = Quaternion.AngleAxis(-(i * anglePerShot), Vector3.forward) * fireDir;
                        Weapon.FireFromPoint(trans_.position, dir1, damage, multiDaggerScale, GameManager.Instance.SortLayerTopEffects, out _);

                        var dir2 = Quaternion.AngleAxis(+(i * anglePerShot), Vector3.forward) * fireDir;
                        Weapon.FireFromPoint(trans_.position, dir2, damage, multiDaggerScale, GameManager.Instance.SortLayerTopEffects, out _);
                    }
                }

                AddForce(lookDir_ * recoil * 1);
                const float RecoilScreenShakeFactor = 2.0f;
                GameManager.Instance.ShakeCamera(recoil * RecoilScreenShakeFactor);
            }
        }
    }

    public void DamagePlayer(float damage)
    {
        if (isDead_ || IsPuppet)
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
        if (Mathf.RoundToInt(Hp) <= 0)
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
        if (isDead_ || (IsPuppet && !puppetMoveToBegin_))
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
            if (puppetMoveToBegin_)
            {
                StopPuppet(moveToBegin: false);
            }
            moveVec_ = newMoveVec_;
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

    void UpdateRambo()
    {
        bool ramboEnded = PlayerUpgrades.Data.IsRambo && G.D.GameTime > PlayerUpgrades.Data.RamboEndTime;
        if (ramboEnded)
        {
            PlayerUpgrades.Data.IsRambo = false;
            OverheadText.enabled = false;
        }

        bool ramboActive = PlayerUpgrades.Data.IsRambo && PlayerUpgrades.Data.RamboEndTime > G.D.GameTime;
        if (ramboActive)
        {
            OverheadText.text = "CHARGE!";
            OverheadText.enabled = true;

            Vector2 uiPos = GameManager.Instance.UiPositionFromWorld(trans_.position + Vector3.up * 1.2f + ((Vector3)RndUtil.RandomInsideUnitCircle() * 0.07f));
            overheadTextTrans_.anchoredPosition = uiPos;

            Color col = Color.HSVToRGB(UnityEngine.Random.value * 0.3f, 1.0f, 1.0f);
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

    float nextBlood_;

    void CheckLowHealth()
    {
        if (Hp > 80 || Time.time < nextBlood_)
            return;

        GameManager.Instance.TriggerBlood(trans_.position, amount: 20, floorBloodRnd: 0.1f);

        float cd = Mathf.Max(Hp / 400, 0.03f);
        nextBlood_ = Time.time + cd;
    }

    void Update()
    {
        if (isDead_)
            return;

        var sprites = isMoving_ && !isAtPuppetTarget ? RunSprites : IdleSprites;

        animationController_.Tick(GameManager.Instance.GameDeltaTime, renderer_, sprites);

        if (G.GetCheatKeyDown(KeyCode.X) && G.GetCheatKey(KeyCode.RightShift))
        {
            immortal_ = !immortal_;
            FloatingTextSpawner.Instance.Spawn(trans_.position + Vector3.up * 0.5f, $"Immortal: {immortal_}", Color.cyan, speed: 0.5f, timeToLive: 0.5f, fontStyle: FontStyles.Bold);
        }

        if (G.GetCheatKeyDown(KeyCode.R) && G.GetCheatKey(KeyCode.RightShift))
        {
            PlayerUpgrades.Data.IsRambo = true;
            PlayerUpgrades.Data.RamboEndTime = G.D.GameTime + 5;
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

        if (IsPuppet)
        {
            float distanceToTarget = Vector2.Distance(playerPos_, puppetDst_);

            const float PuppetMaxSpeed = 5.0f;
            const float PuppetMinSpeed = 0.5f;

            float puppetSpeed = Math.Min(PuppetMaxSpeed, distanceToTarget * 4);
            puppetSpeed = Math.Max(PuppetMinSpeed, puppetSpeed);

            moveVec_ = (puppetDst_ - playerPos_).normalized * G.D.GameDeltaTime * puppetSpeed;
            playerPos_ += moveVec_;

            isAtPuppetTarget = distanceToTarget < 0.1f;
        }
        else
        {
            if (isMoving_ && !isDead_)
                playerPos_ += moveVec_;

            if (!isDead_)
                playerPos_ += 60 * GameManager.Instance.GameDeltaTime * force_;
        }

        playerPos_.z = 0;
        playerPos_ = GameManager.Instance.ClampToBounds(playerPos_, renderer_.sprite);
        trans_.position = playerPos_;
        force_ *= 1.0f - (20.0f * GameManager.Instance.GameDeltaTime);

        CheckLowHealth();
        CheckControls();
        UpdateRambo();

        if (IsPuppet)
        {
            renderer_.flipX = puppetLookDir_.x < 0 ? true : false;
        }
    }
}
