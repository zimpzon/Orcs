using System.Linq;

// Overall game completion: average of enemies unlocked, skins unlocked and upgrade tiers bought.
// Shared by the progress (victory) popup and the bottom text bar.
public static class GameCompletion
{
    public struct Progress
    {
        public int EnemiesUnlocked, EnemiesTotal;
        public int SkinsUnlocked, SkinsTotal;
        public int TiersBought, TiersTotal;
        public double EnemyPct, SkinPct, TierPct, TotalPct;
    }

    // Note: SkinScript.GetUnlockProgress scans the skin objects, so callers should not call this every frame.
    public static Progress GetProgress()
    {
        var p = new Progress();
        (p.EnemiesUnlocked, p.EnemiesTotal) = EnemySpawner.GetTierUnlockProgress();
        (p.SkinsUnlocked, p.SkinsTotal) = SkinScript.GetUnlockProgress();

        // A tier counts as bought once at least one level of it has been purchased.
        var tiers = UpgradeTierList.Tiers;
        p.TiersBought = tiers.Count(t => t.Level() > 0);
        p.TiersTotal = tiers.Length;

        p.EnemyPct = p.EnemiesTotal > 0 ? (double)p.EnemiesUnlocked / p.EnemiesTotal * 100 : 0;
        p.SkinPct = p.SkinsTotal > 0 ? (double)p.SkinsUnlocked / p.SkinsTotal * 100 : 0;
        p.TierPct = p.TiersTotal > 0 ? (double)p.TiersBought / p.TiersTotal * 100 : 0;
        p.TotalPct = (p.EnemyPct + p.SkinPct + p.TierPct) / 3.0;
        return p;
    }
}
