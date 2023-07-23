using System.Collections.Generic;

public static class ChoicesWeapon
{
    public static List<Choice> GetWeaponChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "Weapons: Damage (1)",
                Description = "<color=#00ff00>10%</color> weapon damage.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.WeaponsDamageMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Weapons: Damage (2)",
                    Description = "<color=#00ff00>10%</color> weapon damage.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.WeaponsDamageMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Weapons: Damage (3)",
                        Description = "<color=#00ff00>15%</color> weapon damage.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.WeaponsDamageMul += 0.15f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Quickshot (1)",
                Description = "Weapon attack speed <color=#00ff00>+10%</color>",
                Apply = () =>
                {
                    PlayerUpgrades.Data.WeaponsCdMul -= 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Quickshot (2)",
                    Description = "Weapon attack speed <color=#00ff00>+10%</color>",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.WeaponsCdMul -= 0.1f;
                        PlayerUpgrades.Data.WeaponsDamageMul -= 0.05f;
                    },
                }
            },

            new Choice
            {
                Title = "Machinegun: Trigger-happy (1)",
                Description = "Add <color=#00ff00>+1</color> bullet per round.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.MachinegunBulletsAdd += 1;
                },
                NextLevel = new Choice
                {
                    Title = "Machinegun: Trigger-happy (2)",
                    Description = "Add <color=#00ff00>+1</color> bullet per round.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.MachinegunBulletsAdd += 1;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Machinegun: Trigger-happy (3)",
                        Description = "Add <color=#00ff00>+1</color> bullets per round.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MachinegunBulletsAdd += 1;
                        },
                    }
                }
            },
        };
    }
}
