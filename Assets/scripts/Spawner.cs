using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject prefab; // prefab to spawn
    public float spawnInterval = 20f;  // Time between spawns
    public int waveSize = 5;  // Number of enemies in a wave
    public float waveSizeMultiplier = 1.5f;  // Multiplier for the wave size
    public Vector3 spawnPosition;  // Position to spawn the wave
    public float spawnDelay = 0.5f;  // Delay between each object spawn
    public float delayUntilFirstWave = 5f;  // Delay before the first wave


    private void Start()
    {
        // Start spawning objects at regular intervals
        InvokeRepeating("StartSpawningWave", delayUntilFirstWave, spawnInterval);
    }

    void StartSpawningWave()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        // Spawn a wave of objects with a delay between each spawn
        for (int i = 0; i < waveSize; i++)
        {
            // Instantiate an object
            GameObject obj = Instantiate(prefab);
            // Set the object's position
            obj.transform.position = spawnPosition;

            // Wait for the specified delay before spawning the next object
            yield return new WaitForSeconds(spawnDelay);
        }
        // Multiply the wave size for the next wave
        waveSize = (int)(waveSize * waveSizeMultiplier);
    }
}