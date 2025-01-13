using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(moveX, moveY) * speed * Time.deltaTime;
        transform.Translate(move);
    }
}
