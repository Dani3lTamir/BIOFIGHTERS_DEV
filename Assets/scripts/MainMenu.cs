using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Play Game button clicked.");
        PlayClickSound();
        LevelLoader.Instance.LoadNextLevel();
    }

    public void OpenMicropedia()
    {
        PlayClickSound();
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

    public void OpenSettings()
    {
        PlayClickSound();
        // This will find both active and inactive objects
        SettingsMenu settings = FindObjectOfType<SettingsMenu>(true);

        if (settings != null)
        {
            settings.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("SettingsMenu instance not found in the scene.");
        }
    }

    public void PlayMultiplayer()
    {
        PlayClickSound();
        LevelLoader.Instance.LoadLevel("1v1_multi");
    }

    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play("ButtonPress");
        }
        else
        {
            Debug.LogError("AudioManager instance not found in the scene.");
        }
    }
}
