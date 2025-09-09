using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class WitchDoctorManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelWitchDoctor;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeWitchDoctor;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();
            long currentValue = (long)(ValueForLevel(level) * 100.0);
            long nextValue = (long)(ValueForLevel(level + 1) * 100.0);

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelWitchDoctor,
                SaveGame.Members.LevelWitchDoctorX2,
                UpgradeProgression.InitialPrice_WitchDoctor_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Witch Doctor</color></b></size>");
            sb.AppendLine("<color=#dddddd>Corpses zap nearby enemies, then explode.");
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
            sb.AppendLine($"<color=#dddddd>Witch Doctor Zap damage: <color=COLOR-ARENA>{currentValue}%</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{(nextValue.ToString())}%</color>");
            sb.AppendLine("");
            sb.AppendLine($"<color=#dddddd>Total damage: <color=COLOR-ARENA>{Format512.Format(SaveGame.Members.TotalDamageWitchDoctor)}</color>");

            return sb.ToString();
        }

        private static double ValueForLevel(long level)
            => 1 + 0.5 * (level - 1);

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_WitchDoctor;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelWitchDoctorX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelWitchDoctor;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_WitchDoctor, SaveGame.Members.LevelWitchDoctor);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.WitchDoctorEnabled = SaveGame.Members.LevelWitchDoctor > 0;
            PlayerUpgrades.Data.WitchDoctorEffectiveDamage =
                (long)(PlayerUpgrades.Data.EffectiveZapDamage * ValueForLevel(SaveGame.Members.LevelWitchDoctor));
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelWitchDoctor += GameManager.Instance.BuyAmount;
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_WitchDoctor_X2, SaveGame.Members.LevelWitchDoctorX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelWitchDoctorX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelWitchDoctor,
                SaveGame.Members.LevelWitchDoctorX2,
                UpgradeProgression.InitialPrice_WitchDoctor_X2);

            UpgradeManager.Instance.WitchDoctor.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelWitchDoctor);
        }
    }
}
