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
        public static Decimal512 InitialPrice_FastFeet = InitialPrice_SmartDaggers * 50;
        // 2026-10: the late price steps outgrew income (each tier earns only ~x4-5 more): stagnating at Beefy Earl with
        // Critical Strike ~2 days away. Steps cut: Crypt Master x100 -> x40, Angry Fireballs x80 -> x30, Beefy Earl
        // x60 -> x25, Critical Strike .. Storm Lord x10 -> x6 each (Critical Strike ~27x cheaper, Storm Lord ~124x).
        public static Decimal512 InitialPrice_CryptMaster = InitialPrice_FastFeet * 40;
        public static Decimal512 InitialPrice_SmartFireballs = InitialPrice_CryptMaster * 30;
        public static Decimal512 InitialPrice_BeefyEarl = InitialPrice_SmartFireballs * 25;
        public static Decimal512 InitialPrice_CriticalStrike = InitialPrice_BeefyEarl * 6;
        public static Decimal512 InitialPrice_PowerZap = InitialPrice_CriticalStrike * 6;
        public static Decimal512 InitialPrice_SkullSlicer = InitialPrice_PowerZap * 6;
        public static Decimal512 InitialPrice_StormLord = InitialPrice_SkullSlicer * 6;

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
        // Middle tiers lowered (2026-10): around the first x16 + minor cards and the first rebirth, income jumped so much
        // that players skipped through these tiers. Two even geometric ramps instead: Frenzy 200K -> Chest Master 600M
        // (~x5 per tier, was ~x5.5-6.4) -> Angry Fireballs 2.5T (~x5.3 per tier). Ends unchanged. Was: 275K, 1.75M, 10M,
        // 62M, 300M, 1.4B, 5.5B, 25.2B, 110.1B, 500.25B.
        public static Decimal512 BaseIncome_ZapDamage =               200_000;
        public static Decimal512 BaseIncome_MoneyMaker =            1_000_000;
        public static Decimal512 BaseIncome_DaggerMaster =          4_900_000;
        public static Decimal512 BaseIncome_NecroNinja =           24_000_000;
        public static Decimal512 BaseIncome_SkullCrusher =        120_000_000;
        public static Decimal512 BaseIncome_ChestMaster =         600_000_000;
        public static Decimal512 BaseIncome_Voidgazer =         3_200_000_000;
        public static Decimal512 BaseIncome_SmartDaggers =     17_000_000_000;
        public static Decimal512 BaseIncome_FastFeet =         90_000_000_000;
        public static Decimal512 BaseIncome_CryptMaster =     480_000_000_000;
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
        // Credits 1-10, on top of the early discount. Playtests (2026-10): x4 (credit 3) took ~7 h, and credit 1 took 4 h
        // without constant buying. Set as a smooth price ladder (x2-3 per credit, easing to x1.5-1.8 back into the curve
        // at credit 11) instead of a few cut credits, so there's no stall after x4: 21M, 48M, 95M, 280M, 850M, 2.6B, 7B,
        // 15B, 27B, 46B, then 71.6B. Model (x2.75 credit XP per hour, x2 income at x4): credits at ~2.8, 3.9, 4.8, 5.4,
        // 6.2, 7.2, 8.2, 9.1, 9.7, 10.3 h - about one per hour, back on the old schedule by credit ~11.
        static readonly double[] FirstCreditsMul = { 0.103, 0.074, 0.045, 0.058, 0.096, 0.179, 0.321, 0.484, 0.639, 0.826 };
        const double CreditLateSteepness = 0.486;

        public static Decimal512 MonsterCreditXpForNextLevel(long level)
            => CreditXpCurve(level) * CreditTierDiscount();

        // Last 5 tiers (highest tier owned >= CreditTierDiscountFirstTier): credits get cheaper with each tier,
        // 1 / (1 + 0.15 * (tier - 19)): Beefy Earl x0.87, Critical Strike x0.77, Power Zap x0.69, Skull Slicer x0.63,
        // Storm Lord x0.57. The tier speed limit and the soft limit stay as they are.
        const int CreditTierDiscountFirstTier = 20;
        const double CreditTierDiscountPerTier = 0.15;

        static double CreditTierDiscount()
        {
            int top = UpgradeTierList.CountBought();
            if (top < CreditTierDiscountFirstTier)
                return 1.0;
            return 1.0 / (1.0 + CreditTierDiscountPerTier * (top - CreditTierDiscountFirstTier + 1));
        }

        static Decimal512 CreditXpCurve(long level)
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

        // Second step: from CreditLateDiscountEnd it fades on to CreditLateDiscount2Mul more by CreditLateDiscount2End.
        // Playtests (2026-10): ~12 s per credit at 14K -> x2/3 (total x0.222); then at 14.2K with Storm Lord ~7 s, wanted
        // ~3 s (1 s limit + soft limit: natural 6.9 s -> 2.8 s) -> x0.272 (total x0.091 from 14K).
        const double CreditLateDiscount2End = 14_000;
        const double CreditLateDiscount2Mul = 0.272;

        static double LateDiscount(long level)
        {
            double step1 = 1.0 - (1.0 - CreditLateDiscountMul) * SmoothStep(level, CreditLateDiscountStart, CreditLateDiscountEnd);
            double step2 = 1.0 - (1.0 - CreditLateDiscount2Mul) * SmoothStep(level, CreditLateDiscountEnd, CreditLateDiscount2End);
            return step1 * step2;
        }

        static double SmoothStep(long level, double from, double to)
        {
            double t = Math.Clamp((level - from) / (to - from), 0.0, 1.0);
            return t * t * (3 - 2 * t);
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

        // 0.1024 x 0.5: tuned for the first run's x0.5 credit pace (full bonus x CreditFirstRunPaceMul).
        const double CreditCostMul = 0.0512;
        const double CreditCostRamp = 85;
        const double CreditCostRampPower = 1.17;
        const double CreditCostExtra = 0.5;
        const double CreditCostExtraStart = 20;
        const double CreditCostEarly = 0.25;
        const double CreditCostEarlyFade = 30;

        // Credit XP per second = passive income. (There was a floor of 2% of the best income ever, meant as "~5 h per
        // credit right after a rebirth", but late game that best income is so high it gave ~1 credit per minute with zero
        // income - removed, playtest 2026-10.) Still tracks the best income (mystery buff excluded) for Head Start.
        public static Decimal512 CreditXpPerSecond()
        {
            var m = SaveGame.Members;
            Decimal512 income = GameManager.Instance.TotalPassiveIncome;
            double temp = PlayerUpgrades.Data.PassiveIncomeTempMultiplier;
            Decimal512 baseIncome = temp > 1.0 ? income / temp : income;
            if (baseIncome > m.MaxCreditIncome)
                m.MaxCreditIncome = baseIncome;

            return income;
        }

        // Rebirth credit bonus: credit XP x1.0 right after a rebirth, falling to x0.1 when lifetime credits reach 1.75x
        // what they were at that rebirth (CreditBonusStartLifetime, min CreditBonusMinStart). Curved (1 - 0.9 * t^2): stays high for a
        // while, then drops fast at the end - waiting it out gets slow, so rebirthing is the better move (playtest
        // 2026-10: was linear down to x0.5, then x0.25; waiting still beat rebirthing). Shown as 500% -> 0% (375% halfway; text only, was 200% but full vs drained is x10).
        // The drain never starts before CreditBonusMinStart lifetime credits, first run or after an early rebirth:
        // 500% until ~15.7, 435% at 20, 250% at 24, 0% at ~27.4 (playtest: the drain shouldn't be felt before ~20; it
        // was min 2, which drained an early rebirth within ~1.5 credits). Rebirths later than that: L -> 1.75 L.
        // First run (never rebirthed): actual speed scaled by CreditFirstRunPaceMul, so at full bonus it's exactly the
        // old flat x0.5 that the early credit curve is tuned for - only the late drain is new.
        const double CreditBonusMinStart = 15.68;
        const double CreditFirstRunPaceMul = 0.5;
        const double CreditBonusEndFactor = 1.75;
        const double CreditBonusMinMul = 0.1;
        const double CreditBonusCurvePower = 2;
        const double CreditBonusDisplayMax = 500;

        // Lifetime credits plus the XP towards the next one, so the bonus slides smoothly instead of stepping per credit.
        // Also stored at rebirth (SaveGameAscend), since the leftover XP carries over.
        public static double CreditsEarnedWithPartial(SaveGameMembers m)
        {
            long lifetime = m.MonsterCreditsLifetime_09_08_2025;
            double partial = Math.Clamp((m.MonsterCreditsXp_09_08_2025 / MonsterCreditXpForNextLevel(lifetime + 1)).ToDouble(), 0.0, 1.0);
            return lifetime + partial;
        }

        // 0 = just rebirthed (500%), 1 = reached 1.75x the start (0%).
        public static double CreditBonusProgress()
        {
            var m = SaveGame.Members;
            double earned = CreditsEarnedWithPartial(m);
            if (m.CreditBonusStartLifetime < 0)
                m.CreditBonusStartLifetime = earned;

            double start = Math.Max(CreditBonusMinStart, m.CreditBonusStartLifetime);
            double t = (earned - start) / ((CreditBonusEndFactor - 1.0) * start);
            return Math.Clamp(t, 0.0, 1.0);
        }

        // The bonus curve itself: x1.0 (full) down to CreditBonusMinMul (drained).
        static double CreditBonusCurve()
            => 1.0 - (1.0 - CreditBonusMinMul) * Math.Pow(CreditBonusProgress(), CreditBonusCurvePower);

        // What credit XP is multiplied by. First run: scaled to its tuned pace (see CreditFirstRunPaceMul).
        public static double CreditBonusMultiplier()
            => CreditBonusCurve() * (SaveGame.Members.TimesAscended_09_08_2025 == 0 ? CreditFirstRunPaceMul : 1.0);

        // 500 (full bonus) down to 0, a straight map of the bonus curve. Rounded up: rounding to nearest showed 499%
        // within seconds of a rebirth. Still exactly 0 at the end.
        public static int CreditBonusDisplayPct()
            => (int)Math.Ceiling((CreditBonusCurve() - CreditBonusMinMul) / (1.0 - CreditBonusMinMul) * CreditBonusDisplayMax - 1e-9);

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
        public const double SkinCollectorPerSkin = 0.10;

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
        // being cut off at it, so more income keeps mattering. Effective credits/s = natural / (1 + (natural * T)^p)^(1/p),
        // T = MinSecondsPerCredit, p = CreditSoftLimitSharpness. p = 2: 71% of the limit when natural speed equals it,
        // 89% at 2x, 95% at 3x (p = 1 was 50% / 67% / 75%: too slow at Beefy Earl). Returned as a factor for the credit
        // XP rate. Earlier tiers: x1 (only the plain 1 s cap).
        const double CreditSoftLimitSharpness = 2.0;

        public static double CreditSoftLimitFactor(Decimal512 creditXpPerSecond)
        {
            if (UpgradeTierList.CountBought() < CreditLimitFirstTier)
                return 1.0;

            Decimal512 price = MonsterCreditXpForNextLevel(SaveGame.Members.MonsterCreditsLifetime_09_08_2025 + 1);
            double naturalCreditsPerSecond = (creditXpPerSecond / price).ToDouble();
            if (naturalCreditsPerSecond <= 0 || double.IsNaN(naturalCreditsPerSecond))
                return 1.0;
            double x = naturalCreditsPerSecond * MinSecondsPerCredit();
            return 1.0 / Math.Pow(1.0 + Math.Pow(x, CreditSoftLimitSharpness), 1.0 / CreditSoftLimitSharpness);
        }

        // Head Start rebirth card: seconds of best-ever passive income the new run starts with (SaveGameAscend).
        public const double HeadStartSeconds = 25 * 60;

        // Haggler rebirth card: tier upgrades and X2 cost half (not the 1% bonus).
        public static double ShopPriceMul() => SaveGame.Members.BoughtHaggler ? 0.5 : 1.0;

        // Diamonds held give passive income: base 10% per diamond, Shiny Diamonds cards 1-5 add 10/20/40/60/100% (max
        // 240% per diamond). (Briefly halved in 2026-10; back to these values, the middle tier incomes were lowered
        // instead.) Returned as the bonus fraction, income x(1 + bonus). Spending diamonds on cards lowers it.
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
