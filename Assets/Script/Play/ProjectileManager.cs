using Assets.Script;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour, IObjectFactory<ProjectileManager.Basic>
{
    public enum ProjectileType { HarmsPlayer, HarmsEnemies, HarmsNothing };

    public class Basic
    {
        public static float CalcDamage(Basic b)
        {
            float damage = b.Damage;
            if (b.DamageFalloffDistance != 0.0f && b.DistanceTraveled > b.DamageFalloffDistance)
            {
                float metersPastFalloff = b.DistanceTraveled - b.DamageFalloffDistance;
                float falloffFactor = Mathf.Max(0.0f, (1.0f - (b.DamageFalloffPerMeter * metersPastFalloff)));
                const float MinimumDamage = 0.25f;
                damage = MinimumDamage * damage + ((1.0f - MinimumDamage) * falloffFactor * damage);
            }
            return damage;
        }

        public void OnHit()
        {
            IsLastFrame = true;
        }

        public void Reset()
        {
            SpriteInfo.Renderer = null;
            SpriteInfo.Transform = null;
            Position = Vector3.zero;
            Color = Color.white;
            Speed = 0.0f;
            SwayFactor = 0.0f;
            Radius = 0.0f;
            Blink = false;
            Force = 0.25f;
            Direction = Vector3.zero;
            MaxDistance = 0.0f;
            DistanceTraveled = 0.0f;
            DamageFalloffDistance = 0.0f;
            DamageFalloffPerMeter = 0.0f;
            DieTime = 0;
            Type = ProjectileType.HarmsEnemies;
            IsLastFrame = false;
            IsFirstFrame = true;
            CustomCounter = 0;
            CustomCollisionResponse = null;
            DieOnCollision = true;
        }

        public ProjectileInfo SpriteInfo;
        public Vector3 Position;
        public Color Color;
        public float Speed;
        public float SwayFactor;
        public float Radius;
        public float Damage;
        public float Force;
        public bool Blink;
        public Vector3 Direction;
        public float MaxDistance;
        public float DistanceTraveled;
        public float DamageFalloffDistance;
        public float DamageFalloffPerMeter;
        public float DieTime;
        public ProjectileType Type;
        public bool IsLastFrame;
        public bool IsFirstFrame;
        public int CustomCounter;
        public bool DieOnCollision;
        public Action<Basic, ActorBase, float, Vector3> CustomCollisionResponse;
    }

    [NonSerialized] public Vector3 DeflectSource;
    [NonSerialized] public bool DoDeflect;
    [NonSerialized] public float DeflectRadius;

    public static ProjectileManager Instance;

    ReusableObject<Basic> projectileCache_;
    List<Basic> basicProjectiles_;
    List<Basic> removeListBasic_;

    private void Awake()
    {
        Instance = this;
        projectileCache_ = new ReusableObject<Basic>(50, this);
        basicProjectiles_ = new List<Basic>();
        removeListBasic_ = new List<Basic>();
    }

    public Basic CreateObject()
    {
        return new Basic();
    }

    int LayerFromProjectileType(ProjectileType type)
    {
        if (type == ProjectileType.HarmsEnemies)
            return GameManager.Instance.LayerPlayerProjectile;
        else if (type == ProjectileType.HarmsPlayer)
            return GameManager.Instance.LayerEnemyProjectile;
        else 
            return GameManager.Instance.LayerNeutral;
    }

    public void Fire(Basic basic)
    {
        if (basic.Type == ProjectileType.HarmsEnemies)
        {
            basic.Color *= GameManager.Instance.SelectedHero.BulletColor;
        }

        // Automatically adjust capsule collider to sprite
        var spriteSize = basic.SpriteInfo.Renderer.sprite.bounds.size;
        basic.SpriteInfo.Collider.size = spriteSize * 0.8f; // A little smaller so player won't feel cheated.

        basic.SpriteInfo.Transform.gameObject.layer = LayerFromProjectileType(basic.Type);
        basic.IsFirstFrame = true;
        basic.IsLastFrame = false;
        basic.SpriteInfo.Transform.position = basic.Position;
        basic.SpriteInfo.Renderer.color = basic.Color;
        basicProjectiles_.Add(basic);
    }

    public Basic GetProjectile()
    {
        Basic result = projectileCache_.GetObject();
        return result;
    }

    public void Tick(float delta)
    {
        TickBasicProjectiles(delta);
    }

    void TickBasicProjectiles(float delta)
    {
        removeListBasic_.Clear();

        float time = Time.time;
        for (int i = 0; i < basicProjectiles_.Count; ++i)
        {
            var p = basicProjectiles_[i];
            if (p.IsLastFrame)
            {
                removeListBasic_.Add(p);
            }
            else
            {
                bool outOfTime = p.DieTime > 0.0f && time >= p.DieTime;
                bool endOfDistance = p.DistanceTraveled >= p.MaxDistance;
                if (!p.IsLastFrame && (endOfDistance || outOfTime))
                {
                    p.SpriteInfo.Renderer.color = Color.white;
                    p.IsLastFrame = true;
                }

                if (p.IsFirstFrame)
                {
                    p.IsFirstFrame = false;
                }

                float frameSpeed = p.Speed * delta * Timers.EnemyTimer;
                Vector3 movement = p.Direction * frameSpeed;
                p.Position += movement;
                if (p.Type == ProjectileType.HarmsEnemies)
                {
                    // Find enemies hit by projectile
                    if (BlackboardScript.GetEnemies(p.Position, p.Radius, 1) > 0)
                    {
                        int matchIdx = BlackboardScript.Matches[0].Idx;
                        ActorBase enemy = BlackboardScript.Enemies[matchIdx];

                        float damage = Basic.CalcDamage(p);
                        if (p.CustomCollisionResponse != null)
                        {
                            p.CustomCollisionResponse(p, enemy, damage, p.Direction);
                        }
                        else
                        {
                            GameManager.Instance.DamageEnemy(enemy, damage, p.Direction, p.Force);
                        }

                        if (p.DieOnCollision)
                            removeListBasic_.Add(p);
                    }
                }
                else if (p.Type == ProjectileType.HarmsPlayer && DoDeflect)
                {
                    Vector3 dir = p.Position - DeflectSource;
                    float distance = dir.magnitude;
                    dir.Normalize();
                    if (distance <= DeflectRadius)
                    {
                        p.Direction = Vector3.Reflect(p.Direction, dir) * 3;
                        float rot_z = Mathf.Atan2(p.Direction.y, p.Direction.x) * Mathf.Rad2Deg;
                        p.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
                        p.Type = ProjectileType.HarmsEnemies;
                    }
                }

                Vector3 perpendicular = new Vector3(-p.Direction.y, p.Direction.x, 0).normalized;
                p.SpriteInfo.Transform.position = p.Position + (Mathf.Sin(Time.time * 10) * p.SwayFactor * perpendicular);
                p.DistanceTraveled += frameSpeed;
            }
        }

        DoDeflect = false;

        for (int i = 0; i < removeListBasic_.Count; ++i)
        {
            ProjectileCache.Instance.ReturnSprite(removeListBasic_[i].SpriteInfo);
            removeListBasic_[i].Reset();
            projectileCache_.ReturnObject(removeListBasic_[i]);
            basicProjectiles_.Remove(removeListBasic_[i]);
        }
    }

    public void StopAll()
    {
        for (int i = 0; i < basicProjectiles_.Count; ++i)
        {
            ProjectileCache.Instance.ReturnSprite(basicProjectiles_[i].SpriteInfo);
            basicProjectiles_[i].Reset();
            projectileCache_.ReturnObject(basicProjectiles_[i]);
        }
        basicProjectiles_.Clear();
    }
}
