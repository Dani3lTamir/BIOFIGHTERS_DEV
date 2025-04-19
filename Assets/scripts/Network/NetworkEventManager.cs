using Unity.Netcode;
using UnityEngine;
using System;

public class NetworkEventManager : NetworkBehaviour
{
    // Singleton pattern
    public static NetworkEventManager Instance { get; private set; }

    public static event Action OnPathogenWin;
    public static event Action OnImmuneWin;

    public static event Action OnGameEnded;
    public static event Action OnBothPlayersConnected;

    // Game state tracking
    private NetworkVariable<bool> _gameEnded = new NetworkVariable<bool>(false);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("Player Spawned");
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += HandleClientConnected;
            NetworkTimer.Instance.OnTimerEnded += RequestImmuneWinServerRpc; // End game if timer ends
            OnImmuneWin += () => OnGameEnded?.Invoke();
            OnPathogenWin += () => OnGameEnded?.Invoke();
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (NetworkManager.ConnectedClients.Count == 2)
        {
            DisableLobbyUIClientRpc();
            OnBothPlayersConnected?.Invoke();
            Debug.Log("Both players connected - game starting!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestPathogenWinServerRpc()
    {
        if (_gameEnded.Value) return;
        TriggerPathogenWin();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestImmuneWinServerRpc()
    {
        if (_gameEnded.Value) return;
        TriggerImmuneWin();
    }

    private void TriggerPathogenWin()
    {
        _gameEnded.Value = true;
        OnPathogenWin?.Invoke();
        NotifyClientsOfWinClientRpc(true);
        Debug.Log("Pathogen player wins!");
    }

    private void TriggerImmuneWin()
    {
        _gameEnded.Value = true;
        OnImmuneWin?.Invoke();
        NotifyClientsOfWinClientRpc(false);
        Debug.Log("Immune player wins!");
    }

    [ClientRpc]
    private void NotifyClientsOfWinClientRpc(bool pathogenWon)
    {
        if (pathogenWon)
        {
            OnPathogenWin?.Invoke();
            if (NetworkManager.Singleton.IsHost)
            {
                // Show UI for immune lose
                NetworkManagerUI.Instance.SetImmuneLoseUIActive();
            }
            else
            {
                // Show UI for pathogen win
                NetworkManagerUI.Instance.SetPathogenWinUIActive();
            }
        }
        else
        {
            OnImmuneWin?.Invoke();
            if (NetworkManager.Singleton.IsHost)
            {
                // Show UI for immune win
                NetworkManagerUI.Instance.SetImmuneWinUIActive();
            }
            else
            {
                // Show UI for pathogen lose
                NetworkManagerUI.Instance.SetPathogenLoseUIActive();
            }
        }
    }

    [ClientRpc]
    private void DisableLobbyUIClientRpc()
    {
        NetworkManagerUI.Instance.SetLobbyUIInactive();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback -= HandleClientConnected;
            NetworkTimer.Instance.OnTimerEnded -= RequestImmuneWinServerRpc;
        }
    }

    public bool IsGameEnded()
    {
        return _gameEnded.Value;
    }
}