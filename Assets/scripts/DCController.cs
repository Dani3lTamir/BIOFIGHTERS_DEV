using UnityEngine;

public class DCController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Ensure gravity is disabled
    }

    void Update()
    {
        // Get input in Update
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        moveInput = new Vector2(moveX, moveY) * moveSpeed;
    }

    void FixedUpdate()
    {
        // Apply movement in FixedUpdate
        rb.linearVelocity = moveInput;
    }
}
