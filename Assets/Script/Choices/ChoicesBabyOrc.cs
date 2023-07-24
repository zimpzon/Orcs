using System.Collections.Generic;

public static class ChoicesBabyOrc
{
    public static List<Choice> GetBabyOrcChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "Baby orc pickup: Sawblade (1)",
                Description = "Large sawblade.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.OrcPickupSawbladeEnabled = true;
                },
                NextLevel = new Choice
                {
                    Title = "Baby orc pickup: Sawblade (2)",
                    Description = "All sawblades: pick nearby target when target dies.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.OrcPickupSawbladePickNewTarget = true;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Baby orc pickup: Sawblade (3)",
                        Description = "All sawblades: <color=#00ff00>+25%</color> durabilty, <color=#00ff00>+1%</color> small sawblade.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.OrcPickupSawbladeDurabilityMul += 0.25f;
                            PlayerUpgrades.Data.OrcPickupSmallSawbladeEnabled = true;
                        },
                    }
                }
            },

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
