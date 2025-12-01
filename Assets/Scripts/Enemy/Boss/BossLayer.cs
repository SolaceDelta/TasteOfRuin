using UnityEngine;
using UnityEngine.SceneManagement;

public class BossLayer : MonoBehaviour
{
    private const float L9HP = 2500f, L8HP = 1500f, L7HP = 1000f, L6HP = 750f, L5HP = 500f, L4HP = 350f, L3HP = 250f, L2HP = 150f, L1HP = 100f;
    private const int L9AC = 50, L8AC, = 75, L7AC = 100, L6AC = 125, L5AC = 150, L4AC = 175, L3AC = 200, L2AC = 50, L1AC = 25;

    [SerializeField] private GameObject projectile;
    private BoxCollider2D view;
    private GameObject player;
    private PlayerHealth ph;

    private int atkCooldown = 0;
    private float hp;
    private float maxHP;
    
    void Start()
    {
        view = GameObject.Find("BossView").GetComponent<BoxCollider2D>();
        player = GameObject.Find("Girl");
        ph = player.GetComponent<PlayerHealth>();
        hp = maxHP = L9HP;
    }

    void Update()
    {
        if (hp <= maxHP)
        if (view.IsTouching(player.GetComponent<CapsuleCollider2D>()) && atkCooldown == 0) ShootProjectile();
        else if (atkCooldown > 0) atkCooldown--;
    }
    private void ShootProjectile()
    {
        GameObject p = Instantiate(projectile, transform.position, Quaternion.identity);
        p.GetComponent<Projectile>().Init((player.transform.position - transform.position).normalized);
        atkCooldown = 50;
    }
}
