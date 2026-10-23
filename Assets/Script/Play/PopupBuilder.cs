using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Building blocks for richer message popups on top of the generic popup prefab (GameCanvasScript.CreatePopup): extra
// texts, fixed gaps, and a darkened full-width band with a text on it. Used by ItemDropPopup and WelcomeBackPopup.
// Note: the prefab's text has a 50 px bottom margin to make room for the OK button - callers keep it only on the last
// text (FixMargins).
public static class PopupBuilder
{
    public static readonly Color BandColor = new Color(0f, 0f, 0f, 0.35f);
    const int BandPaddingX = 8, BandPaddingY = 1;
    public const float GapAroundBand = 8f;     // above and below the band
    public const float LastTextBottomMargin = 40f; // room for the OK button below the last text (the prefab's is 50)

    // Fixed vertical gap in the popup's layout.
    public static void AddSpace(Transform parent, int siblingIndex, float height)
    {
        var go = new GameObject("Space", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.transform.SetSiblingIndex(siblingIndex);
        var layout = go.AddComponent<LayoutElement>();
        layout.preferredHeight = layout.minHeight = height;
    }

    // A copy of the popup's text (same font, size, layout settings) at a position in the layout, no margins.
    public static TextMeshProUGUI CloneText(TextMeshProUGUI source, int siblingIndex, string text)
    {
        var copy = Object.Instantiate(source, source.transform.parent);
        copy.name = source.name;
        copy.transform.SetSiblingIndex(siblingIndex);
        copy.text = text;
        copy.margin = Vector4.zero;
        return copy;
    }

    // Darkened band right after `after`, stretched to the popup's width, with gaps around it and a text on it.
    public static TextMeshProUGUI AddBand(TextMeshProUGUI after, string text, string name = "Band")
    {
        var parent = after.transform.parent;
        int index = after.transform.GetSiblingIndex();
        AddSpace(parent, index + 1, GapAroundBand);
        var band = new GameObject(name, typeof(RectTransform));
        band.transform.SetParent(parent, false);
        band.transform.SetSiblingIndex(index + 2);
        AddSpace(parent, index + 3, GapAroundBand);
        var image = band.AddComponent<Image>();
        image.color = BandColor;
        image.raycastTarget = false;
        band.AddComponent<LayoutElement>().flexibleWidth = 1; // full width of the popup
        var layout = band.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(BandPaddingX, BandPaddingX, BandPaddingY, BandPaddingY);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = layout.childControlHeight = true;
        layout.childForceExpandWidth = layout.childForceExpandHeight = false;

        var copy = Object.Instantiate(after, band.transform);
        copy.name = after.name;
        copy.text = text;
        copy.margin = Vector4.zero;
        return copy;
    }

    // Sibling index right after a band added by AddBand (past the band and its lower gap).
    public static int AfterBand(TextMeshProUGUI bandText) => bandText.transform.parent.GetSiblingIndex() + 2;

    // The header keeps its side/top margins but not the prefab's big bottom one; the last text gets the room for OK.
    public static void FixMargins(TextMeshProUGUI header, TextMeshProUGUI last)
    {
        var m = header.margin;
        header.margin = new Vector4(m.x, m.y, m.z, 0);
        last.margin = new Vector4(m.x, 0, m.z, LastTextBottomMargin);
    }
}
