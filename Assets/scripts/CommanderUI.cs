using UnityEngine;

public class CommanderUI : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the commander moves

    private float targetX; // Target X position to move towards

    void Start()
    {
        // Set the initial target X position
        targetX = transform.position.x;
    }

    void Update()
    {
        // Smoothly move towards the target X position
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void MoveToButton(float xPosition)
    {
        // Set the target X position
        targetX = xPosition;
    }
}