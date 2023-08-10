using UnityEngine;
using UnityEngine.UI;

public class GameInfoViewer : MonoBehaviour, IKillableObject
{
    Color defaultColor;
    Color hiddenColor;
    Text text_;
    float hideTime_;

    private void Awake()
    {
        text_ = GetComponent<Text>();
        defaultColor = text_.color;
        hideTime_ = float.MaxValue;
        gameObject.SetActive(false);
    }

    public void Show(string text, float duration, Color? color = null)
    {
        var finalColor = color ?? defaultColor;
        hiddenColor = finalColor;
        hiddenColor.a = 0;
        text_.color = hiddenColor;

        text_.text = text;
        hideTime_ = GameManager.Instance.GameTime + duration;
        gameObject.SetActive(true);

        LeanTween.textColor(text_.rectTransform, finalColor, 0.25f);
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
            LeanTween.textColor(text_.rectTransform, hiddenColor, 1.0f).setOnComplete(OnComplete);
        }
    }

    public void Kill()
    {
        gameObject.SetActive(false);
    }
}
