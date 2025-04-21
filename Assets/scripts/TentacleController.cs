using UnityEngine;
using System.Collections; // For IEnumerator and coroutines
using System;
using TMPro;

public class Tentacle : MonoBehaviour
{
    public Transform characterCenter; // Reference to the character's center
    public float stretchSpeed = 8f;   // Speed of stretching
    public float retractSpeed = 7f;   // Speed of retraction
    public float maxStretch = 5f;     // Maximum stretch length
    public KeyCode key;               // Key to control this tentacle
    public TextMeshProUGUI SalmonelaLeft; // Reference to the Salmonela counter text
    public TextMeshProUGUI TBLeft; // Reference to the TB counter text
    public TextMeshProUGUI CovidLeft; // Reference to the Covid counter text
    public TextMeshProUGUI AlliesLeft; // Reference to the Allies counter text
    private Vector2 originalPosition; // Local position of the tentacle's base
    private bool isStretching = false;
    private bool isRetracting = false;
    private Vector3 originalScale;    // Original scale of the tentacle
    private bool keepStretching = false; // Flag to keep the tentacle stretched
    private Animator mpAnimator; // Reference to the animator component of MP
    


    void Start()
    {
        originalScale = transform.localScale; // Store the original scale
        originalPosition = transform.localPosition;
        mpAnimator = GetComponentInParent<Animator>();
        if (SalmonelaLeft != null)
        {
            SalmonelaLeft.text = "" + GameCountManager.Instance.GetCounterValue("SalmonelaLeft");
        }
        if (TBLeft != null)
        {
            TBLeft.text = "" + GameCountManager.Instance.GetCounterValue("TBLeft");
        }

        if (CovidLeft != null)
        {
            CovidLeft.text = "" + GameCountManager.Instance.GetCounterValue("CovidLeft");
        }

        AlliesLeft.text = "" + GameCountManager.Instance.GetCounterValue("AlliesLeft");

    }

    void Update()
    {
        // Start stretching when the button is pressed
        if (Input.GetKeyDown(key) && !isStretching && !isRetracting && !keepStretching) // Prevent overlapping stretches
        {
            AudioManager.Instance.PlayAt("TentacleWhip", transform); // Play the stretching sound
            isStretching = true;
            isRetracting = false;
        }

        // Stretch or retract the tentacle
        if (isStretching)
        {
            Stretch();
        }
        else if (isRetracting)
        {
            Retract();
        }

        // If the tentacle is set to keep stretching, force it to stay stretched
        if (keepStretching)
        {
            StretchToMax();
        }


    }

    public void Stretch()
    {
        // Check if the tentacle has reached maximum stretch
        if (Math.Abs(transform.localScale.x) >= maxStretch)
        {
            isStretching = false;
            isRetracting = true;
            return; // Exit the method to avoid further stretching
        }

        if (key == KeyCode.A)
        {
            transform.localScale -= new Vector3(stretchSpeed * Time.deltaTime, 0, 0);
            transform.Translate(Vector2.left * stretchSpeed / 2 * Time.deltaTime);
        }
        else
        {
            transform.localScale += new Vector3(stretchSpeed * Time.deltaTime, 0, 0);
            transform.Translate(Vector2.right * stretchSpeed / 2 * Time.deltaTime);
        }
    }

    public void Retract()
    {
        // Calculate the direction for retraction
        Vector3 scaleChange = new Vector3(retractSpeed * Time.deltaTime, 0, 0);
        if (originalScale.x < 0) // If the tentacle's scale is inverted
        {
            scaleChange = -scaleChange; // Invert the direction
        }

        if (Mathf.Abs(transform.localScale.x) > Mathf.Abs(originalScale.x))
        {
            transform.localScale -= scaleChange;
            transform.localPosition = Vector2.MoveTowards(
                transform.localPosition,
                originalPosition,
                retractSpeed / 2 * Time.deltaTime
            );
        }
        else
        {
            transform.localScale = originalScale; // Reset to the original scale
            transform.localPosition = originalPosition;
            isRetracting = false;
        }
    }

    public void RetractToMin()
    {
        keepStretching = false;
        isStretching = false;
        isRetracting = true;
    }


    // Method to force the tentacle to stretch to the maximum length and keep it there
    public void KeepStretching()
    {
        keepStretching = true;
        StretchToMax();
    }

