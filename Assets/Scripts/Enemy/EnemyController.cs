using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 6f, strength = 10f;

    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private BoxCollider2D view;

    private GameObject player;
    private Transform playerPos;
    private CapsuleCollider2D playerCollider;
    private AttributeController attr;

    private int atkCooldown  = 0;
    private int maxAC = 75;
    private bool facingRight = true;
    private float groundY, dir;
    public float enemyBonus = 0.3f;

    void Start()
    {
        rb      = gameObject.GetComponent<Rigidbody2D>();
        col     = gameObject.GetComponent<CapsuleCollider2D>();
        view    = transform.GetChild(0).GetComponent<BoxCollider2D>();
        groundY = transform.GetChild(1).position.y;

        player         = GameObject.Find("Girl");
        playerPos      = player.transform;
        playerCollider = player.GetComponent<CapsuleCollider2D>();
        attr           = player.GetComponent<AttributeController>();
        
        strength += 2f * attr.attr.PERSISTENT.level;
        speed += 0.25f * attr.attr.PERSISTENT.level;
        maxAC -= (int) 0.05f * attr.attr.PERSISTENT.level;
    }

    private bool Grounded(Vector3 point)
    {
        float offsetX  = facingRight ? -0.5f : 0.5f;
        Vector2 origin = new Vector2(point.x + offsetX, groundY);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1.5f, groundLayer);
        Debug.DrawRay(origin, Vector2.down * 1.5f, Color.red);

        return hit.collider != null;
    }

    private void Chase()
    {
        if (view.IsTouching(playerCollider) && atkCooldown < 50)
        {
            dir            = transform.position.x - playerPos.position.x;
            Vector3 target = Vector3.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);
            target.y       = transform.position.y;

            if (Grounded(target))
                transform.position = target;
            else
                rb.linearVelocity = Vector2.zero;

            FlipSprite();
        }
        else rb.linearVelocity = Vector2.zero;
        Attack();
    }

    private void Attack()
    {
        if (col.IsTouching(playerCollider) && atkCooldown == 0)
        {
            player.GetComponent<PlayerHealth>().Damage(strength, false, new PlayerHealth.Condition[] {});
            atkCooldown = maxAC;
        }
    }

    private void FlipSprite()
    {
        if (facingRight && dir < 0 || !facingRight && dir > 0)
        {
            facingRight          = !facingRight;
            Vector3 localScale   = transform.localScale;
            localScale.x        *= -1f;
            transform.localScale = localScale;
        }
    }
    public void Reward() {player.GetComponent<PlayerController>().Reward(enemyBonus);}
    void FixedUpdate() {if (atkCooldown > 0) atkCooldown--;}
    void Update() {Chase();}
}