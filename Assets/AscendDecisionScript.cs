using Assets.Script.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AscendDecisionScript : MonoBehaviour
{
    public AscendProgressScript AscendProgressScript;
    public Button ButtonAscend;
    public TextMeshProUGUI TextCurrentDiamonds;
    public TextMeshProUGUI TextCurrentMonsterCredits;
    public TextMeshProUGUI TextAscendNowGain;

    private long _monsterCreditsAtStart;
    private long _diamondsGainedAtRebirth;

    public void OnAscendClick()
    {
        SaveGame.Members.MonsterCreditsTemp -= _monsterCreditsAtStart;
        SaveGame.Members.DiamondCount += _monsterCreditsAtStart;

        // not doing this right now
        //SaveGameAscend.AscendSaveGame();

        //"You were born again, welcome!",

        FloatingTextSpawner.Instance.Spawn(
        GameManager.Instance.ArenaCenter,
        "(you were not reborn, you suspect this feature is not done)",
        Color.white,
        speed: 0.01f,
        timeToLive: 8.0f,
        fontStyle: FontStyles.Italic);

        AscendProgressScript.OnCloseClick();
    }

    void UpdateUi()
    {
        ButtonAscend.interactable = _diamondsGainedAtRebirth > 0;
        Color gainTextColor = _diamondsGainedAtRebirth == 0 ? new Color(0.9f, 0.2f, 0.1f) : Color.yellow;
        string gainTextColorStr = ColorUtility.ToHtmlStringRGBA(gainTextColor);

        TextCurrentDiamonds.text = $"You have <color=yellow>{SaveGame.Members.DiamondCount}</color> diamonds <sprite=0>";
        TextCurrentMonsterCredits.text = $"You have <color=yellow>{_monsterCreditsAtStart}</color> monster credits  <sprite=0>";
        TextAscendNowGain.text = $"Rebirth now to gain: +<color=#{gainTextColorStr}>{_diamondsGainedAtRebirth}</color> diamonds<sprite=0>";
    }

    private void OnEnable()
    {
        _monsterCreditsAtStart = SaveGame.Members.MonsterCreditsTemp;
        _diamondsGainedAtRebirth = UpgradeProgression.DiamondsForMonsterCredits(_monsterCreditsAtStart);
        UpdateUi();
    }
}
