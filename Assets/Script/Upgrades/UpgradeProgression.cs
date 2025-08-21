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
        public static Decimal256 InitialPrice_ZapDamage =                20_500_000_000;
        public static Decimal256 InitialPrice_MoneyMaker =              210_000_000_000;
        public static Decimal256 InitialPrice_DaggerMaster =          1_900_000_000_000;
        public static Decimal256 InitialPrice_NecroNinja =           14_100_000_000_000;
        public static Decimal256 InitialPrice_SkullCrusher =        260_100_000_000_000;
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
        public static Decimal256 InitialPrice_NecroNinja_X2 = InitialPrice_NecroNinja * 10;
        public static Decimal256 InitialPrice_SkullCrusher_X2 = InitialPrice_SkullCrusher * 10;

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
        public static Decimal256 BaseIncome_NecroNinja = 62_000_000;
        public static Decimal256 BaseIncome_SkullCrusher = 370_000_000;

        public static long DiamondsForMonsterCredits(long monsterCredits)
        {
            return monsterCredits;
        }

        public static Decimal256 MonsterCreditXpForNextLevel(long level)
        {
            // Updated progression: 5B, 15B, 45B, 95B, 165B, 255B, 365B, 495B, 645B, 815B, 1005B, 1215B, 1500B, 1950B,
            // 2650B, 3650B, 5000B, 6750B, 8950B, 11650B, 14900B...
            long n = level - 1;
            if (level <= 11)
            {
                // Original quadratic curve for levels 1-11
                return 5 * OneBillion * (1 + 2 * n * n);
            }
            else if (level == 12)
            {
                return 1500 * OneBillion;
            }
            else
            {
                // Exponential growth after level 12
                // Level 12: 1500B, Level 20: ~40000B
                // 1500 * multiplier^(level-12) = 40000 at level 20
                // multiplier^8 = 40000/1500 ≈ 26.67, so multiplier ≈ 1.55

                Decimal256 baseXp = 1500 * OneBillion;
                long d = level - 12; // levels beyond 12
                Decimal256 multiplier = 1.55m;
                Decimal256 growthFactor = 1m;

                // Calculate multiplier^d
                for (int i = 0; i < d; i++)
                {
                    growthFactor *= multiplier;
                }

                return baseXp * growthFactor;
            }
        }

        public static Decimal256 PriceX2(Decimal256 initialPrice, long levelX2)
        {
            Decimal256 result = initialPrice * Math.Pow(3, levelX2);
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

        public static Decimal256 PriceNextPercentageBonus(long level)
        {
            if (level == 0) return 500;
            if (level == 1) return 10_000;
            if (level == 2) return 100_000;
            if (level == 3) return 250_000;
            Decimal256 basePrice = 500_000;
            double growthRate = 1.2378; // Reduced from 1.245 to halve level 100 cost
            double exponent = level - 3;
            return basePrice * (Decimal256)Math.Pow(growthRate, exponent);
        }
    }
}
