using UnityEngine;
using UnityEngine.UI;

public class DefenderButton : MonoBehaviour
{
    public GameObject defenderPrefab; // The defender prefab to spawn
    public CommanderUI commanderUI; // Reference to the commander UI

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectDefender);
    }

    void SelectDefender()
    {
        // Notify the commander UI to move to this button's position
        commanderUI.MoveToButton(transform.position.x);
        // Get the defender cost from the shop
        int defenderCost = Shop.Instance.GetDefenderPrice(this);
        // if the player can't afford the defender, return
        if (GameCountManager.Instance.GetCounterValue("Coins") < defenderCost)
        {
            Debug.Log("Not enough coins!");
            return;
        }
        // Pass the selected defender prefab and cost to the spawner
        DefenderSpawner.Instance.SetSelectedDefender(defenderPrefab, defenderCost);
    }
}