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
        direction.Normalize();

        long damage = PlayerUpgrades.Data.ClickPower;

        GameManager.Instance.DamageEnemy(closestEnemy, damage, direction, 1.0f);

        FloatingTextSpawner.Instance.Spawn(
            closestEnemy.transform.position + Vector3.up * 0.25f,
            $"-{damage}",
            Color.red,
            speed: 0.75f,
            timeToLive: 2.0f,
            fontStyle: TMPro.FontStyles.Bold);

        GameManager.Instance.MakePoof(mouseWorldPos, 2, 1.0f);
        GameManager.Instance.MakeCircle(mouseWorldPos, 1);

        float trail = distance;
        while (trail > 0)
        {
            Particles.I.ClickTrail.transform.position = mouseWorldPos + direction * trail;
            Particles.I.ClickTrail.Emit(1);
            trail -= 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
