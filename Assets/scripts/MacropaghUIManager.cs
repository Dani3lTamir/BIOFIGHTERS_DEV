using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MacrophagUIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI SalmonelaLeft;
    public TextMeshProUGUI AlliesLeft;
    public SpriteRenderer powerUpIconImage; // Reference to the UI Image component for the power-up icon
    public Timer timer;

    private void Update()
    {
        // Update the timer text
        timerText.text = timer.FormatTime(timer.GetTimeRemaining());

        scoreText.text = "" + ScoreManager.Instance.GetScore();
        SalmonelaLeft.text = "" + GameCountManager.Instance.GetCounterValue("SalmonelaLeft");
        AlliesLeft.text = "" + GameCountManager.Instance.GetCounterValue("AlliesLeft");
    }

    public void UpdatePowerUpIcon(Sprite icon)
    {
                powerUpIconImage.sprite = icon;
     
    }
}