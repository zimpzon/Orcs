using Assets.Script.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PercentageBonusBuyAmount { Buy1 = 0, Buy10 = 1 }

public class PercentageBonusScript : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI ButtonText;
    public TextMeshProUGUI TextBonusStatus;

    public Button ButtonBuy1;
    public Button ButtonBuy10;
    public TextMeshProUGUI TextBuyAmount;

    public Color TextColorEnabled;
    public Color TextColorDisabled;

    [System.NonSerialized] public PercentageBonusBuyAmount SelectedBuyAmount = PercentageBonusBuyAmount.Buy1;

    private void Start()
    {
        GameEvents.OnSaveWiped += OnSaveWiped;
        SetBuyAmount((int)PercentageBonusBuyAmount.Buy1);
        UpdateAll();
    }

    void OnSaveWiped(GameEvents.SaveWipeReason reason)
    {
        UpdateAll();
    }

    public void SetBuyAmount(int selection)
    {
        SelectedBuyAmount = (PercentageBonusBuyAmount)selection;

        if (ButtonBuy1 != null) ButtonBuy1.interactable = true;
        if (ButtonBuy10 != null) ButtonBuy10.interactable = true;

        if (SelectedBuyAmount == PercentageBonusBuyAmount.Buy1)
        {
            if (ButtonBuy1 != null) ButtonBuy1.interactable = false;
        }
        else if (SelectedBuyAmount == PercentageBonusBuyAmount.Buy10)
        {
            if (ButtonBuy10 != null) ButtonBuy10.interactable = false;
        }

        UpdateAll();
    }

    private int GetBuyAmount()
    {
        return SelectedBuyAmount == PercentageBonusBuyAmount.Buy1 ? 1 : 10;
    }

    private static bool CanAfford()
    {
        Decimal512 priceNext = UpgradeProgression.PriceNextPercentageBonus(SaveGame.Members.LevelPctBought);
        return SaveGame.Members.Money >= priceNext;
    }

    private bool CanAffordBuyAmount()
    {
        Decimal512 totalPrice = GetTotalPriceForBuyAmount();
        return SaveGame.Members.Money >= totalPrice;
    }

    private Decimal512 GetTotalPriceForBuyAmount()
    {
        int buyAmount = GetBuyAmount();
        Decimal512 totalPrice = 0;

        for (int i = 0; i < buyAmount; i++)
        {
            totalPrice += UpgradeProgression.PriceNextPercentageBonus(SaveGame.Members.LevelPctBought + i);
        }

        return totalPrice;
    }

    public void OnBuy()
    {
        if (!CanAffordBuyAmount())
            return;

        Decimal512 totalPrice = GetTotalPriceForBuyAmount();
        int buyAmount = GetBuyAmount();

        GameManager.Instance.DeductMoney(totalPrice);

        SaveGame.Members.LevelPctBought += buyAmount;
        SaveGame.Members.TotalLevelPctBought += buyAmount;
        UpdateAll();
    }

    void SetEnabled(bool enabled)
    {
        Button.interactable = enabled;
        ButtonText.color = enabled ? TextColorEnabled : TextColorDisabled;
    }

    bool _lastCanAfford = false;

    void UpdateAll()
    {
        PlayerUpgrades.Data.PassiveIncomePercentageBonuses = SaveGame.Members.LevelPctBought * 0.01f;

        Decimal512 totalPrice = GetTotalPriceForBuyAmount();
        int buyAmount = GetBuyAmount();
        bool canAfford = CanAffordBuyAmount();

        SetEnabled(canAfford);
        ButtonText.text = $"${Format512.Format(totalPrice)}";

        string bonusText = buyAmount == 1 ? "+1% passive income" : $"+{buyAmount}% passive income";
        TextBonusStatus.text = $"{bonusText}\n<size=-2><color=#cccccc>Bonus: {SaveGame.Members.LevelPctBought}%";

        if (TextBuyAmount != null)
        {
            TextBuyAmount.text = buyAmount == 1 ? "Buy 1" : "Buy 10";
        }
    }

    float _nextUpdate;

    void Update()
    {
        bool canAfford = CanAffordBuyAmount();
        bool doUpdate = canAfford != _lastCanAfford || G.D.GameTime > _nextUpdate;
        if (!doUpdate)
            return;

        _nextUpdate = G.D.GameTime + 0.5f;

        _lastCanAfford = canAfford;
        UpdateAll();
    }
}
