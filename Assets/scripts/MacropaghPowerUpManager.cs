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
        );//get enemy pools

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.O))//for debuging purposes get the strech power up when pressing 'o'
        {
            MacropaghPowerUpManager.Instance.ActivateTentaclesStretch();
        }

        if (Input.GetKey(KeyCode.P))//for debuging purposes get the slow down power up when pressing 'p'
        {
            MacropaghPowerUpManager.Instance.ActivateSlowEnemies();
        }
        if (Input.GetKey(KeyCode.I))//for debuging purposes get the double points power up when pressing 'p'
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
            playerController.StretchAllTentacles(); //  call to the function to stretch all tentacles
            StartCoroutine(DeactivateTentaclesStretch());
        }
    }

    private IEnumerator DeactivateTentaclesStretch()
    {
        yield return new WaitForSeconds(tentacleStretchDuration);
        isTentaclesStretched = false;
        playerController.RetractAllTentacles(); // Reset tentacles
    }

    public void ActivateSlowEnemies()
    {
        if (!isEnemiesSlowed)
        {
            isEnemiesSlowed = true;
            foreach (EnemyPool enemyPool in allPools)
            {
                enemyPool.ChangeEnemiesSpeed(slowedSpeed); //  enemyPool to slow enemies
            }
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

    }

    public void ActivateDoublePoints()
    {
        if (!isDoublePointsActive)
        {
            isDoublePointsActive = true;
            ScoreManager.Instance.ActivateDoublePoints(); // Call function to ActivateDoublePoints
            StartCoroutine(DeactivateDoublePoints());
        }
    }

    private IEnumerator DeactivateDoublePoints()
    {
        yield return new WaitForSeconds(doublePointsDuration);
        isDoublePointsActive = false;
        ScoreManager.Instance.DeactivateDoublePoints(); // Reset player Double Points
    }

    // You can also manually check and deactivate the power-ups if you want:
    public void DeactivateAllPowerUps()
    {
        isTentaclesStretched = false;
        isEnemiesSlowed = false;
        isDoublePointsActive = false;

        playerController.RetractAllTentacles();
        DeactivateSlowEnemies();
        ScoreManager.Instance.DeactivateDoublePoints();
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
