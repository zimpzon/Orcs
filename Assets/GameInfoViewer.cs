using UnityEngine;
using UnityEngine.UI;

public class GameInfo
{
    public string Text;
    public float? Duration;
    public float? FadeInDuration;
    public float? FadeOutDuration;
    public Color? Color;
    public int? FontSize;
    public Vector2? Position;
}

public class GameInfoViewer : MonoBehaviour, IKillableObject
{
    Color hiddenColor;
    Text text_;
    float hideTime_;
    float fadeOutDuration_;
    GameInfo defaultInfo_;

    private void Awake()
    {
        text_ = GetComponent<Text>();
        hideTime_ = float.MaxValue;

        defaultInfo_ = new GameInfo
        {
            Text = "default",
            Duration = 3.0f,

            Color = text_.color,
            FontSize = 10,
            Position = Vector2.up * -140,
        };
        gameObject.SetActive(false);
    }

    public void Show(GameInfo info)
    {
        Debug.Log("show");
        var finalColor = (info.Color ?? defaultInfo_.Color).Value;
        hiddenColor = finalColor;
        hiddenColor.a = 0;
        text_.color = hiddenColor;

        text_.text = info.Text;
        text_.fontSize = (info.FontSize ?? defaultInfo_.FontSize).Value;
        hideTime_ = GameManager.Instance.GameTime + (info.Duration ?? defaultInfo_.Duration).Value;
        GetComponent<RectTransform>().anchoredPosition = (info.Position ?? defaultInfo_.Position).Value;

        gameObject.SetActive(true);

        fadeOutDuration_ = (info.FadeOutDuration ?? defaultInfo_.FadeOutDuration).Value;
        float fadeInDuration = (info.FadeInDuration ?? defaultInfo_.FadeInDuration).Value;
        LeanTween.textColor(text_.rectTransform, finalColor, fadeInDuration);
    }

    void Update()
    {
        if (GameManager.Instance.GameTime >= hideTime_)
        {
            void OnComplete()
            {
                gameObject.SetActive(false);
                hideTime_ = float.MaxValue;
            }
            LeanTween.textColor(text_.rectTransform, hiddenColor, fadeOutDuration_).setOnComplete(OnComplete);
        }
    }

    public void Kill()
    {
        gameObject.SetActive(false);
    }
}
