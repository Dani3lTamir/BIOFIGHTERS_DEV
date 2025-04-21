using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton instance
    public Sound[] sounds; // Array of sounds to manage

    public Sound[] backgroundMusic; // Array of background music to manage

    void Awake()
    {
        // Ensure there's only one instance of the AudioManager
        if (Instance == null)
        {
            Instance = this;
           // DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop; 
        }

        foreach (Sound music in backgroundMusic)
        {
            music.source = gameObject.AddComponent<AudioSource>();
            music.source.clip = music.clip;
            music.source.volume = music.volume;
            music.source.pitch = music.pitch;
            music.source.loop = music.loop; 
        }
    }


    public void Play(string soundName)
    {
        Sound sound = System.Array.Find(sounds, s => s.name == soundName);
        if (sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
        }
    }

    public void PlayAt(string soundName, Transform sourceTransform)
    {
        Sound sound = System.Array.Find(sounds, s => s.name == soundName);
        if (sound != null)
        {
            AudioSource tempSource = sourceTransform.gameObject.AddComponent<AudioSource>();
            tempSource.clip = sound.clip;
            tempSource.volume = sound.volume;
            tempSource.pitch = sound.pitch;
            tempSource.loop = sound.loop; // Set loop to true or false based on the sound
            tempSource.spatialBlend = 1f; // make it 3D
            tempSource.Play();

            if (!tempSource.loop)
                Destroy(tempSource, sound.clip.length); // clean up
        }
        else
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
        }
    }


    public void Stop(string soundName)
    {
        Sound sound = System.Array.Find(sounds, s => s.name == soundName);
        if (sound != null)
        {
            sound.source.Stop();
        }
        else
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
        }
    }

    public void PlayBackgroundMusic(string musicName)
    {
        Sound music = System.Array.Find(backgroundMusic, m => m.name == musicName);
        if (music != null)
        {
            music.source.Play();
        }
        else
        {
            Debug.LogWarning("Background Music: " + musicName + " not found!");
        }
    }

    public void StopBackgroundMusic(string musicName)
    {
        Sound music = System.Array.Find(backgroundMusic, m => m.name == musicName);
        if (music != null)
        {
            music.source.Stop();
        }
        else
        {
            Debug.LogWarning("Background Music: " + musicName + " not found!");
        }
    }
}
