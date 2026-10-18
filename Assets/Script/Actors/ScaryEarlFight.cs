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
    const double HpMul = 10; // x one final enemy (The Night Creeper) at the current arena level
    const float Mass = 25f;
    const float SpeedMul = 1.5f; // faster than the plain chaser (was 1.35)

    public const string ArenaText = "SCARY EARL";
    public const string StoryFighting =
        "<color=yellow>Scary Earl</color> was the mastermind behind it all!\nHe looks unbeatable! Is Earl doomed to fight him forever?";
    public const string StoryDefeated =
        "You defeated <color=yellow>Scary Earl!</color>\nEverything can go back to normal now! Well, except for the monsters.";

    public static bool Active => SaveGame.Members.FogTouched && !SaveGame.Members.ScaryEarlDefeated;

    // Text under the HP bar: nothing before the fog was touched, then the story (kept for good once he's beaten).
    public static string StoryText
        => SaveGame.Members.ScaryEarlDefeated ? StoryDefeated : SaveGame.Members.FogTouched ? StoryFighting : "";

    // The last spawned Scary Earl (may be stale between rounds: check IsScaryEarl before using it).
    public static ActorBase Current;

    public static List<ActorBase> Spawn(long arenaLevel)
    {
        var earl = SpawnUtil.Random(BaseType, 1).First();
        Current = earl;
        float scale = Mathf.Abs(G.D.PlayerScript.transform.localScale.x);
        earl.MakeScaryEarl(RunSprites() ?? earl.Animations, scale, Mass, SpeedMul);

        double hpD = EnemySpawner.FinalEnemyHp(arenaLevel) * HpMul;
        long hp = hpD > long.MaxValue / 4 ? long.MaxValue / 4 : (long)hpD;
        earl.BaseHp = hp;
        earl.Hp = hp;
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
