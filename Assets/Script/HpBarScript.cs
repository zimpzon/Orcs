using TMPro;
using UnityEngine;

public class HpBarScript : MonoBehaviour
{
    public Transform ForegroundSprite;
    public Transform ScaleRoot;
    public TextMeshPro HpText;
    public long CurrentHp = 0;
    public long MaxHp = 100;
    public Transform FillTransform;

    public long AddHp(long amount)
    {
        SetHp(CurrentHp + amount, MaxHp);
        return CurrentHp;
    }

    public void SetHp(long current, long max)
    {
        if (current < 0) current = 0;

        CurrentHp = current;
        MaxHp = max;
        var scale = FillTransform.localScale;
        scale.x = max == 0 ? 0 : (float)current / max;
        FillTransform.localScale = scale;
        HpText.text = $"{Format512.FormatWithDecimals(current, alwaysThreeDecimalsForLargeNumbers: true)}/{Format512.FormatWithDecimals(max, alwaysThreeDecimalsForLargeNumbers: true)}";
        //HpText.text = $"{(long)current}/{(long)max}";
    }
}
