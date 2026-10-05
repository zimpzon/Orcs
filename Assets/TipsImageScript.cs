using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// "?" icon below the credits heart; opens the tips popup. Uses Icon if set, else draws a "?" with the game font
// (keeping its own Image, made transparent, as the click area).
public class TipsImageScript : MonoBehaviour, IPointerClickHandler
{
    public TipsPopupScript TipsPopup;
    public Sprite Icon;

    void Awake()
    {
        var image = GetComponent<Image>();
        if (image == null)
            return;

        if (Icon != null)
        {
            image.sprite = Icon;
            return;
        }

        Color tint = image.color;
        image.color = new Color(tint.r, tint.g, tint.b, 0f); // invisible, still clickable

        var go = new GameObject("QuestionMark", typeof(RectTransform));
        go.transform.SetParent(transform, false);
        var text = go.AddComponent<TextMeshProUGUI>();
        var fontSource = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .FirstOrDefault(t => t.font != null && t.font.name.Contains("Lato"));
        if (fontSource != null)
            text.font = fontSource.font;
        text.text = "?";
        text.fontStyle = FontStyles.Bold;
        text.fontSize = ((RectTransform)transform).rect.height;
        text.color = new Color(tint.r, tint.g, tint.b, 1f);
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        var rt = text.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TipsPopup.Show();
    }
}
