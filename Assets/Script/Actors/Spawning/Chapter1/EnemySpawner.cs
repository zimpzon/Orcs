using Assets.Script.Actors.Spawning;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    public static IEnumerable<ActorBase> GetEnemies(long level)
    {
        const long MaxEnemies = 100;
        long HpPerLevel = 50;
        long HpBase = 100;

        // Enemy hp will only increase every X levels or we will just see
        // the same lone enemy getting more and more Hp.
        long levelBucket = (level / 100) * 100 + 1;
        long hpTargetForRound = (HpPerLevel * level) + HpBase;

        long GetRandomizedHp(long baseHp)
        {
            float factor = UnityEngine.Random.Range(0.5f, 1.0f);
            return (long)(baseHp * factor);
        }

        long hpBat = GetRandomizedHp(20 * levelBucket + (level * 2));
        long hpOgreSmall = GetRandomizedHp(50 * levelBucket + (level * 5));
        long hpOgreLarge = GetRandomizedHp(1000 * levelBucket + (level * 100));

        long remainingHp = hpTargetForRound;
        long totalEnemies = 0;

        // Try spawning OgreLarge first (highest HP)
        long ogreLargeCount = remainingHp / hpOgreLarge;

        // Randomly remove some of this size to dynamically get more of the smaller ones
        ogreLargeCount -= Random.Range(0, (int)ogreLargeCount / 5);

        if (ogreLargeCount + totalEnemies > MaxEnemies)
        {
            ogreLargeCount = MaxEnemies - totalEnemies;
        }
        totalEnemies += ogreLargeCount;
        remainingHp -= ogreLargeCount * hpOgreLarge;

        foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreLarge, (int)ogreLargeCount))
        {
            go.GetComponent<ActorBase>().BaseHp = hpOgreLarge;
            yield return go;
        }

        // Then try OgreSmall
        long ogreSmallCount = remainingHp / hpOgreSmall;

        // Randomly remove some of this size to dynamically get more of the smaller ones
        ogreSmallCount -= Random.Range(0, (int)ogreSmallCount / 5);

        if (ogreSmallCount + totalEnemies > MaxEnemies)
        {
            ogreSmallCount = MaxEnemies - totalEnemies;
        }
        totalEnemies += ogreSmallCount;
        remainingHp -= ogreSmallCount * hpOgreSmall;

        foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreSmall, (int)ogreSmallCount))
        {
            go.GetComponent<ActorBase>().BaseHp = hpOgreSmall;
            yield return go;
        }

        // Finally use Bats to fill remaining HP
        long batCount = remainingHp / hpBat;
        if (batCount + totalEnemies > MaxEnemies)
        {
            batCount = MaxEnemies - totalEnemies;
        }

        foreach (var go in SpawnUtil.Random(ActorTypeEnum.BatRed, (int)batCount))
        {
            go.GetComponent<ActorBase>().BaseHp = hpBat;
            yield return go;
        }
    }
}
