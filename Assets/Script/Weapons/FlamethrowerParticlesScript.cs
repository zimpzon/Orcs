using UnityEngine;

public class FlamethrowerParticlesScript : MonoBehaviour
{
    Transform trans_;

    private void Start()
    {
        trans_ = this.transform;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.layer == GameManager.Instance.LayerEnemy)
        {
            ActorBase enemy = other.GetComponent<ActorBase>();
            if (enemy != null)
            {
                Vector3 direction = other.transform.position - trans_.position;
                GameManager.Instance.DamageEnemy(enemy, 30.0f, direction.normalized, 0.1f);
            }
        }
        else if (other.layer == GameManager.Instance.LayerPlayer)
        {
            GameManager.Instance.PlayerScript.KillPlayer();
        }
    }
}
