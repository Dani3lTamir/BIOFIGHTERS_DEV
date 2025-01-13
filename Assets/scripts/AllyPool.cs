using System.Collections.Generic;
using UnityEngine;

public class AllyPool : MonoBehaviour
{
    public GameObject allyPrefab;  // Prefab for the ally
    public int poolSize = 20;       // Number of allies in the pool
    private List<GameObject> pool;

    void Start()
    {
        // Initialize the pool
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject ally = Instantiate(allyPrefab);
            ally.SetActive(false);
            pool.Add(ally);
        }
    }

    public GameObject GetAlly()
    {
        foreach (GameObject ally in pool)
        {
            if (!ally.activeInHierarchy)
            {
                ally.SetActive(true);
                return ally;
            }
        }

        // If no inactive allies are available, optionally expand the pool
        GameObject newAlly = Instantiate(allyPrefab);
        newAlly.SetActive(false);
        pool.Add(newAlly);
        return newAlly;
    }

    public void ReturnAlly(GameObject ally)
    {
        ally.SetActive(false);
    }
}
