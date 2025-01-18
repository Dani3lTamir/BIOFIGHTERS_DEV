using UnityEngine;

public class SpecialEnemy : MonoBehaviour
{
    public EnemyPool pool;

    void Start()
    {
    }

    void Update()
    {
        if (transform.position.y < -15f) //  enemy falls off-screen
        {
            int scoreValue = ScoreManager.Instance.GetObjectScore("SpecialEnemyEscape");//deduct points for uncought enemy
            ScoreManager.Instance.AddScore(scoreValue);
            pool.ReturnEnemy(gameObject);
        }
    }
}
