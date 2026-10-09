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
    // Removed cards (Shiny Diamonds 1-5). Kept as placeholders: card ids are stored as numbers in the scene, so
    // deleting them would shift every card after them.
    ZapLore,     // +1% income per Chain Zapping level (was Shiny Diamonds 1)
    ChestLore,   // +1% income per Richer Chests level (was Shiny Diamonds 2)
    VoodooLore,  // +1% income per Witch Doctor level (was Shiny Diamonds 3)
    WizardLore,  // +1% income per Wizard level (was Shiny Diamonds 4)
    RemovedVeteran, // removed card (the per-rebirth bonus is now built in); placeholder so ids don't shift
    FasterMystery,
    FasterArena,
    SkinScaryEarl,
    PercentBonusX10,
    HalfEnemyHp,
    Haggler1,
    Haggler2,
    Haggler3,
    BeastScholar1,
    BeastScholar2,
    X2Mastery1,
    X2Mastery2,
    X2Mastery3,
    FasterArena2,
    CardDiscount,
    SkinCollector,
    SkinCollector2,
    Completionist,
    HeadStart,   // start every run with 1 minute of the best-ever income
    DiamondHoard, // +1% income per diamond ever earned
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
            AscendUpgradeCardId.PercentBonusX10 => SaveGame.Members.BoughtPercentBonusX10,
            AscendUpgradeCardId.HalfEnemyHp => SaveGame.Members.BoughtHalfEnemyHp,
            AscendUpgradeCardId.Haggler1 => SaveGame.Members.BoughtHaggler1,
            AscendUpgradeCardId.Haggler2 => SaveGame.Members.BoughtHaggler2,
            AscendUpgradeCardId.Haggler3 => SaveGame.Members.BoughtHaggler3,
            AscendUpgradeCardId.BeastScholar1 => SaveGame.Members.BoughtBeastScholar1,
            AscendUpgradeCardId.BeastScholar2 => SaveGame.Members.BoughtBeastScholar2,
            AscendUpgradeCardId.X2Mastery1 => SaveGame.Members.BoughtX2Mastery1,
            AscendUpgradeCardId.X2Mastery2 => SaveGame.Members.BoughtX2Mastery2,
            AscendUpgradeCardId.X2Mastery3 => SaveGame.Members.BoughtX2Mastery3,
            AscendUpgradeCardId.FasterArena2 => SaveGame.Members.BoughtFasterArena2,
            AscendUpgradeCardId.CardDiscount => SaveGame.Members.BoughtCardDiscount,
            AscendUpgradeCardId.ZapLore => SaveGame.Members.BoughtZapLore,
            AscendUpgradeCardId.ChestLore => SaveGame.Members.BoughtChestLore,
            AscendUpgradeCardId.VoodooLore => SaveGame.Members.BoughtVoodooLore,
            AscendUpgradeCardId.WizardLore => SaveGame.Members.BoughtWizardLore,
            AscendUpgradeCardId.SkinCollector => SaveGame.Members.BoughtSkinCollector,
            AscendUpgradeCardId.SkinCollector2 => SaveGame.Members.BoughtSkinCollector2,
            AscendUpgradeCardId.Completionist => SaveGame.Members.BoughtCompletionist,
            AscendUpgradeCardId.HeadStart => SaveGame.Members.BoughtHeadStart,
            AscendUpgradeCardId.DiamondHoard => SaveGame.Members.BoughtDiamondHoard,
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

    // Cost in the Inspector is the base price and is never modified. Diamond Deals makes every other card cheaper.
    const double CardDiscountMul = 0.8;

    long EffectiveCost()
    {
        if (!SaveGame.Members.BoughtCardDiscount || CardId == AscendUpgradeCardId.CardDiscount)
            return Cost;

        return Math.Max(1, (long)Math.Round(Cost * CardDiscountMul));
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
        else if (CardId == AscendUpgradeCardId.BeastScholar1)
        {
            SaveGame.Members.BoughtBeastScholar1 = true;
        }
        else if (CardId == AscendUpgradeCardId.BeastScholar2)
        {
            SaveGame.Members.BoughtBeastScholar2 = true;
        }
        else if (CardId == AscendUpgradeCardId.X2Mastery1)
        {
            SaveGame.Members.BoughtX2Mastery1 = true;
        }
        else if (CardId == AscendUpgradeCardId.X2Mastery2)
        {
            SaveGame.Members.BoughtX2Mastery2 = true;
        }
        else if (CardId == AscendUpgradeCardId.X2Mastery3)
        {
            SaveGame.Members.BoughtX2Mastery3 = true;
        }
        else if (CardId == AscendUpgradeCardId.FasterArena2)
        {
            SaveGame.Members.BoughtFasterArena2 = true;
        }
        else if (CardId == AscendUpgradeCardId.CardDiscount)
        {
            SaveGame.Members.BoughtCardDiscount = true;
        }
        else if (CardId == AscendUpgradeCardId.ZapLore)
        {
            SaveGame.Members.BoughtZapLore = true;
        }
        else if (CardId == AscendUpgradeCardId.ChestLore)
        {
            SaveGame.Members.BoughtChestLore = true;
        }
        else if (CardId == AscendUpgradeCardId.VoodooLore)
        {
            SaveGame.Members.BoughtVoodooLore = true;
        }
        else if (CardId == AscendUpgradeCardId.WizardLore)
        {
            SaveGame.Members.BoughtWizardLore = true;
        }
        else if (CardId == AscendUpgradeCardId.SkinCollector)
        {
            SaveGame.Members.BoughtSkinCollector = true;
        }
        else if (CardId == AscendUpgradeCardId.SkinCollector2)
        {
            SaveGame.Members.BoughtSkinCollector2 = true;
        }
        else if (CardId == AscendUpgradeCardId.Completionist)
        {
            SaveGame.Members.BoughtCompletionist = true;
        }
        else if (CardId == AscendUpgradeCardId.HeadStart)
        {
            SaveGame.Members.BoughtHeadStart = true;
        }
        else if (CardId == AscendUpgradeCardId.DiamondHoard)
        {
            SaveGame.Members.BoughtDiamondHoard = true;
        }
        else
            throw new NotImplementedException();

        SaveGame.Members.DiamondCount_09_08_2025 -= EffectiveCost();
        SaveGame.Save();
        AscendDecisionScript.Instance.UpdateUi();
    }
}
