using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag; // Unique identifier for the pool
        public GameObject prefab; // Prefab to pool
        public int size; // Initial size of the pool
    }

    public static ObjectPool Instance; // Singleton instance for global access
    public List<Pool> pools; // List of pools to create
    private Dictionary<string, Queue<GameObject>> poolDictionary; // Dictionary to hold the pools

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize the pool dictionary
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Create the pools
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Instantiate and deactivate objects for the pool
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            // Add the pool to the dictionary
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // Spawn an object from the pool
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        // Check if the pool exists
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        // Get an object from the pool
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // Activate and position the object
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Re-add the object to the pool for reuse
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    // Return an object to the pool
    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        // Deactivate the object and return it to the pool
        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}