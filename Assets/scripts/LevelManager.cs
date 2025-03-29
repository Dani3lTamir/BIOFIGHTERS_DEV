using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the scene loaded event
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Get the first enabled MonoBehaviour that implements IDifficultyManager
        var manager = FindObjectsOfType<MonoBehaviour>().OfType<IDifficultyManager>().FirstOrDefault();
        manager?.InitializeLevel();
    }

    public void WinLevel()
    {
        var manager = FindObjectsOfType<MonoBehaviour>().OfType<IDifficultyManager>().FirstOrDefault();
        manager?.RecordSuccess();
        LoadNextLevel();
    }

    public void LoseLevel()
    {
        var manager = FindObjectsOfType<MonoBehaviour>().OfType<IDifficultyManager>().FirstOrDefault();
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
}