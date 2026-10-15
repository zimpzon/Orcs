using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Background music: plays Tracks in order and starts over after the last one (a single track just loops). Right / Left
// arrow: next / previous song (not with RightCtrl - that's the time-scale cheat - or while typing in a text field). Started from
// GameManager.Start; volume = VolumeMusic x VolumeMaster (GameManager.ApplyVolume). Add more songs to Tracks in the
// Inspector.
public class MusicManagerScript : MonoBehaviour
{
    public static MusicManagerScript Instance;
    public AudioClip[] Tracks;

    AudioSource audioSource_;
    bool isPlaying_;
    float volume_;
    int _track = -1;
    bool _playlistStarted;
    bool _paused;

    void Awake()
    {
        Instance = this;
        audioSource_ = GetComponent<AudioSource>();
        audioSource_.ignoreListenerPause = true;
        GameEvents.OnSaveWiped += _ => ApplyMusicEnabled(); // a new save has its own music setting
    }

    public void StartPlaylist()
    {
        if (Tracks == null || Tracks.Length == 0)
            return;

        _playlistStarted = true;
        PlayTrack(0);
    }

    void PlayTrack(int index)
    {
        _track = index;
        audioSource_.clip = Tracks[index];
        audioSource_.loop = Tracks.Length == 1;
        audioSource_.volume = volume_;
        // Only actually start it when allowed: Pause() right after Play() in the same frame can be ignored, which let
        // the first song play at game start with music off. Otherwise ApplyPause starts it when music is turned on.
        if (!_paused && !MusicOff)
            audioSource_.Play();
        isPlaying_ = true;
    }

    bool MusicOff => SaveGame.Members != null && !SaveGame.Members.MusicEnabled;

    void Update()
    {
        HandleSongKeys();

        // Track finished (not paused by minimizing or the Music checkbox): next one, wrapping around to the first.
        if (_playlistStarted && !_paused && !MusicOff && Tracks.Length > 1 && !audioSource_.isPlaying)
            PlayTrack((_track + 1) % Tracks.Length);
    }

    void HandleSongKeys()
    {
        if (!_playlistStarted || Tracks.Length < 2 || Input.GetKey(KeyCode.RightControl) || IsTyping())
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            PlayTrack((_track + 1) % Tracks.Length);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            PlayTrack((_track - 1 + Tracks.Length) % Tracks.Length);
    }

    static bool IsTyping()
    {
        var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        return selected != null && selected.TryGetComponent<TMP_InputField>(out var field) && field.isFocused;
    }

    // Music checkbox: off pauses the song (nothing plays in the background), on resumes it where it stopped.
    public void ApplyMusicEnabled() => ApplyPause();

    // Music ignores AudioListener.pause (ignoreListenerPause), so minimizing pauses it here (AudioManager).
    public void SetPaused(bool paused)
    {
        _paused = paused;
        ApplyPause();
    }

    void ApplyPause()
    {
        if (_paused || MusicOff)
        {
            audioSource_.Pause();
            return;
        }

        if (audioSource_.isPlaying || audioSource_.clip == null)
            return;
        if (audioSource_.time > 0f)
            audioSource_.UnPause(); // paused mid-song: continue
        else
            audioSource_.Play();    // never started (music was off at game start)
    }

    public AudioSource Source => audioSource_;

    public void SetVolume(float volume)
    {
        volume_ = volume;
        audioSource_.volume = volume;
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

        audioSource_.volume = volume_;
    }

    public void StopMusic()
    {
        _playlistStarted = false;
        StartCoroutine(Fade(audioSource_.volume, 0.0f));
    }

    public void PlayGameMusic(AudioClip clip)
    {
        _playlistStarted = false;
        StartCoroutine(Play(clip));
    }
}
