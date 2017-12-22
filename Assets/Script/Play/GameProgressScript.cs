using Assets.Script;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum EnabledSpawns { WalkerDefault, BigWalkerDefault, CasterDefault, DeedSniper, DeedMachinegun, DeedWhiteWalkers };

public class GameProgressScript : MonoBehaviour
{
    public static GameProgressScript Instance;

    public Text TextControls;
    public Text TextHowTo;
    public Text TextDeedTitle;
    public Text TextScore;
    public Text TextDeedScore;

    public GameObject SkellieWalkerPrefab;
    public GameObject SkellieChargerPrefab;
    public GameObject BigSkellieWalkerPrefab;
    public GameObject SkellieCasterPrefab;

    bool isRunning_;
    DeedData currentDeed_;

    public void Begin()
    {
        currentDeed_ = GameManager.Instance.CurrentDeedData;
        isRunning_ = true;
        StartCoroutine(StartWave(0));
    }

    public void Stop()
    {
        isRunning_ = false;
        StopAllCoroutines();
    }

    IEnumerator StartWave(int wave)
    {
        bool isDeed = currentDeed_.Deed != DeedEnum.None;

        TextScore.enabled = !isDeed;
        TextDeedTitle.enabled = isDeed;

        if (isDeed)
        {
            TextHowTo.text = string.Format(currentDeed_.Req, currentDeed_.KillReq);
            TextDeedTitle.text = currentDeed_.Title;
        }
        else
        {
            TextHowTo.text = "Save The Baby Orcs";
        }

        TextControls.enabled = true;
        TextHowTo.enabled = true;
        TextDeedScore.enabled = false;

        while (SaveGame.RoundScore == 0)
        {
            yield return null;
        }

        TextDeedScore.enabled = isDeed;
        TextControls.enabled = false;
        TextHowTo.enabled = false;
        TextDeedTitle.enabled = false;

        if (currentDeed_.EnabledSpawns.Count == 0)
        {
            StartCoroutine(Walkers());
            StartCoroutine(Chargers());
            StartCoroutine(BigWalkers());
            StartCoroutine(CasterSkellies());
        }
        else
        {
            foreach (var spawns in currentDeed_.EnabledSpawns)
            {
                switch (spawns)
                {
                    case EnabledSpawns.WalkerDefault:
                        StartCoroutine(Walkers());
                        break;
                    case EnabledSpawns.BigWalkerDefault:
                        StartCoroutine(BigWalkers());
                        break;
                    case EnabledSpawns.CasterDefault:
                        StartCoroutine(CasterSkellies());
                        break;
                    case EnabledSpawns.DeedSniper:
                        StartCoroutine(DeedSniper());
                        break;
                    case EnabledSpawns.DeedMachinegun:
                        StartCoroutine(DeedMachinegun());
                        break;
                    case EnabledSpawns.DeedWhiteWalkers:
                        StartCoroutine(DeedWhiteWalkers());
                        break;
                }
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator DeedWhiteWalkers()
    {
        while (true)
        {
            int amount = 5 + Random.Range(0, 2);
            amount += currentDeed_.DeedCurrentScore / 5;

            PositionUtility.SpawnDirection dir = PositionUtility.SpawnDirection.TopOrBottom;
            yield return PositionUtility.SpawnGroup(BigSkellieWalkerPrefab, amount, 0.1f, true, dir);

            float delay = 5.0f;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator Walkers()
    {
        while (true)
        {
            int amount = 3 + (SaveGame.RoundScore / 8);
            if (Random.value < 0.1f)
                amount += 2;

            bool inside = Random.value < 0.5f || SaveGame.RoundScore == 1;
            PositionUtility.SpawnDirection dir = (PositionUtility.SpawnDirection)Random.Range(0, (int)PositionUtility.SpawnDirection.Last);

            yield return PositionUtility.SpawnGroup(SkellieWalkerPrefab, amount, 0.1f, !inside, dir);
            float delay = 4 + Random.value;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator Chargers()
    {
        while (true)
        {
            while (SaveGame.RoundScore < 5)
                yield return null;

            const float WarnDelay = 1.0f;

            // Warn player
            GameManager.Instance.TextGameWarning.enabled = true;
            AudioManager.Instance.PlayClip(AudioManager.Instance.MiscAudioSource, AudioManager.Instance.AudioData.ChargerWarning);
            yield return new WaitForSeconds(WarnDelay);
            GameManager.Instance.TextGameWarning.enabled = false;

            int amount = 4 + (SaveGame.RoundScore / 15);
            if (Random.value < 0.1f)
                amount += 1;

            PositionUtility.SpawnDirection dir = (PositionUtility.SpawnDirection)Random.Range(0, (int)PositionUtility.SpawnDirection.Last);

            yield return PositionUtility.SpawnGroup(SkellieChargerPrefab, amount, 0.1f, true, dir);

            // Wait semi-random after spawning
            float delay = 10 + Random.value;
            yield return new WaitForSeconds(delay);
        }
    }

    //IEnumerator Chargers()
    //{
    //    while (true)
    //    {
    //        //while (SaveGame.RoundScore < 10)
    //        //    yield return null;

    //        int amount = 1 + (SaveGame.RoundScore / 8);
    //        if (Random.value < 0.1f)
    //            amount += 1;

    //        bool inside = Random.value < 0.5f || SaveGame.RoundScore == 1;
    //        PositionUtility.SpawnDirection dir = (PositionUtility.SpawnDirection)Random.Range(0, (int)PositionUtility.SpawnDirection.Last);

    //        yield return PositionUtility.SpawnGroup(SkellieChargerPrefab, amount, 0.1f, !inside, dir);
    //        float delay = 4 + Random.value;
    //        yield return new WaitForSeconds(delay);
    //    }
    //}

    IEnumerator BigWalkers()
    {
        while (true)
        {
            int amount = 1 + Random.Range(0, 2);
            amount += SaveGame.RoundScore / 10;

            float delay = SaveGame.RoundScore == 1 ? 5 + Random.value * 3 : 20 + Random.value + amount;
            yield return new WaitForSeconds(delay);

            bool inside = Random.value < 0.2f;
            PositionUtility.SpawnDirection dir = (PositionUtility.SpawnDirection)Random.Range(0, (int)PositionUtility.SpawnDirection.Last);
            yield return PositionUtility.SpawnGroup(BigSkellieWalkerPrefab, amount, 0.1f, !inside, dir);
        }
    }

    IEnumerator CasterSkellies()
    {
        while (true)
        {
            int amount = 1 + SaveGame.RoundScore / 15;

            if (amount > 3)
                amount = 3;

            if (Random.value < 0.1f)
                amount++;

            float delay = SaveGame.RoundScore == 1 ? 15 + Random.value * 3 : 20 + Random.value * 5;
            yield return new WaitForSeconds(delay);

            bool inside = Random.value < 0.05f;
            PositionUtility.SpawnDirection dir = (PositionUtility.SpawnDirection)Random.Range(0, (int)PositionUtility.SpawnDirection.Last);
            yield return PositionUtility.SpawnGroup(SkellieCasterPrefab, amount, 0.1f, !inside, dir);
        }
    }

    IEnumerator DeedSniper()
    {
        const float InitialDelay = 3.0f;
        yield return new WaitForSeconds(InitialDelay);

        while (true)
        {
            int amount = 5 + currentDeed_.DeedCurrentScore / 10;

            if (Random.value < 0.1f)
                amount++;

            bool inside = Random.value < 0.05f;
            PositionUtility.SpawnDirection dir = Random.value < 0.5f ? PositionUtility.SpawnDirection.LeftOrRight : PositionUtility.SpawnDirection.TopOrBottom;
            yield return PositionUtility.SpawnGroup(SkellieCasterPrefab, amount, 0.1f, !inside, dir);

            float delay = 5 + Random.value * 2;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator DeedMachinegun()
    {
        while (true)
        {
            int amount = 5 + (currentDeed_.DeedCurrentScore / 5);
            if (Random.value < 0.1f)
                amount += 2;

            bool inside = Random.value < 0.5f || SaveGame.RoundScore == 1;
            PositionUtility.SpawnDirection dir = (PositionUtility.SpawnDirection)Random.Range(0, (int)PositionUtility.SpawnDirection.Last);

            yield return PositionUtility.SpawnGroup(SkellieWalkerPrefab, amount, 0.1f, !inside, dir);
            float delay = 4 + Random.value;
            yield return new WaitForSeconds(delay);
        }
    }

    void Update()
    {
        if (!isRunning_)
            return;
    }
}
