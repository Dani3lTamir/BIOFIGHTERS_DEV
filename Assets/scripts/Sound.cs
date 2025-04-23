using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound 
{
    public string name; // The name of the sound
    public AudioClip clip; // The audio clip to play
    [Range(0f, 1f)]
    public float volume = 0.5f; // The volume of the sound
    [Range(0f, 2f)]
    public float pitch = 1f; // The pitch of the sound

    public bool loop = false; // Whether the sound should loop

    [HideInInspector]
    public AudioMixerGroup mixerGroup; // The audio mixer group to which the sound belongs (SFX or Music)

    [HideInInspector]
    public AudioSource source; // The audio source that will play the sound
}
