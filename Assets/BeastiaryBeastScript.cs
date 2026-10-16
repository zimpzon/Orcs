using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BeastiaryBeastScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [NonSerialized] public int Index = -1;
    [NonSerialized] public ActorTypeEnum ActorType;
    [NonSerialized] public TextMeshProUGUI Hovertext;

    void SetDefaultHovertext()
    {
        Hovertext.text = "Hover over a beast to see details";
    }

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
        Hovertext.text = $"{name}{arena}\n+{bonus}% income";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetDefaultHovertext();
    }
}
