using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BeastiaryBeastScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [NonSerialized] public int Index = -1;
    [NonSerialized] public ActorTypeEnum ActorType;
    [NonSerialized] public TextMeshProUGUI Hovertext;

    const string DefaultHovertext = "Hover over a beast to see details";

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Always shows the beast's income %, locked or not (it applies once Beast Scholar is bought).
        int bonus = BestiaryScript.BeastBonusPct(Index);
        bool isUnlocked = SaveGame.Members.BeastsSeen.Contains(ActorType);
        string name = "Locked";
        if (isUnlocked)
            ActorBase.Names.TryGetValue(ActorType, out name);

        // Arena level where the beast starts to show up.
        long unlockLevel = EnemySpawner.UnlockLevelOf(ActorType);
        string arena = unlockLevel > 0 ? $" <color=#AAAAAA>(arena {unlockLevel:#,0})</color>" : "";
        HoverTextFx.Of(Hovertext).Hover(this, $"{name}{arena}\n+{bonus}% income");
    }

    // Touchscreens: the tapped beast's text stays (a touch exits right away) until another beast is tapped.
    public void OnPointerClick(PointerEventData eventData) => OnPointerEnter(eventData);

    public void OnPointerExit(PointerEventData eventData)
    {
        if (HoverTextFx.IsTouch(eventData))
            return;
        HoverTextFx.Of(Hovertext).Unhover(this, DefaultHovertext);
    }
}
