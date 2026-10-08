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

    public void Awake()
    {
        MyAnimations = SkinAnimations.SkinAnimations.Where(x => x.Animation == AnimationName).Single();
        SkinImage.sprite = MyAnimations.IdleSprites[0];
        SetDefaultHovertext();
    }

    void SetSkin(SkinAnimation skinAnimation)
    {
        SelectedSkinScript.Instance.SelectedSkinAnimation = MyAnimations;
        SaveGame.Members.SelectedSkin = AnimationName;
        SaveGame.Save();
    }

    public void OnClick()
    {
        SetSkin(AnimationName);
        (_, string beastDescription) = GetUnlockStatus(AnimationName);
        // Title (nemesis of...) removed
        //TitleTextScript.Instance?.SetName(beastDescription.Substring(0, beastDescription.IndexOf(':')));
    }

    public static (bool isUnlocked, string hoverText) GetUnlockStatus(SkinAnimation animationName)
    {
        var list = SaveGame.Members.Achieved;

        if (animationName == SkinAnimation.Default)
        {
            return (true, "Good ol' Earl: always there");
        }
        else if (animationName == SkinAnimation.WitchDoctor)
        {
            return (list.Contains(Achieved.WitchDoctor25), "Witch Doctor Earl: Reach Witch Doctor level 25");
        }
        else if (animationName == SkinAnimation.Necromancer)
        {
            return (list.Contains(Achieved.Necromancer200), "Necromancer Earl: Reach Necromancer level 200");
        }
        else if (animationName == SkinAnimation.WhiteWalkerEarl)
        {
            return (list.Contains(Achieved.Rebirth1), "White Walker Earl: Rebirth 1 time");
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
            return (list.Contains(Achieved.Arena35000), "Big Mouth Earl: Reach Arena level 35000");
        }
        else if (animationName == SkinAnimation.Pirate)
        {
            return (list.Contains(Achieved.Chest50), "Pirate Earl: Loot 50 Chests");
        }
        else if (animationName == SkinAnimation.Slug)
        {
            return (list.Contains(Achieved.Mystery100), "Slug Earl: Get 100 mystery rewards");
        }
        else if (animationName == SkinAnimation.Zombie)
        {
            return (list.Contains(Achieved.SkullCrusher5), "Zombie Earl: Reach Skull Crusher level 5");
        }
        else if (animationName == SkinAnimation.EvilEyesEarl)
        {
            return (list.Contains(Achieved.Arena1000), "Evil Eyes Earl: Reach Arena level 1000");
        }
        else if (animationName == SkinAnimation.Wig)
        {
            return (list.Contains(Achieved.Arena10000), "Wig Earl: Reach Arena level 10000");
        }
        else if (animationName == SkinAnimation.KaratEarl)
        {
            return (list.Contains(Achieved.Arena75000), "KaratEarl: Reach Arena level 75000");
        }
        else if (animationName == SkinAnimation.Voidgazer)
        {
            return (list.Contains(Achieved.StormLordTier), "Voidgazer Earl: Reach Storm Lord");
        }
        else if (animationName == SkinAnimation.Wizard)
        {
            return (list.Contains(Achieved.MasterWizardTier), "Wizard Earl: Reach Master Wizard");
        }
        else if (animationName == SkinAnimation.UndeadBeauty)
        {
            return (list.Contains(Achieved.Rebirth8), "Undead Beauty Earl: Rebirth 8 times");
        }
        else if (animationName == SkinAnimation.HonorableKnight)
        {
            return (list.Contains(Achieved.Diamonds250), "Honorable Knight Earl: Have at least 250 diamonds");
        }
        else if (animationName == SkinAnimation.FreakyEarl)
        {
            return (list.Contains(Achieved.Diamonds1000), "Freaky Earl: Have at least 1000 diamonds");
        }
        else if (animationName == SkinAnimation.WellDressedOrc)
        {
            return (list.Contains(Achieved.Played5Days), "Well-dressed Orc Earl: Play for 5 days");
        }
        else if (animationName == SkinAnimation.Alien)
        {
            return (list.Contains(Achieved.ChainZap200), "Alien Earl: Reach Chain Zapping level 200");
        }
        else if (animationName == SkinAnimation.NinjaEarl)
        {
            return (list.Contains(Achieved.Diamonds50), "Ninja Earl: Have at least 50 diamonds");
        }
        else if (animationName == SkinAnimation.SecretiveEarl)
        {
            return (list.Contains(Achieved.X2_150), "Secretive Earl: Reach 150 X2 bonus");
        }
        else if (animationName == SkinAnimation.SkaterEarl)
        {
            return (list.Contains(Achieved.X2_300), "Skater Earl: Reach 300 X2 bonus");
        }
        else if (animationName == SkinAnimation.PrettyEarl)
        {
            return (list.Contains(Achieved.Skins20), "Pretty Earl: Unlock 20 skins");
        }
        else if (animationName == SkinAnimation.AttentivePigEarl)
        {
            return (list.Contains(Achieved.Upgrades500), "Attentive Pig Earl: Buy 500 upgrades");
        }
        else if (animationName == SkinAnimation.ToxicEarl)
        {
            return (list.Contains(Achieved.Upgrades2500), "Toxic Earl: Buy 2500 upgrades");
        }
        else if (animationName == SkinAnimation.DisguisedMonsterEarl)
        {
            return (list.Contains(Achieved.Upgrades10000), "Disguised Monster Earl: Buy 10000 upgrades");
        }
        else if (animationName == SkinAnimation.ScaryEarl)
        {
            return (list.Contains(Achieved.BuyCardScaryEarl), "Scary Earl: Buy with diamonds");
        }
        else if (animationName == SkinAnimation.SlickEarl)
        {
            return (list.Contains(Achieved.SmartDagger10), "Slick Earl: Reach Smart Daggers level 10");
        }
        else if (animationName == SkinAnimation.GangsterEarl)
        {
            return (list.Contains(Achieved.ChestMaster200), "Gangster Earl: Reach Bountiful level 200");
        }
        else if (animationName == SkinAnimation.ChickenEarl)
        {
            return (list.Contains(Achieved.Have1Percent50), "Chicken Earl: Have 50 1% bonuses");
        }
        else if (animationName == SkinAnimation.YoungEarl)
        {
            return (list.Contains(Achieved.Buy1PercentTotal500), "Young Earl: Buy 500 1% bonuses");
        }
        else if (animationName == SkinAnimation.BallEarl)
        {
            return (list.Contains(Achieved.Diamonds5000), "Ball Earl: Have at least 5000 diamonds");
        }
        else if (animationName == SkinAnimation.MonochromeEarl)
        {
            return (list.Contains(Achieved.Completion100), "Earl Grey: Reach 100% game completion");
        }
        else if (animationName == SkinAnimation.SaturatedEarl)
        {
            return (list.Contains(Achieved.Rebirth20), "Saturated Earl: Rebirth 20 times");
        }
        else if (animationName == SkinAnimation.WhiteEarl)
        {
            return (list.Contains(Achieved.ArenasLost100), "White Earl: Buy 25000 upgrades");
        }
        else if (animationName == SkinAnimation.BlackEarl)
        {
            return (list.Contains(Achieved.SuperClears1000), "Black Earl: Get 1000 super fast arena clears");
        }
        else if (animationName == SkinAnimation.InvertedEarl)
        {
            return (list.Contains(Achieved.Credits1000), "Inverted Earl: Earn 1000 monster credits");
        }
        else if (animationName == SkinAnimation.NeonEarl)
        {
            return (list.Contains(Achieved.StormLord5), "Neon Earl: Reach Storm Lord level 5");
        }
        else if (animationName == SkinAnimation.ExpressionistEarl)
        {
            return (list.Contains(Achieved.Mystery50), "Expressionist Earl: Get 50 mystery rewards");
        }
        else
            throw new ArgumentException($"Unknown animation name: {animationName}");
    }

    // How many skins have been unlocked, out of every skin actually present in the game - used for
    // the game progress popup. Reads the live SkinScript instances (Skins popup, may be inactive)
    // rather than the SkinAnimation enum directly, since the enum has an unused GhostEarl entry
    // with no unlock rule and no in-game representation.
    public static (int unlocked, int total) GetUnlockProgress()
    {
        // Count distinct skins (by animation), not objects: the scene has several objects sharing one skin
        // (e.g. 5 with animation 19), which made the total 39 instead of 35.
        var allSkins = FindObjectsOfType<SkinScript>(true);
        int total = allSkins.Select(s => s.AnimationName).Distinct().Count();
        int unlocked = allSkins
            .Where(s => GetUnlockStatus(s.AnimationName).isUnlocked)
            .Select(s => s.AnimationName)
            .Distinct()
            .Count();
        return (unlocked, total);
    }

    // Progress toward a skin's requirement, shown on hover as "(345/1000)". Null for yes/no requirements.
    // Keep the stats and targets in sync with AchievementChecks.
    static (double current, double target)? GetRequirementProgress(SkinAnimation skin)
    {
        var m = SaveGame.Members;
        return skin switch
        {
            SkinAnimation.WitchDoctor => (m.LevelWitchDoctor, 25),
            SkinAnimation.Necromancer => (m.LevelMoneyMaker, 200),
            SkinAnimation.Alien => (m.LevelClickDamage, 200),
            SkinAnimation.Zombie => (m.LevelSkullCrusher, 5),
            SkinAnimation.SlickEarl => (m.LevelSmartDaggers, 10),
            SkinAnimation.GangsterEarl => (m.LevelChestMaster, 200),
            SkinAnimation.NeonEarl => (m.LevelStormLord, 5),

            SkinAnimation.WhiteWalkerEarl => (m.TimesAscended_09_08_2025, 1),
            SkinAnimation.Orc => (m.TimesAscended_09_08_2025, 2),
            SkinAnimation.UndeadBeauty => (m.TimesAscended_09_08_2025, 8),
            SkinAnimation.SaturatedEarl => (m.TimesAscended_09_08_2025, 20),

            SkinAnimation.Monster => (m.DiamondCount_09_08_2025, 10),
            SkinAnimation.NinjaEarl => (m.DiamondCount_09_08_2025, 50),
            SkinAnimation.HonorableKnight => (m.DiamondCount_09_08_2025, 250),
            SkinAnimation.FreakyEarl => (m.DiamondCount_09_08_2025, 1000),
            SkinAnimation.BallEarl => (m.DiamondCount_09_08_2025, 5000),

            SkinAnimation.EvilEyesEarl => (m.ArenaLevel, 1000),
            SkinAnimation.Wig => (m.ArenaLevel, 10000),
            SkinAnimation.BigMouth => (m.ArenaLevel, 35000),
            SkinAnimation.KaratEarl => (m.ArenaLevel, 75000),

            SkinAnimation.Pirate => (m.ChestsCollected, 50),
            SkinAnimation.Slug => (m.MysteryCollected, 100),
            SkinAnimation.ExpressionistEarl => (m.MysteryCollected, 50),
            SkinAnimation.SecretiveEarl => (PlayerUpgrades.Data.NumberOfX2Bought, 150),
            SkinAnimation.SkaterEarl => (PlayerUpgrades.Data.NumberOfX2Bought, 300),
            SkinAnimation.ChickenEarl => (m.LevelPctBought, 50),
            SkinAnimation.YoungEarl => (m.TotalLevelPctBought, 500),
            SkinAnimation.AttentivePigEarl => (m.TotalUpgradesBought, 500),
            SkinAnimation.ToxicEarl => (m.TotalUpgradesBought, 2500),
            SkinAnimation.DisguisedMonsterEarl => (m.TotalUpgradesBought, 10000),
            SkinAnimation.PrettyEarl => (m.Achieved.Count, 20),

            SkinAnimation.WhiteEarl => (m.TotalUpgradesBought, 25000),
            SkinAnimation.BlackEarl => (m.SuperFastClears, 1000),
            SkinAnimation.InvertedEarl => (m.MonsterCreditsLifetime_09_08_2025, 1000),
            SkinAnimation.WellDressedOrc => (m.EstimatedOnlineSeconds2 / (24 * 60 * 60), 5),
            SkinAnimation.MonochromeEarl => (GameCompletion.GetProgress().TotalPct, 100),

            _ => null,
        };
    }

    string GetHoverText()
    {
        (bool isUnlocked, string hovertext) = GetUnlockStatus(AnimationName);
        var progress = isUnlocked ? null : GetRequirementProgress(AnimationName);
        if (progress == null)
            return hovertext;

        (double current, double target) = progress.Value;
        current = Math.Min(Math.Max(current, 0), target);
        // Days played gets a decimal, everything else is a whole number. Floor so it never shows the target early.
        string format = AnimationName == SkinAnimation.WellDressedOrc ? "0.0" : "#,0";
        if (format == "#,0")
            current = Math.Floor(current);
        string cur = Assets.Script.Misc.DisplayNumberFormat.Format(current, format);
        string tgt = Assets.Script.Misc.DisplayNumberFormat.Format(target, "#,0");
        return $"{hovertext} <color=#AAAAAA>({cur}/{tgt})</color>";
    }

    bool _hovered;
    float _nextHoverRefresh;

    private void Update()
    {
        (bool isUnlocked, string text) = GetUnlockStatus(AnimationName);
        SkinImage.color = isUnlocked ? Color.white : Color.black;
        Button.interactable = isUnlocked;
        Button.Select();

        // Keep the progress ticking while hovered.
        if (_hovered && Time.unscaledTime >= _nextHoverRefresh)
        {
            _nextHoverRefresh = Time.unscaledTime + 0.5f;
            Hovertext.text = GetHoverText();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovered = true;
        _nextHoverRefresh = Time.unscaledTime + 0.5f;
        Hovertext.text = GetHoverText();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
        SetDefaultHovertext();
    }
}
