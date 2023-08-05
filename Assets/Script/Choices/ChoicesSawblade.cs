using System;
using System.Collections.Generic;

public static class ChoicesSawblade
{
    static Action<List<Choice>> delayedAdd_;
    public const string Name = "Saw of Sacrilege";

    public static List<Choice> GetSawbladeChoices(Action<List<Choice>> delayedAdd)
    {
        delayedAdd_ = delayedAdd;

        if (!PlayerUpgrades.Data.SawBladeBought)
            return new List<Choice>();

        if (!PlayerUpgrades.Data.SawBladeEnabledInRound)
        {
            return new List<Choice>
            {
                new Choice
                {
                    Title = $"Equip {Name}",
                    Description = "A devastating sawblade",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.SawBladeEnabledInRound = true;
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
                Title = $"{Name}, Durability (1)",
                Description = "Sawblade is <color=#00ff00>10%</color> more durable",
                Apply = () =>
                {
                    PlayerUpgrades.Data.SawBladeDurabilityMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, Speed (1)",
                    Description = "<color=#00ff00>-10%</color> time between sawblades",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.SawBladeCdMul -= 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, Durability (2)",
                        Description = "Sawblade is <color=#00ff00>15%</color> more durable",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.SawBladeDurabilityMul += 0.15f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, Speed (2)",
                            Description = "<color=#00ff00>-10%</color> time between sawblades",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.SawBladeCdMul -= 0.1f;
                            },
                            NextLevel = new Choice
                            {
                                Title = $"{Name}, Durability (3)",
                                Description = "Sawblade is <color=#00ff00>20%</color> more durable",
                                Apply = () =>
                                {
                                    PlayerUpgrades.Data.SawBladeDurabilityMul += 0.2f;
                                },
                            }
                        }
                    }
                }
            },

        };
    }
}
