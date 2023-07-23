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
                Description = "Slowing poison.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PaintballEnabled = true;
                },
                NextLevel = new Choice
                {
                    Title = "Poison jar (2)",
                    Description = "<color=#00ff00>+5</color> projectiles, <color=#00ff00>+10%</color> attack speed.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.PaintballCount += 5;
                        PlayerUpgrades.Data.PaintballCdMul -= 0.1f;
                        PlayerUpgrades.Data.Counters.PaintballTimer = 99999;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Poison jar (3)",
                        Description = "<color=#00ff00>+5</color> projectiles, <color=#00ff00>+10%</color> attack speed.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.PaintballCount += 5;;
                            PlayerUpgrades.Data.PaintballCdMul -= 0.1f;
                            PlayerUpgrades.Data.Counters.PaintballTimer = 99999;
                        },
                    }
                }
            },
        };
    }
}
