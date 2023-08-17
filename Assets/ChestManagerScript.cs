using System;
using UnityEngine;

public class ChestManagerScript : MonoBehaviour, IKillableObject
{
    public Transform ChestProto;
    bool hasSpawned_ = false;
    float spawnTime_ = 60 * 5;

    public void Kill()
    {
        hasSpawned_ = false;
    }

    void Update()
    {
        if (!PlayerUpgrades.Data.SpawnChestUnlocked || hasSpawned_)
            return;

        if (G.D.GameTime > spawnTime_)
        {
            bool isLarge = UnityEngine.Random.value > 0.5f;

            var chestPos = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.value * 5;
            var chest = GameObject.Instantiate(ChestProto, (Vector3)chestPos, Quaternion.identity);
            chest.GetComponent<ChestScript>().IsLarge = isLarge;
    
            if (!isLarge)
                chest.transform.localScale *= 0.5f;

            GameManager.Instance.MakePoof(chest.transform.position, 4, 1.5f);

            var info = new GameInfo
            {
                Text = $"a {(isLarge ? "large" : "small")} treasure chest has arrived",
                Color = G.D.UpgradePositiveColor,
                Duration = 2.5f,
                FadeInDuration = 0.5f,
                FadeOutDuration = 1.0f,
                FontSize = 8,
                Position = Vector2.up * -100,
            };

            GameManager.Instance.TextGameInfo.GetComponent<GameInfoViewer>().Show(info);

            hasSpawned_ = true;
        }
    }
}
