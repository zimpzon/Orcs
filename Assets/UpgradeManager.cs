using Assets.Script.Upgrades;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public UpgradeItemScript ClickDamage;
    public UpgradeItemScript GoldPerKnife;
    public UpgradeItemScript KnifeDamage;
    public UpgradeItemScript KnifeCd;
    public UpgradeItemScript GoldPerRound;

    public void UpdateAllUpgrades()
    {
        ClickDamageManager.UpdateAll();
        GoldPerKnifeThrowManager.UpdateAll();
        KnifeCdManager.UpdateAll();
        KnifeDamageManager.UpdateAll();
        ArenaGoldManager.UpdateAll();
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
        else if (upgradeUiScript == GoldPerRound)
        {
            return ArenaGoldManager.GetText();
        }
        else
        {
            return $"unknown UpgradeItemScript: {upgradeUiScript.name}";
        }
    }

    public Decimal256 GetTotalPassiveIncome()
    {
        float incomeFactorPerFrame = GameManager.Instance.GetIncomeFactorPerFrame();

        SaveGame.Members.TotalIncomeClickDamage += ClickDamageManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeGoldPerKnifeThrow += GoldPerKnifeThrowManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeKnifeCd += KnifeCdManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeKnifeDamage += KnifeDamageManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeGoldPerRound += ArenaGoldManager.PassiveIncome() * incomeFactorPerFrame;

        Decimal256 fullSum = 0;
        fullSum += ClickDamageManager.PassiveIncome();
        fullSum += GoldPerKnifeThrowManager.PassiveIncome();
        fullSum += KnifeCdManager.PassiveIncome();
        fullSum += KnifeDamageManager.PassiveIncome();
        fullSum += ArenaGoldManager.PassiveIncome();

        SaveGame.Members.TotalIncomePassive += fullSum * incomeFactorPerFrame;

        return fullSum;
    }

    public void UpdateUpgradeUi()
    {
        ClickDamageManager.UpdateUi();
        GoldPerKnifeThrowManager.UpdateUi();
        KnifeCdManager.UpdateUi();
        KnifeDamageManager.UpdateUi();
        ArenaGoldManager.UpdateUi();
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

    public void OnBuyGoldPerRound()
    {
        ArenaGoldManager.OnBuy();
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
