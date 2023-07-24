using System.Collections.Generic;

public static class ChoicesPlayer
{
    public static List<Choice> GetPlayerChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "Max health (1)",
                Description = "<color=#00ff00>+10%</color> max health.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.HealthMul += 0.1f;
                    GameManager.Instance.PlayerScript.UpdateMaxHp();
                },
                NextLevel = new Choice
                {
                    Title = "Max health (2)",
                    Description = "<color=#00ff00>+10%</color> max health.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.HealthMul += 0.1f;
                        GameManager.Instance.PlayerScript.UpdateMaxHp();
                    },
                    NextLevel = new Choice
                    {
                        Title = "Max health (3)",
                        Description = "<color=#00ff00>+15%</color> max health.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.HealthMul += 0.15f;
                            GameManager.Instance.PlayerScript.UpdateMaxHp();
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Health regen (1)",
                Description = "<color=#00ff00>+20%</color> health regen.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.HealthRegenSecMul += 0.2f;
                },
                NextLevel = new Choice
                {
                    Title = "Health regen (2)",
                    Description = "<color=#00ff00>+20%</color> health regen.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.HealthMul += 0.2f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Health regen (3)",
                        Description = "<color=#00ff00>+25%</color> health regen.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.HealthMul += 0.25f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Defense (1)",
                Description = "<color=#00ff00>+10%</color> defense.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.HealthDefenseMul -= 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Defense (2)",
                    Description = "<color=#00ff00>+10%</color> defense.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.HealthMul -= 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Defense (3)",
                        Description = "<color=#00ff00>+15%</color> defense.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.HealthMul -= 0.15f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Run speed (1)",
                Description = "<color=#00ff00>+10%</color> run speed.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MoveSpeedMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Run speed (2)",
                    Description = "<color=#00ff00>+10%</color> run speed.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MoveSpeedMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Run speed (3)",
                        Description = "<color=#00ff00>+15%</color> run speed.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MoveSpeedMul += 0.15f;
                        },
                    }
                }
            },

        };
    }
}
