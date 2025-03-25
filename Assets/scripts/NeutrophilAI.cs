using UnityEngine;
using System.Collections;

public class NeutrophilAI : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed at which the Neutrophil moves
    public int maxPuddles = 5; // Maximum number of puddles to spawn
    public GameObject enzymePuddlePrefab; // Prefab for the enzyme puddle
    public GameObject finalPuddlePrefab; // Prefab for the final larger puddle
    public Animator animator; // Reference to the Animator component

    private int puddlesSpawned = 0; // Number of puddles spawned so far

    void Start()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null && !collider.enabled)
        {
            return; // Do nothing if collider is disabled
        }
        StartCoroutine(MoveAndSpawnPuddles());
    }

    IEnumerator MoveAndSpawnPuddles()
    {
        while (puddlesSpawned < maxPuddles)
        {
            // Move to a random position within the screen bounds
            Vector2 randomPosition = GetRandomScreenPosition();
            yield return StartCoroutine(MoveToPosition(randomPosition));

            // Play the spawn animation
            animator.SetTrigger("Puddle");

            yield return new WaitForSeconds(0.5f);
            // Spawn an enzyme puddle
            SpawnPuddle(randomPosition, enzymePuddlePrefab);
            puddlesSpawned++;

        }

        // Move to a final random position
        Vector2 finalPosition = GetRandomScreenPosition();
        yield return StartCoroutine(MoveToPosition(finalPosition));

        // Spawn the final larger puddle
        SpawnPuddle(finalPosition, finalPuddlePrefab);

        // Die after spawning the final puddle
        Destroy(gameObject);
    }

    IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void SpawnPuddle(Vector2 position, GameObject puddlePrefab)
    {
        GameObject puddle = Instantiate(puddlePrefab, position, Quaternion.identity);
    }

    Vector2 GetRandomScreenPosition()
    {
        // Get random position within the screen bounds
        Vector2 viewportPosition = new Vector2(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f));
        return Camera.main.ViewportToWorldPoint(viewportPosition);
    }
}