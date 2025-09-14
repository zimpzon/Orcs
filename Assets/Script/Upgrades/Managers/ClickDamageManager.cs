using Assets.Script.Upgrades.Managers;
using System;
using System.Text;
using UnityEngine.UI.Extensions.Examples;

namespace Assets.Script.Upgrades
{
    public static class ClickDamageManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelClickDamage;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeClickDamage;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();
            long currentValue = ValueForLevel(level);
            long nextValue = ValueForLevel(level + 1);

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelClickDamage,
                SaveGame.Members.LevelClickDamageX2,
                UpgradeProgression.InitialPrice_Clickdamage_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Chain Zapping</color></b></size>");
            sb.AppendLine("<color=#dddddd>Zaps enemies every 3 seconds.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format512.FormatWithDecimals(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format512.FormatWithDecimals(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format512.FormatWithDecimals(earnedSoFar)}</color>.");
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income X2</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-PASSIVE>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine($"<color=#dddddd>Price: <color={colorX2PriceMet}>${Format512.Format(priceX2)}");
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current Zap damage: <color=COLOR-ARENA>{Format512.Format(currentValue)}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{Format512.Format(nextValue)}</color>");
            sb.AppendLine("");
            sb.AppendLine($"<color=#dddddd>Total damage: <color=COLOR-ARENA>{Format512.Format(SaveGame.Members.TotalDamageChainZap)}</color>");

            return sb.ToString();
        }

        private static long ValueForLevel(long level)
        {
            if (level == 0)
                return 0;

            return 50 * level;
        }

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_Clickdamage;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelClickDamageX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelClickDamage;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_Clickdamage, SaveGame.Members.LevelClickDamage, SaveGame.Members.LevelClickDamageX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.BaseZapDamage = ValueForLevel(SaveGame.Members.LevelClickDamage);
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelClickDamage += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelClickDamage, SaveGame.Members.LevelClickDamageX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_Clickdamage_X2, SaveGame.Members.LevelClickDamageX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelClickDamageX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelClickDamage,
                SaveGame.Members.LevelClickDamageX2,
                UpgradeProgression.InitialPrice_Clickdamage_X2);

            UpgradeManager.Instance.ClickDamage.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelClickDamage);
        }
    }
}
