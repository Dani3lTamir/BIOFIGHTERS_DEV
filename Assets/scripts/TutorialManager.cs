using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Tutorial UI")]
    public GameObject tutorialPanel; // Panel to display tutorial messages
    public Text tutorialText; // Text component to display the message
    public Image tutorialImage; // Image component to display the image

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Show a tutorial message with an image
    public void ShowTutorial(string message, Sprite image)
    {
        if (tutorialPanel == null || tutorialText == null || tutorialImage == null)
        {
            Debug.LogWarning("Tutorial UI is not set up!");
            return;
        }

        // Set the tutorial message and image
        tutorialText.text = message;
        tutorialImage.sprite = image;

        // Activate the tutorial panel
        tutorialPanel.SetActive(true);

    }

    // Hide the tutorial
    void HideTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }
}