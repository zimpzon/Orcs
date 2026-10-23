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
        "into your Steam inventory. Items range from common to absolutely legendary.\n\n" +
        "Find them on your Steam profile under Inventory.\n" +
        "Rarer ones drop less often - getting Earl is a legendary achievement!";

    public static void Show(SteamItemDefs.Def def)
    {
        var canvas = GameCanvasScript.Instance;
        if (canvas == null)
            return;

        string name = def != null ? def.Name : "Something";
        string rarity = def != null ? def.Rarity.ToString().ToUpperInvariant() : "";
        string color = def != null ? "#" + ColorUtility.ToHtmlStringRGB(def.Color) : TextColors.Gold;

        // Three texts in the popup's vertical layout: header, the name on its band, then the rest with the link.
        var popup = canvas.CreatePopup($"<color={TextColors.Yellow}><size=130%>ITEM DROP!</size></color>", celebrate: true, sound: false);
        // The jingle chests and the mystery bonus play when they appear.
        AudioManager.Instance.PlayClipForReal(AudioManager.Instance.AudioData.PickupReady, volumeScale: 0.25f, fixedPitch: true);
        if (popup.TryGetComponent<RewardPopupFx>(out var fx))
            fx.Rays = false; // rays in the icon instead (AddIcon)
        var header = popup.GetComponentInChildren<TextMeshProUGUI>();
        if (header == null)
            return;

        // The name + rarity sit on a darkened band across the popup's width (PopupBuilder), so the rarity color reads well.
        var nameText = PopupBuilder.AddBand(header, $"<color={color}><size=115%>{name}</size>\n<size=80%>{rarity}</size></color>");
        var body = PopupBuilder.CloneText(header, PopupBuilder.AfterBand(nameText),
            "A collectible was added to your Steam inventory\n" +
            $"<link=\"{WhatIsThisLink}\"><u><color={TextColors.GreenSmallest}>What is this?</color></u></link>");
        PopupBuilder.FixMargins(header, body);

        body.raycastTarget = true;
        body.gameObject.AddComponent<TmpLinkClick>().OnLink = id =>
        {
            if (id == WhatIsThisLink)
                ShowExplanation(popup);
        };

        var sprite = def != null ? canvas.FindItemDropIcon(def.IconName) : null;
        if (sprite != null)
            AddIcon(header.transform, sprite, def);
    }

    // Icon above the text, inside the popup's vertical layout (the text's parent). Behind its transparent inside a
    // darker square, like Steam shows items on - semi-transparent black, so it darkens whatever the popup color is
    // instead of a solid dark block. Exactly the icon's size (the icon's frame runs to its edges).
    static readonly Color IconBackground = new Color32(90, 80, 65, 255);
    //static readonly Color IconBackground = new Color32(112, 105, 90, 110);
    const float IconPadding = 0f;
    const float IconTopSpace = 10f; // a bit below the popup's top edge

    // Rays inside the icon, in the rarity color: rarer = brighter and a little faster.
    static readonly float[] RayAlpha = { 0.20f, 0.5f, 0.65f, 0.6f, 0.6f };
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
