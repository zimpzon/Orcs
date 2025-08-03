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

        public static Decimal256 BaseIncome_Clickdamage = 0.2;
        public static Decimal256 BaseIncome_DaggerDamage = 2;
        public static Decimal256 BaseIncome_GoldValue = 8;
        public static Decimal256 BaseIncome_DaggerCd = 47;
        public static Decimal256 BaseIncome_WitchDoctor = 260;
        public static Decimal256 BaseIncome_GoldPerKnifeThrown = 1_400;
        public static Decimal256 BaseIncome_Wizard = 7_800;
        public static Decimal256 BaseIncome_Hoarder = 44_000;
        public static Decimal256 BaseIncome_ZapDamage = 246_000;
        public static Decimal256 BaseIncome_MoneyMaker = 1_230_000;

        public static Decimal256 AscendXpForNextLevel(long level)
        {
            Decimal256 xpFirstLevel = 1_000_000_000;
            return xpFirstLevel * (level + 1);
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
    }
}
