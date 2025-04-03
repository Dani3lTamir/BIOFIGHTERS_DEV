using UnityEngine;

public class AllySpawner : MonoBehaviour
{
    public AllyPool allyPool;  // Reference to the ally pool
    public float spawnInterval = 2f;  // Time between spawns
    public float minX = -5f;  // Minimum X position
    public float maxX = 5f;  // Maximum X position
    public float startUpDelay = 5f; // Time before spwaning starts


    private void Start()
    {
        // Start spawning allies at regular intervals
        InvokeRepeating("SpawnAlly", startUpDelay, spawnInterval);
    }

    void SpawnAlly()
    {
        // Get an ally from the pool
        GameObject ally = allyPool.GetAlly();

        // Randomly generate a position for the ally on the X-axis
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, transform.position.y, transform.position.z);

        // Set the ally's position
        ally.transform.position = spawnPosition;

        // Define an array of predefined colors
        Color[] predefinedColors = {
        Color.gray,
        Color.yellow,
        Color.white
    };

        // Randomly pick one of the predefined colors
        int randomIndex = Random.Range(0, predefinedColors.Length);
        if (predefinedColors[randomIndex].Equals(Color.yellow)) ally.tag = "YellowAlly";// notice a yellow ally
        else ally.tag = "Ally";
        // Assign the selected color to the ally
        Renderer allyRenderer = ally.GetComponent<Renderer>();
        if (allyRenderer != null)
        {
            allyRenderer.material.color = predefinedColors[randomIndex];
        }
    }
}
