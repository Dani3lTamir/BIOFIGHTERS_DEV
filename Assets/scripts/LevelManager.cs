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