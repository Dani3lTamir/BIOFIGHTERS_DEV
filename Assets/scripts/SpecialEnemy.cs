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
            ScoreManager.Instance.UpdateScoreForObject("SpecialEnemyEscape");
            pool.ReturnEnemy(gameObject);
        }
    }
}
