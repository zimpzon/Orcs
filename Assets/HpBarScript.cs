using UnityEngine;

public class HpBarScript : MonoBehaviour, IKillableObject
{
    public Transform ForegroundSprite;
    public ActorBase Owner;
    public float HiddenY;
    public float ShownY;

    public Transform FillTransform;

    public void Kill()
    {
        gameObject.SetActive(false);
        LeanTween.moveLocalY(gameObject, HiddenY, 0);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        LeanTween.moveLocalY(gameObject, ShownY, 0.5f);
        SetHp(0, 100);
    }

    public void SetHp(float current, float max)
    {
        var scale = FillTransform.localScale;
        scale.x = current / max;
        FillTransform.localScale = scale;
    }

    void Update()
    {
        SetHp(Owner.Hp, Owner.BaseHp);
    }
}
