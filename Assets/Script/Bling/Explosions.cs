using Assets.Script;
using System.Collections;
using UnityEngine;

public static class Explosions
{
    public static void Push(Vector3 pos, float radius, float force, float damage = 0, bool silent = false)
    {
        if (!silent)
        {
            GameManager.Instance.MakeFlash(pos, radius * 1f);
            GameManager.Instance.MakePoof(pos, 1, radius * 0.25f);
            GameManager.Instance.ShakeCamera(0.2f);
            AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.UnarmedBlast, volumeScale: 0.8f);
        }

        int aliveCount = BlackboardScript.GetEnemies(pos, radius);
        for (int i = 0; i < aliveCount; ++i)
        {
            ActorBase enemy = BlackboardScript.EnemyOverlap[i];

            var dir = enemy.transform.position - pos;
            float distance = dir.magnitude + 0.0001f;
            dir /= distance;
            dir.Normalize();

            //force = Mathf.Clamp((radius - distance) / radius * force, min: force * 0.9f, max: force);
            var push = dir * force;
            enemy.AddForce(push);
            enemy.SetSlowmotion();
            enemy.ApplyDamage(damage, push.normalized, forceModifier: 0.01f);
        }
    }

    public static IEnumerator Explode(Vector3 pos, float radius, float damage)
    {
        GameManager.Instance.MakeFlash(pos, radius * 1.5f);
        GameManager.Instance.MakePoof(pos, 6, radius * 1.5f);
        GameManager.Instance.ShakeCamera(4.0f);

        int deadCount = BlackboardScript.GetDeadEnemies(pos, radius);
        for (int i = 0; i < deadCount; ++i)
        {
            ActorBase enemy = BlackboardScript.EnemyOverlap[i];
            enemy.AddForce((enemy.transform.position - pos) * 0.25f);
        }

        int aliveCount = BlackboardScript.GetEnemies(pos, radius);
        for (int i = 0; i < aliveCount; ++i)
        {
            ActorBase enemy = BlackboardScript.EnemyOverlap[i];
            enemy.ApplyDamage(damage, enemy.transform.position - pos, forceModifier: 1.0f);
        }

        for (int i = 0; i < 10; ++i)
        {
            Vector2 rnd = RndUtil.RandomInsideUnitCircle() * radius * 0.5f;
            Vector3 flamePos = pos;
            flamePos.x += rnd.x;
            flamePos.y += rnd.y;
            GameManager.Instance.EmitFlame(flamePos, Random.value + 0.5f);
            yield return null;
        }
    }
}