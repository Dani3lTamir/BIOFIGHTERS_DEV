using UnityEngine;
using System.Collections;

public class EcoliAI : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed at which the Ecoli moves
    public float damageInterval = 1f; // Time between damage ticks
    public int damagePerTick = 1; // Damage caused per tick

    private Transform targetCell; // The body cell the Ecoli is targeting
    private bool isAttacking = false;

    void OnEnable()
    {
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
        if (targetCell == null)
        {
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
        }
        else
        {
            Debug.LogWarning("No body cells found!");
        }
    }

    void MoveTowardsTarget()
    {
        // Move towards the target body cell
        transform.position = Vector3.MoveTowards(transform.position, targetCell.position, moveSpeed * Time.deltaTime);

        // Check if the Ecoli has reached the target body cell
        if (Vector3.Distance(transform.position, targetCell.position) < 0.1f)
        {
            StartCoroutine(AttackCell());
        }
    }

    IEnumerator AttackCell()
    {
        isAttacking = true;

        while (targetCell != null)
        {
            // Damage the target body cell
            if (targetCell.TryGetComponent<HealthSystem>(out HealthSystem cellHealth))
            {
                cellHealth.TakeDamage(damagePerTick);
            }

            // Wait for the next damage tick
            yield return new WaitForSeconds(damageInterval);
        }

        // If the target is destroyed, stop attacking
        isAttacking = false;
    }
}