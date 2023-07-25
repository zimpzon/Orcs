using System.Collections.Generic;

public static class ChoicesWeapon
{
    public static List<Choice> GetWeaponChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "Magic missile range (1)",
                Description = "<color=#00ff00>10%</color> weapon range.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MagicMissileRangeMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Magic missile range (2)",
                    Description = "<color=#00ff00>10%</color> weapon range.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MagicMissileRangeMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Magic missile range (3)",
                        Description = "<color=#00ff00>15%</color> weapon range.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MagicMissileRangeMul += 0.15f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Magic missile attack speed (1)",
                Description = "<color=#00ff00>+10%</color> attack speed",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MagicMissileCdMul -= 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Magic missile attack speed (2)",
                    Description = "<color=#00ff00>+10%</color> attack speed",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MagicMissileCdMul -= 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Magic missile attack speed ()",
                        Description = "<color=#00ff00>+10%</color> attack speed",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MagicMissileCdMul -= 0.1f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Magic missile salvo (1)",
                Description = "Add <color=#00ff00>+1</color> missile per round.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MagicMissileBulletsAdd += 1;
                },
                NextLevel = new Choice
                {
                    Title = "Magic missile salvo (2)",
                    Description = "Add <color=#00ff00>+1</color> missile per round.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MagicMissileBulletsAdd += 1;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Magic missile salvo (3)",
                        Description = "Add <color=#00ff00>+1</color> missile per round.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MagicMissileBulletsAdd += 1;
                        },
                    }
                }
            },

        };
    }
}
