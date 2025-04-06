using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemyPrefab;  // Prefab for the enemy
    public int poolSize = 20;       // Number of enemies in the pool
    public float originalSpeed;
    private List<GameObject> pool;
    private Vector3 initialScale;
    private Color initialColor;


    void Awake()
    {
        // Initialize the pool
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            pool.Add(enemy);
        }

        // Instantiate a temporary enemy to get its initial properties
        GameObject tempEnemy = Instantiate(enemyPrefab);
        initialScale = tempEnemy.transform.localScale;
        SpriteRenderer spriteRenderer = tempEnemy.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            initialColor = spriteRenderer.color;
        }
        Destroy(tempEnemy);

    }

    public GameObject GetEnemy()
    {
        foreach (GameObject enemy in pool)
        {
            if (!enemy.activeInHierarchy && enemy.transform.parent == null)
            {
                // Reset the scale and color to their initial values
                enemy.transform.localScale = initialScale;
                SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = initialColor;
                }

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
        Debug.Log("Ecoli deactivated by EnemyPool");

        enemy.SetActive(false);
    }

    public void ChangeEnemiesSpeed(float speed)
    {
        originalSpeed = pool[0].GetComponent<Rigidbody2D>().gravityScale;// save enemies og speed
        foreach (GameObject enemy in pool)
        {
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            rb.gravityScale = speed; // Increases gravity effect
        }
    }

    public void ResetEnemiesSpeed()
    {
        foreach (GameObject enemy in pool)
        {
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            rb.gravityScale = originalSpeed;
        }
    }


}
