using TMPro;
using UnityEngine;
using UnityEngine.UI;

// "ITEM DROP!" message for a new Steam item (SteamItemDrops): the generic message popup (GameCanvasScript.CreatePopup)
// with the reward effects, the item's icon above the text, its name in the rarity color, and a "What is this?" link.
// The link swaps to an explanation popup; closing that brings the item drop popup back.
public static class ItemDropPopup
{
    const float IconSize = 80f;
    const string WhatIsThisLink = "whatisthis";

    const string Explanation =
        "<color=" + TextColors.Yellow + "><size=120%>ITEM DROPS</size></color>\n\n" +
        "While you play, Steam now and then drops a collectible\n" +
        "into your Steam inventory: Earl, Scary Earl or one of the beasts\n" +
        "from the arena, each with a rarity.\n\n" +
        "Find them on your Steam profile under Inventory.\n" +
        "Rarer ones drop less often - Earl and Scary Earl are the rarest of all.";

    public static void Show(SteamItemDefs.Def def)
    {
        var canvas = GameCanvasScript.Instance;
        if (canvas == null)
            return;

        string name = def != null ? def.Name : "Something";
        string rarity = def != null ? def.Rarity.ToString().ToUpperInvariant() : "";
        string color = def != null ? "#" + ColorUtility.ToHtmlStringRGB(def.Color) : TextColors.Gold;
        string text =
            $"<color={TextColors.Yellow}><size=130%>ITEM DROP!</size></color>\n\n" +
            $"<color={color}><size=115%>{name}</size>\n<size=80%>{rarity}</size></color>\n\n" +
            "A collectible was added to your Steam inventory.\n\n" +
            $"<link=\"{WhatIsThisLink}\"><u><color={TextColors.GreenSmallest}>What is this?</color></u></link>";

        var popup = canvas.CreatePopup(text, celebrate: true);
        var label = popup.GetComponentInChildren<TextMeshProUGUI>();

        var sprite = def != null ? canvas.FindItemDropIcon(def.IconName) : null;
        if (sprite != null && label != null)
            AddIcon(label.transform, sprite);

        if (label != null)
        {
            label.raycastTarget = true;
            label.gameObject.AddComponent<TmpLinkClick>().OnLink = id =>
            {
                if (id == WhatIsThisLink)
                    ShowExplanation(popup);
            };
        }
    }

    // Icon above the text, inside the popup's vertical layout (the text's parent).
    static void AddIcon(Transform label, Sprite sprite)
    {
        var go = new GameObject("ItemIcon", typeof(RectTransform));
        go.transform.SetParent(label.parent, false);
        go.transform.SetSiblingIndex(label.GetSiblingIndex());
        var image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        ((RectTransform)go.transform).sizeDelta = new Vector2(IconSize, IconSize);
        var layout = go.AddComponent<LayoutElement>();
        layout.preferredWidth = layout.minWidth = IconSize;
        layout.preferredHeight = layout.minHeight = IconSize;
    }

    static void ShowExplanation(GameObject itemPopup)
    {
        var explanation = GameCanvasScript.Instance.CreatePopup(Explanation);
        itemPopup.SetActive(false);
        explanation.AddComponent<OnDestroyCallback>().Action = () =>
        {
            if (itemPopup != null)
                itemPopup.SetActive(true);
        };
    }
}
