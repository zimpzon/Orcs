using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using static PositionUtility;

public static class Chapter1Minute10
{
    public static IEnumerable<IEnumerator> GetEvents()
    {
        yield return SpawnUtil.SpawnAndMaintain(
               ActorTypeEnum.OgreLarge,
               startTime: new TimeSpan(0, 10, 0),
               endTime: new TimeSpan(0, 14, 0),
               startingCount: 1,
               endCount: 1,
               maxSpawnCountPerTick: 1,
               timeBetweenTicks: 15.0f,
               outsideScreen: true,
               SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreShamanStaff,
            startTime: new TimeSpan(0, 10, 0),
            endTime: new TimeSpan(0, 14, 0),
            startingCount: 3,
            endCount: 3,
            maxSpawnCountPerTick: 1,
            timeBetweenTicks: 15.0f,
            outsideScreen: true,
            SpawnDirection.Any);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 10, 0),
                endTime: new TimeSpan(0, 14, 0),
                spawnCountPerTick: 3,
                timeBetweenTicks: 1,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Skeleton,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                startingCount: 1,
                endCount: 2,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 5.5f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreBandana,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                startingCount: 20,
                endCount: 25,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 1.0f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield break;
    }
}
