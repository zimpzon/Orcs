using UnityEngine;

public class HpBarScript : MonoBehaviour, IKillableObject
{
    public Transform HpBar;
    public float HiddenY;
    public float ShownY;

    void Awake()
    {
        Kill();
    }

    public void Kill()
    {
        gameObject.SetActive(false);
        LeanTween.moveLocalY(HpBar.gameObject, HiddenY, 0);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        LeanTween.moveLocalY(HpBar.gameObject, ShownY, 0.5f);
        SetHp(0, 100);
    }

    public void SetHp(float current, float max)
    {
        var scale = HpBar.transform.localScale;
        scale.x = current / max;
    }
}
