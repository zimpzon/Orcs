using Assets.Script.Misc;
using System.Collections;
using UnityEngine;

public static class Chapter1BossUtil
{
    public static IEnumerator MoveBoss(Transform boss, Vector2 dst, float speed)
    {
        bool bossIsAtPosition = false;

        float distance = Vector2.Distance(boss.transform.position, dst);
        float time = distance / speed;

        LeanTween.moveLocal(boss.gameObject, dst, time).setOnComplete(() => bossIsAtPosition = true);

        while (!bossIsAtPosition)
            yield return null;
    }

    public static IEnumerator FireballSpiral(Transform boss)
    {
        var delay = new WaitForSeconds(0.1f);

        float angle = 0;
        while (true)
        {
            Vector2 direction = MathUtil.DegreeToVector2(angle);
            angle += G.D.GameDeltaTime * 1500;

            direction.Normalize();

            Vector3 muzzlePoint = direction * 0.6f;

            ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
            basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
            basic.Type = ProjectileManager.ProjectileType.HarmsPlayer;

            basic.Speed = 5.0f;
            basic.Damage = 50.0f;
            basic.MaxDistance = 20.0f;
            basic.Radius = 0.3f;
            Vector3 scale = basic.SpriteInfo.Transform.localScale;
            scale.x = 1.5f;
            scale.y = 1.5f;
            scale.z = 1.0f;

            basic.Position = boss.position + muzzlePoint;
            basic.SpriteInfo.Renderer.sprite = SpriteData.Instance.ShamanProjectile;
            basic.SpriteInfo.Renderer.sortingLayerID = GameManager.Instance.SortLayerTopEffects;
            basic.Direction = direction;
            basic.Color = new Color(1.0f, 1.0f, 1.0f);
            basic.DieTime = 0.0f;
            basic.SpriteInfo.Transform.localScale = scale;
            float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            basic.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

            ProjectileManager.Instance.Fire(basic);
            AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.AudioData.EnemyShoot, 0.5f);

            yield return delay;
        }
    }
}
