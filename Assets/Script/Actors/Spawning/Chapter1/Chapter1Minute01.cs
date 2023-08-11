using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using static PositionUtility;

public static class Chapter1Minute01
{
    // 0 - 2 min, maintain count of lvl 1-2 enemies
    // 2 - 5 min, maintain count of mostly lvl 2 enemies, + maintain large chasing you
    // 5 - 8 min, maintain lvl 2 enemies, + maintain two large chasing you + maintain 1 caster
    // 8 - 10 min, 
    public static IEnumerable<IEnumerator> GetEvents()
    {
        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 0, 0),
                endTime: new TimeSpan(0, 1, 58),
                startingCount: 1,
                endCount: 100,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 0.2f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 0, 10),
                endTime: new TimeSpan(0, 1, 58),
                startingCount: 20,
                endCount: 40,
                maxSpawnCountPerTick: 5,
                timeBetweenTicks: 3.0f,
                outsideScreen: true,
                SpawnDirection.Any);
    }
}
