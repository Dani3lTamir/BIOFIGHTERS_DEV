using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    public Timer timer;
    public Timer startUpTimer;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coins;
    public Transform startUpPanel;
    public TextMeshProUGUI startUpNum;

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

        // Find EcoliWaveSpawner by its tag
        GameObject ecoliWaveSpawner = GameObject.FindGameObjectWithTag("EcoliWaveSpawner");
        if (ecoliWaveSpawner != null)
        {
            startUpTimer.countdownTime = ecoliWaveSpawner.GetComponent<WaveSpawner>().delayUntilFirstWave;
        }


    }

    private void Start()
    {

        // Initialize the startUpNum text
        startUpNum.text = startUpTimer.GetTimeRemaining().ToString("F2");
    }



    private void Update()
    {
        // Update the timer text
        timerText.text = timer.FormatTime(timer.GetTimeRemaining());
        scoreText.text = "" + ScoreManager.Instance.GetScore();
        coins.text = "" + RewardSystem.Instance.GetCoins();

        // Update the startUpNum text
        startUpNum.text = startUpTimer.GetTimeRemaining().ToString("F2");

        // When time is up, deactivate the startUpPanel
        if (startUpTimer.isTimeUp)
        {
            startUpPanel.gameObject.SetActive(false);
        }
    }

}