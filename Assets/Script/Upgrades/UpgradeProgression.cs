using System;

namespace Assets.Script.Upgrades
{
    public class ConditionsX2
    {
        public Decimal256 Price;
        public long LevelRequirement;
    }

    public static class UpgradeProgression
    {
        public static Decimal256 OneMillion = 1_000_000;
        public static Decimal256 OneBillion = OneMillion * 1000;
        public static Decimal256 OneTrillion = OneBillion * 1000;

        public static Decimal256 InitialPrice_Clickdamage =                          50;
        public static Decimal256 InitialPrice_DaggerDamage =                        300;
        public static Decimal256 InitialPrice_GoldValue =                         3_100;
        public static Decimal256 InitialPrice_DaggerCd =                         42_000;
        public static Decimal256 InitialPrice_WitchDoctor =                     750_000;
        public static Decimal256 InitialPrice_GoldPerKnifeThrown =           10_100_000;
        public static Decimal256 InitialPrice_Wizard =                      150_000_000;
        public static Decimal256 InitialPrice_Hoarder =                   2_100_000_000;
        public static Decimal256 InitialPrice_ZapDamage =                28_500_000_000;
        public static Decimal256 InitialPrice_MoneyMaker =              500_000_000_000;
        public static Decimal256 InitialPrice_DaggerMaster =         11_000_000_000_000;

        // billion :     1_000_000_000
        // trillion: 1_000_000_000_000

        public static Decimal256 InitialPrice_Clickdamage_X2 = InitialPrice_Clickdamage * 10;
        public static Decimal256 InitialPrice_DaggerDamage_X2 = InitialPrice_DaggerDamage * 10;
        public static Decimal256 InitialPrice_GoldValue_X2 = InitialPrice_GoldValue * 10;
        public static Decimal256 InitialPrice_DaggerCd_X2 = InitialPrice_DaggerCd * 10;
        public static Decimal256 InitialPrice_WitchDoctor_X2 = InitialPrice_WitchDoctor * 10;
        public static Decimal256 InitialPrice_GoldPerKnifeThrown_X2 = InitialPrice_GoldPerKnifeThrown * 10;
        public static Decimal256 InitialPrice_Wizard_X2 = InitialPrice_Wizard * 10;
        public static Decimal256 InitialPrice_Hoarder_X2 = InitialPrice_Hoarder * 10;
        public static Decimal256 InitialPrice_ZapDamage_X2 = InitialPrice_ZapDamage * 10;
        public static Decimal256 InitialPrice_MoneyMaker_X2 = InitialPrice_MoneyMaker * 10;
        public static Decimal256 InitialPrice_DaggerMaster_X2 = InitialPrice_DaggerMaster * 10;

        public static Decimal256 BaseIncome_Clickdamage = 0.2;
        public static Decimal256 BaseIncome_DaggerDamage = 2;
        public static Decimal256 BaseIncome_GoldValue = 8;
        public static Decimal256 BaseIncome_DaggerCd = 47;
        public static Decimal256 BaseIncome_WitchDoctor = 260;
        public static Decimal256 BaseIncome_GoldPerKnifeThrown = 1_400;
        public static Decimal256 BaseIncome_Wizard = 7_800;
        public static Decimal256 BaseIncome_Hoarder = 44_000;
        public static Decimal256 BaseIncome_ZapDamage = 255_000;
        public static Decimal256 BaseIncome_MoneyMaker = 1_500_000;
        public static Decimal256 BaseIncome_DaggerMaster = 10_000_000;

        public static long DiamondsForMonsterCredits(long monsterCredits)
        {
            return monsterCredits;
        }

        public static Decimal256 MonsterCreditXpForNextLevel(long level)
        {
            // billion: 2, 6, 18, 38, 66, 102, 146, 198, 258, 326, 402, 486, 578, 678, 786, 902, 1026, 1158, 1298, 1446
            Decimal256 xp = 2 * OneBillion * (1 + 2 * (level - 1) * (level - 1));
            return xp;
        }

        public static Decimal256 PriceX2(Decimal256 initialPrice, long levelX2)
        {
            Decimal256 result = initialPrice * Math.Pow(10, levelX2);
            return result;
        }

        public static long LevelRequirementX2(long levelX2)
        {
            // Copied from Cookie Clicker
            // 1: level 1
            // 2: level 5
            // 3: level 25
            // 4: level 50
            // 5: ...from here it is +50 per level
            if (levelX2 == 0) return 0;
            if (levelX2 == 1) return 1;
            if (levelX2 == 2) return 5;
            if (levelX2 == 3) return 25;

            // > 3
            return (levelX2 - 3) * 50;
        }

        public static Decimal256 PriceNextPercentageBonus(long level)
        {
            // Claude
            //
            // Level 4: ~622,500
            // Level 10: ~1,900,000
            // Level 50: ~75,000,000
            // Level 100: ~1,000,000,000

            if (level == 0) return 500;
            if (level == 1) return 10_000;
            if (level == 2) return 100_000;
            if (level == 3) return 250_000;

            // Geometric growth: base_price * growth_rate^(level - 4)
            // Starting from 500,000 at level 3, with growth rate of ~1.245
            // This reaches approximately 1 billion at level 100
            // You can adjust the growthRate value slightly if you want to
            // fine-tune the final amount - increasing it will make level 100
            // cost more, decreasing it will make it cost less.
            Decimal256 basePrice = 500_000;
            double growthRate = 1.245;
            double exponent = level - 3;

            return basePrice * (Decimal256)Math.Pow(growthRate, exponent);
        }
    }
}
