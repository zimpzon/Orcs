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
        ActorBase actor = other.GetComponent<ActorBase>();
        if (other.layer == GameManager.Instance.LayerEnemy)
        {
            if (actor != null)
            {
                Vector3 direction = other.transform.position - trans_.position;
                GameManager.Instance.DamageEnemy(actor, 30.0f, direction.normalized, 0.1f);
            }
        }
        else if (other.layer == GameManager.Instance.LayerPlayer)
        {
            GameManager.Instance.PlayerScript.DamagePlayer(actor);
        }
    }
}
