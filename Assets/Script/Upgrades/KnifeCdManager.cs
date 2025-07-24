using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class KnifeCdManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelKnifeCd;
            Decimal256 earnedSoFar = SaveGame.Members.TotalIncomeKnifeCd;
            Decimal256 baseIncome = BaseIncome();
            Decimal256 totalIncome = PassiveIncome();
            float currentValue = ValueForLevel(level);
            float nextValue = ValueForLevel(level + 1);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Dagger Cooldown</color></b></size>");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=green>${baseIncome}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=green>${totalIncome}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=green>${Format256.Format(earnedSoFar)}</color>.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current CD: <color=green>{currentValue:0.000}s</color>");
            sb.AppendLine($"<color=#dddddd>Level: <color=green>{level} / {MaxLevel}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=green>{(level >= MaxLevel ? "<color=red>max reached" : $"{nextValue:0.000}s")}</color>");

            return sb.ToString();
        }

        private const int MaxLevel = 50;
        private const double EndValueValue = 0.1;
        private const double StartValue = 0.4;

        private static float ValueForLevel(long level)
        {
            double Step = (StartValue - EndValueValue) / MaxLevel;
            double value = StartValue - level * Step;
            return (float)value;
        }

        private static Decimal256 BaseIncome() => 8;

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelKnifeCd;

        public static long PriceForNext()
        {
            return (long)(12_000 * Math.Pow(1.15, SaveGame.Members.LevelKnifeCd));
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MagicMissileBaseCd = ValueForLevel(SaveGame.Members.LevelKnifeCd);
        }

        public static void OnBuy()
        {
            if (SaveGame.Members.LevelKnifeCd >= MaxLevel)
                return;

            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelKnifeCd++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.KnifeCd.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelKnifeCd);
        }
    }
}
