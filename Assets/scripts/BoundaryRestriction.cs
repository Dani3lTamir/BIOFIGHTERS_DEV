using UnityEngine;

public class BoundaryRestriction : MonoBehaviour
{
    public float minX = -8f; // Left boundary
    public float maxX = 8f;  // Right boundary
    public float minY = -4f; // Bottom boundary
    public float maxY = 4f;  // Top boundary

    void Update()
    {
        // Clamp the character's position within the boundaries
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
