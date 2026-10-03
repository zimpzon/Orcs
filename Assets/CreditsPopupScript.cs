using TMPro;
using UnityEngine;

public class CreditsPopupScript : MonoBehaviour
{
    public TextMeshProUGUI ProgressText;

    static readonly string[] Thanks =
    {
        "Name One",
        "Name Two",
    };

    const string GoldHex = "#FFD54A";

    // Same layout as the victory popup it was cloned from: widen it (staying centered) and grow it downward to fit.
    const float ExtraWidth = 80f;
    RectTransform _popupRect;
    float _popupBaseHeight;
    float _textBaseHeight;

    void Awake()
    {
        _popupRect = (RectTransform)transform;
        _popupRect.sizeDelta += new Vector2(ExtraWidth, 0);
        _popupRect.anchoredPosition -= new Vector2(ExtraWidth * 0.5f, 0);

        var textRect = ProgressText.rectTransform;
        textRect.sizeDelta += new Vector2(ExtraWidth, 0);

        _popupBaseHeight = _popupRect.sizeDelta.y;
        _textBaseHeight = textRect.sizeDelta.y;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }
}
