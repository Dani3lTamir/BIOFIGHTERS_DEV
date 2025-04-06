using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private int totalScore = 0; // Total score for the game

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find all objects that might implement IDifficultyManager
        var managers = FindObjectsOfType<MonoBehaviour>().OfType<IDifficultyManager>();

        // Get the first one (or null if none exist)
        var manager = managers.FirstOrDefault();
        manager?.InitializeLevel();
    }

    public void WinLevel()
    {
        // Update the total score after a level is completed
        UpdateScore();
        var managers = FindObjectsOfType<MonoBehaviour>().OfType<IDifficultyManager>();
        var manager = managers.FirstOrDefault();
        manager?.RecordSuccess();
        LoadNextLevel();
    }

    public void LoseLevel()
    {
        var managers = FindObjectsOfType<MonoBehaviour>().OfType<IDifficultyManager>();
        var manager = managers.FirstOrDefault();
        manager?.RecordFailure();
        ReloadLevel();
    }

    public void LoadNextLevel()
    {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            LevelLoader.Instance.LoadLevel(nextLevelIndex);
        }
        else
        {
            // Reset Score and load the main menu if no more levels are available
            totalScore = 0; // Reset total score
            LevelLoader.Instance.LoadLevel("MainMenu");
        }
    }

    private void ReloadLevel()
    {
        LevelLoader.Instance.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void UpdateScore()
    {
        // Update the total score after a level is completed if ScoreManager is not null
        if (ScoreManager.Instance != null)
        {
            totalScore += ScoreManager.Instance.GetScore();
            ScoreManager.Instance.score = 0; // Reset score after adding to total
        }
    }

    public int GetTotalScore()
    {
        return totalScore;
    }
}