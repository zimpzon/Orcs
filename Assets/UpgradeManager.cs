using Assets.Script.Upgrades;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public UpgradeItemScript ClickDamage;
    public UpgradeItemScript GoldPerKnife;
    public UpgradeItemScript KnifeDamage;
    public UpgradeItemScript KnifeCd;

    public void UpdateAllUpgrades()
    {
        ClickDamageManager.UpdateAll();
    }

    public void UpdateUpgradeUiButtons()
    {
        ClickDamageManager.UpdateUi();
    }

    public void OnBuyClickDamage()
    {
        SaveGame.Members.Money -= ClickDamageManager.PriceForNext();
        SaveGame.Members.LevelClickDamage++;
        UpdateAllUpgrades();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateUpgradeUiButtons();
    }
}
