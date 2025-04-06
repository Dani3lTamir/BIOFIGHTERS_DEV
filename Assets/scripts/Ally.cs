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
        if (transform.position.y < -15f) 
        {
            pool.ReturnAlly(gameObject);
        }
    }
}
