using UnityEngine;
using System.Collections.Generic;

public class GameCountManager : MonoBehaviour
{
    public static GameCountManager Instance; // Singleton instance

    private Dictionary<string, int> counters; // Maps counter names to their values

    [System.Serializable]
    public class CounterDefinition // To make counters configurable in the Inspector
    {
        public string CounterName; // Name of the counter (e.g., "EnemiesDefeated")
        public int InitialValue;  // Initial value for the counter
    }

    public CounterDefinition[] initialCounters; // Array to define counters in the Inspector

    private void Awake()
    {
        // Ensure there's only one instance of the GameCountManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps the manager across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize the counters dictionary
        counters = new Dictionary<string, int>();
        foreach (var counter in initialCounters)
        {
            counters[counter.CounterName] = counter.InitialValue;
        }
    }

    // Update a counter by name
    public void UpdateCounter(string counterName, int amount)
    {
        if (counters.ContainsKey(counterName))
        {
            counters[counterName] += amount;
        }
        else
        {
            counters[counterName] = amount;
        }
    }


    // Get the value of a counter by name
    public int GetCounterValue(string counterName)
    {
        return counters.ContainsKey(counterName) ? counters[counterName] : 0;
    }

    // Reset a counter to its initial value
    public void ResetCounter(string counterName)
    {
        foreach (var counter in initialCounters)
        {
            if (counter.CounterName == counterName)
            {
                counters[counterName] = counter.InitialValue;
                return;
            }
        }
    }
}
