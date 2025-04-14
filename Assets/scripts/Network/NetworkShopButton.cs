using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkShopButton : MonoBehaviour
{
    [SerializeField] private GameObject prefab; // The prefab to spawn
    [SerializeField] private int prefabIndex; // The Index of the prefab as assigned in the Spawner in the inspector 

    [SerializeField] private CommanderUI commanderUI; // Reference to the commander UI
    [SerializeField] private int spawnLimit = 200; // The limit of this kind of prefab
    [SerializeField] private bool isPathogen = false; // Is it a pathogen shop button or a Immune shop button
    [SerializeField] private int prefabCost = 0; // The cost of the prefab
    [SerializeField] private TextMeshProUGUI priceText; // Reference to the price text

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectPrefab);
        priceText.text = prefabCost.ToString(); // Set the price text
    }

    void SelectPrefab()
    {
        // Notify the commander UI to move to this button's position
        commanderUI.MoveToButton(transform.position.x);
        // if the player can't afford the buy, return
        if (NetworkRewardSystem.Instance.GetCoins(isPathogen) < prefabCost)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("אין מספיק משאבים", rectTransform, Color.yellow);
            return;
        }

        // if there are more than the limit of this kind of prefab, throw message and dont spawn
        GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag(prefab.tag);
        if (gameObjectsWithTag.Length >= spawnLimit)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("הגעת למגבלה", rectTransform, Color.yellow);
            return;
        }

        if (prefab == null)
        {
            Debug.LogError("[shop button]Prefab is not assigned in the inspector.");
        }

        // Pass the selected prefab Index and cost to the spawner
        NetworkPrefabSpawner.Instance.SetSelectedPrefab(prefabIndex, prefabCost, isPathogen);
    }
}