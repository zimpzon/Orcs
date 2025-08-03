using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class MoneyMakerManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelMoneyMaker;
            Decimal256 earnedSoFar = SaveGame.Members.TotalIncomeMoneyMaker;
            Decimal256 baseIncome = BaseIncome();
            Decimal256 totalIncome = PassiveIncome();
            long currentValue = (long)(ValueForLevel(level) * 100.0);
            long nextValue = (long)(ValueForLevel(level + 1) * 100.0);

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelMoneyMaker,
                SaveGame.Members.LevelMoneyMakerX2,
                UpgradeProgression.InitialPrice_MoneyMaker_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal256 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Necromancer</color></b></size>");
            sb.AppendLine("<color=#dddddd>Throws aggressive reanimated skulls bitin for Dagger damage.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format256.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format256.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format256.Format(earnedSoFar)}</color>.");

            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income X2</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-PASSIVE>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine($"<color=#dddddd>Price: <color={colorX2PriceMet}>${Format256.Format(priceX2)}");
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Necromancer Dagger damage: <color=COLOR-ARENA>{currentValue}%</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{(nextValue.ToString())}%</color>");
            sb.AppendLine("");
            sb.AppendLine($"<color=#dddddd>Total damage: <color=COLOR-ARENA>{Format256.Format(SaveGame.Members.TotalDamageNecromancer)}</color>");
            return sb.ToString();
        }

        private static double ValueForLevel(long level)
            => 1 + 0.25 * (level - 1);

        private static Decimal256 BaseIncome()
        {
            Decimal256 baseIncome = UpgradeProgression.BaseIncome_MoneyMaker;
            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelMoneyMakerX2);
            return baseIncome;
        }

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelMoneyMaker;

        public static Decimal256 PriceForNext()
        {
            return UpgradeProgression.InitialPrice_MoneyMaker * Math.Pow(1.15, SaveGame.Members.LevelMoneyMaker);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.NecromancerEnabled = SaveGame.Members.LevelMoneyMaker > 0;
            PlayerUpgrades.Data.NecromancerEffectiveDamage =
                PlayerUpgrades.Data.MagicMissileEffectiveDamage * ValueForLevel(SaveGame.Members.LevelMoneyMaker);
        }

        public static void OnBuy()
        {
            Decimal256 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelMoneyMaker++;
        }

        public static void OnBuyX2()
        {
            Decimal256 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_MoneyMaker_X2, SaveGame.Members.LevelMoneyMakerX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelMoneyMakerX2++;
        }

        public static void UpdateUi()
        {
            Decimal256 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelMoneyMaker,
                SaveGame.Members.LevelMoneyMakerX2,
                UpgradeProgression.InitialPrice_MoneyMaker_X2);

            UpgradeManager.Instance.MoneyMaker.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelMoneyMaker);
        }
    }
}
