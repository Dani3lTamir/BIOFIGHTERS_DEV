using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance; // Singleton for easy access

    [Header("Settings")]
    public float transitionTime = 1f; // Duration of the transition effect
    public Animator transitionAnimator; // Reference to the transition animation

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    // Load a level by name
    public void LoadLevel(string levelName)
    {
        StartCoroutine(LoadLevelCoroutine(levelName));
    }

    // Load a level by build index
    public void LoadLevel(int levelIndex)
    {
        StartCoroutine(LoadLevelCoroutine(levelIndex));
    }

    public void LoadNextLevel()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // Load the next scene
        StartCoroutine(LoadLevelCoroutine(currentSceneIndex + 1));
    }
    

    // Coroutine to handle the transition and loading
    IEnumerator LoadLevelCoroutine(string levelName)
    {
        // Play the transition animation (if available)
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start"); // Trigger the transition animation
        }

        // Wait for the transition to complete
        yield return new WaitForSeconds(transitionTime);

        // Load the new scene
        SceneManager.LoadScene(levelName);
    }

    IEnumerator LoadLevelCoroutine(int levelIndex)
    {
        // Play the transition animation (if available)
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start"); // Trigger the transition animation
        }

        // Wait for the transition to complete
        yield return new WaitForSeconds(transitionTime);

        // Load the new scene
        SceneManager.LoadScene(levelIndex);
    }
}