using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class HoarderManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelHoarder;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeHoarder;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelHoarder,
                SaveGame.Members.LevelHoarderX2,
                UpgradeProgression.InitialPrice_Hoarder_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Master Wizard</color></b></size>");
            sb.AppendLine("<color=#dddddd>Two more fireballs!");
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

            return sb.ToString();
        }

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_Hoarder;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelHoarderX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelHoarder;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.InitialPrice_Hoarder * Math.Pow(1.15, SaveGame.Members.LevelHoarder);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.WizardTwoMoreFireballsEnabled = SaveGame.Members.LevelHoarder > 0;
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelHoarder++;
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_Hoarder_X2, SaveGame.Members.LevelHoarderX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelHoarderX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelHoarder,
                SaveGame.Members.LevelHoarderX2,
                UpgradeProgression.InitialPrice_Hoarder_X2);

            UpgradeManager.Instance.Hoarder.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelHoarder);
        }
    }
}
