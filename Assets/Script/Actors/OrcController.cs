using Assets.Script;
using System.Collections;
using UnityEngine;

public enum OrcState { Default, Yoda };

public class OrcController : MonoBehaviour
{
    public OrcState State;
    public Color GhostColor;

    public Transform MeleeWeapon;
    public SpriteRenderer MeleeRenderer;

    bool chasePlayer_;
    float distanceToPlayer_;
    bool playerIsClose_;
    Vector3 startPos_;
    Vector3 playerPos_;
    Vector3 targetVec_;
    Vector3 targetDir_;
    float distanceToTarget_;
    Transform trans_;
    SpriteRenderer renderer_;
    ParticleSystem hearts_;
    Transform arrow_;
    bool pickedUp_;
    Vector3 target_;
    Vector3 lookAt_;
    Color baseColor_;
    float reviveTime_;

    const float MeleeCd = 1.5f;
    const float RunSpeed = 2.0f;

    void Awake()
    {
        trans_ = transform;
        renderer_ = GetComponent<SpriteRenderer>();
        hearts_ = trans_.Find("Hearts").GetComponent<ParticleSystem>();
        arrow_ = trans_.Find("Arrow").GetComponent<Transform>();
        baseColor_ = renderer_.color;
    }

    private void OnEnable()
    {
        ResetAll();
        StartCoroutine(Think());
    }

    public IEnumerator SwingMeleeCo(Vector3 target)
    {
        int flipX = target.x < trans_.position.x ? -1 : 1;
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
            degrees -= 1500 * flipX * GameManager.Instance.GameDeltaTime;
            yield return null;
        }

        MeleeWeapon.gameObject.SetActive(false);

        yield break;
    }

    public void SetChasePlayer(bool chase)
    {
        chasePlayer_ = chase;
    }

    public void SetYoda()
    {
        State = OrcState.Yoda;
    }

    void Hide()
    {
        SetPosition(Vector3.right * 238, startingGame: false);
    }

    public void SetPosition(Vector3 pos, bool startingGame = false)
    {
        startPos_ = pos;
        trans_.position = pos;
        target_ = trans_.position;
        if (startingGame)
            MakeGhost(false);
    }

    public void ResetAll()
    {
        chasePlayer_ = false;
        State = OrcState.Default;
        pickedUp_ = false;
        target_ = trans_.position;
        MeleeWeapon.gameObject.SetActive(false);
        Hide();
        MakeGhost(true);
    }

    void MakeGhost(bool isGhost)
    {
        arrow_.gameObject.SetActive(!isGhost);
        GetComponent<SpriteRenderer>().color = isGhost ? GhostColor : baseColor_;
        GetComponent<Collider2D>().enabled = !isGhost;
        reviveTime_ = G.D.GameTime + PlayerUpgrades.Data.OrcReviveTime * PlayerUpgrades.Data.OrcReviveTimeMul;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (pickedUp_)
            return;

        GameManager.Instance.OnOrcPickup(trans_.position); 
        ResetAll();

        pickedUp_ = true;
        SetPosition(PositionUtility.GetPointInsideArena());

        if (PlayerUpgrades.Data.OrcJedisEnabled)
            SetYoda();
    }

    IEnumerator Think()
    {
        float nextMeleeSwing = 0.0f;

        while (true)
        {
            while (pickedUp_)
            {
                if (G.D.GameTime > reviveTime_)
                {
                    pickedUp_ = false;
                    MakeGhost(false);
                    Explosions.Push(trans_.position, radius: 1.5f, force: 2.0f);
                }

                yield return null;
            }

            var em = hearts_.emission;
            em.enabled = playerIsClose_ || chasePlayer_;
            renderer_.flipX = trans_.position.x > lookAt_.x;

            if (chasePlayer_)
            {
                lookAt_ = G.D.PlayerPos;
                target_ = playerPos_;
            }

            if (State == OrcState.Yoda)
            {
                if (!chasePlayer_)
                    lookAt_ = target_;

                if (G.D.GameTime > nextMeleeSwing)
                {
                    nextMeleeSwing = G.D.GameTime + MeleeCd;
                    StartCoroutine(SwingMeleeCo(lookAt_));
                    float YodaRadius = 2.0f;
                    _ = WeaponSword.Swing(trans_.position, damage: 0.0f, YodaRadius, AudioManager.Instance.AudioData.SaberHit, AudioManager.Instance.AudioData.SaberSwing, out _);

                    if (!chasePlayer_)
                        target_ = (Vector3)(Random.insideUnitCircle * 2) + startPos_;
                }

                if (distanceToPlayer_ < 3)
                    target_ = trans_.position;
            }
            else if (State == OrcState.Default)
            {
                lookAt_ = G.D.PlayerPos;
            }

            yield return null;
        }
    }

    public void Enable(bool isActive)
    {
        if (isActive && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            G.D.OrcEnabled = true;
        }
        else if (!isActive && gameObject.activeSelf)
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            G.D.OrcEnabled = false;
        }
    }

    void Update()
    {
        if (G.D.OrcEnabled && enabled)
        {
            // go away when disabled
            gameObject.SetActive(false);
        }

        renderer_.sortingOrder = Mathf.RoundToInt(trans_.position.y * 100f) * -1;

        playerPos_ = G.D.PlayerPos;
        distanceToPlayer_ = BlackboardScript.DistanceToPlayer(trans_.position);
        playerIsClose_ = distanceToPlayer_ < 3.0f;
        targetVec_ = target_ - trans_.position;
        targetDir_ = targetVec_.normalized;
        distanceToTarget_ = targetVec_.magnitude;

        Vector3 arrowPos = arrow_.localPosition;
        arrowPos.y = 1.0f + Mathf.Sin(G.D.GameTime * 8) * 0.25f;
        arrow_.localPosition = arrowPos;

        if (distanceToTarget_ > 0.1f)
            trans_.position += targetDir_ * RunSpeed * GameManager.Instance.GameDeltaTime;
    }
}
