using Assets.Script.Upgrades;
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

    public long MonsterCreditsAtStart;
    public long DiamondsGainedAtRebirth;

    private void Awake()
    {
        Instance = this;
    }

    public void OnAscendClick()
    {
        SaveGame.Members.MonsterCreditsTemp -= MonsterCreditsAtStart;

        SaveGame.Members.DiamondCount += MonsterCreditsAtStart;

        // not doing this right now
        //SaveGameAscend.AscendSaveGame();

        string msg = $"YOU GAINED {DiamondsGainedAtRebirth} DIAMONDS<sprite=0>" +
        "\n<color=yellow>(you were not reborn, you suspect this feature is not done)";

        FloatingTextSpawner.Instance.Spawn(
        GameManager.Instance.ArenaCenter,
        msg,
        Color.white,
        speed: 0.01f,
        timeToLive: 10.0f,
        fontStyle: FontStyles.Italic);

        AscendProgressScript.OnCloseClick();
    }

    void UpdateUi()
    {
        //ButtonAscend.interactable = _diamondsGainedAtRebirth > 0;
        Color gainTextColor = DiamondsGainedAtRebirth == 0 ? new Color(0.9f, 0.2f, 0.1f) : Color.yellow;
        string gainTextColorStr = ColorUtility.ToHtmlStringRGBA(gainTextColor);

        string diamondTxt = SaveGame.Members.DiamondCount == 1 ? "diamond" : "diamonds";
        string diamondGainTxt = DiamondsGainedAtRebirth == 1 ? "diamond" : "diamonds";
        string creditTxt = MonsterCreditsAtStart == 1 ? "credit" : "credits";

        TextCurrentDiamonds.text = $"You have <color=yellow>{SaveGame.Members.DiamondCount}</color> {diamondTxt} <sprite=0>";
        TextCurrentMonsterCredits.text = $"You have <color=#{gainTextColorStr}>{MonsterCreditsAtStart}</color> monster {creditTxt} <sprite=0>";
        TextAscendNowGain.text = $"Rebirth now to gain: +<color=#{gainTextColorStr}>{DiamondsGainedAtRebirth}</color> {diamondGainTxt}<sprite=0>";
    }

    private void OnEnable()
    {
        MonsterCreditsAtStart = SaveGame.Members.MonsterCreditsTemp;
        DiamondsGainedAtRebirth = UpgradeProgression.DiamondsForMonsterCredits(MonsterCreditsAtStart);
        UpdateUi();
    }
}
