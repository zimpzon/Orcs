using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AscendUpgradeCardId
{
    NotSet,
    PassiveIncomeX2_1,
    PassiveIncomeX2_2,
    PassiveIncomeX4_1,
    PassiveIncomeX4_2,
    PassiveIncomeX4_3,
    PassiveIncomeX6_1,
    PassiveIncomeX7_1,
    FasterMystery,
    FasterArena,
    SkinScaryEarl,
    FasterArena2,
    ShinyDiamonds1, // income bonus per diamond held: +5% (on top of the base 5%)
    ShinyDiamonds2, // +10%
    ShinyDiamonds3, // +20%
    ShinyDiamonds4, // +30%
    ShinyDiamonds5, // +50%
    Haggler,        // tier upgrades and X2 cost half
    HeadStart,      // every run starts with 25 minutes of the best-ever income
    SkinCollector,  // +10% income per unlocked skin
    BeastScholar,   // enables the bestiary income bonus (+1%, +2%, +3%... per unlocked beast)
    Completionist,  // +1% income per game completion %
};

public class AscendUpgradeCardScript : MonoBehaviour
{
    public long Cost = 1;
    public Color CanAffordColor = Color.white;
    public Color CannotAffordColor = Color.gray;
    public Color OwnedColor = Color.black;
    public Image ButtonOverlay;
    public Image CardOverlay;
    public Button ButtonBuy;
    public TextMeshProUGUI ButtonBuyText;
    public TextMeshProUGUI OwnedText;
    public AscendUpgradeCardId CardId = AscendUpgradeCardId.NotSet;
    //public AscendUpgradeCardScript UpgradeCardPassiveX2_1;
    //public AscendUpgradeCardScript UpgradeCardPassiveX2_2;
    //public AscendUpgradeCardScript UpgradeCardPassiveX4_1;

    Image _background;

    private void Awake()
    {
        _background = GetComponent<Image>();
    }

    void Update()
    {
        UpdateUi();
    }

    private void UpdateUi()
    {
        // Diamonds etc.
        AscendDecisionScript.Instance.UpdateUi();

        if (CardId == AscendUpgradeCardId.NotSet)
        {
            // Placeholder card - not wired to a real upgrade yet. Always locked/non-interactable.
            OwnedText.enabled = false;
            CardOverlay.enabled = false;
            _background.color = CannotAffordColor;
            ButtonOverlay.enabled = true;
            ButtonBuy.interactable = false;
            ButtonBuyText.text = "???";
            return;
        }

        bool isOwned = CardId switch
        {
            AscendUpgradeCardId.PassiveIncomeX2_1 => SaveGame.Members.BoughtPassiveX2_1,
            AscendUpgradeCardId.PassiveIncomeX2_2 => SaveGame.Members.BoughtPassiveX2_2,
            AscendUpgradeCardId.PassiveIncomeX4_1 => SaveGame.Members.BoughtPassiveX4_1,
            AscendUpgradeCardId.PassiveIncomeX4_2 => SaveGame.Members.BoughtPassiveX4_2,
            AscendUpgradeCardId.PassiveIncomeX4_3 => SaveGame.Members.BoughtPassiveX4_3,
            AscendUpgradeCardId.PassiveIncomeX6_1 => SaveGame.Members.BoughtPassiveX6_1,
            AscendUpgradeCardId.PassiveIncomeX7_1 => SaveGame.Members.BoughtPassiveX7_1,
            AscendUpgradeCardId.FasterMystery => SaveGame.Members.BoughtFasterMystery,
            AscendUpgradeCardId.FasterArena => SaveGame.Members.BoughtFasterArena,
            AscendUpgradeCardId.SkinScaryEarl => SaveGame.Members.BoughtScaryEarlSkin,
            AscendUpgradeCardId.FasterArena2 => SaveGame.Members.BoughtFasterArena2,
            AscendUpgradeCardId.ShinyDiamonds1 => SaveGame.Members.BoughtShinyDiamonds1,
            AscendUpgradeCardId.ShinyDiamonds2 => SaveGame.Members.BoughtShinyDiamonds2,
            AscendUpgradeCardId.ShinyDiamonds3 => SaveGame.Members.BoughtShinyDiamonds3,
            AscendUpgradeCardId.ShinyDiamonds4 => SaveGame.Members.BoughtShinyDiamonds4,
            AscendUpgradeCardId.ShinyDiamonds5 => SaveGame.Members.BoughtShinyDiamonds5,
            AscendUpgradeCardId.Haggler => SaveGame.Members.BoughtHaggler,
            AscendUpgradeCardId.HeadStart => SaveGame.Members.BoughtHeadStart,
            AscendUpgradeCardId.SkinCollector => SaveGame.Members.BoughtSkinCollector,
            AscendUpgradeCardId.BeastScholar => SaveGame.Members.BoughtBeastScholar,
            AscendUpgradeCardId.Completionist => SaveGame.Members.BoughtCompletionist,
            _ => throw new NotImplementedException()
        };

        UpdateUiForSingleCard(isOwned);
    }

