using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        LevelLoader.Instance.LoadNextLevel();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
