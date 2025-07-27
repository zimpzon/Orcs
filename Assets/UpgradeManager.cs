using Assets.Script.Upgrades;
using System;
using UnityEngine;

public enum UpgradeDisplayStatus
{
    NotSet,     // Default value
    Hidden,     // Parent exists but is locked (== 0)
    NameOnly,   // Parent is unlocked (> 0), but this item is not purchased
    FullyShown  // This upgrade is purchased (> 0)
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public Color ColorPassiveValues = Color.white;
    public Color ColorArenaValues = Color.white;

    public UpgradeItemScript ClickDamage;
    public UpgradeItemScript KnifeDamage;
    public UpgradeItemScript GoldPerRound;
    public UpgradeItemScript KnifeCd;
    public UpgradeItemScript WitchDoctor;
    public UpgradeItemScript GoldPerKnife;
    public UpgradeItemScript Hoarder;
    public UpgradeItemScript Wizard;

    public UpgradeDisplayStatus DisplayStatusClickDamage = UpgradeDisplayStatus.NotSet;
    public UpgradeDisplayStatus DisplayStatusKnife = UpgradeDisplayStatus.NotSet;

    public static UpgradeDisplayStatus GetDisplayStatus(long parentLevel, long ourLevel)
    {
        if (ourLevel > 0)
            return UpgradeDisplayStatus.FullyShown;

        if (parentLevel == -1 || parentLevel > 0)
            return UpgradeDisplayStatus.NameOnly;

        return UpgradeDisplayStatus.Hidden;
    }

    public void UpdateAllUpgrades()
    {
        ClickDamageManager.UpdateAll();
        KnifeDamageManager.UpdateAll();
        ArenaGoldManager.UpdateAll();
        KnifeCdManager.UpdateAll();
        WitchDoctorManager.UpdateAll();
        GoldPerKnifeThrowManager.UpdateAll();
        HoarderManager.UpdateAll();
        WizardManager.UpdateAll();

        GameManager.Instance.TrySaveGame(forceSave: true);
    }

    private const string LockedText = "Buy one to see details.";

    private UpgradeDisplayStatus GetUpgradeDisplayStatus(UpgradeItemScript upgradeUiScript)
    {
        if (upgradeUiScript == ClickDamage)
        {
            return GetDisplayStatus(parentLevel: -1, ourLevel: SaveGame.Members.LevelClickDamage);
        }
        else if (upgradeUiScript == KnifeDamage)
        {
            return GetDisplayStatus(parentLevel: SaveGame.Members.LevelClickDamage, ourLevel: SaveGame.Members.LevelKnifeDamage);
        }
        else if (upgradeUiScript == GoldPerRound)
        {
            return GetDisplayStatus(parentLevel: SaveGame.Members.LevelKnifeDamage, ourLevel: SaveGame.Members.LevelMoneyPerGold);
        }
        else if (upgradeUiScript == KnifeCd)
        {
            return GetDisplayStatus(parentLevel: SaveGame.Members.LevelMoneyPerGold, ourLevel: SaveGame.Members.LevelKnifeCd);
        }
        else if (upgradeUiScript == WitchDoctor)
        {
            return GetDisplayStatus(parentLevel: SaveGame.Members.LevelKnifeCd, ourLevel: SaveGame.Members.LevelWitchDoctor);
        }
        else if (upgradeUiScript == GoldPerKnife)
        {
            return GetDisplayStatus(parentLevel: SaveGame.Members.LevelWitchDoctor, ourLevel: SaveGame.Members.LevelGoldPerKnifeThrown);
        }
        else if (upgradeUiScript == Hoarder)
        {
            return GetDisplayStatus(parentLevel: SaveGame.Members.LevelGoldPerKnifeThrown, ourLevel: SaveGame.Members.LevelHoarder);
        }
        else if (upgradeUiScript == Wizard)
        {
            return GetDisplayStatus(parentLevel: SaveGame.Members.LevelHoarder, ourLevel: SaveGame.Members.LevelWizard);
        }
        else
        {
            throw new NotImplementedException(upgradeUiScript.name);
        }
    }

    public string GetText(UpgradeItemScript upgradeUiScript)
    {
        string text = "";
        if (upgradeUiScript == ClickDamage)
        {
            var display = GetUpgradeDisplayStatus(ClickDamage);
            text = display == UpgradeDisplayStatus.FullyShown ? ClickDamageManager.GetText() : LockedText;
        }
        else if (upgradeUiScript == KnifeDamage)
        {
            var display = GetUpgradeDisplayStatus(KnifeDamage);
            text = display == UpgradeDisplayStatus.FullyShown ? KnifeDamageManager.GetText() : LockedText;
        }
        else if (upgradeUiScript == GoldPerRound)
        {
            var display = GetUpgradeDisplayStatus(GoldPerRound);
            text = display == UpgradeDisplayStatus.FullyShown ? ArenaGoldManager.GetText() : LockedText;
        }
        else if (upgradeUiScript == KnifeCd)
        {
            var display = GetUpgradeDisplayStatus(KnifeCd);
            text = display == UpgradeDisplayStatus.FullyShown ? KnifeCdManager.GetText() : LockedText;
        }
        else if (upgradeUiScript == WitchDoctor)
        {
            var display = GetUpgradeDisplayStatus(WitchDoctor);
            text = display == UpgradeDisplayStatus.FullyShown ? WitchDoctorManager.GetText() : LockedText;
        }
        else if (upgradeUiScript == GoldPerKnife)
        {
            var display = GetUpgradeDisplayStatus(GoldPerKnife);
            text = display == UpgradeDisplayStatus.FullyShown ? GoldPerKnifeThrowManager.GetText() : LockedText;
        }
        else if (upgradeUiScript == Hoarder)
        {
            var display = GetUpgradeDisplayStatus(Hoarder);
            text = display == UpgradeDisplayStatus.FullyShown ? HoarderManager.GetText() : LockedText;
        }
        else if (upgradeUiScript == Wizard)
        {
            var display = GetUpgradeDisplayStatus(Wizard);
            text = display == UpgradeDisplayStatus.FullyShown ? WizardManager.GetText() : LockedText;
        }
        else
        {
            text = $"unknown UpgradeItemScript: {upgradeUiScript.name}";
        }

        string colorPassiveHex = $"#{ColorUtility.ToHtmlStringRGB(ColorPassiveValues)}";
        string colorArenaHex = $"#{ColorUtility.ToHtmlStringRGB(ColorArenaValues)}";
        text = text.Replace("COLOR-PASSIVE", colorPassiveHex);
        text = text.Replace("COLOR-ARENA", colorArenaHex);

        return text;
    }

