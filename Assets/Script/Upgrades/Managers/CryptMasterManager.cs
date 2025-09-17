using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class CryptMasterManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelCryptMaster;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeCryptMaster;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelCryptMaster,
                SaveGame.Members.LevelCryptMasterX2,
                UpgradeProgression.InitialPrice_CryptMaster_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Crypt Master</color></b></size>");
            sb.AppendLine("<color=#dddddd>A fourth skull is summoned from the depths of the crypt.");
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

            return sb.ToString();
        }


        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_CryptMaster;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelCryptMasterX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelCryptMaster;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_CryptMaster, SaveGame.Members.LevelCryptMaster, SaveGame.Members.LevelCryptMasterX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelCryptMaster += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelCryptMaster, SaveGame.Members.LevelCryptMasterX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_CryptMaster_X2, SaveGame.Members.LevelCryptMasterX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelCryptMasterX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelCryptMaster,
                SaveGame.Members.LevelCryptMasterX2,
                UpgradeProgression.InitialPrice_CryptMaster_X2);

            UpgradeManager.Instance.CryptMaster.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelCryptMaster);
        }
    }
}
