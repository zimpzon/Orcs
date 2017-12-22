
using System;
using System.Collections.Generic;

namespace Assets.Script
{
    [Serializable]
    public class DeedData
    {
        public DeedEnum Deed;
        public string Title;
        public string Req;
        public string Description;
        public int KillReq;
        public float PlayerMovementModifier = 1.0f;
        public float MoveSpeedModifier = 1.0f;
        public float FireRateModifier = 1.0f;
        public float FireCountModifier = 1.0f;
        public float HitpointModifier = 1.0f;
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
        }

        public bool OnKill(ActorTypeEnum type)
        {
            if (ValidTargets.Contains(ActorTypeEnum.Any) || ValidTargets.Contains(type))
            {
                if (DeedCurrentScore >= KillReq)
                    return false;

                DeedCurrentScore++;
                if (DeedCurrentScore == KillReq)
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
