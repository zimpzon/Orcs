using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class MoneyMakerManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelMoneyMaker;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeMoneyMaker;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();
            long currentValue = (long)(ValueForLevel(level) * 100.0);
            long nextValue = (long)(ValueForLevel(level + 1) * 100.0);

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelMoneyMaker,
                SaveGame.Members.LevelMoneyMakerX2,
                UpgradeProgression.InitialPrice_MoneyMaker_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Necromancer</color></b></size>");
            sb.AppendLine("<color=#dddddd>Throws aggressive reanimated skulls biting for Dagger damage.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-ARENA>${Format512.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-ARENA>${Format512.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-ARENA>${Format512.Format(earnedSoFar)}</color>.");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(PriceForNext()));

            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income X2</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-ARENA>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(priceX2));
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Necromancer Dagger damage: <color=COLOR-ARENA>{currentValue}%</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{(nextValue.ToString())}%</color>");
            sb.AppendLine("");
            sb.AppendLine($"<color=#dddddd>Total damage: <color=COLOR-ARENA>{Format512.Format(SaveGame.Members.TotalDamageNecromancer)}</color>");
            return sb.ToString();
        }

        private static double ValueForLevel(long level)
            => 1 + 0.25 * (level - 1);

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_MoneyMaker;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelMoneyMakerX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelMoneyMaker;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_MoneyMaker, SaveGame.Members.LevelMoneyMaker, SaveGame.Members.LevelMoneyMakerX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.NecromancerEnabled = SaveGame.Members.LevelMoneyMaker > 0;
            PlayerUpgrades.Data.NecromancerBaseDamage =
                PlayerUpgrades.Data.MagicMissileEffectiveDamage * ValueForLevel(SaveGame.Members.LevelMoneyMaker);
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelMoneyMaker += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelMoneyMaker, SaveGame.Members.LevelMoneyMakerX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_MoneyMaker_X2, SaveGame.Members.LevelMoneyMakerX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelMoneyMakerX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelMoneyMaker,
                SaveGame.Members.LevelMoneyMakerX2,
                UpgradeProgression.InitialPrice_MoneyMaker_X2);

            UpgradeManager.Instance.MoneyMaker.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelMoneyMaker);
        }
    }
}
