using Assets.Script.Misc;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BeastiaryBeastScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [NonSerialized] public int Index = -1;
    [NonSerialized] public ActorTypeEnum ActorType;
    [NonSerialized] public TextMeshProUGUI Hovertext;

    const string DefaultHovertext = "Hover over a beast to see details";

    // Mouse: hover shows the details. Touch: a tap pins them (TouchPin: same beast again or anything else closes).
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TouchPin.IsTouch(eventData))
            TouchPin.Tap(this, ShowDetails, HideDetails);
        else
            ShowDetails();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!TouchPin.IsTouch(eventData))
            HideDetails();
    }

    void HideDetails() => HoverTextFx.Of(Hovertext).Unhover(this, DefaultHovertext);

    // Full number with the player's thousands separator (1.345 / 1,345), never abbreviated.
    static string Grouped(long n) => n.ToString("#,0", DisplayNumberFormat.Info);

    void ShowDetails()
    {
        // Always shows the beast's income %, locked or not (it applies once Beast Scholar is bought).
        int bonus = BestiaryScript.BeastBonusPct(Index);
        bool isUnlocked = SaveGame.Members.BeastsSeen.Contains(ActorType);
        string name = "Locked";
        if (isUnlocked)
            ActorBase.Names.TryGetValue(ActorType, out name);

        // Three calm lines: the name, the income bonus, then one small grey line - the slaying progress (unlocked
        // beasts) and the arena the beast starts to show up in.
        long unlockLevel = EnemySpawner.UnlockLevelOf(ActorType);
        string arena = unlockLevel > 0 ? $"arena {Grouped(unlockLevel)}" : "";
        string facts = arena;
        if (isUnlocked && BeastGoals.Active)
        {
            // Slaying goal after Scary Earl (BeastGoals): "12 / 100 slain" until reached, then "123 / 100 - DECIMATED"
            // (gold). Flaps: "enough".
            long kills = SaveGame.Members.GetBeastKills(ActorType);
            const long goal = BeastGoals.Goal;
            string decimated = $"<color={TextColors.Gold}>DECIMATED</color>";
            string slain = BeastGoals.AlwaysDone(ActorType) ? $"enough  •  {decimated}"
                : kills < goal ? $"{Grouped(kills)} / {Grouped(goal)} slain"
                : $"{Grouped(kills)} / {Grouped(goal)}  •  {decimated}";
            facts = arena.Length > 0 ? $"{slain}  •  {arena}" : slain;
        }

        string title = isUnlocked ? $"<b>{name}</b>" : $"<color=#999999>{name}</color>";
        string text = $"{title}\n<color={TextColors.GreenSmallest}>+{bonus}% income</color>";
        if (facts.Length > 0)
            text += $"\n<size=85%><color=#A0A0A0>{facts}</color></size>";
        HoverTextFx.Of(Hovertext).Hover(this, text);
    }
}
