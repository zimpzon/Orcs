using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class HoarderManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelHoarder;
            Decimal256 earnedSoFar = SaveGame.Members.TotalIncomeHoarder;
            Decimal256 baseIncome = BaseIncome();
            Decimal256 totalIncome = PassiveIncome();

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>$ Collector</color></b></size>");
            sb.AppendLine("<color=#dddddd>Steady, healty income.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format256.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format256.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format256.Format(earnedSoFar)}</color>.");

            return sb.ToString();
        }

        private static Decimal256 BaseIncome() => UpgradeProgression.BaseIncome_Hoarder;

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelHoarder;

        public static Decimal256 PriceForNext()
        {
            return UpgradeProgression.InitialPrice_Hoarder * Math.Pow(1.15, SaveGame.Members.LevelHoarder);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            // Nothing, just $$
        }

        public static void OnBuy()
        {
            Decimal256 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelHoarder++;
        }

        public static void UpdateUi()
        {
            Decimal256 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.Hoarder.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelHoarder);
        }
    }
}
