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

        // Most tiers ever bought in one run, so a rebirth doesn't take completion away. Winning implies all tiers,
        // which also repairs saves that won (and rebirthed) before MaxUpgradeTiersBought existed.
        var members = SaveGame.Members;
        int current = UpgradeTierList.CountBought();
        if (current > members.MaxUpgradeTiersBought)
            members.MaxUpgradeTiersBought = current;
        p.TiersTotal = UpgradeTierList.Tiers.Length;
        p.TiersBought = members.Achieved.Contains(Achieved.Completion100)
            ? p.TiersTotal
            : (int)System.Math.Min(members.MaxUpgradeTiersBought, p.TiersTotal);

        p.EnemyPct = p.EnemiesTotal > 0 ? (double)p.EnemiesUnlocked / p.EnemiesTotal * 100 : 0;
        p.SkinPct = p.SkinsTotal > 0 ? (double)p.SkinsUnlocked / p.SkinsTotal * 100 : 0;
        p.TierPct = p.TiersTotal > 0 ? (double)p.TiersBought / p.TiersTotal * 100 : 0;
        p.TotalPct = (p.EnemyPct + p.SkinPct + p.TierPct) / 3.0;
        return p;
    }
}
