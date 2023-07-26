using Assets.Script.Enemies;
using System.Collections;
using UnityEngine;

public class GameModeUndeadsSpawner : MonoBehaviour
{
    public void Run()
    {
        StartCoroutine(Test1());
        StartCoroutine(Test2());
        StartCoroutine(Test3());
    }

    IEnumerator Test1()
    {
        while (true)
        {
            yield return PositionUtility.SpawnGroup(ActorTypeEnum.OgreShamanStaffLarge, 1, 0.00f, outsideScreen: true, PositionUtility.GetRandomDirOutside());
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator Test2()
    {
        while (true)
        {
            yield return PositionUtility.SpawnGroup(ActorTypeEnum.Skeleton, 25, 0.00f, outsideScreen: true, PositionUtility.GetRandomDirOutside());
            yield return new WaitForSeconds(4);
        }
    }

    IEnumerator Test3()
    {
        while (true)
        {
            yield return PositionUtility.SpawnGroup(ActorTypeEnum.OrcPlain, 30, 0.00f, outsideScreen: true, PositionUtility.GetRandomDirOutside());
            yield return new WaitForSeconds(7);
        }
    }
}
