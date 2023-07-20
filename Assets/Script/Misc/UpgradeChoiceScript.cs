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
            Title = "High caliber",
            Description = "Main weapon damage <color=#00ff00>+20%</color>, main weapon cooldown <color=#ff0000>+5%</color>",
            Apply = () =>
            {
                PlayerUpgrades.Data.PrimaryCdMul += 0.05f;
                PlayerUpgrades.Data.PrimaryDamageMul += 0.2f;
            },
            NextLevel = new Choice
            {
                Title = "High caliber",
                Description = "Main weapon damage <color=#00ff00>+30%</color>, main weapon cooldown <color=#ff0000>+5%</color>",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PrimaryDamageMul += 0.3f;
                    PlayerUpgrades.Data.PrimaryCdMul += 0.05f;
                },
                NextLevel = new Choice
                {
                    Title = "High caliber",
                    Description = "",
                }
            }
        },

        new Choice
        {
            Title = "Quickshot",
            Description = "Main weapon cooldown <color=#00ff00>-10%</color>",
            Apply = () =>
            {
                PlayerUpgrades.Data.PrimaryCdMul -= 0.1f;
            },
            NextLevel = new Choice
            {
                Title = "Quickshot",
                Description = "Main weapon cooldown <color=#00ff00>-15%</color>, main weapon damage <color=#ff0000>-5%</color>",
                Apply = () =>
                {
                    PlayerUpgrades.Data.PrimaryCdMul -= 0.15f;
                    PlayerUpgrades.Data.PrimaryDamageMul -= 0.05f;
                },
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
