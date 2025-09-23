using Assets.Script.Actors.Spawning;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemySpawner
{
    public const long HpScale = 5;
    public const long MaxEnemies = 50;

    // Enemy type definitions with base HP (before HpScale)
    private static readonly EnemyType[] EnemyTypes = new[]
    {
        new EnemyType(ActorTypeEnum.TheDarkness,100_000_000_000),
        new EnemyType(ActorTypeEnum.PigFromSpace,32_000_000_000),
        new EnemyType(ActorTypeEnum.FromTheDeep, 10_000_000_000),
        new EnemyType(ActorTypeEnum.Faceless,     4_400_000_000),
        new EnemyType(ActorTypeEnum.AfroOrc,      2_600_000_000),
        new EnemyType(ActorTypeEnum.BrainZombie,  1_500_000_000),
        new EnemyType(ActorTypeEnum.IronMask,       900_000_000),
        new EnemyType(ActorTypeEnum.Snout,          600_000_000),
        new EnemyType(ActorTypeEnum.Karateeth,      400_000_000),
        new EnemyType(ActorTypeEnum.WannabeNecro,   200_000_000),
        new EnemyType(ActorTypeEnum.UndeadPirate,   120_000_000),
        new EnemyType(ActorTypeEnum.FreakyWiz,       80_000_000),
        new EnemyType(ActorTypeEnum.PigHat,          40_000_000),
        new EnemyType(ActorTypeEnum.Swede,           20_000_000),
        new EnemyType(ActorTypeEnum.Helmet,          10_000_000),
        new EnemyType(ActorTypeEnum.Fez,              4_000_000),
        new EnemyType(ActorTypeEnum.White,            2_000_000),
        new EnemyType(ActorTypeEnum.Pigtail,          1_000_000),
        new EnemyType(ActorTypeEnum.Pig,                500_000),
        new EnemyType(ActorTypeEnum.Red,                200_000),
        new EnemyType(ActorTypeEnum.Raven,               50_000),
        new EnemyType(ActorTypeEnum.Green,               10_000),
        new EnemyType(ActorTypeEnum.HeroChaser,           2_000),
        new EnemyType(ActorTypeEnum.OgreLarge,              200),
        new EnemyType(ActorTypeEnum.OgreSmall,               10),
        new EnemyType(ActorTypeEnum.BatWhite,                 4),
    };

    private struct EnemyType
    {
        public ActorTypeEnum Type { get; }
        public long BaseHp { get; }
        public long ScaledHp => BaseHp * HpScale;

        public EnemyType(ActorTypeEnum type, long baseHp)
        {
            Type = type;
            BaseHp = baseHp;
        }
    }

    public static IEnumerable<ActorBase> GetEnemies(long level)
    {
        long hpTarget = CalculateHpTarget(level);
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
            ActorTypeEnum? circleType = null;

            // Determine which type (if any) will spawn in a circle this round
            if (UnityEngine.Random.value < 0.25f)
            {
                var eligibleTypes = new List<ActorTypeEnum>();

                // First pass: find types that will have 5+ enemies
                foreach (var enemyType in EnemyTypes)
                {
                    long effectiveHp = enemyType.ScaledHp * hpMultiplier;
                    if (remainingHp < effectiveHp) continue;

                    long maxCount = Math.Min(remainingHp / effectiveHp, MaxEnemies - totalEnemies);
                    long count = Math.Max(0, maxCount - UnityEngine.Random.Range(0, 2));

                    if (count > 5)
                        eligibleTypes.Add(enemyType.Type);
                }

                if (eligibleTypes.Count > 0)
                    circleType = eligibleTypes[UnityEngine.Random.Range(0, eligibleTypes.Count)];
            }

            // PHASE 1: Guarantee strongest AFFORDABLE enemy spawns first (85% chance - increased from 60%)
            bool shouldSpawnStrongest = UnityEngine.Random.value < 0.85f;
            if (shouldSpawnStrongest)
            {
                // Find the strongest enemy we can actually afford
                EnemyType? strongestAffordable = null;
                foreach (var enemyType in EnemyTypes)
                {
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
                        bool useCircle = circleType == enemyType.Type && count > 5;
                        SpawnEnemies(enemies, enemyType.Type, (int)count, effectiveHp, useCircle);
                        remainingHp -= count * effectiveHp;
                        totalEnemies += count;
                    }
                }
            }

            // PHASE 2: Fill remaining budget with heavily weighted selection toward strong enemies
            while (remainingHp > 0 && totalEnemies < MaxEnemies)
            {
                var availableTypes = EnemyTypes.Where(e => e.ScaledHp * hpMultiplier <= remainingHp).ToArray();
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
                    bool useCircle = circleType == selectedType.Type && count > 5;
                    SpawnEnemies(enemies, selectedType.Type, (int)count, effectiveHp, useCircle);
                    remainingHp -= count * effectiveHp;
                    totalEnemies += count;
                }
            }

            // Ensure at least one enemy spawns
            if (totalEnemies == 0)
            {
                var batType = EnemyTypes.Last(); // BatWhite
                SpawnEnemies(enemies, batType.Type, 1, batType.ScaledHp * hpMultiplier, false);
            }

            // If we spawned fewer than max enemies, we're done
            if (enemies.Count < MaxEnemies)
                break;

            // Otherwise, increase HP multiplier and try again
            hpMultiplier *= 10;
        }

        return enemies;
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

    private static void SpawnEnemies(List<ActorBase> enemies, ActorTypeEnum type, int count, long hp, bool useCircle = false)
    {
        IEnumerable<ActorBase> spawned;

        if (useCircle)
        {
            float circleHeight = (GameManager.ArenaBounds.height - 2) / 2;
            Vector2 center = GameManager.ArenaBounds.center + Vector2.right * 3;
            spawned = SpawnUtil.Circle(type, center, circleHeight, count);
        }
        else
        {
            spawned = SpawnUtil.Random(type, count);
        }

        foreach (var enemy in spawned)
        {
            enemy.BaseHp = hp;
            enemies.Add(enemy);
        }
    }
}