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
            long cdReductionNow = (long)(level * (100.0 / MaxLevel));
            long cdReductionNext = (long)((level + 1) * (100.0 / MaxLevel));

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Dagger Cooldown</color></b></size>");
            sb.AppendLine("<color=#dddddd>Throw daggers faster.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format256.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format256.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format256.Format(earnedSoFar)}</color>.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Dagger CD reduction: <color=COLOR-ARENA>{cdReductionNow}%</color>");
            sb.AppendLine($"<color=#dddddd>Level: <color=COLOR-ARENA>{level} / {MaxLevel}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{(level >= MaxLevel ? "<color=red>max reached" : $"{cdReductionNext}%")}</color>");

            return sb.ToString();
        }

        private const int MaxLevel = 50;
        private const double EndValueValue = 0.1;
        private const double StartValue = 0.4;

        private static float ValueForLevel(long level)
        {
            if (level > MaxLevel)
                level = MaxLevel;

            double Step = (StartValue - EndValueValue) / MaxLevel;
            double value = StartValue - level * Step;
            return (float)value;
        }

        private static Decimal256 BaseIncome() => UpgradeProgression.BaseIncome_DaggerCd;

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelKnifeCd;

        public static Decimal256 PriceForNext()
        {
            return UpgradeProgression.InitialPrice_DaggerCd * Math.Pow(1.15, SaveGame.Members.LevelKnifeCd);
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
            Decimal256 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelKnifeCd++;
        }

        public static void UpdateUi()
        {
            Decimal256 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.KnifeCd.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelKnifeCd);
        }
    }
}
