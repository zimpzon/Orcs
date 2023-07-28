using UnityEngine;

public class MeleeThrow : MonoBehaviour
{
    Transform trans_;
    Vector3 startPos_;
    Vector3 startDir_;
    float speed_;
    float startSign_;

    private void Awake()
    {
        trans_ = transform;
    }

    float drag_;
    float degrees_;

    public float rotationspeed = 700.0f;

    void Update()
    {
        trans_.rotation = Quaternion.Euler(0.0f, 0.0f, degrees_);
        degrees_ += rotationspeed * Time.deltaTime;

        speed_ -= drag_ * Time.deltaTime;

        //if (speed_ > 0.0f)
        //    speed_ -= drag * Time.deltaTime;
        //else
        //    speed_ += drag * Time.deltaTime;

        trans_.position += startDir_ * speed_ * Time.deltaTime;

        float dist = Vector3.Distance(startPos_, trans_.position);
        GameManager.SetDebugOutput("dist", dist);
        GameManager.SetDebugOutput("speed", speed_);
        bool isBack = Mathf.Sign(speed_) != startSign_ && dist < 0.3f;
        if (isBack)
        {
            GameManager.Instance.MakePoof(trans_.position, 3, 0.25f);
            CacheManager.Instance.MeleeThrowCache.ReturnInstance(gameObject);
        }
    }

    public void Throw(Vector2 dir, float throwPower, float drag)
    {
        startPos_ = trans_.position;
        speed_ = throwPower;
        drag_ = drag;
        startSign_ = Mathf.Sign(speed_);
        startDir_ = dir.normalized;
    }
}
