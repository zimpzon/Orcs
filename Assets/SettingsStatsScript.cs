using System;
using System.Text;
using TMPro;
using UnityEngine;

public class SettingsStatsScript : MonoBehaviour
{
    public TextMeshProUGUI TextStats;

    public string GetIncomeDescription()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Passive income: {(long)Math.Round(PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier * 100)}%");
        sb.AppendLine();
        sb.AppendLine($"X2 multiplier: {1 + PlayerUpgrades.Data.PassiveIncomeX2Multiplier:0.00}");
        sb.AppendLine($"Bestiary multiplier: {1 + PlayerUpgrades.Data.PassiveIncomeBestiaryBonuses:0.00}");
        sb.AppendLine($"Bought 1% multiplier: {1 + PlayerUpgrades.Data.PassiveIncomePercentageBonuses:0.00}");
        sb.AppendLine($"Mystery multiplier: {PlayerUpgrades.Data.PassiveIncomeTempMultiplier:0.00}");
        sb.AppendLine($"Rebirth cards multiplier: {PlayerUpgrades.Data.PassiveIncomeAscendMultiplier:0.00}");
        sb.AppendLine($"Diamond multiplier: {1 + PlayerUpgrades.Data.PassiveIncomeDiamondMultiplier:0.00}");
        return sb.ToString();
    }

    void Update()
    {
        TextStats.text = GetIncomeDescription();
    }
}
