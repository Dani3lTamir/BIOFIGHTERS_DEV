using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MacrophagUIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI InfluenzaLeft;
    public TextMeshProUGUI AlliesLeft;
    public Image powerUpIconImage; // Reference to the UI Image component for the power-up icon

    private void Update()
    {
        scoreText.text = "" + ScoreManager.Instance.GetScore();
        InfluenzaLeft.text = "" + GameCountManager.Instance.GetCounterValue("InfluenzaLeft");
        AlliesLeft.text = "" + GameCountManager.Instance.GetCounterValue("AlliesLeft");
    }

    public void UpdatePowerUpIcon(Sprite icon)
    {
        if (icon != null)
        {
            powerUpIconImage.sprite = icon;
            powerUpIconImage.gameObject.SetActive(true); // Ensure the icon is visible
        }
        else
        {
            powerUpIconImage.gameObject.SetActive(false); // Hide the icon if no power-up is active
        }
    }
}