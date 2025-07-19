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
        GoldPerKnifeThrowManager.UpdateAll();
        KnifeCdManager.UpdateAll();
        KnifeDamageManager.UpdateAll();
    }

    public string GetText(UpgradeItemScript upgradeUiScript)
    {
        if (upgradeUiScript == ClickDamage)
        {
            return ClickDamageManager.GetText();
        }
        else if (upgradeUiScript == GoldPerKnife)
        {
            return GoldPerKnifeThrowManager.GetText();
        }
        else if (upgradeUiScript == KnifeDamage)
        {
            return KnifeDamageManager.GetText();
        }
        else if (upgradeUiScript == KnifeCd)
        {
            return KnifeCdManager.GetText();
        }
        else
        {
            return $"unknown UpgradeItemScript: {upgradeUiScript.name}";
        }
    }

    public void UpdateUpgradeUiButtons()
    {
        ClickDamageManager.UpdateUi();
        GoldPerKnifeThrowManager.UpdateUi();
        KnifeCdManager.UpdateUi();
        KnifeDamageManager.UpdateUi();
    }

    public void OnBuyClickDamage()
    {
        SaveGame.Members.Money -= ClickDamageManager.PriceForNext();
        SaveGame.Members.LevelClickDamage++;
        UpdateAllUpgrades();
    }

    public void OnBuyGoldPerKnife()
    {
        SaveGame.Members.Money -= GoldPerKnifeThrowManager.PriceForNext();
        SaveGame.Members.LevelGoldPerKnifeThrown++;
        UpdateAllUpgrades();
    }

    public void OnBuyKnifeCooldown()
    {
        SaveGame.Members.Money -= ClickDamageManager.PriceForNext();
        SaveGame.Members.LevelKnifeCooldown++;
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
