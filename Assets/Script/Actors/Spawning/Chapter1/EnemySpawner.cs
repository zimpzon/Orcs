using Assets.Script.Actors.Spawning;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    public static IEnumerable<ActorBase> GetEnemies(long level)
    {
        const long MaxEnemies = 100;
        long HpBase = 40;
        long HpBasePerLevel = 60;

        long hpTargetForRound = HpBase + ((level - 1) * HpBasePerLevel) + ((level - 1) * (level - 1) * 10);

        // Base HP values (do NOT scale arbitrarily)
        long hpBat = 20;
        long hpOgreSmall = 50;
        long hpOgreLarge = 1000;

        // Optional: prevent large ogres if budget is too low
        bool allowLarge = hpTargetForRound >= hpOgreLarge;

        List<ActorBase> enemies = new();
        int safety = 100;
        while (true)
        {
            long totalEnemies = 0;
            long remainingHp = hpTargetForRound;
            enemies.Clear();

            // 1. Large Ogres
            long clampedLarge = 0;
            if (allowLarge)
            {
                clampedLarge = Math.Min(remainingHp / hpOgreLarge, MaxEnemies - totalEnemies);
                clampedLarge -= UnityEngine.Random.Range(0, 2); // a bit of variation
                clampedLarge = Math.Max(0, clampedLarge);
                remainingHp -= clampedLarge * hpOgreLarge;
                totalEnemies += clampedLarge;
            }

            // 2. Small Ogres
            long clampedSmall = Math.Min(remainingHp / hpOgreSmall, MaxEnemies - totalEnemies);
            clampedSmall -= UnityEngine.Random.Range(0, 2);
            clampedSmall = Math.Max(0, clampedSmall);
            remainingHp -= clampedSmall * hpOgreSmall;
            totalEnemies += clampedSmall;

            // 3. Bats
            long clampedBat = Math.Min(remainingHp / hpBat, MaxEnemies - totalEnemies);
            clampedBat = Math.Max(0, clampedBat);
            remainingHp -= clampedBat * hpBat;
            totalEnemies += clampedBat;

            // If still nothing spawned, force at least one bat
            if (totalEnemies == 0)
            {
                clampedBat = 1;
                remainingHp -= hpBat;
            }

            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.OgreLarge, (int)clampedLarge))
            {
                actor.BaseHp = hpOgreLarge;
                enemies.Add(actor);
            }

            bool tryCircle = UnityEngine.Random.value < 0.25;
            if (tryCircle && clampedSmall > 5)
            {
                float circleHeight = (GameManager.ArenaBounds.height - 2) / 2;
                Vector2 center = GameManager.ArenaBounds.center + Vector2.right * 3;
                foreach (var actor in SpawnUtil.Circle(ActorTypeEnum.OgreSmall, center, radius: circleHeight, (int)clampedSmall))
                {
                    actor.BaseHp = hpOgreSmall;
                    enemies.Add(actor);
                }
            }
            else
            {
                foreach (var actor in SpawnUtil.Random(ActorTypeEnum.OgreSmall, (int)clampedSmall))
                {
                    actor.BaseHp = hpOgreSmall;
                    enemies.Add(actor);
                }
            }

            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.BatRed, (int)clampedBat))
            {
                actor.BaseHp = hpBat;
                enemies.Add(actor);
            }

            if (enemies.Count < 100)
                break;

            // We reached maxEnemies, give each moer HP and try again.
            hpOgreLarge *= 10;
            hpOgreSmall *= 10;
            hpBat *= 10;

            if (--safety <= 0)
                throw new Exception($"Tried spawning <100 enemies too many times");
        }

        foreach(var actor in enemies)
            yield return actor;
    }
}
