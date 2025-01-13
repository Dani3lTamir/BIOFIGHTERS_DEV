using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton instance

    private int score = 0;
    public TextMeshProUGUI scoreText; // Reference to the TextMeshPro UI element

    private void Awake()
    {
        // Ensure there's only one instance of the ScoreManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps the score manager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI(); // Initialize the score UI
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = ""+score;
        }
        else
        {
            Debug.LogError("ScoreText is not assigned in the Inspector!");
        }
    }
}
