using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class SmartFireballsManager
    {
        const float JumpRangeUnits = 8.0f;

        public static string GetText()
        {
            long level = SaveGame.Members.LevelSmartFireballs;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeSmartFireballs;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelSmartFireballs,
                SaveGame.Members.LevelSmartFireballsX2,
                UpgradeProgression.InitialPrice_SmartFireballs_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine($"<size=+4><b><color={TextColors.Green}>Angry Fireballs</color></b></size>");
            sb.AppendLine("<color=#dddddd>Fireballs do increased damage and continue on to nearby targets.");

            sb.AppendLine("<color=#bbbbbb>___________________________________</color>");
            sb.AppendLine("<size=+4><color=#aaaaff>Income</color></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns $<color=COLOR-ARENA>{Format512.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: $<color=COLOR-ARENA>{Format512.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: $<color=COLOR-ARENA>{Format512.Format(earnedSoFar)}</color>.");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(PriceForNext()));

            sb.AppendLine("<color=#aaaaaa>___________________________________</color>");
            sb.AppendLine("<size=+4><color=#aaaaff>Income X2</color></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-ARENA>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(priceX2));

            sb.AppendLine("<color=#aaaaaa>___________________________________</color>");
            sb.AppendLine("<size=+4><color=#aaaaff>Arena</color></size>");
            sb.AppendLine($"<color=#dddddd>Fireball damage: <color=COLOR-ARENA>{UpgradeManagerHelper.FormatFactor(ValueForLevel(level), ValueForLevel(level + 1))}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{UpgradeManagerHelper.FormatFactor(ValueForLevel(level + 1), ValueForLevel(level))}</color>");

            return sb.ToString();
        }

        static double ValueForLevel(long level)
            => 1.0 + 0.25 * Math.Max(level, 0);

        static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_SmartFireballs;
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;
            baseIncome *= (Decimal512)Math.Pow(2, SaveGame.Members.LevelSmartFireballsX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelSmartFireballs;

        public static Decimal512 PriceForNext()
            => UpgradeProgression.PriceForNextUpgrade(
                UpgradeProgression.InitialPrice_SmartFireballs,
                SaveGame.Members.LevelSmartFireballs,
                SaveGame.Members.LevelSmartFireballsX2);

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            long level = SaveGame.Members.LevelSmartFireballs;
            bool enabled = level > 0;

            PlayerUpgrades.Data.WizardSmartFireballsEnabled = enabled;
            PlayerUpgrades.Data.WizardSmartFireballsDamageMultiplier = enabled ? ValueForLevel(level) : 1.0f;
            if (enabled)
            {
                float arenaScale = GameManager.Instance != null ? GameManager.Instance.ArenaScale : 1.0f;
                PlayerUpgrades.Data.WizardSmartFireballsJumpRange = JumpRangeUnits * arenaScale;
            }
            else
            {
                PlayerUpgrades.Data.WizardSmartFireballsJumpRange = 0.0f;
            }
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelSmartFireballs += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(
                SaveGame.Members.LevelSmartFireballs,
                SaveGame.Members.LevelSmartFireballsX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(
                UpgradeProgression.InitialPrice_SmartFireballs_X2,
                SaveGame.Members.LevelSmartFireballsX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelSmartFireballsX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelSmartFireballs,
                SaveGame.Members.LevelSmartFireballsX2,
                UpgradeProgression.InitialPrice_SmartFireballs_X2);

            UpgradeManager.Instance.SmartFireballs.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelSmartFireballs);
        }
    }
}
