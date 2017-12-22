using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    public enum HeroEnum { StarterKnight, OrcWhisperer, TheKing, Last };

    [Serializable]
    public class Hero
    {
        public HeroEnum HeroType;
        public string Name;
        public string Description;
        public GameCounter GameCounter;
        public int Req;
        public Color BulletColor;
        public List<string> Talk = new List<string>();
        public Sprite[] PlayerRunSprites;
        public Sprite[] PlayerIdleSprites;
        public Sprite ShowoffSprite;
        public OrcMood OrcMood;

        public bool IsUnlocked(bool mustBeExactlyAtCounter = false)
        {
            return IsUnlocked(this, mustBeExactlyAtCounter);
        }

        public static bool IsUnlocked(Hero hero, bool mustBeExactlyAtCounter = false)
        {
            if (mustBeExactlyAtCounter)
                return SaveGame.Members.GetCounter(hero.GameCounter) == hero.Req;
            else
                return SaveGame.Members.GetCounter(hero.GameCounter) >= hero.Req;
        }
    }
}
