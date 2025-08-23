using Assets.Script.Actors.Spawning;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemySpawner
{
    public const long HpScale = 5;
    public const long MaxEnemies = 100;

    // Enemy type definitions with base HP (before HpScale)
    private static readonly EnemyType[] EnemyTypes = new[]
    {
        new EnemyType(ActorTypeEnum.AfroOrc,       16_000_000_000),
        new EnemyType(ActorTypeEnum.BrainZombie,    8_000_000_000),
        new EnemyType(ActorTypeEnum.IronMask,       4_000_000_000),
        new EnemyType(ActorTypeEnum.Snout,          1_600_000_000),
        new EnemyType(ActorTypeEnum.Karateeth, 700_000_000),
        new EnemyType(ActorTypeEnum.WannabeNecro, 320_000_000),
        new EnemyType(ActorTypeEnum.UndeadPirate, 160_000_000),
        new EnemyType(ActorTypeEnum.FreakyWiz, 80_000_000),
        new EnemyType(ActorTypeEnum.PigHat, 40_000_000),
        new EnemyType(ActorTypeEnum.Swede, 20_000_000),
        new EnemyType(ActorTypeEnum.Helmet, 10_000_000),
        new EnemyType(ActorTypeEnum.Fez, 4_000_000),
        new EnemyType(ActorTypeEnum.White, 2_000_000),
        new EnemyType(ActorTypeEnum.Pigtail, 1_000_000),
        new EnemyType(ActorTypeEnum.Pig, 500_000),
        new EnemyType(ActorTypeEnum.Red, 200_000),
        new EnemyType(ActorTypeEnum.Raven, 50_000),
        new EnemyType(ActorTypeEnum.Green, 10_000),
        new EnemyType(ActorTypeEnum.HeroChaser, 2_000),
        new EnemyType(ActorTypeEnum.OgreLarge, 200),
        new EnemyType(ActorTypeEnum.OgreSmall, 10),
        new EnemyType(ActorTypeEnum.BatWhite, 4) // Always available as fallback
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

        const int maxRetries = 10;
        long hpMultiplier = 1;

        for (int retry = 0; retry < maxRetries; retry++)
        {
            enemies.Clear();

            // Uncomment for testing specific enemies
            //var tester = SpawnUtil.Single(ActorTypeEnum.IronMask, Vector2.zero).First();
            //tester.BaseHp = EnemyTypes.First(e => e.Type == ActorTypeEnum.Fez).ScaledHp;
            //enemies.Add(tester);


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

            // Try to spawn enemies from strongest to weakest
            foreach (var enemyType in EnemyTypes)
            {
                if (totalEnemies >= MaxEnemies) break;

                long effectiveHp = enemyType.ScaledHp * hpMultiplier;
                if (remainingHp < effectiveHp) continue;

                long maxCount = Math.Min(remainingHp / effectiveHp, MaxEnemies - totalEnemies);
                long count = Math.Max(0, maxCount - UnityEngine.Random.Range(0, 2));

                if (count > 0)
                {
                    bool useCircle = circleType == enemyType.Type && count > 5;
                    SpawnEnemies(enemies, enemyType.Type, (int)count, effectiveHp, useCircle);
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

    private static long CalculateHpTarget(long level)
    {
        const long HpBase = 40;
        const long HpBasePerLevel = 60;

        return (HpBase + ((level - 1) * HpBasePerLevel) + ((level - 1) * (level - 1) * 10)) * HpScale;
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