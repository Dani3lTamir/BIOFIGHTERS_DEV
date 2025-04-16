using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;

public class NetworkDCAI : NetworkBehaviour
{
    public float moveSpeed = 3f; // Speed at which the DC Defender moves
    public float zigzagAmplitude = 2f; // Height of the zigzag
    public float zigzagFrequency = 1f; // Speed of the zigzag
    public int zigzagCount = 3; // Number of zigzags before leaving
    public float screenExitDelay = 2f; // Time to wait before leaving the screen
    public Vector2 onScreenOffset = new Vector2(1, 0); // Offset to leave the screen's boundaries
    public int catchLimit = 5; // Maximum number of Ecoli that can be caught
    public float damage = 8f; // Damage caused by the DC Defender


    private bool isMoving = false; // Whether the DC Defender is moving
    private List<GameObject> caughtEcoli = new List<GameObject>(); // List of caught Ecoli
    private Vector2 startPosition; // Starting position of the DC Defender
    private float zigzagTimer = 0f; // Timer for zigzag movement
    private int currentZigzag = 0; // Current zigzag count
    private Vector2 moveDirection = Vector2.right; // Direction of movement


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return; // Only the server should control the DC Defender's behavior
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null && !collider.enabled)
        {
            return; // Do nothing if collider is disabled
        }

        StartMovement();

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        // Unparent all caught Ecoli when the DC Defender is destroyed
        foreach (var ecoli in caughtEcoli)
        {
            if (ecoli != null)
            {
                ecoli.transform.SetParent(null);
            }
        }
        caughtEcoli.Clear(); // Clear the list of caught Ecoli
        StopAllCoroutines(); // Stop any ongoing coroutines
        isMoving = false; // Stop the DC Defender's movement
        currentZigzag = 0; // Reset the zigzag count
        zigzagTimer = 0f; // Reset the zigzag timer
        moveDirection = Vector2.right; // Reset the movement direction
    }



    void StartMovement()
    {
        isMoving = true;
        startPosition = transform.position;
    }

    void Update()
    {
        if (!IsServer) return; // Only the server should control the DC Defender's behavior
        if (!isMoving) return;

        // Zigzag movement
        zigzagTimer += Time.deltaTime;
        float xMovement = moveSpeed * Time.deltaTime * moveDirection.x;
        float yMovement = Mathf.Sin(zigzagTimer * zigzagFrequency) * zigzagAmplitude;

        // Move the DC Defender
        Vector2 newPosition = transform.position + new Vector3(xMovement, yMovement, 0);

        // Check for screen boundaries and bounce if necessary
        CheckScreenBoundaries(ref newPosition);

        // Update the position
        transform.position = newPosition;

        // Check if the DC Defender has completed a zigzag
        if (zigzagTimer >= (2 * Mathf.PI) / zigzagFrequency)
        {
            zigzagTimer = 0f;
            currentZigzag++;

            // Check if the DC Defender has completed all zigzags
            if (currentZigzag >= zigzagCount)
            {
                StartCoroutine(LeaveScreen());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return; // Only the server should control the DC Defender's behavior
        if (other.CompareTag("Ecoli") && caughtEcoli.Count < catchLimit && (other.GetComponent<NetworkEcoliAI>().getMovmentStatus()))
        {
            // Catch the Ecoli
            CatchEcoli(other.gameObject);
        }

        else if ((other.GetComponent<IBoss>() != null) && (other.GetComponent<IBoss>().getMovmentStatus()))
        {
            // Damage the Boss
            other.GetComponent<NetworkHealthSystem>().TakeDamage(damage);
        }

    }

    void CatchEcoli(GameObject ecoli)
    {
        // Disable the Ecoli's movement and add it to the caught list
        NetworkEcoliAI ecoliAI = ecoli.GetComponent<NetworkEcoliAI>();
        if (ecoliAI != null)
        {
            ecoliAI.DisableMovement();
        }
        caughtEcoli.Add(ecoli);

        // Parent the Ecoli to the DC Defender so it moves with it
        ecoli.transform.SetParent(transform);
    }

    IEnumerator LeaveScreen()
    {
        isMoving = false;

        // Move off the screen
        Vector2 exitDirection = new Vector2(1, 0); // Move right to exit the screen
        while (IsOnScreen())
        {
            transform.Translate(exitDirection * moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Kill all caught Ecoli
        foreach (var ecoli in caughtEcoli)
        {
            if (ecoli != null)
            {
                // Unparent the Ecoli and kill it
                ecoli.transform.SetParent(null);
                ecoli.GetComponent<NetworkEcoliAI>().Die();
            }
        }
        // return the DC Defender to the pool
        if (IsSpawned) GetComponent<NetworkObject>().Despawn(true);
    }

    bool IsOnScreen()
    {
        // Check if the DC Defender is still on the screen with the offset
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position + (Vector3)onScreenOffset);
        return viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1;
    }

    void CheckScreenBoundaries(ref Vector2 position)
    {
        // Convert the position to viewport space
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(position);

        // Check for horizontal boundaries
        if (viewportPosition.x < 0 || viewportPosition.x > 1)
        {
            // Reverse the horizontal direction
            moveDirection.x *= -1;
        }

        // Check for vertical boundaries
        if (viewportPosition.y < 0 || viewportPosition.y > 1)
        {
            // Reverse the vertical direction 
            moveDirection.y *= -1;
        }
    }

}