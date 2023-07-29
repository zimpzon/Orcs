using System.Collections.Generic;

public static class ChoicesBabyOrc
{
    public static List<Choice> GetBabyOrcChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "Baby orc: Jedi apprentice (1)",
                Description = "May the force be with you.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.OrcJedisEnabled = true;
                    GameManager.Instance.Orc.SetYoda();
                },
                NextLevel = new Choice
                {
                    Title = "Baby orc: Jedi apprentice (2)",
                    Description = "<color=#00ff00>+100%</color> knockback.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.OrcJediKnockBackForceMul += 1.0f;
                    },
                }
            },
        };
    }
}
