using System;
using UnityEngine;

public class ChestManagerScript : MonoBehaviour, IPlayerToggleEfffect
{
    public Transform ChestProto;

    TimeSpan nextCheck_;
    bool enabled_;

    public void Disable()
    {
        enabled_ = false;
    }

    public void TryEnable()
    {
        if (!PlayerUpgrades.Data.SpawnChestUnlocked)
            return;

        enabled_ = true;
        SetNextCheck();
    }

    void SetNextCheck()
    {
        var now = TimeSpan.FromSeconds(G.D.GameTime);
        nextCheck_ = now.Add(TimeSpan.FromSeconds(10));
        //nextCheck_ = new TimeSpan(0, now.Minutes + 1, 2);
    }

    void Update()
    {
        if (!enabled_)
            return;

        if (G.D.GameTime > nextCheck_.TotalSeconds)
        {
            bool isLarge = UnityEngine.Random.value > 0.5f;

            var chestPos = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.value * 5;
            var chest = GameObject.Instantiate(ChestProto, (Vector3)chestPos, Quaternion.identity);
            chest.GetComponent<ChestScript>().IsLarge = isLarge;
    
            if (!isLarge)
                chest.transform.localScale *= 0.5f;

            GameManager.Instance.MakePoof(transform.position, 4, 2.5f);
            GameManager.Instance.MakeFlash(transform.position, 5);

            var info = new GameInfo
            {
                Text = $"a {(isLarge ? "large" : "small")} treasure chest has arrived",
                Color = G.D.UpgradePositiveColor,
                Duration = 2.5f,
                FadeInDuration = 0.5f,
                FadeOutDuration = 1.0f,
                FontSize = 10,
                Position = Vector2.up * -100,
            };

            GameManager.Instance.TextGameInfo.GetComponent<GameInfoViewer>().Show(info);

            SetNextCheck();
        }
    }
}
