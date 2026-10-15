using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestiaryScript : MonoBehaviour
{
    public EnemyPrefabs EnemyPrefabs;
    public TextMeshProUGUI TextBonusHeader; // "You get bonus income for each unlocked beast." (scene text)
    public TextMeshProUGUI TextBonus;
    private List<ActorBase> _beastActors = new();
    string _headerTemplate; // scene text, shown once Beast Scholar is bought

    // Bonus line before the card is bought: shown, but faded (same alpha as the disabled Fullscreen checkbox).
    const float DisabledAlpha = 0.4f;

    private void Awake()
    {
        if (TextBonusHeader != null)
            _headerTemplate = TextBonusHeader.text;

        foreach (var beast in EnemyPrefabs.Enemies)
        {
            _beastActors.Add(beast.GetComponent<ActorBase>());
        }
    }

    // Income bonus per beast (bestiary order), a step progression with a slow start: little for the early beasts, a lot
    // for the last ones. Sums to 840% (a bit over the old 1+2+...+39 = 780%; flattened from 26 up so the top is less
    // heavy), so the early game has no big jump while the late game multiplies strongly (the bestiary, skin and
    // completion bonuses multiply each other).
    static readonly int[] BeastBonusTable =
    {
        1, 1, 1, 1, 1,          // 1-5
        2, 2, 2, 2, 2,          // 6-10
        4, 4, 4, 4, 4,          // 11-15
        8, 8, 8, 8, 8,          // 16-20
        15, 15, 15, 15, 15,     // 21-25
        35, 35, 35, 35, 35,     // 26-30
        50, 50, 50, 50,         // 31-34
        60, 60, 60,             // 35-37
        65, 70,                 // 38-39
    };

    // % for the beast at this bestiary index (0-based). Beyond the table the last value repeats.
    public static int BeastBonusPct(int index)
        => BeastBonusTable[Mathf.Clamp(index, 0, BeastBonusTable.Length - 1)];

    void Update()
    {
        // The bonus is always shown; it only applies once the Beast Scholar rebirth card is bought.
        bool bought = SaveGame.Members.BoughtBeastScholar;

        int incomeBonus = 0;
        int idx = 0;
        foreach (var beastActor in _beastActors)
        {
            if (SaveGame.Members.BeastsSeen.Contains(beastActor.ActorType))
                incomeBonus += BeastBonusPct(idx);
            idx++;
        }

        // Not bought: "Unlock with Beast Scholar" and a faded bonus line (no green). Bought: the scene header, green number.
        if (TextBonusHeader != null)
            TextBonusHeader.text = bought ? _headerTemplate : "Unlock bonus with Beast Scholar";
        if (TextBonus != null)
        {
            TextBonus.text = bought
                ? $"Bonus: +<color={TextColors.Green}>{incomeBonus}</color>% income"
                : $"Bonus: +{incomeBonus}% income";
            TextBonus.alpha = bought ? 1f : DisabledAlpha;
        }

        PlayerUpgrades.Data.PassiveIncomeBestiaryBonuses = bought ? incomeBonus * 0.01 : 0.0;
    }
}
