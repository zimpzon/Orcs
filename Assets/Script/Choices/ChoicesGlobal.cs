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
                Description = $"All: Critical hits <color=#00ff00>+8%</color> chance",
                Apply = () =>
                {
                    PlayerUpgrades.Data.BaseCritChance += 0.08f;
                },
                NextLevel = new Choice
                {
                    Title = $"{NameCrit}, chance (2)",
                    Description = $"All: Critical hits <color=#00ff00>+8%</color> chance",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.BaseCritChance += 0.08f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{NameCrit}, chance (3)",
                        Description = $"All: Critical hits <color=#00ff00>+8%</color> chance",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.BaseCritChance += 0.08f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = $"{NameCrit}, damage (1)",
                Description = "All: Critical hits <color=#00ff00>+30%</color> damage",
                Apply = () =>
                {
                    PlayerUpgrades.Data.CritValueMul += 0.3f;
                },
                NextLevel = new Choice
                {
                    Title = $"{NameCrit}, damage (2)",
                        Description = "All: Critical hits <color=#00ff00>+30%</color> damage",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.CritValueMul += 0.3f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{NameCrit}, damage (3)",
                        Description = "All: Critical hits <color=#00ff00>+30%</color> damage",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.CritValueMul += 0.3f;
                        },
                    }
                }
            },

        };
    }
}
