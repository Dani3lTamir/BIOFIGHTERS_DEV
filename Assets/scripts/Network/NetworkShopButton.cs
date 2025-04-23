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

    public static NetworkShopButton lastClickedButton;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectPrefab);
        priceText.text = prefabCost.ToString(); // Set the price text
    }

    void SelectPrefab()
    {
        AudioManager.Instance.Play("ButtonPress"); // Play click sound
        lastClickedButton = this; // Store the last clicked button
        // Notify the commander UI to move to this button's position
        commanderUI.MoveToButton(transform.position.x);
        // Pass the selected prefab Index and cost to the spawner
        NetworkPrefabSpawner.Instance.TryBuyPrefabServerRpc(prefabIndex, prefabCost, isPathogen, spawnLimit);
    }
}