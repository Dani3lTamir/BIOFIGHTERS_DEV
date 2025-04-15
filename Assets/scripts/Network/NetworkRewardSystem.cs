using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NetworkRewardSystem : NetworkBehaviour
{
    public static NetworkRewardSystem Instance;
    [System.Serializable]
    public class EnemyReward
    {
        public string enemyType;
        public int killsRequired;
        public int rewardAmount;
    }

    [Header("Reward Settings")]
    public List<EnemyReward> enemyRewards = new List<EnemyReward>();
    public float rewardMultiplier = 1f;

    [Header("Periodic Reward")]
    public int periodicCoinAmount = 10;
    public float periodicInterval = 30f;

    public int startingClientCoins = 50;
    public int startingHostCoins = 30;

    [Header("UI")]
    public TextMeshProUGUI coinsText;

    private Dictionary<string, int> enemyKillCounts = new Dictionary<string, int>();

    // NetworkVariable for each player
    private NetworkVariable<int> hostCoins = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> clientCoins = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        NetworkEventManager.OnBothPlayersConnected += ActivateRewardSystem;
        NetworkEventManager.OnPlayerDisconnected += DeactivateRewardSystem;
    }

    private void ActivateRewardSystem()
    {
        if (IsServer)
        {
            StartCoroutine(AddCoinsPeriodically());
        }

        InitializePlayerCoins();
    }

    private void InitializePlayerCoins()
    {
        if (IsServer)
        {
            hostCoins.Value = startingHostCoins;
            clientCoins.Value = startingClientCoins;
            Debug.Log("[RewardSystem] Coins initialized for both host and client.");
        }

        // Register UI listener based on player type
        if (IsHost)
        {
            hostCoins.OnValueChanged += OnCoinsChanged;
            UpdateCoinText(hostCoins.Value);
            Debug.Log("[RewardSystem] Host UI set up.");
        }
        else if (IsClient && !IsServer)
        {
            clientCoins.OnValueChanged += OnCoinsChanged;
            UpdateCoinText(clientCoins.Value);
            Debug.Log("[RewardSystem] Client UI set up.");
        }
    }

    private void OnCoinsChanged(int oldVal, int newVal)
    {
        UpdateCoinText(newVal);
    }

    private void UpdateCoinText(int value)
    {
        if (coinsText != null)
        {
            coinsText.text = value.ToString();
        }
        else
        {
            Debug.LogWarning("[RewardSystem] coinsText is not assigned in the inspector.");
        }
    }

    private IEnumerator AddCoinsPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(periodicInterval);
            int amount = Mathf.RoundToInt(periodicCoinAmount * rewardMultiplier);

            hostCoins.Value += amount;
            clientCoins.Value += amount;

            Debug.Log($"[RewardSystem] +{amount} coins for BOTH players. Host: {hostCoins.Value}, Client: {clientCoins.Value}");

            ShowFloatingTextClientRpc("+" + amount, Color.green, NetworkManager.Singleton.ConnectedClientsList[0].ClientId);
            ShowFloatingTextClientRpc("+" + amount, Color.green, NetworkManager.Singleton.ConnectedClientsList[1].ClientId); // assumes exactly 2 players
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterEnemyKillServerRpc(string enemyType)
    {
        if (!enemyKillCounts.ContainsKey(enemyType))
            enemyKillCounts[enemyType] = 0;

        enemyKillCounts[enemyType]++;
        Debug.Log($"[RewardSystem] Killed {enemyType}. Count: {enemyKillCounts[enemyType]}");

        CheckForReward(enemyType);
    }

    private void CheckForReward(string enemyType)
    {
        EnemyReward reward = enemyRewards.Find(r => r.enemyType == enemyType);
        if (reward != null && enemyKillCounts[enemyType] % reward.killsRequired == 0)
        {
            int rewardAmount = Mathf.RoundToInt(reward.rewardAmount * rewardMultiplier);
            if (enemyType == "BodyCell")
            {
                clientCoins.Value += rewardAmount;
                if (NetworkManager.Singleton.ConnectedClientsList.Count > 1)
                    ShowFloatingTextClientRpc("+" + rewardAmount, Color.green, NetworkManager.Singleton.ConnectedClientsList[1].ClientId);
            }
            else
            {
                hostCoins.Value += rewardAmount;
                if (NetworkManager.Singleton.ConnectedClientsList.Count > 0)
                    ShowFloatingTextClientRpc("+" + rewardAmount, Color.green, NetworkManager.Singleton.ConnectedClientsList[0].ClientId);
            }


            Debug.Log($"[RewardSystem] +{rewardAmount} coins from killing {reward.killsRequired} {enemyType}s.");
        }
    }

    public void DeductCoins(int amount, bool isPathogen)
    {
        if (IsServer)
        {
            if (!isPathogen)
            {
                hostCoins.Value -= amount;
                ShowFloatingTextClientRpc("-" + amount, Color.red, NetworkManager.Singleton.ConnectedClientsList[0].ClientId);
            }
            else
            {
                clientCoins.Value -= amount;
                ShowFloatingTextClientRpc("-" + amount, Color.red, NetworkManager.Singleton.ConnectedClientsList[1].ClientId);
            }
        }
        else
        {
            Debug.LogError("[RewardSystem] DeductCoins called on client. This should only be called on the server.");
        }
    }

    [ClientRpc]
    private void ShowFloatingTextClientRpc(string text, Color color, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (coinsText == null) return;

            RectTransform rectTransform = coinsText.GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText(text, rectTransform, color);
        }
    }

    private void DeactivateRewardSystem()
    {
        StopAllCoroutines();
    }

    public int GetCoins(bool isPathogen)
    {
        if (isPathogen)
            return clientCoins.Value;
        else
            return hostCoins.Value;
    }
}
