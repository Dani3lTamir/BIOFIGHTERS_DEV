using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float Yoffset = 50f; // Offset of the health bar above the object
    public float currentHealth; // Made internal for testing
    public GameObject healthBarPrefab; // Reference to the Health Bar prefab
    public int deathScorePenalty = 100; // Penalty score for death
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

            healthBarInstance = Instantiate(healthBarPrefab, GameObject.Find("Canvas").transform);
            healthBarController = healthBarInstance.GetComponent<HealthBarController>();

            // Hide the health bar initially
            healthBarInstance.SetActive(false);
        }

       
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
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
        if (healthBarInstance != null)
            healthBarInstance.SetActive(false);
    }

    IEnumerator BlinkRed()
    {
        if (spriteRenderer == null) yield break;

        // Store current color before blinking
        Color currentColor = spriteRenderer.color;

        // Change to red while preserving original alpha
        spriteRenderer.color = new Color(1, 0, 0, currentColor.a); // Red with existing alpha

        yield return new WaitForSeconds(0.1f);

        // Restore original RGB values while preserving current alpha
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentColor.a);
    }

    void Die()
    {
        // Handle death (e.g., play animation, destroy object, etc.)
        Debug.Log($"{gameObject.name} has died!");
        if (healthBarInstance != null)
            healthBarInstance.SetActive(false);
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(-deathScorePenalty);
        }
        // If the game object has BodyCell tag check if it is the last one
        if (gameObject.CompareTag("BodyCell"))
        {
            GameObject[] bodyCells = GameObject.FindGameObjectsWithTag("BodyCell");
            if (bodyCells.Length <= 1 && LevelManager.Instance != null)
            {
                LevelManager.Instance.LoseLevel();
            }
            else Debug.Log("There are still BodyCells alive");
        }
        if (this != null && gameObject != null)
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

    public GameObject GetHealthBarInstance()
    { return healthBarInstance; }
}