using UnityEngine;

public class SpecialEnemy : MonoBehaviour
{
    private EnemyPool pool;

    void Start()
    {
        // find the pool by tag
        pool = GameObject.FindGameObjectWithTag("SpecialEnemyPool").GetComponent<EnemyPool>();
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
