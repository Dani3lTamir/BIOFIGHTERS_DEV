using UnityEngine;
using System.Collections;

public class MacrophageAI : MonoBehaviour
{
    public float moveSpeed = 3f; // Speed at which the Macrophage moves
    public float detectionRange = 5f; // Range to detect enemies
    public float eColiCooldownDuration = 2f; // Cooldown after catching an ecoli
    public float damage = 2f;        // Damage caused by the tentacle
    public float cooldownDuration = 7f; // Cooldown after catching other enemies
    public float yOffset = 1f; // Y-axis offset when moving towards the enemy
    public int catchLimit = 10; // Maximum number of enemies that can be caught
    public float deathDelay = 1f; // Delay before dying after reaching the catch limit
    public TentacleAI[] tentacles; // Array of tentacles

    private GameObject closestEnemy; // The closest enemy
    private bool isOnCooldown = false; // Cooldown state
    private bool isStretching = false; // Whether the Macrophage is currently stretching a tentacle
    private int caughtEnemiesCount = 0; // Counter for caught enemies

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
        GameObject[] ecoliEnemies = GameObject.FindGameObjectsWithTag("Ecoli");
        GameObject[] salmonellaEnemies = GameObject.FindGameObjectsWithTag("Salmonela");


        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in ecoliEnemies)
        {
            // only if the enemy is not already caught by another defender
            if (!(enemy.GetComponent<EcoliAI>().getMovmentStatus())) continue;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemy;
            }
        }

        foreach (var enemy in salmonellaEnemies)
        {
            // only if the enemy is not already caught by another defender
            if (!(enemy.GetComponent<SalmonelaAI>().getMovmentStatus())) continue;
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

        // Avoid race condition with other macrophages
        if (closestEnemy.CompareTag("Ecoli"))
        {
            EcoliAI ecoliAI = closestEnemy.GetComponent<EcoliAI>();
            if (ecoliAI.getMovmentStatus())
            {
                ecoliAI.DisableMovement();
                // Catch the enemy
                StartCoroutine(tentacle.VacuumMicrobe(closestEnemy.GetComponent<Collider2D>()));

                // Wait for the tentacle to retract
                yield return new WaitUntil(() => !tentacle.IsStretching());

                // Increment the counter for caught enemies
                caughtEnemiesCount++;
            }
        }

        if (closestEnemy.CompareTag("Salmonela"))
        {

            SalmonelaAI salmonelaAI = closestEnemy.GetComponent<SalmonelaAI>();
            if (salmonelaAI.getMovmentStatus())
            {
                salmonelaAI.DisableMovement();
                // Catch the enemy
                StartCoroutine(tentacle.VacuumMicrobe(closestEnemy.GetComponent<Collider2D>()));

                // Wait for the tentacle to retract
                yield return new WaitUntil(() => !tentacle.IsStretching());

                // Give damage to Salmonela
                closestEnemy.GetComponent<HealthSystem>().TakeDamage(damage);

                // if the enemy is destroyed, activate Eat animation
                if (closestEnemy == null)
                {
                    Animator mpAnimator = GetComponent<Animator>();
                    if (mpAnimator != null)
                    {
                        mpAnimator.SetTrigger("Eat");
                    }
                }

                // Increment the counter for dead enemies
                caughtEnemiesCount++;
            }

        }

        // Check if the catch limit is reached
        if (caughtEnemiesCount >= catchLimit)
        {
            // Add a short delay before dying
            yield return new WaitForSeconds(deathDelay);
            Die();
            yield break; // Exit the coroutine
        }


        // Enter cooldown
        float coolDown = closestEnemy.CompareTag("Ecoli") ? eColiCooldownDuration : cooldownDuration;
        isOnCooldown = true;
        yield return new WaitForSeconds(coolDown);
        isOnCooldown = false;


        // Resume movement
        isStretching = false;
    }

    void Die()
    {
        // Handle the death of the Macrophage (e.g., play animation, destroy object, etc.)
        Debug.Log("Macrophage has reached the catch limit and is dying.");
        Destroy(gameObject);
    }
}