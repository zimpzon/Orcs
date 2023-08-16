using System.Collections;
using UnityEngine;

public class AcidFlaskScript : MonoBehaviour
{
    public ParticleSystem Burst;
    public float Speed = 14;
    Transform trans_;
    bool landed_ = false;

    public void Throw(Vector3 pos)
    {
        StartCoroutine(Think());
        float distance = Vector2.Distance(trans_.position, pos);
        float time = distance / Speed;
        LeanTween.move(gameObject, pos, time).setOnComplete(() => landed_ = true);
    }
        
    IEnumerator Think()
    {
        while (!landed_)
            yield return null;

        yield return new WaitForSeconds(1.0f);

        Particles.I.SpawnAcid(trans_.position);

        GetComponent<SpriteRenderer>().enabled = false;
        GameManager.Instance.MakeFlash(transform.position, 4.0f);
        GameManager.Instance.MakePoof(transform.position, 2, 1.5f);

        yield return new WaitForSeconds(0.6f);

        GameObject.Destroy(gameObject);
    }

    void Awake()
    {
        trans_ = transform;
    }
}
