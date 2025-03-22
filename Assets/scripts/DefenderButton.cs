using UnityEngine;
using UnityEngine.UI;

public class DefenderButton : MonoBehaviour
{
    public GameObject defenderPrefab; // The defender prefab to spawn
    public CommanderUI commanderUI; // Reference to the commander UI
    public int defnderLimit = 200; // The limit of this kind of defender

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
        if (RewardSystem.Instance.GetCoins() < defenderCost)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("����� ��� ���������", rectTransform, Color.yellow);
            return;
        }

        // if there are more than the limit of this kind of defender, throw message and dont spawn
        GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag(defenderPrefab.tag);
        if (gameObjectsWithTag.Length >= defnderLimit)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("������ ����", rectTransform, Color.yellow);
            return;
        }


        // Pass the selected defender prefab and cost to the spawner
        DefenderSpawner.Instance.SetSelectedDefender(defenderPrefab, defenderCost);
    }
}