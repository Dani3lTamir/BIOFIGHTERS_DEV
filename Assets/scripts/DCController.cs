using UnityEngine;

public class DCController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f; // Public variable for jump force
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isFrozen = false;
    private KeyCode jumpKey = KeyCode.Space;
    private bool isJumping = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2; // Enable gravity with a positive value
    }

    void Update()
    {
        // Get input in Update
        float moveX = Input.GetAxis("Horizontal");
        moveInput = new Vector2(moveX, rb.linearVelocity.y) * moveSpeed;

        // Check for jump input
        if (Input.GetKeyDown(jumpKey))
        {
            isJumping = true;
        }
        if (Input.GetKeyUp(jumpKey))
        {
            isJumping = false;
        }
    }

    void FixedUpdate()
    {
        // Apply movement in FixedUpdate
        if (!isFrozen)
        {
            rb.linearVelocity = new Vector2(moveInput.x, rb.linearVelocity.y);

            // Apply continuous upward force if jumping
            if (isJumping)
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Force);
            }
        }
    }


    public void Freeze()
    {
        isFrozen = true;
        rb.linearVelocity = Vector2.zero;
    }

    public void Unfreeze()
    {
        isFrozen = false;
    }
}

