using System.Collections.Generic;

public static class ChoicesCirclingAxe
{
    public static List<Choice> GetCircelingAxeChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "Axe (1)",
                Description = "Damages and briefly slows enemies",
                Apply = () =>
                {
                    PlayerUpgrades.Data.CirclingAxeEnabled = true;
                },
                NextLevel = new Choice
                {
                    Title = "Axe (2)",
                    Description = "<color=#00ff00>+75%</color> damage",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.CirclingAxeDamageMul += 0.75f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Axe (3)",
                        Description = "<color=#00ff00>+50%</color> damage",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.CirclingAxeDamageMul += 0.5f;
                        },
                        NextLevel = new Choice
                        {
                            Title = "Axe (4)",
                            Description = "<color=#00ff00>+25%</color> range",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.CirclingAxeLifetimeMul += 0.25f;
                            },
                            NextLevel = new Choice
                            {
                                Title = "Axe (5)",
                                Description = "Moves <color=#00ff00>+30%</color> faster",
                                Apply = () =>
                                {
                                    PlayerUpgrades.Data.CirclingAxeSpeedMul += 0.3f;
                                },
                                NextLevel = new Choice
                                {
                                    Title = "Axe (6)",
                                    Description = "Moves <color=#00ff00>+20%</color> faster",
                                    Apply = () =>
                                    {
                                        PlayerUpgrades.Data.CirclingAxeSpeedMul+= 0.2f;
                                    },
                                }
                            }
                        }
                    }
                }
            },
        };
    }
}
