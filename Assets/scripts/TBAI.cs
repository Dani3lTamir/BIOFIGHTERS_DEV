using UnityEngine;
using System.Collections;

public class TBAI : MonoBehaviour, IBoss
{
    public float moveSpeed = 2f; // Speed at which the Ecoli moves
    public float damageInterval = 1f; // Time between damage ticks
    public float damagePerTick { get; set; } = 1f; // Damage caused per tick
    public float damagePerTickMultiplier { get; set; } = 1f; // Multiplier for damage per tick
    private Transform targetCell; // The body cell the Ecoli is targeting
    private bool isAttacking = false;
    private Vector3 randomTargetPosition; // Random position inside the cell collider
    private bool isMovementEnabled = true; // Whether movement is enabled

    public HealthSystem healthSystem; // Reference to the HealthSystem component
    private readonly object movementLock = new object(); // Lock object for atomic operations



    void OnEnable()
    {
        AudioManager.Instance.PlayAt("Puke", transform); // Play spawn sound
        // Initialize the TB when it is activated
        if (targetCell == null)
            ChooseRandomTarget();
    }

    void OnDisable()
    {
        // Clean up when the TB is deactivated
        StopAllCoroutines(); // Stop any running coroutines
        isAttacking = false;
        targetCell = null;
    }

    void Update()
    {
        if (!isMovementEnabled) return; // Skip movement logic if movement is disabled
        if (targetCell == null)
        {
            isAttacking = false;
            // If the target is destroyed, choose a new one
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
        else
        {
            Debug.Log("You Lost!");
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

        while (targetCell != null && (Vector3.Distance(transform.position, randomTargetPosition) < 0.1f))
        {
            // Damage the target body cell
            if (targetCell.TryGetComponent<HealthSystem>(out HealthSystem cellHealth))
            {
                cellHealth.TakeDamage(damagePerTick * damagePerTickMultiplier);
            }

            // Wait for the next damage tick
            yield return new WaitForSeconds(damageInterval);
        }
        // make sure to reset the target cell and stop attacking
        targetCell = null;

        // If the target is destroyed, stop attacking
        isAttacking = false;
    }

    public void DisableMovement()
    {
        lock (movementLock)
        {
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

    void OnDestroy()
    {
        // Handle TB death (e.g., update score, deactivate TB)
        GameCountManager.Instance.UpdateCounter("TBKilled", 1); // Update TB counter
        ScoreManager.Instance.UpdateScoreForObject("Tuberculosis"); // Update score for given object
        RewardSystem.Instance.RegisterEnemyKill("Tuberculosis"); // Register the kill for reward purposes
    }

}