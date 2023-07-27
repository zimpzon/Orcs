using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameProgressScript : MonoBehaviour
{
    public static GameProgressScript Instance;

    public Text TextControls;
    public Text TextHowTo;
    public Text TextScore;

    bool isRunning_;

    public void Begin(GameModeEnum gameMode)
    {
        isRunning_ = true;
        StartCoroutine(Run(gameMode));
    }

    public void Stop()
    {
        isRunning_ = false;
        StopAllCoroutines();
        GetComponent<GameModeUndeadsSpawner>().Stop();
    }

    IEnumerator Run(GameModeEnum gameMode)
    {
        TextScore.enabled = true;

        TextHowTo.text = "Save The Baby Orcs";

        TextControls.enabled = true;
        TextHowTo.enabled = true;

        while (SaveGame.RoundScore == 0)
        {
            GameManager.Instance.ShowingHowToPlay();
            yield return null;
        }

        GameManager.Instance.HidingHowToPlay();
        TextControls.enabled = false;
        TextHowTo.enabled = false;

        // only one gamemode for now
        GetComponent<GameModeUndeadsSpawner>().Run();
    }

    private void Awake()
    {
        Instance = this;
    }
}
