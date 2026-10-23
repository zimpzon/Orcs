using System.Linq;
using UnityEngine;

// Easter egg: arena 52 is "AREA 52" (GameManager.ArenaLabel), and in it all enemies wear the Alien Earl skin (not
// Earl himself). Looks only: hit points, speed and everything else stay as they are.
public static class Area52
{
    public const long Level = 52;

    public static bool Active => SaveGame.Members.ArenaLevel == Level && !ScaryEarlFight.Active;

    static SkinAnimations _alien;

    public static SkinAnimations AlienSkin
    {
        get
        {
            if (_alien != null)
                return _alien;
            var skins = Resources.FindObjectsOfTypeAll<SkinsAnimationList>().FirstOrDefault();
            _alien = skins?.SkinAnimations.FirstOrDefault(s => s.Animation == SkinAnimation.Alien);
            return _alien;
        }
    }

    // Walking sprites for an enemy.
    public static Sprite[] EnemySprites
    {
        get
        {
            var skin = AlienSkin;
            if (skin == null)
                return null;
            return skin.RunSprites != null && skin.RunSprites.Length > 0 ? skin.RunSprites : skin.IdleSprites;
        }
    }
}
