using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class NetworkOverallHealth : NetworkBehaviour
{
    // Network variable to store the current health and max health
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );
    private NetworkVariable<float> maxHealth = new NetworkVariable<float>(
    readPerm: NetworkVariableReadPermission.Everyone,
    writePerm: NetworkVariableWritePermission.Server
);

    public NetworkVariable<bool> isReady = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private HealthBarController healthBarController; // Reference to the health bar controller

    private bool dying = false;



    void Awake()
    {
        // Initialize the health bar controller
        healthBarController = GetComponent<HealthBarController>();
        if (healthBarController == null)
        {
            Debug.LogError("HealthBarController not found on the GameObject.");
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return; // Only run on the server
        StartCoroutine(DelayedStart());
    }

    void Update()
    {
        if (!isReady.Value) return; // Skip update if not ready
        if (IsServer)
        {
            currentHealth.Value = CalculateOverallHealth();
        }
        // Update the health bar for all clients
        // Check if the overall health is below a certain threshold
        if (currentHealth.Value <= (maxHealth.Value / 4f) && !dying)
        {
            dying = true;
            AudioManager.Instance.Play("LowHealth"); // Play the Heart sound
        }

        healthBarController.UpdateHealthBar(currentHealth.Value, maxHealth.Value);

    }


    IEnumerator DelayedStart()
    {
        // Wait for the end of the frame to ensure other Start methods have executed
        yield return new WaitForEndOfFrame();

        maxHealth.Value = CalculateOverallHealth();
        currentHealth.Value = maxHealth.Value;
        Debug.Log("Overall health: " + maxHealth.Value);
        isReady.Value = true; // Set the object as ready after initialization
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
            NetworkHealthSystem healthSystem = cell.GetComponent<NetworkHealthSystem>();
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
        if (!IsServer) return; // Only run on the server
        // Handle death (e.g., play animation, destroy object, etc.)
        Debug.Log("you lose");
        GetComponent<NetworkObject>().Despawn(); // Despawn the object
    }

}

