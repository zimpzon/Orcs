using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class ClickDamageManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelClickDamage;
            Decimal256 earnedSoFar = SaveGame.Members.TotalIncomeClickDamage;
            Decimal256 baseIncome = BaseIncome();
            Decimal256 totalIncome = PassiveIncome();
            long currentValue = ValueForLevel(level);
            long nextValue = ValueForLevel(level + 1);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Chain Zapping</color></b></size>");
            sb.AppendLine("<color=#dddddd>Zaps enemies every 3 seconds.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format256.FormatWithDecimals(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format256.FormatWithDecimals(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format256.FormatWithDecimals(earnedSoFar)}</color>.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current zap damage: <color=COLOR-ARENA>{currentValue}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{nextValue}</color>");

            return sb.ToString();
        }

        private static long ValueForLevel(long level)
        {
            if (level == 0)
                return 0;

            return 50 * level;
        }

        private static Decimal256 BaseIncome() => UpgradeProgression.BaseIncome_Clickdamage;
        
        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelClickDamage;

        public static Decimal256 PriceForNext()
        {
            return (UpgradeProgression.InitialPrice_Clickdamage * Math.Pow(1.15, SaveGame.Members.LevelClickDamage));
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.ZapDamage = ValueForLevel(SaveGame.Members.LevelClickDamage);
        }

        public static void OnBuy()
        {
            Decimal256 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelClickDamage++;
        }

        public static void UpdateUi()
        {
            Decimal256 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            UpgradeManager.Instance.ClickDamage.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelClickDamage);
        }
    }
}
