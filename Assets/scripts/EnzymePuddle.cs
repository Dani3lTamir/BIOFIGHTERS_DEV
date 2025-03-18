using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnzymePuddle : MonoBehaviour
{
    public float lifetime = 5f; // Lifetime of the puddle
    private List<GameObject> trappedEcoli = new List<GameObject>(); // List of trapped Enemies
    public int maxCatches = 5; // Maximum number of Enemies that can be caught
    public float damage = 1f; // Damage caused by the puddle

    void Start()
    {
        // Destroy the puddle after its lifetime
        StartCoroutine(DestroyPuddleAfterDelay(lifetime));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //only damge incase of salmonela
        if (other.CompareTag("Salmonela"))
        {
            // Damage the Salmonela
            HealthSystem healthSystem = other.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
            }
        }

        // Check if the puddle has reached its maximum capacity
        if (trappedEcoli.Count >= maxCatches)
        {
            return;
        }

        if (other.CompareTag("Ecoli"))
        {
            // Trap and disable the Ecoli's movement
            TrapEcoli(other.gameObject);
        }
    }

    void TrapEcoli(GameObject ecoli)
    {
        // Disable the Ecoli's movement
        EcoliAI ecoliAI = ecoli.GetComponent<EcoliAI>();
        if (ecoliAI != null && ecoliAI.getMovmentStatus())
        {
            ecoliAI.DisableMovement();
        }

        // Add the Ecoli to the list of trapped Ecoli
        trappedEcoli.Add(ecoli);
    }

    IEnumerator DestroyPuddleAfterDelay(float delay)
    {

        yield return new WaitForSeconds(delay);

        // Kill all trapped Ecoli
        foreach (var ecoli in trappedEcoli)
        {
            if (ecoli != null)
            {
                EcoliAI ecoliAI = ecoli.GetComponent<EcoliAI>();
                if (ecoliAI != null)
                {
                    ecoliAI.Die(); // Kill the Ecoli
                }
            }
        }

        // Clear the list of trapped Ecoli
        trappedEcoli.Clear();

        // Destroy the puddle
        Destroy(gameObject);
    }
}