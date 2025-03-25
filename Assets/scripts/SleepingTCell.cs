using UnityEngine;
using TMPro;

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
            // Hide the wake UI
            DCUIManager.Instance.HideWakeUI();
            // if the antibody is correct write to the console
            if (hasCorrectAntibody)
            {
                Debug.Log("Correct Antibody!");
                // win the level
                LevelManager.Instance.WinLevel();
            }
            else
            {
                Debug.Log("Incorrect Antibody!");
                // Incorrect choice penalty
                GameCountManager.Instance.UpdateCounter("AttemptsLeft", -1);
                AttemptsLeftText.text = "" + GameCountManager.Instance.GetCounterValue("AttemptsLeft");
                RectTransform rectTransform = AttemptsLeftText.GetComponent<RectTransform>();
                FloatingTextManager.Instance.ShowFloatingText("" + -1, rectTransform, Color.red);
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
}

