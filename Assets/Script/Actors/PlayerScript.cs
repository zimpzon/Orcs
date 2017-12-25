using Assets.Script;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [System.NonSerialized] public Vector3 LookAt;
    Vector3 lookDir_;
    Vector3 moveVec_;
    bool isMoving_;
    Vector3 force_;
    Transform trans_;
    SpriteRenderer renderer_;
    Vector3 playerPos_;
    float MoveSpeed = 5.0f;
    float moveSpeedModifier_ = 1.0f;
    float flipX_;
    bool isDead_;
    float playerScale_ = 2.0f;
    bool isShooting_;
    public bool RoundComplete;

    public Text OverheadText;
    public Transform Grenade;
    GrenadeScript grenadeScript_;

    Vector3 basePos_;
    RectTransform overheadTextTrans_;

    SpriteRenderer weaponRenderer_;
    Transform weaponTransform_;

    [System.NonSerialized] public WeaponBase Weapon;
    public Transform MeleeWeapon;
    SpriteRenderer meleeRenderer_;

    Transform laserTransform_;
    SpriteRenderer laserRenderer_;

    ParticleSystem floorTrail_;
    Vector3 lastEmitPos_;

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

        meleeRenderer_ = MeleeWeapon.GetComponentInChildren<SpriteRenderer>();

        weaponTransform_ = trans_.Find("Weapon").GetComponent<Transform>();
        weaponRenderer_ = weaponTransform_.gameObject.GetComponent<SpriteRenderer>();

        laserTransform_ = trans_.Find("LaserLine").GetComponent<Transform>();
        laserRenderer_ = laserTransform_.gameObject.GetComponent<SpriteRenderer>();

        floorTrail_ = trans_.Find("FloorTrail").GetComponent<ParticleSystem>();
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

    string getHolyWord()
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
        if (type == WeaponType.None)
        {
            Weapon = null;
            weaponRenderer_.sprite = null;
        }
        else
        {
            Weapon = WeaponBase.GetWeapon(type);
            weaponTransform_.localScale = Weapon.Scale;
            weaponRenderer_.sprite = Weapon.Sprite;
        }
    }

    IEnumerator Think()
    {
        bool wasFireDown = false;
        while (true)
        {
            if (GameManager.Instance.GameState != GameManager.State.Playing)
            {
                yield return null;
                continue;
            }

            bool fire = Input.GetMouseButton(0);
            if (Weapon != null && !fire && wasFireDown)
            {
                isShooting_ = false;
                Weapon.StopFire();
                moveSpeedModifier_ = 1.0f;
            }
            else if (fire && Weapon != null && Weapon.GetCdLeft() <= 0.0f)
            {
                isShooting_ = true;
                moveSpeedModifier_ = Weapon.MoveSpeedModifier;
                wasFireDown = true;
                float recoil;
                Weapon.Fire(weaponTransform_, lookDir_, GameManager.Instance.SortLayerTopEffects, out recoil);
                AddForce(lookDir_ * -recoil);
                const float RecoilScreenShakeFactor = 4.0f;
                GameManager.Instance.ShakeCamera(recoil * RecoilScreenShakeFactor);

                if (Weapon.Type == WeaponType.Grenade)
                {
                    grenadeScript_.Throw(trans_.position, LookAt);
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
                // Crosshair in on top of player
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
        if (isDead_ || layer == GameManager.Instance.LayerXpPill || layer == GameManager.Instance.LayerNeutral || layer == GameManager.Instance.LayerOrc)
            return;

        KillPlayer();
    }

    IEnumerator EndGame(bool victory)
    {
        yield return new WaitForEndOfFrame();

        isDead_ = true;
        shadowRenderer_.enabled = false;
        GameProgressScript.Instance.Stop();
        SetWeapon(WeaponType.None);
        var clip = victory ? AudioManager.Instance.AudioData.Victory : AudioManager.Instance.AudioData.PlayerDie;
        AudioManager.Instance.PlayClip(AudioManager.Instance.PlayerAudioSource, clip);

        if (!victory)
        {
            GameManager.Instance.TriggerBlood(trans_.position, 1);
            GameEvents.CounterEvent(GameCounter.Player_Death, 1);
            MusicManagerScript.Instance.StopMusic();
        }

        Camera cam = Camera.main;
        Time.timeScale = 0.0f;
        Vector3 cameraTarget = trans_.localPosition;

        RoundComplete = false;
        float targetOrthoSize = 5.0f;
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
            if (GameManager.Instance.GameState != GameManager.State.Dead && cam.orthographicSize < 6.0f)
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

        if (false)
        {
            GameManager.SetDebugOutput("Cheat enabled", Time.time);
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SetWeapon(WeaponType.Sword1);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                SetWeapon(WeaponType.Horn);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                SetWeapon(WeaponType.ShotgunSlug);

            if (Input.GetKeyDown(KeyCode.Alpha4))
                SetWeapon(WeaponType.Staff2);
        }

        float left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1.0f : 0.0f;
        float right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0.0f;
        float up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0.0f;
        float down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1.0f : 0.0f;

        float playerSpeedModifier = GameManager.Instance.CurrentGameModeData.PlayerMoveSpeedModifier;
        float speed = MoveSpeed * playerSpeedModifier;

        bool userMoved = left == 1.0f || right == 1.0f || up == 1.0f || down == 1.0f;
        if (userMoved && playerSpeedModifier == 0.0f)
        {
            // User tried to move but speed modifier is 0
            Talk("My legs! They chewed through the armor!", 0.25f, Color.red);
        }
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

    void LeaveTrail()
    {
        Vector3 pos = trans_.position;
        Vector3 move = pos - lastEmitPos_;
        if (move.sqrMagnitude > 0.05f)
        {
            lastEmitPos_ = pos;
            //Vector3 basePos = floorTrail_.transform.position;
            //Vector3 emitPos = basePos + Random.insideUnitCircle
            floorTrail_.Emit(1);
        }
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
        Talk(getHolyWord(), 1.0f, Color.white);
    }

    void UpdateOverheadText()
    {
        bool isRambo = Weapon != null && Weapon.Type == WeaponType.Rambo;
        if (isShooting_ && isRambo)
        {
            OverheadText.text = "AAAARGH!";
            OverheadText.enabled = true;

            Vector2 uiPos = GameManager.Instance.UiPositionFromWorld(trans_.position + Vector3.up * 0.5f + ((Vector3)Random.insideUnitCircle * 0.07f));
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
//        LeaveTrail();
    }
}
