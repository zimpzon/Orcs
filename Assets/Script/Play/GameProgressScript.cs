using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameProgressScript : MonoBehaviour
{
    public static GameProgressScript Instance;

    public Canvas SelectionCanvas;
    public Text TextControls;
    public Text TextHowTo;
    public Text TextScore;
    public Text TextHowToPause;

    public void Begin(GameModeEnum gameMode)
    {
        StartCoroutine(Run(gameMode));
    }

    public void Stop()
    {
        StopAllCoroutines();
        //GameManager.Instance.Chapter1.GetComponent<Chapter1Controller>().Kill();
    }

    IEnumerator Run(GameModeEnum gameMode)
    {
        TextScore.enabled = true;

        TextHowTo.text = "Save The Ducks";

        TextControls.enabled = true;
        TextHowTo.enabled = true;
        TextHowToPause.enabled = true;

        while (SaveGame.RoundScore == 0)
        {
            GameManager.Instance.ShowingHowToPlay();
            yield return null;
        }

        GameManager.Instance.HidingHowToPlay();
        TextControls.enabled = false;
        TextHowTo.enabled = false;
        TextHowToPause.enabled = false;

        // only one gamemode for now
        GameManager.Instance.Chapter1.SetActive(true);
        GameManager.Instance.Chapter1.GetComponent<Chapter1Controller>().Run();
    }

    private void Awake()
    {
        Instance = this;
    }
}
