using UnityEngine;

public class NetworkPathogenSpawner : MonoBehaviour
{
    public static NetworkPathogenSpawner Instance; // Singleton instance

    private GameObject selectedPathogen; // The currently selected pathogen prefab
    private int selectedPathogenCost; // The cost of the selected pathogen


    void Awake()
    {
        // Set up the singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
    }

}