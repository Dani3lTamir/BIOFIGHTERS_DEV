using UnityEngine;
using UnityEngine.UI;  // Required for Button class

public class BackToMenuButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        // Get the Button component
        button = GetComponent<Button>();

        // Add listener
        button.onClick.AddListener(ReturnToMainMenu);
    }

    private void ReturnToMainMenu()
    {
        LevelManager.Instance.LoadNextLevel();
    }

}