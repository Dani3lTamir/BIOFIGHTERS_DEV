using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Shop : MonoBehaviour
{
    public static Shop Instance; // Singleton instance

    [System.Serializable]
    public class DefenderButtonPrice
    {
        public DefenderButton defenderButton;
        public int price;
        public TextMeshProUGUI priceText; // Reference to the TextMeshProUGUI component for the price
    }

    public List<DefenderButtonPrice> defenderButtonPrices; // List of defender buttons and their prices

    private void Awake()
    {
        // Ensure there's only one instance of the ScoreManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // Initialize the defender buttons with their prices
        foreach (var item in defenderButtonPrices)
        {
            item.priceText.text = item.price.ToString(); // Set the price text
        }
    }


    // Method to get the price of a defender button
    public int GetDefenderPrice(DefenderButton defenderButton)
    {
        foreach (var item in defenderButtonPrices)
        {
            if (item.defenderButton == defenderButton)
            {
                return item.price;
            }
        }
        return 0;
    }
}


