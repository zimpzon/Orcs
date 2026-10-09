using System;

namespace Assets.Script.Upgrades
{
    public class ConditionsX2
    {
        public Decimal512 Price;
        public long LevelRequirement;
    }

    public static class UpgradeProgression
    {
        public static Decimal512 OneMillion = 1_000_000;
        public static Decimal512 OneBillion = OneMillion * 1000;
        public static Decimal512 OneTrillion = OneBillion * 1000;

        public static Decimal512 InitialPrice_Clickdamage =                                 50;
        public static Decimal512 InitialPrice_DaggerDamage =                               300;
        public static Decimal512 InitialPrice_GoldValue =                                3_100;
        public static Decimal512 InitialPrice_DaggerCd =                                42_000;
        public static Decimal512 InitialPrice_WitchDoctor =                            750_000;
        public static Decimal512 InitialPrice_GoldPerKnifeThrown =                  10_100_000;
        public static Decimal512 InitialPrice_Wizard =                             150_000_000;
        public static Decimal512 InitialPrice_Hoarder =                          2_100_000_000;
        public static Decimal512 InitialPrice_ZapDamage =                       20_500_000_000;
        public static Decimal512 InitialPrice_MoneyMaker =                     210_000_000_000;
        public static Decimal512 InitialPrice_DaggerMaster =                 2_300_000_000_000;
        public static Decimal512 InitialPrice_NecroNinja =                  21_100_000_000_000;
        public static Decimal512 InitialPrice_SkullCrusher =               330_100_000_000_000;
        public static Decimal512 InitialPrice_ChestMaster =              6_500_100_000_000_000;
        public static Decimal512 InitialPrice_Voidgazer =              250_000_000_000_000_000;
        public static Decimal512 InitialPrice_SmartDaggers =        15_250_000_000_000_000_000;
        public static Decimal512 InitialPrice_FastFeet = InitialPrice_SmartDaggers * 100;
        public static Decimal512 InitialPrice_CryptMaster = InitialPrice_FastFeet * 100;
        public static Decimal512 InitialPrice_SmartFireballs = InitialPrice_CryptMaster * 80;
        // Trimmed from *80 to *60 to make room for CriticalStrike, which now slots in at BeefyEarl's old price.
        public static Decimal512 InitialPrice_BeefyEarl = InitialPrice_SmartFireballs * 60;
        // Last tiers step x10: each only earns x4 more, and since the diamond cut (240% -> 80%/diamond) late income is
        // lower, so x60 put them months-to-years away. Critical Strike used to be SmartFireballs * 80, only 1.3x Beefy
        // Earl, so it came almost free right after it - now 10x Beefy.
        public static Decimal512 InitialPrice_CriticalStrike = InitialPrice_BeefyEarl * 10;
        public static Decimal512 InitialPrice_PowerZap = InitialPrice_CriticalStrike * 10;
        public static Decimal512 InitialPrice_SkullSlicer = InitialPrice_PowerZap * 10;
        public static Decimal512 InitialPrice_StormLord = InitialPrice_SkullSlicer * 10;

        // billion :     1_000_000_000
        // trillion: 1_000_000_000_000

        public static Decimal512 InitialPrice_Clickdamage_X2 = InitialPrice_Clickdamage * 10;
        public static Decimal512 InitialPrice_DaggerDamage_X2 = InitialPrice_DaggerDamage * 10;
        public static Decimal512 InitialPrice_GoldValue_X2 = InitialPrice_GoldValue * 10;
        public static Decimal512 InitialPrice_DaggerCd_X2 = InitialPrice_DaggerCd * 10;
        public static Decimal512 InitialPrice_WitchDoctor_X2 = InitialPrice_WitchDoctor * 10;
        public static Decimal512 InitialPrice_GoldPerKnifeThrown_X2 = InitialPrice_GoldPerKnifeThrown * 10;
        public static Decimal512 InitialPrice_Wizard_X2 = InitialPrice_Wizard * 10;
        public static Decimal512 InitialPrice_Hoarder_X2 = InitialPrice_Hoarder * 10;
        public static Decimal512 InitialPrice_ZapDamage_X2 = InitialPrice_ZapDamage * 10;
        public static Decimal512 InitialPrice_MoneyMaker_X2 = InitialPrice_MoneyMaker * 10;
        public static Decimal512 InitialPrice_DaggerMaster_X2 = InitialPrice_DaggerMaster * 10;
        public static Decimal512 InitialPrice_NecroNinja_X2 = InitialPrice_NecroNinja * 10;
        public static Decimal512 InitialPrice_SkullCrusher_X2 = InitialPrice_SkullCrusher * 10;
        public static Decimal512 InitialPrice_ChestMaster_X2 = InitialPrice_ChestMaster * 10;
        public static Decimal512 InitialPrice_Voidgazer_X2 = InitialPrice_Voidgazer * 10;
        public static Decimal512 InitialPrice_SmartDaggers_X2 = InitialPrice_SmartDaggers * 10;
        public static Decimal512 InitialPrice_FastFeet_X2 = InitialPrice_FastFeet * 10;
        public static Decimal512 InitialPrice_CryptMaster_X2 = InitialPrice_CryptMaster * 10;
        public static Decimal512 InitialPrice_SmartFireballs_X2 = InitialPrice_SmartFireballs * 10;
        public static Decimal512 InitialPrice_BeefyEarl_X2 = InitialPrice_BeefyEarl * 10;
        public static Decimal512 InitialPrice_CriticalStrike_X2 = InitialPrice_CriticalStrike * 10;
        public static Decimal512 InitialPrice_PowerZap_X2 = InitialPrice_PowerZap * 10;
        public static Decimal512 InitialPrice_SkullSlicer_X2 = InitialPrice_SkullSlicer * 10;
        public static Decimal512 InitialPrice_StormLord_X2 = InitialPrice_StormLord * 10;

