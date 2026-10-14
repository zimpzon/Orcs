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

    // Muted while the game is minimized. WebGL: the page's visibilitychange event (a focus event would also fire on a
    // click outside the game frame, which shouldn't mute). Other platforms: losing window focus (minimize, alt-tab).
    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            try { JsMappings.RegisterVisibilityListener(gameObject.name); }
            catch (System.Exception e) { Debug.LogError("Visibility listener failed: " + e.Message); }
        }
    }

    // Called from save.jslib (SendMessage), 1 = page hidden.
    public void OnPageVisibility(int hidden) => SetMinimized(hidden != 0);

    void OnApplicationFocus(bool hasFocus)
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            SetMinimized(!hasFocus);
    }

    void SetMinimized(bool minimized)
    {
        AudioListener.pause = minimized;
        if (MusicManagerScript.Instance != null)
            MusicManagerScript.Instance.SetPaused(minimized); // music ignores the listener pause
    }

    private void Update()
    {
        // Re-applied every second too, to catch audio sources created later.
        if (Time.unscaledTime >= _nextApplySoundSettings)
            ApplySoundSettings();

        RepeatingSawblade.Update(G.D.GameDeltaTime);
    }

    // Sound setup: UI sounds (menu click, chime, money pickup) and music always play, only the volume slider affects
    // them. Everything else is combat sound, behind the Combat sound setting (off by default).
    bool IsUiClip(AudioClip clip)
        => clip == AudioData.Menu || clip == AudioData.Chime || clip == AudioData.MoneyPickup || clip == AudioData.DialogShow || clip == AudioData.DialogClose
        || clip == AudioData.ExpandTierList || clip == AudioData.CloseTierList;

    bool CombatSoundEnabled => SaveGame.Members != null && SaveGame.Members.CombatSoundEnabled;

    // Sources that don't go through PlayClipForReal (sawblade loop, grenade fuse, ...) are muted per source. The sfx
    // pool is gated per clip in PlayClipForReal instead, since UI and combat sounds share it.
    public void ApplySoundSettings()
    {
        _nextApplySoundSettings = Time.unscaledTime + 1f;
        if (SaveGame.Members == null)
            return;

        AudioListener.volume = 1f;
        bool mute = !CombatSoundEnabled;
        var music = MusicManagerScript.Instance != null ? MusicManagerScript.Instance.Source : null;
        foreach (var source in FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (source == MenuAudioSource || source == music || sfxSources_.Contains(source))
                continue;
            source.mute = mute;
        }
    }

    // A dialog opened (GlobalPopupManager, rebirth dialog, plain message popups). UI sound, always plays.
    public void PlayDialogShow()
        => PlayClipForReal(AudioData.DialogShow, volumeScale: 0.25f, minInterval: 0.1f, fixedPitch: true);

    // A dialog closed (close button, Escape, click outside, clicking its open button again). UI sound, always plays.
    public void PlayDialogClose()
        => PlayClipForReal(AudioData.DialogClose, volumeScale: 0.25f, minInterval: 0.1f, fixedPitch: true);

    // The upgrade tier list expanding / collapsing (UpgradeResizer). UI sounds, always play.
    public void PlayExpandTierList()
        => PlayClipForReal(AudioData.ExpandTierList, volumeScale: 0.25f, minInterval: 0.1f, fixedPitch: true);

    public void PlayCloseTierList()
        => PlayClipForReal(AudioData.CloseTierList, volumeScale: 0.25f, minInterval: 0.1f, fixedPitch: true);

    // Menu click (buying upgrades etc.): always plays.
    public void PlayMenuClick()
    {
        if (SaveGame.Members == null || AudioData.Menu == null)
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
    public void PlayClipForReal(AudioClip clip, float volumeScale = 1.0f, float pitch = 1.0f, bool ignoreListenerPause = false, float minInterval = 0f, bool fixedPitch = false)
    {
        if (clip == null)
            return; // unassigned AudioData slot

        if (!CombatSoundEnabled && !IsUiClip(clip))
            return;

        float now = Time.unscaledTime;
        if (minInterval > 0f && _lastPlayTime.TryGetValue(clip, out float last) && now - last < minInterval)
            return;
        _lastPlayTime[clip] = now;

        const int maxInstances = 1;

        int count = CountPlayingInstancesOfClip(clip, out AudioSource existingPlayingSource);
        AudioSource selectedSource = count >= maxInstances ? existingPlayingSource : GetSourceForNewSound(replaceIfNoneAvailable: true);

        float randomPitchOffset = fixedPitch ? 1.0f : Random.Range(lowPitchRange, highPitchRange);

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
