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
        newSave.AscendXp = oldSave.AscendXp;
        newSave.AscendLevelTemp = oldSave.AscendLevelTemp;
        newSave.MonsterCredits = oldSave.MonsterCredits;

        // Cheat detection
        newSave.HasMoneyCheated = oldSave.HasMoneyCheated;
        newSave.HasArenaIncreaseCheated = oldSave.HasArenaIncreaseCheated;

        // Settings
        newSave.Version = oldSave.Version;
        newSave.VolumeMaster = oldSave.VolumeMaster;
        newSave.VolumeMusic = oldSave.VolumeMusic;
        newSave.VolumeSfx = oldSave.VolumeSfx;

        // Safeguard flag
        newSave.SaveKillSwitch_CanSave = oldSave.SaveKillSwitch_CanSave;

        // Replace old save with new save
        SaveGame.Members = newSave;

        return newSave;
    }
}
