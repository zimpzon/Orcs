using Assets.Script;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour, IObjectFactory<ProjectileManager.Basic>
{
    public enum ProjectileType { HarmsPlayer, HarmsEnemies, HarmsNothing };

    public class Basic
    {
        public static double CalcDamage(Basic b)
        {
            double damage = b.Damage;
            return damage;
        }

        public void OnHit()
        {
            IsLastFrame = true;
        }

        public void Reset()
        {
            PreviousJumpTargets.Clear();
            SpriteInfo.Renderer = null;
            SpriteInfo.Transform = null;
            Position = Vector3.zero;
            Color = Color.white;
            Speed = 0.0f;
            SwayFactor = 0.0f;
            Radius = 0.0f;
            Blink = false;
            Force = 0.25f;
            DamageCd = 0.0f;
            DamageTimeNext = 0.0f;
            DamageSource = ActorDamageSource.Unkonwn;
            Direction = Vector3.zero;
            MaxDistance = 0.0f;
            DistanceTraveled = 0.0f;
            DamageFalloffDistance = 0.0f;
            DamageFalloffPerMeter = 0.0f;
            DieTime = 0;
            Type = ProjectileType.HarmsEnemies;
            JumpToNearbyTarget = false;
            JumpMaxDistance = 1000.0f;
            IsLastFrame = false;
            OnEndOfLife = null;
            IsFirstFrame = true;
            CustomCounter = 0;
            CustomCollisionResponse = null;
            DieOnCollision = true;
            ReflectOnCollision = false;
            ReflectOnEdges = false;
            ReflectRotationOffset = 0;
            RotationSpeed = 0.0f;
            RotationSpeedWhenStuck = 0.0f;
            CollisionSound = null;
            StickToTarget = false;
            StickyNextEnemySeek = 0;
            StickyMaxTotalDamage = 0;
            StickyDamageTimeNext = 0;
            StickyDamageCd = 0;
            StickyDamageDone = 0;
            CurrentTarget = null;
            StickOffset = Vector3.zero;
            Volume = 1.0f;
            ParticleSystem = null;
            ParticleEmitCount = 0;
            ParticleEmitDelay = 0;
            ParticleNextEmit = 0;
        }

        public List<ActorBase> PreviousJumpTargets = new (5);
        public ProjectileInfo SpriteInfo;
        public Vector3 Position;
        public Color Color;
        public float Speed;
        public float SwayFactor;
        public float Radius;
        public double Damage;
        public float DamageCd;
        public float DamageTimeNext;
        public ActorDamageSource DamageSource;
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
        public Action<Basic, bool> OnEndOfLife;
        public bool IsFirstFrame;
        public int CustomCounter;
        public bool DieOnCollision;
        public bool ReflectOnCollision;
        public bool ReflectOnEdges;
        public float ReflectRotationOffset;
        public float RotationSpeed;
        public float RotationSpeedWhenStuck;
        public bool JumpToNearbyTarget;
        public float JumpMaxDistance;
        public double JumpDamageMul;
        public bool StickToTarget;
        public float StickyNextEnemySeek;
        public float StickyDamageCd;
        public float StickyDamageTimeNext;
        public float StickyDamageSimulateCd;
        public float StickyDamageTimeNextSimulate;
        public long StickyDamageCounter;
        public double StickyMaxTotalDamage;
        public double StickyDamageDone;
        public Vector3 StickOffset;
        public ActorBase CurrentTarget;
        public AudioClip CollisionSound;
        public float Volume;
        public RepeatingAudioClip StickySoundRepeater;
        public bool MaintainCollisionSound;
        public ParticleSystem ParticleSystem;
        public int ParticleEmitCount;
        public float ParticleEmitDelay;
        public float ParticleNextEmit;

        public Action<Basic, ActorBase, double, Vector3> CustomCollisionResponse;
    }

    [NonSerialized] public Vector3 DeflectSource;
    [NonSerialized] public bool DoDeflect;
    [NonSerialized] public float DeflectRadius;

    public static ProjectileManager Instance;

    //float arenaBoundsX_;
    //float arenaBoundsY_;
    ReusableObject<Basic> projectileCache_;
    List<Basic> basicProjectiles_;
    List<Basic> removeListBasic_;

    private void Awake()
    {
        Instance = this;
        projectileCache_ = new ReusableObject<Basic>(300, this);
        basicProjectiles_ = new List<Basic>();
        removeListBasic_ = new List<Basic>(50);
        //arenaBoundsY_ = Camera.main.orthographicSize;
        //arenaBoundsX_ = arenaBoundsY_ * AspectUtility.WantedAspectRatio;
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

        float time = G.D.GameTime;
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

                    if (p.OnEndOfLife is not null)
                    {
                        p.OnEndOfLife(p, false);
                    }
                }

                if (p.IsFirstFrame)
                    p.IsFirstFrame = false;

                float frameSpeed = p.Speed * delta;
                Vector3 movement = p.Direction * frameSpeed;

                if (p.ReflectOnEdges)
                {
                    float newX = p.Position.x + movement.x;
                    float newY = p.Position.y + movement.y;
                    if (newX < GameManager.ArenaBounds.xMin + 0.5f || newX > GameManager.ArenaBounds.xMax - 0.5f)
                    {
                        movement = new Vector3(-movement.x, movement.y, 0.0f);
                        p.Direction = new Vector3(-p.Direction.x, p.Direction.y, 0.0f);
                    }

                    if (newY < GameManager.ArenaBounds.yMin + 0.5f || newY > GameManager.ArenaBounds.yMax - 0.5f)
                    {
                        movement = new Vector3(movement.x, -movement.y, 0.0f);
                        p.Direction = new Vector3(p.Direction.x, -p.Direction.y, 0.0f);
                    }

                    float rot_z = (Mathf.Atan2(p.Direction.y, p.Direction.x) * Mathf.Rad2Deg) + p.ReflectRotationOffset;
                    p.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
                }

                p.Position += movement;

                if (p.StickToTarget && p.CurrentTarget is not null)
                {
                    if (p.CurrentTarget.Hp <= 0)
                    {
                        // Target we are stuck to died. Scan for a new one close by or continue moving if nothing found.
                        if (p.StickySoundRepeater != null)
                            p.StickySoundRepeater.StopClip();

                        p.CurrentTarget = null;
                        var closestEnemy = BlackboardScript.GetClosestEnemy(p.Position, 1000.0f); // TODO: parameter?

                        bool hasNearbyEnemy = closestEnemy != null;
                        if (hasNearbyEnemy)
                            p.Direction = (closestEnemy.transform.position - p.Position).normalized;
                    }
                    else
                    {
                        // Keep sticking to target at specific offset
                        p.Position = p.CurrentTarget.transform.position + p.StickOffset;
                        p.CurrentTarget.SetSlowmotion(0.5f);
                        Vector3 damageDirection = (p.StickOffset * -1).normalized;
                        if (GameManager.Instance.GameTime > p.StickyDamageTimeNext)
                        {
                            double damage = p.Damage;
                            GameManager.Instance.DamageEnemy(p.CurrentTarget, damage, damageDirection, forceModifier: 0.25f, p.DamageSource);
                            p.StickyDamageDone += damage;

                            GameManager.Instance.TriggerBlood(p.Position + damageDirection * 0.2f, 8.0f, floorBloodRnd: 0.1f);
                            p.StickyDamageTimeNext = GameManager.Instance.GameTime + p.StickyDamageCd;
                        }
                        else if (GameManager.Instance.GameTime > p.StickyDamageTimeNextSimulate)
                        {
                            // Fake 0 damage to get a visual effect more often than dealing real damage.
                            p.CurrentTarget.ApplyDamage2(amount: 0, damageDirection, forceModifier: 0.25f);
                            p.StickyDamageTimeNextSimulate = GameManager.Instance.GameTime + p.StickyDamageSimulateCd;
                        }
                    }
                }
                else if (p.Type == ProjectileType.HarmsEnemies)
                {
                    // Find enemies hit by projectile
                    if (BlackboardScript.GetEnemies(p.Position, p.Radius) > 0)
                    {
                        bool alreadyVisited = false;
                        const int IdxFirst = 0;
                        ActorBase enemy = BlackboardScript.EnemyOverlap[IdxFirst];
                        if (p.PreviousJumpTargets.Count > 0)
                        {
                            for (int j = 0; j < p.PreviousJumpTargets.Count; ++j)
                            {
                                if (p.PreviousJumpTargets[j].UniqueId == enemy.UniqueId)
                                {
                                    alreadyVisited = true;
                                    break;
                                }
                            }
                        }

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

                        double damage = Basic.CalcDamage(p);
                        if (p.CustomCollisionResponse != null)
                        {
                            p.CustomCollisionResponse(p, enemy, damage, p.Direction);
                        }
                        else
                        {
                            if (!alreadyVisited)
                            {
                                bool offCd = GameManager.Instance.GameTime > p.DamageTimeNext;
                                if (offCd)
                                {
                                    GameManager.Instance.DamageEnemy(enemy, damage, p.Direction, p.Force, p.DamageSource);
                                    p.DamageTimeNext = GameManager.Instance.GameTime + p.DamageCd;
                                }

                                if (offCd && p.JumpToNearbyTarget)
                                {
                                    p.PreviousJumpTargets.Add(enemy);

                                    float jumpRadius = Mathf.Max(p.JumpMaxDistance, 0.0f);
                                    if (jumpRadius > 0.01f)
                                    {
                                        var closestEnemy = BlackboardScript.GetClosestEnemy(p.Position, jumpRadius, p.PreviousJumpTargets); // TODO: parameter?

                                        bool hasNearbyEnemy = closestEnemy != null;
                                        if (hasNearbyEnemy)
                                        {
                                            p.Direction = (closestEnemy.transform.position - p.Position).normalized;
                                            float rot_z = Mathf.Atan2(p.Direction.y, p.Direction.x) * Mathf.Rad2Deg;
                                            p.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
                                        }
                                    }
                                }
                            }
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
                    else if (p.StickToTarget)
                    {
                        if (PlayerUpgrades.Data.NecromancerAggressiveSkulls && G.D.GameTime > p.StickyNextEnemySeek)
                        {
                            p.StickyNextEnemySeek = G.D.GameTime + 0.1f;

                            // Moving around and not close to an enemy. Move towards any close enemy.
                            var closestEnemy = BlackboardScript.GetClosestEnemy(p.Position, Mathf.Max(p.JumpMaxDistance, 0.0f)); // TODO: parameter?

                            bool hasNearbyEnemy = closestEnemy != null;
                            if (hasNearbyEnemy)
                                p.Direction = (closestEnemy.transform.position - p.Position).normalized;
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
                        p.Color = G.D.DeflectedProjectileColor;
                        p.SpriteInfo.Renderer.color = p.Color;
                        p.Type = ProjectileType.HarmsEnemies;
                        p.SpriteInfo.Transform.gameObject.layer = LayerFromProjectileType(p.Type);
                    }
                }

                if (p.RotationSpeed > 0.0f)
                {
                    float baseRotation = Mathf.Atan2(p.Direction.y, p.Direction.x) * Mathf.Rad2Deg;
                    // Use major axis to determine rotation direction: clockwise for positive X/Y, counter-clockwise for negative
                    float rotationDirection = (Mathf.Abs(p.Direction.x) > Mathf.Abs(p.Direction.y)) ?
                        Mathf.Sign(p.Direction.x) : Mathf.Sign(p.Direction.y);
                    float spinRotation = G.D.GameTime * -rotationDirection * (p.CurrentTarget == null ? p.RotationSpeed : p.RotationSpeedWhenStuck);
                    p.SpriteInfo.Transform.rotation = Quaternion.Euler(0.0f, 0.0f, baseRotation + spinRotation);
                }

                if (p.SwayFactor > 0.0f)
                {
                    Vector3 perpendicular = new Vector3(-p.Direction.y, p.Direction.x, 0).normalized;
                    p.SpriteInfo.Transform.position = p.Position + (Mathf.Sin(G.D.GameTime * 10) * p.SwayFactor * perpendicular);
                }
                else
                {
                    p.SpriteInfo.Transform.position = p.Position;
                }

                p.DistanceTraveled += frameSpeed;
            }

            if (p.ParticleSystem && G.D.GameTime > p.ParticleNextEmit)
            {
                p.ParticleSystem.transform.position = p.Position;
                p.ParticleSystem.Emit(p.ParticleEmitCount);
                p.ParticleNextEmit = G.D.GameTime + p.ParticleEmitDelay;
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
