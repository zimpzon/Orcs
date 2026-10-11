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
        public static Decimal512 InitialPrice_NecroNinja =                  26_375_000_000_000;
        public static Decimal512 InitialPrice_SkullCrusher =               412_625_000_000_000;
        public static Decimal512 InitialPrice_ChestMaster =              8_125_125_000_000_000;
        public static Decimal512 InitialPrice_Voidgazer =              312_500_000_000_000_000;
        public static Decimal512 InitialPrice_SmartDaggers =        19_062_500_000_000_000_000.0; // .0: too big for an integer literal
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

        // Credits 1-20: the tuned early curve below. From credit 21: the cubic shape of the released game's curve
        // (495B + 58B * (level - 8)^3), scaled to join the early curve at credit 20. Starting point, to be tuned.
        const long CreditCurveJoin = 20;
        const double EarlyCreditDiscount = 0.2;
        // Credits 1-4, on top of the early discount. Credit 1: ~28M XP; a model of a first run buying every ~10 min
        // (calibrated on the old 6 h playtest) puts it at ~3 h (was ~4.5 h). Credits 2-4 (x0.5, x0.6, x0.75, fading back
        // to x1 at credit 5) so x4 income (4 diamonds) comes sooner. Prices: 28M, 324M, 1.26B, 3.59B, then 8.88B.
        static readonly double[] FirstCreditsMul = { 0.136, 0.5, 0.6, 0.75 };
        const double CreditLateSteepness = 0.486;

        public static Decimal512 MonsterCreditXpForNextLevel(long level)
        {
            if (level <= CreditCurveJoin)
            {
                // Early discount fading out linearly: -20% at credit 1, none at credit 20, so the curve stays continuous.
                double discount = EarlyCreditDiscount * (CreditCurveJoin - level) / (CreditCurveJoin - 1);
                Decimal512 price = EarlyCreditXp(level) * (1 - discount);
                // The first credits cheaper still, so the first x2 cards (x4 at 4 diamonds) come sooner.
                return level <= FirstCreditsMul.Length ? price * FirstCreditsMul[level - 1] : price;
            }

            // Steepened: ((level - 8) / 12)^CreditLateSteepness is x1 at the join (credit 20), x6 at credit 487 (2 min per
            // credit there instead of 20 s, playtest), x2.7 at 100, x8.7 at 1000.
            double steeper = Math.Pow((level - 8) / (double)(CreditCurveJoin - 8), CreditLateSteepness);
            return EarlyCreditXp(CreditCurveJoin) * (ReleaseCurveShape(level) / ReleaseCurveShape(CreditCurveJoin) * steeper * LateTail(level) * LateDiscount(level));
        }

        // Late discount: x1 up to CreditLateDiscountStart, smoothstep down to CreditLateDiscountMul by CreditLateDiscountEnd,
        // then flat. Playtest (2026-10): at 9,200 lifetime credits with 19 tiers credits took ~40 s; x1/3 makes it ~13 s.
        // Floods stay capped by the tier speed limit (MinSecondsPerCredit). Fades in slower than prices grow, so the
        // price keeps rising every credit (checked to 40,000).
        const double CreditLateDiscountStart = 3_000;
        const double CreditLateDiscountEnd = 9_000;
        const double CreditLateDiscountMul = 1.0 / 3.0;

        static double LateDiscount(long level)
        {
            double t = Math.Clamp((level - CreditLateDiscountStart) / (CreditLateDiscountEnd - CreditLateDiscountStart), 0.0, 1.0);
            double s = t * t * (3 - 2 * t);
            return 1.0 - (1.0 - CreditLateDiscountMul) * s;
        }

        // Endgame tail: extra per-credit price growth on top of the polynomial. The rate rises linearly from 0 at
        // CreditTailStart to CreditTailPeakRate at CreditTailPeak (the playtest point: ~27 s per credit at ~4050 with
        // income near the run's ceiling), then eases off to CreditTailFinalRate at CreditTailEnd and stays there. The
        // 1-per-second credit cap handles floods, so the late curve can be flat; the remaining tiers and beasts (~x1000-
        // 5000 income) should bring credits to ~1 s by the end. Credits per x10 income: ~720 at 4050, ~1000 at 4500,
        // ~1770 at 5000, ~2100 at 7000.
        const double CreditTailStart = 2000;
        const double CreditTailPeak = 4050;
        const double CreditTailEnd = 5000;
        const double CreditTailPeakRate = 0.00235;
        const double CreditTailFinalRate = 0.0006;

        static double LateTail(long level)
        {
            double x = level - CreditTailStart;
            if (x <= 0)
                return 1.0;

            double rise = CreditTailPeak - CreditTailStart;
            double fall = CreditTailEnd - CreditTailPeak;
            double exponent;
            if (x <= rise)
            {
                exponent = CreditTailPeakRate * x * x / (2 * rise);
            }
            else
            {
                double atPeak = CreditTailPeakRate * rise / 2;
                double y = Math.Min(x - rise, fall);
                exponent = atPeak + CreditTailPeakRate * y - (CreditTailPeakRate - CreditTailFinalRate) * y * y / (2 * fall);
                exponent += CreditTailFinalRate * Math.Max(0, x - rise - fall);
            }
            return Math.Exp(Math.Min(700, exponent)); // no double overflow
        }

        static double ReleaseCurveShape(long level)
        {
            double d = level - 8;
            return 495 + 58 * d * d * d;
        }

        // 1B x (1 + 2n^2), n = level - 1, with three gentle factors: ramp (1 + n/85)^1.17, extra
        // 1 + 0.5 * n^2 / (n^2 + 20^2) and an early bump (credit 2 x1.03, 5 x1.16, 10-20 ~x1.22).
        static Decimal512 EarlyCreditXp(long level)
        {
            double n = level - 1;
            double ramp = Math.Pow(1 + n / CreditCostRamp, CreditCostRampPower);
            double extra = 1 + CreditCostExtra * n * n / (n * n + CreditCostExtraStart * CreditCostExtraStart);
            double early = 1 + CreditCostEarly * (n * n / (n * n + 9)) / (1 + Math.Pow(n / CreditCostEarlyFade, 4));
            return 5 * OneBillion * (1 + 2 * n * n) * (CreditCostMul * ramp * extra * early);
        }

        // 0.1024 x 0.5: the rebirth credit bonus bottoms out at x0.5 (shown 0%), which then matches the tuned pace.
        const double CreditCostMul = 0.0512;
        const double CreditCostRamp = 85;
        const double CreditCostRampPower = 1.17;
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

        // Rebirth credit bonus: credit XP x1.0 right after a rebirth, falling in a straight line to x0.5 when lifetime
        // credits reach 1.75x what they were at that rebirth (CreditBonusStartLifetime, min 2). Shown as 100% -> 0%.
        // No bonus (x0.5, 0%) before the first rebirth.
        const long CreditBonusMinLifetime = 2;
        const double CreditBonusEndFactor = 1.75;
        const double CreditBonusMinMul = 0.5;

        // Lifetime credits plus the XP towards the next one, so the bonus slides smoothly instead of stepping per credit.
        // Also stored at rebirth (SaveGameAscend), since the leftover XP carries over.
        public static double CreditsEarnedWithPartial(SaveGameMembers m)
        {
            long lifetime = m.MonsterCreditsLifetime_09_08_2025;
            double partial = Math.Clamp((m.MonsterCreditsXp_09_08_2025 / MonsterCreditXpForNextLevel(lifetime + 1)).ToDouble(), 0.0, 1.0);
            return lifetime + partial;
        }

        // 0 = just rebirthed (100%), 1 = reached 1.75x the start (0%).
        public static double CreditBonusProgress()
        {
            var m = SaveGame.Members;
            double earned = CreditsEarnedWithPartial(m);
            if (m.CreditBonusStartLifetime < 0)
                m.CreditBonusStartLifetime = earned;

            double start = Math.Max(CreditBonusMinLifetime, m.CreditBonusStartLifetime);
            double t = (earned - start) / ((CreditBonusEndFactor - 1.0) * start);
            return Math.Clamp(t, 0.0, 1.0);
        }

        public static double CreditBonusMultiplier()
        {
            if (SaveGame.Members.TimesAscended_09_08_2025 == 0)
                return CreditBonusMinMul;

            return 1.0 - (1.0 - CreditBonusMinMul) * CreditBonusProgress();
        }

        // 100 (just rebirthed) down to 0, linear like the multiplier. Rounded up: rounding to nearest showed 99% within
        // seconds of a rebirth. Still exactly 0 at the end.
        public static int CreditBonusDisplayPct()
            => (int)Math.Ceiling((CreditBonusMultiplier() - CreditBonusMinMul) / (1.0 - CreditBonusMinMul) * 100 - 1e-9);

        // Comparison with original:
        // Level 20: Original ~25000B, New ~31250B (+25%)
        // Level 30: Original ~82815B, New ~103519B (+25%) 
        // Level 50: Original ~571815B, New ~714769B (+25%)
        public static Decimal512 PriceX2(Decimal512 initialPrice, long levelX2)
        {
            Decimal512 result = initialPrice * Math.Pow(3, levelX2);
            return result * ShopPriceMul();
        }

        // Skin Collector rebirth card: passive income bonus per unlocked skin.
        public const double SkinCollectorPerSkin = 0.05;

        // Completionist rebirth card: passive income bonus per game completion % (GameCompletion.TotalPct).
        public const double CompletionistPerPct = 0.01;

        // Credit speed limit in the last tiers, where income explodes: with tier 15 as the highest tier bought (tiers unlock
        // in order, so count = highest tier) credits can't come faster than every 10 s, tier 16 every 9 s ... Storm Lord
        // (tier 24) every 1 s. Below tier 15 only the normal 1-per-second cap applies.
        const int CreditLimitFirstTier = 15;
        const int CreditLimitSlowestSeconds = 10;

        public static double MinSecondsPerCredit()
        {
            int top = UpgradeTierList.CountBought();
            if (top < CreditLimitFirstTier)
                return 1.0;

            int last = UpgradeTierList.Tiers.Length; // 24 = Storm Lord
            return Math.Max(1, CreditLimitSlowestSeconds - (top - CreditLimitFirstTier) * (CreditLimitSlowestSeconds - 1) / (last - CreditLimitFirstTier));
        }

        // Soft speed limit in the last tiers (tier >= CreditLimitFirstTier): the credit speed approaches the limit instead of
        // being cut off at it, so more income keeps mattering. Effective credits/s = natural / (1 + natural * T), T =
        // MinSecondsPerCredit: half the limit when natural speed equals it, 90% at 9x, 95% at 19x. Returned as a factor
        // for the credit XP rate. Earlier tiers: x1 (only the plain 1 s cap).
        public static double CreditSoftLimitFactor(Decimal512 creditXpPerSecond)
        {
            if (UpgradeTierList.CountBought() < CreditLimitFirstTier)
                return 1.0;

            Decimal512 price = MonsterCreditXpForNextLevel(SaveGame.Members.MonsterCreditsLifetime_09_08_2025 + 1);
            double naturalCreditsPerSecond = (creditXpPerSecond / price).ToDouble();
            if (naturalCreditsPerSecond <= 0 || double.IsNaN(naturalCreditsPerSecond))
                return 1.0;
            return 1.0 / (1.0 + naturalCreditsPerSecond * MinSecondsPerCredit());
        }

        // Head Start rebirth card: seconds of best-ever passive income the new run starts with (SaveGameAscend).
        public const double HeadStartSeconds = 25 * 60;

        // Haggler rebirth card: tier upgrades and X2 cost half (not the 1% bonus).
        public static double ShopPriceMul() => SaveGame.Members.BoughtHaggler ? 0.5 : 1.0;

        // Diamonds held give passive income: base 10% per diamond, Shiny Diamonds cards 1-5 add 10/20/40/60/100%.
        // Returned as the bonus fraction, income x(1 + bonus). Spending diamonds on cards lowers it.
        const double DiamondBonusBase = 0.10;

        public static double DiamondBonusPerDiamond()
        {
            var m = SaveGame.Members;
            double pct = DiamondBonusBase;
            if (m.BoughtShinyDiamonds1) pct += 0.10;
            if (m.BoughtShinyDiamonds2) pct += 0.20;
            if (m.BoughtShinyDiamonds3) pct += 0.40;
            if (m.BoughtShinyDiamonds4) pct += 0.60;
            if (m.BoughtShinyDiamonds5) pct += 1.00;
            return pct;
        }

        public static double DiamondIncomeBonus()
            => DiamondBonusPerDiamond() * SaveGame.Members.DiamondCount_09_08_2025;

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

        // Each tier level costs this much more than the previous one. Was 1.15: runs stalled too early (level 100 now x0.17,
        // level 300 x0.005 of the old price). 1.14 is the middle step if this turns out too generous.
        const double LevelPriceGrowth = 1.13;

        public static Decimal512 PriceForNextUpgrade(Decimal512 initialPrice, long currentLevel, long currentLevelX2)
        {
            long buyAmount = GetActualBuyAmountFromSelectedBuyAmount(currentLevel, currentLevelX2);

            Decimal512 sum = 0;
            for (int i = 0; i < buyAmount; ++i)
            {
                Decimal512 priceForLevel = initialPrice * Math.Pow(LevelPriceGrowth, currentLevel + i);
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
            double growthRate = 1.2; // was 1.2378 (and 1.245 before that): level 100 x0.05 of the 1.2378 price
            double exponent = level - 3;
            return basePrice * (Decimal512)Math.Pow(growthRate, exponent);
        }
    }
}
