using UnityEngine;

public class SpecialEnemySpawner : MonoBehaviour
{
    public EnemyPool enemyPool;  // Reference to the enemy pool
    public float spawnInterval = 2f;  // Time between spawns
    public float minX = -5f;  // Minimum X position
    public float maxX = 5f;  // Maximum X position
    public bool isStarted = false;
    public float scoreThreshold = 100f;
    public float slowedEnemySpeed = 1;

    private void Start()
    {
    }

    private void Update()
    {
        if (!isStarted && (ScoreManager.Instance.GetScore() >= scoreThreshold))
        {
            InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
            isStarted = true;

        }

        else if (isStarted && (ScoreManager.Instance.GetScore() < scoreThreshold))
        {
            CancelInvoke("SpawnEnemy");
            isStarted = false;
        }
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

        // Assign the selected color to the enemy
        Renderer enemyRenderer = enemy.GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
           // enemyRenderer.material.color = predefinedColors[randomIndex];
        }
    }


}
