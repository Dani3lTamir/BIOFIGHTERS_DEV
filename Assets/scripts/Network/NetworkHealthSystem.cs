using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class NetworkHealthSystem : NetworkBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float Yoffset = 50f;

    [SerializeField] private GameObject healthBarPrefab;
    private GameObject healthBarInstance;
    private HealthBarController healthBarController;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Coroutine hideHealthBarCoroutine;

    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        currentHealth.OnValueChanged += HandleHealthChanged;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // UI: Local health bar setup
        if (IsOwner || IsClient)
        {
            if (healthBarPrefab != null)
            {
                healthBarInstance = Instantiate(healthBarPrefab, GameObject.Find("Canvas").transform);
                healthBarController = healthBarInstance.GetComponent<HealthBarController>();
                healthBarInstance.SetActive(false);
            }
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        currentHealth.OnValueChanged -= HandleHealthChanged;
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }

    private void Update()
    {
        if (NetworkEventManager.Instance.IsGameEnded() && healthBarInstance != null)
        {
            healthBarInstance.SetActive(false);
            return;
        }

        if (healthBarInstance != null && healthBarInstance.activeSelf)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            healthBarInstance.transform.position = screenPos + new Vector3(0, Yoffset, 0);
        }
    }

    public void TakeDamage(float amount)
    {
        if (NetworkEventManager.Instance.IsGameEnded())
        {
            return;
        }
        if (IsServer && NetworkEventManager.Instance != null && !NetworkEventManager.Instance.IsGameEnded())
        {
            currentHealth.Value = Mathf.Clamp(currentHealth.Value - amount, 0, maxHealth);
            if (currentHealth.Value <= 0)
                Die();
        }
    }

    private void HandleHealthChanged(float oldHealth, float newHealth)
    {
        if (IsClient && healthBarController != null)
        {
            if (!healthBarInstance.activeSelf && NetworkEventManager.Instance.IsGameEnded() == false)
                healthBarInstance.SetActive(true);

            healthBarController.UpdateHealthBar(newHealth, maxHealth);

            if (hideHealthBarCoroutine != null)
                StopCoroutine(hideHealthBarCoroutine);

            hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay(3f));
        }

        if (spriteRenderer != null)
        {
            StartCoroutine(BlinkRed());
        }
    }

    IEnumerator HideHealthBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (healthBarInstance != null)
            healthBarInstance.SetActive(false);
    }

    IEnumerator BlinkRed()
    {
        Color currentColor = spriteRenderer.color;
        spriteRenderer.color = new Color(1, 0, 0, currentColor.a);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        if (healthBarInstance != null)
            healthBarInstance.SetActive(false);

        if (gameObject.CompareTag("BodyCell"))
        {
            GameObject[] bodyCells = GameObject.FindGameObjectsWithTag("BodyCell");
            if (bodyCells.Length <= 1)
            {
                // Request pathogen win from server
                NetworkEventManager.Instance.RequestPathogenWinServerRpc();
            }
            else
            {
                // Reward Pathogen for killing a BodyCell
                NetworkRewardSystem.Instance.RegisterEnemyKillServerRpc("BodyCell");
            }
        }

        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn(true); // will return to pool if set up
        }
    }

    public float GetCurrentHealth() => currentHealth.Value;
    public void SetCurrentHealth(float value) => currentHealth.Value = value;
}
