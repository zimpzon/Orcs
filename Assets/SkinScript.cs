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
            return (list.Contains(Achieved.WitchDoctor25), "Witch Doctor Earl: Reach Witch Doctor level 25");
        }
        else if (animationName == SkinAnimation.Necromancer)
        {
            return (list.Contains(Achieved.Necromancer25), "Necromancer Earl: Reach Necromancer level 25");
        }
        else if (animationName == SkinAnimation.Orc)
        {
            return (list.Contains(Achieved.Rebirth2), "Orc Earl: Rebirth 2 times");
        }
        else if (animationName == SkinAnimation.Monster)
        {
            return (list.Contains(Achieved.Diamonds10), "Monster Earl: Have at least 10 diamonds");
        }
        else if (animationName == SkinAnimation.BigMouth)
        {
            return (list.Contains(Achieved.Arena1000), "Big Mouth Earl: Reach Arena level 1000");
        }
        else if (animationName == SkinAnimation.Pirate)
        {
            return (list.Contains(Achieved.Chest25), "Pirate Earl: Loot 25 Chests");
        }
        else if (animationName == SkinAnimation.Slug)
        {
            return (list.Contains(Achieved.Mystery25), "Slug Earl: Get 25 mystery rewards");
        }
        else if (animationName == SkinAnimation.Wizard)
        {
            return (list.Contains(Achieved.Rebirth3), "Wizard Earl: Rebirth 3 times");
        }
        else if (animationName == SkinAnimation.Zombie)
        {
            return (list.Contains(Achieved.SkullCrusher5), "Zombie Earl: Reach Skull Crusher level 5");
        }
        else if (animationName == SkinAnimation.Wig)
        {
            return (list.Contains(Achieved.Arena500), "Wig Earl: Reach Arena level 500");
        }
        else if (animationName == SkinAnimation.KaratEarl)
        {
            return (list.Contains(Achieved.Arena1500), "KaratEarl: Reach Arena level 1500");
        }
        else if (animationName == SkinAnimation.Voidgazer)
        {
            return (list.Contains(Achieved.VoidgazerTier), "Voidgazer Earl: Reach Voidgazer");
        }

        else if (animationName == SkinAnimation.UndeadBeauty)
        {
            return (list.Contains(Achieved.Rebirth8), "Undead Beauty Earl: Rebirth 8 times");
        }
        else if (animationName == SkinAnimation.HonorableKnight)
        {
            return (list.Contains(Achieved.Diamonds250), "Honorable Knight Earl: Have at least 250 diamonds");
        }
        else if (animationName == SkinAnimation.WellDressedOrc)
        {
            return (list.Contains(Achieved.Chest50), "Well-dressed Orc Earl: Loot 50 chests");
        }
        else if (animationName == SkinAnimation.Alien)
        {
            return (list.Contains(Achieved.ChainZap200), "Alien Earl: Reach Chain Zapping level 200");
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
