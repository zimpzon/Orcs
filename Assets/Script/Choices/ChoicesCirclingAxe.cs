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
                Description = "Throw long range axes",
                Apply = () =>
                {
                    PlayerUpgrades.Data.CirclingAxeEnabled = true;
                },
                NextLevel = new Choice
                {
                    Title = "Axe (2)",
                    Description = "<color=#00ff00>30%</color> damage",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.CirclingAxeDamageMul += 0.3f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Axe (3)",
                        Description = "<color=#00ff00>30%</color> damage",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.CirclingAxeDamageMul += 0.3f;
                        },
                        NextLevel = new Choice
                        {
                            Title = "Axe (4)",
                            Description = "<color=#00ff00>30%</color> range",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.CirclingAxeLifetimeMul += 0.3f;
                            },
                            NextLevel = new Choice
                            {
                                Title = "Axe (5)",
                                Description = "Moves <color=#00ff00>20%</color> faster",
                                Apply = () =>
                                {
                                    PlayerUpgrades.Data.CirclingAxeSpeedMul += 0.2f;
                                },
                                NextLevel = new Choice
                                {
                                    Title = "Axe (6)",
                                    Description = "Moves <color=#00ff00>20%</color> faster",
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
