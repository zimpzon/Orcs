using Assets.Script;
using System.Collections;
using UnityEngine;

public class ActorOgreShamanShoot : MonoBehaviour
{
    PlayerScript _player;
    float cd;

    void Awake()
    {
        _player = GetComponent<PlayerScript>();
        StartCoroutine(Think());
    }

    void Shoot()
    {
        bool hasCloseTarget = ActorBase.PlayerClosestEnemy is not null;

        Vector2 direction = hasCloseTarget ?
            ActorBase.PlayerClosestEnemy.transform.position - G.D.PlayerPos :
            (Vector2)G.D.PlayerPos + Random.insideUnitCircle.normalized;

        direction.Normalize();
        direction = RndUtil.RandomSpread(direction, 5);

        Vector3 muzzlePoint = direction * 0.6f;
        _player.AddForce(-direction * 0.1f);

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;
        basic.SwayFactor = 0.05f;
        basic.Speed = 7.0f;
        basic.Damage = PlayerUpgrades.Data.WizardEffectiveDamage;
        basic.MaxDistance = 20.0f;
        basic.ReflectOnEdges = true;
        basic.Radius = 0.3f;
        Vector3 scale = basic.SpriteInfo.Transform.localScale;
        scale.x = 1.0f;
        scale.y = 0.75f;
        scale.z = 1.0f;
        scale *= 2.0f;

        basic.DieOnCollision = false;
        basic.Position = (Vector2)_player.transform.position + direction * 0.5f;
        basic.SpriteInfo.Renderer.sprite = SpriteData.Instance.ShamanProjectile;
        basic.SpriteInfo.Renderer.sortingLayerID = GameManager.Instance.SortLayerTopEffects;
        basic.Direction = direction;
        basic.Color = new Color(1.0f, 1.0f, 1.01f);
        basic.DieTime = 0.0f;
        basic.SpriteInfo.Transform.localScale = scale;
        basic.ParticleSystem = Particles.I.FireballTail;
        basic.ParticleEmitCount = 5;
        basic.ParticleEmitDelay = 0.05f;
        float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        basic.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
        basic.OnEndOfLife = OnProjectileEndOfLife;

        ProjectileManager.Instance.Fire(basic);
        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.AudioData.EnemyShoot, 0.5f);
    }

    void OnProjectileEndOfLife(ProjectileManager.Basic p, bool hitEnemy)
    {
        var pos = p.Position;
        Particles.I.ExplosionSpriteSheet.transform.position = pos;
        Particles.I.ExplosionSpriteSheet.Emit(1);
        Particles.I.ExplosionSpread.transform.position = pos;
        Particles.I.ExplosionSpread.Emit(40);
        GameManager.Instance.MakeCircle(pos, 2.0f);
        GameManager.Instance.MakeFlash(pos, 1.0f);
    }

    IEnumerator Think()
    {
        float nextShoot = 0;

        while (true)
        {
            if (PlayerUpgrades.Data is null || !PlayerUpgrades.Data.WizardEnabled)
            {
                yield return null;
                continue;
            }

            bool isActiveArena = GameManager.Instance.GameState == GameManager.State.Idle_Fighting;
            if (isActiveArena && GameManager.Instance.GameTime > nextShoot)
            {
                int projectileCount = 2;

                for (int i = 0; i < projectileCount; ++i)
                {
                    isActiveArena = GameManager.Instance.GameState == GameManager.State.Idle_Fighting;
                    if (!isActiveArena)
                        break;

                    Shoot();
                    yield return new WaitForSeconds(0.25f);
                }

                nextShoot = GameManager.Instance.GameTime + cd;
                cd = 2 + Random.value * 0.75f;
            }

            yield return null;
        }
    }
}
