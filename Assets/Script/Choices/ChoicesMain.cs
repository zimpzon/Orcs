using System;
using System.Collections.Generic;

public class Choice
{
    public string Title;
    public string Description;
    public Action Apply;
    public Choice NextLevel;
}

public static class UpgradeChoices
{
    public static Choice GetRandomChoice()
    {
        if (CurrentChoices.Count == 0)
        {
            return new Choice
            {
                Title = "Minor Heal",
                Description = "Heal for <color=#00ff00>50</color> HP",
                Apply = () =>
                {
                    G.D.PlayerScript.AddHp(50);
                },
            };
        }

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
        void DelayedAdd(List<Choice> delayedChoices)
        {
            CurrentChoices.AddRange(delayedChoices);
        }

        CurrentChoices = new List<Choice>();
        CurrentChoices.AddRange(ChoicesGlobal.GetGlobalChoices());
        CurrentChoices.AddRange(ChoicesWeapon.GetWeaponChoices());
        CurrentChoices.AddRange(ChoicesCirclingAxe.GetCircelingAxeChoices());
        CurrentChoices.AddRange(ChoicesPlayer.GetPlayerChoices());
        CurrentChoices.AddRange(ChoicesBabyOrc.GetBabyOrcChoices());
        CurrentChoices.AddRange(ChoicesSawblade.GetSawbladeChoices(DelayedAdd));
        CurrentChoices.AddRange(ChoicesPoison.GetPoisonChoices(DelayedAdd));
        CurrentChoices.AddRange(ChoicesBurstOfFrost.GetBurstOfFrostChoices(DelayedAdd));
        CurrentChoices.AddRange(ChoicesMeleeThrow.GetMeleeThrowChoices(DelayedAdd));
    }
}
