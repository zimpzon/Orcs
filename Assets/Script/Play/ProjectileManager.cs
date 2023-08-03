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
            ReflectOnCollision = false;
            ReflectOnEdges = false;
            RotationSpeed = 0.0f;
            RotationSpeedWhenStuck = 0.0f;
            CollisionSound = null;
            StickToTarget = false;
            StickyMaxTotalDamage = 0;
            StickyDamageTimeNext = 0;
            StickyDamageCd = 0;
            StickyDamageDone = 0;
            CurrentTarget = null;
            StickOffset = Vector3.zero;
            Volume = 1.0f;
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
        public bool ReflectOnCollision;
        public bool ReflectOnEdges;
        public float RotationSpeed;
        public float RotationSpeedWhenStuck;
        public bool StickToTarget;
        public float StickyDamageCd;
        public float StickyDamageTimeNext;
        public float StickyMaxTotalDamage;
        public float StickyDamageDone;
        public Vector3 StickOffset;
        public ActorBase CurrentTarget;
        public AudioClip CollisionSound;
        public float Volume;
        public RepeatingAudioClip StickySoundRepeater;
        public bool MaintainCollisionSound;
        public Action<Basic, ActorBase, float, Vector3> CustomCollisionResponse;
    }

    [NonSerialized] public Vector3 DeflectSource;
    [NonSerialized] public bool DoDeflect;
    [NonSerialized] public float DeflectRadius;

    public static ProjectileManager Instance;

    float arenaBoundsX_;
    float arenaBoundsY_;
    ReusableObject<Basic> projectileCache_;
    List<Basic> basicProjectiles_;
    List<Basic> removeListBasic_;

    private void Awake()
    {
        Instance = this;
        projectileCache_ = new ReusableObject<Basic>(300, this);
        basicProjectiles_ = new List<Basic>();
        removeListBasic_ = new List<Basic>(50);
        arenaBoundsY_ = Camera.main.orthographicSize;
        arenaBoundsX_ = arenaBoundsY_ * AspectUtility.WantedAspectRatio;
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
                if (p.CurrentTarget != null && p.StickySoundRepeater != null)
                    p.StickySoundRepeater.StopClip();

                removeListBasic_.Add(p);
            }
            else
            {
                bool outOfTime = p.DieTime > 0.0f && time >= p.DieTime;
                bool outOfDamage = p.StickyMaxTotalDamage > 0 && p.StickyDamageDone >= p.StickyMaxTotalDamage;
                bool endOfDistance = p.DistanceTraveled >= p.MaxDistance;
                if (!p.IsLastFrame && (endOfDistance || outOfDamage || outOfTime))
                {
                    p.SpriteInfo.Renderer.color = Color.white;
                    p.IsLastFrame = true;
                }

                if (p.IsFirstFrame)
                    p.IsFirstFrame = false;

                float frameSpeed = p.Speed * delta;
                Vector3 movement = p.Direction * frameSpeed;

                if (p.ReflectOnEdges)
                {
                    float newX = p.Position.x + movement.x;
                    float newY = p.Position.y + movement.y;
                    if (newX < GameManager.Instance.ArenaBounds.xMin + 0.5f || newX > GameManager.Instance.ArenaBounds.xMax - 0.5f)
                    {
                        movement = new Vector3(-movement.x, movement.y, 0.0f);
                        p.Direction = new Vector3(-p.Direction.x, p.Direction.y, 0.0f);
                    }

                    if (newY < GameManager.Instance.ArenaBounds.yMin + 0.5f || newY > GameManager.Instance.ArenaBounds.yMax - 0.5f)
                    {
                        movement = new Vector3(movement.x, -movement.y, 0.0f);
                        p.Direction = new Vector3(p.Direction.x, -p.Direction.y, 0.0f);
                    }
                }

                p.Position += movement;

                if (p.StickToTarget && p.CurrentTarget != null)
                {
                    if (p.CurrentTarget.Hp <= 0)
                    {
                        // Target we are stuck to died. Scan for a new one close by or continue moving if nothing found.
                        if (p.StickySoundRepeater != null)
                            p.StickySoundRepeater.StopClip();

                        p.CurrentTarget = null;
                        var closestEnemy = BlackboardScript.GetIdxClosestEnemy(p.Position, 1.5f);

                        bool hasNearbyEnemy = closestEnemy != null;
                        if (hasNearbyEnemy)
                            p.Direction = (closestEnemy.transform.position - p.Position).normalized;
                    }
                    else
                    {
                        // Keep sticking to target at specific offset
                        p.Position = p.CurrentTarget.transform.position + p.StickOffset;
                        p.CurrentTarget.SetSlowmotion(0.5f);
                        if (GameManager.Instance.GameTime > p.StickyDamageTimeNext)
                        {
                            Vector3 damageDirection = (p.StickOffset * -1).normalized;

                            float damage = p.Damage;
                            p.CurrentTarget.ApplyDamage(damage, damageDirection, forceModifier: 0.25f);
                            p.StickyDamageDone += damage;

                            GameManager.Instance.TriggerBlood(p.Position + damageDirection * 0.2f, 8.0f, floorBloodRnd: 0.1f);
                            p.StickyDamageTimeNext = GameManager.Instance.GameTime + p.StickyDamageCd;
                        }
                    }
                }
                else if (p.Type == ProjectileType.HarmsEnemies)
                {
                    // Find enemies hit by projectile
                    if (BlackboardScript.GetEnemies(p.Position, p.Radius) > 0)
                    {
                        const int IdxFirst = 0;
                        ActorBase enemy = BlackboardScript.EnemyOverlap[IdxFirst];
                        if (p.StickToTarget)
                        {
                            // Begin sticking to target
                            p.CurrentTarget = enemy;
                            p.StickOffset = (p.SpriteInfo.Transform.position - enemy.transform.position) * 0.9f;

                            if (p.StickySoundRepeater != null)
                                p.StickySoundRepeater.StartClipWithRandomPitch(p.CollisionSound, p.Volume);
                            else
                                AudioManager.Instance.PlayClipWithRandomPitch(p.CollisionSound, p.Volume);
                        }

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

                        if (p.ReflectOnCollision)
                        {
                            if (UnityEngine.Random.value < 0.5f)
                                p.Direction = Vector3.Reflect(p.Direction, Vector3.up);
                            else
                                p.Direction = Vector3.Reflect(p.Direction, Vector3.right);
                        }
                    }
                }
                else if (p.Type == ProjectileType.HarmsPlayer && DoDeflect)
                {
                    Vector3 dir = p.Position - DeflectSource;
                    float distance = dir.magnitude;
                    if (distance <= DeflectRadius)
                    {
                        // Projectile deflected
                        dir.Normalize();
                        p.Direction = Vector3.Reflect(p.Direction, dir) * 3;
                        float rot_z = Mathf.Atan2(p.Direction.y, p.Direction.x) * Mathf.Rad2Deg;
                        p.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
                        p.Color = new Color(0.8f, 1.0f, 1.0f);
                        p.SpriteInfo.Renderer.color = p.Color;
                        p.Type = ProjectileType.HarmsEnemies;
                        p.SpriteInfo.Transform.gameObject.layer = LayerFromProjectileType(p.Type);
                    }
                }

                if (p.RotationSpeed > 0.0f)
                    p.SpriteInfo.Transform.rotation = Quaternion.Euler(0.0f, 0.0f, Time.time * (p.CurrentTarget == null ? p.RotationSpeed : p.RotationSpeedWhenStuck));

                if (p.SwayFactor > 0.0f)
                {
                    Vector3 perpendicular = new Vector3(-p.Direction.y, p.Direction.x, 0).normalized;
                    p.SpriteInfo.Transform.position = p.Position + (Mathf.Sin(Time.time * 10) * p.SwayFactor * perpendicular);
                }
                else
                {
                    p.SpriteInfo.Transform.position = p.Position;
                }

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
