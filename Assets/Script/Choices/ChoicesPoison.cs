using System;
using System.Collections.Generic;

public static class ChoicesPoison
{
    static Action<List<Choice>> delayedAdd_;
    public const string Name = "Chill Tornado";

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
                    Description = $"Damages and slows enemies",
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
                Title = $"{Name}, count (1)",
                Description = "<color=#00ff00>+50%</color> chillflakes, <color=#00ff00>+30%</color> damage",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PaintballRangeMul += 0.5f;
                    PlayerUpgrades.Data.PaintballDamagePerSecMul += 0.3f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, count (2)",
                    Description = "<color=#00ff00>+30%</color> chillflakes, <color=#00ff00>+20%</color> damage",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.PaintballRangeMul += 0.3f;
                        PlayerUpgrades.Data.PaintballDamagePerSecMul += 0.2f;
                    },
                }
            },
        };
    }
}
