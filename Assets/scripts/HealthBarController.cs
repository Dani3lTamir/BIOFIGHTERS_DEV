using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Image fillImage; // Reference to the health bar fill image
    public Slider slider; // Reference to the slider component

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        // Update the slider value
        slider.value = currentHealth / maxHealth;
        // Calculate the fill amount (0 to 1)s
        float fillAmount = currentHealth / maxHealth;
        fillImage.fillAmount = fillAmount;

       // Change color based on health
        if (fillAmount > 0.75f)
        {
            fillImage.color = Color.white;
        }
        else if (fillAmount > 0.3f)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }
    }
}