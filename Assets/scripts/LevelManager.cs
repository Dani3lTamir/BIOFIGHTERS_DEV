using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;


    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make persistent

        }
        else
        {
            Destroy(gameObject);
        }
    }



    // Handle winning the level
    public void WinLevel()
    {
        Debug.Log("You Win!");

        // Load the next level or return to the main menu
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            LevelLoader.Instance.LoadLevel(nextLevelIndex); // Load the next level
        }
        else
        {
            LevelLoader.Instance.LoadLevel("MainMenu"); // Return to main menu if no more levels
        }
    }

    // Handle losing the level
    public void LoseLevel()
    {
        Debug.Log("You Lose!");

        // Reload the current level or return to the main menu
        LevelLoader.Instance.LoadLevel(SceneManager.GetActiveScene().buildIndex); // Reload the current level
    }
}