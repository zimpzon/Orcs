using System.Collections.Generic;

public static class ChoicesWeapon
{
    public const string Name = "Your trusty knife";

    public static List<Choice> GetWeaponChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = $"{Name}, range (1)",
                Description = "Range <color=#00ff00>+20%</color>",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MagicMissileRangeMul += 0.2f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, range (2)",
                    Description = "Range <color=#00ff00>+20%</color>",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MagicMissileRangeMul += 0.2f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, range (3)",
                        Description = "Range <color=#00ff00>+20%</color>",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MagicMissileRangeMul += 0.2f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, range (4)",
                            Description = "Range <color=#00ff00>+20%</color>",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.MagicMissileRangeMul += 0.2f;
                            },
                        },
                    }
                }
            },

            new Choice
            {
                Title = $"{Name}, speed (1)",
                Description = $"<color=#00ff00>+8%</color> shorter cooldown, {G.D.UpgradeNegativeColorHex}-8%</color> range",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MagicMissileCdMul -= 0.08f;
                    PlayerUpgrades.Data.MagicMissileRangeMul -= 0.08f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, speed (2)",
                    Description = $"<color=#00ff00>+8%</color> shorter cooldown, {G.D.UpgradeNegativeColorHex}-8%</color> range",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MagicMissileCdMul -= 0.08f;
                        PlayerUpgrades.Data.MagicMissileRangeMul -= 0.08f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, speed (3)",
                        Description = $"<color=#00ff00>+8%</color> shorter cooldown, {G.D.UpgradeNegativeColorHex}-8%</color> range",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MagicMissileCdMul -= 0.08f;
                            PlayerUpgrades.Data.MagicMissileRangeMul -= 0.08f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, speed (4)",
                            Description = $"<color=#00ff00>+8%</color> shorter cooldown, {G.D.UpgradeNegativeColorHex}-8%</color> range",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.MagicMissileCdMul -= 0.08f;
                                PlayerUpgrades.Data.MagicMissileRangeMul -= 0.08f;
                            },
                        }
                    }
                }
            },

            new Choice
            {
                Title = $"{Name}, salvo (1)",
                Description = $"<color=#00ff00>+0.4</color> more knives, {G.D.UpgradeNegativeColorHex}-4%</color> longer cooldown",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MagicMissileBulletsAdd += 0.4f;
                    PlayerUpgrades.Data.MagicMissileCdMul += 0.04f;
                },
                NextLevel = new Choice
                {
                    Title = $"{Name}, salvo (2)",
                    Description = $"<color=#00ff00>+0.4</color> more knives, {G.D.UpgradeNegativeColorHex}-4%</color> longer cooldown",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MagicMissileBulletsAdd += 0.4f;
                        PlayerUpgrades.Data.MagicMissileCdMul += 0.04f;
                    },
                    NextLevel = new Choice
                    {
                        Title = $"{Name}, salvo (3)",
                        Description = $"<color=#00ff00>+0.4</color> more knives, {G.D.UpgradeNegativeColorHex}-4%</color> longer cooldown",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MagicMissileBulletsAdd += 0.4f;
                            PlayerUpgrades.Data.MagicMissileCdMul += 0.04f;
                        },
                        NextLevel = new Choice
                        {
                            Title = $"{Name}, salvo (4)",
                            Description = $"<color=#00ff00>+0.4</color> more knives, {G.D.UpgradeNegativeColorHex}-4%</color> longer cooldown",
                            Apply = () =>
                            {
                                PlayerUpgrades.Data.MagicMissileBulletsAdd += 0.4f;
                                PlayerUpgrades.Data.MagicMissileCdMul += 0.04f;
                            },
                        }
                    }
                }
            },
        };
    }
}
