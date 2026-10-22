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

        // Arena level where the beast starts to show up.
        long unlockLevel = EnemySpawner.UnlockLevelOf(ActorType);
        string arena = unlockLevel > 0 ? $" <color=#AAAAAA>(arena {Grouped(unlockLevel)})</color>" : "";
        // Lifetime kills of this beast (unlocked ones only).
        string slain = isUnlocked
            ? $"\n<color=#AAAAAA>Slain: {Grouped(SaveGame.Members.GetBeastKills(ActorType))}</color>"
            : "";
        HoverTextFx.Of(Hovertext).Hover(this, $"{name}{arena}\n+{bonus}% income{slain}");
    }
}
