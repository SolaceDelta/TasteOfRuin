using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform enemies;
    public GameObject enemy;
    //public GameObject enemy1;
    //public GameObject enemy2;
    void Start()
    {
        enemies = GameObject.Find("Enemies").transform;
        Instantiate(enemy, transform.position, Quaternion.identity, enemies);
    }
}
