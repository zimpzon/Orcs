using System;
using System.Collections.Generic;

public static class ChoicesMeleeThrow
{
    static Action<List<Choice>> delayedAdd_;
    const string Name = "Swirling Swords";

    public static List<Choice> GetMeleeThrowChoices(Action<List<Choice>> delayedAdd)
    {
        delayedAdd_ = delayedAdd;

        if (!PlayerUpgrades.Data.MeleeThrowBought)
            return new List<Choice>();

        if (!PlayerUpgrades.Data.MeleeThrowEnabledInRound)
        {
            return new List<Choice>
            {
                new Choice
                {
                    Title = $"Equip {Name}",
                    Description = "Rotating swords that return to their origin",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MeleeThrowEnabledInRound = true;
                        PlayerUpgrades.Data.MeleeThrowLeft = true;
                        PlayerUpgrades.Data.MeleeThrowRight = true;
                        delayedAdd_(GetUnlockedChoices());
                    },
                }
            };
        }
        return new List<Choice>();
    }

    static List<Choice> GetUnlockedChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = $"{Name}, Vertical",
                Description = "Add <color=#00ff00>2</color> vertical swords doing <color=#00ff00>50%</color> damage",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MeleeThrowUp = true;
                    PlayerUpgrades.Data.MeleeThrowDown = true;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, Diagonal",
                    Description = "Add <color=#00ff00>4</color> diagonal swords doing <color=#00ff00>25%</color> damage",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MeleeThrowUpLeft = true;
                        PlayerUpgrades.Data.MeleeThrowUpRight = true;
                        PlayerUpgrades.Data.MeleeThrowDownLeft = true;
                        PlayerUpgrades.Data.MeleeThrowDownRight = true;
                    },
                }
            },

            new Choice
            {
                Title = $"{Name}, Speed (1)",
                Description = "<color=#00ff00>-10%</color> time between swords",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MeleeThrowCdMul -= 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, Speed (2)",
                    Description = "<color=#00ff00>-10%</color> time between swords",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MeleeThrowCdMul -= 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, Speed (3)",
                        Description = "<color=#00ff00>-15%</color> time between swords",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MeleeThrowCdMul -= 0.15f;
                        },
                    }
                }
            },
        };
    }
}
