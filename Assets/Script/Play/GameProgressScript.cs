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
    public Text TextSandboxCreator;
    public Text TextSandboxWeapons;

    public GameObject SkellieWalkerPrefab;
    public GameObject SkellieChargerPrefab;
    public GameObject BigSkellieWalkerPrefab;
    public GameObject SkellieCasterPrefab;

    bool isRunning_;
    DeedData currentDeed_;
    SandboxData currentSandbox_;

    public void Begin()
    {
        currentDeed_ = GameManager.Instance.CurrentDeedData;
        currentSandbox_ = GameManager.Instance.CurrentSandboxData;
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

        TextSandboxCreator.enabled = false;
        TextSandboxWeapons.enabled = false;

        if (isDeed)
        {
            TextHowTo.text = string.Format(currentDeed_.Req, currentDeed_.UpdatedKillReq);
            TextDeedTitle.text = currentDeed_.Title;
            if (currentDeed_.Deed == DeedEnum.Sandbox)
            {
                TextSandboxCreator.text = string.Format("By {0}", currentSandbox_.creator_name);
                TextSandboxCreator.enabled = true;
                bool hasRightClickWeapon = (WeaponType)currentSandbox_.weapon_right_click != WeaponType.None;
                if (hasRightClickWeapon)
                {
                    TextSandboxWeapons.text = string.Format("Left Mouse: {0}\nRight Mouse: {1}",
                        WeaponBase.WeaponDisplayName((WeaponType)currentSandbox_.weapon_left_click),
                        WeaponBase.WeaponDisplayName((WeaponType)currentSandbox_.weapon_right_click));
                    TextSandboxWeapons.enabled = true;
                }
            }
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
        TextSandboxCreator.enabled = false;
        TextSandboxWeapons.enabled = false;

        if (currentDeed_.Deed == DeedEnum.Sandbox)
        {
            foreach(var w in currentSandbox_.Waves)
            {
                StartCoroutine(SandboxWave(w));
            }
        }
        else if (currentDeed_.EnabledSpawns.Count == 0)
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

    IEnumerator SandboxWave(Wave wave)
    {
        yield return new WaitForSeconds(wave.start_time);
        GameObject enemyPrefab;
        switch ((ActorTypeEnum)wave.enemy_type)
        {
            case ActorTypeEnum.SmallWalker: enemyPrefab = SkellieWalkerPrefab; break;
            case ActorTypeEnum.LargeWalker: enemyPrefab = BigSkellieWalkerPrefab; break;
            case ActorTypeEnum.Caster: enemyPrefab = SkellieCasterPrefab; break;
            case ActorTypeEnum.SmallCharger: enemyPrefab = SkellieChargerPrefab; break;
            default: enemyPrefab = SkellieWalkerPrefab; break;
        }
        yield return PositionUtility.SpawnGroup(enemyPrefab, wave.count, wave.interval, wave.where != PositionUtility.SpawnDirection.Inside, wave.where);
    }

    private void Awake()
    {
        Instance = this;
    }

    public void EnemyExplosion(GameObject prefab, Vector3 pos, int count, float force)
    {
        if (currentDeed_.Deed == DeedEnum.Sandbox)
        {
            // Not pretty... but we have to add these newly spawned enemies to the total count of the sandbox
            currentDeed_.UpdatedKillReq += count;
            TextHowTo.text = string.Format(currentDeed_.Req, currentDeed_.UpdatedKillReq);
        }

        for (int i = 0; i < count; ++i)
        {
            var spawn = GameObject.Instantiate<GameObject>(prefab);
            spawn.transform.position = pos;
            var actor = spawn.GetComponent<ActorBase>();
            actor.AddForce(Random.insideUnitCircle.normalized * force);
        }
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
            int amount = 5 + (SaveGame.RoundScore / 5);
            if (Random.value < 0.1f)
                amount += 2;

            bool inside = Random.value < 0.5f || SaveGame.RoundScore == 1;
            PositionUtility.SpawnDirection dir = PositionUtility.GetRandomDirOutside();

            yield return PositionUtility.SpawnGroup(SkellieWalkerPrefab, amount, 0.1f, !inside, dir);
            float delay = 6 + Random.value;
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

            yield return PositionUtility.SpawnGroup(SkellieChargerPrefab, amount, 0.1f, true, dir);

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
            PositionUtility.SpawnDirection dir = PositionUtility.GetRandomDirOutside();
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
            PositionUtility.SpawnDirection dir = PositionUtility.GetRandomDirOutside();

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
