using UnityEngine;
using System.Collections; // For IEnumerator and coroutines
using System;

public class MacropaghPowerUpManager : MonoBehaviour, IPowerUpManager
{
    public static MacropaghPowerUpManager Instance;

    // References to the player and other necessary components
    public CharacterController playerController;
    private EnemyPool[] allPools;

    // Public bool flags for each power-up
    public bool isTentaclesStretched = false;
    public float tentacleStretchDuration = 5f; // Duration for tentacles stretch

    public bool isEnemiesSlowed = false;
    public float slowEnemiesDuration = 5f; // Duration for slowing enemies
    public float slowedSpeed = 0.5f;

    public bool isDoublePointsActive = false;
    public float doublePointsDuration = 5f; // Duration for Double Points

    // Sprites for power-up icons
    public Sprite tentaclesStretchIcon;
    public Sprite slowEnemiesIcon;
    public Sprite doublePointsIcon;

    // Reference to the UIManager
    public UIManager uiManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        allPools = FindObjectsByType<EnemyPool>(
            FindObjectsInactive.Exclude, // Only active objects
            FindObjectsSortMode.None     // No sorting
        ); // Get enemy pools
        uiManager.UpdatePowerUpIcon(null); // Clear the Power Up UI initially
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.O)) // For debugging purposes, get the stretch power-up when pressing 'O'
        {
            MacropaghPowerUpManager.Instance.ActivateTentaclesStretch();
        }

        if (Input.GetKey(KeyCode.P)) // For debugging purposes, get the slow down power-up when pressing 'P'
        {
            MacropaghPowerUpManager.Instance.ActivateSlowEnemies();
        }
        if (Input.GetKey(KeyCode.I)) // For debugging purposes, get the double points power-up when pressing 'I'
        {
            MacropaghPowerUpManager.Instance.ActivateDoublePoints();
        }
    }

    // Power-up functions
    public void ActivateTentaclesStretch()
    {
        if (!isTentaclesStretched)
        {
            isTentaclesStretched = true;
            playerController.StretchAllTentacles(); // Call to the function to stretch all tentacles
            uiManager.UpdatePowerUpIcon(tentaclesStretchIcon); // Update the UI
            StartCoroutine(DeactivateTentaclesStretch());
        }
    }

    private IEnumerator DeactivateTentaclesStretch()
    {
        yield return new WaitForSeconds(tentacleStretchDuration);
        isTentaclesStretched = false;
        playerController.RetractAllTentacles(); // Reset tentacles
        uiManager.UpdatePowerUpIcon(null); // Clear the UI
    }

    public void ActivateSlowEnemies()
    {
        if (!isEnemiesSlowed)
        {
            isEnemiesSlowed = true;
            foreach (EnemyPool enemyPool in allPools)
            {
                enemyPool.ChangeEnemiesSpeed(slowedSpeed); // Slow enemies
            }
            uiManager.UpdatePowerUpIcon(slowEnemiesIcon); // Update the UI
            StartCoroutine(DeactivateSlowEnemies());
        }
    }

    private IEnumerator DeactivateSlowEnemies()
    {
        yield return new WaitForSeconds(slowEnemiesDuration);
        isEnemiesSlowed = false;
        foreach (EnemyPool enemyPool in allPools)
        {
            enemyPool.ResetEnemiesSpeed(); // Reset enemies speed
        }
        uiManager.UpdatePowerUpIcon(null); // Clear the UI
    }

    public void ActivateDoublePoints()
    {
        if (!isDoublePointsActive)
        {
            isDoublePointsActive = true;
            ScoreManager.Instance.ActivateDoublePoints(); // Call function to activate double points
            uiManager.UpdatePowerUpIcon(doublePointsIcon); // Update the UI
            StartCoroutine(DeactivateDoublePoints());
        }
    }

    private IEnumerator DeactivateDoublePoints()
    {
        yield return new WaitForSeconds(doublePointsDuration);
        isDoublePointsActive = false;
        ScoreManager.Instance.DeactivateDoublePoints(); // Reset player double points
        uiManager.UpdatePowerUpIcon(null); // Clear the UI
    }

    public void DeactivateAllPowerUps()
    {
        isTentaclesStretched = false;
        isEnemiesSlowed = false;
        isDoublePointsActive = false;

        playerController.RetractAllTentacles();
        DeactivateSlowEnemies();
        ScoreManager.Instance.DeactivateDoublePoints();
        uiManager.UpdatePowerUpIcon(null); // Clear the UI
    }

    public void ActivateRandomPowerUp()
    {
        int randomPowerUp = UnityEngine.Random.Range(0, 3);

        switch (randomPowerUp)
        {
            case 0:
                ActivateTentaclesStretch();
                Debug.Log("Stretch All Tentacles Activated!");
                break;
            case 1:
                ActivateSlowEnemies();
                Debug.Log("Slow Enemies Activated!");
                break;
            case 2:
                ActivateDoublePoints();
                Debug.Log("Double Points Activated!");
                break;
        }
    }
}