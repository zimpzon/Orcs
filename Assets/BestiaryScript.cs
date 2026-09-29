using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestiaryScript : MonoBehaviour
{
    public EnemyPrefabs EnemyPrefabs;
    public TextMeshProUGUI TextBonus;
    private List<ActorBase> _beastActors = new();

    private void Awake()
    {
        foreach (var beast in EnemyPrefabs.Enemies)
        {
            _beastActors.Add(beast.GetComponent<ActorBase>());
        }
    }

    // Bonus step per beast index. Base 3%, each Beast Scholar ascend card adds +1% (back up to the original 5%).
    public static int PctPerBeast()
    {
        int pct = 3;
        if (SaveGame.Members.BoughtBeastScholar1) pct++;
        if (SaveGame.Members.BoughtBeastScholar2) pct++;
        return pct;
    }

    void Update()
    {
        int incomeBonus = 0;
        int idx = 0;
        foreach (var beastActor in _beastActors)
        {
            bool isUnlocked = SaveGame.Members.BeastsSeen.Contains(beastActor.ActorType);

            // Cap beastiary bonus at 10% so it won't go crazy at high levels. Diamonds are more than enough.
            // Calc is both here and in BeastiaryBeastScript
            incomeBonus += isUnlocked ? (idx + 1) * PctPerBeast() : 0;
            idx++;
        }

        TextBonus.text = $"Bonus: +{incomeBonus}% passive income";
        PlayerUpgrades.Data.PassiveIncomeBestiaryBonuses = incomeBonus * 0.01f;
    }
}
