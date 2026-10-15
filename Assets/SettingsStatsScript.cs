using Assets.Script.Misc;
using Assets.Script.Upgrades;
using System;
using System.Text;
using TMPro;
using UnityEngine;

public class SettingsStatsScript : MonoBehaviour
{
    public TextMeshProUGUI TextStats;

    public string GetIncomeDescription()
    {
        // Grey label left, bold value right (same line). Value colors are per kind of stat; all white for now.
        const string Multiplier = "#FFFFFF";
        const string Money = "#FFFFFF";
        const string Rebirth = "#FFFFFF";
        const string Plain = "#FFFFFF";

        string BuildLine(string left, string right, string valueColor = Plain, string labelColor = "#BBBBBB")
            => $"<align=left><color={labelColor}>{left}</color><line-height=0>\n<align=right><b><color={valueColor}>{right}</color></b><line-height=1em>";

        // Raw multiplier value: two decimals with thousands separators up to a million ("1,234.56"), then the
        // game's short big-number format ("12.3M") so huge late-game values stay short.
        string Num(double value)
        {
            if (value < 1_000_000)
                return DisplayNumberFormat.Format(value, "#,0.00");
            if (value < 9e18)
                return Format512.Format((long)value);
            return DisplayNumberFormat.Format(value, "0.00e0");
        }

        string Mul(double value) => $"x{Num(value)}";
        string Pct(double value) => $"+{DisplayNumberFormat.Format(Math.Round(value * 100), "#,0")}%"; // whole percents

        // Small grey detail under a line (no right-aligned value), e.g. the parts of the card bonus multiplier.
        string DetailLine(string text) => $"<align=left><size=80%><color=#BBBBBB>    {text}</color></size>"; // same grey as the labels

        var lifetimeIncome = SaveGame.Members.TotalIncomePassive + SaveGame.Members.TotalIncomeArena;
        // Counted every minute (GameManager.UpdateTimePlayed) but shown in whole 10 minutes.
        static TimeSpan RoundedTo10Min(double seconds) => TimeSpan.FromSeconds(Math.Floor(seconds / 600) * 600);
        var timePlayed = RoundedTo10Min(SaveGame.Members.EstimatedOnlineSeconds2);
        var timeSinceLastAscend = RoundedTo10Min(SaveGame.Members.TimeSinceLastAscend);

        string strTextMoney = $"${Format512.Format(SaveGame.Members.MaxMoney)}";

        string strTotalIncome = $"${Format512.Format(lifetimeIncome)}";

        string strMaxIncome = $"${Format512.Format(SaveGame.Members.MaxIncome)}";

        var sb = new StringBuilder();
        sb.AppendLine(BuildLine("<b>Income</b>", $"x{Num(PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier)}", Multiplier, "#EEEEEE"));
        sb.AppendLine(BuildLine("X2 multiplier", Mul(1 + PlayerUpgrades.Data.PassiveIncomeX2Multiplier), Multiplier));
        // Bestiary, skin and completion add up into one multiplier (PlayerUpgrades.PassiveIncomeCardBonuses).
        sb.AppendLine(BuildLine("Card bonus multiplier", Mul(1 + PlayerUpgrades.Data.PassiveIncomeCardBonuses), Multiplier));
        sb.AppendLine(DetailLine($"Bestiary {Pct(PlayerUpgrades.Data.PassiveIncomeBestiaryBonuses)}"));
        sb.AppendLine(DetailLine($"Skins {Pct(PlayerUpgrades.Data.PassiveIncomeSkinBonus)}"));
        sb.AppendLine(DetailLine($"Completion {Pct(PlayerUpgrades.Data.PassiveIncomeCompletionBonus)}"));
        sb.AppendLine(BuildLine("Diamond multiplier", Mul(1 + UpgradeProgression.DiamondIncomeBonus()), Multiplier));
        sb.AppendLine(BuildLine("Bought 1% multiplier", Mul(1 + PlayerUpgrades.Data.PassiveIncomePercentageBonuses), Multiplier));
        sb.AppendLine(BuildLine("Mystery multiplier", Mul(PlayerUpgrades.Data.PassiveIncomeTempMultiplier), Multiplier));
        sb.AppendLine(BuildLine("Card X2 multiplier", Mul(PlayerUpgrades.Data.PassiveIncomeAscendMultiplier), Multiplier));
        sb.AppendLine("<size=50%> </size>");
        sb.AppendLine(BuildLine("Max Money", strTextMoney, Money));
        sb.AppendLine(BuildLine("Max Income", strMaxIncome, Money));
        //sb.AppendLine(BuildLine("Total Income", strTotalIncome));
        sb.AppendLine(BuildLine("Upgrades Bought", Format512.Format(SaveGame.Members.TotalUpgradesBought)));
        sb.AppendLine(BuildLine("X2 Bought", Format512.Format(SaveGame.Members.TotalX2UpgradesBought)));
        sb.AppendLine(BuildLine("1% Bonuses Bought", Format512.Format(SaveGame.Members.TotalLevelPctBought)));
        sb.AppendLine(BuildLine("Credits Earned", Format512.Format(SaveGame.Members.MonsterCreditsLifetime_09_08_2025), Rebirth));
        sb.AppendLine(BuildLine("Rebirths", Format512.Format(SaveGame.Members.TimesAscended_09_08_2025), Rebirth));
        if (SaveGame.Members.TimesAscended_09_08_2025 > 0)
        {
            sb.AppendLine(BuildLine("Time Since Last Rebirth", $"{FormatTime.Format(timeSinceLastAscend.Days, timeSinceLastAscend.Hours, timeSinceLastAscend.Minutes, useShorthand: true)}", Rebirth));
        }
        else
        {
            sb.AppendLine(BuildLine("Time Since Last Rebirth", "-", Rebirth));
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

    float _nextUpdate = 0;
    void Update()
    {
        if (G.D.GameTime < _nextUpdate)
            return;

        _nextUpdate = G.D.GameTime + 0.5f;
        TextStats.text = GetIncomeDescription();
    }
}
