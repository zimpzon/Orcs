using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using static PositionUtility;

public static class Chapter1Minute06
{
    public static IEnumerable<IEnumerator> GetEvents()
    {
        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreLarge,
                startTime: new TimeSpan(0, 6, 15),
                endTime: new TimeSpan(0, 10, 0),
                startingCount: 1,
                endCount: 1,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 15.0f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreShamanStaff,
            startTime: new TimeSpan(0, 6, 0),
            endTime: new TimeSpan(0, 10, 0),
            startingCount: 2,
            endCount: 3,
            maxSpawnCountPerTick: 1,
            timeBetweenTicks: 8.0f,
            outsideScreen: true,
            SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 6, 10),
                endTime: new TimeSpan(0, 10, 0),
                startingCount: 5,
                endCount: 5,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 1.2f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 6, 10),
                endTime: new TimeSpan(0, 10, 0),
                startingCount: 25,
                endCount: 35,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 3.0f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Skeleton,
                startTime: new TimeSpan(0, 6, 10),
                endTime: new TimeSpan(0, 10, 0),
                startingCount: 1,
                endCount: 4,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 5.5f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 6, 0),
                endTime: new TimeSpan(0, 10, 0),
                spawnCountPerTick: 2,
                timeBetweenTicks: 1,
                outsideScreen: true,
                SpawnDirection.Any);

        yield break;
    }
}
