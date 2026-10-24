static class SaveGameAscend
{
    // This creates a new save and keeps all the settings that should carry over.
    public static SaveGameMembers AscendSaveGame()
    {
        var oldSave = SaveGame.Members;
        var newSave = new SaveGameMembers();

        // ---------------------- Copy permanent data ----------------------
        newSave.PlayerId = oldSave.PlayerId;
        newSave.UserId = oldSave.UserId;
        newSave.SaveId = oldSave.SaveId;
        newSave.LastSeenUtcStr = oldSave.LastSeenUtcStr;

        // Ascending
        // Partial progress toward the next credit carries over (lifetime is unchanged, so the price and the % stay the
        // same), but at most 99%: a finished credit held back by the speed cap must not land for free after a rebirth.
        Decimal512 maxCarry = Assets.Script.Upgrades.UpgradeProgression.MonsterCreditXpForNextLevel(oldSave.MonsterCreditsLifetime_09_08_2025 + 1) * 0.99;
        newSave.MonsterCreditsXp_09_08_2025 = oldSave.MonsterCreditsXp_09_08_2025 > maxCarry ? maxCarry : oldSave.MonsterCreditsXp_09_08_2025;
        newSave.TimesAscended_09_08_2025 = oldSave.TimesAscended_09_08_2025;
        newSave.MonsterCredits_09_08_2025 = oldSave.MonsterCredits_09_08_2025;
        newSave.MonsterCreditsLifetime_09_08_2025 = oldSave.MonsterCreditsLifetime_09_08_2025;
        newSave.CreditBonusStartLifetime = Assets.Script.Upgrades.UpgradeProgression.CreditsEarnedWithPartial(oldSave); // credit bonus back to full
        newSave.DiamondCount_09_08_2025 = oldSave.DiamondCount_09_08_2025;

        // Permanent upgrades
        newSave.BoughtPassiveX2_1 = oldSave.BoughtPassiveX2_1;
        newSave.BoughtPassiveX2_2 = oldSave.BoughtPassiveX2_2;
        newSave.BoughtPassiveX4_1 = oldSave.BoughtPassiveX4_1;
        newSave.BoughtPassiveX4_2 = oldSave.BoughtPassiveX4_2;
        newSave.BoughtPassiveX4_3 = oldSave.BoughtPassiveX4_3;
        newSave.BoughtPassiveX6_1 = oldSave.BoughtPassiveX6_1;
        newSave.BoughtPassiveX7_1 = oldSave.BoughtPassiveX7_1;
        newSave.BoughtFasterMystery = oldSave.BoughtFasterMystery;
        newSave.BoughtFasterArena = oldSave.BoughtFasterArena;
        newSave.BoughtFasterArena2 = oldSave.BoughtFasterArena2;
        newSave.BoughtShinyDiamonds1 = oldSave.BoughtShinyDiamonds1;
        newSave.BoughtShinyDiamonds2 = oldSave.BoughtShinyDiamonds2;
        newSave.BoughtShinyDiamonds3 = oldSave.BoughtShinyDiamonds3;
        newSave.BoughtShinyDiamonds4 = oldSave.BoughtShinyDiamonds4;
        newSave.BoughtShinyDiamonds5 = oldSave.BoughtShinyDiamonds5;
        newSave.BoughtHaggler = oldSave.BoughtHaggler;
        newSave.BoughtHeadStart = oldSave.BoughtHeadStart;
        newSave.BoughtSkinCollector = oldSave.BoughtSkinCollector;
        newSave.BoughtBeastScholar = oldSave.BoughtBeastScholar;
        newSave.BoughtCompletionist = oldSave.BoughtCompletionist;
        newSave.BoughtScaryEarlSkin = oldSave.BoughtScaryEarlSkin;

        // Settings
        newSave.Version = oldSave.Version;

        newSave.ShowFloatingDamageNumbers = oldSave.ShowFloatingDamageNumbers;
        newSave.ShowFloatingGoldNumbers = oldSave.ShowFloatingGoldNumbers;
        newSave.ShowDetailsOnHover = oldSave.ShowDetailsOnHover;
        newSave.HideVictoryBanner = oldSave.HideVictoryBanner;
        newSave.ShowHealthBars = oldSave.ShowHealthBars;
        newSave.PctBonusBuyAmount = oldSave.PctBonusBuyAmount;
        newSave.SelectedSkin = oldSave.SelectedSkin;

        // Stats
        newSave.EstimatedOnlineSeconds2 = oldSave.EstimatedOnlineSeconds2;
        newSave.CompletionSeconds = oldSave.CompletionSeconds;
        newSave.CompletionRebirths = oldSave.CompletionRebirths;
        newSave.ScaryEarlSeconds = oldSave.ScaryEarlSeconds;
        newSave.SaveKillSwitch_CanSave = true;

        newSave.Achieved = oldSave.Achieved;
        newSave.ViewedVictoryDialog = oldSave.ViewedVictoryDialog;
        newSave.ViewedRebirthDialog = oldSave.ViewedRebirthDialog;
        newSave.ViewedTipsDialog = oldSave.ViewedTipsDialog;
        newSave.ViewedExpandedTiers = oldSave.ViewedExpandedTiers;
        newSave.FogTouched = oldSave.FogTouched;
        newSave.ViewedBestiaryAfterScaryEarl = oldSave.ViewedBestiaryAfterScaryEarl;
        newSave.ViewedProgressDialog = oldSave.ViewedProgressDialog;
        newSave.ScaryEarlDefeated = oldSave.ScaryEarlDefeated;
        newSave.ScaryEarlDefeatedLevel = oldSave.ScaryEarlDefeatedLevel;
        newSave.BeastsSeen = oldSave.BeastsSeen;
        newSave.BeastKills = oldSave.BeastKills;

        // Collectables
        newSave.ChestsCollected = oldSave.ChestsCollected;
        newSave.MysteryCollected = oldSave.MysteryCollected;

        // Stats
        newSave.MaxDps = oldSave.MaxDps;
        newSave.MaxArena = oldSave.MaxArena;
        newSave.MaxDiamonds = oldSave.MaxDiamonds;
        newSave.EnemiesKilled = oldSave.EnemiesKilled; // lifetime (stats, the stats panel)
        newSave.MaxUpgradeTiersBought = System.Math.Max(oldSave.MaxUpgradeTiersBought, UpgradeTierList.CountBought()); // SaveGame.Members is still the old run here
        newSave.TotalArenas = oldSave.TotalArenas;
        newSave.TotalArenasWon = oldSave.TotalArenasWon;
        newSave.SuperFastClears = oldSave.SuperFastClears;
        newSave.MaxIncome = oldSave.MaxIncome;
        newSave.MaxCreditIncome = oldSave.MaxCreditIncome;
        newSave.MaxMoney = oldSave.MaxMoney;
        newSave.MaxCredits = oldSave.MaxCredits;
        newSave.TotalUpgradesBought = oldSave.TotalUpgradesBought;
        newSave.TotalX2UpgradesBought = oldSave.TotalX2UpgradesBought;
        newSave.TotalLevelPctBought = oldSave.TotalLevelPctBought;

        // Head Start card: the new run starts with 25 minutes of the best-ever passive income (mystery buff excluded).
        if (oldSave.BoughtHeadStart)
        {
            Decimal512 headStart = oldSave.MaxCreditIncome * Assets.Script.Upgrades.UpgradeProgression.HeadStartSeconds;
            if (headStart > newSave.Money)
                newSave.Money = headStart;
        }

        // Replace old save with new save
        SaveGame.Members = newSave;
        return newSave;
    }
}
