using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;


[RequireComponent(typeof(NetworkObject))]
public class NetworkTimer : NetworkBehaviour
{
    public static NetworkTimer Instance { get; private set; }

    [Header("Timer Settings")]
    [SerializeField] private float _gameDuration = 180f;
    [SerializeField] private bool _autoStart = true;
    [SerializeField] private TextMeshProUGUI _timerText;


    public event Action OnTimerStarted;
    public event Action OnTimerEnded;

    private NetworkVariable<float> _remainingTime = new NetworkVariable<float>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private NetworkVariable<bool> _timerRunning = new NetworkVariable<bool>();
    private bool _timerInitialized;

    // Host uses raw time, clients use smoothed time
    private float _displayedTime;
    private float _smoothVelocity;
    private const float SMOOTH_TIME = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[NetworkTimer] Another instance of NetworkTimer was found. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _remainingTime.Value = _gameDuration;
            NetworkManager.OnClientConnectedCallback += HandleNewConnection;
        }

        _remainingTime.OnValueChanged += HandleTimeUpdate;
        _timerRunning.OnValueChanged += HandleTimerStateChange;

        // Initialize displayed time
        _displayedTime = _remainingTime.Value;
    }

    private void Update()
    {
        // Don't update if we're not spawned or the NetworkManager is shutting down
        if (!IsSpawned || !NetworkManager.Singleton || !NetworkManager.Singleton.IsListening)
            return;

        // Server updates the actual time
        if (IsServer && _timerRunning.Value)
        {
            _remainingTime.Value = Mathf.Max(0, _remainingTime.Value - Time.deltaTime);
            if (_remainingTime.Value <= 0) EndTimer();
        }

        // All clients (including host client) get smooth updates
        if (!IsServer || IsHost)
        {
            _displayedTime = Mathf.SmoothDamp(
                _displayedTime,
                _remainingTime.Value,
                ref _smoothVelocity,
                SMOOTH_TIME
            );
        }

        // Update UI for everyone
        if (_timerText != null)
        {
            _timerText.text = FormatTime(GetDisplayTime());
        }
    }

    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        if (time > 0)
        {
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            return "00:00";
        }
    }


    private float GetDisplayTime()
    {
        // Host sees server time directly (no smoothing needed)
        return IsServer ? _remainingTime.Value : _displayedTime;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestStartTimerServerRpc()
    {
        if (!_timerRunning.Value && NetworkManager.ConnectedClients.Count >= 2)
        {
            _remainingTime.Value = _gameDuration;
            _timerRunning.Value = true;
            OnTimerStarted?.Invoke();
        }
    }

    private void EndTimer()
    {
        _timerRunning.Value = false;
        OnTimerEnded?.Invoke();
    }

    private void HandleNewConnection(ulong clientId)
    {
        if (_autoStart && NetworkManager.ConnectedClients.Count >= 2)
        {
            RequestStartTimerServerRpc();
        }
    }

    private void HandleTimeUpdate(float oldTime, float newTime)
    {
        if (newTime <= 0) OnTimerEnded?.Invoke();
    }

    private void HandleTimerStateChange(bool oldState, bool newState)
    {
        if (newState) OnTimerStarted?.Invoke();
    }

    private void OnDestroy()
    {

        NetworkManager.OnClientConnectedCallback -= HandleNewConnection;
        _remainingTime.OnValueChanged -= HandleTimeUpdate;
        _timerRunning.OnValueChanged -= HandleTimerStateChange;
    }
}