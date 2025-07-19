using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class KnifeCdManager
    {
        public static string GetText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Cooldown between each knife in seconds.");
            sb.AppendLine("");
            sb.AppendLine("Current:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelKnifeCooldown):0.0}</color>");
            sb.AppendLine("Next:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelKnifeCooldown + 1):0.0}</color>");
            return sb.ToString();
        }

        public static long PriceForNext()
        {
            return 10 + (long)Math.Pow(SaveGame.Members.LevelKnifeCooldown, 2.8f);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        private static float ValueForLevel(long level)
        {
            float value = 2.0f - level * 0.1f;
            value = Math.Clamp(value, 0.1f, 100);
            return value;
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MagicMissileBaseCd = ValueForLevel(SaveGame.Members.LevelKnifeCooldown);
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.KnifeCd.SetCanAfford(canAfford, priceForNext, SaveGame.Members.LevelKnifeCooldown);
        }
    }
}
