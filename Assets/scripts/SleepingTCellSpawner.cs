using UnityEngine;
using System.Collections.Generic;

public class SleepingTCellSpawner : MonoBehaviour
{
    public GameObject tCellPrefab; // Prefab for the T cell
    public List<Transform> spawnLocations; // List of spawn locations
    public int numberOfTCells = 10; // Number of T cells to spawn
    public float antibodyYOffset = 1.0f; // Offset for the antibody's Y position
    public Vector3 antibodyScale = new Vector3(2.0f, 2.0f, 2.0f); // Scale for the antibody
    public bool isHideAntiBodyOnExit = false; // Flag to hide the antibody on exit
    private Color[] antigenColors = { Color.red, new Color(1.0f, 0.75f, 0.8f), Color.green, new Color(0.5f, 0.0f, 0.5f) }; // Array of colors (red, pink, green, purple)


    private Dictionary<SleepingTCell, AntibodyGenerator> tCellAntibodyMap = new Dictionary<SleepingTCell, AntibodyGenerator>(); // Dictionary to store the mapping

    void Start()
    {
        // Ensure there are enough spawn locations
        if (spawnLocations.Count < numberOfTCells)
        {
            Debug.LogWarning("Not enough spawn locations for the number of T cells.");
            numberOfTCells = spawnLocations.Count;
        }

        // Spawn T cells and set their antibodies
        for (int i = 0; i < numberOfTCells; i++)
        {
            // Get a random spawn location from the available locations
            int randomIndex = Random.Range(0, spawnLocations.Count);
            Transform spawnLocation = spawnLocations[randomIndex];

            // Remove the used spawn location from the list
            spawnLocations.RemoveAt(randomIndex);

            // Instantiate the T cell at the spawn location
            GameObject tCell = Instantiate(tCellPrefab, spawnLocation.position, spawnLocation.rotation);
            // Tag the T cell for identification
            tCell.tag = "SleepingTCell";
            // Set the hideAntiBodyOnExit flag
            tCell.GetComponent<SleepingTCell>().hideAntiBodyOnExit = isHideAntiBodyOnExit;
            // Create the Antibody using the AntibodyGenerator
            GameObject antibody = new GameObject("Antibody");
            Vector3 antibodyPosition = spawnLocation.position;
            antibodyPosition.y += antibodyYOffset; // Apply the Y offset
            antibody.transform.position = antibodyPosition;
            antibody.transform.rotation = spawnLocation.rotation;
            antibody.transform.localScale = antibodyScale; // Apply the scale

            AntibodyGenerator antibodyGenerator = antibody.AddComponent<AntibodyGenerator>();
            // Set the color randomly
            antibodyGenerator.shapeColor = antigenColors[Random.Range(0, antigenColors.Length)];

            // Initialize the antibody
            antibodyGenerator.Initialize();
            //check if it's unique
            List<AntibodyGenerator> antibodies = new List<AntibodyGenerator>(tCellAntibodyMap.Values);
            bool isUnique = false;
            while (!isUnique)
            {
                if (AntibodyGenerator.IsUniqueAntibody(antibodies, antibodyGenerator))
                {
                    isUnique = true;
                }
                else
                {
                    antibodyGenerator.Initialize();
                }
            }


            // Set the antibody reference in the SleepingTCell script
            SleepingTCell tCellScript = tCell.GetComponent<SleepingTCell>();
            tCellScript.antibody = antibody; // Assign the antibody GameObject

            // Add the T cell and its antibody to the dictionary
            tCellAntibodyMap[tCellScript] = antibodyGenerator;
        }

        // Initialize the antigen generator after all antibodies are created
        AntigenGenerator antigenGenerator = GetComponent<AntigenGenerator>();
        if (antigenGenerator != null)
        {
            antigenGenerator.GenerateAntigen(tCellAntibodyMap);
        }
        else
        {
            Debug.LogError("AntigenGenerator component not found!");
        }
    }
}