using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class KnifeDamageManager
    {
        public static string GetText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Damage done by each dagger.");
            sb.AppendLine("");
            sb.AppendLine("Current:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelKnifeDamage):0.0}</color>");
            sb.AppendLine("Next:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelKnifeDamage + 1):0.0}</color>");
            return sb.ToString();
        }

        private static long ValueForLevel(long level)
            => 10 + (10 * level);

        public static double PassiveIncome()
            => 2 * SaveGame.Members.LevelKnifeDamage;

        public static long PriceForNext()
        {
            return 150 + (long)Math.Pow(SaveGame.Members.LevelKnifeDamage, 2.5f);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MagicMissileBaseDamage = ValueForLevel(SaveGame.Members.LevelKnifeDamage);
        }

        public static void OnBuy()
        {
            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            SaveGame.Members.Money -= priceForNext;
            SaveGame.Members.LevelKnifeDamage++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.KnifeDamage.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelKnifeDamage);
        }
    }
}
