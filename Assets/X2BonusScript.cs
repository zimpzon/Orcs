using System;
using TMPro;
using UnityEngine;

public class X2BonusScript : MonoBehaviour
{
    public TextMeshProUGUI BonusText;

    // "You get X% bonus income for every 5 of X2 bought." - was static text in the scene, but X2 Mastery cards
    // change the percentage, so it's filled in from code. Found by name to avoid a scene wiring change.
    TextMeshProUGUI _descriptionText;
    const string GreenHex = "#8DBE4C";

    // X2s bought per tier, listed for unlocked tiers only (level > 0) so the full roster stays a surprise.
    // Tier order matches the upgrade list.
    static readonly (string Name, Func<long> Level, Func<long> X2)[] Tiers =
    {
        ("Chain Zapping",   () => SaveGame.Members.LevelClickDamage,        () => SaveGame.Members.LevelClickDamageX2),
        ("Dagger Damage",   () => SaveGame.Members.LevelKnifeDamage,        () => SaveGame.Members.LevelKnifeDamageX2),
        ("Gold Value",      () => SaveGame.Members.LevelMoneyPerGold,       () => SaveGame.Members.LevelMoneyPerGoldX2),
        ("Dagger Cooldown", () => SaveGame.Members.LevelKnifeCd,            () => SaveGame.Members.LevelKnifeCdX2),
        ("Witch Doctor",    () => SaveGame.Members.LevelWitchDoctor,        () => SaveGame.Members.LevelWitchDoctorX2),
        ("Gold Per Dagger", () => SaveGame.Members.LevelGoldPerKnifeThrown, () => SaveGame.Members.LevelGoldPerKnifeThrownX2),
        ("Wizard",          () => SaveGame.Members.LevelWizard,             () => SaveGame.Members.LevelWizardX2),
        ("Master Wizard",   () => SaveGame.Members.LevelHoarder,            () => SaveGame.Members.LevelHoarderX2),
        ("Frenzy",          () => SaveGame.Members.LevelZapDamage,          () => SaveGame.Members.LevelZapDamageX2),
        ("Necromancer",     () => SaveGame.Members.LevelMoneyMaker,         () => SaveGame.Members.LevelMoneyMakerX2),
        ("Dagger Master",   () => SaveGame.Members.LevelDaggerMaster,       () => SaveGame.Members.LevelDaggerMasterX2),
        ("Necro Ninja",     () => SaveGame.Members.LevelNecroNinja,         () => SaveGame.Members.LevelNecroNinjaX2),
        ("Skull Crusher",   () => SaveGame.Members.LevelSkullCrusher,       () => SaveGame.Members.LevelSkullCrusherX2),
        ("Bountiful",       () => SaveGame.Members.LevelChestMaster,        () => SaveGame.Members.LevelChestMasterX2),
        ("Voidgazer",       () => SaveGame.Members.LevelVoidgazer,          () => SaveGame.Members.LevelVoidgazerX2),
        ("Smart Daggers",   () => SaveGame.Members.LevelSmartDaggers,       () => SaveGame.Members.LevelSmartDaggersX2),
        ("Windwalker",      () => SaveGame.Members.LevelFastFeet,           () => SaveGame.Members.LevelFastFeetX2),
        ("Crypt Master",    () => SaveGame.Members.LevelCryptMaster,        () => SaveGame.Members.LevelCryptMasterX2),
        ("Angry Fireballs", () => SaveGame.Members.LevelSmartFireballs,     () => SaveGame.Members.LevelSmartFireballsX2),
        ("Beefy Earl",      () => SaveGame.Members.LevelBeefyEarl,          () => SaveGame.Members.LevelBeefyEarlX2),
        ("Critical Strike", () => SaveGame.Members.LevelCriticalStrike,     () => SaveGame.Members.LevelCriticalStrikeX2),
        ("Power Zap",       () => SaveGame.Members.LevelPowerZap,           () => SaveGame.Members.LevelPowerZapX2),
        ("Skull Slicer",    () => SaveGame.Members.LevelSkullSlicer,        () => SaveGame.Members.LevelSkullSlicerX2),
    };

    // The popup grows downward (top-left pivot, children top-anchored, OK button bottom-anchored) to fit the list.
    RectTransform _popupRect;
    float _popupBaseHeight;
    float _bonusTextBaseHeight;
    string _lastText;
    readonly System.Text.StringBuilder _sb = new();

    void Awake()
    {
        var description = transform.Find("TextBonusDescription");
        if (description != null)
            _descriptionText = description.GetComponent<TextMeshProUGUI>();

        _popupRect = (RectTransform)transform;
        _popupBaseHeight = _popupRect.sizeDelta.y;
        _bonusTextBaseHeight = BonusText.rectTransform.sizeDelta.y;
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

    void Update()
    {
        long bought = PlayerUpgrades.Data.NumberOfX2Bought;
        long bonuses = bought / 5;
        long next = (bonuses + 1) * 5;

        _sb.Clear();
        _sb.Append($"Current: <color={GreenHex}>{bought}</color>, next: <color={GreenHex}>{next}</color>\r\nBonus: <color={GreenHex}>{PlayerUpgrades.Data.PassiveIncomeX2Multiplier * 100:0}</color>%");

        // Two columns: left entries start at 0%, right entries at 50% of the text width.
        bool anyListed = false;
        int column = 0;
        foreach (var tier in Tiers)
        {
            if (tier.Level() <= 0)
                continue;

            if (!anyListed)
            {
                // Left-align just the list; the lines above keep the text's own alignment.
                _sb.Append("\r\n<align=left>");
                anyListed = true;
            }

            // Name at the column start, X2 count at a fixed spot so the numbers line up.
            _sb.Append(column == 0 ? "\r\n<pos=4%>" : "<pos=53%>");
            _sb.Append($"<color=#cccccc>{tier.Name}</color>");
            _sb.Append(column == 0 ? "<pos=38%>" : "<pos=87%>");
            _sb.Append($"<color={GreenHex}>{tier.X2()}</color>");
            column = 1 - column;
        }

        string text = _sb.ToString();
        if (text != _lastText)
        {
            _lastText = text;
            BonusText.text = text;

            // Resize the text box and the popup to fit (only grows from the scene-authored size).
            var textRect = BonusText.rectTransform;
            float needed = BonusText.GetPreferredValues(text, textRect.rect.width, 0).y;
            float textHeight = Mathf.Max(_bonusTextBaseHeight, needed);
            textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textHeight);
            _popupRect.sizeDelta = new Vector2(_popupRect.sizeDelta.x, _popupBaseHeight + (textHeight - _bonusTextBaseHeight));
        }

        if (_descriptionText != null)
        {
            double pctPerRank = UpgradeManager.X2BonusPerRank() * 100;
            _descriptionText.text = $"You get <color={GreenHex}>{pctPerRank:0}%</color> bonus income for every <color={GreenHex}>5</color> of X2 bought.";
        }
    }
}
