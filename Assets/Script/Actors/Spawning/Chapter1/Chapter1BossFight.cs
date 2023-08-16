//using System.Collections;
//using UnityEngine;

//public static class Chapter1BossFight
//{
//    static Chapter1Controller C;

//    public static IEnumerator Run(Chapter1Controller controller)
//    {
//        C = controller;

//        C.HpBar.Show();

//        // enable player
//        G.D.PlayerScript.StopPuppet(moveToBegin: true);
//        G.D.PlayerScript.TryEnableToggledEffects();

//        while (G.D.PlayerScript.IsPuppet)
//            yield return null;

//        GameManager.Instance.Orc.SetPosition(PositionUtility.GetPointInsideArena(), startingGame: true);
//        PlayerUpgrades.Data.OrcReviveTimeMul *= 3.0f;

//        const float BossSpeed = 5.0f;

//        yield return Chapter1BossUtil.MoveBoss(C.Boss.transform, Vector2.zero + C.BossOffsetY, BossSpeed);

//        C.StartCoroutine(Chapter1BossUtil.FollowPlayer(C.Boss));
//        C.StartCoroutine(Chapter1BossUtil.ThrowFlasks(C.Boss, C.AcidFlaskProto));

//        bool armySpawned = false;

//        var actor = C.Boss.BodyTransform.GetComponent<ActorBase>();

//        while (true)
//        {
//            if (actor.IsDead)
//            {
//                yield break;
//            }

//            GameManager.Instance.Orc.SetChasePlayer(chase: true);

//            yield return Chapter1BossUtil.FireballSpiral(C.Boss.transform, 5.0f);
//            yield return new WaitForSeconds(5.0f);
//            yield return Chapter1BossUtil.Bombard(C.Boss, C.AcidFlaskProto);
//            yield return new WaitForSeconds(5.0f);

//            if (!armySpawned)
//            {
//                armySpawned = true;
//                yield return Chapter1BossUtil.SpawnArmy(C.Boss);
//            }
//        }
//    }
//}
