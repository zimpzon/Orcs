using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemScript : MonoBehaviour
{
    public Color DefaultBackgroundColor;
    public Color MaxedBackgroundColor;
    public Color DefaultButtonBackgroundColor;
    public Color DisabledButtonBackgroundColor;
    public ShopItemType ItemType;
    public Text Title;
    public Text Description;
    public Text Level;
    public Text ButtonText;
    public Button BuyButton;

    public void OnClick()
    {
        OnClickCallback?.Invoke(ItemType);
    }

    public void Awake()
    {
        SetIsMaxed(false);
    }

    public void SetIsMaxed(bool maxed)
    {
        GetComponent<Image>().color = maxed ? MaxedBackgroundColor : DefaultBackgroundColor;
    }

    public void SetDisableButton(bool disable)
    {
        BuyButton.GetComponent<Image>().color = disable ? DisabledButtonBackgroundColor : DefaultButtonBackgroundColor;
    }

    public Action<ShopItemType> OnClickCallback;
}
