using Assets.Script;
using Assets.Script.Actors.Spawning;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemySpawner
{
    public const long HpScale = 5;
    // Extra enemy HP, applied per enemy in SpawnEnemies. Phased in with the arena level: x1 at arena 1, rising in a
    // straight line to the full EnemyHpMul at arena EnemyHpMulFullLevel (and beyond), so the early game is unchanged.
    public const long EnemyHpMul = 150; // was 500: more arena progress per hour played
    public const long EnemyHpMulFullLevel = 10_000;
    static double _hpMulForLevel = 1.0; // set per wave in GetEnemies

    public static double EnemyHpMulForLevel(long level)
    {
        double t = Math.Clamp((level - 1) / (double)(EnemyHpMulFullLevel - 1), 0.0, 1.0);
        return (1.0 + (EnemyHpMul - 1) * t) * LateHpMul(level);
    }

    // Late arena ramp, on top of the above: x1 up to LateHpStart, then 10^(LateHpLog * ((L - start) / LateHpScale)^LateHpPower):
    // x3.3 at 32K, x12 at 40K, x32 at 52K, x65 at 63K, x103 at 72K, x187 at 85K, x338 at 100K. Playtest (2026-10): with 19
    // of 24 tiers the player stalls at ~32K (9th-last enemy). Together with the round budget (grows with level^2), clearing
    // 100K takes ~x1000 the damage of that wall - the estimate for the last 5 tiers (crit x24-77, Beefy/Power Zap/Storm
    // x16-52 per stream, Skull Slicer x10-31) plus the earlier tiers' extra levels. LateHpLog scales the whole ramp,
    // LateHpPower only how much it keeps growing later. Max enemy HP stays far under the long clamp.
    const double LateHpStart = 30_000;
    const double LateHpScale = 15_000;
    const double LateHpLog = 1.274;
    const double LateHpPower = 0.445;

    static double LateHpMul(long level)
    {
        if (level <= LateHpStart)
            return 1.0;
        return Math.Pow(10.0, LateHpLog * Math.Pow((level - LateHpStart) / LateHpScale, LateHpPower));
    }
    public const long MaxEnemies = 50;

    // Enemy type definitions with base HP (before HpScale) and the arena level at which the type
    // becomes eligible to spawn at all (independent of HP-budget affordability). MinLevel gaps
    // keep accelerating from Karateeth (4200) up to TheDarkness (~25,000) - deliberately NOT
    // linear, since arenas clear much faster at high levels so a fixed level-gap would feel
    // front-loaded in real play time even though it looks even on paper.
    // A handful of BaseHp values below (Helmet through Karateeth, and PigFromSpace) were also
    // lowered so HP-budget affordability doesn't become the binding gate and silently override
    // MinLevel further out than intended (as happened with TheDarkness at its original 100B HP,
    // which wasn't naturally affordable until ~arena 40,800 regardless of any MinLevel gate).
    // Conversely, BatWhite through Green had BaseHp raised: with MinLevel now gating the roster
    // to just 1-2 available types for long early stretches, their old (much smaller) BaseHp meant
    // the HP budget at their own gate level needed 40-120+ of them to fill a round, blowing past
    // MaxEnemies and forcing the hpMultiplier retry into oddly tanky reskins of weak mobs. Raised
    // BaseHp keeps budget/effectiveHp in the same ~10-17x range every other tier already sits in.
    // 2026-10: whole curve halved so beasts unlock about twice as early in play time (~12h now reaches beast #8
    // instead of #6). MinLevel x0.5 everywhere, BaseHp ~x0.25 so budget/effectiveHp at each type's gate is the
    // same as before (the HP budget is quadratic in level). The level/HP numbers in the comments above and below
    // refer to the pre-halving values.
    // 2026-10: top 8 BaseHp raised: x10 for the last (FromTheVoid), the added part scaling down linearly to x1.45
    // for the 8th (InnocentOrc). Everything below unchanged.
    // 2026-10: top 8 MinLevel compressed into ~40k-100k so arena numbers stay below ~100k. BaseHp only changes how many
    // enemies fill a round's HP budget; late difficulty comes from LateHpMul (all enemies). The 4th arg (Toughness)
    // adds extra HP for one type at spawn, outside the budget, so rounds with it really get harder without moving its
    // unlock arena: x10 for the last, scaling linearly down to x1.45 for the 8th from the top.
    private static readonly EnemyType[] EnemyTypes = new[]
    {
        // AI-Added pixel dudes: 13 new tiers extending the roster past TheDarkness, continuing the
        // same accelerating MinLevel-gap convention as the stretch above (gaps growing from ~5,000
        // up to ~18,000). BaseHp is deliberately compressed to a ~1.148x-per-tier geometric curve
        // topping out at 6x TheDarkness's HP (FromTheVoid, shown as The Night Creeper), not a straight continuation of the
        // lower tiers' ~1.37x-per-tier spacing. The uncapped combat-damage multipliers (Zap Damage
        // +20%/level, Smart Daggers +80%/level, Beefy Earl +5%/level, all linear-per-level against
        // the same 1.15^level exponential price curve) mean a much steeper HP ceiling here would
        // demand a disproportionate number of extra upgrade levels beyond whatever it took to reach
        // TheDarkness - a wall, not "grind a bit more." 6x keeps this a real but reachable extension.
        new EnemyType(ActorTypeEnum.FromTheVoid,    530_000_000_000, 100_000, 10.0),
        new EnemyType(ActorTypeEnum.TheGrey,        403_810_000_000,  92_000, 4.0),
        new EnemyType(ActorTypeEnum.DarkWizard,     302_290_000_000,  85_000, 2.0),
        new EnemyType(ActorTypeEnum.Blueberry,       221_750_000_000,  78_000, 1.4),
        new EnemyType(ActorTypeEnum.ShirlieShaman,  153_430_000_000,  72_000, 1.3),
        new EnemyType(ActorTypeEnum.UndeadPig,      101_210_000_000,  63_000, 1.2),
        new EnemyType(ActorTypeEnum.ChillZombie,     61_440_000_000,  52_000, 1.1),
        new EnemyType(ActorTypeEnum.InnocentOrc,     29_000_000_000,  40_000, 1),
        new EnemyType(ActorTypeEnum.AngryDude,       17_000_000_000,  28_000),
        new EnemyType(ActorTypeEnum.HrDude,          15_000_000_000,  24_000),
        new EnemyType(ActorTypeEnum.OrangeMan,       14_000_000_000,  21_000),
        new EnemyType(ActorTypeEnum.BabyOrc,         11_000_000_000,  17_750),
        new EnemyType(ActorTypeEnum.NakedDude,       10_000_000_000,  15_000),
        new EnemyType(ActorTypeEnum.TheDarkness,      8_700_000_000,  12_500),
        new EnemyType(ActorTypeEnum.PigFromSpace,     5_200_000_000,  10_250),
        new EnemyType(ActorTypeEnum.FromTheDeep,      2_500_000_000,   8_250),
        new EnemyType(ActorTypeEnum.Faceless,         1_100_000_000,   6_500),
        new EnemyType(ActorTypeEnum.AfroOrc,            680_000_000,   5_250),
        new EnemyType(ActorTypeEnum.BrainZombie,        360_000_000,   4_000),
        new EnemyType(ActorTypeEnum.IronMask,           220_000_000,   3_250),
        new EnemyType(ActorTypeEnum.Snout,              150_000_000,   2_500),
        new EnemyType(ActorTypeEnum.Karateeth,           73_000_000,   2_000),
        new EnemyType(ActorTypeEnum.WannabeNecro,        44_000_000,   1_750),
        new EnemyType(ActorTypeEnum.UndeadPirate,        24_000_000,   1_500),
        new EnemyType(ActorTypeEnum.FreakyWiz,           13_000_000,   1_250),
        new EnemyType(ActorTypeEnum.PigHat,               7_000_000,   1_000),
        new EnemyType(ActorTypeEnum.Swede,                3_700_000,     750),
        new EnemyType(ActorTypeEnum.Helmet,               2_000_000,     600),
        new EnemyType(ActorTypeEnum.Fez,                  1_000_000,     450),
        new EnemyType(ActorTypeEnum.White,                  500_000,     300),
        new EnemyType(ActorTypeEnum.Pigtail,                250_000,     200),
        new EnemyType(ActorTypeEnum.Pig,                    120_000,     160),
        new EnemyType(ActorTypeEnum.Red,                     50_000,     130),
        new EnemyType(ActorTypeEnum.Raven,                   12_000,     100),
        new EnemyType(ActorTypeEnum.Green,                    8_600,      75),
        new EnemyType(ActorTypeEnum.HeroChaser,               3_400,      50),
        new EnemyType(ActorTypeEnum.OgreLarge,                  480,      25),
        new EnemyType(ActorTypeEnum.OgreSmall,                   13,       5),
        new EnemyType(ActorTypeEnum.BatWhite,                     4,       1),
    };

    private struct EnemyType
    {
        public ActorTypeEnum Type { get; }
        public long BaseHp { get; }
        public long MinLevel { get; }
        // Extra HP multiplier applied at spawn, NOT counted against the round's HP budget: makes rounds with this
        // enemy really harder (the budget alone only changes how many enemies fill a round, not its total HP).
        public double Toughness { get; }
        public long ScaledHp => BaseHp * HpScale;

        public EnemyType(ActorTypeEnum type, long baseHp, long minLevel, double toughness = 1.0)
        {
            Type = type;
            BaseHp = baseHp;
            MinLevel = minLevel;
            Toughness = toughness;
        }
    }

    // Arena level where this enemy type starts to spawn (its unlock level in the bestiary). -1 if not in the table.
    public static long UnlockLevelOf(ActorTypeEnum type)
    {
        foreach (var e in EnemyTypes)
        {
            if (e.Type == type)
                return e.MinLevel;
        }
        return -1;
    }

    static double ToughnessOf(ActorTypeEnum type)
    {
        foreach (var e in EnemyTypes)
        {
            if (e.Type == type)
                return e.Toughness;
        }
        return 1.0;
    }

    private enum SpawnPattern { None, Circle, FilledCircle, Square, Line, Triangle }

    public static IEnumerable<ActorBase> GetEnemies(long level)
    {
        long hpTarget = CalculateHpTarget(level);
        _hpMulForLevel = EnemyHpMulForLevel(level);
        var enemies = new List<ActorBase>();


        //// (DOESN'T WORK - what? it is cleared right after!) -> TEST NEW ENEMIES: Add them here
        //enemies.AddRange(SpawnUtil.Random(ActorTypeEnum.TheDarkness, 1).ToList());
        //enemies.AddRange(SpawnUtil.Random(ActorTypeEnum.PigFromSpace, 1).ToList());
        //enemies.AddRange(SpawnUtil.Random(ActorTypeEnum.FromTheDeep, 1).ToList());


        const int maxRetries = 10;
        long hpMultiplier = 1;

        for (int retry = 0; retry < maxRetries; retry++)
        {
            enemies.Clear();

            long remainingHp = hpTarget;
            long totalEnemies = 0;
            ActorTypeEnum? patternType = null;
            SpawnPattern chosenPattern = SpawnPattern.None;

            // Pick an interesting formation for one enemy type roughly every 5 arena levels rather
            // than every round - keeps it a treat instead of noise.
            if (level % 5 == 0)
            {
                var eligibleTypes = new List<ActorTypeEnum>();

                // First pass: find types that will have 5+ enemies
                foreach (var enemyType in EnemyTypes)
                {
                    if (level < enemyType.MinLevel) continue;

                    long effectiveHp = enemyType.ScaledHp * hpMultiplier;
                    if (remainingHp < effectiveHp) continue;

                    long maxCount = Math.Min(remainingHp / effectiveHp, MaxEnemies - totalEnemies);
                    long count = Math.Max(0, maxCount - UnityEngine.Random.Range(0, 2));

                    if (count > 5)
                        eligibleTypes.Add(enemyType.Type);
                }

                if (eligibleTypes.Count > 0)
                {
                    patternType = eligibleTypes[UnityEngine.Random.Range(0, eligibleTypes.Count)];

                    var patterns = (SpawnPattern[])Enum.GetValues(typeof(SpawnPattern));
                    chosenPattern = patterns[UnityEngine.Random.Range(1, patterns.Length)]; // skip None
                }
            }

            // PHASE 1: Guarantee strongest AFFORDABLE enemy spawns first (85% chance - increased from 60%)
            bool shouldSpawnStrongest = UnityEngine.Random.value < 0.85f;
            if (shouldSpawnStrongest)
            {
                // Find the strongest enemy we can actually afford
                EnemyType? strongestAffordable = null;
                foreach (var enemyType in EnemyTypes)
                {
                    if (level < enemyType.MinLevel) continue;

                    long effectiveHp = enemyType.ScaledHp * hpMultiplier;
                    if (remainingHp >= effectiveHp)
                    {
                        strongestAffordable = enemyType;
                        break; // First one we can afford is the strongest (array is sorted strongest to weakest)
                    }
                }

                if (strongestAffordable.HasValue)
                {
                    var enemyType = strongestAffordable.Value;
                    long effectiveHp = enemyType.ScaledHp * hpMultiplier;
                    long maxCount = Math.Min(remainingHp / effectiveHp, MaxEnemies - totalEnemies);
                    // Spawn more of the strongest enemy (1-6 instead of 1-3)
                    long count = Math.Min(maxCount, UnityEngine.Random.Range(1, 7));

                    if (count > 0)
                    {
                        SpawnPattern pattern = patternType == enemyType.Type && count > 5 ? chosenPattern : SpawnPattern.None;
                        SpawnEnemies(enemies, enemyType.Type, (int)count, effectiveHp, pattern);
                        remainingHp -= count * effectiveHp;
                        totalEnemies += count;
                    }
                }
            }

            // PHASE 2: Fill remaining budget with heavily weighted selection toward strong enemies
            while (remainingHp > 0 && totalEnemies < MaxEnemies)
            {
                var availableTypes = EnemyTypes.Where(e => level >= e.MinLevel && e.ScaledHp * hpMultiplier <= remainingHp).ToArray();
                if (availableTypes.Length == 0) break;

                // Create weights that extremely favor stronger enemies (much higher exponential bias)
                var weights = new float[availableTypes.Length];
                for (int i = 0; i < availableTypes.Length; i++)
                {
                    // Find the original index in EnemyTypes array to get proper strength ordering
                    int originalIndex = Array.FindIndex(EnemyTypes, e => e.Type == availableTypes[i].Type);
                    // Lower index = stronger enemy, much higher exponential weight (increased from 2f to 4f)
                    weights[i] = Mathf.Pow(4f, EnemyTypes.Length - originalIndex);
                }

                var selectedType = WeightedRandomSelect(availableTypes, weights);
                long effectiveHp = selectedType.ScaledHp * hpMultiplier;

                // Prefer spawning more of stronger enemies
                long maxCount = Math.Min(remainingHp / effectiveHp, MaxEnemies - totalEnemies);
                int selectedIndex = Array.FindIndex(EnemyTypes, e => e.Type == selectedType.Type);

                // If it's a top-tier enemy (first 8 in the list), spawn more of them
                long count;
                if (selectedIndex < 8) // Top 8 strongest enemies
                {
                    count = Math.Min(maxCount, UnityEngine.Random.Range(2, 6)); // 2-5 instead of 1-3
                }
                else
                {
                    count = Math.Min(maxCount, UnityEngine.Random.Range(1, 3)); // Keep weaker enemies at 1-2
                }

                if (count > 0)
                {
                    SpawnPattern pattern = patternType == selectedType.Type && count > 5 ? chosenPattern : SpawnPattern.None;
                    SpawnEnemies(enemies, selectedType.Type, (int)count, effectiveHp, pattern);
                    remainingHp -= count * effectiveHp;
                    totalEnemies += count;
                }
            }

            // Ensure a minimum number of enemies spawn even for high-tier "boss" rounds where a
            // single enemy eats almost the entire HP budget (see the MinLevel comment above -
            // FromTheDeep/PigFromSpace/TheDarkness are tuned close to their affordability
            // threshold on purpose). Padding with cheap BatWhite filler adds only negligible extra
            // HP on top of the round's target budget.
            const int MinEnemiesPerRound = 5;
            if (totalEnemies < MinEnemiesPerRound)
            {
                var batType = EnemyTypes.Last(); // BatWhite
                int padCount = (int)(MinEnemiesPerRound - totalEnemies);
                SpawnEnemies(enemies, batType.Type, padCount, batType.ScaledHp * hpMultiplier);
                totalEnemies += padCount;
            }

            // If we spawned fewer than max enemies, we're done
            if (enemies.Count < MaxEnemies)
                break;

            // Otherwise, increase HP multiplier and try again
            hpMultiplier *= 10;
        }

        return enemies;
    }

    // Index into EnemyTypes (0 = strongest/TheDarkness, higher = weaker); -1 for non-tiered types.
    public static int GetTierRank(ActorTypeEnum type)
        => Array.FindIndex(EnemyTypes, e => e.Type == type);

    // Scales Witch Doctor zap damage down for weaker enemies relative to whichever tier is
    // currently the toughest ALIVE enemy in the arena: full damage against that enemy, decaying
    // exponentially per tier below it so 5 tiers down lands at ~20%, with a 10% hard floor so
    // trash mobs are never fully immune to the zap.
    public static double GetWitchDoctorTierMultiplier(ActorTypeEnum targetType)
    {
        int targetRank = GetTierRank(targetType);
        if (targetRank < 0)
            return 1.0;

        int strongestAliveRank = int.MaxValue;
        foreach (var enemy in BlackboardScript.GetAllEnemies())
        {
            if (enemy.Hp <= 0) continue;

            int rank = GetTierRank(enemy.ActorType);
            if (rank >= 0 && rank < strongestAliveRank)
                strongestAliveRank = rank;
        }

        if (strongestAliveRank == int.MaxValue)
            return 1.0;

        int tiersBelow = targetRank - strongestAliveRank;
        if (tiersBelow <= 0)
            return 1.0;

        const double DecayPerTier = 0.72478; // 0.72478^5 ~= 0.20
        const double MinMultiplier = 0.10;
        return Math.Max(MinMultiplier, Math.Pow(DecayPerTier, tiersBelow));
    }

    // How many enemy tiers have ever been reached, out of the full roster - used for the game
    // progress popup. Based on MaxArena (lifetime best) rather than the current ArenaLevel, so it
    // doesn't regress after ascending back down to arena 1.
    public static (int unlocked, int total) GetTierUnlockProgress()
    {
        int unlocked = EnemyTypes.Count(e => e.MinLevel <= SaveGame.Members.MaxArena);
        return (unlocked, EnemyTypes.Length);
    }

    private static EnemyType WeightedRandomSelect(EnemyType[] types, float[] weights)
    {
        float totalWeight = weights.Sum();
        float randomValue = UnityEngine.Random.value * totalWeight;

        float currentWeight = 0;
        for (int i = 0; i < types.Length; i++)
        {
            currentWeight += weights[i];
            if (randomValue <= currentWeight)
                return types[i];
        }

        return types.Last(); // Fallback
    }

    //private static long CalculateHpTarget(long level)
    //{
    //    // 5 billion at level 10K, HpScale is included.
    //    const long HpBase = 40;
    //    const long HpBasePerLevel = 60;

    //    return (HpBase + ((level - 1) * HpBasePerLevel) + ((level - 1) * (level - 1) * 10)) * HpScale;
    //}

    // HP of one final enemy (The Night Creeper) at this arena level, as spawned: base HP x the level's extra HP x its
    // toughness. Scary Earl is a multiple of it (ScaryEarlFight). Clamped like SpawnEnemies.
    public static double FinalEnemyHp(long level)
    {
        var finalEnemy = EnemyTypes.First(e => e.Type == ActorTypeEnum.FromTheVoid);
        return (double)finalEnemy.ScaledHp * EnemyHpMulForLevel(level) * finalEnemy.Toughness;
    }

    private static long CalculateHpTarget(long level)
    {
        const long HpBase = 40;
        const long HpBasePerLevel = 60;
        const long HpScale = 5;

        // Desired HP at level 10,000 (adjustable)
        const long HpAtTargetLevel = 30_000_000_000;

        // Just a fixpoint to have an idea of where HP scaling is going.
        const int TargetLevel = 10_000;

        // Quadratic coefficient is solved so that HP(10000) == HpAtMaxLevel
        double quadCoeff =
            (HpAtTargetLevel / (double)HpScale
            - (HpBase + (TargetLevel - 1) * HpBasePerLevel))
            / ((TargetLevel - 1L) * (TargetLevel - 1L));

        return (long)((HpBase
            + ((level - 1) * HpBasePerLevel)
            + ((level - 1L) * (level - 1L) * quadCoeff)) * HpScale);
    }

    private static void SpawnEnemies(List<ActorBase> enemies, ActorTypeEnum type, int count, long hp, SpawnPattern pattern = SpawnPattern.None)
    {
        float halfSize = (GameManager.ArenaBounds.height - 2) / 2;
        Vector2 center = GameManager.ArenaBounds.center + Vector2.right * 3;

        IEnumerable<ActorBase> spawned;
        switch (pattern)
        {
            case SpawnPattern.Circle:
                spawned = SpawnUtil.Circle(type, center, halfSize, count);
                break;

            case SpawnPattern.FilledCircle:
                spawned = SpawnUtil.FilledCircle(type, center, halfSize, count);
                break;

            case SpawnPattern.Square:
                int gridSize = Mathf.Max(1, Mathf.CeilToInt(Mathf.Sqrt(count)));
                float gridSpacing = (halfSize * 2) / gridSize;
                spawned = SpawnUtil.Square(type, center, gridSize, gridSpacing).Take(count);
                break;

            case SpawnPattern.Line:
                spawned = SpawnUtil.Line(type, center, halfSize * 2, count, UnityEngine.Random.Range(0f, 180f));
                break;

            case SpawnPattern.Triangle:
                float triangleSpacing = (halfSize * 2) / Mathf.Max(1, count);
                spawned = SpawnUtil.Triangle(type, center, triangleSpacing, count);
                break;

            default:
                spawned = SpawnUtil.Random(type, count);
                break;
        }

        // Every enemy gets the level-scaled extra HP (EnemyHpMulForLevel) on top of what the spawn budget was planned
        // with: same enemy count, tougher enemies.
        // Clamped: at extreme arena levels the multiplier could overflow a long.
        double scaledHp = hp * _hpMulForLevel * ToughnessOf(type);
        hp = scaledHp > long.MaxValue / 4 ? long.MaxValue / 4 : (long)scaledHp;
        foreach (var enemy in spawned)
        {
            enemy.BaseHp = hp;
            // Reset() (Hp = BaseHp) only runs when returning to the cache, i.e. with the previous BaseHp.
            enemy.Hp = hp;
            enemies.Add(enemy);
        }
    }
}