        public static Decimal512 BaseIncome_Clickdamage = 0.2;
        public static Decimal512 BaseIncome_DaggerDamage = 2;
        public static Decimal512 BaseIncome_GoldValue = 10;
        public static Decimal512 BaseIncome_DaggerCd = 50;
        public static Decimal512 BaseIncome_WitchDoctor = 275;
        public static Decimal512 BaseIncome_GoldPerKnifeThrown = 1_400;
        public static Decimal512 BaseIncome_Wizard = 10_000;
        public static Decimal512 BaseIncome_Hoarder = 50_000;
        public static Decimal512 BaseIncome_ZapDamage = 275_000;
        public static Decimal512 BaseIncome_MoneyMaker =            1_750_000;
        public static Decimal512 BaseIncome_DaggerMaster =         10_000_000;
        public static Decimal512 BaseIncome_NecroNinja =           62_000_000;
        public static Decimal512 BaseIncome_SkullCrusher =        300_000_000;
        public static Decimal512 BaseIncome_ChestMaster =       1_400_000_000;
        public static Decimal512 BaseIncome_Voidgazer =         5_500_000_000;
        public static Decimal512 BaseIncome_SmartDaggers =     25_200_000_000;
        public static Decimal512 BaseIncome_FastFeet =        110_100_000_000;
        public static Decimal512 BaseIncome_CryptMaster =     500_250_000_000;
        public static Decimal512 BaseIncome_SmartFireballs =2_500_250_000_000;
        public static Decimal512 BaseIncome_BeefyEarl =    10_625_625_000_000;
        public static Decimal512 BaseIncome_CriticalStrike = 42_502_500_000_000;
        public static Decimal512 BaseIncome_PowerZap =    170_010_000_000_000;
        public static Decimal512 BaseIncome_SkullSlicer = 680_040_000_000_000;
        public static Decimal512 BaseIncome_StormLord = 2_720_160_000_000_000;

        public static long DiamondsForMonsterCredits(long monsterCredits)
        {
            return monsterCredits;
        }

        public static Decimal512 MonsterCreditXpForNextLevel(long level)
        {
            // One smooth, always-rising curve: 5B x (1 + 2n^2) x CreditCostMul x (1 + n/CreditCostRamp)^CreditCostRampPower,
            // n = level - 1. The ramp factor makes later credits progressively pricier: vs the previous pure quadratic
            // (x0.125) the first 10 are ~10-20% cheaper, credit 100 x2, 250 x4, 1000 x16.
            // Late factor 1 + (n/CreditCostLateScale)^CreditCostLatePower on top: ~x1 up to credit 100, then steep
            // (200 x2, 300 x5, 400 x13, 500 x27, 600 x50, 1000 x286) - at ~600 credits came about once per second.
            // Extra factor 1 + CreditCostExtra * n^2 / (n^2 + CreditCostExtraStart^2): +50% overall but sparing the start
            // (credit 10 x1.08, 20 x1.24, 50 x1.43, 100+ ~x1.5).
            double n = level - 1;
            double ramp = Math.Pow(1 + n / CreditCostRamp, CreditCostRampPower);
            double late = 1 + Math.Pow(n / CreditCostLateScale, CreditCostLatePower);
            double extra = 1 + CreditCostExtra * n * n / (n * n + CreditCostExtraStart * CreditCostExtraStart);
            // Early bump: the first credits (up to ~20) a bit pricier, credit 1 unchanged, gone again by ~50-75
            // (credit 2 x1.03, 5 x1.16, 10-20 ~x1.22, 30 x1.13, 50 x1.03).
            double early = 1 + CreditCostEarly * (n * n / (n * n + 9)) / (1 + Math.Pow(n / CreditCostEarlyFade, 4));
            return 5 * OneBillion * (1 + 2 * n * n) * (CreditCostMul * ramp * late * extra * early);
        }

