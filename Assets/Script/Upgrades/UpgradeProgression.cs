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
        const double CreditLateSteepness = 0.486;

        public static Decimal512 MonsterCreditXpForNextLevel(long level)
        {
            if (level <= CreditCurveJoin)
            {
                // Early discount fading out linearly: -20% at credit 1, none at credit 20, so the curve stays continuous.
                double discount = EarlyCreditDiscount * (CreditCurveJoin - level) / (CreditCurveJoin - 1);
                return EarlyCreditXp(level) * (1 - discount);
            }

            // Steepened: ((level - 8) / 12)^CreditLateSteepness is x1 at the join (credit 20), x6 at credit 487 (2 min per
            // credit there instead of 20 s, playtest), x2.7 at 100, x8.7 at 1000.
            double steeper = Math.Pow((level - 8) / (double)(CreditCurveJoin - 8), CreditLateSteepness);
            return EarlyCreditXp(CreditCurveJoin) * (ReleaseCurveShape(level) / ReleaseCurveShape(CreditCurveJoin) * steeper * LateTail(level));
        }

        // Endgame tail: the polynomial above flattens (at ~2000 credits a x10 income jump was worth ~1300 credits, and the
        // last tiers multiply income many times). A wide, smooth bend: the extra per-credit price growth ramps linearly
        // from 0 at CreditTailStart to CreditTailRate at CreditTailEnd, then stays there. Credits per x10 income: ~870 at
        // 2900, ~740 at 3550, ~530 at 5000, ~380 from 7000 on. Playtest: ~30 s per credit at 3550 with upgrades bought.
        const double CreditTailStart = 1500;
        const double CreditTailEnd = 7000;
        const double CreditTailRate = 0.00569;

        static double LateTail(long level)
        {
            double x = level - CreditTailStart;
            if (x <= 0)
                return 1.0;

            double width = CreditTailEnd - CreditTailStart;
            double exponent = x <= width
                ? CreditTailRate * x * x / (2 * width)
                : CreditTailRate * (width / 2 + (x - width));
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

        // Head Start rebirth card: seconds of best-ever passive income the new run starts with (SaveGameAscend).
        public const double HeadStartSeconds = 10;

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
    }
}
