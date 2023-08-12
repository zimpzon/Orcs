using Assets.Script;
using System.Collections;
using UnityEngine;

public class ActorOgreShamanBossShoot : MonoBehaviour
{
    ActorBase actorBase_;
    float cd;

    void Awake()
    {
        actorBase_ = GetComponent<ActorBase>();
    }

    void OnEnable()
    {
        StartCoroutine(Think());
    }

    void Shoot()
    {
        if (actorBase_.Hp <= 0.0f)
            return;

        Vector2 direction = G.D.PlayerPos - actorBase_.transform.position;
        direction.Normalize();
        direction = RndUtil.RandomSpread(direction, 1.0f);

        Vector3 muzzlePoint = direction * 0.6f;
        actorBase_.AddForce(-direction * 0.1f);

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

        basic.Position = actorBase_.transform.position + muzzlePoint;
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
    }

    IEnumerator Think()
    {
        while (!actorBase_.IsFullyReady)
            yield return null;

        float nextShoot = GameManager.Instance.GameTime;

        while (!actorBase_.IsDead)
        {
            if (actorBase_.IsFullyReady && GameManager.Instance.GameTime > nextShoot)
            {
                int projectileCount = 2;

                for (int i = 0; i < projectileCount; ++i)
                {
                    if (actorBase_.IsDead)
                        break;

                    Shoot();
                    GameManager.Instance.MakeFlash(actorBase_.transform.position, 1.0f);
                    //yield return new WaitForSeconds(0.1f);
                }

                nextShoot = GameManager.Instance.GameTime + cd;
                cd = 2;
            }

            yield return null;
        }
    }
}
