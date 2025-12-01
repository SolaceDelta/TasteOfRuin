using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 15f;
    private float dmg = 1f;
    private Vector2 direction;

    public void Init(Vector2 dir) {direction = dir;}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().Damage(dmg, false, new PlayerHealth.Condition[] {});
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    void Update() {transform.position += (Vector3)(direction * speed * Time.deltaTime);}
}
