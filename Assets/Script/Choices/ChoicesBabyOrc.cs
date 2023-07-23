using System.Collections.Generic;

public static class ChoicesBabyOrc
{
    public static List<Choice> GetBabyOrcChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "Baby orc: Sawblade (1)",
                Description = "Spawn a large sawblade when saving a baby orc.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.OrcPickupSawbladeEnabled = true;
                },
                NextLevel = new Choice
                {
                    Title = "Baby orc: Sawblade (2)",
                    Description = "All sawblades automatically move to nearby targets when their target dies.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.SawbladePickNewTarget = true;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Baby orc: Sawblade (3)",
                        Description = "Add <color=#00ff00>+25%</color> durabilty to all sawblades and spawn a second smaller sawblade when you save a baby orc.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.SawbladeDurabilityMul += 0.25f;
                            PlayerUpgrades.Data.OrcPickupSmallSawbladeEnabled = true;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Jedi apprentice (1)",
                Description = "Baby orcs are Jedi apprentices, wielding a Knightsaber to push and briefly slow enemies.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.OrcJedisEnabled = true;
                    GameManager.Instance.Orc.SetYoda();
                },
                NextLevel = new Choice
                {
                    Title = "Jedi apprentice (2)",
                    Description = "Knightsaber knockback force increased by <color=#00ff00>+100%</color>.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.OrcJediKnockBackForceMul += 1.0f;
                    },
                }
            },

        };
    }
}
