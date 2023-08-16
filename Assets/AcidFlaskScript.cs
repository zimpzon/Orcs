using System.Collections;
using UnityEngine;

public class AcidFlaskScript : MonoBehaviour, IKillableObject
{
    public SpriteRenderer WarningSprite;
    Transform trans_;
    bool landed_ = false;

    public void Throw(Vector3 pos, float speed)
    {
        gameObject.SetActive(true);
        StartCoroutine(Think());

        float distance = Vector2.Distance(trans_.position, pos);
        float time = distance / speed;
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
        WarningSprite.enabled = false;

        yield return new WaitForSeconds(0.6f);

        Kill();
    }

    void Awake()
    {
        trans_ = transform;
    }

    public void Kill()
    {
        GameObject.Destroy(gameObject);
    }
}
