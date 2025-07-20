using Assets.Script.Upgrades;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public UpgradeItemScript ClickDamage;
    public UpgradeItemScript GoldPerKnife;
    public UpgradeItemScript KnifeDamage;
    public UpgradeItemScript KnifeCd;
    public UpgradeItemScript HeroRunspeed;
    public UpgradeItemScript GoldPerRound;

    public void UpdateAllUpgrades()
    {
        ClickDamageManager.UpdateAll();
        GoldPerKnifeThrowManager.UpdateAll();
        KnifeCdManager.UpdateAll();
        KnifeDamageManager.UpdateAll();
        HeroRunSpeedManager.UpdateAll();
        GoldPerRoundManager.UpdateAll();
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
        else if (upgradeUiScript == HeroRunspeed)
        {
            return HeroRunSpeedManager.GetText();
        }
        else if (upgradeUiScript == GoldPerRound)
        {
            return GoldPerRoundManager.GetText();
        }
        else
        {
            return $"unknown UpgradeItemScript: {upgradeUiScript.name}";
        }
    }

    public void UpdateUpgradeUi()
    {
        ClickDamageManager.UpdateUi();
        GoldPerKnifeThrowManager.UpdateUi();
        KnifeCdManager.UpdateUi();
        KnifeDamageManager.UpdateUi();
        HeroRunSpeedManager.UpdateUi();
        GoldPerRoundManager.UpdateUi();
    }

    public void OnBuyClickDamage()
    {
        ClickDamageManager.OnBuy();
        ClickDamage.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyGoldPerKnife()
    {
        GoldPerKnifeThrowManager.OnBuy();
        GoldPerKnife.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyKnifeCooldown()
    {
        KnifeCdManager.OnBuy();
        KnifeCd.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyKnifeDamage()
    {
        KnifeDamageManager.OnBuy();
        KnifeDamage.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyHeroRunspeed()
    {
        HeroRunSpeedManager.OnBuy();
        HeroRunspeed.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyGoldPerRound()
    {
        GoldPerRoundManager.OnBuy();
        GoldPerRound.SetPopupText();
        UpdateAllUpgrades();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateUpgradeUi();
    }
}
