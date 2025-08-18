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
        newSave.LastSeenUtcStr = oldSave.LastSeenUtcStr;

        // Ascending
        newSave.MonsterCreditsXp_09_08_2025 = oldSave.MonsterCreditsXp_09_08_2025;
        newSave.TimesAscended_09_08_2025 = oldSave.TimesAscended_09_08_2025;
        newSave.MonsterCredits_09_08_2025 = oldSave.MonsterCredits_09_08_2025;
        newSave.MonsterCreditsLifetime_09_08_2025 = oldSave.MonsterCreditsLifetime_09_08_2025;
        newSave.DiamondCount_09_08_2025 = oldSave.DiamondCount_09_08_2025;

        // Permanent upgrades
        newSave.BoughtPassiveX2_1 = oldSave.BoughtPassiveX2_1;
        newSave.BoughtPassiveX2_2 = oldSave.BoughtPassiveX2_2;
        newSave.BoughtPassiveX4_1 = oldSave.BoughtPassiveX4_1;

        // Settings
        newSave.Version = oldSave.Version;
        newSave.VolumeMaster = oldSave.VolumeMaster;
        newSave.VolumeMusic = oldSave.VolumeMusic;
        newSave.VolumeSfx = oldSave.VolumeSfx;

        newSave.ShowFloatingDamageNumbers = oldSave.ShowFloatingDamageNumbers;
        newSave.ShowFloatingGoldNumbers = oldSave.ShowFloatingGoldNumbers;

        // Stats
        newSave.EstimatedOnlineSeconds2 = oldSave.EstimatedOnlineSeconds2;
        newSave.SaveKillSwitch_CanSave = true;

        newSave.Achieved = oldSave.Achieved;
        newSave.BeastsSeen = oldSave.BeastsSeen;

        // Progress

        // Replace old save with new save
        SaveGame.Members = newSave;
        return newSave;
    }
}
