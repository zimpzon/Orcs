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

    private void Awake()
    {
        Instance = this;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        BaseSprite = _spriteRenderer.sprite;
        _wasClicked = false;
        _particles = GetComponentInChildren<ParticleSystem>();
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

                int randomSec = Random.Range(0, MaxRandomExtraHideTime);
                float showTime = HideTime + randomSec + G.D.GameTime;
                while (G.D.GameTime < showTime && !_forceShow)
                    yield return null;

                _forceShow = false;

                // Show.
                SetRandomPos();
                _particles.Play();
                _wasClicked = false;
                float shownEndTime = G.D.RealTime + ShowTime;
                bool outOfTime = G.D.RealTime > shownEndTime;

                while (true)
                {
                    outOfTime = G.D.RealTime > shownEndTime;
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

            // Same blue popup as a mystery bonus, with the full amount received (like the money display at the top).
            string amount = SaveGame.Members.UseScientificNotation ?
                FormatScientific.Format(reward) :
                Format512.FormatWithDecimals(reward, abbreviate: false, alwaysThreeDecimalsForLargeNumbers: true);
            string multiplier = PlayerUpgrades.Data.BetterChests ? "2 X " : "";
            GameCanvasScript.Instance.ShowPopup(
                $"<color=yellow><size=+1>CHEST COLLECTED ({SaveGame.Members.ChestsCollected})</size>\n\n</color>" +
                $"{multiplier}{numberOfSeconds}X income\n<size=+2>+${amount}</size>");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _wasClicked = true;
    }
}
