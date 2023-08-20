using UnityEngine;

public class PoisonDaggersCollision : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        var enemy = other.GetComponent<ActorBase>();
        if (enemy != null)
        {
            enemy.OnPaintballHit(PoisonDaggers.Instance.Color, WeaponPaintball.SlowTime);
        }
    }
}
