using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class KnifeDamageManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelKnifeDamage;
            Decimal256 earnedSoFar = SaveGame.Members.TotalIncomeKnifeDamage;
            Decimal256 baseIncome = BaseIncome();
            Decimal256 totalIncome = PassiveIncome();
            long currentValue = ValueForLevel(level);
            long nextValue = ValueForLevel(level + 1);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Dagger Damage</color></b></size>");
            sb.AppendLine("<color=#dddddd>Each dagger does more damage.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format256.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format256.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format256.Format(earnedSoFar)}</color>.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current damage: <color=COLOR-ARENA>{currentValue}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{(nextValue.ToString())}</color>");

            return sb.ToString();
        }

        private static long ValueForLevel(long level)
            => 10 + (4 * level);

        private static Decimal256 BaseIncome() => UpgradeProgression.BaseIncome_DaggerDamage;

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelKnifeDamage;

        public static Decimal256 PriceForNext()
        {
            return UpgradeProgression.InitialPrice_DaggerDamage * Math.Pow(1.15, SaveGame.Members.LevelKnifeDamage);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MagicMissileBaseDamage = ValueForLevel(SaveGame.Members.LevelKnifeDamage);
        }

        public static void OnBuy()
        {
            Decimal256 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelKnifeDamage++;
        }

        public static void UpdateUi()
        {
            Decimal256 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.KnifeDamage.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelKnifeDamage);
        }
    }
}
