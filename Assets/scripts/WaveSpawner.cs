using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public EnemyPool enemyPool;  // Reference to the enemy pool
    public float spawnInterval = 20f;  // Time between spawns
    public int waveSize = 5;  // Number of enemies in a wave
    public float waveSizeMultiplier = 1.5f;  // Multiplier for the wave size
    public Vector3 spawnPosition;  // Position to spawn the wave
    public float spawnDelay = 0.5f;  // Delay between each enemy spawn
    public float delayUntilFirstWave = 5f;  // Delay before the first wave


    private void Start()
    {
        // Start spawning enemies at regular intervals
        InvokeRepeating("StartSpawningWave", delayUntilFirstWave, spawnInterval);
    }

    void StartSpawningWave()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        // Spawn a wave of enemies with a delay between each spawn
        for (int i = 0; i < waveSize; i++)
        {
            // Get an enemy from the pool
            GameObject enemy = enemyPool.GetEnemy();
            // Set the enemy's position
            enemy.transform.position = spawnPosition;

            // Wait for the specified delay before spawning the next enemy
            yield return new WaitForSeconds(spawnDelay);
        }
        // Multiply the wave size for the next wave
        waveSize = (int)(waveSize * waveSizeMultiplier);
    }
}