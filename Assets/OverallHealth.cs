using UnityEngine;
using System.Collections;

public class OverallHealth : MonoBehaviour
{
    private float maxHealth;
    private float currentHealth;

    private HealthBarController healthBarController; // Reference to the health bar controller

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        // Wait for the end of the frame to ensure other Start methods have executed
        yield return new WaitForEndOfFrame();

        healthBarController = gameObject.GetComponent<HealthBarController>();
        maxHealth = CalculateOverallHealth();
        currentHealth = maxHealth;
        Debug.Log("Overall health: " + maxHealth);
    }


    float CalculateOverallHealth()
    {
        // Calculate overall health based on individual cell health
        float overallHealth = 0;
        // Find all body cells in the scene by tag
        GameObject[] bodyCells = GameObject.FindGameObjectsWithTag("BodyCell");
        foreach (GameObject cell in bodyCells)
        {
            // Get the health component of each body cell
            HealthSystem healthSystem = cell.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                // Add the health of each cell to the overall health
                overallHealth += healthSystem.GetCurrentHealth();
            }
        }
        return overallHealth;
    }

    void Lose()
    {
        // Handle death (e.g., play animation, destroy object, etc.)
        Debug.Log("you lose");
        Destroy(gameObject);
    }

    void Update()
    {
        currentHealth = CalculateOverallHealth();
        // Update the health bar
        if (healthBarController != null)
        {
            healthBarController.UpdateHealthBar(currentHealth, maxHealth);
        }
    }
}

