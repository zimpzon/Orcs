using Assets.Script;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// NB! this script is set to run after all other scripts, so we can be sure closestEnemy was updated
public class PlayerScript : MonoBehaviour
{
    const float BaseCd = 0.5f;
    const float BaseMoveSpeed = 4f;

    public float Hp;
    public float MaxHp;

    [System.NonSerialized] public Vector3 CursorPos;
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
    int shotsLeft_ = 0;
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

    PlayerUpgrades _playerUpgrades;

    [System.NonSerialized] public WeaponBase Weapon;
    public Transform MeleeWeapon;

    SpriteRenderer shadowRenderer_;

    AnimationController animationController_ = new AnimationController();

    float talkEndTime_;
    int lastHolyWordIdx_;

    private void Awake()
    {
        trans_ = this.transform;
        playerScale_ = trans_.localScale.x; // Assume uniform scale
        renderer_ = GetComponent<SpriteRenderer>();
        playerPos_ = trans_.position;
        basePos_ = trans_.position;
        _playerUpgrades = GetComponent<PlayerUpgrades>();

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

    public void SwingMelee()
    {
        StartCoroutine(SwingMeleeCo());
    }

    public IEnumerator SwingMeleeCo()
    {
        int flipX = lookDir_.x < trans_.position.x ? -1 : 1;
        MeleeWeapon.gameObject.SetActive(true);
        float degreeStart = 90 * flipX;
        float degreeEnd = -180 * flipX;
        float degrees = degreeStart;

        while (true)
        {
            if (flipX == 1.0f && degrees < degreeEnd)
                break;

            if (flipX == -1.0f && degrees > degreeEnd)
                break;

            MeleeWeapon.rotation = Quaternion.Euler(0.0f, 0.0f, degrees);
            degrees -= 1500 * flipX * Time.deltaTime;
            yield return null;
        }

        MeleeWeapon.gameObject.SetActive(false);

        yield break;
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

        lastRegenTick_ = 0;
        UpgradesActive = false;
        MeleeWeapon.gameObject.SetActive(false);
        shadowRenderer_.enabled = true;
        isDead_ = false;
        isMoving_ = false;
        force_ = Vector3.zero;
        moveVec_ = Vector3.zero;
        SetPlayerPos(basePos_);
        lookDir_ = lookDir_.x < 0.0f ? Vector3.left : Vector3.right;
        SetWeapon(WeaponType.None);
    }

    string GetHolyWord()
    {
        var words = GameManager.Instance.SelectedHero.Talk;
        int idx = 0;
        for (int i = 0; i < 50; ++i)
        {
            idx = Random.Range(0, words.Count);
            if (idx == lastHolyWordIdx_)
                continue;

            break;
        }

        lastHolyWordIdx_ = idx;
        return words[idx];
    }

    private void Start()
    {
        ResetAll();
        StartCoroutine(Think());
    }

    public void SetWeapon(WeaponType type)
    {
        Weapon = WeaponBase.GetWeapon(type);
        if (type != WeaponType.None)
            SetNextFire();
    }

    void RefreshBulletCount()
    {
        shotsLeft_ = PlayerUpgrades.Data.MagicMissileBaseBullets + PlayerUpgrades.Data.MagicMissileBulletsAdd;
    }

    void SetNextFire()
    {
        float FireCd = BaseCd * PlayerUpgrades.Data.MagicMissileCdMul;
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

            bool hasBullets = shotsLeft_ > 0;

            if (!hasBullets)
            {
                // Stop firing
                isShooting_ = false;
                Weapon.StopFire();
            }
            else if (hasBullets && Weapon.GetCdLeft() <= 0.0f)
            {
                shotsLeft_--;
                if (shotsLeft_ == 0)
                    SetNextFire();

                isShooting_ = true;
                float recoil;
                Weapon.Fire(trans_, lookDir_, GameManager.Instance.SortLayerTopEffects, out recoil);
                AddForce(lookDir_ * -recoil);
                const float RecoilScreenShakeFactor = 2.0f;
                GameManager.Instance.ShakeCamera(recoil * RecoilScreenShakeFactor);
            }

            if (!isMoving_)
            {
                animationController_.Tick(Time.deltaTime, renderer_, GameManager.Instance.SelectedHero.PlayerIdleSprites);
            }
            else
            {
                animationController_.Tick(Time.deltaTime, renderer_, GameManager.Instance.SelectedHero.PlayerRunSprites);
            }
            yield return null;
        }
    }

    public void DamagePlayer(float damage)
    {
        if (isDead_)
            return;

        if (Time.time < immunityEnd_)
            return;

        damage = damage * PlayerUpgrades.Data.HealthDefenseMul;

        if (!immortal_)
            Hp -= damage;
        else
            FloatingTextSpawner.Instance.Spawn(trans_.position + Vector3.up * 1.0f, $"immortal", Color.white, speed: 0.1f, timeToLive: 0.2f, fontStyle: FontStyles.Normal);

        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerStaffHit, 4.0f);

