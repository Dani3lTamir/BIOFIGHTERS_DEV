using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.BossRoom.Infrastructure;

public class NetworkBCellAI : NetworkBehaviour
{
    [Header("Settings")]
    public float colliderRadius = 5f; // Radius within which the B cell can detect Enemies
    public float shootCooldown = 3f; // Time between antibody shots

    [SerializeField] private int maxAmmo = 20; 
    private int ammo; // Number of antibodies the B cell have left

    [SerializeField] private int antiBodyPrefabIndex; // Index for the antibody prefab in the network pool

    private float cooldownTimer; // Timer to track cooldown

    void OnEnable()
    {
        cooldownTimer = shootCooldown; // Initialize the cooldown timer
        ammo = maxAmmo; // Initialize ammo to max ammo
    }


    void Update()
    {
        if (!IsServer) return; // Only the server should control the B cell's behavior
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null && !collider.enabled)
        {
            return; // Do nothing if collider is disabled
        }

        if (ammo <= 0)
        {
            // Despawn the B cell when it runs out of ammo
            GetComponent<NetworkObject>().Despawn(true);
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
        List<GameObject> bossList = new List<GameObject>();



        foreach (Collider2D collider in hitColliders)
        {
            // Filter out only the E. coli objects
            if (collider.CompareTag("Ecoli") && (collider.GetComponent<NetworkEcoliAI>().getMovmentStatus())) // Ensure your E. coli objects have the tag "EColi" and its not caught by other defender
            {
                eColiList.Add(collider.gameObject);
            }
            IBoss bossAI = collider.GetComponent<IBoss>();
            // Filter out only the Boss objects
            if ((bossAI != null) && (bossAI.getMovmentStatus()) && !(collider.CompareTag("CamoCovid"))) // Ensure your Boss objects is not caught by other defender
            {
                bossList.Add(collider.gameObject);
            }
        }

        GameObject target = null;

        // If there are Bosses in range, Choose it as the target
        if (bossList.Count > 0)
        {
            int randomIndex = Random.Range(0, bossList.Count);
            target = bossList[randomIndex];

        }

        else if (eColiList.Count > 0)
        {
            // If there are E. coli in range, Choose it as the target
            int randomIndex = Random.Range(0, eColiList.Count);
            target = eColiList[randomIndex];
        }
        else
        {
            // If no targets are found, return early
            return;
        }

        NetworkObject targetNetObj = target.GetComponent<NetworkObject>();


        // Get the antibody from the pool
        GameObject prefab = NetworkPrefabSpawner.Instance.GetPrefab(antiBodyPrefabIndex);
        NetworkObject antibody = NetworkObjectPool.Singleton.GetNetworkObject(prefab, transform.position, Quaternion.identity);

        // Set the antibody's target by a network object reference and spawn it
        if (antibody.TryGetComponent<NetworkAntibodyBehavior>(out NetworkAntibodyBehavior antibodyBehavior))
        {
            antibodyBehavior.SetTarget(targetNetObj);
            antibody.Spawn(true);
        }
        // Reduce ammo by 1
        ammo--;

    }

    //  Visualize the collider radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }
}