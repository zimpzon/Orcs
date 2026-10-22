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

        // Three calm lines: the name, the income bonus, then the small grey facts - the arena it starts to show up
        // in, and (once unlocked) how many have been slain.
        long unlockLevel = EnemySpawner.UnlockLevelOf(ActorType);
        string facts = unlockLevel > 0 ? $"arena {Grouped(unlockLevel)}" : "";
        if (isUnlocked)
        {
            string slain = $"{Grouped(SaveGame.Members.GetBeastKills(ActorType))} slain";
            facts = facts.Length > 0 ? $"{facts}  •  {slain}" : slain;
        }

        string title = isUnlocked ? $"<b>{name}</b>" : $"<color=#999999>{name}</color>";
        string text = $"{title}\n<color={TextColors.GreenSmallest}>+{bonus}% income</color>";
        if (facts.Length > 0)
            text += $"\n<size=85%><color=#A0A0A0>{facts}</color></size>";
        HoverTextFx.Of(Hovertext).Hover(this, text);
    }
}
