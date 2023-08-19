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
        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreBandanaGun, despawnAtDestination: true, breakFreeAtDamage: false,
            time: new TimeSpan(0, 3, 1), RightMidOut, LeftMidOut, ActorForcedTargetType.Direction, w: 3, h: 2, stepX: 1, stepY: 1, pivotX: 1, pivotY: 0.5f, skewX: 0.2f);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.OgreBandana,
                startTime: new TimeSpan(0, 4, 0),
                endTime: new TimeSpan(0, 5, 30),
                spawnCountPerTick: 2,
                timeBetweenTicks: 5,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreLarge,
                startTime: new TimeSpan(0, 3, 15),
                endTime: new TimeSpan(0, 5, 30),
                startingCount: 1,
                endCount: 1,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 30.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreShamanStaff,
            startTime: new TimeSpan(0, 3, 0),
            endTime: new TimeSpan(0, 5, 0),
            startingCount: 1,
            endCount: 1,
            maxSpawnCountPerTick: 1,
            timeBetweenTicks: 5.0f,
            SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 3, 0),
                endTime: new TimeSpan(0, 5, 30),
                startingCount: 10,
                endCount: 15,
                maxSpawnCountPerTick: 5,
                timeBetweenTicks: 1.2f,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 3, 0),
                endTime: new TimeSpan(0, 5, 30),
                startingCount: 30,
                endCount: 40,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 2.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 3, 0),
                endTime: new TimeSpan(0, 5, 30),
                spawnCountPerTick: 1,
                timeBetweenTicks: 2,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreBandanaGun, despawnAtDestination: true, breakFreeAtDamage: false,
            time: new TimeSpan(0, 3, 30), TopMidOut - Vector2.left * 5, BottomMidOut - Vector2.left * 2, ActorForcedTargetType.Direction, w: 4, h: 2, stepX: 1, stepY: 1, pivotX: 0.5f, pivotY: 0.5f, skewX: 0.75f);

        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreBandanaGun, despawnAtDestination: true, breakFreeAtDamage: false,
            time: new TimeSpan(0, 3, 30), TopMidOut + Vector2.left * 5, BottomMidOut + Vector2.left * 2, ActorForcedTargetType.Direction, w: 4, h: 2, stepX: 1, stepY: 1, pivotX: 0.5f, pivotY: 0.5f, skewX: -0.75f);

        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreShaman, despawnAtDestination: true, breakFreeAtDamage: true,
           time: new TimeSpan(0, 3, 30), RightMidOut, LeftMidOut, ActorForcedTargetType.Direction, w: 2, h: 2, stepX: 1, stepY: 1, pivotX: 1, pivotY: 0.5f, skewX: 0.2f, skewY: 0.2f);

        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreShaman, despawnAtDestination: true, breakFreeAtDamage: true,
           time: new TimeSpan(0, 5, 10), BottomMidOut, TopMidOut, ActorForcedTargetType.Direction, w: 8, h: 2, stepX: 1, stepY: 1, pivotX: 1, pivotY: 0.0f, skewX: 0.0f, skewY: 0.0f);

        yield return SpawnUtil.Message(new TimeSpan(0, 5, 48), "the flapping of wings draws closer", G.D.UpgradePositiveColor);
        yield return SpawnUtil.ActionAtTime(new TimeSpan(0, 5, 40), () => SpawnUtil.FleeAllActors());

        yield return SpawnUtil.Swarm(
            ActorTypeEnum.BatWhite,
            startTime: new TimeSpan(0, 5, 54),
            endTime: new TimeSpan(0, 6, 6),
            spawnCountPerTick: 15,
            timeBetweenTicks: 1,
            SpawnDirection.Any);

        yield break;
    }
}
