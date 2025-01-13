using UnityEngine;

public class Ally : MonoBehaviour
{
    private AllyPool pool;

    void Start()
    {
        pool = FindFirstObjectByType<AllyPool>();
    }

    void Update()
    {
        if (transform.position.y < -15f) // Example condition: Ally falls off-screen
        {
            pool.ReturnAlly(gameObject);
        }
    }
}
