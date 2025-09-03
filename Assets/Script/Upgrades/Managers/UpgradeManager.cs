using Assets.Script.Upgrades;
using System;
using UnityEngine;

public enum UpgradeDisplayStatus
{
    NotSet,
    Hidden,
    NameOnly,
    FullyShown
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
    public UpgradeItemScript Wizard;
    public UpgradeItemScript Hoarder;
    public UpgradeItemScript ZapDamage;
    public UpgradeItemScript MoneyMaker;
    public UpgradeItemScript DaggerMaster;
    public UpgradeItemScript NecroNinja;
    public UpgradeItemScript SkullCrusher;
    public UpgradeItemScript ChestMaster;
    public UpgradeItemScript Voidgazer;

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
        WizardManager.UpdateAll();
        HoarderManager.UpdateAll();
        ZapDamageManager.UpdateAll();
        MoneyMakerManager.UpdateAll();
        DaggerMasterManager.UpdateAll();
        NecroNinjaManager.UpdateAll();
        SkullCrusherManager.UpdateAll();
        ChestMasterManager.UpdateAll();
        VoidgazerManager.UpdateAll();

        GameManager.Instance.TrySaveGame(forceSave: true);
    }

    private const string LockedText = "Buy one to see details.";

    private UpgradeDisplayStatus GetUpgradeDisplayStatus(UpgradeItemScript upgradeUiScript)
    {
        if (upgradeUiScript == ClickDamage)
            return GetDisplayStatus(-1, SaveGame.Members.LevelClickDamage);
        else if (upgradeUiScript == KnifeDamage)
            return GetDisplayStatus(SaveGame.Members.LevelClickDamage, SaveGame.Members.LevelKnifeDamage);
        else if (upgradeUiScript == GoldPerRound)
            return GetDisplayStatus(SaveGame.Members.LevelKnifeDamage, SaveGame.Members.LevelMoneyPerGold);
        else if (upgradeUiScript == KnifeCd)
            return GetDisplayStatus(SaveGame.Members.LevelMoneyPerGold, SaveGame.Members.LevelKnifeCd);
        else if (upgradeUiScript == WitchDoctor)
            return GetDisplayStatus(SaveGame.Members.LevelKnifeCd, SaveGame.Members.LevelWitchDoctor);
        else if (upgradeUiScript == GoldPerKnife)
            return GetDisplayStatus(SaveGame.Members.LevelWitchDoctor, SaveGame.Members.LevelGoldPerKnifeThrown);
        else if (upgradeUiScript == Wizard)
            return GetDisplayStatus(SaveGame.Members.LevelGoldPerKnifeThrown, SaveGame.Members.LevelWizard);
        else if (upgradeUiScript == Hoarder)
            return GetDisplayStatus(SaveGame.Members.LevelWizard, SaveGame.Members.LevelHoarder);
        else if (upgradeUiScript == ZapDamage)
            return GetDisplayStatus(SaveGame.Members.LevelHoarder, SaveGame.Members.LevelZapDamage);
        else if (upgradeUiScript == MoneyMaker)
            return GetDisplayStatus(SaveGame.Members.LevelZapDamage, SaveGame.Members.LevelMoneyMaker);
        else if (upgradeUiScript == DaggerMaster)
            return GetDisplayStatus(SaveGame.Members.LevelMoneyMaker, SaveGame.Members.LevelDaggerMaster);
        else if (upgradeUiScript == NecroNinja)
            return GetDisplayStatus(SaveGame.Members.LevelDaggerMaster, SaveGame.Members.LevelNecroNinja);
        else if (upgradeUiScript == SkullCrusher)
            return GetDisplayStatus(SaveGame.Members.LevelNecroNinja, SaveGame.Members.LevelSkullCrusher);
        else if (upgradeUiScript == ChestMaster)
            return GetDisplayStatus(SaveGame.Members.LevelSkullCrusher, SaveGame.Members.LevelChestMaster);
        else if (upgradeUiScript == Voidgazer)
            return GetDisplayStatus(SaveGame.Members.LevelChestMaster, SaveGame.Members.LevelVoidgazer);
        else
            throw new NotImplementedException(upgradeUiScript.name);
    }

    public string GetText(UpgradeItemScript upgradeUiScript)
    {
        string text = "";

        if (upgradeUiScript == ClickDamage)
            text = GetUpgradeDisplayStatus(ClickDamage) == UpgradeDisplayStatus.FullyShown ? ClickDamageManager.GetText() : LockedText;
        else if (upgradeUiScript == KnifeDamage)
            text = GetUpgradeDisplayStatus(KnifeDamage) == UpgradeDisplayStatus.FullyShown ? KnifeDamageManager.GetText() : LockedText;
        else if (upgradeUiScript == GoldPerRound)
            text = GetUpgradeDisplayStatus(GoldPerRound) == UpgradeDisplayStatus.FullyShown ? ArenaGoldManager.GetText() : LockedText;
        else if (upgradeUiScript == KnifeCd)
            text = GetUpgradeDisplayStatus(KnifeCd) == UpgradeDisplayStatus.FullyShown ? KnifeCdManager.GetText() : LockedText;
        else if (upgradeUiScript == WitchDoctor)
            text = GetUpgradeDisplayStatus(WitchDoctor) == UpgradeDisplayStatus.FullyShown ? WitchDoctorManager.GetText() : LockedText;
        else if (upgradeUiScript == GoldPerKnife)
            text = GetUpgradeDisplayStatus(GoldPerKnife) == UpgradeDisplayStatus.FullyShown ? GoldPerKnifeThrowManager.GetText() : LockedText;
        else if (upgradeUiScript == Wizard)
            text = GetUpgradeDisplayStatus(Wizard) == UpgradeDisplayStatus.FullyShown ? WizardManager.GetText() : LockedText;
        else if (upgradeUiScript == Hoarder)
            text = GetUpgradeDisplayStatus(Hoarder) == UpgradeDisplayStatus.FullyShown ? HoarderManager.GetText() : LockedText;
        else if (upgradeUiScript == ZapDamage)
            text = GetUpgradeDisplayStatus(ZapDamage) == UpgradeDisplayStatus.FullyShown ? ZapDamageManager.GetText() : LockedText;
        else if (upgradeUiScript == MoneyMaker)
            text = GetUpgradeDisplayStatus(MoneyMaker) == UpgradeDisplayStatus.FullyShown ? MoneyMakerManager.GetText() : LockedText;
        else if (upgradeUiScript == DaggerMaster)
            text = GetUpgradeDisplayStatus(DaggerMaster) == UpgradeDisplayStatus.FullyShown ? DaggerMasterManager.GetText() : LockedText;
        else if (upgradeUiScript == NecroNinja)
            text = GetUpgradeDisplayStatus(NecroNinja) == UpgradeDisplayStatus.FullyShown ? NecroNinjaManager.GetText() : LockedText;
        else if (upgradeUiScript == SkullCrusher)
            text = GetUpgradeDisplayStatus(SkullCrusher) == UpgradeDisplayStatus.FullyShown ? SkullCrusherManager.GetText() : LockedText;
        else if (upgradeUiScript == ChestMaster)
            text = GetUpgradeDisplayStatus(ChestMaster) == UpgradeDisplayStatus.FullyShown ? ChestMasterManager.GetText() : LockedText;
        else if (upgradeUiScript == Voidgazer)
            text = GetUpgradeDisplayStatus(Voidgazer) == UpgradeDisplayStatus.FullyShown ? VoidgazerManager.GetText() : LockedText;
        else
            text = $"unknown UpgradeItemScript: {upgradeUiScript.name}";

        string colorPassiveHex = $"#{ColorUtility.ToHtmlStringRGB(ColorPassiveValues)}";
        string colorArenaHex = $"#{ColorUtility.ToHtmlStringRGB(ColorArenaValues)}";
        text = text.Replace("COLOR-PASSIVE", colorPassiveHex);
        text = text.Replace("COLOR-ARENA", colorArenaHex);

        return text;
    }

    // Will also update statistics, which is bad. Yikes.
    float _prevCallTimeGetTotalPassiveIncomeForFrame = 0;
    public Decimal512 GetTotalPassiveIncome()
    {
        if (G.D.GameTime == _prevCallTimeGetTotalPassiveIncomeForFrame)
            throw new("May not be called twice per frame, it has side effects!");

        _prevCallTimeGetTotalPassiveIncomeForFrame = G.D.GameTime;

        float incomeFactorPerFrame =
            GameManager.Instance.GetIncomeFactorPerFrame();

        SaveGame.Members.TotalIncomeClickDamage += ClickDamageManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeKnifeDamage += KnifeDamageManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeGoldPerRound += ArenaGoldManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeKnifeCd += KnifeCdManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeWitchDoctor += WitchDoctorManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeGoldPerKnifeThrow += GoldPerKnifeThrowManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeWizard += WizardManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeHoarder += HoarderManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeZapDamage += ZapDamageManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeMoneyMaker += MoneyMakerManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeDaggerMaster += DaggerMasterManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeNecroNinja += NecroNinjaManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeSkullCrusher += SkullCrusherManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeChestMaster += ChestMasterManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeVoidgazer += VoidgazerManager.PassiveIncome() * incomeFactorPerFrame;

        Decimal512 fullSum = 0;
        fullSum += ClickDamageManager.PassiveIncome();
        fullSum += KnifeDamageManager.PassiveIncome();
        fullSum += ArenaGoldManager.PassiveIncome();
        fullSum += KnifeCdManager.PassiveIncome();
        fullSum += WitchDoctorManager.PassiveIncome();
        fullSum += GoldPerKnifeThrowManager.PassiveIncome();
        fullSum += WizardManager.PassiveIncome();
        fullSum += HoarderManager.PassiveIncome();
        fullSum += ZapDamageManager.PassiveIncome();
        fullSum += MoneyMakerManager.PassiveIncome();
        fullSum += DaggerMasterManager.PassiveIncome();
        fullSum += NecroNinjaManager.PassiveIncome();
        fullSum += SkullCrusherManager.PassiveIncome();
        fullSum += ChestMasterManager.PassiveIncome();
        fullSum += VoidgazerManager.PassiveIncome();
        return fullSum;
    }

    private void SetIsVisble(UpgradeItemScript upgradeItemScript)
    {
        var display = GetUpgradeDisplayStatus(upgradeItemScript);
        bool showGo = display is UpgradeDisplayStatus.NameOnly or UpgradeDisplayStatus.FullyShown;
        bool wasActive = upgradeItemScript.gameObject.activeSelf;

        upgradeItemScript.gameObject.SetActive(showGo);

        if (!wasActive && showGo)
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
        SetIsVisble(Wizard);
        SetIsVisble(Hoarder);
        SetIsVisble(ZapDamage);
        SetIsVisble(MoneyMaker);
        SetIsVisble(DaggerMaster);
        SetIsVisble(NecroNinja);
        SetIsVisble(SkullCrusher);
        SetIsVisble(ChestMaster);
        SetIsVisble(Voidgazer);

        ClickDamageManager.UpdateUi();
        KnifeDamageManager.UpdateUi();
        ArenaGoldManager.UpdateUi();
        KnifeCdManager.UpdateUi();
        WitchDoctorManager.UpdateUi();
        GoldPerKnifeThrowManager.UpdateUi();
        WizardManager.UpdateUi();
        HoarderManager.UpdateUi();
        ZapDamageManager.UpdateUi();
        MoneyMakerManager.UpdateUi();
        DaggerMasterManager.UpdateUi();
        NecroNinjaManager.UpdateUi();
        SkullCrusherManager.UpdateUi();
        ChestMasterManager.UpdateUi();
        VoidgazerManager.UpdateUi();
    }

    void OnItemBought()
    {
        SaveGame.Members.TotalUpgradesBought++;
        AudioManager.Instance.PlayClipForReal(AudioManager.Instance.AudioData.Menu);
        UpdateAllUpgrades();
    }

    void OnX2ItemBought()
    {
        SaveGame.Members.TotalX2UpgradesBought++;
        AudioManager.Instance.PlayClipForReal(AudioManager.Instance.AudioData.Menu);
        UpdateAllUpgrades();
    }

    public void OnBuyClickDamage()
    {
        ClickDamageManager.OnBuy();
        ClickDamage.SetPopupText();
        OnItemBought();
    }

    public void OnBuyClickDamageX2()
    {
        ClickDamageManager.OnBuyX2();
        ClickDamage.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyKnifeDamage()
    {
        KnifeDamageManager.OnBuy();
        KnifeDamage.SetPopupText();
        OnItemBought();
    }

    public void OnBuyKnifeDamageX2()
    {
        KnifeDamageManager.OnBuyX2();
        KnifeDamage.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyArenaGold()
    {
        ArenaGoldManager.OnBuy();
        GoldPerRound.SetPopupText();
        OnItemBought();
    }

    public void OnBuyArenaGoldX2()
    {
        ArenaGoldManager.OnBuyX2();
        GoldPerRound.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyKnifeCooldown()
    {
        KnifeCdManager.OnBuy();
        KnifeCd.SetPopupText();
        OnItemBought();
    }

    public void OnBuyKnifeCooldownX2()
    {
        KnifeCdManager.OnBuyX2();
        KnifeCd.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyWitchDoctorDamage()
    {
        WitchDoctorManager.OnBuy();
        WitchDoctor.SetPopupText();
        OnItemBought();
    }

    public void OnBuyWitchDoctorDamageX2()
    {
        WitchDoctorManager.OnBuyX2();
        WitchDoctor.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyGoldPerKnife()
    {
        GoldPerKnifeThrowManager.OnBuy();
        GoldPerKnife.SetPopupText();
        OnItemBought();
    }

    public void OnBuyGoldPerKnifeX2()
    {
        GoldPerKnifeThrowManager.OnBuyX2();
        GoldPerKnife.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyHoarder()
    {
        HoarderManager.OnBuy();
        Hoarder.SetPopupText();
        OnItemBought();
    }

    public void OnBuyHoarderX2()
    {
        HoarderManager.OnBuyX2();
        Hoarder.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyWizard()
    {
        WizardManager.OnBuy();
        Wizard.SetPopupText();
        OnItemBought();
    }

    public void OnBuyWizardX2()
    {
        WizardManager.OnBuyX2();
        Wizard.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyZapDamage()
    {
        ZapDamageManager.OnBuy();
        ZapDamage.SetPopupText();
        OnItemBought();
    }

    public void OnBuyZapDamageX2()
    {
        ZapDamageManager.OnBuyX2();
        ZapDamage.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyMoneyMaker()
    {
        MoneyMakerManager.OnBuy();
        MoneyMaker.SetPopupText();
        OnItemBought();
    }

    public void OnBuyMoneyMakerX2()
    {
        MoneyMakerManager.OnBuyX2();
        MoneyMaker.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyDaggerMaster()
    {
        DaggerMasterManager.OnBuy();
        DaggerMaster.SetPopupText();
        OnItemBought();
    }

    public void OnBuyDaggerMasterX2()
    {
        DaggerMasterManager.OnBuyX2();
        DaggerMaster.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyNecroNinja()
    {
        NecroNinjaManager.OnBuy();
        NecroNinja.SetPopupText();
        OnItemBought();
    }

    public void OnBuyNecroNinjaX2()
    {
        NecroNinjaManager.OnBuyX2();
        NecroNinja.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuySkullCrusher()
    {
        SkullCrusherManager.OnBuy();
        SkullCrusher.SetPopupText();
        OnItemBought();
    }

    public void OnBuySkullCrusherX2()
    {
        SkullCrusherManager.OnBuyX2();
        SkullCrusher.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyChestMaster()
    {
        ChestMasterManager.OnBuy();
        ChestMaster.SetPopupText();
        OnItemBought();
    }

    public void OnBuyChestMasterX2()
    {
        ChestMasterManager.OnBuyX2();
        ChestMaster.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyVoidgazer()
    {
        VoidgazerManager.OnBuy();
        Voidgazer.SetPopupText();
        OnItemBought();
    }

    public void OnBuyVoidgazerX2()
    {
        VoidgazerManager.OnBuyX2();
        Voidgazer.SetPopupText();
        OnX2ItemBought();
    }

    private void UpdatePlayerUpgrades()
    {
        ClickDamageManager.UpdatePlayerUpgrades();
        KnifeDamageManager.UpdatePlayerUpgrades();
        ArenaGoldManager.UpdatePlayerUpgrades();
        KnifeCdManager.UpdatePlayerUpgrades();
        WitchDoctorManager.UpdatePlayerUpgrades();
        GoldPerKnifeThrowManager.UpdatePlayerUpgrades();
        WizardManager.UpdatePlayerUpgrades();
        HoarderManager.UpdatePlayerUpgrades();
        ZapDamageManager.UpdatePlayerUpgrades();
        MoneyMakerManager.UpdatePlayerUpgrades();
        DaggerMasterManager.UpdatePlayerUpgrades();
        NecroNinjaManager.UpdatePlayerUpgrades();
        SkullCrusherManager.UpdatePlayerUpgrades();
        ChestMasterManager.UpdatePlayerUpgrades();
        VoidgazerManager.UpdatePlayerUpgrades();
    }

    void UpdateNumberOfX2Bought()
    {
        long total = 0;
        total += SaveGame.Members.LevelClickDamageX2;
        total += SaveGame.Members.LevelKnifeDamageX2;
        total += SaveGame.Members.LevelMoneyPerGoldX2;
        total += SaveGame.Members.LevelKnifeCdX2;
        total += SaveGame.Members.LevelWitchDoctorX2;
        total += SaveGame.Members.LevelGoldPerKnifeThrownX2;
        total += SaveGame.Members.LevelWizardX2;
        total += SaveGame.Members.LevelHoarderX2;
        total += SaveGame.Members.LevelZapDamageX2;
        total += SaveGame.Members.LevelMoneyMakerX2;
        total += SaveGame.Members.LevelDaggerMasterX2;
        total += SaveGame.Members.LevelNecroNinjaX2;
        total += SaveGame.Members.LevelSkullCrusherX2;
        total += SaveGame.Members.LevelChestMasterX2;
        total += SaveGame.Members.LevelVoidgazerX2;
        PlayerUpgrades.Data.NumberOfX2Bought = total;

        const float BonusPerRank = 0.1f;
        long bought = PlayerUpgrades.Data.NumberOfX2Bought;
        long bonuses = bought / 5;
        PlayerUpgrades.Data.PassiveIncomeX2Multiplier = BonusPerRank * bonuses;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdatePlayerUpgrades();
        UpdateNumberOfX2Bought();
        UpdateUpgradeUi();
    }
}