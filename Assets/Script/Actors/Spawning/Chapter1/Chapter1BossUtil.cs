using Assets.Script.Actors.Spawning;
using Assets.Script.Misc;
using System.Collections;
using UnityEngine;

public static class Chapter1BossUtil
{
    public static IEnumerator MoveBoss(Transform boss, Vector2 dst, float speed)
    {
        bool bossIsAtPosition = false;

        float distance = Vector2.Distance(boss.transform.position, dst);
        float time = distance / speed;

        LeanTween.moveLocal(boss.gameObject, dst, time).setOnComplete(() => bossIsAtPosition = true);

        while (!bossIsAtPosition)
            yield return null;
    }

    public static IEnumerator ThrowGold(ActorReaperBoss boss)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int k = 0; k < 10; ++k)
                GameManager.Instance.ThrowPickups(AutoPickUpType.Money, boss.BodyTransform.position, 1, 1, forceScale: UnityEngine.Random.value * 1 + 0.5f);

            yield return new WaitForSeconds(0.25f);
        }
    }

    public static IEnumerator DeathSequence(ActorReaperBoss boss)
    {
        SaveGame.Members.Chapter1BossKilled++;

        PlayerUpgrades.Data.MagicMissileCdMul = 9999;

        MusicManagerScript.Instance.StopMusic();
        yield return new WaitForSeconds(1.0f);

        yield return boss.Speak("I can't move!", 1.5f);
        yield return boss.Speak("What have you done??", 1.5f);
        yield return boss.Speak("My hounds will avenge me!", 1.5f);
        yield return boss.Speak("...", 0.5f);
        yield return boss.Speak("Forget I said that", 1.5f);
        yield return boss.Speak("", 0.0f);

        float a = 1.0f;
        var baseCol = boss.BodyRenderer.color;
        float nextPoof = 0;
        var poofBase = boss.BodyTransform.position;

        boss.StartCoroutine(ThrowGold(boss));

        while (a > 0)
        {
            baseCol.a = a;
            boss.BodyRenderer.color = baseCol;
            a -= G.D.GameDeltaTime * 0.2f;
            if (a < 0)
                a = 0;

            if (G.D.GameTime > nextPoof)
            {
                var pos = poofBase;
                pos.y += UnityEngine.Random.value * 3;
                pos.x += UnityEngine.Random.value * 2;
                GameManager.Instance.MakePoof(boss.BodyTransform.position, 3, 2.0f);
                GameManager.Instance.MakeFlash(boss.BodyTransform.position, 2.0f);
                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.UnarmedBlast, volumeScale: 1.0f);
                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.SqueakyDie, volumeScale: 1.0f, pitch: UnityEngine.Random.value * 0.5f + 0.5f);

                nextPoof = G.D.GameTime + UnityEngine.Random.value * 0.2f + 0.35f;
            }

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        LeanTween.move(boss.ScytheTransform.gameObject, boss.ScytheTransform.position + Vector3.down * 1.25f, 0.5f).setEaseInQuart();
        
        LeanTween.rotateZ(boss.ScytheTransform.gameObject, 48, 0.5f).setEaseInQuart();

        yield return new WaitForSeconds(1.0f);

        MusicManagerScript.Instance.PlayGameMusic(GameManager.Instance.CurrentGameModeData.Music);
    }

    public static IEnumerator SpawnArmy(ActorReaperBoss boss)
    {
        yield return boss.GetComponent<ActorReaperBoss>().Speak("HAHA! Reinforcements are coming!", pause: 1, sound: false);
        yield return boss.GetComponent<ActorReaperBoss>().Speak("The best!", pause: 0.5f, sound: false);
        yield return boss.GetComponent<ActorReaperBoss>().Speak("The strongest!", pause: 0.5f, sound: false);
        yield return boss.GetComponent<ActorReaperBoss>().Speak("The fastest!", pause: 0.5f, sound: false);
        yield return boss.GetComponent<ActorReaperBoss>().Speak("The deadest!", pause: 0.5f, sound: false);
        yield return boss.GetComponent<ActorReaperBoss>().Speak("", pause: 0, sound: false);

        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreSmall, despawnAtDestination: false, breakFreeAtDamage: false,
            time: null, PositionUtility.LeftMidOut + Vector2.up * 0.5f, Vector2.zero + Vector2.right * 10, ActorForcedTargetType.Absolute, w: 4, h: 15, stepX: 1, stepY: 1, pivotX: 0.5f, pivotY: 0.5f);
    }

    public static IEnumerator FollowPlayer(ActorReaperBoss Boss)
    {
        const float BossSpeed = 1.0f;
        while (true)
        {
            float chaseEnd = G.D.GameTime + UnityEngine.Random.value * 1;
            while (G.D.GameTime < chaseEnd)
            {
                float distance = Vector2.Distance(Boss.transform.position, G.D.PlayerPos);
                if (distance < 1.0f)
                    break;

                var bossPos = Boss.transform.position;
                var direction = G.D.PlayerPos.x < bossPos.x ? Vector2.left : Vector2.right;

                bossPos.x += (direction * G.D.GameDeltaTime * BossSpeed).x;
                Boss.transform.position = bossPos;

                yield return null;
            }

            float pauseEnd = G.D.GameTime + UnityEngine.Random.value * 3;
            while (G.D.GameTime < pauseEnd)
            {
                yield return null;
            }

            yield return null;
        }
    }

    public static IEnumerator ThrowFlasks(ActorReaperBoss Boss, AcidFlaskScript flaskProto)
    {
        var delay = new WaitForSeconds(2);
        while (true)
        {
            yield return delay;

            var flask = GameObject.Instantiate(flaskProto.gameObject).GetComponent<AcidFlaskScript>();
            flask.transform.position = Boss.BodyTransform.position;
            flask.Throw(G.D.PlayerPos + (Vector3)UnityEngine.Random.insideUnitCircle * 2, speed: 10);
        }
    }

    public static IEnumerator Bombard(ActorReaperBoss Boss, AcidFlaskScript flaskProto)
    {
        yield return Boss.Speak("Flaskapult Detonare!", pause: 1, sound: false);
        yield return Boss.Speak("", pause: 0, sound: false);

        Boss.Fly();
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 15; ++i)
        {
            var flask = GameObject.Instantiate(flaskProto.gameObject).GetComponent<AcidFlaskScript>();
            flask.transform.position = Boss.BodyTransform.position;
            flask.Throw(G.D.PlayerPos + (Vector3)UnityEngine.Random.insideUnitCircle * 3, speed: 14);

            yield return new WaitForSeconds(0.5f);
        }

        Boss.Land();
    }

    public static IEnumerator FireballSpiral(Transform boss, float time)
    {
        yield return boss.GetComponent<ActorReaperBoss>().Speak("PyroCircum  Vortex!", pause: 1, sound: false);
        yield return boss.GetComponent<ActorReaperBoss>().Speak("", pause: 0, sound: false);

        var delay = new WaitForSeconds(0.1f);

        float angle = 0;
        float endTime = G.D.GameTime + time;

        while (G.D.GameTime < endTime)
        {
            Vector2 direction = MathUtil.DegreeToVector2(angle);
            angle += G.D.GameDeltaTime * 1500;

            direction.Normalize();

            Vector3 muzzlePoint = direction * 0.6f;

            ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
            basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
            basic.Type = ProjectileManager.ProjectileType.HarmsPlayer;

            basic.Speed = 3.0f;
            basic.Damage = 50.0f;
            basic.MaxDistance = 20.0f;
            basic.Radius = 0.3f;
            Vector3 scale = basic.SpriteInfo.Transform.localScale;
            scale.x = 1.5f;
            scale.y = 1.5f;
            scale.z = 1.0f;

            basic.Position = boss.position + muzzlePoint;
            basic.SpriteInfo.Renderer.sprite = SpriteData.Instance.ShamanProjectile;
            basic.SpriteInfo.Renderer.sortingLayerID = GameManager.Instance.SortLayerTopEffects;
            basic.Direction = direction;
            basic.Color = new Color(1.0f, 1.0f, 1.0f);
            basic.DieTime = 0.0f;
            basic.SpriteInfo.Transform.localScale = scale;
            float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            basic.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

            ProjectileManager.Instance.Fire(basic);
            AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.AudioData.EnemyShoot, 0.5f);

            yield return delay;
        }
    }
}
