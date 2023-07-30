using System.Collections.Generic;

public static class ChoicesBabyOrc
{
    public static List<Choice> GetBabyOrcChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "Pirate duck, Knightsaber (1)",
                Description = "Pushes away nearby enemies",
                Apply = () =>
                {
                    PlayerUpgrades.Data.OrcJedisEnabled = true;
                    GameManager.Instance.Orc.SetYoda();
                },
                NextLevel = new Choice
                {
                    Title = "Pirate duck, Knightsaber (2)",
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
