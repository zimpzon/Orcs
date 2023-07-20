using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioData AudioData;
    public AudioMixerGroup SfxMixerGroup;

    [System.NonSerialized] public AudioSource PlayerAudioSource;
    [System.NonSerialized] public AudioSource MiscAudioSource;
    [System.NonSerialized] public AudioSource EnemyShootAudioSource;
    [System.NonSerialized] public RepeatingAudioClip RepeatingSawblade;

    [System.NonSerialized] public static AudioManager Instance;

    public float MasterVolume;
    float lowPitchRange = .95f;
    float highPitchRange = 1.05f;
    List<AudioSource> sfxSources_ = new List<AudioSource>();

    private void Awake()
    {
        Instance = this;

        int priority = 0;
        PlayerAudioSource = this.gameObject.AddComponent<AudioSource>();
        PlayerAudioSource.priority = priority++;

        RepeatingSawblade = new RepeatingAudioClip(this.gameObject, maxConcurrent: 2, priority, fadeSpeed: 4.0f);
        priority++;

        EnemyShootAudioSource = this.gameObject.AddComponent<AudioSource>();
        EnemyShootAudioSource.priority = priority++;

        MiscAudioSource = this.gameObject.AddComponent<AudioSource>();
        MiscAudioSource.priority = priority++;

        const int SfxSourceCount = 20;

        for (int i = 0; i < SfxSourceCount; ++i)
        {
            var sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = SfxMixerGroup;
            sfxSources_.Add(sfxSource);
        }
    }

    int CountPlayingInstancesOfClip(AudioClip clip, out AudioSource currentlyPlayingInstance)
    {
        currentlyPlayingInstance = null;
        int result = 0;
        for (int i = 0; i < sfxSources_.Count; ++i)
        {
            if (sfxSources_[i].isPlaying && sfxSources_[i].clip == clip)
            {
                result++;
                currentlyPlayingInstance = sfxSources_[i];
            }
        }
        return result;
    }

    AudioSource GetSourceForNewSound(bool replaceIfNoneAvailable)
    {
        for (int i = 0; i < sfxSources_.Count; ++i)
        {
            if (!sfxSources_[i].isPlaying)
                return sfxSources_[i];
        }

        // No sources were available, replace a random existing if requested, else return null
        return replaceIfNoneAvailable ? sfxSources_[UnityEngine.Random.Range(0, sfxSources_.Count)] : null;
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

    public void PlayClipWithRandomPitch(AudioClip clip, float volumeScale = 1.0f)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        PlayClip(clip, volumeScale, randomPitch);
    }

    public void PlayClip(AudioClip clip, float volumeScale = 1.0f, float pitch = 1.0f)
    {
        const int maxInstances = 1;

        AudioSource selectedSource = null;
        int count = CountPlayingInstancesOfClip(clip, out AudioSource existingPlayingSource);
        selectedSource = count >= maxInstances ? existingPlayingSource : GetSourceForNewSound(replaceIfNoneAvailable: true);
        if (selectedSource != null)
        {
            // Replace an existing source
            selectedSource.Stop();
            selectedSource.clip = clip;
            selectedSource.pitch = pitch;
            selectedSource.Play();
        }
    }

    public void StopClip(AudioSource source)
    {
        if (source.isPlaying)
            source.Stop();
    }
}
