using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform enemies;
    public GameObject enemy;
    void Start()
    {
        enemies = GameObject.Find("Enemies").transform;
        Instantiate(enemy, transform.position, Quaternion.identity, enemies);
    }
}
