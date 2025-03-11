using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float countdownTime = 60f; // Duration of the timer in seconds
    public bool isTimeUp { get; private set; } // Boolean to indicate if the time is up
    public static Timer Instance; // Singleton instance
    private float timeRemaining;

    private void Awake()
    {
        // Ensure there's only one instance of the Timer
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
            if (timeRemaining < 0)
            {
                timeRemaining = 0;
                isTimeUp = true;
                OnTimerEnd();
            }
        }
    }

    private void OnTimerEnd()
    {
        // Handle what happens when the timer reaches zero
        Debug.Log("Timer has ended!");
        // Add any additional logic here
    }

    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
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
    }

    public void AddTime(float time)
    {
        timeRemaining += time;
    }
}

