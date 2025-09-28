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
        bool isUnlocked = SaveGame.Members.BeastsSeen.Contains(ActorType);
        if (isUnlocked)
        {
            // Calc is both here and in BeastiaryScript
            int bonus = (Index / 2) + 1;
            ActorBase.Names.TryGetValue(ActorType, out string name);
            Hovertext.text = $"{name}\n +{bonus}% passive income";
        }
        else
        {
            Hovertext.text = $"Locked";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetDefaultHovertext();
    }
}
