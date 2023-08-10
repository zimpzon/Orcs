using System.Collections.Generic;

public static class ChoicesBabyOrc
{
    public const string Name = "Pirate duck";

    public static List<Choice> GetBabyOrcChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = $"{Name}, first aid (1)",
                Description = $"<color=#00ff00>+5</color> more life when rescuing a {Name}",
                Apply = () =>
                {
                    PlayerUpgrades.Data.RescueDuckHp += 5;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, first aid (2)",
                    Description = $"<color=#00ff00>+5</color> more life when rescuing a {Name}",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.RescueDuckHp += 5;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, first aid (3)",
                        Description = $"<color=#00ff00>+5</color> more life when rescuing a {Name}",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.RescueDuckHp += 5;
                        },
                    }
                }
            },

            new Choice
            {
                Title = $"{Name}, Knightsaber (1)",
                Description = "Pushes away nearby enemies",
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
