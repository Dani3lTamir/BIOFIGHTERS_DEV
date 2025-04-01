using UnityEngine;
using System.Collections.Generic;

public class BCellAI : MonoBehaviour
{
    [Header("Settings")]
    public float colliderRadius = 5f; // Radius within which the B cell can detect Enemies
    public float shootCooldown = 2f; // Time between antibody shots
    public int ammo = 5; // Number of antibodies the B cell can shoot before destroying itself
    public GameObject antibodyPrefab; // Prefab for the antibody

    private float cooldownTimer; // Timer to track cooldown

    void Start()
    {
        cooldownTimer = shootCooldown; // Initialize the cooldown timer
    }

    void Update()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null && !collider.enabled)
        {
            return; // Do nothing if collider is disabled
        }

        if (ammo <= 0)
        {
            // Destroy the B cell when it runs out of ammo
            Destroy(gameObject);
            return;
        }

        // Update the cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else
        {
            // Attempt to shoot an antibody if the cooldown is over
            ShootAntibody();
            cooldownTimer = shootCooldown; // Reset the cooldown timer
        }
    }

    void ShootAntibody()
    {
        // Find all Objects within the collider radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, colliderRadius);
        List<GameObject> eColiList = new List<GameObject>();
        List<GameObject> salmonelaList = new List<GameObject>();
        List<GameObject> TBList = new List<GameObject>();



        foreach (Collider2D collider in hitColliders)
        {
            // Filter out only the E. coli objects

            if (collider.CompareTag("Ecoli") && (collider.GetComponent<EcoliAI>().getMovmentStatus())) // Ensure your E. coli objects have the tag "EColi" and its not caught by other defender
            {
                eColiList.Add(collider.gameObject);
            }

            // Filter out only the Salmonela objects
            if (collider.CompareTag("Salmonela") && (collider.GetComponent<SalmonelaAI>().getMovmentStatus())) // Ensure your Salmonela objects have the tag "Salmonela" and its not caught by other defender
            {
                salmonelaList.Add(collider.gameObject);
            }

            // Filter out only the TB objects
            if (collider.CompareTag("Tuberculosis") && (collider.GetComponent<TBAI>().getMovmentStatus())) 
            {
                TBList.Add(collider.gameObject);
            }


        }

        // If there are Salmonemla in range, shoot at a random one
        if (salmonelaList.Count > 0)
        {
            int randomIndex = Random.Range(0, salmonelaList.Count);
            GameObject targetSalmonela = salmonelaList[randomIndex];

            // Get the antibody from the pool
            GameObject antibody = ObjectPool.Instance.SpawnFromPool("Antibody", transform.position, Quaternion.identity);

            // Set the antibody's target to the Salmonela
            if (antibody.TryGetComponent<AntibodyBehavior>(out AntibodyBehavior antibodyBehavior))
            {
                antibodyBehavior.SetTarget(targetSalmonela);
            }

            // Reduce ammo by 1
            ammo--;
        }

        else if (TBList.Count > 0)
        {
            int randomIndex = Random.Range(0, TBList.Count);
            GameObject targetTB = TBList[randomIndex];

            // Get the antibody from the pool
            GameObject antibody = ObjectPool.Instance.SpawnFromPool("Antibody", transform.position, Quaternion.identity);

            // Set the antibody's target to the Salmonela
            if (antibody.TryGetComponent<AntibodyBehavior>(out AntibodyBehavior antibodyBehavior))
            {
                antibodyBehavior.SetTarget(targetTB);
            }

            // Reduce ammo by 1
            ammo--;
        }


        // Else If there are E. coli in range, shoot at a random one
        else if (eColiList.Count > 0)
        {
            int randomIndex = Random.Range(0, eColiList.Count);
            GameObject targetEcoli = eColiList[randomIndex];

            // Get the antibody from the pool
            GameObject antibody = ObjectPool.Instance.SpawnFromPool("Antibody", transform.position, Quaternion.identity);

            // Set the antibody's target to the E. coli
            if (antibody.TryGetComponent<AntibodyBehavior>(out AntibodyBehavior antibodyBehavior))
            {
                antibodyBehavior.SetTarget(targetEcoli);
            }

            // Reduce ammo by 1
            ammo--;
        }
    }

    // Optional: Visualize the collider radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }
}