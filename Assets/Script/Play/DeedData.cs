
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Script
{
    [Serializable]
    public class DeedData
    {
        public DeedEnum Deed;
        public string Title;
        public string Req;
        public string Description;
        public string Creator;
        public int BaseKillReq;
        public int UpdatedKillReq;
        public float PlayerMovementModifier = 1.0f;
        public float MoveSpeedModifier = 1.0f;
        public float FireRateModifier = 1.0f;
        public float FireCountModifier = 1.0f;
        public float HitpointModifier = 1.0f;
        public float ChargeCdModifier = 1.0f;
        public Color BackgroundTint = Color.white;
        public bool ShowOrcs = true;
        public List<EnabledSpawns> EnabledSpawns = new List<EnabledSpawns>();
        public List<WeaponType> WeaponRestrictions = new List<WeaponType>();
        public List<ActorTypeEnum> ValidTargets = new List<ActorTypeEnum>();
        public GameCounter CompletionCounter;
        [NonSerialized] public int DeedCurrentScore;
        [NonSerialized] public bool DeedComplete;

        public void Reset()
        {
            DeedCurrentScore = 0;
            DeedComplete = false;
            UpdatedKillReq = BaseKillReq;
        }

        public static void UpdateWithSandboxData(DeedData deed, SandboxData sand)
        {
            // This is silly. When running sandbox the current GameMode is a special sandbox GameMode. We update that GameMode before starting the sandbox.
            deed.Deed = DeedEnum.Sandbox;
            deed.Req = "Kill All Enemies";
            deed.BaseKillReq = sand.Waves.Sum(w => w.count);
            deed.ShowOrcs = false;
            deed.CompletionCounter = GameCounter.Kill_Any;
            deed.Title = sand.challenge_name;
            deed.BackgroundTint = sand.background_tint;
            deed.HitpointModifier = sand.enemy_hitpoint_scale;
            deed.MoveSpeedModifier = sand.enemy_move_speed_scale;
            deed.FireRateModifier = sand.enemy_fire_interval_scale;
            deed.FireCountModifier = sand.enemy_fire_count_scale;
            deed.ChargeCdModifier = sand.enemy_charge_cooldown_scale;
            deed.PlayerMovementModifier = sand.player_move_speed_scale;
            deed.WeaponRestrictions = new List<WeaponType> { (WeaponType)sand.weapon_left_click };
            deed.ValidTargets = new List<ActorTypeEnum> { ActorTypeEnum.Any };
        }

        public bool OnKill(ActorTypeEnum type)
        {
            if (ValidTargets.Contains(ActorTypeEnum.Any) || ValidTargets.Contains(type))
            {
                if (DeedCurrentScore >= UpdatedKillReq)
                    return false;

                DeedCurrentScore++;
                if (DeedCurrentScore == UpdatedKillReq)
                {
                    DeedComplete = true;
                    GameManager.Instance.PlayerScript.Victory();
                }
                return true;
            }
            return false;
        }
    }
}
