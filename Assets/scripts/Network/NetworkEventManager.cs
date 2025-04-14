using Unity.Netcode;
using UnityEngine;
public class NetworkEventManager : MonoBehaviour
{
    public static event System.Action OnBothPlayersConnected;
    public static event System.Action OnPlayerDisconnected;

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            OnBothPlayersConnected?.Invoke(); // Notify all systems
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        OnPlayerDisconnected?.Invoke(); // Notify all systems
    }
}
