using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemScript : MonoBehaviour
{
    public RawImage BuyButtonOverlay;
    public TextMeshProUGUI PriceLabel;
    public TextMeshProUGUI LevelLabel;

    public bool IsEnabled;

    public void Awake()
    {
        Enable(true);
    }

    public void Enable(bool enable)
    {
        BuyButtonOverlay.gameObject.SetActive(!enable);
    }

    void Update()
    {
        Enable(IsEnabled);
    }
}
