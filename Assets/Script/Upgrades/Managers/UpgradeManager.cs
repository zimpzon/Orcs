using Assets.Script.Upgrades;
using Assets.Script.Misc;
using Assets.Script.Upgrades.Managers;
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
    public UpgradeItemScript SmartDaggers;
    public UpgradeItemScript FastFeet;
    public UpgradeItemScript CryptMaster;
    public UpgradeItemScript SmartFireballs;

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
        SmartDaggersManager.UpdateAll();
        FastFeetManager.UpdateAll();
        CryptMasterManager.UpdateAll();
        SmartFireballsManager.UpdateAll();

        GameManager.Instance.TrySaveGame(forceSave: true);
    }

    private const string LockedDescription = "Buy one to see details.";

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
        else if (upgradeUiScript == SmartDaggers)
            return GetDisplayStatus(SaveGame.Members.LevelVoidgazer, SaveGame.Members.LevelSmartDaggers);
        else if (upgradeUiScript == FastFeet)
            return GetDisplayStatus(SaveGame.Members.LevelSmartDaggers, SaveGame.Members.LevelFastFeet);
        else if (upgradeUiScript == CryptMaster)
            return GetDisplayStatus(SaveGame.Members.LevelFastFeet, SaveGame.Members.LevelCryptMaster);
        else if (upgradeUiScript == SmartFireballs)
            return GetDisplayStatus(SaveGame.Members.LevelCryptMaster, SaveGame.Members.LevelSmartFireballs);
        else
            throw new NotImplementedException(upgradeUiScript.name);
    }

    private Decimal512 GetPriceForNext(UpgradeItemScript upgradeUiScript)
    {
        if (upgradeUiScript == ClickDamage)
            return ClickDamageManager.PriceForNext();
        else if (upgradeUiScript == KnifeDamage)
            return KnifeDamageManager.PriceForNext();
        else if (upgradeUiScript == GoldPerRound)
            return ArenaGoldManager.PriceForNext();
        else if (upgradeUiScript == KnifeCd)
            return KnifeCdManager.PriceForNext();
        else if (upgradeUiScript == WitchDoctor)
            return WitchDoctorManager.PriceForNext();
        else if (upgradeUiScript == GoldPerKnife)
            return GoldPerKnifeThrowManager.PriceForNext();
        else if (upgradeUiScript == Wizard)
            return WizardManager.PriceForNext();
        else if (upgradeUiScript == Hoarder)
            return HoarderManager.PriceForNext();
        else if (upgradeUiScript == ZapDamage)
            return ZapDamageManager.PriceForNext();
        else if (upgradeUiScript == MoneyMaker)
            return MoneyMakerManager.PriceForNext();
        else if (upgradeUiScript == DaggerMaster)
            return DaggerMasterManager.PriceForNext();
        else if (upgradeUiScript == NecroNinja)
            return NecroNinjaManager.PriceForNext();
        else if (upgradeUiScript == SkullCrusher)
            return SkullCrusherManager.PriceForNext();
        else if (upgradeUiScript == ChestMaster)
            return ChestMasterManager.PriceForNext();
        else if (upgradeUiScript == Voidgazer)
            return VoidgazerManager.PriceForNext();
        else if (upgradeUiScript == SmartDaggers)
            return SmartDaggersManager.PriceForNext();
        else if (upgradeUiScript == FastFeet)
            return FastFeetManager.PriceForNext();
        else if (upgradeUiScript == CryptMaster)
            return CryptMasterManager.PriceForNext();
        else if (upgradeUiScript == SmartFireballs)
            return SmartFireballsManager.PriceForNext();
        else
            throw new NotImplementedException(upgradeUiScript.name);
    }

    private string GetLockedText(UpgradeItemScript upgradeUiScript)
    {
        Decimal512 priceForNext = GetPriceForNext(upgradeUiScript);
        if (priceForNext <= 0)
            return LockedDescription;

        string priceSummary = UpgradeManagerHelper.FormatPrice(priceForNext);
        return $"{LockedDescription}\n{priceSummary}";
    }

    public string GetText(UpgradeItemScript upgradeUiScript)
    {
        string text = "";

        if (upgradeUiScript == ClickDamage)
            text = GetUpgradeDisplayStatus(ClickDamage) == UpgradeDisplayStatus.FullyShown ? ClickDamageManager.GetText() : GetLockedText(ClickDamage);
        else if (upgradeUiScript == KnifeDamage)
            text = GetUpgradeDisplayStatus(KnifeDamage) == UpgradeDisplayStatus.FullyShown ? KnifeDamageManager.GetText() : GetLockedText(KnifeDamage);
        else if (upgradeUiScript == GoldPerRound)
            text = GetUpgradeDisplayStatus(GoldPerRound) == UpgradeDisplayStatus.FullyShown ? ArenaGoldManager.GetText() : GetLockedText(GoldPerRound);
        else if (upgradeUiScript == KnifeCd)
            text = GetUpgradeDisplayStatus(KnifeCd) == UpgradeDisplayStatus.FullyShown ? KnifeCdManager.GetText() : GetLockedText(KnifeCd);
        else if (upgradeUiScript == WitchDoctor)
            text = GetUpgradeDisplayStatus(WitchDoctor) == UpgradeDisplayStatus.FullyShown ? WitchDoctorManager.GetText() : GetLockedText(WitchDoctor);
        else if (upgradeUiScript == GoldPerKnife)
            text = GetUpgradeDisplayStatus(GoldPerKnife) == UpgradeDisplayStatus.FullyShown ? GoldPerKnifeThrowManager.GetText() : GetLockedText(GoldPerKnife);
        else if (upgradeUiScript == Wizard)
            text = GetUpgradeDisplayStatus(Wizard) == UpgradeDisplayStatus.FullyShown ? WizardManager.GetText() : GetLockedText(Wizard);
        else if (upgradeUiScript == Hoarder)
            text = GetUpgradeDisplayStatus(Hoarder) == UpgradeDisplayStatus.FullyShown ? HoarderManager.GetText() : GetLockedText(Hoarder);
        else if (upgradeUiScript == ZapDamage)
            text = GetUpgradeDisplayStatus(ZapDamage) == UpgradeDisplayStatus.FullyShown ? ZapDamageManager.GetText() : GetLockedText(ZapDamage);
        else if (upgradeUiScript == MoneyMaker)
            text = GetUpgradeDisplayStatus(MoneyMaker) == UpgradeDisplayStatus.FullyShown ? MoneyMakerManager.GetText() : GetLockedText(MoneyMaker);
        else if (upgradeUiScript == DaggerMaster)
            text = GetUpgradeDisplayStatus(DaggerMaster) == UpgradeDisplayStatus.FullyShown ? DaggerMasterManager.GetText() : GetLockedText(DaggerMaster);
        else if (upgradeUiScript == NecroNinja)
            text = GetUpgradeDisplayStatus(NecroNinja) == UpgradeDisplayStatus.FullyShown ? NecroNinjaManager.GetText() : GetLockedText(NecroNinja);
        else if (upgradeUiScript == SkullCrusher)
            text = GetUpgradeDisplayStatus(SkullCrusher) == UpgradeDisplayStatus.FullyShown ? SkullCrusherManager.GetText() : GetLockedText(SkullCrusher);
        else if (upgradeUiScript == ChestMaster)
            text = GetUpgradeDisplayStatus(ChestMaster) == UpgradeDisplayStatus.FullyShown ? ChestMasterManager.GetText() : GetLockedText(ChestMaster);
        else if (upgradeUiScript == Voidgazer)
            text = GetUpgradeDisplayStatus(Voidgazer) == UpgradeDisplayStatus.FullyShown ? VoidgazerManager.GetText() : GetLockedText(Voidgazer);
        else if (upgradeUiScript == SmartDaggers)
            text = GetUpgradeDisplayStatus(SmartDaggers) == UpgradeDisplayStatus.FullyShown ? SmartDaggersManager.GetText() : GetLockedText(SmartDaggers);
        else if (upgradeUiScript == FastFeet)
            text = GetUpgradeDisplayStatus(FastFeet) == UpgradeDisplayStatus.FullyShown ? FastFeetManager.GetText() : GetLockedText(FastFeet);
        else if (upgradeUiScript == CryptMaster)
            text = GetUpgradeDisplayStatus(CryptMaster) == UpgradeDisplayStatus.FullyShown ? CryptMasterManager.GetText() : GetLockedText(CryptMaster);
        else if (upgradeUiScript == SmartFireballs)
            text = GetUpgradeDisplayStatus(SmartFireballs) == UpgradeDisplayStatus.FullyShown ? SmartFireballsManager.GetText() : GetLockedText(SmartFireballs);
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
        SaveGame.Members.TotalIncomeSmartDaggers += SmartDaggersManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeFastFeet += FastFeetManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeCryptMaster += CryptMasterManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeSmartFireballs += SmartFireballsManager.PassiveIncome() * incomeFactorPerFrame;

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
        fullSum += SmartDaggersManager.PassiveIncome();
        fullSum += FastFeetManager.PassiveIncome();
        fullSum += CryptMasterManager.PassiveIncome();
        fullSum += SmartFireballsManager.PassiveIncome();
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
        SetIsVisble(SmartDaggers);
        SetIsVisble(FastFeet);
        SetIsVisble(CryptMaster);
        SetIsVisble(SmartFireballs);

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
        SmartDaggersManager.UpdateUi();
        FastFeetManager.UpdateUi();
        CryptMasterManager.UpdateUi();
        SmartFireballsManager.UpdateUi();
    }

    void OnItemBought(long actualBuyAmount)
    {
        SaveGame.Members.TotalUpgradesBought += actualBuyAmount;
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
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelClickDamage, SaveGame.Members.LevelClickDamageX2);
        ClickDamageManager.OnBuy();
        ClickDamage.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyClickDamageX2()
    {
        ClickDamageManager.OnBuyX2();
        ClickDamage.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyKnifeDamage()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelKnifeDamage, SaveGame.Members.LevelKnifeDamageX2);
        KnifeDamageManager.OnBuy();
        KnifeDamage.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyKnifeDamageX2()
    {
        KnifeDamageManager.OnBuyX2();
        KnifeDamage.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyArenaGold()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelMoneyPerGold, SaveGame.Members.LevelMoneyPerGoldX2);
        ArenaGoldManager.OnBuy();
        GoldPerRound.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyArenaGoldX2()
    {
        ArenaGoldManager.OnBuyX2();
        GoldPerRound.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyKnifeCooldown()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelKnifeCd, SaveGame.Members.LevelKnifeCdX2);
        KnifeCdManager.OnBuy();
        KnifeCd.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyKnifeCooldownX2()
    {
        KnifeCdManager.OnBuyX2();
        KnifeCd.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyWitchDoctorDamage()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelWitchDoctor, SaveGame.Members.LevelWitchDoctorX2);
        WitchDoctorManager.OnBuy();
        WitchDoctor.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyWitchDoctorDamageX2()
    {
        WitchDoctorManager.OnBuyX2();
        WitchDoctor.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyGoldPerKnife()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelGoldPerKnifeThrown, SaveGame.Members.LevelGoldPerKnifeThrownX2);
        GoldPerKnifeThrowManager.OnBuy();
        GoldPerKnife.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyGoldPerKnifeX2()
    {
        GoldPerKnifeThrowManager.OnBuyX2();
        GoldPerKnife.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyHoarder()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelHoarder, SaveGame.Members.LevelHoarderX2);
        HoarderManager.OnBuy();
        Hoarder.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyHoarderX2()
    {
        HoarderManager.OnBuyX2();
        Hoarder.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyWizard()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelWizard, SaveGame.Members.LevelWizardX2);
        WizardManager.OnBuy();
        Wizard.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyWizardX2()
    {
        WizardManager.OnBuyX2();
        Wizard.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyZapDamage()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelZapDamage, SaveGame.Members.LevelZapDamageX2);
        ZapDamageManager.OnBuy();
        ZapDamage.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyZapDamageX2()
    {
        ZapDamageManager.OnBuyX2();
        ZapDamage.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyMoneyMaker()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelMoneyMaker, SaveGame.Members.LevelMoneyMakerX2);
        MoneyMakerManager.OnBuy();
        MoneyMaker.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyMoneyMakerX2()
    {
        MoneyMakerManager.OnBuyX2();
        MoneyMaker.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyDaggerMaster()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelDaggerMaster, SaveGame.Members.LevelDaggerMasterX2);
        DaggerMasterManager.OnBuy();
        DaggerMaster.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyDaggerMasterX2()
    {
        DaggerMasterManager.OnBuyX2();
        DaggerMaster.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyNecroNinja()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelNecroNinja, SaveGame.Members.LevelNecroNinjaX2);
        NecroNinjaManager.OnBuy();
        NecroNinja.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyNecroNinjaX2()
    {
        NecroNinjaManager.OnBuyX2();
        NecroNinja.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuySkullCrusher()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelSkullCrusher, SaveGame.Members.LevelSkullCrusherX2);
        SkullCrusherManager.OnBuy();
        SkullCrusher.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuySkullCrusherX2()
    {
        SkullCrusherManager.OnBuyX2();
        SkullCrusher.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyChestMaster()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelChestMaster, SaveGame.Members.LevelChestMasterX2);
        ChestMasterManager.OnBuy();
        ChestMaster.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyChestMasterX2()
    {
        ChestMasterManager.OnBuyX2();
        ChestMaster.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyVoidgazer()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelVoidgazer, SaveGame.Members.LevelVoidgazerX2);
        VoidgazerManager.OnBuy();
        Voidgazer.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyVoidgazerX2()
    {
        VoidgazerManager.OnBuyX2();
        Voidgazer.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuySmartDaggers()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelSmartDaggers, SaveGame.Members.LevelSmartDaggersX2);
        SmartDaggersManager.OnBuy();
        SmartDaggers.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuySmartDaggersX2()
    {
        SmartDaggersManager.OnBuyX2();
        SmartDaggers.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyFastFeet()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelFastFeet, SaveGame.Members.LevelFastFeetX2);
        FastFeetManager.OnBuy();
        FastFeet.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyFastFeetX2()
    {
        FastFeetManager.OnBuyX2();
        FastFeet.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuyCryptMaster()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelCryptMaster, SaveGame.Members.LevelCryptMasterX2);
        CryptMasterManager.OnBuy();
        CryptMaster.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuyCryptMasterX2()
    {
        CryptMasterManager.OnBuyX2();
        CryptMaster.SetPopupText();
        OnX2ItemBought();
    }

    public void OnBuySmartFireballs()
    {
        long buyAmount = UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelSmartFireballs, SaveGame.Members.LevelSmartFireballsX2);
        SmartFireballsManager.OnBuy();
        SmartFireballs.SetPopupText();
        OnItemBought(buyAmount);
    }

    public void OnBuySmartFireballsX2()
    {
        SmartFireballsManager.OnBuyX2();
        SmartFireballs.SetPopupText();
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
        SmartDaggersManager.UpdatePlayerUpgrades();
        FastFeetManager.UpdatePlayerUpgrades();
        CryptMasterManager.UpdatePlayerUpgrades();
        SmartFireballsManager.UpdatePlayerUpgrades();
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
        total += SaveGame.Members.LevelSmartDaggersX2;
        total += SaveGame.Members.LevelFastFeetX2;
        total += SaveGame.Members.LevelCryptMasterX2;
        total += SaveGame.Members.LevelSmartFireballsX2;
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
