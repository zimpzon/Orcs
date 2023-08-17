using System;
using TMPro;
using UnityEngine;

public class HpBarScript : MonoBehaviour, IKillableObject
{
    public HpBarScript I;
    public Transform ForegroundSprite;
    [NonSerialized] public ActorBase Owner;
    public Transform ScaleRoot;
    public TextMeshPro HpText;
    public float HiddenY;
    public float ShownY;

    public Transform FillTransform;

    void Awake()
    {
        I = this;
    }

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
        HpText.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
    }

    void Update()
    {
        if (Owner != null)
            SetHp(Owner.Hp, Owner.BaseHp);
    }
}
