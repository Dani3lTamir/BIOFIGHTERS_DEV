using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    private TriviaManager triviaManager;


    private void Start()
    {
        // Find the TriviaManager in the scene
        triviaManager = FindFirstObjectByType<TriviaManager>();
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            // if TriviaManager is present, pause the game only if the trivia UI is not active
            else if (triviaManager == null || !triviaManager.triviaUI.activeSelf)
            {
                Pause();
            }

        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void QuitToMenu()
    {
        LevelLoader.Instance.LoadLevel(0);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
}
