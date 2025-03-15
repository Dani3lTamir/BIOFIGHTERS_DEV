using UnityEngine;

public class DefenderSpawner : MonoBehaviour
{
    public static DefenderSpawner Instance; // Singleton instance

    private GameObject selectedDefender; // The currently selected defender prefab
    private int selectedDefenderCost; // The cost of the selected defender

    private GameObject defenderPreview; // Preview of the defender being dragged
    private bool isDragging = false; // Whether the player is currently dragging a defender

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
        if (isDragging)
        {
            // Follow the mouse position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            defenderPreview.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);

            // Place the defender when the player clicks the mouse button
            if (Input.GetMouseButtonDown(0))
            {
                PlaceDefender(mousePosition);
            }
            // Cancel dragging when the player right-clicks
            else if (Input.GetMouseButtonDown(1))
            {
                Destroy(defenderPreview);
                isDragging = false;
            }
        }
    }

    public void SetSelectedDefender(GameObject defenderPrefab, int cost)
    {
        // Set the selected defender and its cost
        selectedDefender = defenderPrefab;
        selectedDefenderCost = cost;

        // Start dragging a preview of the defender
        StartDragging();
    }

    void StartDragging()
    {
        // Instantiate a preview of the defender
        defenderPreview = Instantiate(selectedDefender);
        defenderPreview.GetComponent<Collider2D>().enabled = false; // Disable collisions for the preview
        isDragging = true;
    }

    void PlaceDefender(Vector2 position)
    {
        // Instantiate the actual defender at the placement position
        GameObject placedDefender = Instantiate(selectedDefender, position, Quaternion.identity);

        // Clean up the preview
        Destroy(defenderPreview);
        isDragging = false;

        // Deduct the defender's cost from the player's coins
        RewardSystem.Instance.DeductCoins(selectedDefenderCost);
    }
}