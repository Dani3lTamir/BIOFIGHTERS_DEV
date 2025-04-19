using UnityEngine;
using Unity.Netcode;
using Unity.BossRoom.Infrastructure;
using System.Collections;
using UnityEngine.SceneManagement;


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
    }

    private void OnClientDisconnect(ulong clientId)
    {
        // If this is the local player and the game didn't end, show the disconnect screen
        if (!(NetworkEventManager.Instance.IsGameEnded()))
        {
            Debug.Log("Disconnected from server. Showing disconnect screen.");
            NetworkManagerUI.Instance.SetPlayerDisconnectUIActive();
        }
    }

    public void OnClientLeave()
    {
        StartCoroutine(LeaveGameGracefully());
    }

    private IEnumerator LeaveGameGracefully()
    {
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsClient)
            {
                NetworkObjectPool.Singleton?.OnNetworkDespawn();
                // Gracefully shutdown
                NetworkManager.Singleton.Shutdown();
            }
        }

        // Give Unity a frame or two to clean up
        yield return null;
        yield return null;

        // Load your main menu scene (or whatever)
        SceneManager.LoadScene("MainMenu");
    }


}
