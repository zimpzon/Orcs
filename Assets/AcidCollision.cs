using System.Collections.Generic;
using UnityEngine;

public class AcidCollision : MonoBehaviour
{
    private void OnParticleTrigger()
    {
        G.D.PlayerScript.DamagePlayer(1);
    }
}
