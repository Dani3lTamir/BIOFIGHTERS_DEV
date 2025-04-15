using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class NetworkNeutrophilAI : NetworkBehaviour
{
    public float moveSpeed = 2f; // Speed at which the Neutrophil moves
    public int maxPuddles = 5; // Maximum number of puddles to spawn
    public int enzymePuddlePrefabIndex; // Prefab Index for the enzyme puddle
    public int finalPuddlePrefabIndex; // Prefab Index for the final larger puddle
    public Animator animator; // Reference to the Animator component

    private int puddlesSpawned = 0; // Number of puddles spawned so far

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return; // Only run on the server
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null && !collider.enabled)
        {
            return; // Do nothing if collider is disabled
        }
        StartCoroutine(MoveAndSpawnPuddles());
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        StopAllCoroutines(); // Stop all coroutines when the object is despawned
        puddlesSpawned = 0; // Reset puddles spawned count
    }

    IEnumerator MoveAndSpawnPuddles()
    {
        if (!IsServer) yield break; // Only run on the server

        while (puddlesSpawned < maxPuddles)
        {
            // Move to a random position within the screen bounds
            Vector2 randomPosition = GetRandomScreenPosition();
            yield return StartCoroutine(MoveToPosition(randomPosition));

            // Play the spawn animation
            animator.SetTrigger("Puddle");

            yield return new WaitForSeconds(0.5f);
            // Spawn an enzyme puddle
            SpawnPuddle(randomPosition, enzymePuddlePrefabIndex);
            puddlesSpawned++;

        }

        // Move to a final random position
        Vector2 finalPosition = GetRandomScreenPosition();
        yield return StartCoroutine(MoveToPosition(finalPosition));

        // Spawn the final larger puddle
        SpawnPuddle(finalPosition, finalPuddlePrefabIndex);

        // Return to pool after spawning the final puddle
        if (IsSpawned) GetComponent<NetworkObject>().Despawn(true);
    }

    IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        if (!IsServer) yield break; // Only run on the server

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void SpawnPuddle(Vector2 position, int puddlePrefabIndex)
    {
        if (!IsServer) return; // Only run on the server
        NetworkPrefabSpawner.Instance.SpawnPrefabServerRpc(puddlePrefabIndex, position);
    }

    Vector2 GetRandomScreenPosition()
    {
        // Get random position within the screen bounds
        Vector2 viewportPosition = new Vector2(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f));
        return Camera.main.ViewportToWorldPoint(viewportPosition);
    }
}