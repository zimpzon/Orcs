using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using static PositionUtility;

public static class Chapter1Minute14
{
    public static IEnumerable<IEnumerator> GetEvents()
    {
        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreShamanStaff,
            startTime: new TimeSpan(0, 14, 0),
            endTime: new TimeSpan(0, 14, 44),
            startingCount: 12,
            endCount: 12,
            maxSpawnCountPerTick: 3,
            timeBetweenTicks: 3.0f,
            SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreBandana,
                startTime: new TimeSpan(0, 14, 10),
                endTime: new TimeSpan(0, 14, 44),
                startingCount: 15,
                endCount: 20,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 3.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Skeleton,
                startTime: new TimeSpan(0, 14, 10),
                endTime: new TimeSpan(0, 14, 44),
                startingCount: 10,
                endCount: 10,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 4.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreLarge,
                startTime: new TimeSpan(0, 14, 10),
                endTime: new TimeSpan(0, 14, 44),
                startingCount: 2,
                endCount: 2,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 4.0f,
                SpawnDirection.Any);

        yield break;
    }
}
