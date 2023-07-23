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
                Description = "Adds <color=#00ff00>10%</color> damage to weapons and increases firing cooldown by <color=#ff0000>5%</color>.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.WeaponsCdMul += 0.05f;
                    PlayerUpgrades.Data.WeaponsDamageMul += 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Weapons: Damage (2)",
                    Description = "Adds <color=#00ff00>10%</color> damage to weapons and increases firing cooldown by <color=#ff0000>2%</color>.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.WeaponsCdMul += 0.02f;
                        PlayerUpgrades.Data.WeaponsDamageMul += 0.1f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Weapons: Damage (3)",
                        Description = "Adds <color=#00ff00>15%</color> damage to weapons and decreases number of machinegun bullets by <color=#ff0000>1</color>.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MachinegunBulletsAdd -= 1;
                            PlayerUpgrades.Data.WeaponsDamageMul += 0.15f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Quickshot (1)",
                Description = "Main weapon cooldown <color=#00ff00>-10%</color>",
                Apply = () =>
                {
                    PlayerUpgrades.Data.WeaponsCdMul -= 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Quickshot (2)",
                    Description = "Main weapon cooldown <color=#00ff00>-15%</color>, main weapon damage <color=#ff0000>-5%</color>",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.WeaponsCdMul -= 0.15f;
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
                        Description = "Add <color=#00ff00>+2</color> bullets per round.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.MachinegunBulletsAdd += 2;
                        },
                    }
                }
            },
        };
    }
}
