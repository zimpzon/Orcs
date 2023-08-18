using System;
using System.Collections.Generic;
using UnityEngine;

public static class ChoicesMeleeThrow
{
    static Action<List<Choice>> delayedAdd_;
    public const string Name = "Darkness";

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
                    Description = $"Sucks in and damages enemies",
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
                Title = $"{Name}, Speed (1)",
                Description = "<color=#00ff00>+10%</color> shorter cooldown",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MeleeThrowCdMul -= 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, Speed (2)",
                    Description = "<color=#00ff00>+10%</color> shorter cooldown",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MeleeThrowCdMul -= 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, Speed (3)",
                        Description = "<color=#00ff00>+10%</color> shorter cooldown",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MeleeThrowCdMul -= 0.1f;
                        },
                    }
                }
            },
        };
    }
}
