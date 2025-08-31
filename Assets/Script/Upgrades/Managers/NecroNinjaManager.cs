using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class NecroNinjaManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelNecroNinja;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeNecroNinja;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();
            long currentValue = (long)(ValueForLevel(level) * 100.0);
            long nextValue = (long)(ValueForLevel(level + 1) * 100.0);

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelNecroNinja,
                SaveGame.Members.LevelNecroNinjaX2,
                UpgradeProgression.InitialPrice_NecroNinja_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Necro Ninja</color></b></size>");
            sb.AppendLine("<color=#dddddd>Skulls will furiously seek out nearby enemies.");
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

            return sb.ToString();
        }

        private static double ValueForLevel(long level)
            => 1 + 0.25 * (level - 1);

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_NecroNinja;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelNecroNinjaX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelNecroNinja;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.InitialPrice_NecroNinja * Math.Pow(1.15, SaveGame.Members.LevelNecroNinja);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.NecromancerAggressiveSkulls = SaveGame.Members.LevelNecroNinja > 0;
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelNecroNinja++;
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_NecroNinja_X2, SaveGame.Members.LevelNecroNinjaX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelNecroNinjaX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelNecroNinja,
                SaveGame.Members.LevelNecroNinjaX2,
                UpgradeProgression.InitialPrice_NecroNinja_X2);

            UpgradeManager.Instance.NecroNinja.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelNecroNinja);
        }
    }
}
