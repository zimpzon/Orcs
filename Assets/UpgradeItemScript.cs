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

    long _latestPrice = -1;
    long _latestLevel = -1;

    public void SetPrice(long price)
    {
        if (price == _latestPrice)
            return;

        PriceLabel.text = $"${price}";
        _latestPrice = price;
    }

    public void SetLevel(long level, long maxLevel = -1)
    {
        if (level == _latestLevel)
            return;

        if (level == 0)
        {
            LevelLabel.text = "";
        }
        else if (maxLevel == -1)
        {
            LevelLabel.text = $"{level}";
        }
        else
        {
            LevelLabel.text = $"{level}/{maxLevel}";
        }
        _latestPrice = level;
    }

    private void OnMouseEnter()
    {
        Debug.DrawLine(Input.mousePosition, Vector2.zero, Color.green, 2.0f);
    }

    private void OnMouseExit()
    {
        Debug.DrawLine(Input.mousePosition, Vector2.zero, Color.magenta, 2.0f);
    }

    void Update()
    {
        Enable(IsEnabled);
    }
}
