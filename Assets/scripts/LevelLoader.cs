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

        //Change the time scale to 1 when the game starts
        Time.timeScale = 1f;
    }

    void Update()
    {
        // For debugging purposes, load the next level when pressing the 'N' key except in the multiplayer lobby
        if (Input.GetKeyDown(KeyCode.N) && SceneManager.GetActiveScene().name != "1v1_multi")
        {
            LoadNextLevel();
        }
        // For debugging purposes, reload the level when pressing the 'O' key except in the multiplayer lobby
        if (Input.GetKeyDown(KeyCode.O) && SceneManager.GetActiveScene().name != "1v1_multi")
        {
            LoadLevel(SceneManager.GetActiveScene().buildIndex);
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
        Debug.Log("Loading next level...");
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // If next scene is null, return to the main menu
        if (currentSceneIndex + 1 >= SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel("MainMenu");
            return;
        }
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
        Debug.Log("Loading next level...1");

        // Play the transition animation (if available)
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start"); // Trigger the transition animation
            Debug.Log("Loading next level...2");

        }
        Debug.Log("Loading next level...3");

        // Wait for the transition to complete
        yield return new WaitForSeconds(transitionTime); ;
        Debug.Log("Loading next level...4");

        // Load the new scene
        SceneManager.LoadScene(levelIndex);
    }


}