using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestiaryScript : MonoBehaviour
{
    public EnemyPrefabs EnemyPrefabs;
    public TextMeshProUGUI TextBonusHeader; // "You get bonus income for each unlocked beast." (scene text)
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
        // Beast Scholar rebirth card: the bestiary bonus (and its texts) only once bought.
        bool bought = SaveGame.Members.BoughtBeastScholar;
        SetShown(TextBonusHeader, bought);
        SetShown(TextBonus, bought);

        int incomeBonus = 0;
        if (bought)
        {
            int idx = 0;
            foreach (var beastActor in _beastActors)
            {
                bool isUnlocked = SaveGame.Members.BeastsSeen.Contains(beastActor.ActorType);

                // Bonus grows by 1 per beast: 1, 2, 3... %. Calc is both here and in BeastiaryBeastScript.
                incomeBonus += isUnlocked ? idx + 1 : 0;
                idx++;
            }
            TextBonus.text = $"Bonus: +<color=#8DBE4C>{incomeBonus}</color>% income";
        }

        PlayerUpgrades.Data.PassiveIncomeBestiaryBonuses = incomeBonus * 0.01f;
    }

    static void SetShown(TextMeshProUGUI text, bool shown)
    {
        if (text == null)
            return;
        text.enabled = shown;
        if (text.gameObject.activeSelf != shown)
            text.gameObject.SetActive(shown);
    }
}
