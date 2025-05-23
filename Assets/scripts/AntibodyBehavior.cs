using UnityEngine;

public class AntibodyBehavior : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the antibody moves
    public float damage = 7f; // Damage caused by the antibody
    private GameObject target; // The Enemy the antibody is targeting


    private void OnEnable()
    {
        AudioManager.Instance.PlayAt("Arrow", transform); // Play the sound effect for the antibody
    }

    void Update()
    {
        if (target != null)
        {
            // if its a caught Ecoli, return the antibody to the pool
            if (target.CompareTag("Ecoli") && !target.GetComponent<EcoliAI>().getMovmentStatus())
            {
                ObjectPool.Instance.ReturnToPool("Antibody", gameObject);
                return;
            }
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
                    // Play the sound effect for killing the E. coli
                    AudioManager.Instance.Play("Squish");
                    target.GetComponent<EcoliAI>().Die();
                }

                // Otherwise if its a Boss, damage it
                if ((target.GetComponent<IBoss>() != null) && (target.GetComponent<IBoss>().getMovmentStatus()))
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