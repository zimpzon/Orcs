using UnityEngine;

public class AcidCollision : MonoBehaviour
{
    private void OnParticleTrigger()
    {
        const float AcidDamage = 10;
        G.D.PlayerScript.DamagePlayer(AcidDamage);
    }
}
