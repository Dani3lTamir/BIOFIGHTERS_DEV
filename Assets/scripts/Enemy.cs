using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyPool pool;

    void Start()
    {
    }

    void Update()
    {
        if (transform.position.y < -15f) // Example condition: enemy falls off-screen
        {
            pool.ReturnEnemy(gameObject);
        }
    }
}
