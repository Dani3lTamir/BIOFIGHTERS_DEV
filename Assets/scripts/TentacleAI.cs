using UnityEngine;
using System.Collections;

public class TentacleAI : MonoBehaviour
{
    public Transform characterCenter; // Reference to the character's center
    public float stretchSpeed = 8f;   // Speed of stretching
    public float retractSpeed = 7f;   // Speed of retraction
    public float maxStretch = 5f;     // Maximum stretch length
    private Vector2 originalPosition; // Local position of the tentacle's base
    private bool isStretching = false;
    private bool isRetracting = false;
    private Vector3 originalScale;    // Original scale of the tentacle

    void Start()
    {
        originalScale = transform.localScale; // Store the original scale
        originalPosition = transform.localPosition;
    }

    public void Stretch()
    {
        isStretching = true;
        isRetracting = false;
    }

    public void Retract()
    {
        isStretching = false;
        isRetracting = true;
    }

    public bool IsStretching()
    {
        return isStretching;
    }

    void Update()
    {
        if (isStretching)
        {
            // Check if the tentacle has reached maximum stretch
            if (Mathf.Abs(transform.localScale.x) >= maxStretch)
            {
                Retract();
                return; // Exit the method to avoid further stretching
            }

            if (transform.localScale.x < 0) // Left tentacle
            {
                transform.localScale -= new Vector3(stretchSpeed * Time.deltaTime, 0, 0);
                transform.Translate(Vector2.left * stretchSpeed / 2 * Time.deltaTime);
            }
            else // Right tentacle
            {
                transform.localScale += new Vector3(stretchSpeed * Time.deltaTime, 0, 0);
                transform.Translate(Vector2.right * stretchSpeed / 2 * Time.deltaTime);
            }
        }
        else if (isRetracting)
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
    }

    public IEnumerator VacuumMicrobe(Collider2D other)
    {
        EcoliAI ecoliAI = other.GetComponent<EcoliAI>();
        // Move the Ecoli towards the Macrophage
        while (Vector2.Distance(other.transform.position, characterCenter.position) > 0.1f)
            {
                other.transform.position = Vector2.MoveTowards(
                    other.transform.position,
                    characterCenter.position,
                    2f * stretchSpeed * Time.deltaTime
                );
                yield return null;
            }

        // Handle Ecoli capture (e.g., update score, deactivate Ecoli)
        if (other.CompareTag("Ecoli"))
        {
            GameCountManager.Instance.UpdateCounter("EcoliLeft", -1); // Update Ecoli counter
        }
        ScoreManager.Instance.UpdateScoreForObject(other.tag); // Update score for given object
        other.gameObject.SetActive(false);

        // Re-enable the Ecoli's movement 
        if (ecoliAI != null)
        {
            ecoliAI.EnableMovement();
        }
    }
}