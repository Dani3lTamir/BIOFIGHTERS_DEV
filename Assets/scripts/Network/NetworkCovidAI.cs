using UnityEngine;
using System.Collections;
using Unity.Netcode;
using Unity.BossRoom.Infrastructure;

public class NetworkCovidAI : NetworkBehaviour, IBoss
{
    public float moveSpeed = 2f;
    public float damageInterval = 1f;
    public float damagePerTick = 1f;
    public float damagePerTickMultiplier { get; set; } = 1f;
    public NetworkHealthSystem healthSystem;
    public int multiFactor = 2;
    public float duplicationWeakeningFactor = 3f;
    public int multiLimit = 13; // Maximum number of covids that can be created
    [SerializeField] private int covidPrefabIndex;
    [SerializeField] private Animator animator;
    private Transform targetCell;
    private bool isAttacking = false;
    private bool isMultiplying = false;
    private Vector3 randomTargetPosition;
    private bool isMovementEnabled = true;

    private readonly object movementLock = new object();


    void OnEnable()
    {
        animator.SetTrigger("Camo");
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        StopAllCoroutines();
        isAttacking = false;
        targetCell = null;
        gameObject.tag = "CamoCovid";
        NetworkRewardSystem.Instance.RegisterEnemyKillServerRpc("Covid");
    }

    void Update()
    {
        if (!IsServer) return; // Only the server should control the Covid's behavior
        if (!isMovementEnabled || isMultiplying) return;

        if (targetCell == null)
        {
            isAttacking = false;
            ChooseWeakestTarget();
            return;
        }

        if (!isAttacking)
        {
            MoveTowardsTarget();
        }
    }

    void ChooseWeakestTarget()
    {
        GameObject[] bodyCells = GameObject.FindGameObjectsWithTag("BodyCell");


        if (bodyCells.Length > 0)
        {
            float lowestHealth = Mathf.Infinity;
            GameObject weakestCell = null;
            foreach (GameObject cell in bodyCells)
            {
                NetworkHealthSystem cellHealth = cell.GetComponent<NetworkHealthSystem>();
                if (cellHealth != null && cellHealth.GetCurrentHealth() < lowestHealth)
                {
                    lowestHealth = cellHealth.GetCurrentHealth();
                    weakestCell = cell;
                }
            }
            targetCell = weakestCell.transform;
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
        transform.position = Vector3.MoveTowards(transform.position, randomTargetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, randomTargetPosition) < 0.1f)
        {
            StartCoroutine(AttackCell());
        }
    }

    IEnumerator AttackCell()
    {
        isAttacking = true;
        // Reveal camo when attacking
        DisableCamoClientRpc();

        Transform currentTarget = targetCell;
        bool cellWasDestroyed = false;

        while (currentTarget != null && Vector3.Distance(transform.position, randomTargetPosition) < 0.1f)
        {
            NetworkHealthSystem cellHealth = currentTarget.GetComponent<NetworkHealthSystem>();
            if (cellHealth != null)
            {
                float healthBefore = cellHealth.GetCurrentHealth();
                DamageCellServerRpc(currentTarget.GetComponent<NetworkObject>(), damagePerTick * damagePerTickMultiplier);

                if (healthBefore > 0 && (cellHealth == null || cellHealth.GetCurrentHealth() <= 0))
                {
                    cellWasDestroyed = true;
                    break;
                }
            }

            yield return new WaitForSeconds(damageInterval);

            if (currentTarget == null || targetCell != currentTarget)
            {
                if (currentTarget == null) cellWasDestroyed = true;
                currentTarget = targetCell;
            }
        }

        if (cellWasDestroyed)
        {
            isMultiplying = true;
            Debug.Log("Covid requesting multiplying!");
            RequestMultiplyServerRpc();
        }

        if (targetCell == currentTarget)
        {
            targetCell = null;
        }

        isAttacking = false;
        yield break;
    }

    [ServerRpc(RequireOwnership = false)]
    void DamageCellServerRpc(NetworkObjectReference cellRef, float damage)
    {
        if (cellRef.TryGet(out NetworkObject cellNetObj))
        {
            if (cellNetObj.TryGetComponent<NetworkHealthSystem>(out NetworkHealthSystem cellHealth))
            {
                cellHealth.TakeDamage(damage);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestMultiplyServerRpc()
    {
        StartCoroutine(Multiply());
    }

    IEnumerator Multiply()
    {
        GameObject[] covids = GameObject.FindGameObjectsWithTag("Covid");
        if (covids.Length >= multiLimit)
        {
            isMultiplying = false;
            yield break; // Limit reached, do not multiply
        }

        for (int i = 0; i < multiFactor; i++)
        {
            GameObject prefab = NetworkPrefabSpawner.Instance.GetPrefab(covidPrefabIndex);
            NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(prefab, transform.position, Quaternion.identity);
            obj.Spawn(true);

            obj.GetComponent<NetworkCovidAI>().DisableCamoClientRpc();

            NetworkHealthSystem cloneHealth = obj.GetComponent<NetworkHealthSystem>();
            if (cloneHealth != null)
            {
                cloneHealth.SetCurrentHealth(healthSystem.maxHealth / duplicationWeakeningFactor);
            }

            IBoss boss = obj.GetComponent<IBoss>();
            if (boss != null)
                boss.damagePerTickMultiplier = this.damagePerTickMultiplier / duplicationWeakeningFactor;

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


    [ClientRpc(RequireOwnership = false)]
    public void DisableCamoClientRpc()
    {
        DisableCamo();
    }

    public void DisableCamo()
    {
        animator.SetTrigger("DisableCamo");
        Debug.Log("Removing camo visuals from Covid.");
        gameObject.tag = "Covid";
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
