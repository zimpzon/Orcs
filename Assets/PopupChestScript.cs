using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupChestScript : MonoBehaviour, IPointerClickHandler
{
    public LayerMask LayersToHit;
    public float margin = 100f;
    public const float ShowTime = 60 * 1;
    public const float HideTime = 60 * 10;
    public const int MaxRandomExtraHideTime = 60;
    public Sprite BaseSprite;
    public Sprite[] Animation;
    public SpriteRenderer _spriteRenderer;
    private bool _wasClicked = false;
    private ParticleSystem _particles;

    private void Awake()
    {
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
                yield return new WaitForSeconds(HideTime + randomSec);

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
            long numberOfSeconds = Random.Range(100, 300);
            Decimal256 reward = GameManager.Instance.TotalPassiveIncome * (Decimal256)numberOfSeconds;
            reward += 100;

            GameManager.Instance.AddMoney(reward);
            SaveGame.Members.ChestsCollected += 1;

            FloatingTextSpawner.Instance.Spawn(
                transform.position + Vector3.up * 2,
                $"<size=+1>CHEST COLLECTED</size>\n<color=yellow>{numberOfSeconds}</color> X income = $<color=yellow>{Format256.Format(reward)}</color>",
                Color.white,
                speed: 0.1f,
                timeToLive: 5.0f,
                fontStyle: TMPro.FontStyles.Bold);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _wasClicked = true;
    }
}
