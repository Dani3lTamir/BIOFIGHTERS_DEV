using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine.UI;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;

public class NetworkRelayManager : MonoBehaviour
{
    [SerializeField] private Button joinRelayButton;
    [SerializeField] private Button createRelayButton;
    [SerializeField] TMP_InputField joinCodeInputField;
    [SerializeField] private TextMeshProUGUI relayCodeText;

    async void Start()
    {
        Debug.Log("[NetworkRelayManager] Start() called");

        try
        {
            Debug.Log("[NetworkRelayManager] Initializing Unity Services...");
            await UnityServices.InitializeAsync();
            Debug.Log("[NetworkRelayManager] Unity Services initialized successfully");

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("[NetworkRelayManager] Attempting anonymous sign-in...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("[NetworkRelayManager] Signed in anonymously successfully. PlayerID: " + AuthenticationService.Instance.PlayerId);
            }
            else
            {
                Debug.Log("[NetworkRelayManager] Already signed in. PlayerID: " + AuthenticationService.Instance.PlayerId);
            }

            Debug.Log("[NetworkRelayManager] Setting up button listeners...");
            createRelayButton.onClick.AddListener(() =>
            {
                Debug.Log("[NetworkRelayManager] Create Relay button clicked");
                CreateRelay();
            });

            joinRelayButton.onClick.AddListener(() =>
            {
                Debug.Log("[NetworkRelayManager] Join Relay button clicked");
                if (!string.IsNullOrWhiteSpace(joinCodeInputField.text))
                {
                    Debug.Log($"[NetworkRelayManager] Attempting to join with code: {joinCodeInputField.text}");
                    JoinRelay(joinCodeInputField.text);
                }
                else
                {
                    Debug.LogWarning("[NetworkRelayManager] Join code input field is empty");
                }
            });

            Debug.Log("[NetworkRelayManager] Initialization complete");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[NetworkRelayManager] Initialization failed: {e.Message}");
            relayCodeText.text = "Init Error: " + e.Message;
        }
    }

    async void CreateRelay()
    {
        Debug.Log("[NetworkRelayManager] CreateRelay() started");
        relayCodeText.text = "Creating relay...";

        try
        {
            Debug.Log("[NetworkRelayManager] Creating allocation...");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            Debug.Log($"[NetworkRelayManager] Allocation created. ID: {allocation.AllocationId}, Region: {allocation.Region}");

            Debug.Log("[NetworkRelayManager] Getting join code...");
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"[NetworkRelayManager] Join code received: {joinCode}");

            relayCodeText.text = joinCode;

            Debug.Log("[NetworkRelayManager] Converting to relay server data...");
            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            Debug.Log($"[NetworkRelayManager] Relay server data: {relayServerData}");

            Debug.Log("[NetworkRelayManager] Setting up Unity Transport...");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            Debug.Log("[NetworkRelayManager] Starting host...");
            NetworkManager.Singleton.StartHost();
            Debug.Log("[NetworkRelayManager] Host started successfully");
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"[NetworkRelayManager] Relay creation failed: {e.Message}");
            relayCodeText.text = "Error: " + e.Message;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[NetworkRelayManager] General error in CreateRelay: {e.Message}");
            relayCodeText.text = "System Error: " + e.Message;
        }
    }

    async void JoinRelay(string joinCode)
    {
        joinCode = joinCode.Trim().ToUpper();
        Debug.Log($"[NetworkRelayManager] Attempting to join with code: '{joinCode}'");
        relayCodeText.text = "Joining...";
        joinRelayButton.interactable = false;

        try
        {
            // Validate code format first
            if (string.IsNullOrWhiteSpace(joinCode) || joinCode.Length != 6)
            {
                throw new System.ArgumentException("Join code must be 6 characters");
            }

            // Show loading state
            relayCodeText.text = "Connecting...";

            // Join with timeout
            var joinTask = RelayService.Instance.JoinAllocationAsync(joinCode);
            var timeoutTask = Task.Delay(10000); // 10 second timeout

            var completedTask = await Task.WhenAny(joinTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                throw new System.TimeoutException("Connection timed out");
            }

            var allocation = await joinTask;

            Debug.Log($"[NetworkRelayManager] Joined allocation. Region: {allocation.Region}");

            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            Debug.Log("[NetworkRelayManager] Starting client...");
            if (!NetworkManager.Singleton.StartClient())
            {
                throw new System.Exception("Failed to start client");
            }

            relayCodeText.text = "Connected!";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[NetworkRelayManager] Join failed: {e}");
            relayCodeText.text = $"Join failed: {e.Message}";
            joinRelayButton.interactable = true;
        }
    }
    void OnDestroy()
    {
        Debug.Log("[NetworkRelayManager] OnDestroy() called");
        // Clean up button listeners to prevent memory leaks
        createRelayButton.onClick.RemoveAllListeners();
        joinRelayButton.onClick.RemoveAllListeners();
    }
}