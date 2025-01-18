using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyPool enemyPool;  // Reference to the enemy pool
    public float spawnInterval = 2f;  // Time between spawns
    public float minX = -5f;  // Minimum X position
    public float maxX = 5f;  // Maximum X position

    private void Start()
    {
        // Start spawning enemies at regular intervals
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        // Get an enemy from the pool
        GameObject enemy = enemyPool.GetEnemy();

        // Randomly generate a position for the enemy on the X-axis
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, transform.position.y, transform.position.z);

        // Set the enemy's position
        enemy.transform.position = spawnPosition;

        // Define an array of predefined colors
        Color[] predefinedColors = {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.white
    };

        // Randomly pick one of the predefined colors
        int randomIndex = Random.Range(0, predefinedColors.Length);
        if (predefinedColors[randomIndex].Equals(Color.white)) enemy.tag = "WhiteEnemy";// notice a white enemy
        else enemy.tag = "Enemy";
        // Assign the selected color to the enemy
        Renderer enemyRenderer = enemy.GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = predefinedColors[randomIndex];
        }
    }
}
