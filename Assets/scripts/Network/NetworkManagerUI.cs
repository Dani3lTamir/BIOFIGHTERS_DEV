using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    public static NetworkManagerUI Instance; // Singleton instance
    [SerializeField] private GameObject immuneUI; // Assign in Inspector
    [SerializeField] private GameObject pathogenUI; // Assign in Inspector

    [SerializeField] private GameObject immuneWinUI; // Assign in Inspector
    [SerializeField] private GameObject immuneLoseUI; // Assign in Inspector
    [SerializeField] private GameObject pathogenWinUI; // Assign in Inspector
    [SerializeField] private GameObject pathogenLoseUI; // Assign in Inspector

    [SerializeField] private GameObject playerDisconnectUI; // Assign in Inspector

    [SerializeField] private GameObject lobbyUI; // Assign in Inspector




    void Awake()
    {
        // Ensure there's only one instance of the NetworkManagerUI
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }


    public void SetUIActive(bool isHost)
    {
        if (isHost)
        {
            immuneUI.SetActive(true);
            pathogenUI.SetActive(false);
        }
        else
        {
            immuneUI.SetActive(false);
            pathogenUI.SetActive(true);
        }
    }

    public void SetImmuneWinUIActive()
    {
        immuneWinUI.SetActive(true);
    }
    public void SetImmuneLoseUIActive()
    {
        immuneLoseUI.SetActive(true);
    }

    public void SetPathogenLoseUIActive()
    {
        pathogenLoseUI.SetActive(true);
    }

    public void SetPathogenWinUIActive()
    {
        pathogenWinUI.SetActive(true);
    }

    public void SetPlayerDisconnectUIActive()
    {
        playerDisconnectUI.SetActive(true);
    }

    public void SetLobbyUIInactive()
    {
        lobbyUI.SetActive(false);
    }
}