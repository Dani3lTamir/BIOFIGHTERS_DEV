using UnityEngine;
using System.Collections;


public class InfectionManager : MonoBehaviour
{
    public float delayUntilFirstInfected = 60f;  // Delay before the first infected
    public float afterInfectionDelay = 10f;  // Delay after infection
    private int infectedCounter = 0;  // Number of infected
    public int maxInfected = 2;  // Maximum number of infected
    public float UntilInfectionCheck = 5f;  // Delay before checking for infection counter
    public GameObject mpPrefab;  // Reference to the Macrophage prefab
    public Vector3 mpSpawnLocation;  // Spawn location of the Macrophage


    void Start()
    {
        // Start the infection coroutine
        InvokeRepeating("Infect", delayUntilFirstInfected, afterInfectionDelay);
        // Check the infection counter in a coroutine
        StartCoroutine(InfectionCheck());
    }

    void Infect()
    {
        // If the maximum number of infected has been reached, return
        if (infectedCounter >= maxInfected)
            return;
        // Search for all MacrophageAI scripts in the scene
        MacrophageAI[] macrophages = FindObjectsOfType<MacrophageAI>();
        // If there are no macrophages in the scene, return
        if (macrophages.Length == 0)
            return;
        else
        {
            // Infect a random macrophage
            macrophages[Random.Range(0, macrophages.Length)].Infect();
            // Increment the infected counter
            infectedCounter++;
        }
    }

    IEnumerator InfectionCheck()
    {
        // Infection check delay
        yield return new WaitForSeconds(UntilInfectionCheck);
        // If the maximum number of infected has been reached, return
        if (infectedCounter >= maxInfected)
            yield break;
        // Otherwise Spawn MPs as the number of infections left to max
        int infectedLeft = (maxInfected - infectedCounter);
        for (int i = 0; i < infectedLeft; i++)
        {
            GameObject mp = Instantiate(mpPrefab);
            mp.transform.position = mpSpawnLocation;
            // short delay between spawns
            yield return new WaitForSeconds(2f);
        }
    }
}
