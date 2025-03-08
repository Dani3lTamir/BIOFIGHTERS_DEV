using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target the camera will follow
    public Vector3 offset; // Offset from the target
    public float smoothSpeed = 0.125f; // Smooth speed for camera movement

    private float initialZ; // Initial Z position of the camera

    void Start()
    {
        // Store the initial Z position of the camera
        initialZ = transform.position.z;
    }

    void Update()
    {
        Vector3 desiredPosition = target.position + offset;
        // Ensure the Z position remains constant
        desiredPosition.z = initialZ;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}