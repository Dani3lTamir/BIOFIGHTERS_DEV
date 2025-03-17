using UnityEngine;

public class AntibodyBehavior : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the antibody moves
    public float damage = 7f; // Damage caused by the antibody
    private GameObject target; // The Enemy the antibody is targeting


    void Update()
    {
        if (target != null)
        {
            // Calculate the direction to the target
            Vector2 direction = (target.transform.position - transform.position).normalized;

            // Move toward the target
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);

            // Rotate the antibody to face the target
            RotateTowardsTarget(direction);


            // Check if the antibody has reached the target
            if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            {
                // If its an uncaught E. coli, kill it
                if (target.CompareTag("Ecoli") && target.GetComponent<EcoliAI>().getMovmentStatus())
                {
                    target.GetComponent<EcoliAI>().Die();
                }

                // If its a Salmonela, damage it
                if (target.CompareTag("Salmonela") && target.GetComponent<SalmonelaAI>().getMovmentStatus())
                {
                    target.GetComponent<HealthSystem>().TakeDamage(damage);
                }


                // return the antibody to the pool
                ObjectPool.Instance.ReturnToPool("Antibody", gameObject);
            }
        }
        else
        {
            // If the target is destroyed (e.g., by another antibody), return the antibody to the pool
            ObjectPool.Instance.ReturnToPool("Antibody", gameObject);
        }
    }

    void RotateTowardsTarget(Vector2 direction)
    {
        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the rotation to the antibody
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // Set the target E. coli
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
}