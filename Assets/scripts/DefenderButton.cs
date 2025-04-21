using UnityEngine;
using UnityEngine.UI;

public class DefenderButton : MonoBehaviour
{
    public GameObject defenderPrefab; // The defender prefab to spawn
    public CommanderUI commanderUI; // Reference to the commander UI
    public int defnderLimit = 200; // The limit of this kind of defender

    private Button button;

    private AudioManager audioManager; // Reference to the AudioManager

    void Start()
    {
        button = GetComponent<Button>();
        audioManager = AudioManager.Instance; // Get the AudioManager instance
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
            audioManager.Play("WrongAnswer"); // Play button press sound when selecting defender
            RectTransform rectTransform = GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("םיבאשמ ןיא", rectTransform, Color.yellow);
            return;
        }

        // if there are more than the limit of this kind of defender, throw message and dont spawn
        GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag(defenderPrefab.tag);
        if (gameObjectsWithTag.Length >= defnderLimit)
        {
            audioManager.Play("WrongAnswer"); // Play button press sound when selecting defender
            RectTransform rectTransform = GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("הלבגמל תעגה", rectTransform, Color.yellow);
            return;
        }


        // Pass the selected defender prefab and cost to the spawner
        DefenderSpawner.Instance.SetSelectedDefender(defenderPrefab, defenderCost);
    }
}