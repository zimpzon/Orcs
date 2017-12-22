using System.Collections;
using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
    public static MusicManagerScript Instance;
    public AudioClip IntroMusicClip;
    public AudioClip GameMusic1Clip;

    public float MusicVolume = 0.7f;
    AudioSource audioSource_;
    bool isPlaying_;
    bool musicEnabled_ = true;

    void Awake()
    {
        Instance = this;
        audioSource_ = GetComponent<AudioSource>();
    }

    public bool ToggleMusic()
    {
        StopAllCoroutines();

        musicEnabled_ = !musicEnabled_;
        if (!musicEnabled_)
        {
            // Turn off music
            StartCoroutine(Fade(audioSource_.volume, 0.0f));
        }
        else
        {
            // Turn on music
            StartCoroutine(Fade(audioSource_.volume, MusicVolume));
        }

        return musicEnabled_;
    }

    IEnumerator Fade(float from, float to)
    {
        const float Speed = 1.0f;
        float vol = from;
        if (from > to)
        {
            while (vol > to)
            {
                vol -= Time.unscaledDeltaTime * Speed;
                audioSource_.volume = vol;
                yield return null;
            }
        }
        else
        {
            while (vol < to)
            {
                vol += Time.unscaledDeltaTime * Speed;
                audioSource_.volume = vol;
                yield return null;
            }
        }

        audioSource_.volume = to;
    }

    IEnumerator Play(AudioClip clip)
    {
        if (isPlaying_)
            yield return Fade(audioSource_.volume, 0.0f);

        audioSource_.clip = clip;
        audioSource_.loop = true;
        audioSource_.Play();
        isPlaying_ = true;

        if (musicEnabled_)
            audioSource_.volume = MusicVolume;
    }

    public void StopMusic()
    {
        StartCoroutine(Fade(audioSource_.volume, 0.0f));
    }

    public void PlayIntroMusic()
    {
        StartCoroutine(Play(IntroMusicClip));
    }

    public void PlayGameMusic(AudioClip clip)
    {
        StartCoroutine(Play(clip));
    }
}
