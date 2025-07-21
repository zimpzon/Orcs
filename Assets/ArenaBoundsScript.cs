using Assets.Script;
using Assets.Script.Misc;
using UnityEngine;

public class ArenaBoundsScript : MonoBehaviour
{
    public LineRenderer LineRenderer;

    float _lineRendererBaseWidth;
    float _nextZapReadyAt;

    private void Awake()
    {
        _lineRendererBaseWidth = LineRenderer.startWidth;
        LineRenderer.startWidth = 0;
        LineRenderer.endWidth = 0;
    }

    private void OnMouseDown()
    {
        if (G.D.GameTime < _nextZapReadyAt)
            return;

        _nextZapReadyAt = G.D.GameTime + 0.1f;

        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3 target;
        var closestEnemy = BlackboardScript.GetClosestEnemy(mouseWorldPos, radius: 20);
        if (closestEnemy is null)
        {
            target = mouseWorldPos + (Vector3)Random.insideUnitCircle * 5;
        }
        else
        {
            target = closestEnemy.transform.position;
        }

        var direction = target - mouseWorldPos;
        float distance = direction.magnitude;
        direction.Normalize();

        Trails.DrawJaggedTrail(mouseWorldPos, target, LineRenderer);

        LeanTween.cancel(LineRenderer.gameObject);
        LeanTween.value(LineRenderer.gameObject, _lineRendererBaseWidth, 0.0f, time: 0.25f)
            .setOnUpdate((float val) =>
            {
                LineRenderer.startWidth = val;
                LineRenderer.endWidth = val;
            });

        long damage = PlayerUpgrades.Data.ClickDamage;

        GameManager.Instance.MakePoof(mouseWorldPos, 2, 1.0f);
        GameManager.Instance.MakeCircle(mouseWorldPos, 1);
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerStaffHit, volumeScale: 1.0f, pitch: 1.0f);

        if (closestEnemy is null)
            return;

        SaveGame.Members.CountDamageClicks++;
        GameManager.Instance.DamageEnemy(closestEnemy, damage, direction, 1.0f);

        FloatingTextSpawner.Instance.Spawn(
            closestEnemy.transform.position + Vector3.up * 0.25f,
            $"-{damage}",
            Color.red,
            speed: 0.75f,
            timeToLive: 1.0f,
            fontStyle: TMPro.FontStyles.Bold);
    }

    void Update()
    {
    }
}
