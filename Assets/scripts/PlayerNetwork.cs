using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{


    // Called when the network spawns this player
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;

        if (IsHost)
        {
            NetworkManagerUI.Instance.SetUIActive(true);
        }
        else
        {
            Debug.Log("Client spawned");
            NetworkManagerUI.Instance.SetUIActive(false);
        }
    }

}