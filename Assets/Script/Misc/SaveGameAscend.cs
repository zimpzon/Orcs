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
        newSave.MonsterCreditsXp_09_08_2025 = oldSave.MonsterCreditsXp_09_08_2025;
        newSave.TimesAscended_09_08_2025 = oldSave.TimesAscended_09_08_2025;
        newSave.MonsterCredits_09_08_2025 = oldSave.MonsterCredits_09_08_2025;
        newSave.MonsterCreditsLifetime_09_08_2025 = oldSave.MonsterCreditsLifetime_09_08_2025;
        newSave.CreditBonusStartLifetime = Assets.Script.Upgrades.UpgradeProgression.CreditsEarnedWithPartial(oldSave); // credit bonus back to x2.00
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
        newSave.BoughtPercentBonusX10 = oldSave.BoughtPercentBonusX10;
        newSave.BoughtHalfEnemyHp = oldSave.BoughtHalfEnemyHp;
        newSave.BoughtHaggler1 = oldSave.BoughtHaggler1;
        newSave.BoughtHaggler2 = oldSave.BoughtHaggler2;
        newSave.BoughtHaggler3 = oldSave.BoughtHaggler3;
        newSave.BoughtBeastScholar1 = oldSave.BoughtBeastScholar1;
        newSave.BoughtBeastScholar2 = oldSave.BoughtBeastScholar2;
        newSave.BoughtX2Mastery1 = oldSave.BoughtX2Mastery1;
        newSave.BoughtX2Mastery2 = oldSave.BoughtX2Mastery2;
        newSave.BoughtX2Mastery3 = oldSave.BoughtX2Mastery3;
        newSave.BoughtFasterArena2 = oldSave.BoughtFasterArena2;
        newSave.BoughtCardDiscount = oldSave.BoughtCardDiscount;
        newSave.BoughtZapLore = oldSave.BoughtZapLore;
        newSave.BoughtChestLore = oldSave.BoughtChestLore;
        newSave.BoughtVoodooLore = oldSave.BoughtVoodooLore;
        newSave.BoughtWizardLore = oldSave.BoughtWizardLore;
        newSave.BoughtSkinCollector = oldSave.BoughtSkinCollector;
        newSave.BoughtSkinCollector2 = oldSave.BoughtSkinCollector2;
        newSave.BoughtCompletionist = oldSave.BoughtCompletionist;
        newSave.BoughtHeadStart = oldSave.BoughtHeadStart;
        newSave.BoughtScaryEarlSkin = oldSave.BoughtScaryEarlSkin;

        // Settings
        newSave.Version = oldSave.Version;
        newSave.VolumeMaster = oldSave.VolumeMaster;
        newSave.VolumeMusic = oldSave.VolumeMusic;
        newSave.VolumeSfx = oldSave.VolumeSfx;

        newSave.ShowFloatingDamageNumbers = oldSave.ShowFloatingDamageNumbers;
        newSave.ShowFloatingGoldNumbers = oldSave.ShowFloatingGoldNumbers;
        newSave.UseScientificNotation = oldSave.UseScientificNotation;
        newSave.ShowDetailsOnHover = oldSave.ShowDetailsOnHover;
        newSave.SoundEnabled = oldSave.SoundEnabled;
        newSave.SelectedSkin = oldSave.SelectedSkin;

        // Stats
        newSave.EstimatedOnlineSeconds2 = oldSave.EstimatedOnlineSeconds2;
        // Rebirth bonus: set (not added) to 5% per hour played in total.
        newSave.RebirthIncomeBonus = Assets.Script.Upgrades.UpgradeProgression.RebirthBonusPerHour * oldSave.EstimatedOnlineSeconds2 / 3600.0;
        newSave.SaveKillSwitch_CanSave = true;

        newSave.Achieved = oldSave.Achieved;
        newSave.ViewedVictoryDialog = oldSave.ViewedVictoryDialog;
        newSave.ViewedRebirthDialog = oldSave.ViewedRebirthDialog;
        newSave.BeastsSeen = oldSave.BeastsSeen;

        // Collectables
        newSave.ChestsCollected = oldSave.ChestsCollected;
        newSave.MysteryCollected = oldSave.MysteryCollected;

        // Stats
        newSave.MaxDps = oldSave.MaxDps;
        newSave.MaxArena = oldSave.MaxArena;
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


        // Head Start card: the new run starts with a minute of the best-ever passive income (mystery buff excluded).
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
