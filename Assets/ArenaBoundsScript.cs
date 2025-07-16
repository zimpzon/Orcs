using Assets.Script;
using UnityEngine;

public class ArenaBoundsScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        var closestEnemy = BlackboardScript.GetClosestEnemy(mouseWorldPos, radius: 20);
        if (closestEnemy is null)
            return;

        var direction = closestEnemy.transform.position - mouseWorldPos;
        float distance = direction.magnitude;

        long damage = PlayerUpgrades.Data.ClickPower;

        GameManager.Instance.DamageEnemy(closestEnemy, damage, direction.normalized, 1.0f);
        FloatingTextSpawner.Instance.Spawn(
            closestEnemy.transform.position + Vector3.up * 0.25f,
            $"-{damage}",
            Color.red,
            speed: 0.75f,
            timeToLive: 2.0f,
            fontStyle: TMPro.FontStyles.Bold);

        Debug.DrawLine(mouseWorldPos, closestEnemy.transform.position, Color.red, 10);
        GameManager.Instance.MakeFlash(mouseWorldPos, 0.5f);
        GameManager.Instance.MakePoof(mouseWorldPos, 3, 1.0f);

        GameManager.Instance.MakeFlash(closestEnemy.transform.position, 0.5f);
        GameManager.Instance.MakePoof(closestEnemy.transform.position, 3, size: 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
