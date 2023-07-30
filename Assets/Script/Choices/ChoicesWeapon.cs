using System.Collections.Generic;

public static class ChoicesWeapon
{
    public static List<Choice> GetWeaponChoices()
    {
        const string Name = "Knife of Nobility";

        return new List<Choice>
        {
            new Choice
            {
                Title = $"{Name}, damage (1)",
                Description = "Does <color=#00ff00>10%</color> more damage",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MagicMissileDamageMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, damage (2)",
                    Description = "Does <color=#00ff00>10%</color> more damage",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MagicMissileDamageMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, damage (3)",
                        Description = "Does <color=#00ff00>15%</color> more damage",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MagicMissileDamageMul += 0.15f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, damage (4)",
                            Description = "Does <color=#00ff00>20%</color> more damage",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.MagicMissileDamageMul += 0.2f;
                            },
                        },
                    }
                }
            },

            new Choice
            {
                Title = $"{Name}, range (1)",
                Description = "Throw <color=#00ff00>10%</color> farther",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MagicMissileRangeMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, range (2)",
                    Description = "Throw <color=#00ff00>10%</color> farther",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MagicMissileRangeMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, range (3)",
                        Description = "Throw <color=#00ff00>15%</color> farther",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MagicMissileRangeMul += 0.15f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, range (4)",
                            Description = "Throw <color=#00ff00>15%</color> farther",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.MagicMissileRangeMul += 0.15f;
                            },
                        },
                    }
                }
            },

            new Choice
            {
                Title = $"{Name}, speed (1)",
                Description = "<color=#00ff00>-10%</color> time between knifes",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MagicMissileCdMul -= 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, speed (2)",
                    Description = "<color=#00ff00>-10%</color> time between knifes",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MagicMissileCdMul -= 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, speed (3)",
                        Description = "<color=#00ff00>-10%</color> time between knifes",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MagicMissileCdMul -= 0.1f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, speed (4)",
                            Description = "<color=#00ff00>-15%</color> time between knifes",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.MagicMissileCdMul -= 0.15f;
                            },
                        }
                    }
                }
            },

            new Choice
            {
                Title = $"{Name}, salvo (1)",
                Description = "<color=#00ff00>+30%</color> chance for extra knifes",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MagicMissileBulletsMul += 0.3f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, salvo (2)",
                    Description = "<color=#00ff00>+30%</color> chance for extra knifes",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MagicMissileBulletsMul += 0.3f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, salvo (3)",
                        Description = "<color=#00ff00>+30%</color> chance for extra knifes",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MagicMissileBulletsMul += 0.3f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, salvo (4)",
                            Description = "<color=#00ff00>+40%</color> chance for extra knifes",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.MagicMissileBulletsMul += 0.4f;
                            },
                        }
                    }
                }
            },
        };
    }
}
