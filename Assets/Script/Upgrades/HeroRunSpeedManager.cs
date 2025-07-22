using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class HeroRunSpeedManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelHeroRunspeed;
            double baseIncome = BaseIncome();
            double totalIncome = PassiveIncome();
            float currentValue = ValueForLevel(level);
            float nextValue = ValueForLevel(level + 1);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Hero Run Speed</color></b></size>");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=yellow>${baseIncome:F1}</color>/sec.");
            sb.AppendLine($"<color=#dddddd>Current: <color=yellow>${totalIncome:F1}</color>/sec.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current speed: <color=yellow>{currentValue:F1}</color>");
            sb.AppendLine($"<color=#dddddd>Level: <color=yellow>{level} / {MaxLevel}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=yellow>{(level >= MaxLevel ? "max reached" : nextValue.ToString("F1"))}</color>");

            return sb.ToString();
        }

        private const int MaxLevel = 30;

        private static float ValueForLevel(long level)
            => level >= MaxLevel ? ValueForLevel(MaxLevel) : 3 + 0.1f * level;

        private static double BaseIncome() => 45;

        public static double PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelHeroRunspeed;

        public static long PriceForNext()
        {
            return (long)(12_000 * Math.Pow(1.15, SaveGame.Members.LevelHeroRunspeed));
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MoveSpeedAdd = ValueForLevel(SaveGame.Members.LevelHeroRunspeed);
        }

        public static void OnBuy()
        {
            if (SaveGame.Members.LevelHeroRunspeed >= MaxLevel)
                return;

            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelHeroRunspeed++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.HeroRunspeed.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelHeroRunspeed);
        }
    }
}
