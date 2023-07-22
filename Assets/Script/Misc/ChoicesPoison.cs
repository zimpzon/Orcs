using System.Collections.Generic;

public static class ChoicesPoison
{
    public static List<Choice> GetPoisonChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "Poison jar (1)",
                Description = "Periodically open a poison jar and fling poison in all directions, slowing enemy movement for a while.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PaintballEnabled = true;
                },
                NextLevel = new Choice
                {
                    Title = "Poison jar (2)",
                    Description = "Increase number of projectiles by <color=#00ff00>2</color> and reduce cooldown by <color=#00ff00>10%</color>",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.PaintballCount += 2;
                        PlayerUpgrades.Data.PaintballCdMul -= 0.1f;
                        PlayerUpgrades.Data.Counters.PaintballTimer = 99999;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Poison jar (3)",
                        Description = "Increase number of projectiles by <color=#00ff00>5</color> and reduce cooldown by <color=#00ff00>10%</color>",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.PaintballCount += 5;;
                            PlayerUpgrades.Data.PaintballCdMul -= 0.1f;
                        },
                    }
                }
            },
        };
    }
}
