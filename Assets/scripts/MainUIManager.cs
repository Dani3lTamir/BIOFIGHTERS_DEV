using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    public Timer timer;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coins;
    public Image powerUpIconImage; // Reference to the UI Image component for the power-up icon
    public static MainUIManager Instance;

    private void Awake()
    {
        // Initialize the Instance field
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        timerText.text = timer.FormatTime(timer.GetTimeRemaining());
        scoreText.text = "" + ScoreManager.Instance.GetScore();
        coins.text = "" + GameCountManager.Instance.GetCounterValue("Coins");

    }

}