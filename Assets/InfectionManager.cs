using UnityEngine;

public class InfectionManager : MonoBehaviour
{
    public float delayUntilFirstInfected = 60f;  // Delay before the first infected
    public float afterInfectionDelay = 10f;  // Delay after infection
    private int infectedCounter = 0;  // Number of infected
    public int maxInfected = 2;  // Maximum number of infected


    void Start()
    {
        // Start the infection process after a delay
        InvokeRepeating("Infect", delayUntilFirstInfected, afterInfectionDelay);
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
}
