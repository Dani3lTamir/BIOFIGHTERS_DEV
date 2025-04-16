using UnityEngine;
using Unity.Netcode;

public class DisconnectionHandler : NetworkBehaviour
{

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        //   Destroy(NetworkManager.Singleton.gameObject); // Destroys the NetworkManager
    }

    private void OnClientDisconnect(ulong clientId)
    {
        // If this is the local player and the game didn't end, show the disconnect screen
        if (!(NetworkEventManager.Instance.IsGameEnded()))
        {
            Debug.Log("Disconnected from server. Showing disconnect screen.");
            NetworkManagerUI.Instance.SetPlayerDisconnectUIActive();
        }

        if (NetworkManager.Singleton.IsServer)
        {
            // Clean up objects owned by the disconnected client except for this object
            // This is important to avoid destroying the object that is handling the disconnection
            foreach (var obj in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
            {
                if (obj.Key != clientId && obj.Value.IsOwner)
                {
                    obj.Value.Despawn(true);
                }
            }

            Debug.Log($"[Network] Cleaned up objects for disconnected client {clientId}");

        }

    }

}
