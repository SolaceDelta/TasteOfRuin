using UnityEngine;

public class Projectile : MonoBehaviour
{
    private readonly float[] LS = {30f, 10f, 12f, 15f, 10f, 15f, 20f, 10f, 15f};
    private readonly float[] LD = {15f, 10f, 8f, 6f, 7f, 5f, 8f, 10f, 15f};
    private static readonly PlayerHealth.Condition sour  = new PlayerHealth.Condition("sour", 1f, 1f, 120);
    private static readonly PlayerHealth.Condition spice = new PlayerHealth.Condition("spice", 1f, 5f, 90);
    private static readonly PlayerHealth.Condition mint  = new PlayerHealth.Condition("mint", 1f, 3f, 150);
    private readonly PlayerHealth.Condition[][] conds = 
    {
        new PlayerHealth.Condition[] {},
        new PlayerHealth.Condition[] {sour, spice, mint},
        new PlayerHealth.Condition[] {spice, mint},
        new PlayerHealth.Condition[] {sour, mint},
        new PlayerHealth.Condition[] {mint},
        new PlayerHealth.Condition[] {spice},
        new PlayerHealth.Condition[] {sour},
        new PlayerHealth.Condition[] {},
        new PlayerHealth.Condition[] {}
    };
    private Vector2 dir;
    private int stage;

    public void Init(Vector2 d, int s) 
    {
        dir = d;
        stage = s;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().Damage(LD[stage - 1], false, conds[stage - 1]);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall")) Destroy(gameObject);
    }

    void Update() {transform.position += (Vector3)(dir * LS[stage - 1] * Time.deltaTime);}
}
