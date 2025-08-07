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
        newSave.MonsterCreditsXp = oldSave.MonsterCreditsXp;
        newSave.TimesAscendedTemp = oldSave.TimesAscendedTemp;
        newSave.MonsterCreditsTemp = oldSave.MonsterCreditsTemp;
        newSave.MonsterCreditsLifetimeTemp = oldSave.MonsterCreditsLifetimeTemp;
        newSave.DiamondCount = oldSave.DiamondCount;

        // Permanent upgrades
        newSave.BoughtPassiveX2_1 = oldSave.BoughtPassiveX2_1;
        newSave.BoughtPassiveX2_2 = oldSave.BoughtPassiveX2_2;
        newSave.BoughtPassiveX4_1 = oldSave.BoughtPassiveX4_1;

        // Settings
        newSave.Version = oldSave.Version;
        newSave.VolumeMaster = oldSave.VolumeMaster;
        newSave.VolumeMusic = oldSave.VolumeMusic;
        newSave.VolumeSfx = oldSave.VolumeSfx;

        // Stats
        newSave.EstimatedOnlineSeconds2 = oldSave.EstimatedOnlineSeconds2;

        // Safeguard flag (was already there)
        newSave.SaveKillSwitch_CanSave = oldSave.SaveKillSwitch_CanSave;

        // Replace old save with new save
        SaveGame.Members = newSave;
        return newSave;
    }
}
