using System;
using System.Collections.Generic;

public static class UpgradeChoices
{
    public class Choice
    {
        public string Title;
        public string Description;
        public Action Apply;
        public Choice NextLevel;
    }

    public static Choice GetRandomChoice()
    {
        int idx = UnityEngine.Random.Range(0, CurrentChoices.Count - 1);
        var result = CurrentChoices[idx];
        CurrentChoices.RemoveAt(idx);
        return result;
    }

    public static void ReturnChoice(Choice choice)
    {
        if (choice != null)
            CurrentChoices.Add(choice);
    }

    public static List<Choice> CurrentChoices = new List<Choice>();

    public static void InitChoices()
    {
        CurrentChoices = new List<Choice>
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
                    Description = "Knightsaber knockback force increased by <color=#00ff00>+50%</color>.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.OrcJediKnockBackForceMul += 0.5f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Jedi apprentice (3)",
                    Description = "Knightsaber knockback force increased by <color=#00ff00>+75%</color>.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.OrcJediKnockBackForceMul += 0.75f;
                        },
                    }
                }
            },

            new Choice
            {
                Title = "Force push (1)",
                Description = "When you save a baby orc you execute a force push, damaging and pushing away nearby enemies.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.OrcPickupForcePushEnabled = true;
                },
                NextLevel = new Choice
                {
                    Title = "Force push (2)",
                    Description = "When saving a baby orc, force push radius increased by <color=#00ff00>+50%</color>.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.OrcPickupForcePushRadiusMul += 0.5f;
                    },
                    NextLevel = new Choice
                    {
                        Title = "Force push (3)",
                        Description = "When saving a baby orc, force push damage increased by <color=#00ff00>+100%</color>.",
                        Apply = () =>
                        {
                            PlayerUpgrades.Data.OrcPickupForcePushDamageMul += 1.0f;
                        },
                    }
                }
            },
        };
    }
}
