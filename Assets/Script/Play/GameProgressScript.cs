using Assets.Script;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum EnabledSpawns { WalkerDefault, BigWalkerDefault, CasterDefault };

public class GameProgressScript : MonoBehaviour
{
    public static GameProgressScript Instance;

    public Text TextControls;
    public Text TextHowTo;
    public Text TextScore;

    bool isRunning_;

    public void Begin()
    {
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

        StartCoroutine(Walkers());
        StartCoroutine(Chargers());
        StartCoroutine(BigWalkers());
        StartCoroutine(CasterSkellies());
    }

    private void Awake()
    {
        Instance = this;
    }

    public void EnemyExplosion(ActorTypeEnum type, Vector3 pos, int count, float force)
    {
        for (int i = 0; i < count; ++i)
        {
            var spawn = EnemyManager.Instance.GetEnemyFromCache(type);
            spawn.transform.position = pos;
            var actor = spawn.GetComponent<ActorBase>();
            actor.AddForce(RndUtil.RandomInsideUnitCircle().normalized * force);
            spawn.SetActive(true);
        }
    }

    IEnumerator Walkers()
    {
        while (true)
        {
            int amount = 5 + (SaveGame.RoundScore / 5);
            if (Random.value < 0.1f)
                amount += 2;

            bool inside = false;// Random.value < 0.5f || SaveGame.RoundScore == 1;
            PositionUtility.SpawnDirection dir = PositionUtility.GetRandomDirOutside();

            yield return PositionUtility.SpawnGroup(ActorTypeEnum.SmallWalker, amount, 0.1f, !inside, dir);
            float delay = 1 + Random.value;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator Chargers()
    {
        while (true)
        {
            while (SaveGame.RoundScore < 5)
                yield return null;

            const float WarnDelay = 0.5f;

            // Warn player
            AudioManager.Instance.PlayClip(AudioManager.Instance.MiscAudioSource, AudioManager.Instance.AudioData.ChargerWarning);
            yield return new WaitForSeconds(WarnDelay);

            int amount = 1 + (SaveGame.RoundScore / 15);
            if (Random.value < 0.1f)
                amount += 1;

            PositionUtility.SpawnDirection dir = PositionUtility.GetRandomDirOutside();

            yield return PositionUtility.SpawnGroup(ActorTypeEnum.SmallCharger, amount, 0.1f, true, dir);

            // Wait semi-random after spawning
            float delay = 12 + Random.value * 2;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator BigWalkers()
    {
        while (true)
        {
            int amount = 1 + Random.Range(0, 2);
            amount += SaveGame.RoundScore / 15;

            float delay = SaveGame.RoundScore == 1 ? 5 + Random.value * 3 : 20 + Random.value + amount;
            yield return new WaitForSeconds(delay);

            bool inside = Random.value < 0.2f;
            PositionUtility.SpawnDirection dir = PositionUtility.GetRandomDirOutside();
            yield return PositionUtility.SpawnGroup(ActorTypeEnum.LargeWalker, amount, 0.1f, !inside, dir);
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
            PositionUtility.SpawnDirection dir = PositionUtility.GetRandomDirOutside();
            yield return PositionUtility.SpawnGroup(ActorTypeEnum.Caster, amount, 0.1f, !inside, dir);
        }
    }

    void Update()
    {
        if (!isRunning_)
            return;
    }
}
