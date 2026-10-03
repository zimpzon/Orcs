using TMPro;
using UnityEngine;

// Small grey "unlocked / total" line under a popup's header text (e.g. "Skins", "Bestiary"). Created at runtime by
// cloning the header, so it inherits its font and anchoring without any scene changes.
public static class PopupHeaderProgress
{
    const float SizeFactor = 0.65f;
    static readonly Color TextColor = new Color(0.67f, 0.67f, 0.67f);

    public static TextMeshProUGUI Create(Transform popup, string headerName)
    {
        var headerTransform = popup.Find(headerName);
        if (headerTransform == null || !headerTransform.TryGetComponent<TextMeshProUGUI>(out var header))
            return null;

        var go = Object.Instantiate(header.gameObject, header.transform.parent);
        go.name = headerName + "Progress";
        var text = go.GetComponent<TextMeshProUGUI>();
        text.fontSize = header.fontSize * SizeFactor;
        text.fontStyle = FontStyles.Normal;
        text.color = TextColor;
        text.enableWordWrapping = false;
        text.text = string.Empty;

        // Directly under the header line.
        var rt = text.rectTransform;
        rt.anchoredPosition = header.rectTransform.anchoredPosition + new Vector2(0, -header.fontSize * 1.15f);
        return text;
    }
}
