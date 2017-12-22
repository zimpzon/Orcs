using UnityEngine;

public class AnimationController
{
    public bool WasLastFrame;

    int idx_ = 0;
    float nextChange_;
    float time_;
    Sprite[] prevAnimation;

    public void ResetTime()
    {
        nextChange_ = 0;
    }

    public void Tick(float delta, SpriteRenderer renderer, Sprite[] animations, float delay = 0.15f)
    {
        if (!object.ReferenceEquals(animations, prevAnimation))
        {
            nextChange_ = 0;
            prevAnimation = animations;
            idx_ = 0;
        }

        time_ += delta;
        if (time_ >= nextChange_)
        {
            WasLastFrame = false;
            renderer.sprite = animations[idx_++];
            nextChange_ = time_ + delay;
            if (idx_ >= animations.Length)
            {
                idx_ = 0;
                WasLastFrame = true;
            }
        }
    }
}
