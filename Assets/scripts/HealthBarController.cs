using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Image fillImage; // Reference to the health bar fill image

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        // Calculate the fill amount (0 to 1)
        float fillAmount = currentHealth / maxHealth;
        fillImage.fillAmount = fillAmount;

       // Change color based on health
        if (fillAmount > 0.5f)
        {
            fillImage.color = Color.white;
        }
        else if (fillAmount > 0.25f)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }
    }
}