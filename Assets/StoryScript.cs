using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryScript : MonoBehaviour
{
    public float Speed;

    Transform trans_;

    void Start()
    {
        trans_ = transform;
    }

    void Update()
    {
        trans_.Translate(Camera.main.transform.up * Speed * Time.deltaTime);
        Debug.Log(transform.position.magnitude);

        if (Input.anyKeyDown)
        {
            SceneManager.UnloadSceneAsync("Story");
            SceneManager.LoadScene("Arena");
        }
    }
}
