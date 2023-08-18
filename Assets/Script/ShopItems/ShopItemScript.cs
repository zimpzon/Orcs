using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemScript : MonoBehaviour
{
    public Color DefaultBackgroundColor;
    public Color MaxedBackgroundColor;
    public ShopItemType ItemType;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI ExtraInfo;
    public Text Level;
    public Text ButtonText;
    public Button BuyButton;
    public Button RefundButton;

    public void OnBuyClick()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.Menu);
        OnBuyClickCallback?.Invoke(ItemType);
    }

    public void OnRefundClick()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.Menu);
        OnRefundClickCallback?.Invoke(ItemType);
    }

    public void SetButtonStates(bool enableBuyButton, bool enableRefundButton)
    {
        BuyButton.GetComponent<Image>().color = enableBuyButton ? G.D.DefaultButtonBackgroundColor : G.D.DisabledButtonBackgroundColor;
        BuyButton.enabled = enableBuyButton;

        RefundButton.GetComponent<Image>().color = enableRefundButton ? G.D.DefaultButtonBackgroundColor : G.D.DisabledButtonBackgroundColor;
        RefundButton.enabled = enableRefundButton;
    }

    public Action<ShopItemType> OnBuyClickCallback;
    public Action<ShopItemType> OnRefundClickCallback;
}
