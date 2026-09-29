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
    ShinyDiamonds,
    ShinyDiamonds2,
    ShinyDiamonds3,
    ShinyDiamonds4,
    ShinyDiamonds5,
    FasterMystery,
    FasterArena,
    SkinScaryEarl,
    PercentBonusX10,
    HalfEnemyHp,
    Haggler1,
    Haggler2,
    Haggler3,
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
            AscendUpgradeCardId.ShinyDiamonds => SaveGame.Members.BoughtShinyDiamonds,
            AscendUpgradeCardId.ShinyDiamonds2 => SaveGame.Members.BoughtShinyDiamonds2,
            AscendUpgradeCardId.ShinyDiamonds3 => SaveGame.Members.BoughtShinyDiamonds3,
            AscendUpgradeCardId.ShinyDiamonds4 => SaveGame.Members.BoughtShinyDiamonds4,
            AscendUpgradeCardId.ShinyDiamonds5 => SaveGame.Members.BoughtShinyDiamonds5,
            AscendUpgradeCardId.FasterArena => SaveGame.Members.BoughtFasterArena,
            AscendUpgradeCardId.SkinScaryEarl => SaveGame.Members.BoughtScaryEarlSkin,
            AscendUpgradeCardId.PercentBonusX10 => SaveGame.Members.BoughtPercentBonusX10,
            AscendUpgradeCardId.HalfEnemyHp => SaveGame.Members.BoughtHalfEnemyHp,
            AscendUpgradeCardId.Haggler1 => SaveGame.Members.BoughtHaggler1,
            AscendUpgradeCardId.Haggler2 => SaveGame.Members.BoughtHaggler2,
            AscendUpgradeCardId.Haggler3 => SaveGame.Members.BoughtHaggler3,
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
            bool canAfford = SaveGame.Members.DiamondCount_09_08_2025 >= Cost;
            _background.color = canAfford ? CanAffordColor : CannotAffordColor;
            ButtonOverlay.enabled = !canAfford;
            ButtonBuy.interactable = canAfford;
        }
        ButtonBuyText.text = $"{Cost} <sprite=0>";
    }

    public void OnBuy()
    {
        if (CardId == AscendUpgradeCardId.NotSet)
            return;

        Debug.Log("Bought " + CardId);
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
        else if (CardId == AscendUpgradeCardId.ShinyDiamonds)
        {
            SaveGame.Members.BoughtShinyDiamonds = true;
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
        else if (CardId == AscendUpgradeCardId.PercentBonusX10)
        {
            SaveGame.Members.BoughtPercentBonusX10 = true;
        }
        else if (CardId == AscendUpgradeCardId.HalfEnemyHp)
        {
            SaveGame.Members.BoughtHalfEnemyHp = true;
        }
        else if (CardId == AscendUpgradeCardId.Haggler1)
        {
            SaveGame.Members.BoughtHaggler1 = true;
        }
        else if (CardId == AscendUpgradeCardId.Haggler2)
        {
            SaveGame.Members.BoughtHaggler2 = true;
        }
        else if (CardId == AscendUpgradeCardId.Haggler3)
        {
            SaveGame.Members.BoughtHaggler3 = true;
        }
        else
            throw new NotImplementedException();

        SaveGame.Members.DiamondCount_09_08_2025 -= Cost;
        SaveGame.Save();
        AscendDecisionScript.Instance.UpdateUi();
    }
}
