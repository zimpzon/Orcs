using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemScript : MonoBehaviour
{
    public ShopItemType ItemType;
    public Text Title;
    public Text Description;
    public Text ButtonText;
    public Button BuyButton;

    public void OnClick()
    {
        OnClickCallback?.Invoke(ItemType);
    }

    public Action<ShopItemType> OnClickCallback;
}
