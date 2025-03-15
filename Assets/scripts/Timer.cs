using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float countdownTime = 60f; // Duration of the timer in seconds
    public bool isTimeUp { get; private set; } // Boolean to indicate if the time is up
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

