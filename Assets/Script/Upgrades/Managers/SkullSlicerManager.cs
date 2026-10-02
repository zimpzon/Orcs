using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class SkullSlicerManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelSkullSlicer;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeSkullSlicer;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelSkullSlicer,
                SaveGame.Members.LevelSkullSlicerX2,
                UpgradeProgression.InitialPrice_SkullSlicer_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Skull Slicer</color></b></size>");
            sb.AppendLine("<color=#dddddd>Enemies with a skull attached take more damage.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns $<color=COLOR-ARENA>{Format512.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: $<color=COLOR-ARENA>{Format512.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: $<color=COLOR-ARENA>{Format512.Format(earnedSoFar)}</color>.");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(PriceForNext()));

            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income X2</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-ARENA>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(priceX2));
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Damage to skulled enemies: <color=COLOR-ARENA>x{DamageMulForLevel(level):0.0}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>x{DamageMulForLevel(level + 1):0.0}</color>");

            return sb.ToString();
        }

        // Damage multiplier for ALL damage to an enemy that currently has a skull stuck to it (ActorBase.SkullMarkedUntil,
        // applied in GameManager.DamageEnemy). +30% per level - stronger per level than Frenzy since it only affects the
        // few enemies carrying a skull.
        private static double DamageMulForLevel(long level)
            => 1.0 + 0.3 * Math.Max(level, 0);

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_SkullSlicer;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelSkullSlicerX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelSkullSlicer;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_SkullSlicer, SaveGame.Members.LevelSkullSlicer, SaveGame.Members.LevelSkullSlicerX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.SkullSlicerDamageMul = DamageMulForLevel(SaveGame.Members.LevelSkullSlicer);
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelSkullSlicer += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelSkullSlicer, SaveGame.Members.LevelSkullSlicerX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_SkullSlicer_X2, SaveGame.Members.LevelSkullSlicerX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelSkullSlicerX2++;
        }

        public static void UpdateUi()
        {
            if (UpgradeManager.Instance.SkullSlicer == null)
                return;

            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelSkullSlicer,
                SaveGame.Members.LevelSkullSlicerX2,
                UpgradeProgression.InitialPrice_SkullSlicer_X2);

            UpgradeManager.Instance.SkullSlicer.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelSkullSlicer);
        }
    }
}
