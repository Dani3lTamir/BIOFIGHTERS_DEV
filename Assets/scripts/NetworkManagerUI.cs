using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    public static NetworkManagerUI Instance; // Singleton instance
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private GameObject immuneUI; // Assign in Inspector
    [SerializeField] private GameObject pathogenUI; // Assign in Inspector




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

        // Set up button listeners
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
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


}