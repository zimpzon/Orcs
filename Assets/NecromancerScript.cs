using System.Collections;
using UnityEngine;

public class NecromancerScript : MonoBehaviour
{
    PlayerScript _player;

    void Awake()
    {
        _player = GetComponent<PlayerScript>();
    }

    void Shoot()
    {
        var saw = WeaponBase.GetWeapon(WeaponType.Sawblade);
        var randomDir = Random.insideUnitCircle.normalized;
        saw.Eject(_player.transform.position, randomDir, Color.white);
    }

    public IEnumerator Think()
    {
        int shotsLeft = SaveGame.Members.LevelCryptMaster > 0 ? 4 : 3;

        while (true)
        {
            if (PlayerUpgrades.Data is null || !PlayerUpgrades.Data.NecromancerEnabled)
            {
                yield return null;
                continue;
            }

            while (shotsLeft > 0)
            {
                yield return null;

                bool isActiveArena = GameManager.Instance.GameState == GameManager.State.Idle_Fighting;
                if (isActiveArena)
                {
                    Shoot();
                    shotsLeft--;
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // Wait for next round
            while (true)
                yield return null;
        }
    }
}
