using System.Collections.Generic;

public static class ChoicesWeapon
{
    public static List<Choice> GetWeaponChoices()
    {
        return new List<Choice>
        {
            new Choice
            {
                Title = "High caliber (1)",
                Description = "Main weapon damage <color=#00ff00>+20%</color>, main weapon cooldown <color=#ff0000>+5%</color>",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PrimaryCdMul += 0.05f;
                    PlayerUpgrades.Data.PrimaryDamageMul += 0.2f;
                },
                NextLevel = new Choice
                {
                    Title = "High caliber (2)",
                    Description = "Main weapon damage <color=#00ff00>+30%</color>, main weapon cooldown <color=#ff0000>+5%</color>",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.PrimaryDamageMul += 0.3f;
                        PlayerUpgrades.Data.PrimaryCdMul += 0.05f;
                    },
                }
            },

            new Choice
            {
                Title = "Quickshot (1)",
                Description = "Main weapon cooldown <color=#00ff00>-10%</color>",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PrimaryCdMul -= 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Quickshot (2)",
                    Description = "Main weapon cooldown <color=#00ff00>-15%</color>, main weapon damage <color=#ff0000>-5%</color>",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.PrimaryCdMul -= 0.15f;
                        PlayerUpgrades.Data.PrimaryDamageMul -= 0.05f;
                    },
                }
            },

            new Choice
            {
                Title = "Trigger-happy (1)",
                Description = "Main weapon <color=#00ff00>+1</color> bullet per round, <color=#00ff00>-10%</color> time between bullets.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PrimaryBulletsAdd += 1;
                    PlayerUpgrades.Data.PrimaryCdBetweenBulletsMul -= 0.1f;
                },
                NextLevel = new Choice
                {
                    Title = "Trigger-happy (2)",
                    Description = "Main weapon <color=#00ff00>+2</color> bullets per round.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.PrimaryBulletsAdd += 2;
                    },
                }
            },
        };
    }
}
