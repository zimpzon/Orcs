using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class KnifeCdManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelKnifeCd;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeKnifeCd;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();
            long cdReductionNow = (long)(ClampLevel(level) * (100.0 / MaxLevel));
            long cdReductionNext = (long)((ClampLevel(level + 1)) * (100.0 / MaxLevel));

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelKnifeCd,
                SaveGame.Members.LevelKnifeCdX2,
                UpgradeProgression.InitialPrice_DaggerCd_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Dagger Cooldown</color></b></size>");
            sb.AppendLine("<color=#dddddd>Throw daggers faster.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format512.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format512.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format512.Format(earnedSoFar)}</color>.");

            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income X2</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-PASSIVE>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine($"<color=#dddddd>Price: <color={colorX2PriceMet}>${Format512.Format(priceX2)}");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Dagger CD reduction: <color=COLOR-ARENA>{cdReductionNow}%</color>");
            sb.AppendLine($"<color=#dddddd>Level: <color=COLOR-ARENA>{ClampLevel(level)} / {MaxLevel}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{(level >= MaxLevel ? "<color=#DFA0A0>max reached" : $"{cdReductionNext}%")}</color>");

            return sb.ToString();
        }

        private const int MaxLevel = 50;
        private const double EndValueValue = 0.1;
        private const double StartValue = 0.4;

        private static long ClampLevel(long level)
            => level > MaxLevel ? MaxLevel : level;

        private static float ValueForLevel(long level)
        {
            level = ClampLevel(level);

            double Step = (StartValue - EndValueValue) / MaxLevel;
            double value = StartValue - level * Step;
            return (float)value;
        }

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_DaggerCd;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelKnifeCdX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelKnifeCd;

        public static Decimal512 PriceForNext()
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
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelKnifeCd++;
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_DaggerCd_X2, SaveGame.Members.LevelKnifeCdX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelKnifeCdX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelKnifeCd,
                SaveGame.Members.LevelKnifeCdX2,
                UpgradeProgression.InitialPrice_DaggerCd_X2);

            UpgradeManager.Instance.KnifeCd.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelKnifeCd);
        }
    }
}
