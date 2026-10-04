using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSkinScript : MonoBehaviour
{
    public static SelectedSkinScript Instance;

    public Image CurrentSelected;
    public SkinsAnimationList SkinAnimations;
    public SkinAnimations SelectedSkinAnimation;

    public void Awake()
    {
        Instance = this;
        SelectedSkinAnimation = Current ?? SkinAnimations.SkinAnimations.Where(x => x.Animation == SkinAnimation.Default).Single();
    }

    // The skin saved in SaveGame.Members.SelectedSkin, resolved without needing this (Skins popup) object to be
    // active. Falls back to Default if the saved skin is missing or not unlocked (e.g. after importing a save).
    static SkinsAnimationList _list;
    static SkinAnimation _cachedFor = SkinAnimation.NotSet;
    static SkinAnimations _cached;

    public static SkinAnimations Current
    {
        get
        {
            var wanted = SaveGame.Members.SelectedSkin;
            if (wanted == _cachedFor && _cached != null)
                return _cached;

            if (_list == null)
                _list = Instance != null ? Instance.SkinAnimations : Resources.FindObjectsOfTypeAll<SkinsAnimationList>().FirstOrDefault();
            if (_list == null)
                return null;

            bool unlocked;
            try { unlocked = SkinScript.GetUnlockStatus(wanted).isUnlocked; }
            catch (System.ArgumentException) { unlocked = false; }

            _cached = (unlocked ? _list.SkinAnimations.FirstOrDefault(x => x.Animation == wanted) : null)
                ?? _list.SkinAnimations.FirstOrDefault(x => x.Animation == SkinAnimation.Default);
            _cachedFor = wanted;
            return _cached;
        }
    }

    float _nextSwitch;
    int _spriteIdx;

    private void Update()
    {
        if (G.D.GameTime < _nextSwitch)
            return;

        _nextSwitch = G.D.GameTime + 0.15f;
        CurrentSelected.sprite = SelectedSkinAnimation.IdleSprites[_spriteIdx];

        _spriteIdx++;
        if (_spriteIdx >= SelectedSkinAnimation.IdleSprites.Length)
            _spriteIdx = 0;
    }
}
