using UnityEngine;
using System.Collections;

public class MacrophageAI : MonoBehaviour
{
    public float moveSpeed = 3f; // Speed at which the Macrophage moves
    public float detectionRange = 5f; // Range to detect enemies
    public float cooldownDuration = 2f; // Cooldown after catching an enemy
    public TentacleAI[] tentacles; // Array of tentacles
    public float yOffset = 1f; // Y-axis offset when moving towards the enemy


    private GameObject closestEnemy; // The closest enemy
    private bool isOnCooldown = false; // Cooldown state
    private bool isStretching = false; // Whether the Macrophage is currently stretching a tentacle

    void Update()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (isOnCooldown || isStretching || (collider != null && !collider.enabled)) return; // Skip logic if on cooldown, stretching, or collider is disabled

        // Find the closest enemy
        closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            // Move towards the closest enemy
            MoveTowardsEnemy();
            Debug.Log("Moving towards enemy!");

            // Check if any tentacle is in range to catch the enemy
            foreach (var tentacle in tentacles)
            {

                if (Vector2.Distance(tentacle.transform.position, closestEnemy.transform.position) <= detectionRange)
                {
                    Debug.Log("Catching enemy!");
                    StartCoroutine(CatchEnemy(tentacle));
                    break; // Stop checking other tentacles
                }
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ecoli");
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    void MoveTowardsEnemy()
    {
        if (closestEnemy == null) return;

        // Calculate the target position with the Y-axis offset
        Vector2 targetPosition = new Vector2(closestEnemy.transform.position.x, closestEnemy.transform.position.y + yOffset);
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }


    IEnumerator CatchEnemy(TentacleAI tentacle)
    {
        // Stop moving
        isStretching = true;

        // Stretch the tentacle to catch the enemy
        tentacle.Stretch();

        //Avoid race condition with other macrophages
        EcoliAI ecoliAI = closestEnemy.GetComponent<EcoliAI>();
        if (ecoliAI.getMovmentStatus())
        {
            ecoliAI.DisableMovement();
            // Catch the enemy
            StartCoroutine(tentacle.VacuumMicrobe(closestEnemy.GetComponent<Collider2D>()));

            // Wait for the tentacle to retract
            yield return new WaitUntil(() => !tentacle.IsStretching());

            // Enter cooldown
            isOnCooldown = true;
            yield return new WaitForSeconds(cooldownDuration);
            isOnCooldown = false;
        }
        // Resume movement
        isStretching = false;
    }
}