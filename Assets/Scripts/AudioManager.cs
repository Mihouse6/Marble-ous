using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Singleton instance

    public AudioSource MusicSource; // Audio source for background music
    public AudioSource GlobalSfxSource; // Audio source for global sound effects
    public AudioSource[] SFXSources; // Collection of audio sources for sound effects
    public AudioClip[] MusicClips; // Array to store background music clips
    public AudioClip[] SfxClips; // Array to store sound effect clips

    [HideInInspector] public float MaxSFXVolume = 1;

    private Dictionary<string, AudioClip> musicDictionary;
    private Dictionary<string, AudioClip> sfxDictionary;

    void Awake()
    {
        // Ensure only one instance of AudioManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize dictionaries
        musicDictionary = new Dictionary<string, AudioClip>();
        sfxDictionary = new Dictionary<string, AudioClip>();

        // Populate dictionaries
        foreach (var clip in MusicClips)
        {
            musicDictionary.Add(clip.name, clip);
        }

        foreach (var clip in SfxClips)
        {
            sfxDictionary.Add(clip.name, clip);
        }
    }

    public void PlayMusic(string name)
    {
        if (musicDictionary.TryGetValue(name, out AudioClip clip))
        {
            MusicSource.clip = clip;
            MusicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music clip not found: " + name);
        }
    }

    public void PlaySFX(string name)
    {
        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            GlobalSfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + name);
        }
    }

    public void PlaySFXAtPoint(string name, Vector3 pos)
    {
        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            AudioSource.PlayClipAtPoint(clip, pos);
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + name);
        }
    }

    public void PlaySFXAtPoint(string name, Vector3 pos, float volume)
    {
        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            AudioSource.PlayClipAtPoint(clip, pos, volume);
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + name);
        }
    }

    public void StopMusic()
    {
        MusicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        MusicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        GlobalSfxSource.volume = volume;
        for (int i = 0; i < SFXSources.Length; i++)
            SFXSources[i].volume = volume;

        MaxSFXVolume = volume;
    }
}
