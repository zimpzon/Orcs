using UnityEngine;

public class RepeatingAudioClip
{
    AudioSource[] audioSource_;
    float[] fading_;
    bool[] isFading_;
    float[] startPitch_;
    float[] startVolume_;
    float fadeSpeed_;
    int maxConcurrent_;
    int currentRequested_;
    float lowPitchRange = .95f;
    float highPitchRange = 1.05f;

    public RepeatingAudioClip(GameObject parentGO, int maxConcurrent, int priority, float fadeSpeed = -1)
    {
        maxConcurrent_ = maxConcurrent;
        audioSource_ = new AudioSource[maxConcurrent_];
        for (int i = 0; i < audioSource_.Length; ++i)
        {
            audioSource_[i] = parentGO.AddComponent<AudioSource>();
            audioSource_[i].priority = priority;
        }

        fadeSpeed_ = fadeSpeed;
        fading_ = new float[maxConcurrent];
        isFading_ = new bool[maxConcurrent];
        startPitch_ = new float[maxConcurrent];
        startVolume_ = new float[maxConcurrent];
    }

    int GetAvailableSource()
    {
        for (int i = 0; i < audioSource_.Length; ++i)
        {
            // If a source is stopped or fading it is considered available
            if (!audioSource_[i].isPlaying || isFading_[i])
                return i;
        }
        return -1;
    }

    int GetPlayingSource()
    {
        for (int i = 0; i < audioSource_.Length; ++i)
        {
            // A source that is fading is considered already stopped
            if (audioSource_[i].isPlaying && !isFading_[i])
                return i;
        }
        return -1;
    }

    public void StartClipWithRandomPitch(AudioClip clip, float volumeScale = 1.0f)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        StartClip(clip, volumeScale, randomPitch);
    }

    public void StartClip(AudioClip clip, float volumeScale = 1.0f, float pitch = 1.0f)
    {
        currentRequested_++;
        int sourceIdx = GetAvailableSource();
        if (sourceIdx == -1)
            return;

        var source = audioSource_[sourceIdx];
        source.pitch = pitch;
        source.loop = true;
        source.volume = volumeScale * AudioManager.Instance.MasterVolume;
        source.clip = clip;
        source.Play();

        startPitch_[sourceIdx] = source.pitch;
        startVolume_[sourceIdx] = source.volume;
        isFading_[sourceIdx] = false;
    }

    public void StopAllClips()
    {
        for (int i = 0; i < audioSource_.Length; ++i)
        {
            if (audioSource_[i].isPlaying)
                audioSource_[i].Stop();

            isFading_[i] = false;
        }
        currentRequested_ = 0;
    }

    public void StopClip(bool doFade = true)
    {
        if (currentRequested_ == 0)
            return;

        currentRequested_--;
        if (currentRequested_ < audioSource_.Length)
        {
            int sourceIdx = GetPlayingSource();
            if (sourceIdx == -1)
                return;

            if (doFade)
            {
                fading_[sourceIdx] = 1.0f;
                isFading_[sourceIdx] = true;
            }
            else
            {
                audioSource_[sourceIdx].Stop();
            }
        }
    }

    public void Update(float deltaMs)
    {
        for (int i = 0; i < fading_.Length; ++i)
        {
            // Fade to 0 and stop clip when 0 is reached
            if (isFading_[i])
            {
                fading_[i] = Mathf.Max(0.0f, fading_[i] - fadeSpeed_ * deltaMs);
                if (audioSource_[i].isPlaying && fading_[i] == 0.0f)
                {
                    audioSource_[i].Stop();
                    isFading_[i] = false;
                }

                float pitchRange = 0.8f;
                float endPitch = startPitch_[i] - pitchRange;
                audioSource_[i].pitch = endPitch + pitchRange * fading_[i];
                audioSource_[i].volume = startVolume_[i] * fading_[i];
            }
        }
    }
}
