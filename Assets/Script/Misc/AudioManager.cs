using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioData AudioData;
    [System.NonSerialized] public AudioSource PlayerAudioSource;
    [System.NonSerialized] public AudioSource MiscAudioSource;
    [System.NonSerialized] public AudioSource EnemyShootAudioSource;
    [System.NonSerialized] public RepeatingAudioClip RepeatingSawblade;

    [System.NonSerialized] public static AudioManager Instance;
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

    public void PlayClipWithRandomPitch(AudioSource source, AudioClip clip, float volumeScale = 1.0f)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        PlayClip(source, clip, volumeScale, randomPitch);
    }

    public void PlayClip(AudioSource source, AudioClip clip, float volumeScale = 1.0f, float pitch = 1.0f)
    {
        source.pitch = pitch;
        source.PlayOneShot(clip, volumeScale * MasterVolume);
    }

    public void StopClip(AudioSource source)
    {
        if (source.isPlaying)
            source.Stop();
    }
}
