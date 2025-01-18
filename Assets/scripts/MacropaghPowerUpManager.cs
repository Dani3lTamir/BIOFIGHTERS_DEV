using UnityEngine;
using System.Collections; // For IEnumerator and coroutines
using System;


public class MacropaghPowerUpManager : MonoBehaviour
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

    public bool isSpeedUpActive = false;
    public float speedUpDuration = 5f; // Duration for speeding up player

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

    public void ActivateSpeedUp()
    {
        if (!isSpeedUpActive)
        {
            isSpeedUpActive = true;
        //    playerController.SpeedUp(); // Call function to speed up player
            StartCoroutine(DeactivateSpeedUp());
        }
    }

    private IEnumerator DeactivateSpeedUp()
    {
        yield return new WaitForSeconds(speedUpDuration);
        isSpeedUpActive = false;
      //  playerController.ResetSpeed(); // Reset player speed
    }

    // You can also manually check and deactivate the power-ups if you want:
    public void DeactivateAllPowerUps()
    {
        isTentaclesStretched = false;
        isEnemiesSlowed = false;
        isSpeedUpActive = false;

        playerController.RetractAllTentacles();
        DeactivateSlowEnemies();
      //  playerController.ResetSpeed();
    }
}
