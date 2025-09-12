using Assets.Script.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PercentageBonusScript : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI ButtonText;
    public TextMeshProUGUI TextBonusStatus;

    public Color TextColorEnabled;
    public Color TextColorDisabled;

    private void Start()
    {
        GameEvents.OnSaveWiped += OnSaveWiped;
        UpdateAll();
    }

    void OnSaveWiped(GameEvents.SaveWipeReason reason)
    {
        UpdateAll();
    }

    private static bool CanAfford()
    {
        Decimal512 priceNext = UpgradeProgression.PriceNextPercentageBonus(SaveGame.Members.LevelPctBought);
        return SaveGame.Members.Money >= priceNext;
    }

    public void OnBuy()
    {
        if (!CanAfford())
            return;

        Decimal512 priceNext = UpgradeProgression.PriceNextPercentageBonus(SaveGame.Members.LevelPctBought);
        GameManager.Instance.DeductMoney(priceNext);

        SaveGame.Members.LevelPctBought++;
        SaveGame.Members.TotalLevelPctBought++;
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

        Decimal512 priceNext = UpgradeProgression.PriceNextPercentageBonus(SaveGame.Members.LevelPctBought);
        bool canAfford = CanAfford();
        SetEnabled(canAfford);
        ButtonText.text = $"${Format512.Format(priceNext)}";
        TextBonusStatus.text = $"+1% passive income\n<size=-2><color=#cccccc>Bonus: {SaveGame.Members.LevelPctBought}%";
    }

    float _nextUpdate;

    void Update()
    {
        bool canAfford = CanAfford();
        bool doUpdate = canAfford != _lastCanAfford || G.D.GameTime > _nextUpdate;
        if (!doUpdate)
            return;

        _nextUpdate = G.D.GameTime + 0.5f;

        _lastCanAfford = canAfford;
        UpdateAll();
    }
}
