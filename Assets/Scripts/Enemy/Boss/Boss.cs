using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    private readonly float[] LHP = {100f, 150f, 250f, 350f, 500f, 750f, 1000f, 1500f, 2500f};
    private readonly int[] LAC = {25, 50, 200, 175, 150, 125, 100, 75, 50};
    private readonly float[] LDF = {10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f, 50f};
    
    [SerializeField] private GameObject[] layers;
    [SerializeField] private GameObject[] projectiles;

    private BoxCollider2D view;
    private GameObject player;
    private PlayerHealth ph;
    private int stage;

    private int atkCooldown = 0;
    private float hp;
    private float maxHP;
    
    void Start()
    {
        view = GameObject.Find("BossView").GetComponent<BoxCollider2D>();
        player = GameObject.Find("Girl");
        stage = 9;
        ph = player.GetComponent<PlayerHealth>();
        hp = maxHP = LHP[stage - 1];
    }

    void Update()
    {
        if (view.IsTouching(player.GetComponent<CapsuleCollider2D>()) && atkCooldown == 0) ShootProjectile();
        else if (atkCooldown > 0) atkCooldown--;
    }
    private void ShootProjectile()
    {
        GameObject projectile;
        projectile = projectiles[stage - 1];
        atkCooldown = LAC[stage - 1];
        GameObject p = Instantiate(projectile, transform.position, Quaternion.identity);
        p.GetComponent<Projectile>().Init((player.transform.position - transform.position).normalized, stage);
    }

    public void Damage(float dmg, bool defPierce, PlayerHealth.Condition[] conditions)
    {
        if (dmg <= 0)
        {
            Debug.LogError("|ERROR| Negative or zero damage");
            return;
        }
        if (!defPierce) dmg -= LDF[stage - 1];
        hp -= dmg > 0 ? dmg : 0;
        if (hp <= 0) NextStage();
    }

    private void NextStage()
    {
        Destroy(layers[stage - 1]);
        stage--;
        if (stage <= 0) 
        {
            player.GetComponent<PlayerController>().Win();
            Destroy(this);
        }
        else hp = maxHP = LHP[stage - 1];
    }
}
