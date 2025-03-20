using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float Yoffset = 50f; // Offset of the health bar above the object
    private float currentHealth;

    public GameObject healthBarPrefab; // Reference to the Health Bar prefab
    private GameObject healthBarInstance; // Instance of the health bar
    private HealthBarController healthBarController; // Reference to the health bar controller

    private Coroutine hideHealthBarCoroutine; // Coroutine to hide the health bar
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private Color originalColor; // Store the original color of the sprite

    void Start()
    {
        currentHealth = maxHealth;

        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Store the original color
        }

        // Instantiate the health bar
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, FindFirstObjectByType<Canvas>().transform);
            healthBarController = healthBarInstance.GetComponent<HealthBarController>();

            // Hide the health bar initially
            healthBarInstance.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Show the health bar when damage is taken
        if (healthBarInstance != null && !healthBarInstance.activeSelf)
        {
            healthBarInstance.SetActive(true);
        }

        // Update the health bar
        if (healthBarController != null)
        {
            healthBarController.UpdateHealthBar(currentHealth, maxHealth);
        }

        // Start/restart the coroutine to hide the health bar after a delay
        if (hideHealthBarCoroutine != null)
        {
            StopCoroutine(hideHealthBarCoroutine);
        }
        hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay(3f)); // Hide after 3 seconds

        // Make the sprite blink red
        if (spriteRenderer != null)
        {
            StartCoroutine(BlinkRed());
        }

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator HideHealthBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        healthBarInstance.SetActive(false);
    }

    IEnumerator BlinkRed()
    {
        // Change the sprite color to red
        spriteRenderer.color = Color.red;

        // Wait for a short duration (e.g., 0.1 seconds)
        yield return new WaitForSeconds(0.1f);

        // Revert the sprite color back to the original color
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        // Handle death (e.g., play animation, destroy object, etc.)
        Debug.Log($"{gameObject.name} has died!");
        healthBarInstance.SetActive(false);
        // If the game object has BodyCell tag check if it is the last one
        if (gameObject.CompareTag("BodyCell"))
        {
            GameObject[] bodyCells = GameObject.FindGameObjectsWithTag("BodyCell");
            if (bodyCells.Length == 1)
            {
                // If it is the last one, call the LoseLevel method
                LevelManager.Instance.LoseLevel();
            }
            else Debug.Log("There are still BodyCells alive");
        }
        Destroy(gameObject);
    }

    void Update()
    {
        // Position the health bar above the Object
        if (healthBarInstance != null && healthBarInstance.activeSelf)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            healthBarInstance.transform.position = screenPosition + new Vector3(0, Yoffset, 0); // Adjust offset as needed
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}