    private void UpdateUiForSingleCard(bool isOwned)
    {
        OwnedText.enabled = isOwned;
        CardOverlay.enabled = isOwned;

        if (isOwned)
        {
            _background.color = OwnedColor;
            ButtonOverlay.enabled = false;
            ButtonBuy.interactable = false;
        }
        else
        {
            bool canAfford = SaveGame.Members.DiamondCount_09_08_2025 >= EffectiveCost();
            _background.color = canAfford ? CanAffordColor : CannotAffordColor;
            ButtonOverlay.enabled = !canAfford;
            ButtonBuy.interactable = canAfford;
        }
        ButtonBuyText.text = $"{EffectiveCost()} <sprite=0>";
    }

    long EffectiveCost() => Cost;

    public void OnBuy()
    {
        if (CardId == AscendUpgradeCardId.NotSet)
            return;

        Debug.Log("Bought " + CardId);
        AudioManager.Instance.PlayClipForReal(AudioManager.Instance.AudioData.Chime, minInterval: 0.3f, fixedPitch: true);
        if (CardId == AscendUpgradeCardId.PassiveIncomeX2_1)
        {
            SaveGame.Members.BoughtPassiveX2_1 = true;
        }
        else if (CardId == AscendUpgradeCardId.PassiveIncomeX2_2)
        {
            SaveGame.Members.BoughtPassiveX2_2 = true;
        }
        else if (CardId == AscendUpgradeCardId.PassiveIncomeX4_1)
        {
            SaveGame.Members.BoughtPassiveX4_1 = true;
        }
        else if (CardId == AscendUpgradeCardId.PassiveIncomeX4_2)
        {
            SaveGame.Members.BoughtPassiveX4_2 = true;
        }
        //else if (CardId == AscendUpgradeCardId.PassiveIncomeX4_3)
        //{
        //    SaveGame.Members.BoughtPassiveX4_3 = true;
        //}
        //else if (CardId == AscendUpgradeCardId.PassiveIncomeX6_1)
        //{
        //    SaveGame.Members.BoughtPassiveX6_1 = true;
        //}
        //else if (CardId == AscendUpgradeCardId.PassiveIncomeX7_1)
        //{
        //    SaveGame.Members.BoughtPassiveX7_1 = true;
        //}
        else if (CardId == AscendUpgradeCardId.FasterMystery)
        {
            SaveGame.Members.BoughtFasterMystery = true;
            SaveGame.Members.QuestionMarkRealTimeLeft *= 0.5f;
        }
        else if (CardId == AscendUpgradeCardId.FasterArena)
        {
            SaveGame.Members.BoughtFasterArena = true;
        }
        else if (CardId == AscendUpgradeCardId.SkinScaryEarl)
        {
            SaveGame.Members.BoughtScaryEarlSkin = true;
        }
        else if (CardId == AscendUpgradeCardId.FasterArena2)
        {
            SaveGame.Members.BoughtFasterArena2 = true;
        }
        else if (CardId == AscendUpgradeCardId.ShinyDiamonds1)
        {
            SaveGame.Members.BoughtShinyDiamonds1 = true;
        }
        else if (CardId == AscendUpgradeCardId.ShinyDiamonds2)
        {
            SaveGame.Members.BoughtShinyDiamonds2 = true;
        }
        else if (CardId == AscendUpgradeCardId.ShinyDiamonds3)
        {
            SaveGame.Members.BoughtShinyDiamonds3 = true;
        }
        else if (CardId == AscendUpgradeCardId.ShinyDiamonds4)
        {
            SaveGame.Members.BoughtShinyDiamonds4 = true;
        }
        else if (CardId == AscendUpgradeCardId.ShinyDiamonds5)
        {
            SaveGame.Members.BoughtShinyDiamonds5 = true;
        }
        else if (CardId == AscendUpgradeCardId.Haggler)
        {
            SaveGame.Members.BoughtHaggler = true;
        }
        else if (CardId == AscendUpgradeCardId.HeadStart)
        {
            SaveGame.Members.BoughtHeadStart = true;
        }
        else if (CardId == AscendUpgradeCardId.SkinCollector)
        {
            SaveGame.Members.BoughtSkinCollector = true;
        }
        else if (CardId == AscendUpgradeCardId.BeastScholar)
        {
            SaveGame.Members.BoughtBeastScholar = true;
        }
        else if (CardId == AscendUpgradeCardId.Completionist)
        {
            SaveGame.Members.BoughtCompletionist = true;
        }
        else
            throw new NotImplementedException();

        SaveGame.Members.DiamondCount_09_08_2025 -= EffectiveCost();
        SaveGame.Save();
        AscendDecisionScript.Instance.UpdateUi();
    }
}
