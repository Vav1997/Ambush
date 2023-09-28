using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SingleMusic
{
    public AudioClip music;
    public float volume;
}
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance; // Singleton instance

    private AudioSource currentMusic; // Currently playing music
    private float fadeDuration = 10f; // Duration of music fade-in/out
    private float musicStopFadeDuration = 5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayMusic(SingleMusic singleMusic)
    {
        // If there's already music playing, fade it out
        if (currentMusic != null)
        {
            StartCoroutine(FadeOutMusic(currentMusic, fadeDuration));
        }

        // Create new audio source for music
        currentMusic = gameObject.AddComponent<AudioSource>();
        currentMusic.clip = singleMusic.music;
        currentMusic.loop = true;
        currentMusic.Play();

        // Fade in the new music
        
        StartCoroutine(FadeInMusic(currentMusic, fadeDuration, singleMusic.volume));
        
        
    }

    public void StopMusic(bool instant)
    {
        // If there's no music playing, return
        if (currentMusic == null)
        {
            return;
        }

        // Fade out the music and then stop it
        if(instant)
        {
            currentMusic.volume = 0;
        }
        else
        {
            StartCoroutine(FadeOutMusic(currentMusic, musicStopFadeDuration));
        }
        
        currentMusic = null;
    }

    private IEnumerator FadeInMusic(AudioSource audioSource, float duration, float volume)
    {
        float startVolume = 0.0f;
        float endVolume = volume;

        // Set starting volume to 0 and gradually increase it over the duration
        audioSource.volume = startVolume;
        while (audioSource.volume < endVolume)
        {
            audioSource.volume += Time.deltaTime / duration;
            yield return null;
        }

        // Make sure volume is exactly 1 at the end of the fade-in
        audioSource.volume = endVolume;
    }

    private IEnumerator FadeOutMusic(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float endVolume = 0.0f;
        float timeElapsed = 0;

        // Gradually decrease volume over the duration
        while (audioSource.volume > endVolume)
        {
            //audioSource.volume -= Time.deltaTime / duration;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Stop the audio source and destroy it
        audioSource.Stop();
        Destroy(audioSource);

        // Make sure volume is exactly 0 at the end of the fade-out
        audioSource.volume = endVolume;
    }
}