    public Decimal256 GetTotalPassiveIncome()
    {
        float incomeFactorPerFrame = GameManager.Instance.GetIncomeFactorPerFrame();

        SaveGame.Members.TotalIncomeClickDamage += ClickDamageManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeKnifeDamage += KnifeDamageManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeGoldPerRound += ArenaGoldManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeKnifeCd += KnifeCdManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeWitchDoctor += WitchDoctorManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeGoldPerKnifeThrow += GoldPerKnifeThrowManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeHoarder += HoarderManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeWizard += WizardManager.PassiveIncome() * incomeFactorPerFrame;

        Decimal256 fullSum = 0;
        fullSum += ClickDamageManager.PassiveIncome();
        fullSum += KnifeDamageManager.PassiveIncome();
        fullSum += ArenaGoldManager.PassiveIncome();
        fullSum += KnifeCdManager.PassiveIncome();
        fullSum += WitchDoctorManager.PassiveIncome();
        fullSum += GoldPerKnifeThrowManager.PassiveIncome();
        fullSum += HoarderManager.PassiveIncome();
        fullSum += WizardManager.PassiveIncome();

        SaveGame.Members.TotalIncomePassive += fullSum * incomeFactorPerFrame;

        return fullSum;
    }

    private void SetIsVisble(UpgradeItemScript upgradeItemScript)
    {
        var display = GetUpgradeDisplayStatus(upgradeItemScript);
        bool showGo = display is (UpgradeDisplayStatus.NameOnly or UpgradeDisplayStatus.FullyShown);
        bool wasActive = upgradeItemScript.gameObject.activeSelf;
        bool willBeVisible = showGo;

        upgradeItemScript.gameObject.SetActive(willBeVisible);

        if (!wasActive && willBeVisible)
        {
            var t = upgradeItemScript.transform;
            t.localScale = Vector3.zero;
            LeanTween.scale(t.gameObject, Vector3.one, 0.3f).setEaseOutBack();
        }
    }

    public void UpdateUpgradeUi()
    {
        SetIsVisble(ClickDamage);
        SetIsVisble(KnifeDamage);
        SetIsVisble(GoldPerRound);
        SetIsVisble(KnifeCd);
        SetIsVisble(WitchDoctor);
        SetIsVisble(GoldPerKnife);
        SetIsVisble(Hoarder);
        SetIsVisble(Wizard);

        ClickDamageManager.UpdateUi();
        KnifeDamageManager.UpdateUi();
        ArenaGoldManager.UpdateUi();
        KnifeCdManager.UpdateUi();
        WitchDoctorManager.UpdateUi();
        GoldPerKnifeThrowManager.UpdateUi();
        HoarderManager.UpdateUi();
        WizardManager.UpdateUi();
    }

    public void OnBuyClickDamage()
    {
        ClickDamageManager.OnBuy();
        ClickDamage.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyKnifeDamage()
    {
        KnifeDamageManager.OnBuy();
        KnifeDamage.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyArenaGold()
    {
        ArenaGoldManager.OnBuy();
        GoldPerRound.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyKnifeCooldown()
    {
        KnifeCdManager.OnBuy();
        KnifeCd.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyWitchDoctorDamage()
    {
        WitchDoctorManager.OnBuy();
        WitchDoctor.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyGoldPerKnife()
    {
        GoldPerKnifeThrowManager.OnBuy();
        GoldPerKnife.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyHoarder()
    {
        HoarderManager.OnBuy();
        Hoarder.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyWizard()
    {
        WizardManager.OnBuy();
        Wizard.SetPopupText();
        UpdateAllUpgrades();
    }

    private void UpdatePlayerUpgrades()
    {
        ClickDamageManager.UpdatePlayerUpgrades();
        KnifeDamageManager.UpdatePlayerUpgrades();
        ArenaGoldManager.UpdatePlayerUpgrades();
        KnifeCdManager.UpdatePlayerUpgrades();
        WitchDoctorManager.UpdatePlayerUpgrades();
        GoldPerKnifeThrowManager.UpdatePlayerUpgrades();
        HoarderManager.UpdatePlayerUpgrades();
        WizardManager.UpdatePlayerUpgrades();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdatePlayerUpgrades();
        UpdateUpgradeUi();
    }
}
