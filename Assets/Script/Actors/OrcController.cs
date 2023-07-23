using Assets.Script;
using System.Collections;
using TMPro;
using UnityEngine;

public enum OrcMood { Default, Nervous };

public enum OrcState { Default, Yoda };

public class OrcController : MonoBehaviour
{
    public OrcMood Mood;
    public OrcState State;
    public TextMeshPro Text;

    public SpriteRenderer Sunglasses;
    public Transform WeaponTransform;
    public SpriteRenderer WeaponRenderer;
    public Transform LaserTransform;
    public SpriteRenderer LaserRenderer;
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

    const float MeleeCd = 1.5f;
    const float RunSpeed = 2.0f;
    const float NervousMinDistance = 4.0f;

    void Awake()
    {
        trans_ = this.transform;
        renderer_ = GetComponent<SpriteRenderer>();
        hearts_ = trans_.Find("Hearts").GetComponent<ParticleSystem>();
        arrow_ = trans_.Find("Arrow").GetComponent<Transform>();
        Text.text = "";
    }

    private void Start()
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
            degrees -= 1500 * flipX * Time.deltaTime;
            yield return null;
        }

        MeleeWeapon.gameObject.SetActive(false);

        yield break;
    }

    public void SetUseWeapon(bool use)
    {
        Sunglasses.enabled = use;
        WeaponRenderer.enabled = use;
        LaserRenderer.enabled = use;

        Text.text = "";
    }

    public void SetChasePlayer(bool chase)
    {
        chasePlayer_ = true;
    }

    public void SetYoda()
    {
        State = OrcState.Yoda;
    }

    public void Hide()
    {
        SetPosition(Vector3.right * 10000);
        ResetAll();
    }

    public void SetPosition(Vector3 pos)
    {
        startPos_ = pos;
        trans_.position = pos;
        target_ = trans_.position;
    }

    void ResetAll()
    {
        chasePlayer_ = false;
        State = OrcState.Default;
        pickedUp_ = false;
        target_ = trans_.position;
        Text.text = string.Empty;
        MeleeWeapon.gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (pickedUp_)
            return;

        if (Mood != OrcMood.Nervous)
            hearts_.Emit(1);

        pickedUp_ = true;
        ResetAll();
        GameManager.Instance.OnOrcPickup(trans_.position);
    }

    IEnumerator Think()
    {
        float nextNervousMove = Time.time + 3.0f;
        float nextMeleeSwing = 0.0f;

        while (true)
        {
            var em = hearts_.emission;
            em.enabled = (playerIsClose_ || chasePlayer_) && Mood != OrcMood.Nervous;
            renderer_.flipX = trans_.position.x > lookAt_.x;

            if (chasePlayer_)
            {
                lookAt_ = GameManager.Instance.PlayerTrans.position;
                target_ = playerPos_;
            }

            if (State == OrcState.Yoda)
            {
                if (!chasePlayer_)
                    lookAt_ = target_;

                if (Time.time > nextMeleeSwing)
                {
                    nextMeleeSwing = Time.time + MeleeCd;
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
                lookAt_ = GameManager.Instance.PlayerTrans.position;
                if (Mood == OrcMood.Nervous)
                {
                    if (Time.time > nextNervousMove)
                    {
                        // Look confused for a little bit
                        Text.text = "?";
                        yield return new WaitForSeconds(2.0f);
                        if (Mood != OrcMood.Nervous || State != OrcState.Default) continue;

                        // Get alarmed for a very short time
                        Text.text = "!";
                        yield return new WaitForSeconds(0.5f);

                        if (Mood != OrcMood.Nervous || State != OrcState.Default) continue;
                        Text.text = "";

                        // Run randomly
                        target_ = PositionUtility.GetPointInsideArena(1.0f, 1.0f);
                        nextNervousMove = Time.time + 5 + Random.value * 5;
                    }

                    if (distanceToPlayer_ <= NervousMinDistance || distanceToTarget_ < 0.2f)
                        target_ = trans_.position; // Stop
                }
            }

            if (Mood != OrcMood.Nervous && Text.text != string.Empty)
                Text.text = string.Empty;

            yield return null;
        }
    }

    void Update()
    {
        Text.enabled = Mood == OrcMood.Nervous;
        renderer_.sortingOrder = Mathf.RoundToInt(trans_.position.y * 100f) * -1;

        playerPos_ = GameManager.Instance.PlayerTrans.position;
        distanceToPlayer_ = BlackboardScript.DistanceToPlayer(trans_.position);
        playerIsClose_ = distanceToPlayer_ < 3.0f;
        targetVec_ = target_ - trans_.position;
        targetDir_ = targetVec_.normalized;
        distanceToTarget_ = targetVec_.magnitude;

        Vector3 arrowPos = arrow_.localPosition;
        arrowPos.y = 1.0f + Mathf.Sin(Time.time * 8) * 0.25f;
        arrow_.localPosition = arrowPos;

        if (distanceToTarget_ > 0.1f)
            trans_.position += targetDir_ * RunSpeed * Time.deltaTime;
    }
}
