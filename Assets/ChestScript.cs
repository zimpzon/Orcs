using UnityEngine;

public class ChestScript : MonoBehaviour
{
    public bool IsLarge = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int money = 25;
        int xpValue = 15;

        if (IsLarge)
        {
            money = 40;
            xpValue = 25;
        }

        GameManager.Instance.MakePoof(transform.position, 4, 2.5f);
        GameManager.Instance.MakeFlash(transform.position, 5);
        GameManager.Instance.ThrowPickups(AutoPickUpType.Money, transform.position,money, 1, 2.0f);
        GameManager.Instance.ThrowPickups(AutoPickUpType.Xp, transform.position, 20, xpValue, forceScale: 8.0f);

        for (int i = 0; i < 10; ++i)
            SawBladeController.I.Throw();

        GameObject.Destroy(this.gameObject);
    }
}
