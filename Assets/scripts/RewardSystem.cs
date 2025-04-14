using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;



public class RewardSystem : MonoBehaviour
{
    [System.Serializable]
    public class EnemyReward
    {
        public string enemyType; // Type of enemy (e.g., "Ecoli", "Salmonella")
        public int killsRequired; // Number of kills required to earn the reward
        public int rewardAmount; // Coins earned when the reward is triggered
    }

    public static RewardSystem Instance; // Singleton instance

    // List of enemy rewards (customizable in the Inspector)
    public List<EnemyReward> enemyRewards = new List<EnemyReward>();

    // Dictionary to track kills for each enemy type
    private Dictionary<string, int> enemyKillCounts = new Dictionary<string, int>();

    // Coins the player starts with
    public int startingCoins = 50;

    // Coins earned by the player
    private int coins;
    // Text to display the current coin count
    public TextMeshProUGUI coinsText;

    // Reward multiplier
    public float rewardMultiplier = 1f; // Multiplier for the reward amount

    // Fields for periodic coin reward
    public int periodicCoinAmount = 10; // Amount of coins to add periodically
    public float periodicInterval = 30f; // Interval in seconds to add coins


    private void Awake()
    {
        // Ensure there's only one instance of the RewardSystem
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        coins = startingCoins;
    }

    private void Start()
    {
        // Start the coroutine to add coins periodically
        StartCoroutine(AddCoinsPeriodically());
    }

    private void Update()
    {
        // Update the coin text
        coinsText.text = coins.ToString();
    }

    // Coroutine to add coins periodically
    private IEnumerator AddCoinsPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(periodicInterval);
            int amount = (int)(periodicCoinAmount * rewardMultiplier);
            AddCoins(amount);
            RectTransform rectTransform = coinsText.GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("+"+ amount, rectTransform, Color.green);

            Debug.Log("Periodic reward: " + amount + " coins. Total coins: " + coins);
        }
    }


    // Method to register an enemy kill
    public void RegisterEnemyKill(string enemyType)
    {
        // Increment the kill count for the enemy type
        if (enemyKillCounts.ContainsKey(enemyType))
        {
            enemyKillCounts[enemyType]++;
        }
        else
        {
            enemyKillCounts[enemyType] = 1;
        }

        Debug.Log(enemyType + " killed. Total kills: " + enemyKillCounts[enemyType]);

        // Check if the player has earned a reward for this enemy type
        CheckForReward(enemyType);
    }

    // Method to check if the player has earned a reward
    private void CheckForReward(string enemyType)
    {
        // Find the reward details for this enemy type
        EnemyReward reward = enemyRewards.Find(r => r.enemyType == enemyType);
        if (reward != null)
        {
            // Check if the player has reached the required number of kills
            if (enemyKillCounts[enemyType] % reward.killsRequired == 0)
            {
                // Reward the player
                int rewardAmount = (int)(reward.rewardAmount * rewardMultiplier);
                coins += rewardAmount;
                RectTransform rectTransform = coinsText.GetComponent<RectTransform>();
                FloatingTextManager.Instance.ShowFloatingText("+" + rewardAmount, rectTransform, Color.green);

                Debug.Log("Reward earned: " + rewardAmount + " coins for killing " + reward.killsRequired + " " + enemyType + "s. Total coins: " + coins);
            }
        }
    }

    // Method to get the current coin count
    public int GetCoins()
    {
        return coins;
    }

    public void DeductCoins(int amount)
    {
        coins -= amount;
        RectTransform rectTransform = coinsText.GetComponent<RectTransform>();
        FloatingTextManager.Instance.ShowFloatingText("-" + amount, rectTransform, Color.white);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        RectTransform rectTransform = coinsText.GetComponent<RectTransform>();
        FloatingTextManager.Instance.ShowFloatingText("" + amount, rectTransform, Color.green);

    }
}