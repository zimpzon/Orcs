using Assets.Script.Upgrades;
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
    public UpgradeItemScript BeefyEarl;
    public UpgradeItemScript CriticalStrike;
    public UpgradeItemScript PowerZap;
    public UpgradeItemScript SkullSlicer;
    public UpgradeItemScript StormLord;

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
        BeefyEarlManager.UpdateAll();
        CriticalStrikeManager.UpdateAll();
        PowerZapManager.UpdateAll();
        SkullSlicerManager.UpdateAll();
        StormLordManager.UpdateAll();

        GameManager.Instance.InvalidateIncome();
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
        else if (upgradeUiScript == BeefyEarl)
            return GetDisplayStatus(SaveGame.Members.LevelSmartFireballs, SaveGame.Members.LevelBeefyEarl);
        else if (upgradeUiScript == CriticalStrike)
            return GetDisplayStatus(SaveGame.Members.LevelBeefyEarl, SaveGame.Members.LevelCriticalStrike);
        else if (PowerZap != null && upgradeUiScript == PowerZap)
            return GetDisplayStatus(SaveGame.Members.LevelCriticalStrike, SaveGame.Members.LevelPowerZap);
        else if (SkullSlicer != null && upgradeUiScript == SkullSlicer)
            return GetDisplayStatus(SaveGame.Members.LevelPowerZap, SaveGame.Members.LevelSkullSlicer);
        else if (StormLord != null && upgradeUiScript == StormLord)
            return GetDisplayStatus(SaveGame.Members.LevelSkullSlicer, SaveGame.Members.LevelStormLord);
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
        else if (upgradeUiScript == BeefyEarl)
            return BeefyEarlManager.PriceForNext();
        else if (upgradeUiScript == CriticalStrike)
            return CriticalStrikeManager.PriceForNext();
        else if (PowerZap != null && upgradeUiScript == PowerZap)
            return PowerZapManager.PriceForNext();
        else if (SkullSlicer != null && upgradeUiScript == SkullSlicer)
            return SkullSlicerManager.PriceForNext();
        else if (StormLord != null && upgradeUiScript == StormLord)
            return StormLordManager.PriceForNext();
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
        else if (upgradeUiScript == BeefyEarl)
            text = GetUpgradeDisplayStatus(BeefyEarl) == UpgradeDisplayStatus.FullyShown ? BeefyEarlManager.GetText() : GetLockedText(BeefyEarl);
        else if (upgradeUiScript == CriticalStrike)
            text = GetUpgradeDisplayStatus(CriticalStrike) == UpgradeDisplayStatus.FullyShown ? CriticalStrikeManager.GetText() : GetLockedText(CriticalStrike);
        else if (PowerZap != null && upgradeUiScript == PowerZap)
            text = GetUpgradeDisplayStatus(PowerZap) == UpgradeDisplayStatus.FullyShown ? PowerZapManager.GetText() : GetLockedText(PowerZap);
        else if (SkullSlicer != null && upgradeUiScript == SkullSlicer)
            text = GetUpgradeDisplayStatus(SkullSlicer) == UpgradeDisplayStatus.FullyShown ? SkullSlicerManager.GetText() : GetLockedText(SkullSlicer);
        else if (StormLord != null && upgradeUiScript == StormLord)
            text = GetUpgradeDisplayStatus(StormLord) == UpgradeDisplayStatus.FullyShown ? StormLordManager.GetText() : GetLockedText(StormLord);
        else
            text = $"unknown UpgradeItemScript: {upgradeUiScript.name}";

        text = text.Replace("COLOR-ARENA", TextColors.GreenSmall);

        return text;
    }

    // Sum of all tiers' income per second. Also adds each tier's income * statSeconds to its "earned so far" stat
    // (GameManager calls this a few times per second, statSeconds = real time since the previous call).
    public Decimal512 GetTotalPassiveIncome(float statSeconds)
    {
        Decimal512 fullSum = 0;
        Decimal512 income;
        income = ClickDamageManager.PassiveIncome(); SaveGame.Members.TotalIncomeClickDamage += income * statSeconds; fullSum += income;
        income = KnifeDamageManager.PassiveIncome(); SaveGame.Members.TotalIncomeKnifeDamage += income * statSeconds; fullSum += income;
        income = ArenaGoldManager.PassiveIncome(); SaveGame.Members.TotalIncomeGoldPerRound += income * statSeconds; fullSum += income;
        income = KnifeCdManager.PassiveIncome(); SaveGame.Members.TotalIncomeKnifeCd += income * statSeconds; fullSum += income;
        income = WitchDoctorManager.PassiveIncome(); SaveGame.Members.TotalIncomeWitchDoctor += income * statSeconds; fullSum += income;
        income = GoldPerKnifeThrowManager.PassiveIncome(); SaveGame.Members.TotalIncomeGoldPerKnifeThrow += income * statSeconds; fullSum += income;
        income = WizardManager.PassiveIncome(); SaveGame.Members.TotalIncomeWizard += income * statSeconds; fullSum += income;
        income = HoarderManager.PassiveIncome(); SaveGame.Members.TotalIncomeHoarder += income * statSeconds; fullSum += income;
        income = ZapDamageManager.PassiveIncome(); SaveGame.Members.TotalIncomeZapDamage += income * statSeconds; fullSum += income;
        income = MoneyMakerManager.PassiveIncome(); SaveGame.Members.TotalIncomeMoneyMaker += income * statSeconds; fullSum += income;
        income = DaggerMasterManager.PassiveIncome(); SaveGame.Members.TotalIncomeDaggerMaster += income * statSeconds; fullSum += income;
        income = NecroNinjaManager.PassiveIncome(); SaveGame.Members.TotalIncomeNecroNinja += income * statSeconds; fullSum += income;
        income = SkullCrusherManager.PassiveIncome(); SaveGame.Members.TotalIncomeSkullCrusher += income * statSeconds; fullSum += income;
        income = ChestMasterManager.PassiveIncome(); SaveGame.Members.TotalIncomeChestMaster += income * statSeconds; fullSum += income;
        income = VoidgazerManager.PassiveIncome(); SaveGame.Members.TotalIncomeVoidgazer += income * statSeconds; fullSum += income;
        income = SmartDaggersManager.PassiveIncome(); SaveGame.Members.TotalIncomeSmartDaggers += income * statSeconds; fullSum += income;
        income = FastFeetManager.PassiveIncome(); SaveGame.Members.TotalIncomeFastFeet += income * statSeconds; fullSum += income;
        income = CryptMasterManager.PassiveIncome(); SaveGame.Members.TotalIncomeCryptMaster += income * statSeconds; fullSum += income;
        income = SmartFireballsManager.PassiveIncome(); SaveGame.Members.TotalIncomeSmartFireballs += income * statSeconds; fullSum += income;
        income = BeefyEarlManager.PassiveIncome(); SaveGame.Members.TotalIncomeBeefyEarl += income * statSeconds; fullSum += income;
        income = CriticalStrikeManager.PassiveIncome(); SaveGame.Members.TotalIncomeCriticalStrike += income * statSeconds; fullSum += income;
        income = PowerZapManager.PassiveIncome(); SaveGame.Members.TotalIncomePowerZap += income * statSeconds; fullSum += income;
        income = SkullSlicerManager.PassiveIncome(); SaveGame.Members.TotalIncomeSkullSlicer += income * statSeconds; fullSum += income;
        income = StormLordManager.PassiveIncome(); SaveGame.Members.TotalIncomeStormLord += income * statSeconds; fullSum += income;
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
        SetIsVisble(BeefyEarl);
        SetIsVisble(CriticalStrike);
        if (PowerZap != null)
            SetIsVisble(PowerZap);
        if (SkullSlicer != null)
            SetIsVisble(SkullSlicer);
        if (StormLord != null)
            SetIsVisble(StormLord);

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
        BeefyEarlManager.UpdateUi();
        CriticalStrikeManager.UpdateUi();
        PowerZapManager.UpdateUi();
        SkullSlicerManager.UpdateUi();
        StormLordManager.UpdateUi();
    }

    void OnItemBought(long actualBuyAmount)
    {
        SaveGame.Members.TotalUpgradesBought += actualBuyAmount;
        AudioManager.Instance.PlayMenuClick();
        UpdateAllUpgrades();
    }

    void OnX2ItemBought(long count)
    {
        SaveGame.Members.TotalX2UpgradesBought += count;
        AudioManager.Instance.PlayMenuClick();
        // Consumed by the next rank check (UpdateNumberOfX2Bought, runs in Update - after this click).
        _x2BoughtByClick = count > 0;
        UpdateAllUpgrades();
    }

    // Holding Shift while clicking buys as many as affordable (levels and X2), instead of the selected amount / one.
    public static bool BuyMaxHeld => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    const int MaxBuyIterations = 10_000;

    // Buys the selected amount once; with Shift held, single levels until the next one is unaffordable. Returns the
    // number of levels actually bought.
    long BuyLevels(Action buyOnce, Func<long> level)
    {
        long bought = BuyLevelsInner(buyOnce, level);
        // Small "+N" at the cursor, so the selected buy amount (1/10/50/100, or the Shift max) is easy to see.
        if (bought > 0 && ClickDamage != null)
            X2RankUpEffect.SpawnSmall(ClickDamage, Input.mousePosition, $"+{bought}");
        return bought;
    }

    long BuyLevelsInner(Action buyOnce, Func<long> level)
    {
        long start = level();
        if (!BuyMaxHeld)
        {
            buyOnce();
            return level() - start;
        }

        var selection = GameManager.Instance.SelectedBuyAmount;
        GameManager.Instance.SelectedBuyAmount = GameManager.BuyAmountSelection.Buy1;
        try
        {
            for (int i = 0; i < MaxBuyIterations; ++i)
            {
                long before = level();
                buyOnce();
                if (level() == before)
                    break;
            }
        }
        finally
        {
            GameManager.Instance.SelectedBuyAmount = selection;
        }
        return level() - start;
    }

    // Buys one X2; with Shift held, keeps going while the next X2 is level-unlocked and affordable (the manager's
    // OnBuyX2 checks the price). Returns the number of X2s actually bought.
    // Holding Ctrl while clicking an X2 button buys every affordable X2 on all tiers, cheapest first.
    public static bool BuyAllX2Held => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

    // Every tier's X2: buy action, level, X2 count and initial X2 price (same order as the shop).
    static readonly (Action Buy, Func<long> Level, Func<long> X2, Func<Decimal512> InitialPrice)[] AllX2 =
    {
        (ClickDamageManager.OnBuyX2, () => SaveGame.Members.LevelClickDamage, () => SaveGame.Members.LevelClickDamageX2, () => UpgradeProgression.InitialPrice_Clickdamage_X2),
        (KnifeDamageManager.OnBuyX2, () => SaveGame.Members.LevelKnifeDamage, () => SaveGame.Members.LevelKnifeDamageX2, () => UpgradeProgression.InitialPrice_DaggerDamage_X2),
        (ArenaGoldManager.OnBuyX2, () => SaveGame.Members.LevelMoneyPerGold, () => SaveGame.Members.LevelMoneyPerGoldX2, () => UpgradeProgression.InitialPrice_GoldValue_X2),
        (KnifeCdManager.OnBuyX2, () => SaveGame.Members.LevelKnifeCd, () => SaveGame.Members.LevelKnifeCdX2, () => UpgradeProgression.InitialPrice_DaggerCd_X2),
        (WitchDoctorManager.OnBuyX2, () => SaveGame.Members.LevelWitchDoctor, () => SaveGame.Members.LevelWitchDoctorX2, () => UpgradeProgression.InitialPrice_WitchDoctor_X2),
        (GoldPerKnifeThrowManager.OnBuyX2, () => SaveGame.Members.LevelGoldPerKnifeThrown, () => SaveGame.Members.LevelGoldPerKnifeThrownX2, () => UpgradeProgression.InitialPrice_GoldPerKnifeThrown_X2),
        (HoarderManager.OnBuyX2, () => SaveGame.Members.LevelHoarder, () => SaveGame.Members.LevelHoarderX2, () => UpgradeProgression.InitialPrice_Hoarder_X2),
        (WizardManager.OnBuyX2, () => SaveGame.Members.LevelWizard, () => SaveGame.Members.LevelWizardX2, () => UpgradeProgression.InitialPrice_Wizard_X2),
        (ZapDamageManager.OnBuyX2, () => SaveGame.Members.LevelZapDamage, () => SaveGame.Members.LevelZapDamageX2, () => UpgradeProgression.InitialPrice_ZapDamage_X2),
        (MoneyMakerManager.OnBuyX2, () => SaveGame.Members.LevelMoneyMaker, () => SaveGame.Members.LevelMoneyMakerX2, () => UpgradeProgression.InitialPrice_MoneyMaker_X2),
        (DaggerMasterManager.OnBuyX2, () => SaveGame.Members.LevelDaggerMaster, () => SaveGame.Members.LevelDaggerMasterX2, () => UpgradeProgression.InitialPrice_DaggerMaster_X2),
        (NecroNinjaManager.OnBuyX2, () => SaveGame.Members.LevelNecroNinja, () => SaveGame.Members.LevelNecroNinjaX2, () => UpgradeProgression.InitialPrice_NecroNinja_X2),
        (SkullCrusherManager.OnBuyX2, () => SaveGame.Members.LevelSkullCrusher, () => SaveGame.Members.LevelSkullCrusherX2, () => UpgradeProgression.InitialPrice_SkullCrusher_X2),
        (ChestMasterManager.OnBuyX2, () => SaveGame.Members.LevelChestMaster, () => SaveGame.Members.LevelChestMasterX2, () => UpgradeProgression.InitialPrice_ChestMaster_X2),
        (VoidgazerManager.OnBuyX2, () => SaveGame.Members.LevelVoidgazer, () => SaveGame.Members.LevelVoidgazerX2, () => UpgradeProgression.InitialPrice_Voidgazer_X2),
        (SmartDaggersManager.OnBuyX2, () => SaveGame.Members.LevelSmartDaggers, () => SaveGame.Members.LevelSmartDaggersX2, () => UpgradeProgression.InitialPrice_SmartDaggers_X2),
        (FastFeetManager.OnBuyX2, () => SaveGame.Members.LevelFastFeet, () => SaveGame.Members.LevelFastFeetX2, () => UpgradeProgression.InitialPrice_FastFeet_X2),
        (CryptMasterManager.OnBuyX2, () => SaveGame.Members.LevelCryptMaster, () => SaveGame.Members.LevelCryptMasterX2, () => UpgradeProgression.InitialPrice_CryptMaster_X2),
        (SmartFireballsManager.OnBuyX2, () => SaveGame.Members.LevelSmartFireballs, () => SaveGame.Members.LevelSmartFireballsX2, () => UpgradeProgression.InitialPrice_SmartFireballs_X2),
        (BeefyEarlManager.OnBuyX2, () => SaveGame.Members.LevelBeefyEarl, () => SaveGame.Members.LevelBeefyEarlX2, () => UpgradeProgression.InitialPrice_BeefyEarl_X2),
        (CriticalStrikeManager.OnBuyX2, () => SaveGame.Members.LevelCriticalStrike, () => SaveGame.Members.LevelCriticalStrikeX2, () => UpgradeProgression.InitialPrice_CriticalStrike_X2),
        (PowerZapManager.OnBuyX2, () => SaveGame.Members.LevelPowerZap, () => SaveGame.Members.LevelPowerZapX2, () => UpgradeProgression.InitialPrice_PowerZap_X2),
        (SkullSlicerManager.OnBuyX2, () => SaveGame.Members.LevelSkullSlicer, () => SaveGame.Members.LevelSkullSlicerX2, () => UpgradeProgression.InitialPrice_SkullSlicer_X2),
        (StormLordManager.OnBuyX2, () => SaveGame.Members.LevelStormLord, () => SaveGame.Members.LevelStormLordX2, () => UpgradeProgression.InitialPrice_StormLord_X2),
    };

    // Buys the cheapest affordable, level-unlocked X2 on any tier, repeatedly. Returns the number bought.
    long BuyAllX2()
    {
        long bought = 0;
        for (int i = 0; i < MaxBuyIterations; ++i)
        {
            int best = -1;
            Decimal512 bestPrice = 0;
            for (int t = 0; t < AllX2.Length; ++t)
            {
                var tier = AllX2[t];
                long next = tier.X2() + 1;
                if (tier.Level() < 1 || tier.Level() < UpgradeProgression.LevelRequirementX2(next))
                    continue;
                Decimal512 price = UpgradeProgression.PriceX2(tier.InitialPrice(), next);
                if (price > SaveGame.Members.Money)
                    continue;
                if (best < 0 || price < bestPrice)
                {
                    best = t;
                    bestPrice = price;
                }
            }
            if (best < 0)
                break;

            long before = AllX2[best].X2();
            AllX2[best].Buy();
            if (AllX2[best].X2() == before)
                break;
            bought++;
        }
        return bought;
    }

    long BuyX2(Action buyOnce, Func<long> level, Func<long> x2)
    {
        if (BuyAllX2Held)
            return BuyAllX2();

        long start = x2();
        for (int i = 0; i < MaxBuyIterations; ++i)
        {
            long before = x2();
            buyOnce();
            if (x2() == before || !BuyMaxHeld)
                break;
            if (level() < UpgradeProgression.LevelRequirementX2(x2() + 1))
                break;
        }
        return x2() - start;
    }

    public void OnBuyClickDamage()
    {
        long bought = BuyLevels(ClickDamageManager.OnBuy, () => SaveGame.Members.LevelClickDamage);
        ClickDamage.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyClickDamageX2()
    {
        long bought = BuyX2(ClickDamageManager.OnBuyX2, () => SaveGame.Members.LevelClickDamage, () => SaveGame.Members.LevelClickDamageX2);
        ClickDamage.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyKnifeDamage()
    {
        long bought = BuyLevels(KnifeDamageManager.OnBuy, () => SaveGame.Members.LevelKnifeDamage);
        KnifeDamage.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyKnifeDamageX2()
    {
        long bought = BuyX2(KnifeDamageManager.OnBuyX2, () => SaveGame.Members.LevelKnifeDamage, () => SaveGame.Members.LevelKnifeDamageX2);
        KnifeDamage.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyArenaGold()
    {
        long bought = BuyLevels(ArenaGoldManager.OnBuy, () => SaveGame.Members.LevelMoneyPerGold);
        GoldPerRound.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyArenaGoldX2()
    {
        long bought = BuyX2(ArenaGoldManager.OnBuyX2, () => SaveGame.Members.LevelMoneyPerGold, () => SaveGame.Members.LevelMoneyPerGoldX2);
        GoldPerRound.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyKnifeCooldown()
    {
        long bought = BuyLevels(KnifeCdManager.OnBuy, () => SaveGame.Members.LevelKnifeCd);
        KnifeCd.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyKnifeCooldownX2()
    {
        long bought = BuyX2(KnifeCdManager.OnBuyX2, () => SaveGame.Members.LevelKnifeCd, () => SaveGame.Members.LevelKnifeCdX2);
        KnifeCd.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyWitchDoctorDamage()
    {
        long bought = BuyLevels(WitchDoctorManager.OnBuy, () => SaveGame.Members.LevelWitchDoctor);
        WitchDoctor.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyWitchDoctorDamageX2()
    {
        long bought = BuyX2(WitchDoctorManager.OnBuyX2, () => SaveGame.Members.LevelWitchDoctor, () => SaveGame.Members.LevelWitchDoctorX2);
        WitchDoctor.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyGoldPerKnife()
    {
        long bought = BuyLevels(GoldPerKnifeThrowManager.OnBuy, () => SaveGame.Members.LevelGoldPerKnifeThrown);
        GoldPerKnife.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyGoldPerKnifeX2()
    {
        long bought = BuyX2(GoldPerKnifeThrowManager.OnBuyX2, () => SaveGame.Members.LevelGoldPerKnifeThrown, () => SaveGame.Members.LevelGoldPerKnifeThrownX2);
        GoldPerKnife.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyHoarder()
    {
        long bought = BuyLevels(HoarderManager.OnBuy, () => SaveGame.Members.LevelHoarder);
        Hoarder.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyHoarderX2()
    {
        long bought = BuyX2(HoarderManager.OnBuyX2, () => SaveGame.Members.LevelHoarder, () => SaveGame.Members.LevelHoarderX2);
        Hoarder.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyWizard()
    {
        long bought = BuyLevels(WizardManager.OnBuy, () => SaveGame.Members.LevelWizard);
        Wizard.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyWizardX2()
    {
        long bought = BuyX2(WizardManager.OnBuyX2, () => SaveGame.Members.LevelWizard, () => SaveGame.Members.LevelWizardX2);
        Wizard.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyZapDamage()
    {
        long bought = BuyLevels(ZapDamageManager.OnBuy, () => SaveGame.Members.LevelZapDamage);
        ZapDamage.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyZapDamageX2()
    {
        long bought = BuyX2(ZapDamageManager.OnBuyX2, () => SaveGame.Members.LevelZapDamage, () => SaveGame.Members.LevelZapDamageX2);
        ZapDamage.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyMoneyMaker()
    {
        long bought = BuyLevels(MoneyMakerManager.OnBuy, () => SaveGame.Members.LevelMoneyMaker);
        MoneyMaker.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyMoneyMakerX2()
    {
        long bought = BuyX2(MoneyMakerManager.OnBuyX2, () => SaveGame.Members.LevelMoneyMaker, () => SaveGame.Members.LevelMoneyMakerX2);
        MoneyMaker.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyDaggerMaster()
    {
        long bought = BuyLevels(DaggerMasterManager.OnBuy, () => SaveGame.Members.LevelDaggerMaster);
        DaggerMaster.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyDaggerMasterX2()
    {
        long bought = BuyX2(DaggerMasterManager.OnBuyX2, () => SaveGame.Members.LevelDaggerMaster, () => SaveGame.Members.LevelDaggerMasterX2);
        DaggerMaster.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyNecroNinja()
    {
        long bought = BuyLevels(NecroNinjaManager.OnBuy, () => SaveGame.Members.LevelNecroNinja);
        NecroNinja.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyNecroNinjaX2()
    {
        long bought = BuyX2(NecroNinjaManager.OnBuyX2, () => SaveGame.Members.LevelNecroNinja, () => SaveGame.Members.LevelNecroNinjaX2);
        NecroNinja.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuySkullCrusher()
    {
        long bought = BuyLevels(SkullCrusherManager.OnBuy, () => SaveGame.Members.LevelSkullCrusher);
        SkullCrusher.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuySkullCrusherX2()
    {
        long bought = BuyX2(SkullCrusherManager.OnBuyX2, () => SaveGame.Members.LevelSkullCrusher, () => SaveGame.Members.LevelSkullCrusherX2);
        SkullCrusher.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyChestMaster()
    {
        long bought = BuyLevels(ChestMasterManager.OnBuy, () => SaveGame.Members.LevelChestMaster);
        ChestMaster.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyChestMasterX2()
    {
        long bought = BuyX2(ChestMasterManager.OnBuyX2, () => SaveGame.Members.LevelChestMaster, () => SaveGame.Members.LevelChestMasterX2);
        ChestMaster.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyVoidgazer()
    {
        long bought = BuyLevels(VoidgazerManager.OnBuy, () => SaveGame.Members.LevelVoidgazer);
        Voidgazer.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyVoidgazerX2()
    {
        long bought = BuyX2(VoidgazerManager.OnBuyX2, () => SaveGame.Members.LevelVoidgazer, () => SaveGame.Members.LevelVoidgazerX2);
        Voidgazer.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuySmartDaggers()
    {
        long bought = BuyLevels(SmartDaggersManager.OnBuy, () => SaveGame.Members.LevelSmartDaggers);
        SmartDaggers.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuySmartDaggersX2()
    {
        long bought = BuyX2(SmartDaggersManager.OnBuyX2, () => SaveGame.Members.LevelSmartDaggers, () => SaveGame.Members.LevelSmartDaggersX2);
        SmartDaggers.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyFastFeet()
    {
        long bought = BuyLevels(FastFeetManager.OnBuy, () => SaveGame.Members.LevelFastFeet);
        FastFeet.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyFastFeetX2()
    {
        long bought = BuyX2(FastFeetManager.OnBuyX2, () => SaveGame.Members.LevelFastFeet, () => SaveGame.Members.LevelFastFeetX2);
        FastFeet.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyCryptMaster()
    {
        long bought = BuyLevels(CryptMasterManager.OnBuy, () => SaveGame.Members.LevelCryptMaster);
        CryptMaster.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyCryptMasterX2()
    {
        long bought = BuyX2(CryptMasterManager.OnBuyX2, () => SaveGame.Members.LevelCryptMaster, () => SaveGame.Members.LevelCryptMasterX2);
        CryptMaster.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuySmartFireballs()
    {
        long bought = BuyLevels(SmartFireballsManager.OnBuy, () => SaveGame.Members.LevelSmartFireballs);
        SmartFireballs.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuySmartFireballsX2()
    {
        long bought = BuyX2(SmartFireballsManager.OnBuyX2, () => SaveGame.Members.LevelSmartFireballs, () => SaveGame.Members.LevelSmartFireballsX2);
        SmartFireballs.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyBeefyEarl()
    {
        long bought = BuyLevels(BeefyEarlManager.OnBuy, () => SaveGame.Members.LevelBeefyEarl);
        BeefyEarl.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyBeefyEarlX2()
    {
        long bought = BuyX2(BeefyEarlManager.OnBuyX2, () => SaveGame.Members.LevelBeefyEarl, () => SaveGame.Members.LevelBeefyEarlX2);
        BeefyEarl.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyCriticalStrike()
    {
        long bought = BuyLevels(CriticalStrikeManager.OnBuy, () => SaveGame.Members.LevelCriticalStrike);
        CriticalStrike.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyCriticalStrikeX2()
    {
        long bought = BuyX2(CriticalStrikeManager.OnBuyX2, () => SaveGame.Members.LevelCriticalStrike, () => SaveGame.Members.LevelCriticalStrikeX2);
        CriticalStrike.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyPowerZap()
    {
        long bought = BuyLevels(PowerZapManager.OnBuy, () => SaveGame.Members.LevelPowerZap);
        PowerZap.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyPowerZapX2()
    {
        long bought = BuyX2(PowerZapManager.OnBuyX2, () => SaveGame.Members.LevelPowerZap, () => SaveGame.Members.LevelPowerZapX2);
        PowerZap.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuySkullSlicer()
    {
        long bought = BuyLevels(SkullSlicerManager.OnBuy, () => SaveGame.Members.LevelSkullSlicer);
        SkullSlicer.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuyStormLord()
    {
        long bought = BuyLevels(StormLordManager.OnBuy, () => SaveGame.Members.LevelStormLord);
        StormLord.SetPopupText();
        OnItemBought(bought);
    }

    public void OnBuySkullSlicerX2()
    {
        long bought = BuyX2(SkullSlicerManager.OnBuyX2, () => SaveGame.Members.LevelSkullSlicer, () => SaveGame.Members.LevelSkullSlicerX2);
        SkullSlicer.SetPopupText();
        OnX2ItemBought(bought);
    }

    public void OnBuyStormLordX2()
    {
        long bought = BuyX2(StormLordManager.OnBuyX2, () => SaveGame.Members.LevelStormLord, () => SaveGame.Members.LevelStormLordX2);
        StormLord.SetPopupText();
        OnX2ItemBought(bought);
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
        BeefyEarlManager.UpdatePlayerUpgrades();
        CriticalStrikeManager.UpdatePlayerUpgrades();
        PowerZapManager.UpdatePlayerUpgrades();
        SkullSlicerManager.UpdatePlayerUpgrades();
        StormLordManager.UpdatePlayerUpgrades();
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
        total += SaveGame.Members.LevelBeefyEarlX2;
        total += SaveGame.Members.LevelCriticalStrikeX2;
        total += SaveGame.Members.LevelPowerZapX2;
        total += SaveGame.Members.LevelSkullSlicerX2;
        total += SaveGame.Members.LevelStormLordX2;
        PlayerUpgrades.Data.NumberOfX2Bought = total;

        long bought = PlayerUpgrades.Data.NumberOfX2Bought;
        long bonuses = bought / 5;
        PlayerUpgrades.Data.PassiveIncomeX2Multiplier = X2BonusPerRank() * bonuses;

        // Celebrate a new X2 rank at the cursor. Skips the first frame (save load) and big jumps (save import);
        // decreases (ascend, wipe) just resync.
        // A buy-all (Ctrl on an X2) can cross several ranks at once, so any gain right after a click counts.
        // One popup per rank for the first two, then the rest coalesced into a third ("+N% income! X2 rank 3-8").
        // They go through X2RankUpEffect's queue (1 s apart, shared with beast / skin unlocks).
        if (_lastX2Ranks >= 0 && bonuses > _lastX2Ranks && (bonuses - _lastX2Ranks <= 3 || _x2BoughtByClick) && ClickDamage != null)
        {
            const long Separate = 2;
            long rank = _lastX2Ranks + 1;
            for (; rank <= bonuses && rank <= _lastX2Ranks + Separate; ++rank)
                X2RankUpEffect.Spawn(ClickDamage, Input.mousePosition, X2BonusPerRank() * 100, rank);
            if (rank <= bonuses)
                X2RankUpEffect.Spawn(ClickDamage, Input.mousePosition, X2BonusPerRank() * 100 * (bonuses - rank + 1), rank, bonuses);
        }

        _lastX2Ranks = bonuses;
        _x2BoughtByClick = false; // used up (it was cleared before this check ever ran, so big buy-alls never showed)
    }

    long _lastX2Ranks = -1;
    bool _x2BoughtByClick;

    // 10% passive income per X2 rank.
    public static double X2BonusPerRank() => 0.1;

    private void Awake()
    {
        Instance = this;
    }

    // Player upgrades and the upgrade list are refreshed 10x per second, not every frame: every price / affordability
    // check is big-number math that allocates (GC). Buys refresh right away (UpdateAllUpgrades).
    const float UiRefreshInterval = 0.1f;
    float _nextUiRefresh;

    private void Update()
    {
        UpdateNumberOfX2Bought();

        if (Time.unscaledTime < _nextUiRefresh)
            return;

        _nextUiRefresh = Time.unscaledTime + UiRefreshInterval;
        UpdatePlayerUpgrades();
        UpdateUpgradeUi();
    }
}
