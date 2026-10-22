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
        if (popup.TryGetComponent<RewardPopupFx>(out var fx))
            fx.Rays = false; // rays in the icon instead (AddIcon)
        var label = popup.GetComponentInChildren<TextMeshProUGUI>();

        var sprite = def != null ? canvas.FindItemDropIcon(def.IconName) : null;
        if (sprite != null && label != null)
            AddIcon(label.transform, sprite, def);

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

    // Icon above the text, inside the popup's vertical layout (the text's parent). Behind its transparent inside a
    // darker square, like Steam shows items on - semi-transparent black, so it darkens whatever the popup color is
    // instead of a solid dark block. Exactly the icon's size (the icon's frame runs to its edges).
    static readonly Color IconBackground = new Color(0f, 0f, 0f, 0.4f);
    const float IconPadding = 0f;
    const float IconTopSpace = 10f; // a bit below the popup's top edge

    // Rays inside the icon, in the rarity color: rarer = brighter and a little faster.
    static readonly float[] RayAlpha = { 0.18f, 0.26f, 0.34f, 0.44f, 0.6f };
    static readonly float[] RaySpeed = { 10f, 12f, 14f, 17f, 20f };

    static void AddIcon(Transform label, Sprite sprite, SteamItemDefs.Def def)
    {
        float size = IconSize + IconPadding * 2;

        var space = new GameObject("ItemIconSpace", typeof(RectTransform));
        space.transform.SetParent(label.parent, false);
        space.transform.SetSiblingIndex(label.GetSiblingIndex());
        var spaceLayout = space.AddComponent<LayoutElement>();
        spaceLayout.preferredHeight = spaceLayout.minHeight = IconTopSpace;

        var back = new GameObject("ItemIconBackground", typeof(RectTransform));
        back.transform.SetParent(label.parent, false);
        back.transform.SetSiblingIndex(label.GetSiblingIndex());
        var backImage = back.AddComponent<Image>();
        backImage.color = IconBackground;
        backImage.raycastTarget = false;
        ((RectTransform)back.transform).sizeDelta = new Vector2(size, size);
        var layout = back.AddComponent<LayoutElement>();
        layout.preferredWidth = layout.minWidth = size;
        layout.preferredHeight = layout.minHeight = size;
        back.AddComponent<RectMask2D>(); // clips the rays to the square

        int r = (int)def.Rarity;
        var rays = new GameObject("ItemIconRays", typeof(RectTransform));
        rays.transform.SetParent(back.transform, false);
        var raysImage = rays.AddComponent<Image>();
        raysImage.sprite = VictoryCelebration.RaySprite();
        Color c = def.Color;
        raysImage.color = new Color(c.r, c.g, c.b, RayAlpha[r]);
        raysImage.raycastTarget = false;
        ((RectTransform)rays.transform).sizeDelta = Vector2.one * size * 1.6f;
        rays.AddComponent<ItemIconRays>().Speed = RaySpeed[r];

        var go = new GameObject("ItemIcon", typeof(RectTransform));
        go.transform.SetParent(back.transform, false);
        var image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        var rt = (RectTransform)go.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(IconPadding, IconPadding);
        rt.offsetMax = new Vector2(-IconPadding, -IconPadding);
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
