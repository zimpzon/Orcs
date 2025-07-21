using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class ClickDamageManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelClickDamage;
            double baseIncome = BaseIncome();
            double totalIncome = PassiveIncome();
            long currentValue = ValueForLevel(level);
            long nextValue = ValueForLevel(level + 1);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Zap Damage</color></b></size>");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=yellow>${baseIncome:F1}</color>/sec.");
            sb.AppendLine($"<color=#dddddd>Current: <color=yellow>${totalIncome:F1}</color>/sec.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current zap damage: <color=yellow>{currentValue}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=yellow>{nextValue}</color>");

            return sb.ToString();
        }

        private static long ValueForLevel(long level)
            => 10 + (1 * level);

        private static double BaseIncome() => 1;
        
        public static double PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelClickDamage;

        public static long PriceForNext()
        {
            return (long)(100 * Math.Pow(1.15, SaveGame.Members.LevelClickDamage));
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.ClickDamage = ValueForLevel(SaveGame.Members.LevelClickDamage);
        }

        public static void OnBuy()
        {
            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            SaveGame.Members.Money -= priceForNext;
            SaveGame.Members.LevelClickDamage++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.ClickDamage.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelClickDamage);
        }
    }
}
