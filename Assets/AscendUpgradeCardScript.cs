using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum AscendUpgradeCardId
{
    NotSet,
    PassiveIncomeX2_1,
    PassiveIncomeX2_2,
    PassiveIncomeX4_1,
    PassiveIncomeX4_2,
    PassiveIncomeX4_3,
    PassiveIncomeX6_1,
    PassiveIncomeX7_1,
    FasterMystery,
    FasterArena,
    SkinScaryEarl,
    FasterArena2,
    ShinyDiamonds1, // income bonus per diamond held: +10% (on top of the base 10%)
    ShinyDiamonds2, // +20%
    ShinyDiamonds3, // +40%
    ShinyDiamonds4, // +60%
    ShinyDiamonds5, // +100%
    Haggler,        // tier upgrades and X2 cost half
    HeadStart,      // every run starts with 25 minutes of the best-ever income
    SkinCollector,  // +10% income per unlocked skin
    BeastScholar,   // enables the bestiary income bonus (step progression per beast, 840% total: BestiaryScript)
    Completionist,  // +1% income per game completion %
};

public class AscendUpgradeCardScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public long Cost = 1;
    public Color CanAffordColor = Color.white;
    public Color CannotAffordColor = Color.gray;
    public Color OwnedColor = Color.black;
    public Image ButtonOverlay;
    public Image CardOverlay;
    public Button ButtonBuy;
    public string HoverText = "(hover text missing)";
    public TextMeshProUGUI ButtonBuyText;
    public TextMeshProUGUI OwnedText;
    public AscendUpgradeCardId CardId = AscendUpgradeCardId.NotSet;
    //public AscendUpgradeCardScript UpgradeCardPassiveX2_1;
    //public AscendUpgradeCardScript UpgradeCardPassiveX2_2;
    //public AscendUpgradeCardScript UpgradeCardPassiveX4_1;

    Image _background;
    TextMeshProUGUI _refundLabel;
    [Tooltip("REFUND label: gap above the buy button's top edge (applied live, tweak it in play mode).")]
    public float RefundLabelY = -0.3f;

    // Owned cards can be sold back for their full price (the buy button turns into "+cost" with REFUND above it),
    // except Head Start and the Scary Earl skin.
    bool Refundable => CardId != AscendUpgradeCardId.NotSet && CardId != AscendUpgradeCardId.HeadStart
                       && CardId != AscendUpgradeCardId.SkinScaryEarl;

    private void Awake()
    {
        _background = GetComponent<Image>();

        // The owned overlay darkens the card but must not swallow the refund click: the button goes above it, with the
        // can't-afford overlay still above the button.
        CardOverlay.raycastTarget = false;
        // The card's texts (title, description, OWNED) reach past the card and caught clicks meant for buttons nearby;
        // the card's hover comes from its own background image anyway.
        foreach (var text in GetComponentsInChildren<TMP_Text>(true))
            text.raycastTarget = false;
        ButtonBuy.transform.SetAsLastSibling();
        ButtonOverlay.transform.SetAsLastSibling();

        // A clicked button stays "selected", and the prefab's selected color is a dull blue: right after buying, the
        // (now refund) button looked disabled. Selected looks like normal instead.
        var colors = ButtonBuy.colors;
        colors.selectedColor = colors.normalColor;
        ButtonBuy.colors = colors;

        if (Refundable)
        {
            _refundLabel = Instantiate(ButtonBuyText, ButtonBuy.transform);
            _refundLabel.name = "RefundLabel";
            _refundLabel.text = "REFUND";
            _refundLabel.fontSize = ButtonBuyText.fontSize * 0.85f;
            _refundLabel.raycastTarget = false;
            if (ColorUtility.TryParseHtmlString(TextColors.Gold, out var gold))
                _refundLabel.color = gold;
            var rt = _refundLabel.rectTransform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.sizeDelta = new Vector2(0f, 10f);
            rt.anchoredPosition = new Vector2(0f, RefundLabelY);
            _refundLabel.enabled = false;
        }
    }

    // Hovering a card shows its HoverText in the text below the cards (AscendDecisionScript.TextHover).
    // Touch: a tap pins it (TouchPin: same card again or anything else closes); a tap on the buy button doesn't.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!TouchPin.IsTouch(eventData))
            ShowDetails();
        else if (!TouchPin.HitChildButton(eventData, transform))
            TouchPin.Tap(this, ShowDetails, HideDetails);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!TouchPin.IsTouch(eventData))
            HideDetails();
    }

    void ShowDetails() => AscendDecisionScript.Instance.SetCardHover(this, HoverText);

    void HideDetails() => AscendDecisionScript.Instance.SetCardHover(this, null);

    void OnDisable()
    {
        if (AscendDecisionScript.Instance != null)
            AscendDecisionScript.Instance.SetCardHover(this, null);
    }

    void Update()
    {
        UpdateUi();
    }

    private void UpdateUi()
    {
        // Diamonds etc.
        AscendDecisionScript.Instance.UpdateUi();

        if (CardId == AscendUpgradeCardId.NotSet)
        {
            // Placeholder card - not wired to a real upgrade yet. Always locked/non-interactable.
            OwnedText.enabled = false;
            CardOverlay.enabled = false;
            _background.color = CannotAffordColor;
            ButtonOverlay.enabled = true;
            ButtonBuy.interactable = false;
            ButtonBuyText.text = "???";
            return;
        }

        UpdateUiForSingleCard(IsOwned());
    }

    bool IsOwned()
    {
        return CardId switch
        {
            AscendUpgradeCardId.PassiveIncomeX2_1 => SaveGame.Members.BoughtPassiveX2_1,
            AscendUpgradeCardId.PassiveIncomeX2_2 => SaveGame.Members.BoughtPassiveX2_2,
            AscendUpgradeCardId.PassiveIncomeX4_1 => SaveGame.Members.BoughtPassiveX4_1,
            AscendUpgradeCardId.PassiveIncomeX4_2 => SaveGame.Members.BoughtPassiveX4_2,
            AscendUpgradeCardId.PassiveIncomeX4_3 => SaveGame.Members.BoughtPassiveX4_3,
            AscendUpgradeCardId.PassiveIncomeX6_1 => SaveGame.Members.BoughtPassiveX6_1,
            AscendUpgradeCardId.PassiveIncomeX7_1 => SaveGame.Members.BoughtPassiveX7_1,
            AscendUpgradeCardId.FasterMystery => SaveGame.Members.BoughtFasterMystery,
            AscendUpgradeCardId.FasterArena => SaveGame.Members.BoughtFasterArena,
            AscendUpgradeCardId.SkinScaryEarl => SaveGame.Members.BoughtScaryEarlSkin,
            AscendUpgradeCardId.FasterArena2 => SaveGame.Members.BoughtFasterArena2,
            AscendUpgradeCardId.ShinyDiamonds1 => SaveGame.Members.BoughtShinyDiamonds1,
            AscendUpgradeCardId.ShinyDiamonds2 => SaveGame.Members.BoughtShinyDiamonds2,
            AscendUpgradeCardId.ShinyDiamonds3 => SaveGame.Members.BoughtShinyDiamonds3,
            AscendUpgradeCardId.ShinyDiamonds4 => SaveGame.Members.BoughtShinyDiamonds4,
            AscendUpgradeCardId.ShinyDiamonds5 => SaveGame.Members.BoughtShinyDiamonds5,
            AscendUpgradeCardId.Haggler => SaveGame.Members.BoughtHaggler,
            AscendUpgradeCardId.HeadStart => SaveGame.Members.BoughtHeadStart,
            AscendUpgradeCardId.SkinCollector => SaveGame.Members.BoughtSkinCollector,
            AscendUpgradeCardId.BeastScholar => SaveGame.Members.BoughtBeastScholar,
            AscendUpgradeCardId.Completionist => SaveGame.Members.BoughtCompletionist,
            _ => throw new NotImplementedException()
        };
    }

    private void UpdateUiForSingleCard(bool isOwned)
    {
        OwnedText.enabled = isOwned;
        CardOverlay.enabled = isOwned;

        bool refund = isOwned && Refundable;
        if (_refundLabel != null)
        {
            _refundLabel.enabled = refund;
            _refundLabel.rectTransform.anchoredPosition = new Vector2(0f, RefundLabelY);
        }

        if (isOwned)
        {
            _background.color = OwnedColor;
            ButtonOverlay.enabled = !refund; // Head Start / Scary Earl skin: owned for good, the button looks disabled
            ButtonBuy.interactable = refund;
        }
        else
        {
            bool canAfford = SaveGame.Members.DiamondCount_09_08_2025 >= EffectiveCost();
            _background.color = canAfford ? CanAffordColor : CannotAffordColor;
            ButtonOverlay.enabled = !canAfford;
            ButtonBuy.interactable = canAfford;
        }
        ButtonBuyText.text = refund ? $"+{EffectiveCost()} <sprite=0>" : $"{EffectiveCost()} <sprite=0>";
    }

    long EffectiveCost() => Cost;

    // The card's title as shown on it (for the event log).
    string CardTitle()
    {
        var title = transform.Find("TextTitle");
        string text = title != null && title.TryGetComponent<TMP_Text>(out var t) ? t.text : CardId.ToString();
        return System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", "").Replace('\n', ' ').Trim();
    }

    public void OnBuy()
    {
        if (CardId == AscendUpgradeCardId.NotSet)
            return;

        if (IsOwned())
        {
            if (Refundable)
                Refund();
            return;
        }

        Debug.Log("Bought " + CardId);
        EventLog.Add($"Bought the rebirth card {CardTitle()} for {EffectiveCost()} diamonds.");
        AudioManager.Instance.PlayClipForReal(AudioManager.Instance.AudioData.Chime, minInterval: 0.3f, fixedPitch: true);
        SetOwned(true);
        if (CardId == AscendUpgradeCardId.FasterMystery)
            SaveGame.Members.QuestionMarkRealTimeLeft *= 0.5f;

        SaveGame.Members.DiamondCount_09_08_2025 -= EffectiveCost();
        SaveGame.Save();
        AscendDecisionScript.Instance.UpdateUi();
    }

    // Sell an owned card back: full price returned, the effect is gone (all card effects read the Bought flags).
    void Refund()
    {
        Debug.Log("Refunded " + CardId);
        EventLog.Add($"Refunded the rebirth card {CardTitle()}: +{EffectiveCost()} diamonds.");
        AudioManager.Instance.PlayMenuClick();
        SetOwned(false);
        if (CardId == AscendUpgradeCardId.FasterMystery && SaveGame.Members.QuestionMarkRealTimeLeft > 0)
            SaveGame.Members.QuestionMarkRealTimeLeft *= 2f; // undo the halving from the purchase
        SaveGame.Members.DiamondCount_09_08_2025 += EffectiveCost();
        long cost = EffectiveCost();
        X2RankUpEffect.SpawnSmall(ButtonBuy, Input.mousePosition, $"+{cost} {(cost == 1 ? "diamond" : "diamonds")}");
        SaveGame.Save();
        AscendDecisionScript.Instance.UpdateUi();
    }

    void SetOwned(bool owned)
    {
        var m = SaveGame.Members;
        switch (CardId)
        {
            case AscendUpgradeCardId.PassiveIncomeX2_1: m.BoughtPassiveX2_1 = owned; break;
            case AscendUpgradeCardId.PassiveIncomeX2_2: m.BoughtPassiveX2_2 = owned; break;
            case AscendUpgradeCardId.PassiveIncomeX4_1: m.BoughtPassiveX4_1 = owned; break;
            case AscendUpgradeCardId.PassiveIncomeX4_2: m.BoughtPassiveX4_2 = owned; break;
            case AscendUpgradeCardId.FasterMystery: m.BoughtFasterMystery = owned; break;
            case AscendUpgradeCardId.FasterArena: m.BoughtFasterArena = owned; break;
            case AscendUpgradeCardId.SkinScaryEarl: m.BoughtScaryEarlSkin = owned; break;
            case AscendUpgradeCardId.FasterArena2: m.BoughtFasterArena2 = owned; break;
            case AscendUpgradeCardId.ShinyDiamonds1: m.BoughtShinyDiamonds1 = owned; break;
            case AscendUpgradeCardId.ShinyDiamonds2: m.BoughtShinyDiamonds2 = owned; break;
            case AscendUpgradeCardId.ShinyDiamonds3: m.BoughtShinyDiamonds3 = owned; break;
            case AscendUpgradeCardId.ShinyDiamonds4: m.BoughtShinyDiamonds4 = owned; break;
            case AscendUpgradeCardId.ShinyDiamonds5: m.BoughtShinyDiamonds5 = owned; break;
            case AscendUpgradeCardId.Haggler: m.BoughtHaggler = owned; break;
            case AscendUpgradeCardId.HeadStart: m.BoughtHeadStart = owned; break;
            case AscendUpgradeCardId.SkinCollector: m.BoughtSkinCollector = owned; break;
            case AscendUpgradeCardId.BeastScholar: m.BoughtBeastScholar = owned; break;
            case AscendUpgradeCardId.Completionist: m.BoughtCompletionist = owned; break;
            // PassiveIncomeX4_3 / X6_1 / X7_1: not in the game (no buy before either).
            default: throw new NotImplementedException();
        }
    }
}
