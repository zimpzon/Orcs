using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class WizardManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelWizard;
            Decimal256 earnedSoFar = SaveGame.Members.TotalIncomeWizard;
            Decimal256 baseIncome = BaseIncome();
            Decimal256 totalIncome = PassiveIncome();
            long currentValue = (long)(ValueForLevel(level) * 100.0);
            long nextValue = (long)(ValueForLevel(level + 1) * 100.0);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Wizard</color></b></size>");
            sb.AppendLine("<color=#dddddd>High damage fireballs using zap damage.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format256.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format256.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format256.Format(earnedSoFar)}</color>.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Zap damage: <color=COLOR-ARENA>{currentValue}%</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{(nextValue.ToString())}%</color>");
            sb.AppendLine("");
            sb.AppendLine($"<color=#dddddd>Total damage: <color=COLOR-ARENA>{Format256.Format(SaveGame.Members.TotalDamageWizard)}</color>");

            return sb.ToString();
        }

        private static double ValueForLevel(long level)
            => 2 + 0.5 * (level - 1);

        private static Decimal256 BaseIncome() => UpgradeProgression.BaseIncome_Wizard;

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelWizard;

        public static Decimal256 PriceForNext()
        {
            return UpgradeProgression.InitialPrice_Wizard * Math.Pow(1.15, SaveGame.Members.LevelWizard);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.WizardEnabled = SaveGame.Members.LevelWizard > 0;
            PlayerUpgrades.Data.WizardEffectiveDamage =
                (long)(PlayerUpgrades.Data.ZapDamage * ValueForLevel(SaveGame.Members.LevelWizard));
        }

        public static void OnBuy()
        {
            Decimal256 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelWizard++;
        }

        public static void UpdateUi()
        {
            Decimal256 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.Wizard.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelWizard);
        }
    }
}
