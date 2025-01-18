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
            int scoreValue = ScoreManager.Instance.GetObjectScore("EnemyEscape");//deduct points for uncought enemy
            ScoreManager.Instance.AddScore(scoreValue);
            pool.ReturnEnemy(gameObject);
        }
    }
}
