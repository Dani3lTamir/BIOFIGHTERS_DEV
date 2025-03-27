using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public string animationClipName = "Outro"; // Name of the animation clip
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
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
        LevelManager.Instance.WinLevel();
    }
}