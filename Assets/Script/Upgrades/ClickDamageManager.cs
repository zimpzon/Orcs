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

            sb.AppendLine("<size=+2>Zap Damage</size>");
            sb.AppendLine("");
            sb.AppendLine($"Each level earns <color=#00ffff>${baseIncome:F1}</color>/sec.");
            sb.AppendLine($"Current: <color=#00ffff>${totalIncome:F1}</color>/sec.");
            sb.AppendLine("");
            sb.AppendLine($"Current zap damage: <color=#ffff00>{currentValue}</color>");
            sb.AppendLine($"Next: <color=#ffff00>{(nextValue == -1 ? "max reached" : nextValue)}</color>");

            return sb.ToString();
        }

        private static long ValueMaxLevel(long level)
            => 3;

        private static long ValueForLevel(long level)
            => level >= ValueMaxLevel(level) ? -1 : 5 + level;

        private static double BaseIncome() => 0.1;
        
        public static double PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelClickDamage;

        public static long PriceForNext()
        {
            return 30 + (long)Math.Pow(SaveGame.Members.LevelClickDamage, 2.5f);
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
