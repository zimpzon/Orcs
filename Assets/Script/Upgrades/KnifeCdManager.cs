using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class KnifeCdManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelKnifeCooldown;
            double baseIncome = BaseIncome();
            double totalIncome = PassiveIncome();
            float currentValue = ValueForLevel(level);
            float nextValue = ValueForLevel(level + 1);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Dagger Cooldown</color></b></size>");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=yellow>${baseIncome:F1}</color>/sec.");
            sb.AppendLine($"<color=#dddddd>Current: <color=yellow>${totalIncome:F1}</color>/sec.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current CD: <color=yellow>{currentValue:0.00}s</color>");
            sb.AppendLine($"<color=#dddddd>Level: <color=yellow>{level} / {MaxLevel}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=yellow>{(level >= MaxLevel ? "max reached" : $"{nextValue:0.00}s")}</color>");

            return sb.ToString();
        }

        private const int MaxLevel = 19;
        private const double EndValueValue = 0.05;
        private const double StartValue = 0.5;

        private static float ValueForLevel(long level)
        {
            double Step = (StartValue - EndValueValue) / MaxLevel;
            double value = StartValue - level * Step;
            return (float)value;
        }

        private static double BaseIncome() => 250;

        public static double PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelKnifeCooldown;

        public static long PriceForNext()
        {
            return (long)(12_000 * Math.Pow(1.15, SaveGame.Members.LevelKnifeCooldown));
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MagicMissileBaseCd = ValueForLevel(SaveGame.Members.LevelKnifeCooldown);
        }

        public static void OnBuy()
        {
            if (SaveGame.Members.LevelKnifeCooldown >= MaxLevel)
                return;

            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelKnifeCooldown++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.KnifeCd.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelKnifeCooldown);
        }
    }
}
