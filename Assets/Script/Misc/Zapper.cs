using UnityEngine;

namespace Assets.Script.Misc
{
    public static class Zapper
    {
        public static void DoZap(Vector2 from, Vector2 to)
        {
            Trails.DrawJaggedTrail(from, to, ArenaBoundsScript.Instance.LineRenderer);

            LeanTween.cancel(ArenaBoundsScript.Instance.LineRenderer.gameObject);
            LeanTween.value(
                ArenaBoundsScript.Instance.LineRenderer.gameObject,
                ArenaBoundsScript.Instance.LineRendererBaseWidth, to: 0.0f, time: 0.5f)
                .setOnUpdate((float val) =>
                {
                    ArenaBoundsScript.Instance.LineRenderer.startWidth = val;
                    ArenaBoundsScript.Instance.LineRenderer.endWidth = val;
                });
        }

        public static bool TryZapEnemy(Vector2 from, ActorBase actor, long damage, bool floatingDamage = true)
        {
            Vector2 to = actor is null ? from + (Vector2)Random.insideUnitCircle * 5 : actor.transform.position;
            DoZap(from, to);

            if (actor is null)
                return false;

            var direction = (to - from).normalized;
            GameManager.Instance.DamageEnemy(actor, damage, direction, 1.5f);

            if (floatingDamage)
            {
                FloatingTextSpawner.Instance.Spawn(
                    to + Vector2.up * 0.25f,
                    $"-{damage}",
                    Color.red,
                    speed: 0.75f,
                    timeToLive: 1.0f,
                    fontStyle: TMPro.FontStyles.Bold);
            }

            return true;
        }
    }
}