        const double CreditCostMul = 0.1;
        const double CreditCostRamp = 85;
        const double CreditCostRampPower = 1.17;
        const double CreditCostLateScale = 195;
        const double CreditCostLatePower = 3.46;
        const double CreditCostExtra = 0.5;
        const double CreditCostExtraStart = 20;
        const double CreditCostEarly = 0.25;
        const double CreditCostEarlyFade = 30;

        // Small credit speed floor: credit XP per second never drops below 2% of the best passive income reached
        // (mystery buff excluded, so a 10x spike can't inflate it). Only matters right after a rebirth, when income
        // restarts near zero: still slow (~5 h per credit) but the bar visibly moves. Real income takes over soon.
        const double CreditSpeedFloorFraction = 0.02;

        public static Decimal512 CreditXpPerSecond()
        {
            var m = SaveGame.Members;
            Decimal512 income = GameManager.Instance.TotalPassiveIncome;
            double temp = PlayerUpgrades.Data.PassiveIncomeTempMultiplier;
            Decimal512 baseIncome = temp > 1.0 ? income / temp : income;
            if (baseIncome > m.MaxCreditIncome)
                m.MaxCreditIncome = baseIncome;

            Decimal512 floor = m.MaxCreditIncome * CreditSpeedFloorFraction;
            return income > floor ? income : floor;
        }

        // Rebirth credit bonus: credits are cheap, so to still force a rebirth now and then the credit XP rate is
        // multiplied by x1.0 right after a rebirth, falling by the same % per credit to CreditBonusMinMul when lifetime credits reach
        // 1.75x what they were at that rebirth (CreditBonusStartLifetime, min 20). Below 20 lifetime credits it's always
        // x1.0. Shown to the player as a bonus going from 500% down to 100%.
        const long CreditBonusMinLifetime = 20;
        const double CreditBonusEndFactor = 1.75;
        const double CreditBonusMinMul = 0.02;

        // 0 = just rebirthed (x1.0, 500%), 1 = reached 1.75x the start (x0.02, 100%).
        public static double CreditBonusProgress()
        {
            var m = SaveGame.Members;
            long lifetime = m.MonsterCreditsLifetime_09_08_2025;
            if (m.CreditBonusStartLifetime < 0)
                m.CreditBonusStartLifetime = lifetime; // saves from before this existed start fresh at 500%

            if (lifetime < CreditBonusMinLifetime)
                return 0.0;

            double start = Math.Max(CreditBonusMinLifetime, m.CreditBonusStartLifetime);
            double t = (lifetime - start) / ((CreditBonusEndFactor - 1.0) * start);
            return Math.Clamp(t, 0.0, 1.0);
        }

        // MinMul^(t^power): x1.0 -> CreditBonusMinMul. A straight line made the last credits x1.5-x2 slower each
        // (x0.01 -> x0.005 halves speed in one credit); a plain MinMul^t (power 1) dropped most of it early (x0.38 at
        // 25%). Power 2 is in between: x0.96 / 0.78 / 0.38 / 0.11 at 10/25/50/75%, at most ~3% slower per credit.
        const double CreditBonusCurvePower = 2.0;

        public static double CreditBonusMultiplier()
            => Math.Pow(CreditBonusMinMul, Math.Pow(CreditBonusProgress(), CreditBonusCurvePower));

        // Shown from 500% (just rebirthed, x1.0) down to 100% (x0.02).
        public static int CreditBonusDisplayPct()
            => (int)Math.Round(500 - 400 * CreditBonusProgress());

        // Comparison with original:
        // Level 20: Original ~25000B, New ~31250B (+25%)
        // Level 30: Original ~82815B, New ~103519B (+25%) 
        // Level 50: Original ~571815B, New ~714769B (+25%)
        public static Decimal512 PriceX2(Decimal512 initialPrice, long levelX2)
        {
            Decimal512 result = initialPrice * Math.Pow(3, levelX2);
            return result * ShopPriceMul();
        }

