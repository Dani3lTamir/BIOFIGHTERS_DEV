using UnityEngine;
using System.Collections;

public class EcoliAI : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed at which the Ecoli moves
    public float damageInterval = 1f; // Time between damage ticks
    public float damagePerTick = 1f; // Damage caused per tick
    public Animator animator; // Reference to the Animator component
    private Transform targetCell; // The body cell the Ecoli is targeting
    private bool isAttacking = false;
    private Vector3 randomTargetPosition; // Random position inside the cell collider
    public bool isMovementEnabled = true; // Whether movement is enabled
    private readonly object movementLock = new object(); // Lock object for atomic operations
    private readonly object deathLock = new object(); // Lock object for atomic operations

    void OnEnable()
    {
        EnableMovement();
        // Initialize the Ecoli when it is activated
        ChooseRandomTarget();
    }

    void OnDisable()
    {
        // Clean up when the Ecoli is deactivated
        StopAllCoroutines(); // Stop any running coroutines
        isAttacking = false;
        targetCell = null;
    }

    void Update()
    {
        if (!isMovementEnabled) return; // Skip movement logic if movement is disabled

        if (targetCell == null)
        {
            // If the target is destroyed, stop attacking and return to Idle
            if (isAttacking)
            {
                StopAttacking();
            }

            // Choose a new target
            ChooseRandomTarget();
            return;
        }

        if (!isAttacking)
        {
            // Move towards the target body cell
            MoveTowardsTarget();
        }
    }

    void ChooseRandomTarget()
    {
        // Get all body cells in the scene
        GameObject[] bodyCells = GameObject.FindGameObjectsWithTag("BodyCell");

        if (bodyCells.Length > 0)
        {
            // Choose a random body cell from the list
            targetCell = bodyCells[Random.Range(0, bodyCells.Length)].transform;
            // Generate a random position inside the cell collider
            GenerateRandomTargetPosition();
        }
    }

    void GenerateRandomTargetPosition()
    {
        if (targetCell.TryGetComponent<Collider2D>(out Collider2D collider))
        {
            Bounds bounds = collider.bounds;
            randomTargetPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                transform.position.z
            );
        }
        else
        {
            Debug.LogWarning("Target cell does not have a Collider2D component!");
        }
    }

    void MoveTowardsTarget()
    {
        // Move towards the random position inside the target body cell
        transform.position = Vector3.MoveTowards(transform.position, randomTargetPosition, moveSpeed * Time.deltaTime);

        // Check if the Ecoli has reached the random position inside the target body cell
        if (Vector3.Distance(transform.position, randomTargetPosition) < 0.1f)
        {
            StartCoroutine(AttackCell());
        }
    }

    IEnumerator AttackCell()
    {
        isAttacking = true;

        while (targetCell != null && (Vector3.Distance(transform.position, randomTargetPosition) < 0.1f) && getMovmentStatus())
        {
            // Damage the target body cell
            if (targetCell.TryGetComponent<HealthSystem>(out HealthSystem cellHealth))
            {
                cellHealth.TakeDamage(damagePerTick);
                // Show the attack animation
                animator.SetTrigger("Attack");
            }

            // Wait for the next damage tick
            yield return new WaitForSeconds(damageInterval);
        }

        // If the target is destroyed or the Ecoli stops attacking, return to Idle
        StopAttacking();
    }

    void StopAttacking()
    {
        isAttacking = false;
        animator.ResetTrigger("Attack"); // Reset the Attack trigger
        animator.SetTrigger("Idle"); // Set the Idle trigger
    }

    public void DisableMovement()
    {
        lock (movementLock)
        {
            Debug.Log("Disabling movement");
            animator.ResetTrigger("Attack"); // Reset the Attack trigger
            animator.SetTrigger("Idle");
            isMovementEnabled = false;
        }
    }

    public void EnableMovement()
    {
        lock (movementLock)
        {
            isMovementEnabled = true;
        }
    }

    public bool getMovmentStatus()
    {
        return isMovementEnabled;
    }

    public void setTargetCell(Transform newTarget)
    {
        targetCell = newTarget;
    }

    public Transform getTargetCell()
    {
        return targetCell;
    }


    public void Die()
    {
        lock (deathLock)
        {
            // Handle Ecoli capture (e.g., update score, deactivate Ecoli)
            GameCountManager.Instance.UpdateCounter("EcoliKilled", 1); // Update Ecoli counter
            ScoreManager.Instance.UpdateScoreForObject(gameObject.tag); // Update score for given object
            RewardSystem.Instance.RegisterEnemyKill(gameObject.tag); // Register the kill for reward purposes
            EnableMovement();
            gameObject.SetActive(false);
        }
    }
}