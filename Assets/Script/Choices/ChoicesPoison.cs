using System;
using System.Collections.Generic;

public static class ChoicesPoison
{
    static Action<List<Choice>> delayedAdd_;
    public const string Name = "Dagger of Pestilence";

    public static List<Choice> GetPoisonChoices(Action<List<Choice>> delayedAdd)
    {
        delayedAdd_ = delayedAdd;

        if (!PlayerUpgrades.Data.PaintballBought)
            return new List<Choice>();

        if (!PlayerUpgrades.Data.PaintballActiveInRound)
        {
            return new List<Choice>
            {
                new Choice
                {
                    Title = $"Equip {Name}",
                    Description = "Poisons enemies",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.PaintballActiveInRound = true;
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
                Title = $"{Name}, range (1)",
                Description = "Throw <color=#00ff00>10%</color> farther",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PaintballRangeMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, range (2)",
                    Description = "Throw <color=#00ff00>10%</color> farther",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.PaintballRangeMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, range (3)",
                        Description = "Throw <color=#00ff00>15%</color> farther",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.PaintballRangeMul += 0.15f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, range (4)",
                            Description = "Throw <color=#00ff00>15%</color> farther",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.PaintballRangeMul += 0.15f;
                            },
                        }
                    }
                }
            },

            new Choice
            {
                Title = $"{Name}, damage (1)",
                Description = "Does <color=#00ff00>10%</color> more damage/sec",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PaintballDamagePerSecMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, damage (2)",
                    Description = "Does <color=#00ff00>10%</color> more damage/sec",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.PaintballDamagePerSecMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, damage (3)",
                        Description = "Does <color=#00ff00>15%</color> more damage/sec",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.PaintballDamagePerSecMul += 0.15f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, damage (4)",
                            Description = "Does <color=#00ff00>20%</color> more damage/sec",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.PaintballDamagePerSecMul += 0.2f;
                            },
                        }
                    }
                }
            },

            new Choice
            {
                Title = $"{Name}, amount (1)",
                Description = "<color=#00ff00>+4</color> knives thrown",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PaintballCount += 4;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, amount (2)",
                    Description = "<color=#00ff00>+4</color> knives thrown",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.PaintballCount += 4;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, amount (3)",
                        Description = "<color=#00ff00>+4</color> knives thrown",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.PaintballCount += 4;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, amount (4)",
                            Description = "<color=#00ff00>+4</color> knives thrown",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.PaintballCount += 4;
                            },
                        }
                    }
                }
            },
        };
    }
}
