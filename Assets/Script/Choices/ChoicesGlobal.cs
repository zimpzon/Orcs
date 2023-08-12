using System.Collections.Generic;

public static class ChoicesGlobal
{
    public static List<Choice> GetGlobalChoices()
    {
        const string NameCrit = "Critical hits";

        return new List<Choice>
        {
            new Choice
            {
                Title = "Damage (1)",
                Description = "All: <color=#00ff00>+5%</color> damage",
                Apply = () =>
                {
                    PlayerUpgrades.Data.DamageMul += 0.05f;
                },
                NextLevel = new Choice
                {
                    Title = "Damage (2)",
                    Description = "All: <color=#00ff00>+5%</color> damage",
                    Apply = () =>
                    {
                    PlayerUpgrades.Data.DamageMul += 0.05f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Damage (3)",
                        Description = "All: <color=#00ff00>+5%</color> damage",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.DamageMul += 0.05f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = $"{NameCrit}, chance (1)",
                Description = $"All: Critical hits <color=#00ff00>+10%</color> chance, {ChoicesWeapon.Name} {G.D.UpgradeNegativeColorHex}-10%</color> damage",
                Apply = () =>
                {
                    PlayerUpgrades.Data.BaseCritChance += 0.1f;
                    PlayerUpgrades.Data.MagicMissileDamageMul -= 0.15f;
                },
                NextLevel = new Choice
                {
                    Title = $"{NameCrit}, chance (2)",
                    Description = $"All: Critical hits <color=#00ff00>+10%</color> chance, {ChoicesWeapon.Name} {G.D.UpgradeNegativeColorHex}-10%</color> damage",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.BaseCritChance += 0.1f;
                        PlayerUpgrades.Data.MagicMissileDamageMul -= 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{NameCrit}, chance (3)",
                        Description = $"All: Critical hits <color=#00ff00>+10%</color> chance, {ChoicesWeapon.Name} {G.D.UpgradeNegativeColorHex}-10%</color> damage",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.BaseCritChance += 0.1f;
                            PlayerUpgrades.Data.MagicMissileDamageMul -= 0.1f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = $"{NameCrit}, damage (1)",
                Description = "All: Critical hits <color=#00ff00>+20%</color> damage",
                Apply = () =>
                {
                    PlayerUpgrades.Data.CritValueMul += 0.2f;
                },
                NextLevel = new Choice
                {
                    Title = $"{NameCrit}, damage (2)",
                        Description = "All: Critical hits <color=#00ff00>+20%</color> damage",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.CritValueMul += 0.2f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{NameCrit}, damage (3)",
                        Description = "All: Critical hits <color=#00ff00>+20%</color> damage",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.CritValueMul += 0.2f;
                        },
                    }
                }
            },

        };
    }
}
