using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float Speed = -1200;
    Transform trans_;
    float angle_;

    void Awake()
    {
        trans_ = transform;
    }

    private void Update()
    {
        trans_.Rotate(0, 0, angle_);
        angle_ += G.D.GameDeltaTime * Speed;
    }
}
