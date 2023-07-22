using Assets.Script;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    const float BaseCd = 1.0f;
    const int BaseBullets = 2;

    [System.NonSerialized] public Vector3 LookAt;
    Vector3 lookDir_;
    Vector3 moveVec_;
    bool isMoving_;
    Vector3 force_;
    Transform trans_;
    SpriteRenderer renderer_;
    Vector3 playerPos_;
    float MoveSpeed = 4f;
    float moveSpeedModifier_ = 1.0f;
    float flipX_;
    bool isDead_ = true;
    float playerScale_ = 2.0f;
    bool isShooting_;
    float nextFire_ = float.MaxValue;
    int shotsLeft_ = 0;
    public bool RoundComplete;
    public bool UpgradesActive = false;

    public Text OverheadText;

    public GameObject MineProto;
    public Transform Grenade;
    GrenadeScript grenadeScript_;

    Vector3 basePos_;
    RectTransform overheadTextTrans_;

    SpriteRenderer weaponRenderer_;
    Transform weaponTransform_;

    PlayerUpgrades _playerUpgrades;

    WeaponType WeaponTypeLeft;
    [System.NonSerialized] public WeaponBase Weapon;
    public Transform MeleeWeapon;

    Transform laserTransform_;
    SpriteRenderer laserRenderer_;

    ParticleSystem flames_;
    Transform flamesTransform_;

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
        grenadeScript_ = Grenade.GetComponent<GrenadeScript>();
        basePos_ = trans_.position;
        _playerUpgrades = GetComponent<PlayerUpgrades>();

        weaponTransform_ = trans_.Find("Weapon").GetComponent<Transform>();
        weaponRenderer_ = weaponTransform_.gameObject.GetComponent<SpriteRenderer>();

        laserTransform_ = trans_.Find("LaserLine").GetComponent<Transform>();
        laserRenderer_ = laserTransform_.gameObject.GetComponent<SpriteRenderer>();

        flames_ = trans_.Find("FlamethrowerParticles").GetComponent<ParticleSystem>();
        flamesTransform_ = flames_.transform;

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
        int flipX = LookAt.x < trans_.position.x ? -1 : 1;
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

    public void ResetAll()
    {
        UpgradesActive = false;
        MeleeWeapon.gameObject.SetActive(false);
        shadowRenderer_.enabled = true;
        moveSpeedModifier_ = 1.0f;
        isDead_ = false;
        isMoving_ = false;
        force_ = Vector3.zero;
        moveVec_ = Vector3.zero;
        SetPlayerPos(basePos_);
        laserRenderer_.enabled = false;
        grenadeScript_.Hide();
        LookAt = LookAt.x < 0.0f ? Vector3.left : Vector3.right;
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
        weaponTransform_.localScale = Weapon.Scale;
        weaponRenderer_.sprite = Weapon.Sprite;
        if (type != WeaponType.None)
            SetNextFire();
    }

    void RefreshBulletCount()
    {
        shotsLeft_ = BaseBullets + PlayerUpgrades.Data.MachinegunBulletsAdd;
    }

    void SetNextFire()
    {
        float FireCd = BaseCd * PlayerUpgrades.Data.WeaponsCdMul;
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
                Weapon.Fire(weaponTransform_, lookDir_, GameManager.Instance.SortLayerTopEffects, out recoil);
                AddForce(lookDir_ * -recoil);
                const float RecoilScreenShakeFactor = 2.0f;
                GameManager.Instance.ShakeCamera(recoil * RecoilScreenShakeFactor);

                if (Weapon.Type == WeaponType.Grenade)
                {
                    grenadeScript_.Throw(trans_.position, LookAt);
                }
                else if (Weapon.Type == WeaponType.Mine)
                {
                    var newMine = Instantiate<GameObject>(MineProto).GetComponent<MineScript>();
                    newMine.Throw(trans_.position, LookAt);
                }
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

    public void EjectGrenade(Vector3 pos, float radius, float damage)
    {
        var grenade = GameObject.Instantiate(Grenade);
        var script = grenade.GetComponent<GrenadeScript>();
        script.Throw(GameManager.Instance.Orc.transform.position, pos, radius, damage);
    }

    void UpdateWeapon()
    {
        if (Weapon == null)
            return;

        Vector3 muzzlePoint = Weapon.GetMuzzlePoint(weaponTransform_);
        Vector3 muzzleLook = (LookAt - muzzlePoint);

        // If closer than the muzzle use player position instead. Prevents insane oscillation.
        float lookLen2 = muzzleLook.sqrMagnitude;
        bool tooCloseForLaser = false;
        if (lookLen2 <= 1.0f)
        {
            tooCloseForLaser = true;
            muzzleLook = LookAt - trans_.position;
            lookLen2 = muzzleLook.sqrMagnitude;
            if (lookLen2 < 0.1f)
            {
                // Crosshair is on top of player
                return;
            }
        }

        float rot_z = Mathf.Atan2(muzzleLook.y, muzzleLook.x) * Mathf.Rad2Deg;
        flamesTransform_.SetPositionAndRotation(muzzlePoint, Quaternion.Euler(-rot_z, 90, 0)); // Compensate for initial emitter position (TODO PE: details...)

        if (flipX_ < 0)
            rot_z += 180;

        weaponTransform_.rotation = Quaternion.Euler(0f, 0f, rot_z);

        bool enable = Weapon.Type == WeaponType.Sniper && !tooCloseForLaser;
        laserRenderer_.enabled = enable;
        if (!enable)
            return;

        laserTransform_.SetPositionAndRotation(muzzlePoint, Quaternion.Euler(0f, 0f, rot_z));
    }

    public void KillPlayer()
    {
        StartCoroutine(EndGame(victory: false));
    }

    public void Victory()
    {
        StartCoroutine(EndGame(victory: true));
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        int layer = col.gameObject.layer;
        if (isDead_ || layer == GameManager.Instance.LayerPlayerProjectile || layer == GameManager.Instance.LayerNeutral || layer == GameManager.Instance.LayerOrc)
            return;

        var actor = col.gameObject.GetComponent<ActorBase>();
        if (actor != null && !actor.IsFullyReady)
            return;

        KillPlayer();
    }

    IEnumerator EndGame(bool victory)
    {
        yield return new WaitForEndOfFrame();

        isDead_ = true;
        shadowRenderer_.enabled = false;
        GameProgressScript.Instance.Stop();
        AudioManager.Instance.StopAllRepeating();
        SetWeapon(WeaponType.None);
        var clip = victory ? AudioManager.Instance.AudioData.Victory : AudioManager.Instance.AudioData.PlayerDie;
        AudioManager.Instance.PlayClip(clip);

        if (!victory)
        {
            GameManager.Instance.TriggerBlood(trans_.position, 1);
            GameEvents.CounterEvent(GameCounter.Player_Death, 1);
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
        if (Input.GetKeyDown(KeyCode.E))
            FloatingTextSpawner.Instance.Spawn(trans_.position, "-45", Color.red, speed: 0.01f, timeToLive: 0.5f);

        if (isDead_)
            return;

        if (GameManager.Instance.GameState != GameManager.State.Playing)
            return;

        float left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1.0f : 0.0f;
        float right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0.0f;
        float up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0.0f;
        float down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1.0f : 0.0f;

        float playerSpeedModifier = GameManager.Instance.CurrentGameModeData.PlayerMoveSpeedModifier;
        float speed = MoveSpeed * playerSpeedModifier;

        bool userMoved = left == 1.0f || right == 1.0f || up == 1.0f || down == 1.0f;
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

        LookAt = GameManager.Instance.CrosshairWorldPosition;
        lookDir_ = (LookAt - trans_.position).normalized;

        flipX_ = LookAt.x < trans_.position.x ? -playerScale_ : playerScale_;
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
        if (GameManager.Instance.PauseGameTime)
            return;

        renderer_.sortingOrder = Mathf.RoundToInt(trans_.position.y * 100f) * -1;

        if (isMoving_ && !isDead_)
            playerPos_ += moveVec_ * moveSpeedModifier_ * Timers.PlayerTimer;

        if (!isDead_)
            playerPos_ += force_ * Time.deltaTime * 60;

        playerPos_.z = 0;
        playerPos_ = GameManager.Instance.ClampToBounds(playerPos_, renderer_.sprite);
        trans_.position = playerPos_;
        force_ *= 1.0f - (20.0f * Time.deltaTime);

        CheckControls();
        UpdateWeapon();
        UpdateOverheadText();

        if (UpgradesActive && !isDead_)
            _playerUpgrades.UpdateUpgrades();
    }
}
