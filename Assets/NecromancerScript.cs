using System.Collections;
using UnityEngine;

public class NecromancerScript : MonoBehaviour
{
    PlayerScript _player;
    int _currentShotIndex = 0;

    void Awake()
    {
        _player = GetComponent<PlayerScript>();
    }

    void Shoot()
    {
        var saw = WeaponBase.GetWeapon(WeaponType.Sawblade);

        // Calculate direction based on shot index to avoid middle zone
        Vector2 direction = GetSkullDirection(_currentShotIndex);
        _currentShotIndex++;

        saw.Eject(_player.transform.position, direction, Color.white);
    }

    Vector2 GetSkullDirection(int shotIndex)
    {
        // Two zones: 315°-350° (forward-down) and 10°-45° (forward-up)
        // Avoid the middle zone around 0° (straight forward)

        float angleInDegrees;

        if (shotIndex % 2 == 0)
        {
            // Even shots: forward-down zone (315°-350°)
            angleInDegrees = Random.Range(315f, 350f);
        }
        else
        {
            // Odd shots: forward-up zone (10°-45°)
            angleInDegrees = Random.Range(10f, 45f);
        }

        // Convert to radians and create direction vector
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
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

            // Reset shot index for new volley
            _currentShotIndex = 0;

            while (shotsLeft > 0)
            {
                yield return null;

                bool isActiveArena = GameManager.Instance.GameState == GameManager.State.Idle_Fighting;
                if (isActiveArena)
                {
                    Shoot();
                    shotsLeft--;
                    yield return new WaitForSeconds(0.2f);
                }
            }

            // Wait for next round
            while (true)
                yield return null;
        }
    }
}
