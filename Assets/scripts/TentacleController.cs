using UnityEngine;
using System.Collections; // For IEnumerator and coroutines
using System;

public class Tentacle : MonoBehaviour
{
    public Transform characterCenter; // Reference to the character's center
    public float stretchSpeed = 8f;   // Speed of stretching
    //public float vacuumSpeed = 15f;   // Speed of stretching
    public float retractSpeed = 7f;  // Speed of retraction
    public float maxStretch = 5f;    // Maximum stretch length
    public KeyCode key;              // Key to control this tentacle
    private Vector2 originalPosition; // Local position of the tentacle's base
    private bool isStretching = false;
    private bool isRetracting = false;
    private Vector3 originalScale;   // Original scale of the tentacle

    void Start()
    {
        originalScale = transform.localScale; // Store the original scale
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Start stretching when the button is pressed
        if (Input.GetKeyDown(key) && !isStretching && !isRetracting) // Prevent overlapping stretches
        {
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
    }

    void Stretch()
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

    void Retract()
    {
        if (transform.localScale.x > originalScale.x)
        {
            transform.localScale -= new Vector3(retractSpeed * Time.deltaTime, 0, 0);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Enemy") || other.CompareTag("Ally") || other.CompareTag("Special Enemy")) && isStretching)
        {
            StartCoroutine(VacuumMicrobe(other.gameObject));
        }
    }

    private IEnumerator VacuumMicrobe(GameObject target)
    {
        //neutrulize the physics apllied to the 
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
        Renderer targetRenderer = target.GetComponent<Renderer>();
        Color targetColor = targetRenderer.material.color;
        // Check the tag and update the score
        if (target.CompareTag("Enemy"))
        {
            if(targetColor == Color.white) ScoreManager.Instance.AddScore(10); // Add points for a white enemy
            else ScoreManager.Instance.AddScore(10);
        }
        else if (target.CompareTag("Ally"))
        {
            ScoreManager.Instance.AddScore(-2); // Deduct points for an ally
        }

        target.SetActive(false);
        rb.bodyType = RigidbodyType2D.Dynamic;
        //rb.linearVelocity = Vector2.zero;

    }
}
