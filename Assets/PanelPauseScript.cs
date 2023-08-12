using UnityEngine;
using UnityEngine.UI;

public class PanelPauseScript : MonoBehaviour
{
    public Text TextSpeedInfo;
    public Button SpeedButton;

    private void OnEnable()
    {
        SetSpeedInfo();
    }

    void SetSpeedInfo()
    {
        float speedIncrease = PlayerUpgrades.Data.TimeScale - 1;
        bool isIncreasedSpeed = speedIncrease > 0;
        TextSpeedInfo.text = isIncreasedSpeed ?
            $"Game is running at {G.D.UpgradePositiveColorHex}+{Mathf.RoundToInt(speedIncrease * 100)}%</color> speed" :
            $"Game is running at default speed";

        SpeedButton.enabled = isIncreasedSpeed;
        SpeedButton.GetComponent<Image>().color = isIncreasedSpeed ? G.D.DefaultButtonBackgroundColor : G.D.DisabledButtonBackgroundColor;
    }

    public void OnResetSpeedClick()
    {
        PlayerUpgrades.Data.TimeScale = 1.0f;
        SetSpeedInfo();
    }
}
