using System;

// All upgrade tiers in shop order, with display name, level and X2 level. Single source for anything that lists or
// counts tiers (X2 popup, progress popup). Add a line here whenever a new upgrade tier is added to the shop.
public static class UpgradeTierList
{
    public static readonly (string Name, Func<long> Level, Func<long> X2)[] Tiers =
    {
        ("Chain Zapping",   () => SaveGame.Members.LevelClickDamage,        () => SaveGame.Members.LevelClickDamageX2),
        ("Dagger Damage",   () => SaveGame.Members.LevelKnifeDamage,        () => SaveGame.Members.LevelKnifeDamageX2),
        ("Gold Value",      () => SaveGame.Members.LevelMoneyPerGold,       () => SaveGame.Members.LevelMoneyPerGoldX2),
        ("Dagger Cooldown", () => SaveGame.Members.LevelKnifeCd,            () => SaveGame.Members.LevelKnifeCdX2),
        ("Witch Doctor",    () => SaveGame.Members.LevelWitchDoctor,        () => SaveGame.Members.LevelWitchDoctorX2),
        ("Gold Per Dagger", () => SaveGame.Members.LevelGoldPerKnifeThrown, () => SaveGame.Members.LevelGoldPerKnifeThrownX2),
        ("Wizard",          () => SaveGame.Members.LevelWizard,             () => SaveGame.Members.LevelWizardX2),
        ("Master Wizard",   () => SaveGame.Members.LevelHoarder,            () => SaveGame.Members.LevelHoarderX2),
        ("Frenzy",          () => SaveGame.Members.LevelZapDamage,          () => SaveGame.Members.LevelZapDamageX2),
        ("Necromancer",     () => SaveGame.Members.LevelMoneyMaker,         () => SaveGame.Members.LevelMoneyMakerX2),
        ("Dagger Master",   () => SaveGame.Members.LevelDaggerMaster,       () => SaveGame.Members.LevelDaggerMasterX2),
        ("Necro Ninja",     () => SaveGame.Members.LevelNecroNinja,         () => SaveGame.Members.LevelNecroNinjaX2),
        ("Skull Crusher",   () => SaveGame.Members.LevelSkullCrusher,       () => SaveGame.Members.LevelSkullCrusherX2),
        ("Bountiful",       () => SaveGame.Members.LevelChestMaster,        () => SaveGame.Members.LevelChestMasterX2),
        ("Voidgazer",       () => SaveGame.Members.LevelVoidgazer,          () => SaveGame.Members.LevelVoidgazerX2),
        ("Smart Daggers",   () => SaveGame.Members.LevelSmartDaggers,       () => SaveGame.Members.LevelSmartDaggersX2),
        ("Windwalker",      () => SaveGame.Members.LevelFastFeet,           () => SaveGame.Members.LevelFastFeetX2),
        ("Crypt Master",    () => SaveGame.Members.LevelCryptMaster,        () => SaveGame.Members.LevelCryptMasterX2),
        ("Angry Fireballs", () => SaveGame.Members.LevelSmartFireballs,     () => SaveGame.Members.LevelSmartFireballsX2),
        ("Beefy Earl",      () => SaveGame.Members.LevelBeefyEarl,          () => SaveGame.Members.LevelBeefyEarlX2),
        ("Critical Strike", () => SaveGame.Members.LevelCriticalStrike,     () => SaveGame.Members.LevelCriticalStrikeX2),
        ("Power Zap",       () => SaveGame.Members.LevelPowerZap,           () => SaveGame.Members.LevelPowerZapX2),
        ("Skull Slicer",    () => SaveGame.Members.LevelSkullSlicer,        () => SaveGame.Members.LevelSkullSlicerX2),
        ("Storm Lord",      () => SaveGame.Members.LevelStormLord,        () => SaveGame.Members.LevelStormLordX2),
    };
}
