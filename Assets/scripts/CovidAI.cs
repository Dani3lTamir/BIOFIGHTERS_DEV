using UnityEngine;
using System.Collections;

public class CovidAI : MonoBehaviour, IBoss
{
    public float moveSpeed = 2f; // Speed at which the Ecoli moves
    public float damageInterval = 1f; // Time between damage ticks
    public float damagePerTick { get; set; } = 1f; // Damage caused per tick
    public float damagePerTickMultiplier { get; set; } = 1f; // Multiplier for damage per tick
    public HealthSystem healthSystem; // Reference to the HealthSystem component
    public int multiFactor = 2; // To how many duplicates a Covid multiply
    public bool isCamo = false; // Is the Covid camoflaged
    public GameObject covidPrefab;
    private Transform targetCell; // The body cell the Ecoli is targeting
    private bool isAttacking = false;
    private bool isMultiplying = false;

    private Vector3 randomTargetPosition; // Random position inside the cell collider
    private bool isMovementEnabled = true; // Whether movement is enabled

    private readonly object movementLock = new object(); // Lock object for atomic operations



    void OnEnable()
    {

        // Initialize the Covid when it is activated
        EnableCamo();
        ChooseWeakestTarget();

    }

    void OnDisable()
    {
        // Clean up when the Covid is deactivated
        StopAllCoroutines(); // Stop any running coroutines
        isAttacking = false;
        targetCell = null;
    }

    void Update()
    {
        if (!isMovementEnabled || isMultiplying) return; // Skip movement logic if movement is disabled or Covid is multiplying
        if (targetCell == null)
        {
            isAttacking = false;
            // If the target is destroyed, choose a new one
            ChooseWeakestTarget();
            return;
        }

        if (!isAttacking)
        {
            // Move towards the target body cell
            MoveTowardsTarget();
        }
    }

    void ChooseWeakestTarget()
    {
        // Get all body cells in the scene
        GameObject[] bodyCells = GameObject.FindGameObjectsWithTag("BodyCell");

        if (bodyCells.Length > 0)
        {
            //Choose a body cell with the lowest current health in its Health System Component
            float lowestHealth = Mathf.Infinity;
            GameObject weakestCell = null;
            foreach (GameObject cell in bodyCells)
            {
                // Get the HealthSystem component
                HealthSystem healthSystem = cell.GetComponent<HealthSystem>();

                if (healthSystem != null && healthSystem.currentHealth < lowestHealth)
                {
                    lowestHealth = healthSystem.currentHealth;
                    weakestCell = cell;
                }
            }
            targetCell = weakestCell.transform;

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

        // Reveal camo when attacking
        if (isCamo) DisableCamo();

        // Store initial target reference
        Transform currentTarget = targetCell;
        bool cellWasDestroyed = false;

        // Attack while we have a valid target and are in range
        while (currentTarget != null &&
               Vector3.Distance(transform.position, randomTargetPosition) < 0.1f)
        {
            HealthSystem cellHealth = currentTarget.GetComponent<HealthSystem>();
            if (cellHealth != null)
            {
                // Store health before attacking
                float healthBefore = cellHealth.GetCurrentHealth();
                cellHealth.TakeDamage(damagePerTick * damagePerTickMultiplier);

                // Check if cell was destroyed (object might be null now)
                if (healthBefore > 0 && (cellHealth == null || cellHealth.GetCurrentHealth() <= 0))
                {
                    cellWasDestroyed = true;
                    break;
                }
            }

            yield return new WaitForSeconds(damageInterval);

            // Update reference - the GameObject might have been destroyed
            if (currentTarget == null || targetCell != currentTarget)
            {
                // Exit if our target was destroyed
                if (currentTarget == null) cellWasDestroyed = true;
                currentTarget = targetCell;
            }
        }

        // Multiply if our target cell was destroyed
        if (cellWasDestroyed)
        {
            isMultiplying = true;
            StartCoroutine(Multiply());
        }

        // Only clear target if it's still the one we were attacking
        if (targetCell == currentTarget)
        {
            targetCell = null;
        }

        isAttacking = false;
        yield break;
    }

    IEnumerator Multiply()
    {
        // Spawn more Covids with delay between each spawn
        for (int i = 0; i < multiFactor; i++)
        {
            // Instantiate an object
            GameObject obj = Instantiate(covidPrefab);
            //Disable Camo for Clone
            obj.GetComponent<CovidAI>().DisableCamo();
            // Set the object's position
            obj.transform.position = transform.position;
            // Find the clones HealthSystem component and set the health
            HealthSystem cloneHealth = obj.GetComponent<HealthSystem>();
            if (cloneHealth != null)
            {
                cloneHealth.maxHealth = healthSystem.maxHealth;
                cloneHealth.currentHealth = cloneHealth.maxHealth;
            }

            // Find the objects IBoss implemnting component and set the damage
            IBoss boss = obj.GetComponent<IBoss>();
            if (boss != null)
                boss.damagePerTickMultiplier = this.damagePerTickMultiplier;

            // Wait for the specified delay before spawning the next object
            yield return new WaitForSeconds(1f);
        }
        isMultiplying = false;
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

    public void EnableCamo()
    {
        isCamo = true;
        // Change tag to CamoCovid
        gameObject.tag = "CamoCovid";
        // Make the sprite translucent
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = 0.5f; // 50% opacity (adjust as needed)
            spriteRenderer.color = spriteColor;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found for camo effect!");
        }
    }

    public void DisableCamo()
    {
        isCamo = false;
        // Change tag to just Covid
        gameObject.tag = "Covid";
        // Make the sprite visible
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = 1f; 
            spriteRenderer.color = spriteColor;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found for camo effect!");
        }
    }

    void OnDestroy()
    {
        // Handle Covid death (e.g., update score, deactivate Covid)
        GameCountManager.Instance.UpdateCounter("CovidKilled", 1); // Update Covid counter
        ScoreManager.Instance.UpdateScoreForObject("Covid"); // Update score for given object
        RewardSystem.Instance.RegisterEnemyKill("Covid"); // Register the kill for reward purposes
    }

}