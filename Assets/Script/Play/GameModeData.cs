﻿using System;
using UnityEngine;

namespace Assets.Script
{
    [Serializable]
    public class GameModeData
    {
        public GameModeEnum GameMode;
        public Color BackgroundTint;
        public AudioClip Music;
        public float HitpointModifier = 1.0f;
        public float MoveSpeedModifier = 1.0f;
        public float ExplosionModifier = 1.0f;
        public float FireRateModifier = 1.0f;
        public float FireCountModifier = 1.0f;
        public float ChargeCdModifier = 1.0f;
        public float MassModifier = 1.0f;
        public float PlayerMoveSpeedModifier = 1.0f;
    }
}
