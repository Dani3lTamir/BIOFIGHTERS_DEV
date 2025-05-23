using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float countdownTime = 60f; // Duration of the timer in seconds
    public bool isTimeUp = false; // Boolean to indicate if the time is up
    public bool timerEndWin = false; // Boolean to indicate if the timer ending results in a win
    public bool inSeconds = false; // Boolean to indicate if the timer is in seconds
    private bool last10Seconds = false; // Boolean to indicate if the last 10 seconds are reached
    public TextMeshProUGUI timerText;

    private float timeRemaining;



    private void Start()
    {
        // Initialize the timer
        timeRemaining = countdownTime;
        isTimeUp = false;
        Debug.Log("Timer initialized with timeRemaining: " + timeRemaining);
    }

    private void Update()
    {
        // Update the timer
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timerText == null)
            {
                return;
            }
            if (!inSeconds)
            {
                timerText.text = FormatTime(timeRemaining);
            }
            else
            {
                timerText.text = timeRemaining.ToString("F0");
            }

            if (timeRemaining <= 10 && !last10Seconds && CompareTag("LevelTimer"))
            {
                last10Seconds = true;
                AudioManager.Instance.Play("Clock"); // Play the last 10 seconds sound
            }
        }

        else if (!isTimeUp)
        {
            timeRemaining = 0;
            isTimeUp = true;
            OnTimerEnd();
        }
    }

    private void OnTimerEnd()
    {
        if (CompareTag("LevelTimer"))
        {
            AudioManager.Instance.Stop("Clock"); // Stop the clock sound
            if (timerEndWin)
            {
                LevelManager.Instance.WinLevel();
            }
            else
            {
                LevelManager.Instance.LoseLevel();
            }
        }
    }

    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        if (time > 0)
        {
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            return "00:00";
        }
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public float GetRoundedTimeRemaining()
    {
        return Mathf.Ceil(timeRemaining);
    }

    public void SetTime(float time)
    {
        timeRemaining = time;

        if (timeRemaining < 0)
        {
            timeRemaining = 0;
        }

    }

    public void AddTime(float time)
    {
        timeRemaining += time;
        RectTransform rectTransform = timerText.GetComponent<RectTransform>();
        if (FloatingTextManager.Instance != null)
        {
            FloatingTextManager.Instance.ShowFloatingText("" + time, rectTransform, Color.red, false);
        }
        if (timeRemaining < 0)
        {
            timeRemaining = 0;
        }
    }
}

