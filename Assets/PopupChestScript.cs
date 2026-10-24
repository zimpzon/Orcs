using Assets.Script.Misc;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupChestScript : MonoBehaviour, IPointerClickHandler
{
    public static PopupChestScript Instance;

    public Color ColorDefault = Color.white;
    public Color ColorHighlight = Color.yellow;

    public LayerMask LayersToHit;
    public float margin = 100f;
    public const float ShowTime = 60 * 1;
    public const float HideTime = 60 * 8;
    public const int MaxRandomExtraHideTime = 60;
    public Sprite BaseSprite;
    public Sprite[] Animation;
    public SpriteRenderer _spriteRenderer;
    private bool _wasClicked = false;
    private ParticleSystem _particles;
    private ChestBling _bling;
    private Vector3 _baseScale;

    private void Awake()
    {
        Instance = this;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        BaseSprite = _spriteRenderer.sprite;
        _wasClicked = false;
        _particles = GetComponentInChildren<ParticleSystem>(true);
        _bling = gameObject.AddComponent<ChestBling>(); // glints + pickup show
        _baseScale = transform.localScale;
    }

    void Start()
    {
        StartCoroutine(AnimateCo());
        StartCoroutine(StateCo());
    }

    IEnumerator AnimateCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            for (int i = 0; i < Animation.Length; i++)
            {
                _spriteRenderer.sprite = Animation[i];
                yield return new WaitForSeconds(0.1f);
            }

            _spriteRenderer.sprite = BaseSprite;
        }
    }

    void SetRandomPos()
    {
        var randomPoint = PositionUtility.GetPointInsideArena(0.1f, 0.1f, 0.9f, 0.9f, avoidPlayer: false);
        transform.parent.position = randomPoint;
    }

    bool _forceShow;
    public void ShowNow()
    {
        _forceShow = true;
    }
    // hide
    // wait x seconds
    // show
    //  if timeout
    //    hide
    //      start over
    //  if clicked
    //    do pickup effects
    //    start over
    IEnumerator StateCo()
    {
        while (true)
        {
            while (true)
            {
                _wasClicked = false;

                // Stay hidden.
                transform.parent.position = Vector2.right * 77777;
                _particles.Stop();

                // Real time, like the mystery countdown: runs CheatSpeed times faster with the speed cheat.
                int randomSec = Random.Range(0, MaxRandomExtraHideTime);
                float showTime = HideTime + randomSec + G.D.RealTime;
                while (G.D.RealTime < showTime && !_forceShow)
                    yield return null;

                _forceShow = false;

                // Show (at full size again: the pickup show shrinks it to nothing).
                transform.localScale = _baseScale;
                SetRandomPos();
                _particles.Play();
                AudioManager.Instance.PlayClipForReal(AudioManager.Instance.AudioData.PickupReady, volumeScale: 0.25f, fixedPitch: true);
                _wasClicked = false;
                // Game time, not sped up by the speed cheat (G.D.RealTime is: at x10 the chest was only up for 6 s).
                float shownEndTime = G.D.GameTime + ShowTime;
                bool outOfTime = G.D.GameTime > shownEndTime;

                while (true)
                {
                    outOfTime = G.D.GameTime > shownEndTime;
                    if (_wasClicked || outOfTime)
                    {
                        break;
                    }
                    yield return null;
                }

                if (outOfTime)
                {
                    break; // To outermost loop.
                }

                // Was clicked.
                DoReward();
                yield return _bling.PickupCo(); // pop + shrink away, coins and glints
                break; // to outermost loop.
            }
        }

        void DoReward()
        {
            // Richer Chests (Gold Value tier) adds +1 per level, up to GoldPerRoundManager.MaxChestBonus.
            long numberOfSeconds = Random.Range(100, 200) + PlayerUpgrades.Data.ChestBonusSeconds;
            Decimal512 reward = GameManager.Instance.TotalPassiveIncome * (Decimal512)numberOfSeconds;
            if (PlayerUpgrades.Data.BetterChests)
                reward *= 2;

            reward += 100;

            GameManager.Instance.AddMoney(reward);
            SaveGame.Members.ChestsCollected += 1;

            // Reward popup like a mystery bonus: the money counting up on the band, how it was worked out below.
            string worth = PlayerUpgrades.Data.BetterChests
                ? $"<color={TextColors.GreyLight}>2 X {numberOfSeconds} seconds of your income.</color>"
                : $"<color={TextColors.GreyLight}>{numberOfSeconds} seconds of your income.</color>";
            RewardPopup.ShowMoney("CHEST LOOTED!\n", reward, $"\nYou found an old treasure chest!\n{worth}");
            SaveGame.Save(); // don't lose a chest reward to a reload before the next auto-save
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _wasClicked = true;
    }
}
