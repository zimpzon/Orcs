using System;
using System.Collections;
using UnityEngine;

public class GameModeUndeadsSpawner : MonoBehaviour
{
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
        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 0, 0),
                endTime: new TimeSpan(0, 1, 0),
                count: 50,
                countPerTick: 5,
                timeBetweenTicks: 0.2f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.TopOrBottom)
            );

        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 0, 10),
                endTime: new TimeSpan(0, 1, 0),
                count: 50,
                countPerTick: 5,
                timeBetweenTicks: 0.2f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.LeftOrRight)
            );

        while (true)
        {
            yield return null;
        }
    }
}

