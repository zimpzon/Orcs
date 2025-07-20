using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class KnifeCdManager
    {
        public static string GetText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Cooldown between each dagger in seconds.");
            sb.AppendLine("");
            sb.AppendLine("Current:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelKnifeCooldown):0.00}</color>");
            sb.AppendLine("Next:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelKnifeCooldown + 1):0.00}</color>");
            return sb.ToString();
        }

        public static long PriceForNext()
        {
            return 100 + (long)Math.Pow(SaveGame.Members.LevelKnifeCooldown, 2.5f);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        private static float ValueForLevel(long level)
        {
            float value = 1.0f - level * 0.05f;
            value = Math.Clamp(value, 0.1f, 100);
            return value;
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MagicMissileBaseCd = ValueForLevel(SaveGame.Members.LevelKnifeCooldown);
        }

        public static void OnBuy()
        {
            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            SaveGame.Members.Money -= priceForNext;
            SaveGame.Members.LevelKnifeCooldown++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.KnifeCd.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelKnifeCooldown, maxLevel: 20);
        }
    }
}
