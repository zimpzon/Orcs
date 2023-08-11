using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PositionUtility;

public static class Chapter1Minute03
{
    public static IEnumerable<IEnumerator> GetEvents()
    {
        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreLarge,
                startTime: new TimeSpan(0, 3, 15),
                endTime: new TimeSpan(0, 6, 0),
                startingCount: 1,
                endCount: 1,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 30.0f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreShamanStaff,
            startTime: new TimeSpan(0, 3, 0),
            endTime: new TimeSpan(0, 5, 0),
            startingCount: 1,
            endCount: 1,
            maxSpawnCountPerTick: 1,
            timeBetweenTicks: 5.0f,
            outsideScreen: true,
            SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 3, 10),
                endTime: new TimeSpan(0, 6, 0),
                startingCount: 10,
                endCount: 20,
                maxSpawnCountPerTick: 5,
                timeBetweenTicks: 1.2f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 3, 10),
                endTime: new TimeSpan(0, 6, 0),
                startingCount: 10,
                endCount: 10,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 2.0f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 3, 0),
                endTime: new TimeSpan(0, 6, 0),
                spawnCountPerTick: 1,
                timeBetweenTicks: 2,
                outsideScreen: true,
                SpawnDirection.Any);

        var yAdjust = Vector2.up * 2;
        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreShaman, despawnAtDestination: true, breakFreeAtDamage: true,
           time: new TimeSpan(0, 5, 10), RightMidOut + yAdjust, LeftMidOut, ActorForcedTargetType.Direction, w: 4, h: 6, stepX: 1, stepY: 1, pivotX: 1, pivotY: 0.5f, skewX: 0.2f, skewY: 0.2f);

        yield break;
    }
}
