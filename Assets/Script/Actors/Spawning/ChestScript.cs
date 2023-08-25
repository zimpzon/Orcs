using UnityEngine;

public class ChestScript : MonoBehaviour, IKillableObject
{
    public bool IsLarge = false;

    public void Kill()
    {
        GameObject.Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int money = 25;
        int xpValue = 15;

        if (IsLarge)
        {
            money = 40;
            xpValue = 25;
        }

        GameManager.Instance.MakePoof(transform.position, 4, 1.5f);
        GameManager.Instance.MakeFlash(transform.position, 2);
        GameManager.Instance.ThrowPickups(AutoPickUpType.Money, transform.position,money, 1, 2.0f);
        GameManager.Instance.ThrowPickups(AutoPickUpType.Xp, transform.position, 20, xpValue, forceScale: 8.0f);

        for (int i = 0; i < 5; ++i)
            SawBladeController.I.Throw();

        FloatingTextSpawner.Instance.Spawn(transform.position + Vector3.up * 2, "5 SAWS RELEASED", Color.yellow, speed: 0.5f, timeToLive: 3.5f, TMPro.FontStyles.Bold);
        GameObject.Destroy(this.gameObject);
    }
}
