using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class ProgressPopupScript : MonoBehaviour
{
    public TextMeshProUGUI ProgressText;

    // The main passive-income shop upgrades (each also has its own X2 tier, roughly every 25
    // levels past the first few). A tier counts as "bought" once at least one level of it has
    // been purchased. Add a new line here whenever a new upgrade type is added to the shop.
    static readonly Func<long>[] UpgradeTierLevels =
    {
        () => SaveGame.Members.LevelClickDamage,
        () => SaveGame.Members.LevelKnifeDamage,
        () => SaveGame.Members.LevelMoneyPerGold,
        () => SaveGame.Members.LevelKnifeCd,
        () => SaveGame.Members.LevelWitchDoctor,
        () => SaveGame.Members.LevelGoldPerKnifeThrown,
        () => SaveGame.Members.LevelWizard,
        () => SaveGame.Members.LevelHoarder,
        () => SaveGame.Members.LevelZapDamage,
        () => SaveGame.Members.LevelMoneyMaker,
        () => SaveGame.Members.LevelDaggerMaster,
        () => SaveGame.Members.LevelNecroNinja,
        () => SaveGame.Members.LevelSkullCrusher,
        () => SaveGame.Members.LevelChestMaster,
        () => SaveGame.Members.LevelVoidgazer,
        () => SaveGame.Members.LevelSmartDaggers,
        () => SaveGame.Members.LevelFastFeet,
        () => SaveGame.Members.LevelCryptMaster,
        () => SaveGame.Members.LevelSmartFireballs,
        () => SaveGame.Members.LevelBeefyEarl,
    };

    public void Show()
    {
        this.gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }

    void Update()
    {
        (int enemiesUnlocked, int enemiesTotal) = EnemySpawner.GetTierUnlockProgress();
        (int skinsUnlocked, int skinsTotal) = SkinScript.GetUnlockProgress();
        int tiersBought = UpgradeTierLevels.Count(getLevel => getLevel() > 0);
        int tiersTotal = UpgradeTierLevels.Length;

        double enemyPct = enemiesTotal > 0 ? (double)enemiesUnlocked / enemiesTotal * 100 : 0;
        double skinPct = skinsTotal > 0 ? (double)skinsUnlocked / skinsTotal * 100 : 0;
        double tierPct = tiersTotal > 0 ? (double)tiersBought / tiersTotal * 100 : 0;
        double totalPct = (enemyPct + skinPct + tierPct) / 3.0;

        ProgressText.text =
            $"Enemies unlocked: <color=#8DBE4C>{enemyPct:0}%</color> ({enemiesUnlocked}/{enemiesTotal})\r\n" +
            $"Skins unlocked: <color=#8DBE4C>{skinPct:0}%</color> ({skinsUnlocked}/{skinsTotal})\r\n" +
            $"Upgrade tiers bought: <color=#8DBE4C>{tierPct:0}%</color> ({tiersBought}/{tiersTotal})\r\n" +
            $"\r\nTotal completion: <color=#8DBE4C>{totalPct:0}%</color>";
    }
}
