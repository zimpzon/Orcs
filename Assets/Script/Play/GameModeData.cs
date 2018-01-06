
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    [Serializable]
    public class GameModeData
    {
        public GameModeEnum GameMode;
        public Color BackgroundTint;
        public Color AmbientColor = Color.white;
        public AudioClip Music;
        public float HitpointModifier = 1.0f;
        public float MoveSpeedModifier = 1.0f;
        public float ExplosionModifier = 1.0f;
        public float FireRateModifier = 1.0f;
        public float FireCountModifier = 1.0f;
        public float ChargeCdModifier = 1.0f;
        public float MassModifier = 1.0f;
        public float PlayerMoveSpeedModifier = 1.0f;
        public List<WeaponType> WeaponRestrictions = new List<WeaponType>();
        public bool SmallEnemiesExplode = false;
        public bool HasExtraLargeWalkers = false;

        public static void UpdateWithDeedData(GameModeData gameMode, DeedData deed)
        {
            // This is silly. When running deeds the current GameMode is a special deed GameMode. We update that GameMode before starting the deed.
            gameMode.BackgroundTint = deed.BackgroundTint;
            gameMode.HitpointModifier = deed.HitpointModifier;
            gameMode.MoveSpeedModifier = deed.MoveSpeedModifier;
            gameMode.FireRateModifier = deed.FireRateModifier;
            gameMode.FireCountModifier = deed.FireCountModifier;
            gameMode.PlayerMoveSpeedModifier = deed.PlayerMovementModifier;
            gameMode.ChargeCdModifier = deed.ChargeCdModifier;
            gameMode.WeaponRestrictions = deed.WeaponRestrictions;
        }
    }
}
