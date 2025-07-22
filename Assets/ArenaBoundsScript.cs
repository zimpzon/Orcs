using Assets.Script;
using Assets.Script.Misc;
using UnityEngine;

public class ArenaBoundsScript : MonoBehaviour
{
    public static ArenaBoundsScript Instance;

    public LineRenderer LineRenderer;
    public float LineRendererBaseWidth;

    float _nextZapReadyAt;

    private void Awake()
    {
        Instance = this;

        LineRendererBaseWidth = LineRenderer.startWidth;
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

        var closestEnemy = BlackboardScript.GetClosestEnemy(mouseWorldPos, radius: 20);
        long damage = PlayerUpgrades.Data.ClickDamage;
        Zapper.TryZapEnemy(mouseWorldPos, closestEnemy, damage);

        GameManager.Instance.MakePoof(mouseWorldPos, 2, 1.0f);
        GameManager.Instance.MakeCircle(mouseWorldPos, 1);
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerStaffHit, volumeScale: 1.0f, pitch: 1.0f);

        SaveGame.Members.CountDamageClicks++;
    }

    void Update()
    {
    }
}
