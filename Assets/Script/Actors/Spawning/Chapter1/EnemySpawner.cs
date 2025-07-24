using Assets.Script.Actors.Spawning;
using System;
using System.Collections.Generic;

public static class EnemySpawner
{
    public static IEnumerable<ActorBase> GetEnemies(long level)
    {
        const long MaxEnemies = 100;
        double HpLevelScaling = 1.15;
        long HpBase = 40;
        long HpBasePerLevel = 60;

        long hpTargetForRound = (long)(HpBase + ((level - 1) * HpBasePerLevel) + Math.Pow(HpLevelScaling, level + 5));

        // Base HP values (do NOT scale arbitrarily)
        long hpBat = 20;
        long hpOgreSmall = 50;
        long hpOgreLarge = 1000;

        long remainingHp = hpTargetForRound;
        long totalEnemies = 0;

        // Optional: prevent large ogres if budget is too low
        bool allowLarge = hpTargetForRound >= hpOgreLarge;

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

        foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreLarge, (int)clampedLarge))
        {
            go.GetComponent<ActorBase>().BaseHp = hpOgreLarge;
            yield return go;
        }

        foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreSmall, (int)clampedSmall))
        {
            go.GetComponent<ActorBase>().BaseHp = hpOgreSmall;
            yield return go;
        }

        foreach (var go in SpawnUtil.Random(ActorTypeEnum.BatRed, (int)clampedBat))
        {
            go.GetComponent<ActorBase>().BaseHp = hpBat;
            yield return go;
        }
    }
}
