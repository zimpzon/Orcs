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

            // Bonus grows by 1 per beast: 1, 2, 3... %. Calc is both here and in BeastiaryBeastScript.
            incomeBonus += isUnlocked ? idx + 1 : 0;
            idx++;
        }

        TextBonus.text = $"Bonus: +<color=#8DBE4C>{incomeBonus}</color>% passive income";
        PlayerUpgrades.Data.PassiveIncomeBestiaryBonuses = incomeBonus * 0.01f;
    }
}
