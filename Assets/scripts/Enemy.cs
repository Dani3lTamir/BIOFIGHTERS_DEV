using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyPool pool;

    void Start()
    {
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
