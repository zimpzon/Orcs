using System;
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
        SelectedSkinAnimation = SkinAnimations.SkinAnimations.Where(x => x.Animation == SkinAnimation.Default).Single();
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
