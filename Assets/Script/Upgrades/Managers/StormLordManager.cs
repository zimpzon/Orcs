using Assets.Script.Misc;
using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class StormLordManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelStormLord;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeStormLord;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelStormLord,
                SaveGame.Members.LevelStormLordX2,
                UpgradeProgression.InitialPrice_StormLord_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Storm Lord</color></b></size>");
            sb.AppendLine("<color=#dddddd>Lightning storms strike random enemies from above.");
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
            sb.AppendLine($"<color=#dddddd>A storm every <color=COLOR-ARENA>{StormInterval:0}</color> seconds.");
            sb.AppendLine($"<color=#dddddd>Strikes: <color=COLOR-ARENA>{StrikesForLevel(level)}</color>, damage: <color=COLOR-ARENA>x{DisplayNumberFormat.Format(DamageMulForLevel(level), "0.0")}</color> zap damage");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{StrikesForLevel(level + 1)}</color> strikes, <color=COLOR-ARENA>x{DisplayNumberFormat.Format(DamageMulForLevel(level + 1), "0.0")}</color>");

            return sb.ToString();
        }

        // Every StormInterval seconds of arena time, StrikesForLevel bolts hit random enemies for zap damage times
        // DamageMulForLevel (GameManager.CheckStorm). Strong from level 1 since it's the final tier.
        public const float StormInterval = 4f;

        private static int StrikesForLevel(long level)
            => level <= 0 ? 0 : (int)Math.Min(8, 2 + level / 25);

        private static double DamageMulForLevel(long level)
            => level <= 0 ? 0 : 2.0 + 0.5 * level;

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_StormLord;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelStormLordX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelStormLord;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_StormLord, SaveGame.Members.LevelStormLord, SaveGame.Members.LevelStormLordX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.StormLordStrikes = StrikesForLevel(SaveGame.Members.LevelStormLord);
            PlayerUpgrades.Data.StormLordDamageMul = DamageMulForLevel(SaveGame.Members.LevelStormLord);
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelStormLord += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelStormLord, SaveGame.Members.LevelStormLordX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_StormLord_X2, SaveGame.Members.LevelStormLordX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelStormLordX2++;
        }

        public static void UpdateUi()
        {
            if (UpgradeManager.Instance.StormLord == null)
                return;

            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelStormLord,
                SaveGame.Members.LevelStormLordX2,
                UpgradeProgression.InitialPrice_StormLord_X2);

            UpgradeManager.Instance.StormLord.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelStormLord);
        }
    }
}
