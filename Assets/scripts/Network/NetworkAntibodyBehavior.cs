using UnityEngine;
using Unity.Netcode;

public class NetworkAntibodyBehavior : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float damage = 7f;
    private NetworkObjectReference target;

    private NetworkObject resolvedTarget;

    void Update()
    {
        if (!IsServer) return;

        // Resolve the reference into a live object
        if (resolvedTarget == null && target.TryGet(out resolvedTarget) == false)
        {
            DespawnSelf();
            return;
        }

        if (resolvedTarget != null)
        {
            GameObject targetGO = resolvedTarget.gameObject;

            if (targetGO.CompareTag("Ecoli") && !targetGO.GetComponent<NetworkEcoliAI>().getMovmentStatus())
            {
                DespawnSelf();
                return;
            }

            Vector2 direction = (targetGO.transform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, targetGO.transform.position, moveSpeed * Time.deltaTime);
            RotateTowardsTarget(direction);

            if (Vector3.Distance(transform.position, targetGO.transform.position) < 0.1f)
            {
                if (targetGO.CompareTag("Ecoli") && targetGO.GetComponent<NetworkEcoliAI>().getMovmentStatus())
                {
                    targetGO.GetComponent<NetworkEcoliAI>().Die();
                }
                else if (targetGO.GetComponent<IBoss>() != null && targetGO.GetComponent<IBoss>().getMovmentStatus())
                {
                    targetGO.GetComponent<NetworkHealthSystem>().TakeDamage(damage);
                }

                DespawnSelf();
            }
        }
        else
        {
            DespawnSelf();
        }
    }

    void RotateTowardsTarget(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    public void SetTarget(NetworkObject targetNO)
    {
        if (targetNO != null)
        {
            target = targetNO;
            resolvedTarget = targetNO;
        }
    }

    void DespawnSelf()
    {
        if (IsSpawned) GetComponent<NetworkObject>().Despawn(true);
    }
}
