using Assets.Script.Misc;
using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class PowerZapManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelPowerZap;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomePowerZap;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelPowerZap,
                SaveGame.Members.LevelPowerZapX2,
                UpgradeProgression.InitialPrice_PowerZap_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Power Zap</color></b></size>");
            sb.AppendLine("<color=#dddddd>Zaps jump to one more enemy and hit much harder.");
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
            sb.AppendLine($"<color=#dddddd>Extra zap jumps: <color=COLOR-ARENA>+{ExtraJumpsForLevel(Math.Max(1, level))}</color>");
            sb.AppendLine($"<color=#dddddd>Zap damage: <color=COLOR-ARENA>x{DisplayNumberFormat.Format(DamageMulForLevel(level), "0.0")}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>x{DisplayNumberFormat.Format(DamageMulForLevel(level + 1), "0.0")}</color>");

            return sb.ToString();
        }

        // Zap damage multiplier, applied only to actual zaps (Zapper.TryZapEnemy: player chain zap and Witch Doctor
        // corpse zaps), not to things derived from zap damage like Wizard fireballs. +50% per level, multiplicative -
        // stronger per level than broad tiers like Frenzy since it only boosts zaps, which fall behind late game.
        private static double DamageMulForLevel(long level)
            => 1.0 + 0.5 * Math.Max(level, 0);

        private static int ExtraJumpsForLevel(long level)
            => level > 0 ? 1 : 0;

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_PowerZap;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelPowerZapX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelPowerZap;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_PowerZap, SaveGame.Members.LevelPowerZap, SaveGame.Members.LevelPowerZapX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            long level = SaveGame.Members.LevelPowerZap;
            PlayerUpgrades.Data.PowerZapExtraJumps = ExtraJumpsForLevel(level);
            PlayerUpgrades.Data.PowerZapDamageMul = DamageMulForLevel(level);
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelPowerZap += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelPowerZap, SaveGame.Members.LevelPowerZapX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_PowerZap_X2, SaveGame.Members.LevelPowerZapX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelPowerZapX2++;
        }

        public static void UpdateUi()
        {
            if (UpgradeManager.Instance.PowerZap == null)
                return;

            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelPowerZap,
                SaveGame.Members.LevelPowerZapX2,
                UpgradeProgression.InitialPrice_PowerZap_X2);

            UpgradeManager.Instance.PowerZap.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelPowerZap);
        }
    }
}
