using UnityEngine;
using UnityEngine.UI;

// Thin divider line, same look as the ones in the settings popup (2 px, sprite-less Image, black at 27% alpha).
public static class UiDivider
{
    public static readonly Color DividerColor = new Color(0f, 0f, 0f, 0.27450982f);
    public const float Thickness = 2f;

    // Center-anchored line under `parent`; place it with localPosition and size with sizeDelta.
    public static RectTransform Create(Transform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var image = go.AddComponent<Image>();
        image.color = DividerColor;
        image.raycastTarget = false;
        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        return rt;
    }
}
