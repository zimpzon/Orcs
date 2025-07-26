using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class ArenaGoldManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelMoneyPerGold;
            Decimal256 earnedSoFar = SaveGame.Members.TotalIncomeGoldPerRound;
            Decimal256 baseIncome = BaseIncome();
            Decimal256 totalIncome = PassiveIncome();
            long currentValue = (long)(ValueForLevel(level) * 100);
            long nextValue = (long)(ValueForLevel(level + 1) * 100);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Gold Value</color></b></size>");
            sb.AppendLine("<color=#dddddd>Get more $ from gold.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format256.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format256.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format256.Format(earnedSoFar)}</color>.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Gold value: <color=COLOR-ARENA>{currentValue}%</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{nextValue}%</color>");

            return sb.ToString();
        }

        private static double ValueForLevel(long level)
            => 1 + 0.25 * (level - 1);

        private static Decimal256 BaseIncome() => UpgradeProgression.BaseIncome_GoldValue;

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelMoneyPerGold;

        public static Decimal256 PriceForNext()
        {
            return (UpgradeProgression.InitialPrice_GoldValue * Math.Pow(1.15, SaveGame.Members.LevelMoneyPerGold));
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MoneyPerGold = ValueForLevel(SaveGame.Members.LevelMoneyPerGold);
        }

        public static void OnBuy()
        {
            Decimal256 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelMoneyPerGold++;
        }

        public static void UpdateUi()
        {
            Decimal256 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.GoldPerRound.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelMoneyPerGold);
        }
    }
}
