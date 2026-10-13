using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioData AudioData;
    public AudioMixerGroup SfxMixerGroup;

    [System.NonSerialized] public AudioSource PlayerAudioSource;
    [System.NonSerialized] public AudioSource MiscAudioSource;
    [System.NonSerialized] public AudioSource MenuAudioSource; // menu clicks: ignores the Sound setting, has its own
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

        MenuAudioSource = this.gameObject.AddComponent<AudioSource>();
        MenuAudioSource.priority = priority++;

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

    float _nextApplySoundSettings;

    private void Update()
    {
        // Re-applied every second too, to catch audio sources created later.
        if (Time.unscaledTime >= _nextApplySoundSettings)
            ApplySoundSettings();

        RepeatingSawblade.Update(G.D.GameDeltaTime);
    }

    // Sound setting: mutes every audio source (music, sfx, ...) except the menu click source, which has its own
    // Menu sound setting. Per source, since AudioListener.volume can't leave one sound out.
    public void ApplySoundSettings()
    {
        _nextApplySoundSettings = Time.unscaledTime + 1f;
        if (SaveGame.Members == null)
            return;

        AudioListener.volume = 1f;
        bool mute = !SaveGame.Members.SoundEnabled;
        foreach (var source in FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (source != MenuAudioSource)
                source.mute = mute;
        }
    }

    // Menu click (buying upgrades etc.): only the Menu sound setting applies, not Sound.
    public void PlayMenuClick()
    {
        if (SaveGame.Members == null || !SaveGame.Members.MenuSoundEnabled || AudioData.Menu == null)
            return;

        MenuAudioSource.Stop();
        MenuAudioSource.clip = AudioData.Menu;
        MenuAudioSource.volume = MasterVolume * SaveGame.Members.VolumeSfx;
        MenuAudioSource.Play();
    }

    public void SetVolume(float volume)
    {
        MasterVolume = volume;
    }

    public void PlayClipWithRandomPitchForReal(AudioClip clip, float volumeScale = 1.0f, float minInterval = 0f)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        PlayClipForReal(clip, volumeScale, randomPitch, minInterval: minInterval);
    }

    public void PlayClipWithRandomPitch(AudioClip clip, float volumeScale = 1.0f)
    {
    }

    // Last real time each clip was played, for minInterval.
    readonly Dictionary<AudioClip, float> _lastPlayTime = new();

    // minInterval: seconds (real time); the clip is skipped if it was played less than this long ago. 0 = no limit.
    public void PlayClipForReal(AudioClip clip, float volumeScale = 1.0f, float pitch = 1.0f, bool ignoreListenerPause = false, float minInterval = 0f)
    {
        if (clip == null)
            return; // unassigned AudioData slot

        float now = Time.unscaledTime;
        if (minInterval > 0f && _lastPlayTime.TryGetValue(clip, out float last) && now - last < minInterval)
            return;
        _lastPlayTime[clip] = now;

        const int maxInstances = 1;

        int count = CountPlayingInstancesOfClip(clip, out AudioSource existingPlayingSource);
        AudioSource selectedSource = count >= maxInstances ? existingPlayingSource : GetSourceForNewSound(replaceIfNoneAvailable: true);

        float randomPitchOffset = Random.Range(lowPitchRange, highPitchRange);

        if (selectedSource != null)
        {
            // Replace an existing source
            selectedSource.ignoreListenerPause = ignoreListenerPause;
            selectedSource.Stop();
            selectedSource.clip = clip;
            selectedSource.pitch = pitch * randomPitchOffset;
            selectedSource.volume = AudioManager.Instance.MasterVolume * SaveGame.Members.VolumeSfx * volumeScale;
            selectedSource.Play();
        }
    }

    public void PlayClip(AudioClip clip, float volumeScale = 1.0f, float pitch = 1.0f, bool ignoreListenerPause = false)
    {
    }

    public void StopClip(AudioSource source)
    {
        if (source.isPlaying)
            source.Stop();
    }
}
