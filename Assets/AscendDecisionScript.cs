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
                SaveGame.Members.MonsterCredits_09_08_2025 -= MonsterCreditsAtStart;
                SaveGame.Members.DiamondCount_09_08_2025 += MonsterCreditsAtStart;
                SaveGame.Members.TimesAscended_09_08_2025++;
                SaveGame.Members.TimeSinceLastAscend = 0;

                GameManager.Instance.ResetAllProgress(ascend: true);

                string diamondGainTxt = DiamondsGainedAtRebirth == 1 ? "DIAMOND" : "DIAMONDS";
                string msg = $"<color=yellow>REBIRTH</color>\n\nWelcome back!\n\nYOU GAINED {DiamondsGainedAtRebirth} {diamondGainTxt}";
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
        //Color gainTextColor = DiamondsGainedAtRebirth == 0 ? new Color(0.9f, 0.2f, 0.1f) : Color.yellow;
        string gainTextColorStr = DiamondsGainedAtRebirth == 0 ?  "E35125" : "9DE05C";// ColorUtility.ToHtmlStringRGBA(gainTextColor);

        string diamondTxt = SaveGame.Members.DiamondCount_09_08_2025 == 1 ? "diamond" : "diamonds";
        string diamondGainTxt = DiamondsGainedAtRebirth == 1 ? "diamond" : "diamonds";
        string creditTxt = MonsterCreditsAtStart == 1 ? "credit" : "credits";

        int diamondBonusPct = (int)Math.Round(PlayerUpgrades.Data.PassiveIncomeDiamondMultiplier * 100.0f);
        string diamondIncomeBonustext = $"(+<color=#9DE05C>{diamondBonusPct}</color>% income)";

        TextCurrentDiamonds.text = $"You have <color=#9DE05C>{SaveGame.Members.DiamondCount_09_08_2025}</color> {diamondTxt} <sprite=0>  {diamondIncomeBonustext}";
        TextCurrentMonsterCredits.text = $"You have <color=#{gainTextColorStr}>{MonsterCreditsAtStart}</color> {creditTxt}";
        TextAscendNowGain.text = $"Rebirth now to gain: +<color=#{gainTextColorStr}>{DiamondsGainedAtRebirth}</color> {diamondGainTxt}<sprite=0>";

        ButtonAscend.interactable = SaveGame.Members.MonsterCredits_09_08_2025 > 0;
    }

    private void OnEnable()
    {
        MonsterCreditsAtStart = SaveGame.Members.MonsterCredits_09_08_2025;
        DiamondsGainedAtRebirth = UpgradeProgression.DiamondsForMonsterCredits(MonsterCreditsAtStart);
        UpdateUi();

        _clickCount = 0;
        TextButtonRebirth.text = "REBIRTH!";
    }
}
