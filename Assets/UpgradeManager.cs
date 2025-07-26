using Assets.Script.Upgrades;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public Color ColorPassiveValues = Color.white;
    public Color ColorArenaValues = Color.white;

    public UpgradeItemScript ClickDamage;
    public UpgradeItemScript KnifeDamage;
    public UpgradeItemScript GoldPerRound;
    public UpgradeItemScript KnifeCd;
    public UpgradeItemScript WitchDoctor;
    public UpgradeItemScript GoldPerKnife;

    public void UpdateAllUpgrades()
    {
        ClickDamageManager.UpdateAll();
        KnifeDamageManager.UpdateAll();
        ArenaGoldManager.UpdateAll();
        KnifeCdManager.UpdateAll();
        WitchDoctorManager.UpdateAll();
        GoldPerKnifeThrowManager.UpdateAll();
    }

    public string GetText(UpgradeItemScript upgradeUiScript)
    {
        string text = "";
        if (upgradeUiScript == ClickDamage)
        {
            text = ClickDamageManager.GetText();
        }
        else if (upgradeUiScript == KnifeDamage)
        {
            text = KnifeDamageManager.GetText();
        }
        else if (upgradeUiScript == GoldPerRound)
        {
            text = ArenaGoldManager.GetText();
        }
        else if (upgradeUiScript == KnifeCd)
        {
            text = KnifeCdManager.GetText();
        }
        else if (upgradeUiScript == WitchDoctor)
        {
            text = WitchDoctorManager.GetText();
        }
        else if (upgradeUiScript == GoldPerKnife)
        {
            text = GoldPerKnifeThrowManager.GetText();
        }
        else
        {
            text = $"unknown UpgradeItemScript: {upgradeUiScript.name}";
        }

        string colorPassiveHex = $"#{ColorUtility.ToHtmlStringRGB(ColorPassiveValues)}";
        string colorArenaHex = $"#{ColorUtility.ToHtmlStringRGB(ColorArenaValues)}";
        text = text.Replace("COLOR-PASSIVE", colorPassiveHex);
        text = text.Replace("COLOR-ARENA", colorArenaHex);

        return text;
    }

    public Decimal256 GetTotalPassiveIncome()
    {
        float incomeFactorPerFrame = GameManager.Instance.GetIncomeFactorPerFrame();

        SaveGame.Members.TotalIncomeClickDamage += ClickDamageManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeKnifeDamage += KnifeDamageManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeGoldPerRound += ArenaGoldManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeKnifeCd += KnifeCdManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeWitchDoctor += WitchDoctorManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeGoldPerKnifeThrow += GoldPerKnifeThrowManager.PassiveIncome() * incomeFactorPerFrame;

        Decimal256 fullSum = 0;
        fullSum += ClickDamageManager.PassiveIncome();
        fullSum += KnifeDamageManager.PassiveIncome();
        fullSum += ArenaGoldManager.PassiveIncome();
        fullSum += KnifeCdManager.PassiveIncome();
        fullSum += WitchDoctorManager.PassiveIncome();
        fullSum += GoldPerKnifeThrowManager.PassiveIncome();

        SaveGame.Members.TotalIncomePassive += fullSum * incomeFactorPerFrame;

        return fullSum;
    }

    public void UpdateUpgradeUi()
    {
        ClickDamageManager.UpdateUi();
        KnifeDamageManager.UpdateUi();
        ArenaGoldManager.UpdateUi();
        KnifeCdManager.UpdateUi();
        WitchDoctorManager.UpdateUi();
        GoldPerKnifeThrowManager.UpdateUi();
    }

    public void OnBuyClickDamage()
    {
        ClickDamageManager.OnBuy();
        ClickDamage.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyKnifeDamage()
    {
        KnifeDamageManager.OnBuy();
        KnifeDamage.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyArenaGold()
    {
        ArenaGoldManager.OnBuy();
        GoldPerRound.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyKnifeCooldown()
    {
        KnifeCdManager.OnBuy();
        KnifeCd.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyWitchDoctorDamage()
    {
        WitchDoctorManager.OnBuy();
        WitchDoctor.SetPopupText();
        UpdateAllUpgrades();
    }

    public void OnBuyGoldPerKnife()
    {
        GoldPerKnifeThrowManager.OnBuy();
        GoldPerKnife.SetPopupText();
        UpdateAllUpgrades();
    }

    private void UpdatePlayerUpgrades()
    {
        ClickDamageManager.UpdatePlayerUpgrades();
        KnifeDamageManager.UpdatePlayerUpgrades();
        ArenaGoldManager.UpdatePlayerUpgrades();
        KnifeCdManager.UpdatePlayerUpgrades();
        WitchDoctorManager.UpdatePlayerUpgrades();
        GoldPerKnifeThrowManager.UpdatePlayerUpgrades();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdatePlayerUpgrades();
        UpdateUpgradeUi();
    }
}
