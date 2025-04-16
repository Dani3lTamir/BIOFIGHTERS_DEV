using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine.UI;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkRelayManager : MonoBehaviour
{
    [SerializeField] private Button joinRelayButton;
    [SerializeField] private Button createRelayButton;
    [SerializeField] TMP_InputField joinCodeInputField;
    [SerializeField] private TextMeshProUGUI relayCodeText;
    async void Start()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in anonymously.");
        }
        else
        {
            Debug.Log("Already signed in.");
        }
        createRelayButton.onClick.AddListener(CreateRelay);
        joinRelayButton.onClick.AddListener(() =>
        {
            if (!string.IsNullOrWhiteSpace(joinCodeInputField.text))
                JoinRelay(joinCodeInputField.text);
        });
    }

    async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            relayCodeText.text = joinCode;

            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost(); // Start the host after setting up the relay server data
        }
        catch (RelayServiceException e)
        {
            relayCodeText.text = "Error: " + e.Message;
        }

    }

    async void JoinRelay(string joinCode)
    {
        try
        {
            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient(); // Start the client after setting up the relay server data
            relayCodeText.text = "Connection successful!";
        }
        catch (RelayServiceException e)
        {
            relayCodeText.text = "Error: " + e.Message;
        }
    }


}
