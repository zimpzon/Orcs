using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioData AudioData;
    [System.NonSerialized] public AudioSource PlayerAudioSource;
    [System.NonSerialized] public AudioSource MiscAudioSource;
    [System.NonSerialized] public AudioSource EnemyShootAudioSource;
    [System.NonSerialized] public RepeatingAudioClip RepeatingSawblade;

    [System.NonSerialized] public static AudioManager Instance;
    Dictionary<int, float> _clipThrottle = new Dictionary<int, float>();

    public float MasterVolume;
    float lowPitchRange = .95f;
    float highPitchRange = 1.05f;

    private void Awake()
    {
        Instance = this;

        int priority = 0;
        PlayerAudioSource = this.gameObject.AddComponent<AudioSource>();
        PlayerAudioSource.priority = priority++;

        RepeatingSawblade = new RepeatingAudioClip(this.gameObject, 3, priority, 4.0f);
        priority++;

        EnemyShootAudioSource = this.gameObject.AddComponent<AudioSource>();
        EnemyShootAudioSource.priority = priority++;

        MiscAudioSource = this.gameObject.AddComponent<AudioSource>();
        MiscAudioSource.priority = priority++;
    }

    public void StopAllRepeating()
    {
        RepeatingSawblade.StopAllClips();
    }

    private void Update()
    {
        RepeatingSawblade.Update(Time.deltaTime);
    }

    public void SetVolume(float volume)
    {
        MasterVolume = volume;
    }

    const float ThrottleTime = 0.1f;

    bool ThrottleClip(AudioClip clip)
    {
        return false;// TODOOOO
        GameManager.SetDebugOutput("id", clip.GetInstanceID());
        _clipThrottle.TryGetValue(clip.GetInstanceID(), out float nextPlay);
        if (Time.realtimeSinceStartup < nextPlay)
        {
            GameManager.SetDebugOutput("next", nextPlay);
            GameManager.SetDebugOutput("rt", Time.realtimeSinceStartup);
            return false;
        }

        GameManager.SetDebugOutput("GO", Time.realtimeSinceStartup);
        _clipThrottle[clip.GetInstanceID()] = Time.realtimeSinceStartup + ThrottleTime;
        return false;
    }

    public void PlayClipWithRandomPitch(AudioSource source, AudioClip clip, float volumeScale = 1.0f)
    {
        if (ThrottleClip(clip))
            return;

        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        PlayClip(source, clip, volumeScale, randomPitch);
    }

    public void PlayClip(AudioSource source, AudioClip clip, float volumeScale = 1.0f, float pitch = 1.0f)
    {
        if (ThrottleClip(clip))
            return;

        source.pitch = pitch;
        source.PlayOneShot(clip, volumeScale * MasterVolume);
    }

    public void StopClip(AudioSource source)
    {
        if (source.isPlaying)
            source.Stop();
    }
}
