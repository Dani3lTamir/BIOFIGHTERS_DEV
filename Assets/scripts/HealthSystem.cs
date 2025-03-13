using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public GameObject healthBarPrefab; // Reference to the Health Bar prefab
    private GameObject healthBarInstance; // Instance of the health bar
    private HealthBarController healthBarController; // Reference to the health bar controller

    private Coroutine hideHealthBarCoroutine; // Coroutine to hide the health bar

    void Start()
    {
        currentHealth = maxHealth;

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

    void Die()
    {
        // Handle death (e.g., play animation, destroy object, etc.)
        Debug.Log($"{gameObject.name} has died!");
        healthBarInstance.SetActive(false);
        Destroy(gameObject); 
    }

    void Update()
    {
        // Position the health bar above the body cell
        if (healthBarInstance != null && healthBarInstance.activeSelf)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            healthBarInstance.transform.position = screenPosition + new Vector3(0, 50, 0); // Adjust offset as needed
        }
    }
}