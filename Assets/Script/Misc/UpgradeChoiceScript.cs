using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeChoiceScript : MonoBehaviour
{
    class Choice
    {
        public string Title;
        public string Description;
        public Action Apply;
        public Choice NextLevel;
    }

    List<Choice> Level1Choices = new List<Choice>
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
            Description = "Main weapon <color=#ff0000>+1</color> bullet per round, <color=#ff0000>-10%</color> time between bullets.",
            Apply = () =>
            {
                PlayerUpgrades.Data.PrimaryBulletsAdd += 1;
                PlayerUpgrades.Data.PrimaryCdBetweenBulletsMul -= 0.1f;
            },
            NextLevel = new Choice
            {
                Title = "Trigger-happy (2)",
                Description = "Main weapon <color=#ff0000>+2</color> bullets per round.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PrimaryBulletsAdd += 2;
                },
            }
        },

        new Choice
        {
            Title = "Padawan (1)",
            Description = "Every <color=#ff0000>3rd</color> color=#ff0000>+1</color> baby orc is a Jedi apprentice, wielding their Knightsaber to damage, push, and briefly slow enemies.",
            Apply = () =>
            {
                PlayerUpgrades.Data.OrcJedisEnabled = true;
            },
            NextLevel = new Choice
            {
                Title = "Padawan (2)",
                Description = "Knightsaber knockback force increased by <color=#ff0000>+100%</color>.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.OrcJediKnockBackForceMul += 1;
                },
                NextLevel = new Choice
                {
                    Title = "Padawan (3)",
                    Description = "Knightsaber knockback damage increased by <color=#ff0000>+400% damage.</color>.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.OrcJediDamageMul += 4;
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
                Description = "When saving a baby orc, force push radius increased by 50%.",
                Apply = () =>
                {
                    PlayerUpgrades.Data.OrcPickupForcePushRadiusMul += 0.5f;
                },
                NextLevel = new Choice
                {
                    Title = "Force push (3)",
                    Description = "When saving a baby orc, force push damage increased by 100%.",
                    Apply = () =>
                    {
                        PlayerUpgrades.Data.OrcPickupForcePushDamageMul += 1;
                    },
                }
            }
        },

    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
