using System.Collections.Generic;

public static class ChoicesPlayer
{
    public static List<Choice> GetPlayerChoices()
    {
        const string Name = "Super Knight";

        return new List<Choice>
        {
            new Choice
            {
                Title = $"{Name}, max life (1)",
                Description = "<color=#00ff00>+20%</color> base max life",
                Apply = () =>
                {
                    PlayerUpgrades.Data.HealthMul += 0.2f;
                    GameManager.Instance.PlayerScript.UpdateMaxHp();
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, max life (2)",
                    Description = "<color=#00ff00>+20%</color> base max life",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.HealthMul += 0.2f;
                        GameManager.Instance.PlayerScript.UpdateMaxHp();
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, max life (3)",
                        Description = "<color=#00ff00>+30%</color> base max life",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.HealthMul += 0.3f;
                            GameManager.Instance.PlayerScript.UpdateMaxHp();
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, max life (4)",
                            Description = "<color=#00ff00>+30%</color> base max life",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.HealthMul += 0.3f;
                                GameManager.Instance.PlayerScript.UpdateMaxHp();
                            },
                        }
                    },
                }
            },

            new Choice
            {
                Title = $"{Name}, recovery (1)",
                Description = "Heal <color=#00ff00>+0.5</color> life/sec",
                Apply = () =>
                {
                    PlayerUpgrades.Data.HealthRegenSecAdd += 0.5f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, recovery (2)",
                    Description = "Heal <color=#00ff00>+0.5</color> life/sec",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.HealthRegenSecAdd += 0.5f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, recovery (3)",
                        Description = "Heal <color=#00ff00>+1</color> life/sec",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.HealthRegenSecAdd += 1.0f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, recovery (4)",
                            Description = "Heal <color=#00ff00>+1</color> life/sec",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.HealthRegenSecAdd += 1.0f;
                            },
                        }
                    }
                }
            },

            new Choice
            {
                Title = $"{Name}, defense (1)",
                Description = "Take <color=#00ff00>10%</color> less damage",
                Apply = () =>
                {
                    PlayerUpgrades.Data.HealthDefenseMul -= 0.1f;
                },
                NextLevel = new Choice
                {
                Title = $"{Name}, defense (2)",
                    Description = "Take <color=#00ff00>10%</color> less damage",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.HealthDefenseMul -= 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, defense (3)",
                        Description = "Take <color=#00ff00>10%</color> less damage",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.HealthDefenseMul -= 0.1f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Turbo Knight (1)",
                Description = "Run <color=#00ff00>10%</color> faster",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MoveSpeedMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Turbo Knight (2)",
                    Description = "Run <color=#00ff00>10%</color> faster",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MoveSpeedMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Turbo Knight (3)",
                        Description = "Run <color=#00ff00>10%</color> faster",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MoveSpeedMul += 0.1f;
                        },
                    }
                }
            },

        };
    }
}
