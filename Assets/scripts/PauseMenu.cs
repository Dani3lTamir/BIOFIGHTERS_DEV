using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    [SerializeField] private bool shouldPauseGame = true; // Set this to true if you want to pause the game when the menu is opened
    private TriviaManager triviaManager;

    private AudioManager audioManager;


    private void Start()
    {
        // Find the TriviaManager in the scene
        triviaManager = FindFirstObjectByType<TriviaManager>();
        audioManager = AudioManager.Instance; // Get the AudioManager instance
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
            // if TriviaManager is present, pause the game only if the trivia UI is not active, same for tutorial
            else if ((triviaManager == null || !triviaManager.triviaUI.activeSelf) && (TutorialManager.Instance == null || !TutorialManager.Instance.isTutorialActive))
            {
                Pause();
            }

        }
    }

    public void Resume()
    {
        audioManager.Play("ButtonPress"); // Play button press sound when resuming
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        audioManager.Play("BackButtonPress"); // Play back button press sound when pausing

        pauseMenuUI.SetActive(true);
        if (shouldPauseGame)
        {
            // Pause the game
            Time.timeScale = 0f;
        }
        else
        {
            // Resume the game
            Time.timeScale = 1f;
        }
        GameIsPaused = true;
    }

    public void OpenMicropedia()
    {
        // Play button feedback sound
        audioManager.Play("ButtonPress");
        // This will find both active and inactive objects
        MicroPediaUI micropediaUI = FindObjectOfType<MicroPediaUI>(true);

        if (micropediaUI != null)
        {
            micropediaUI.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("MicroPediaUI instance not found in the scene.");
        }
    }


    public void QuitToMenu()
    {
        // Play button feedback sound
        audioManager.Play("BackButtonPress");
        LevelLoader.Instance.LoadLevel(0);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
}
