using PlayFab.ClientModels;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// cannot figure out how to place randomly as an image. And sprite didn't receive click events... *sigh*
public class PopupChestScript : MonoBehaviour, IPointerClickHandler
{
    public RectTransform imageRect; // Assign this in the Inspector
    public float margin = 100f;
    public const float ShowTime = 3;
    public const float HideTime = 3;
    public Sprite BaseSprite;
    public Sprite[] Animation;
    public Image image;
    private bool _wasClicked = false;

    private void Awake()
    {
        image = GetComponent<Image>();
        imageRect = GetComponent<RectTransform>();
        _wasClicked = false;
    }

    void Start()
    {
        image.sprite = BaseSprite;
        StartCoroutine(AnimateCo());
        StartCoroutine(StateCo());
    }

    private void OnMouseDown()
    {
        _wasClicked = true;
    }

    IEnumerator AnimateCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            for (int i = 0; i < Animation.Length; i++)
            {
                image.sprite = Animation[i];
                yield return new WaitForSeconds(0.1f);
            }

            image.sprite = BaseSprite;
        }
    }

    void SetRandomPos()
    {
        var randomPoint = PositionUtility.GetPointInsideArena(0.1f, 0.1f, 0.9f, 0.9f, avoidPlayer: false);
        transform.position = randomPoint;
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
                transform.position = Vector2.right * 77777;
                yield return new WaitForSeconds(HideTime);

                // Show.
                //PlaceRandomly();
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
                yield return new WaitForSeconds(2);
                break; // to outermost loop.
            }
        }

        void DoReward()
        {
            const int ForSeconds = 60;
            Decimal256 reward = GameManager.Instance.TotalPassiveIncome * ForSeconds;

            FloatingTextSpawner.Instance.Spawn(
                transform.position + Vector3.up * 2,
                $"$<color=yellow>{Format256.Format(reward)}</color>",
                Color.white,
                speed: 0.05f,
                timeToLive: 3.0f,
                fontStyle: TMPro.FontStyles.Bold);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICK");
    }
}
