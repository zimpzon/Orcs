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
    bool enabled_;

    private void Awake()
    {
        text_ = GetComponent<Text>();
        text_.text = "";
        hideTime_ = float.MaxValue;

        defaultInfo_ = new GameInfo
        {
            Text = "default",
            Duration = 3.0f,

            Color = text_.color,
            FontSize = 10,
            Position = Vector2.up * -140,
        };
        enabled_ = false;
    }

    public void Show(GameInfo info)
    {
        var finalColor = (info.Color ?? defaultInfo_.Color).Value;
        hiddenColor = finalColor;
        hiddenColor.a = 0;
        text_.color = hiddenColor;

        text_.text = info.Text;
        text_.fontSize = (info.FontSize ?? defaultInfo_.FontSize).Value;
        hideTime_ = GameManager.Instance.GameTime + (info.Duration ?? defaultInfo_.Duration).Value;
        GetComponent<RectTransform>().anchoredPosition = (info.Position ?? defaultInfo_.Position).Value;

        fadeOutDuration_ = (info.FadeOutDuration ?? defaultInfo_.FadeOutDuration).Value;
        float fadeInDuration = (info.FadeInDuration ?? defaultInfo_.FadeInDuration).Value;
        LeanTween.textColor(text_.rectTransform, finalColor, fadeInDuration);
        enabled_ = true;
    }

    void Update()
    {
        if (!enabled_)
            return;

        if (GameManager.Instance.GameTime >= hideTime_)
        {
            void OnComplete()
            {
                text_.text = "";
                enabled_ = false;
                hideTime_ = float.MaxValue;
            }
            LeanTween.textColor(text_.rectTransform, hiddenColor, fadeOutDuration_).setOnComplete(OnComplete);
        }
    }

    public void Kill()
    {
        text_.text = "";
        enabled_ = false;
    }
}
