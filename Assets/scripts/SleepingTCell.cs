using UnityEngine;
using TMPro;
using System.Collections;

public class SleepingTCell : MonoBehaviour
{
    public GameObject antibody; // Reference to the antibody GameObject
    private SpriteRenderer antibodySpriteRenderer; // Reference to the SpriteRenderer of the antibody
    public bool isAwake = false; // Flag to indicate if the T cell is awake
    public KeyCode wakeKey = KeyCode.C; // Key to wake up the T cell
    public bool hasCorrectAntibody = false; // Flag to indicate if the antibody is correct
    private bool playerInRange = false; // Flag to indicate if the player is in range
    public bool hideAntiBodyOnExit = false;
    public float timePenalty = -5f; // Time penalty for incorrect antibody
    public Sprite wokeSprite; // Sprite to show when the T cell is awake
    public Sprite comTSprite; // Sprite to show when the T cell is awake
    public float comTMoveSpeed = 3f; // Speed of the T cell moving up
    private TextMeshProUGUI AttemptsLeftText;

    private Timer timer;

    public void Start()
    {
        // Get the SpriteRenderer component of the antibody
        antibodySpriteRenderer = antibody.GetComponent<SpriteRenderer>();
        // Initially hide the antibody sprite
        antibodySpriteRenderer.enabled = false;
        // Get the Timer component
        GameObject levelTimer = GameObject.FindGameObjectWithTag("LevelTimer");
        // Get attempts left text
        AttemptsLeftText = DCUIManager.Instance.AttemptsLeftText;
        if (levelTimer != null)
        {
            timer = levelTimer.GetComponent<Timer>();
        }

        AudioManager.Instance.PlayAt("Snore", transform); // Play the sound effect for the T cell

    }

    void Update()
    {
        // Check if the player is in range and the wake key is pressed
        if (playerInRange && !isAwake && Input.GetKeyDown(wakeKey))
        {
            // Wake up the T cell
            isAwake = true;
            // Hide the antibody sprite
            antibodySpriteRenderer.enabled = false;
            // Disable the animator
            GetComponent<Animator>().enabled = false;
            // Hide the wake UI
            DCUIManager.Instance.HideWakeUI();
            // if the antibody is correct write to the console
            if (hasCorrectAntibody)
            {
                Debug.Log("Correct Antibody!");
                // win the level
                StartCoroutine(OnRightChoice());
            }
            else
            {
                Debug.Log("Incorrect Antibody!");
                // Incorrect choice penalty
                GameCountManager.Instance.UpdateCounter("AttemptsLeft", -1);
                ScoreManager.Instance.UpdateScoreForObject("WrongCell");
                AttemptsLeftText.text = "" + GameCountManager.Instance.GetCounterValue("AttemptsLeft");
                RectTransform rectTransform = AttemptsLeftText.GetComponent<RectTransform>();
                FloatingTextManager.Instance.ShowFloatingText("" + -1, rectTransform, Color.red);
                // Play the sound effect for the incorrect choice
                GetComponent<AudioSource>().Stop();
                AudioManager.Instance.Play("Grunt");
                // change the sprite to the woke sprite
                GetComponent<SpriteRenderer>().sprite = wokeSprite;
                // Check lose condition
                if (GameCountManager.Instance.GetCounterValue("AttemptsLeft") == 0)
                {
                    LevelManager.Instance.LoseLevel();
                }
                else timer.AddTime(timePenalty);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the collider
        if (other.CompareTag("Player") && !isAwake)
        {
            playerInRange = true;
            // Show the antibody sprite
            antibodySpriteRenderer.enabled = true;
            // Show the wake UI
            DCUIManager.Instance.ShowWakeUI();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exits the collider
        if (other.CompareTag("Player") && !isAwake)
        {
            playerInRange = false;
            // Hide the antibody sprite if its not destroyed
            if (antibody != null && hideAntiBodyOnExit)
            {
                antibodySpriteRenderer.enabled = false;
            }
            // Hide the wake UI
            DCUIManager.Instance.HideWakeUI();
        }
    }

    public void SetCorrectAntibody(bool isCorrect)
    {
        hasCorrectAntibody = isCorrect;
    }

    IEnumerator OnRightChoice()
    {
        // Add Score 
        ScoreManager.Instance.UpdateScoreForObject("RightCell");
        // Check Timer for bonus
        if (timer != null)
        {
            float timeLeft = timer.GetRoundedTimeRemaining();
            if (timeLeft > 0)
            {
                ScoreManager.Instance.AddScore((int)(timeLeft));
            }
        }
        Debug.Log("Final Score: " + ScoreManager.Instance.GetScore());
        // Play the sound effect for the correct choice
        AudioManager.Instance.Play("CorrectAnswer");
        // Freeze the player (DCController)
        DCController dcController = FindFirstObjectByType<DCController>();
        // Freeze the player
        dcController.Freeze();
        //  Change sprite
        GetComponent<SpriteRenderer>().sprite = comTSprite;
        yield return new WaitForSeconds(2);

        //  Calculate screen top in world units
        float screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        float moveSpeed = comTMoveSpeed; // units/second

        //  Move until above screen
        while (transform.position.y < screenTop + 1f) // +1f for margin
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 4. Win level
        LevelManager.Instance.WinLevel();
    }
}

