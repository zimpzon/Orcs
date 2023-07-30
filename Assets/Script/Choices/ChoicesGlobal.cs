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
                Description = "<color=#00ff00>+5%</color> to all damage",
                Apply = () =>
                {
                    PlayerUpgrades.Data.DamageMul += 0.05f;
                },
                NextLevel = new Choice
                {
                    Title = "Damage (2)",
                    Description = "<color=#00ff00>+5%</color> to all damage",
                    Apply = () =>
                    {
                    PlayerUpgrades.Data.DamageMul += 0.05f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Damage (3)",
                        Description = "<color=#00ff00>10%</color> to all damage",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.DamageMul += 0.1f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = $"{NameCrit}, chance (1)",
                Description = "<color=#00ff00>5%</color> more critical hits",
                Apply = () =>
                {
                    PlayerUpgrades.Data.CritChanceMul += 0.05f;
                },
                NextLevel = new Choice
                {
                    Title = $"{NameCrit}, chance (2)",
                    Description = "<color=#00ff00>5%</color> more critical hits",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.CritChanceMul += 0.05f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{NameCrit}, chance (3)",
                        Description = "<color=#00ff00>10%</color> more critical hits",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.CritChanceMul += 0.1f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = $"{NameCrit}, damage (1)",
                Description = "Critical hits do <color=#00ff00>10%</color> more damage",
                Apply = () =>
                {
                    PlayerUpgrades.Data.CritValueMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = $"{NameCrit}, damage (2)",
                        Description = "Critical hits do <color=#00ff00>10%</color> more damage",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.CritValueMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{NameCrit}, damage (3)",
                        Description = "Critical hits do <color=#00ff00>15%</color> more damage",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.CritValueMul += 0.15f;
                        },
                    }
                }
            },

        };
    }
}