    // Method to stretch the tentacle to its maximum length and keep it there
    private void StretchToMax()
    {
        if (Math.Abs(transform.localScale.x) < maxStretch)
        {
            if (key == KeyCode.A)
            {
                transform.localScale -= new Vector3(stretchSpeed * Time.deltaTime, 0, 0);
                transform.Translate(Vector2.left * stretchSpeed / 2 * Time.deltaTime);
            }
            else
            {
                transform.localScale += new Vector3(stretchSpeed * Time.deltaTime, 0, 0);
                transform.Translate(Vector2.right * stretchSpeed / 2 * Time.deltaTime);
            }

        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((isStretching || keepStretching))
        {
            if (!(keepStretching && (other.CompareTag("Ally") || other.CompareTag("YellowAlly")))) // no ally vaccuming with strech power up
                StartCoroutine(VacuumMicrobe(other.gameObject));
        }
    }

    private IEnumerator VacuumMicrobe(GameObject target)
    {
        // Neutralize the physics applied to the target
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        while (Vector2.Distance(target.transform.position, characterCenter.position) > 0.1f)
        {
            target.transform.position = Vector2.MoveTowards(
                target.transform.position,
                characterCenter.position,
                2f * stretchSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Trigger the eat animation
        if (mpAnimator != null)
        {
            mpAnimator.SetTrigger("Eat");
            AudioManager.Instance.PlayAt("Eat", characterCenter); // Play the eating sound
        }


        Renderer targetRenderer = target.GetComponent<Renderer>();
        Color targetColor = targetRenderer.material.color;
        // Check the tag and update the stats
        if (target.CompareTag("Ally") || target.CompareTag("YellowAlly"))
        {
            if (target.activeInHierarchy)
            {
                GameCountManager.Instance.UpdateCounter("AlliesLeft", -1); // update ally counter
                AudioManager.Instance.Play("WrongAnswer"); // Play the ally sound
            }
            RectTransform rectTransform = AlliesLeft.GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("" + -1, rectTransform, Color.white);

            AlliesLeft.text = "" + GameCountManager.Instance.GetCounterValue("AlliesLeft");

            // Check lose condition
            if (GameCountManager.Instance.GetCounterValue("AlliesLeft") == 0)
            {
                LevelManager.Instance.LoseLevel();
            }
        }
        else if (target.CompareTag("Salmonela"))
        {
            if (target.activeInHierarchy)
            {
                GameCountManager.Instance.UpdateCounter("SalmonelaLeft", -1); // update Salmonela counter
                AudioManager.Instance.Play("CorrectAnswer"); // Play the Salmonela sound
            }

            RectTransform rectTransform = SalmonelaLeft.GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("" + 1, rectTransform, Color.green);

            SalmonelaLeft.text = "" + GameCountManager.Instance.GetCounterValue("SalmonelaLeft");

            // Check win condition
            if (GameCountManager.Instance.GetCounterValue("SalmonelaLeft") == 0)
            {
                LevelManager.Instance.WinLevel();
            }
        }

        else if (target.CompareTag("Tuberculosis"))
        {
            if (target.activeInHierarchy)
            {
                GameCountManager.Instance.UpdateCounter("TBLeft", -1); // update Salmonela counter
                AudioManager.Instance.Play("CorrectAnswer"); // Play the Salmonela sound
            }

            RectTransform rectTransform = TBLeft.GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("" + 1, rectTransform, Color.green);

            TBLeft.text = "" + GameCountManager.Instance.GetCounterValue("TBLeft");

            // Check win condition
            if (GameCountManager.Instance.GetCounterValue("TBLeft") == 0)
            {
                LevelManager.Instance.WinLevel();
            }
        }

        else if (target.CompareTag("Covid"))
        {
            if (target.activeInHierarchy)
            {
                GameCountManager.Instance.UpdateCounter("CovidLeft", -1); // update Salmonela counter
                AudioManager.Instance.Play("CorrectAnswer"); // Play the Salmonela sound
            }

            RectTransform rectTransform = CovidLeft.GetComponent<RectTransform>();
            FloatingTextManager.Instance.ShowFloatingText("" + 1, rectTransform, Color.green);

            CovidLeft.text = "" + GameCountManager.Instance.GetCounterValue("CovidLeft");

            // Check win condition
            if (GameCountManager.Instance.GetCounterValue("CovidLeft") == 0)
            {
                LevelManager.Instance.WinLevel();
            }
        }


        if (target.activeInHierarchy)
        {
            ScoreManager.Instance.UpdateScoreForObject(target.tag);//update score for given object
        }
        target.SetActive(false);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

}