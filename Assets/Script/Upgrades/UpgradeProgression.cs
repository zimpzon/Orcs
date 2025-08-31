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

        public static Decimal512 InitialPrice_Clickdamage =                          50;
        public static Decimal512 InitialPrice_DaggerDamage =                        300;
        public static Decimal512 InitialPrice_GoldValue =                         3_100;
        public static Decimal512 InitialPrice_DaggerCd =                         42_000;
        public static Decimal512 InitialPrice_WitchDoctor =                     750_000;
        public static Decimal512 InitialPrice_GoldPerKnifeThrown =           10_100_000;
        public static Decimal512 InitialPrice_Wizard =                      150_000_000;
        public static Decimal512 InitialPrice_Hoarder =                   2_100_000_000;
        public static Decimal512 InitialPrice_ZapDamage =                20_500_000_000;
        public static Decimal512 InitialPrice_MoneyMaker =              210_000_000_000;
        public static Decimal512 InitialPrice_DaggerMaster =          2_300_000_000_000;
        public static Decimal512 InitialPrice_NecroNinja =           21_100_000_000_000;
        public static Decimal512 InitialPrice_SkullCrusher =        330_100_000_000_000;
        public static Decimal512 InitialPrice_ChestMaster =       6_500_100_000_000_000;
        public static Decimal512 InitialPrice_Voidgazer =       250_000_000_000_000_000;
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

        public static Decimal512 BaseIncome_Clickdamage = 0.2;
        public static Decimal512 BaseIncome_DaggerDamage = 2;
        public static Decimal512 BaseIncome_GoldValue = 8;
        public static Decimal512 BaseIncome_DaggerCd = 47;
        public static Decimal512 BaseIncome_WitchDoctor = 260;
        public static Decimal512 BaseIncome_GoldPerKnifeThrown = 1_400;
        public static Decimal512 BaseIncome_Wizard = 7_800;
        public static Decimal512 BaseIncome_Hoarder = 44_000;
        public static Decimal512 BaseIncome_ZapDamage = 255_000;
        public static Decimal512 BaseIncome_MoneyMaker =            1_500_000;
        public static Decimal512 BaseIncome_DaggerMaster =         10_000_000;
        public static Decimal512 BaseIncome_NecroNinja =           62_000_000;
        public static Decimal512 BaseIncome_SkullCrusher =        300_000_000;
        public static Decimal512 BaseIncome_ChestMaster =       1_400_000_000;
        public static Decimal512 BaseIncome_Voidgazer =         5_100_000_000;

        public static long DiamondsForMonsterCredits(long monsterCredits)
        {
            return monsterCredits;
        }

        public static Decimal512 MonsterCreditXpForNextLevel(long level)
        {
            // Original progression for levels 1-8: 5B, 15B, 45B, 95B, 165B, 255B, 365B, 495B
            // Steep cubic growth starting from level 9
            long n = level - 1;
            if (level <= 8)
            {
                // Original quadratic curve for first 8 levels
                return 5 * OneBillion * (1 + 2 * n * n);
            }
            else
            {
                // Steep cubic growth starting from level 9
                // Base XP at level 8 is 495B
                Decimal512 baseXp = 495 * OneBillion;
                long d = level - 8;
                Decimal512 extraXp = 58.0 * OneBillion * d * d * d; // Cubic growth
                return baseXp + extraXp;
            }
        }

        // Comparison with original:
        // Level 20: Original ~25000B, New ~31250B (+25%)
        // Level 30: Original ~82815B, New ~103519B (+25%) 
        // Level 50: Original ~571815B, New ~714769B (+25%)
        public static Decimal512 PriceX2(Decimal512 initialPrice, long levelX2)
        {
            Decimal512 result = initialPrice * Math.Pow(3, levelX2);
            return result;
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
