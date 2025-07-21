namespace Assets.Script.Upgrades
{
    public abstract class UpgradeItemSettings
    {
        public abstract long Level();
        public abstract double BaseIncome();
        public abstract string GetText();
        public abstract long PriceForNext();
        public abstract void UpdateAll();
        public abstract void UpdateUI();
        public abstract void OnBuy();
    }
}
