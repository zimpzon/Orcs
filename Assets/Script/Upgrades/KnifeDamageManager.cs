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
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=green>${baseIncome}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=green>${totalIncome}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=green>${Format256.Format(earnedSoFar)}</color>.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current damage: <color=green>{currentValue}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=green>{(nextValue.ToString())}</color>");

            return sb.ToString();
        }

        private static long ValueForLevel(long level)
            => 10 + (4 * level);

        private static Decimal256 BaseIncome() => 2;

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelKnifeDamage;

        public static long PriceForNext()
        {
            return (long)(500 * Math.Pow(1.15, SaveGame.Members.LevelKnifeDamage));
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
            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelKnifeDamage++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.KnifeDamage.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelKnifeDamage);
        }
    }
}
