using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyPool pool;

    void Start()
    {
        // find the pool by tag
        pool = GameObject.FindGameObjectWithTag("EnemyPool").GetComponent<EnemyPool>();
    }

    void Update()
    {
        if (transform.position.y < -15f) //  enemy falls off-screen
        {
            ScoreManager.Instance.UpdateScoreForObject("EnemyEscape");
            pool.ReturnEnemy(gameObject);
        }
    }
}
