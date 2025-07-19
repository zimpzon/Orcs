using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class KnifeDamageManager
    {
        public static string GetText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Damage done by each knife.");
            sb.AppendLine("");
            sb.AppendLine("Current:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelKnifeDamage):0.0}</color>");
            sb.AppendLine("Next:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelKnifeDamage + 1):0.0}</color>");
            return sb.ToString();
        }

        public static long PriceForNext()
        {
            return 10 + (long)Math.Pow(SaveGame.Members.LevelKnifeDamage, 2.8f);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        private static float ValueForLevel(long level)
            => 10 + level * 1.5f;


        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MagicMissileBaseDamage = ValueForLevel(SaveGame.Members.LevelKnifeDamage);
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.KnifeDamage.SetCanAfford(canAfford, priceForNext, SaveGame.Members.LevelKnifeDamage);
        }
    }
}
