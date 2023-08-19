using System;
using System.Collections.Generic;

public static class ChoicesSawblade
{
    static Action<List<Choice>> delayedAdd_;
    public const string Name = "Cutter Comet";

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
                    Description = $"Sticks to targets and slices",
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
                Title = $"{Name}, Mayhem (1)",
                Description = "<color=#00ff00>+1</color> sawblade, <color=#00ff00>+30%</color> more durable",
                Apply = () =>
                {
                    float reciprocal = 1.0f / PlayerUpgrades.Data.SawBladeCdMul;
                    PlayerUpgrades.Data.SawBladeCdMul = 1.0f / (reciprocal + 1);
                    PlayerUpgrades.Data.SawBladeDurabilityMul += 0.3f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, Mayhem (2)",
                    Description = "<color=#00ff00>+1</color> sawblade, <color=#00ff00>+30%</color> more durable",
                    Apply = () =>
                    {
                        float reciprocal = 1.0f / PlayerUpgrades.Data.SawBladeCdMul;
                        PlayerUpgrades.Data.SawBladeCdMul = 1.0f / (reciprocal + 1);
                        PlayerUpgrades.Data.SawBladeDurabilityMul += 0.3f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, Mayhem (3)",
                        Description = "<color=#00ff00>+1</color> sawblade, <color=#00ff00>+30%</color> more durable",
                        Apply = () =>
                        {
                            float reciprocal = 1.0f / PlayerUpgrades.Data.SawBladeCdMul;
                            PlayerUpgrades.Data.SawBladeCdMul = 1.0f / (reciprocal + 1);
                            PlayerUpgrades.Data.SawBladeDurabilityMul += 0.3f;
                        },
                    }
                }
            },

        };
    }
}
