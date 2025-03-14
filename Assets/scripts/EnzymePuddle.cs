using UnityEngine;
using System.Collections;

public class EnzymePuddle : MonoBehaviour
{
    public float lifetime = 5f; // Lifetime of the puddle

    void Start()
    {
        // Destroy the puddle after its lifetime
        Destroy(gameObject, lifetime + 1f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ecoli"))
        {
            // Trap and kill the Ecoli
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

        // Parent the Ecoli to the puddle so it stays trapped
        ecoli.transform.SetParent(transform);

        // Kill the Ecoli when the puddle is destroyed
        StartCoroutine(KillEcoliAfterDelay(ecoli, lifetime));
    }

    IEnumerator KillEcoliAfterDelay(GameObject ecoli, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Unparent the Ecoli before destroying the puddle
        if (ecoli != null)
        {
            ecoli.transform.SetParent(null); // Unparent the Ecoli
            EcoliAI ecoliAI = ecoli.GetComponent<EcoliAI>();
            if (ecoliAI != null)
            {
                ecoliAI.Die(); // Kill the Ecoli
            }
        }
    }
}