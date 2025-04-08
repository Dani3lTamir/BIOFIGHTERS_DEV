using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        LevelLoader.Instance.LoadNextLevel();
    }

    public void OpenMicropedia()
    {
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
