using Assets.Script.Misc;
using System;
using System.Text;
using TMPro;
using UnityEngine;

public class SettingsStatsScript : MonoBehaviour
{
    public TextMeshProUGUI TextStats;

    public string GetIncomeDescription()
    {
        string BuildLine(string left, string right)
            => $"<align=left>{left}<line-height=0>\n<align=right><color=#EEEEEE>{right}</color><line-height=1em>";

        var lifetimeIncome = SaveGame.Members.TotalIncomePassive + SaveGame.Members.TotalIncomeArena;
        var timePlayed = TimeSpan.FromSeconds(SaveGame.Members.EstimatedOnlineSeconds2);
        var timeSinceLastAscend = TimeSpan.FromSeconds(SaveGame.Members.TimeSinceLastAscend);

        var sb = new StringBuilder();
        sb.AppendLine(BuildLine("Passive income", $"{(long)Math.Round(PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier * 100)}%"));
        sb.AppendLine(BuildLine("X2 multiplier", $"{1 + PlayerUpgrades.Data.PassiveIncomeX2Multiplier:0.00}"));
        sb.AppendLine(BuildLine("Bestiary multiplier", $"{1 + PlayerUpgrades.Data.PassiveIncomeBestiaryBonuses:0.00}"));
        sb.AppendLine(BuildLine("Bought 1% multiplier", $"{1 + PlayerUpgrades.Data.PassiveIncomePercentageBonuses:0.00}"));
        sb.AppendLine(BuildLine("Mystery multiplier", $"{PlayerUpgrades.Data.PassiveIncomeTempMultiplier:0.00}"));
        sb.AppendLine(BuildLine("Rebirth cards multiplier", $"{PlayerUpgrades.Data.PassiveIncomeAscendMultiplier:0.00}"));
        sb.AppendLine(BuildLine("Diamond multiplier", $"{1 + PlayerUpgrades.Data.PassiveIncomeDiamondMultiplier:0.00}"));
        sb.AppendLine();
        sb.AppendLine(BuildLine("Max Money", Format512.Format(SaveGame.Members.MaxMoney)));
        sb.AppendLine(BuildLine("Max Income", Format512.Format(SaveGame.Members.MaxIncome)));
        sb.AppendLine(BuildLine("Total Income", Format512.Format(lifetimeIncome)));
        sb.AppendLine(BuildLine("Upgrades Bought", Format512.Format(SaveGame.Members.TotalUpgradesBought)));
        sb.AppendLine(BuildLine("X2 Bought", Format512.Format(SaveGame.Members.TotalX2UpgradesBought)));
        sb.AppendLine(BuildLine("Credits Earned", Format512.Format(SaveGame.Members.MonsterCreditsLifetime_09_08_2025)));
        sb.AppendLine(BuildLine("Rebirths", Format512.Format(SaveGame.Members.TimesAscended_09_08_2025)));
        if (SaveGame.Members.TimesAscended_09_08_2025 > 0)
        {
            sb.AppendLine(BuildLine("Time Since Last Rebirth", $"{FormatTime.Format(timeSinceLastAscend.Days, timeSinceLastAscend.Hours, timeSinceLastAscend.Minutes, useShorthand: true)}"));
        }
        else
        {
            sb.AppendLine(BuildLine("Time Since Last Rebirth", $"-"));
        }
        sb.AppendLine(BuildLine("Chests", SaveGame.Members.ChestsCollected.ToString()));
        sb.AppendLine(BuildLine("Mystery Bonuses", SaveGame.Members.MysteryCollected.ToString()));
        sb.AppendLine(BuildLine("Max Arena", Format512.Format(SaveGame.Members.MaxArena)));
        sb.AppendLine(BuildLine("Total Arenas", Format512.Format(SaveGame.Members.TotalArenas)));
        sb.AppendLine(BuildLine("Arenas Won", Format512.Format(SaveGame.Members.TotalArenasWon)));
        sb.AppendLine(BuildLine("Enemies Killed", Format512.FormatWithDecimals(SaveGame.Members.EnemiesKilled, alwaysThreeDecimalsForLargeNumbers: true)));
        sb.AppendLine(BuildLine("Total Damage", Format512.FormatWithDecimals(SaveGame.Members.DamageDone, alwaysThreeDecimalsForLargeNumbers: true)));
        sb.AppendLine(BuildLine("Max DpS", Format512.Format(SaveGame.Members.MaxDps)));

        sb.AppendLine(BuildLine("Time played", $"{FormatTime.Format(timePlayed.Days, timePlayed.Hours, timePlayed.Minutes, useShorthand: true)}"));

        return sb.ToString();
    }

    //sb.AppendLine($"Passive income: {(long)Math.Round(PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier * 100)}%");
    //sb.AppendLine();
    //sb.AppendLine($"X2 multiplier: {1 + PlayerUpgrades.Data.PassiveIncomeX2Multiplier:0.00}");
    //sb.AppendLine($"Bestiary multiplier: {1 + PlayerUpgrades.Data.PassiveIncomeBestiaryBonuses:0.00}");
    //sb.AppendLine($"Bought 1% multiplier: {1 + PlayerUpgrades.Data.PassiveIncomePercentageBonuses:0.00}");
    //sb.AppendLine($"Mystery multiplier: {PlayerUpgrades.Data.PassiveIncomeTempMultiplier:0.00}");
    //sb.AppendLine($"Rebirth cards multiplier: {PlayerUpgrades.Data.PassiveIncomeAscendMultiplier:0.00}");
    //sb.AppendLine($"Diamond multiplier: {1 + PlayerUpgrades.Data.PassiveIncomeDiamondMultiplier:0.00}");

    float _nextUpdate = 0;
    void Update()
    {
        if (G.D.GameTime < _nextUpdate)
            return;

        _nextUpdate = G.D.GameTime + 0.5f;
        TextStats.text = GetIncomeDescription();
    }
}