        // Haggler ascend cards: each owned tier halves all gold shop prices (not the 1% bonus).
        public static double ShopPriceMul()
        {
            double mul = 1.0;
            if (SaveGame.Members.BoughtHaggler1) mul *= 0.5;
            if (SaveGame.Members.BoughtHaggler2) mul *= 0.5;
            if (SaveGame.Members.BoughtHaggler3) mul *= 0.5;
            return mul;
        }

        public static long LevelRequirementX2(long levelX2)
        {
            // Copied from Cookie Clicker
            // 1: level 1
            // 2: level 5
            // 3: level 25
            // 4: level 50
            // 5: ...from here it is +25 per level
            if (levelX2 == 0) return 0;
            if (levelX2 == 1) return 1;
            if (levelX2 == 2) return 5;
            if (levelX2 == 3) return 25;

            // > 3
            return (levelX2 - 3) * 25;
        }

        public static long GetActualBuyAmountFromSelectedBuyAmount(long currentLevel, long currentLevelX2)
        {
            var selection = GameManager.Instance.SelectedBuyAmount;
            if (selection == GameManager.BuyAmountSelection.Buy1)
            {
                return 1;
            }
            else if (selection == GameManager.BuyAmountSelection.Buy10)
            {
                return 10;
            }
            else if (selection == GameManager.BuyAmountSelection.Buy100)
            {
                return 100;
            }
            else if (selection == GameManager.BuyAmountSelection.Buy50)
            {
                return 50;
            }
            else
                throw new NotImplementedException($"{selection}");
        }

        public static Decimal512 PriceForNextUpgrade(Decimal512 initialPrice, long currentLevel, long currentLevelX2)
        {
            long buyAmount = GetActualBuyAmountFromSelectedBuyAmount(currentLevel, currentLevelX2);

            Decimal512 sum = 0;
            for (int i = 0; i < buyAmount; ++i)
            {
                Decimal512 priceForLevel = initialPrice * Math.Pow(1.15, currentLevel + i);
                sum += priceForLevel;
            }
            return sum * ShopPriceMul();
        }

        public static Decimal512 PriceNextPercentageBonus(long level)
        {
            if (level == 0) return 500;
            if (level == 1) return 10_000;
            if (level == 2) return 100_000;
            if (level == 3) return 250_000;
            Decimal512 basePrice = 500_000;
            double growthRate = 1.2378; // Reduced from 1.245 to halve level 100 cost
            double exponent = level - 3;
            return basePrice * (Decimal512)Math.Pow(growthRate, exponent);
        }

        // Lore rebirth cards: +1% passive income per level owned (this run) of one tier each; the four multiply.
        const double TierLorePctPerLevel = 0.01;

        public static double TierLoreMultiplier(bool bought, long level)
            => bought ? 1.0 + TierLorePctPerLevel * Math.Max(0, level) : 1.0;

        public static double ZapLoreMultiplier() => TierLoreMultiplier(SaveGame.Members.BoughtZapLore, SaveGame.Members.LevelClickDamage);
        public static double ChestLoreMultiplier() => TierLoreMultiplier(SaveGame.Members.BoughtChestLore, SaveGame.Members.LevelMoneyPerGold);
        public static double VoodooLoreMultiplier() => TierLoreMultiplier(SaveGame.Members.BoughtVoodooLore, SaveGame.Members.LevelWitchDoctor);
        public static double WizardLoreMultiplier() => TierLoreMultiplier(SaveGame.Members.BoughtWizardLore, SaveGame.Members.LevelWizard);

        public static double TierLoreTotalMultiplier()
            => ZapLoreMultiplier() * ChestLoreMultiplier() * VoodooLoreMultiplier() * WizardLoreMultiplier();

        // Rebirth bonus: at every rebirth the passive income bonus is set to 5% per hour played in total
        // (SaveGameAscend), x(1 + bonus). The number of rebirths doesn't matter, so tiny 1-credit rebirths can't farm
        // it. Shown in the rebirth dialog (AscendDecisionScript) and on the stats page.
        public const double RebirthBonusPerHour = 0.05;

        public static double HoursPlayed()
            => SaveGame.Members.EstimatedOnlineSeconds2 / 3600.0;

        // Current bonus (set at the last rebirth).
        public static double RebirthBonus()
            => SaveGame.Members.RebirthIncomeBonus;

        // What the bonus becomes when rebirthing now.
        public static double RebirthBonusIfRebirthNow()
            => RebirthBonusPerHour * HoursPlayed();

        // Head Start rebirth card: seconds of best-ever passive income the new run starts with (SaveGameAscend).
        public const double HeadStartSeconds = 60;
    }
}
