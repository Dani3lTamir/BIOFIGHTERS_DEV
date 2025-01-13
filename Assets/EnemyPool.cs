using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemyPrefab;  // Prefab for the enemy
    public int poolSize = 20;       // Number of enemies in the pool
    private List<GameObject> pool;

    void Start()
    {
        // Initialize the pool
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            pool.Add(enemy);
        }
    }

    public GameObject GetEnemy()
    {
        foreach (GameObject enemy in pool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        // If no inactive enemies are available, optionally expand the pool
        GameObject newEnemy = Instantiate(enemyPrefab);
        newEnemy.SetActive(false);
        pool.Add(newEnemy);
        return newEnemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
    }
}
