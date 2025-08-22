using System;
using UnityEngine;

public enum SkinAnimation { NotSet, Default, WitchDoctor, Necromancer,
    Monster, Orc, Pirate, BigMouth, Wig, Wizard, Zombie, Slug, Voidgazer, KaratEarl
}

[Serializable]
public class SkinAnimations
{
    public SkinAnimation Animation;
    public Sprite[] IdleSprites;
    public Sprite[] RunSprites;
}
