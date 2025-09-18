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

    void Update()
    {
        int incomeBonus = 0;
        int idx = 0;
        foreach (var beastActor in _beastActors)
        {
            bool isUnlocked = SaveGame.Members.BeastsSeen.Contains(beastActor.ActorType);

            // Cap beastiary bonus at 10% so it won't go crazy at high levels. Diamonds are more than enough.
            // Calc is both here and in BeastiaryBeastScript
            incomeBonus += Mathf.Min(10, isUnlocked ? (idx / 2) + 1 : 0);
            idx++;
        }

        TextBonus.text = $"Bonus: +{incomeBonus}% passive income";
        PlayerUpgrades.Data.PassiveIncomeBestiaryBonuses = incomeBonus * 0.01f;
    }
}
