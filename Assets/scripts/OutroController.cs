using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public string animationClipName = "Outro"; // Name of the animation clip
    private Animator animator;

    private AudioManager audioManager; // Reference to the AudioManager

    void Start()
    {
        animator = GetComponent<Animator>();
        audioManager = AudioManager.Instance; // Get the AudioManager instance
        PlayCutscene();

    }

    public void PlayCutscene()
    {
        animator.Play(animationClipName); // Play the animation clip
    }

    // Called at the end of the animation (use Animation Event)
    public void OnCutsceneEnd()
    {
        Debug.Log("Cutscene finished!");
        // Enable player control, load next scene, etc.
        LevelManager.Instance.LoadNextLevel();
    }

    public void PlayThrowSound()
    {
        audioManager.Play("TentacleWhip"); // Play the throw sound
    }

    public void PlayCoinSound()
    {
        audioManager.Play("Coins"); // Play the coin sound
    }

    public void PlayWhooshSound()
    {
        audioManager.Play("BigWhoosh"); // Play the whoosh sound
    }

    public void PlaySirenSound()
    {
        audioManager.Play("Siren"); // Play the siren sound
    }

    public void StopSirenSound()
    {
        audioManager.Stop("Siren"); // Stop the siren sound
    }
}