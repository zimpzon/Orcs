using System.Collections.Generic;

public static class ChoicesGlobal
{
    public static List<Choice> GetGlobalChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "Damage (1)",
                Description = "<color=#00ff00>10%</color> damage.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.DamageMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Damage (2)",
                    Description = "<color=#00ff00>10%</color> damage.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.DamageMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Damage (3)",
                        Description = "<color=#00ff00>15%</color> damage.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.DamageMul += 0.15f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Crit chance (1)",
                Description = "<color=#00ff00>+10%</color> chance.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.CritChanceMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Crit chance (2)",
                    Description = "<color=#00ff00>+10%</color> chance.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.CritChanceMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Crit chance (3)",
                        Description = "<color=#00ff00>+10%</color> chance.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.CritChanceMul += 0.1f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Crit damage (1)",
                Description = "<color=#00ff00>+10%</color> crit damage.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.CritValueMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Crit chance (2)",
                    Description = "<color=#00ff00>+10%</color> crit damage.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.CritChanceMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Crit chance (3)",
                        Description = "<color=#00ff00>+15%</color> crit damage.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.CritChanceMul += 0.15f;
                        },
                    }
                }
            },

        };
    }
}
