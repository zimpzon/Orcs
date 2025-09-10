using System;
using UnityEngine;

public enum SkinAnimation { NotSet, Default, WitchDoctor, Necromancer,
    Monster, Orc, Pirate, BigMouth, Wig, Wizard, Zombie, Slug, Voidgazer, KaratEarl,
    UndeadBeauty, WellDressedOrc, Alien, HonorableKnight, SecretiveEarl, NinjaEarl,
    SkaterEarl, WhiteWalkerEarl, PrettyEarl, EvilEyesEarl, ToxicEarl, AttentivePigEarl,
    DisguisedMonsterEarl, FreakyEarl, ScaryEarl, SlickEarl, ChickenEarl, YoungEarl,
    GangsterEarl
}

[Serializable]
public class SkinAnimations
{
    public SkinAnimation Animation;
    public Sprite[] IdleSprites;
    public Sprite[] RunSprites;
}
