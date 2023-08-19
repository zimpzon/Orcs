using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using static PositionUtility;

public static class Chapter1Minute06
{
    public static IEnumerable<IEnumerator> GetEvents()
    {
        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreBandanaGun, despawnAtDestination: true, breakFreeAtDamage: false,
            time: new TimeSpan(0, 6, 3), RightMidOut, LeftMidOut, ActorForcedTargetType.Direction, w: 4, h: 2, stepX: 1, stepY: 1, pivotX: 1, pivotY: 0.5f, skewX: 0.2f);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.OgreBandana,
                startTime: new TimeSpan(0, 6, 5),
                endTime: new TimeSpan(0, 10, 0),
                spawnCountPerTick: 2,
                timeBetweenTicks: 3,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreLarge,
                startTime: new TimeSpan(0, 6, 25),
                endTime: new TimeSpan(0, 10, 0),
                startingCount: 1,
                endCount: 1,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 10.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreShamanStaff,
            startTime: new TimeSpan(0, 6, 10),
            endTime: new TimeSpan(0, 10, 0),
            startingCount: 3,
            endCount: 3,
            maxSpawnCountPerTick: 1,
            timeBetweenTicks: 5.0f,
            SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.BatWhite,
                startTime: new TimeSpan(0, 6, 10),
                endTime: new TimeSpan(0, 10, 0),
                startingCount: 10,
                endCount: 25,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 3.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 6, 15),
                endTime: new TimeSpan(0, 10, 0),
                startingCount: 40,
                endCount: 40,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 3.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Skeleton,
                startTime: new TimeSpan(0, 6, 10),
                endTime: new TimeSpan(0, 10, 0),
                startingCount: 1,
                endCount: 2,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 10.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 6, 40),
                endTime: new TimeSpan(0, 10, 0),
                spawnCountPerTick: 1,
                timeBetweenTicks: 3,
                SpawnDirection.Any);

        yield return SpawnUtil.Single(
                ActorTypeEnum.OgreShamanStaffLarge,
                time: new TimeSpan(0, 9, 0),
                SpawnDirection.Top);

        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreShaman, despawnAtDestination: true, breakFreeAtDamage: true,
           time: new TimeSpan(0, 7, 20), TopMidOut, BottomMidOut, ActorForcedTargetType.Direction, w: 4, h: 16, stepX: 1, stepY: 1, pivotX: 0.5f, pivotY: 0.0f, skewX: 0.0f, skewY: 0.0f);

        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreBandanaGun, despawnAtDestination: true, breakFreeAtDamage: false,
            time: new TimeSpan(0, 8, 0), LeftMidOut, RightMidOut, ActorForcedTargetType.Direction, w: 2, h: 16, stepX: 1, stepY: 1, pivotX: 1, pivotY: 0.5f, skewX: 0.0f);

        yield break;
    }
}
