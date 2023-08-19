using System;
using System.Collections.Generic;

public static class ChoicesBurstOfFrost
{
    static Action<List<Choice>> delayedAdd_;
    public const string Name = "Coldslap";

    public static List<Choice> GetBurstOfFrostChoices(Action<List<Choice>> delayedAdd)
    {
        delayedAdd_ = delayedAdd;

        if (!PlayerUpgrades.Data.BurstOfFrostBought)
            return new List<Choice>();

        if (!PlayerUpgrades.Data.BurstOfFrostEnabledInRound)
        {
            return new List<Choice>
            {
                new Choice
                {
                    Title = $"Equip {Name}",
                    Description = "Chance to freeze nearby enemies",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.BurstOfFrostEnabledInRound = true;
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
                Title = $"{Name}, radius (1)",
                Description = "<color=#00ff00>10%</color> radius",
                Apply = () =>
                {
                    PlayerUpgrades.Data.BurstOfFrostRangeMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, radius (2)",
                    Description = "<color=#00ff00>10%</color> radius",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.BurstOfFrostRangeMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, radius (3)",
                        Description = "<color=#00ff00>10%</color> radius",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.BurstOfFrostRangeMul += 0.1f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = $"{Name}, freeze (1)",
                Description = "<color=#00ff00>+5%</color> chance to freeze",
                Apply = () =>
                {
                    PlayerUpgrades.Data.BurstOfFrostFreezeChanceMul += 0.05f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, freeze (2)",
                    Description = "<color=#00ff00>+5%</color> chance to freeze",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.BurstOfFrostFreezeChanceMul += 0.05f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, freeze (3)",
                        Description = "<color=#00ff00>+5%</color> chance to freeze",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.BurstOfFrostFreezeChanceMul += 0.05f;
                        },
                    }
                },
            }
        };
    }
}
