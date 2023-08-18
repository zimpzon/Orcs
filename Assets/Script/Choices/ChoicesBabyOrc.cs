using System.Collections.Generic;

public static class ChoicesBabyOrc
{
    public const string Name = "StarDucks";

    public static List<Choice> GetBabyOrcChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = $"{Name}, first aid (1)",
                Description = $"<color=#00ff00>+10</color> more life when rescuing a duck",
                Apply = () =>
                {
                    PlayerUpgrades.Data.RescueDuckHp += 10;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, first aid (2)",
                    Description = $"<color=#00ff00>+10</color> more life when rescuing a duck",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.RescueDuckHp += 10;
                    },
                }
            },

            new Choice
            {
                Title = $"{Name}, Knightsaber (1)",
                Description = "Pushes enemies near the duck",
                Apply = () =>
                {
                    PlayerUpgrades.Data.OrcJedisEnabled = true;
                    GameManager.Instance.Orc.SetYoda();
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, Knightsaber (2)",
                    Description = "<color=#00ff00>+100%</color> push power",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.OrcJediKnockBackForceMul += 1.0f;
                    },
                }
            },
        };
    }
}
