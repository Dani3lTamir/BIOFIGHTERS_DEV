using UnityEngine;

public class DCController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public KeyCode jumpKey = KeyCode.Space;

    private Rigidbody2D rb;
    private bool isFrozen = false;
    private bool isJumping = false;

    private AudioManager audioManager;

    void Start()
    {
        // Initialize the AudioManager
        audioManager = AudioManager.Instance;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2;
    }

    void Update()
    {
        // Movement input
        float moveX = Input.GetAxis("Horizontal");
        Vector2 moveInput = new Vector2(moveX, rb.linearVelocity.y) * moveSpeed;

        // Flip sprite based on movement direction (multiply scale by -1)
        if (moveX != 0)
        {
            Vector3 scale = transform.localScale;
            if ((moveX > 0 && scale.x > 0) || (moveX < 0 && scale.x < 0))
            {
                transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            }
        }


        // Jump input
        if (Input.GetKeyDown(jumpKey)) {
            isJumping = true;
            // Play jump sound
            audioManager.PlayAt("JumpUp", transform);
        } 
        if (Input.GetKeyUp(jumpKey)) isJumping = false;
    }

    void FixedUpdate()
    {
        if (!isFrozen)
        {
            // Apply movement
            rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, rb.linearVelocity.y);

            // Apply jump
            if (isJumping) rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
        }
    }

    public void Freeze() => isFrozen = true;
    public void Unfreeze() => isFrozen = false;
}