using UnityEngine;

namespace Assets.Script.Misc
{
    public static class Zapper
    {
        public static void DoZap(Vector2 from, Vector2 to)
        {
            LightningBolts.Zap(from, to);
        }

        public static bool TryZapEnemy(Vector2 from, ActorBase actor, long damage, ActorDamageSource damageSource)
        {
            Vector2 to = actor is null ? from + (Vector2)Random.insideUnitCircle * 5 : actor.transform.position;
            DoZap(from, to);

            if (actor is null)
                return false;

            // Power Zap boosts actual zaps only (player chain zap + Witch Doctor corpse zaps both come through here),
            // not things that merely scale from zap damage, like Wizard fireballs.
            damage = (long)(damage * PlayerUpgrades.Data.PowerZapDamageMul);

            var direction = (to - from).normalized;
            GameManager.Instance.DamageEnemy(actor, damage, direction, forceModifier: 1.5f, damageSource: damageSource);

            return true;
        }
    }
}