        FloatingTextSpawner.Instance.Spawn(trans_.position + Vector3.up * 0.5f, $"-{(int)damage:0}", Color.blue, speed: 0.5f, timeToLive: 0.5f, fontStyle: FontStyles.Bold);
        if (Hp <= 0)
        {
            Hp = 0;
            isDead_ = true;
            StartCoroutine(EndGame(victory: false));
        }
        else
        {
            immunityEnd_ = Time.time + PlayerUpgrades.Data.OnDamageTimeImmune;
            SetFlash(true);
        }
    }

    void SetFlash(bool setActive)
    {
        if (setActive && !flashActive_)
        {
            material_.SetFloat(flashParamId_, 1.0f);
            material_.SetColor(flashColorParamId_, new Color(1.0f, 0.6f, 0.6f));
            flashEndTime_ = Time.time + PlayerUpgrades.Data.OnDamageTimeImmune;
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
        if (actor != null && !actor.IsFullyReady)
            return;

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
        SetWeapon(WeaponType.None);
        var clip = victory ? AudioManager.Instance.AudioData.Victory : AudioManager.Instance.AudioData.PlayerDie;
        AudioManager.Instance.PlayClip(clip);

        if (!victory)
        {
            GameManager.Instance.TriggerBlood(trans_.position, 1);
            MusicManagerScript.Instance.StopMusic();
        }

        Camera cam = Camera.main;
        Time.timeScale = 0.0001f;
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

        float speed = BaseMoveSpeed * PlayerUpgrades.Data.MoveSpeedMul;

        speed *= Time.deltaTime;

        Vector3 newMoveVec_ = Vector3.left * (left * speed) + Vector3.right * (right * speed) + Vector3.up * (up * speed) + Vector3.down * (down * speed);
        if (newMoveVec_ == Vector3.zero)
        {
            const float Damp = 12.0f;
            moveVec_ *= 1.0f - (Time.deltaTime * Damp);
            if (moveVec_.sqrMagnitude < 0.0001f)
                moveVec_ = Vector3.zero;
        }
        else
        {
            moveVec_ = newMoveVec_;
        }

        isMoving_ = moveVec_ != Vector3.zero;

        CursorPos = GameManager.Instance.CrosshairWorldPosition;

        if (ActorBase.PlayerClosestEnemy != null)
            lookDir_ = (ActorBase.PlayerClosestEnemy.transform.position - trans_.position).normalized;
        else
            lookDir_ = Vector2.right;

        flipX_ = lookDir_.x < trans_.position.x ? -playerScale_ : playerScale_;
        Vector3 scale = trans_.localScale;
        scale.x = flipX_;
        trans_.localScale = scale;
    }

    public void AddForce(Vector3 f)
    {
        force_ += f;
    }

    public void Talk(string text, float time, Color color)
    {
        talkEndTime_ = Time.time + time;
        OverheadText.color = color;
        OverheadText.text = text;
        OverheadText.enabled = true;
    }

    public void HolyTalk()
    {
        Talk(GetHolyWord(), 1.0f, Color.white);
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
        else if (Time.time < talkEndTime_)
        {
            Vector2 uiPos = GameManager.Instance.UiPositionFromWorld(trans_.position + Vector3.up * 0.5f);
            overheadTextTrans_.anchoredPosition = uiPos;
        }
        else
        {
            OverheadText.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            immortal_ = !immortal_;
            FloatingTextSpawner.Instance.Spawn(trans_.position + Vector3.up * 0.5f, $"Immortal: {immortal_}", Color.cyan, speed: 0.5f, timeToLive: 0.5f, fontStyle: FontStyles.Bold);
        }

        if (GameManager.Instance.PauseGameTime)
            return;

        if (Time.time > lastRegenTick_ + 1.0f)
        {
            lastRegenTick_ = Time.time;

            if (Hp >= MaxHp)
                return;

            int newHp = (int)(Hp + PlayerUpgrades.Data.BaseHealthRegenSec * PlayerUpgrades.Data.HealthRegenSecMul + 0.5f);
            int change = newHp - (int)(Hp + 0.5f);
            Hp = newHp;

            if (change > 0)
                FloatingTextSpawner.Instance.Spawn(transform.position + Vector3.up * 0.5f, $"+{change}", new Color(0, 1, 0, 0.2f), speed: 0.75f, timeToLive: 0.5f);

            if (Hp > MaxHp)
                Hp = MaxHp;
        }

        if (flashActive_ && Time.time > flashEndTime_)
            SetFlash(false);

        renderer_.sortingOrder = Mathf.RoundToInt(trans_.position.y * 100f) * -1;

        if (isMoving_ && !isDead_)
            playerPos_ += moveVec_;

        if (!isDead_)
            playerPos_ += force_ * Time.deltaTime * 60;

        playerPos_.z = 0;
        playerPos_ = GameManager.Instance.ClampToBounds(playerPos_, renderer_.sprite);
        trans_.position = playerPos_;
        force_ *= 1.0f - (20.0f * Time.deltaTime);

        CheckControls();
        UpdateOverheadText();

        if (UpgradesActive && !isDead_)
            _playerUpgrades.UpdateUpgrades();
    }
}
