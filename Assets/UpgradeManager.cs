using Assets.Script.Upgrades;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public Color ColorPassiveValues = Color.white;
    public Color ColorArenaValues = Color.white;

    public UpgradeItemScript ClickDamage;
    public UpgradeItemScript GoldPerKnife;
    public UpgradeItemScript KnifeDamage;
    public UpgradeItemScript KnifeCd;
    public UpgradeItemScript WitchDoctor;
    public UpgradeItemScript GoldPerRound;

    public void UpdateAllUpgrades()
    {
        ClickDamageManager.UpdateAll();
        GoldPerKnifeThrowManager.UpdateAll();
        KnifeCdManager.UpdateAll();
        KnifeDamageManager.UpdateAll();
        ArenaGoldManager.UpdateAll();
        WitchDoctorManager.UpdateAll();
    }

    public string GetText(UpgradeItemScript upgradeUiScript)
    {
        string text = "";
        if (upgradeUiScript == ClickDamage)
        {
            text = ClickDamageManager.GetText();
        }
        else if (upgradeUiScript == GoldPerKnife)
        {
            text = GoldPerKnifeThrowManager.GetText();
        }
        else if (upgradeUiScript == KnifeDamage)
        {
            text = KnifeDamageManager.GetText();
        }
        else if (upgradeUiScript == KnifeCd)
        {
            text = KnifeCdManager.GetText();
        }
        else if (upgradeUiScript == WitchDoctor)
        {
            text = WitchDoctorManager.GetText();
        }
        else if (upgradeUiScript == GoldPerRound)
        {
            text = ArenaGoldManager.GetText();
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
        SaveGame.Members.TotalIncomeGoldPerKnifeThrow += GoldPerKnifeThrowManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeKnifeCd += KnifeCdManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeKnifeDamage += KnifeDamageManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeGoldPerRound += WitchDoctorManager.PassiveIncome() * incomeFactorPerFrame;
        SaveGame.Members.TotalIncomeGoldPerRound += ArenaGoldManager.PassiveIncome() * incomeFactorPerFrame;

        Decimal256 fullSum = 0;
        fullSum += ClickDamageManager.PassiveIncome();
        fullSum += GoldPerKnifeThrowManager.PassiveIncome();
        fullSum += KnifeCdManager.PassiveIncome();
        fullSum += KnifeDamageManager.PassiveIncome();
        fullSum += WitchDoctorManager.PassiveIncome();
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
        WitchDoctorManager.UpdateUi();
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

    public void OnBuyWitchDoctorDamage()
    {
        WitchDoctorManager.OnBuy();
        WitchDoctor.SetPopupText();
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
