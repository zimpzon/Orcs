using Assets.Script;
using System.Collections;
using UnityEngine;

public static class Explosions
{
    public static void Push(Vector3 pos, float radius, float force, float damage = 0)
    {
        GameManager.Instance.MakeFlash(pos, radius * 1f);
        GameManager.Instance.MakePoof(pos, 1, radius * 0.25f);
        GameManager.Instance.ShakeCamera(1.0f);
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.UnarmedBlast, volumeScale: 0.5f);

        int aliveCount = BlackboardScript.GetEnemies(pos, radius);
        for (int i = 0; i < aliveCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.Enemies[idx];

            var dir = enemy.transform.position - pos;
            float distance = dir.magnitude + 0.0001f;
            dir /= distance;
            dir.Normalize();

            float baseForce = Mathf.Clamp((radius - distance) * distance, 2.0f, 100.0f) * 0.25f;
            var push = dir * baseForce * force;
            enemy.AddForce(push);
            enemy.SetSlowmotion();
            enemy.ApplyDamage(damage, push.normalized, forceModifier: 0.1f, headshot: false);
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
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.DeadEnemies[idx];
            enemy.AddForce((enemy.transform.position - pos) * 0.25f);
            //            enemy.Explode(2.0f + Random.value * 2);
        }

        int aliveCount = BlackboardScript.GetEnemies(pos, radius);
        for (int i = 0; i < aliveCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.Enemies[idx];
            enemy.ApplyDamage(damage, enemy.transform.position - pos, 1.0f, true);
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