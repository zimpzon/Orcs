using System;
using System.Collections;
using UnityEngine;

public class GameModeUndeadsSpawner : MonoBehaviour
{
    // 20 minutes
    // 0:00 1:30 min of small ogre, start X, increase to Y
    // 1:00 5 min of ogre, start X, increase to Y
    // 2:00 large ogre formation, 

    public void Run()
    {
        StartCoroutine(Test1());
    }

    public void Stop()
    {
        StopAllCoroutines();
    }

    IEnumerator Test1()
    {
        // small 00:00 - 5:00
        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 0, 0),
                endTime: new TimeSpan(0, 3, 0),
                maintainCount: 20,
                maintainCountIncreasePerSec: 0.25f,
                spawnCountPerTick: 5,
                timeBetweenTicks: 1.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.TopOrBottom)
            );

        // normal 01:30 - 5:00
        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 0, 20),
                //startTime: new TimeSpan(0, 1, 30),
                endTime: new TimeSpan(0, 5, 0),
                maintainCount: 500,
                maintainCountIncreasePerSec: 10.0f,
                spawnCountPerTick: 20,
                timeBetweenTicks: 0.5f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.LeftOrRight)
            );

        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.OgreBandana,
                startTime: new TimeSpan(0, 0, 30),
                //startTime: new TimeSpan(0, 1, 30),
                endTime: new TimeSpan(0, 5, 0),
                maintainCount: 100,
                maintainCountIncreasePerSec: 1.0f,
                spawnCountPerTick: 10,
                timeBetweenTicks: 0.5f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.LeftOrRight)
            );

        // spam for 1 min
        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 5, 0),
                endTime: new TimeSpan(0, 10, 0),
                maintainCount: 100,
                maintainCountIncreasePerSec: 0,
                spawnCountPerTick: 5,
                timeBetweenTicks: 0.25f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.TopOrBottom)
            );

        while (true)
        {
            yield return null;
        }
    }
}

