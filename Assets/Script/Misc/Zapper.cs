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
                ArenaBoundsScript.Instance.LineRendererBaseWidth, to: 0.0f, time: 0.3f)
                .setOnUpdate((float val) =>
                {
                    ArenaBoundsScript.Instance.LineRenderer.startWidth = val;
                    ArenaBoundsScript.Instance.LineRenderer.endWidth = val;
                });
        }

        public static bool TryZapEnemy(Vector2 from, ActorBase actor, long damage, ActorDamageSource damageSource)
        {
            Vector2 to = actor is null ? from + (Vector2)Random.insideUnitCircle * 5 : actor.transform.position;
            DoZap(from, to);

            if (actor is null)
                return false;

            var direction = (to - from).normalized;
            GameManager.Instance.DamageEnemy(actor, damage, direction, forceModifier: 1.5f, damageSource: damageSource);

            return true;
        }
    }
}
