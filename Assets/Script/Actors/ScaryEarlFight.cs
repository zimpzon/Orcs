using System.Collections.Generic;
using System.Linq;
using Assets.Script.Actors.Spawning;
using UnityEngine;

// After "Touch fog" in the bestiary (SaveGame.FogTouched), Scary Earl is the only enemy in the arena until he's
// defeated: normal size, heavy, 10x the final enemy's (The Night Creeper) HP at the current arena level. The arena level stays put while
// fighting him (no step on a win or back on a timeout); after the win normal arenas resume from it.
// He is a cached chaser dressed up as Scary Earl (ActorBase.MakeScaryEarl), turned back on its return to the cache.
public static class ScaryEarlFight
{
    const ActorTypeEnum BaseType = ActorTypeEnum.HeroChaser; // human-sized chaser
    const double HpMul = 6; // x one final enemy (The Night Creeper) at the level it first appears (Spawn)
    const float Mass = 60f; // hit knockback is divided by mass (was 25: Earl pushed him around)
    const float SpeedMul = 1.5f; // faster than the plain chaser (was 1.35)

    public const string ArenaText = "SCARY EARL";
    public static readonly string StoryFighting =
        $"<color={TextColors.Gold}>Scary Earl</color> was the mastermind behind it all!\nHe looks unbeatable! Is Earl doomed to fight him forever?";

    public static readonly string StoryDefeated =
        $"You defeated <color={TextColors.Gold}>Scary Earl!</color>\nEverything can go back to normal now! Well, except for the monsters.";

    // Marks him beaten and freezes the play time it took (shown in the victory dialog). Arena win and RightShift+E.
    public static void MarkDefeated()
    {
        var m = SaveGame.Members;
        if (!m.ScaryEarlDefeated)
            m.BeastKills.Clear(); // the slaying goals (BeastGoals) count from his defeat
        m.ScaryEarlDefeated = true;
        if (m.ScaryEarlDefeatedLevel < 0)
            m.ScaryEarlDefeatedLevel = m.ArenaLevel;
        if (m.ScaryEarlSeconds < 0)
            m.ScaryEarlSeconds = m.EstimatedOnlineSeconds2;
    }

    public static bool Active => SaveGame.Members.FogTouched && !SaveGame.Members.ScaryEarlDefeated;

    // Text under the HP bar: nothing before the fog was touched, then the story (kept for good once he's beaten).
    public static string StoryText
        => SaveGame.Members.ScaryEarlDefeated ? StoryDefeated : SaveGame.Members.FogTouched ? StoryFighting : "";

    // The last spawned Scary Earl (may be stale between rounds: check IsScaryEarl before using it).
    public static ActorBase Current;

    // HP from the final enemy at the level it first appears (100K), not the current level: playing on for many more
    // levels before touching the fog doesn't make him much harder.
    public static List<ActorBase> Spawn()
    {
        var earl = SpawnUtil.Random(BaseType, 1).First();
        Current = earl;
        float scale = Mathf.Abs(G.D.PlayerScript.transform.localScale.x);
        earl.MakeScaryEarl(RunSprites() ?? earl.Animations, scale, Mass, SpeedMul);

        // HP is a long, capped like every enemy's (long.MaxValue / 4, ~2.3e18). Late arenas pass that, so the part over
        // the cap becomes damage resistance instead: same kill time as the full HP would give.
        const long HpCap = long.MaxValue / 4;
        double hpD = EnemySpawner.FinalEnemyHp(EnemySpawner.FinalEnemyFirstLevel) * HpMul;
        long hp = hpD > HpCap ? HpCap : (long)hpD;
        earl.BaseHp = hp;
        earl.Hp = hp;
        earl.DamageTakenMul = hpD > HpCap ? HpCap / hpD : 1.0;
        return new List<ActorBase> { earl };
    }

    static Sprite[] _runSprites;

    static Sprite[] RunSprites()
    {
        if (_runSprites != null && _runSprites.Length > 0)
            return _runSprites;

        // The skin list asset is referenced by the (possibly never opened) skins popup: look it up directly.
        var skins = Resources.FindObjectsOfTypeAll<SkinsAnimationList>().FirstOrDefault();
        var anim = skins?.SkinAnimations.FirstOrDefault(s => s.Animation == SkinAnimation.ScaryEarl);
        _runSprites = anim?.RunSprites != null && anim.RunSprites.Length > 0 ? anim.RunSprites : anim?.IdleSprites;
        if (_runSprites == null || _runSprites.Length == 0)
            Debug.LogWarning("ScaryEarlFight: no Scary Earl sprites found");
        return _runSprites;
    }
}
