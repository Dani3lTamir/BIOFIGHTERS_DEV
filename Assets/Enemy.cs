using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyPool pool;

    void Start()
    {
        pool = FindFirstObjectByType<EnemyPool>();
    }

    void Update()
    {
        if (transform.position.y < -15f) // Example condition: enemy falls off-screen
        {
            pool.ReturnEnemy(gameObject);
        }
    }
}
