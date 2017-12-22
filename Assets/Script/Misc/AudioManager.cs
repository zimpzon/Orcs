using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioData AudioData;

    [System.NonSerialized]
    public AudioSource PlayerAudioSource;
    [System.NonSerialized]
    public AudioSource MiscAudioSource;
    [System.NonSerialized]
    public AudioSource EnemyShootAudioSource;
    [System.NonSerialized]
    public AudioSource XpAudioSource;

    public static AudioManager Instance;

    float lowPitchRange = .95f;
    float highPitchRange = 1.05f;

    private void Awake()
    {
        Instance = this;

        int priority = 0;
        PlayerAudioSource = this.gameObject.AddComponent<AudioSource>();
        PlayerAudioSource.priority = priority++;

        EnemyShootAudioSource = this.gameObject.AddComponent<AudioSource>();
        EnemyShootAudioSource.priority = priority++;

        MiscAudioSource = this.gameObject.AddComponent<AudioSource>();
        MiscAudioSource.priority = priority++;

        XpAudioSource = this.gameObject.AddComponent<AudioSource>();
        XpAudioSource.priority = priority++;
    }

    private void Start()
    {
    }

    public void PlayClipWithRandomPitch(AudioSource source, AudioClip clip, float volumeScale = 1.0f)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        PlayClip(source, clip, volumeScale, randomPitch);
    }

    public void PlayClip(AudioSource source, AudioClip clip, float volumeScale = 1.0f, float pitch = 1.0f)
    {
        source.pitch = pitch;
        source.PlayOneShot(clip, volumeScale);
    }

    public void StopClip(AudioSource source)
    {
        if (source.isPlaying)
            source.Stop();
    }
}
