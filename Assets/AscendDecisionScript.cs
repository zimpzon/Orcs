using Assets.Script.Upgrades;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AscendDecisionScript : MonoBehaviour
{
    public static AscendDecisionScript Instance;

    public AscendProgressScript AscendProgressScript;
    public Button ButtonAscend;
    public TextMeshProUGUI TextCurrentDiamonds;
    public TextMeshProUGUI TextCurrentMonsterCredits;
    public TextMeshProUGUI TextAscendNowGain;
    public TextMeshProUGUI TextButtonRebirth;

    [NonSerialized] public long MonsterCreditsAtStart;
    [NonSerialized] public long DiamondsGainedAtRebirth;

    private int _clickCount;

    private void Awake()
    {
        Instance = this;
    }

    public void OnAscendClick()
    {
        if (_clickCount == 0)
        {
            _clickCount = 1;
            TextButtonRebirth.text = "YOU SURE?";
        }
        else if (_clickCount == 1)
        {
            if (AscendProgressScript.RebirthEnabled)
            {
                SaveGame.Members.MonsterCreditsTemp -= MonsterCreditsAtStart;
                SaveGame.Members.DiamondCount += MonsterCreditsAtStart;

                GameManager.Instance.ResetAllProgress(ascend: true);

                string msg = $"<color=yellow>REBIRTH</color>\n\nWelcome back!\n\nYOU GAINED {DiamondsGainedAtRebirth} DIAMONDS";
                GameCanvasScript.Instance.ShowPopup(msg);
            }
            else
            {
                GameCanvasScript.Instance.ShowPopup("Nothing happened. This feature is not implemented yet.");
            }

            AscendProgressScript.OnCloseClick();
        }
    }

    public void UpdateUi()
    {
        Color gainTextColor = DiamondsGainedAtRebirth == 0 ? new Color(0.9f, 0.2f, 0.1f) : Color.yellow;
        string gainTextColorStr = ColorUtility.ToHtmlStringRGBA(gainTextColor);

        string diamondTxt = SaveGame.Members.DiamondCount == 1 ? "diamond" : "diamonds";
        string diamondGainTxt = DiamondsGainedAtRebirth == 1 ? "diamond" : "diamonds";
        string creditTxt = MonsterCreditsAtStart == 1 ? "credit" : "credits";

        int diamondBonusPct = (int)(PlayerUpgrades.Data.PassiveIncomeDiamondMultiplier * 100.0f);
        string diamondIncomeBonustext = $"(+<color=yellow>{diamondBonusPct}</color>% income)";

        TextCurrentDiamonds.text = $"You have <color=yellow>{SaveGame.Members.DiamondCount}</color> {diamondTxt} <sprite=0>  {diamondIncomeBonustext}";
        TextCurrentMonsterCredits.text = $"You have <color=#{gainTextColorStr}>{MonsterCreditsAtStart}</color> monster {creditTxt} <sprite=0>";
        TextAscendNowGain.text = $"Rebirth now to gain: +<color=#{gainTextColorStr}>{DiamondsGainedAtRebirth}</color> {diamondGainTxt}<sprite=0>";

        ButtonAscend.interactable = SaveGame.Members.MonsterCreditsTemp > 0;
    }

    private void OnEnable()
    {
        MonsterCreditsAtStart = SaveGame.Members.MonsterCreditsTemp;
        DiamondsGainedAtRebirth = UpgradeProgression.DiamondsForMonsterCredits(MonsterCreditsAtStart);
        UpdateUi();

        _clickCount = 0;
        TextButtonRebirth.text = "REBIRTH!";
    }
}
