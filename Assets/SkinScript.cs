using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkinScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SkinAnimation AnimationName;
    public SkinsAnimationList SkinAnimations;
    public SelectedSkinScript SelectedSkinScript;
    public Image SkinImage;
    public Button Button;
    public TextMeshProUGUI Hovertext;

    private SkinAnimations MyAnimations;

    void SetDefaultHovertext()
    {
        Hovertext.text = "Hover over a skin to see unlock requirements";
    }

    public void OnClick()
    {
        SaveGame.Members.CurrentSkin = (int)AnimationName;
        SelectedSkinScript.Instance.SelectedSkinAnimation = MyAnimations;
    }

    private (bool isUnlocked, string hoverText) GetUnlockStatus(SkinAnimation animationName)
    {
        var list = SaveGame.Members.Achieved;

        if (animationName == SkinAnimation.Default)
        {
            return (true, "Good ol' Earl, always there");
        }
        else if (animationName == SkinAnimation.WitchDoctor)
        {
            return (list.Contains(Achieved.WitchDoctor75), "Witch Doctor Earl: Reach Witch Doctor level 75");
        }
        else if (animationName == SkinAnimation.Necromancer)
        {
            return (list.Contains(Achieved.Necromancer75), "Necromancer Earl: Reach Necromancer level 75");
        }
        else if (animationName == SkinAnimation.Orc)
        {
            return (list.Contains(Achieved.Rebirth2), "Orc Earl: Rebirth at least twice");
        }
        else if (animationName == SkinAnimation.Monster)
        {
            return (list.Contains(Achieved.Arena500), "Monster Earl: Reach Arena level 500");
        }
        else
            throw new ArgumentException($"Unknown animation name: {animationName}");
    }

    public void Awake()
    {
        MyAnimations = SkinAnimations.SkinAnimations.Where(x => x.Animation == AnimationName).Single();
        SkinImage.sprite = MyAnimations.IdleSprites[0];
        SetDefaultHovertext();
    }

    private void Update()
    {
        (bool isUnlocked, _) = GetUnlockStatus(AnimationName);
        SkinImage.color = isUnlocked ? Color.white : Color.black;
        Button.interactable = isUnlocked;
        Button.Select();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        (_, string hovertext) = GetUnlockStatus(AnimationName);
        Hovertext.text = hovertext;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetDefaultHovertext();
    }
}
