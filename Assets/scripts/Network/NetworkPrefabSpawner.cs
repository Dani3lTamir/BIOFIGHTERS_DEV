using UnityEngine;
using Unity.Netcode;
using Unity.BossRoom.Infrastructure;


public class NetworkPrefabSpawner : NetworkBehaviour
{
    public static NetworkPrefabSpawner Instance;

    [SerializeField] private GameObject[] spawnablePrefabs; // Assign in Inspector

    private int selectedPrefabIndex = -1;
    private int selectedCost;
    private GameObject preview;
    private bool isDragging = false;
    private bool isPathogen = false;

    [SerializeField] private Vector2 fixedSpawnPosition = new Vector2(1.5f, 15f);

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!IsOwner) return;

        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            preview.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);

            if (Input.GetMouseButtonDown(0))
            {
                SpawnPrefab(mousePosition);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelDragging();
            }
        }
    }

    public void SetSelectedPrefab(int prefabIndex, int cost, bool pathogen)
    {
        if (prefabIndex < 0 || prefabIndex >= spawnablePrefabs.Length)
        {
            Debug.LogError("[NetworkPrefabSpawner] Invalid prefab index.");
            return;
        }

        selectedPrefabIndex = prefabIndex;
        selectedCost = cost;
        isPathogen = pathogen;

        if (!isPathogen)
        {
            preview = Instantiate(spawnablePrefabs[prefabIndex]);
            preview.GetComponent<Collider2D>().enabled = false;
            isDragging = true;
        }
        else
        {
            Vector2 spawnPosition = GetFixedSpawnPosition();
            SpawnPrefabServerRpc(prefabIndex, spawnPosition);
            DeductCoinsServerRpc(selectedCost, isPathogen);
        }
    }

    void CancelDragging()
    {
        if (preview) Destroy(preview);
        isDragging = false;
    }

    Vector2 GetFixedSpawnPosition()
    {
        return fixedSpawnPosition;
    }

    void SpawnPrefab(Vector2 position)
    {
        if (selectedPrefabIndex < 0 || selectedPrefabIndex >= spawnablePrefabs.Length)
        {
            Debug.LogError("[NetworkPrefabSpawner] Invalid prefab index at spawn.");
            return;
        }

        SpawnPrefabServerRpc(selectedPrefabIndex, position);

        if (preview) Destroy(preview);
        isDragging = false;

        NetworkRewardSystem.Instance.DeductCoins(selectedCost, isPathogen);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnPrefabServerRpc(int prefabIndex, Vector2 position)
    {
        if (prefabIndex < 0 || prefabIndex >= spawnablePrefabs.Length)
        {
            Debug.LogError("[ServerRpc] Invalid prefab index!");
            return;
        }

        GameObject prefab = spawnablePrefabs[prefabIndex];
        NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(prefab, position, Quaternion.identity);
        obj.Spawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    void DeductCoinsServerRpc(int selectedCost, bool isPathogen)
    {
        if (isPathogen)
        {
            NetworkRewardSystem.Instance.DeductCoins(selectedCost, isPathogen);
        }
    }
}
