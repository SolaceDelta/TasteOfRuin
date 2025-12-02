using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer, enemyLayer, interactLayer, bossLayer;

    private AttributeController attr;
    private PlayerControls controls;
    private Rigidbody2D rb;
    private Transform groundChecker, actuator;

    private InputActionMap pMap, uiMap;
    private InputAction move;

    private int atkCooldown = 0;
    private bool facingRight = true;
    private float hIn;

    private bool paused = false;
    private GameObject pauseMenu;

    private void Awake()
    {
        controls = new PlayerControls();
        uiMap    = controls.UI;
        pMap     = controls.Player;
        move     = pMap.FindAction("Move");
        
        pauseMenu = GameObject.Find("Pause_Menu");
        pauseMenu.SetActive(paused);
    }

    void Start()
    {
        attr          = gameObject.GetComponent<AttributeController>();
        rb            = gameObject.GetComponent<Rigidbody2D>();
        groundChecker = transform.GetChild(0);
        actuator      = transform.GetChild(1);
        Cursor.visible = paused;
    }

    void Update()
    {
        hIn = move.ReadValue<Vector2>().x;
        FlipSprite();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(hIn * (attr.attr.RUN.speed * attr.attr.RUN.speedMult), rb.linearVelocity.y);
        if (atkCooldown > 0) atkCooldown--;
    }

    private void FlipSprite()
    {
        if (facingRight && hIn < 0f || !facingRight && hIn > 0f)
        {
            facingRight          = !facingRight;
            Vector3 localScale   = transform.localScale;
            localScale.x        *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnAttack()
    {
        if (atkCooldown == 0)
        {
            PlayerHealth.Condition[] cond = new PlayerHealth.Condition[]
            {
                new PlayerHealth.Condition("sour", attr.attr.RUN.sourChance, attr.attr.RUN.sourDMG, attr.attr.RUN.sourDuration),
                new PlayerHealth.Condition("spice", attr.attr.RUN.spiceChance, attr.attr.RUN.spiceDMG, attr.attr.RUN.spiceDuration),
                new PlayerHealth.Condition("mint", attr.attr.RUN.mintChance, attr.attr.RUN.mintDMG, attr.attr.RUN.mintDuration),
            };
            //play animation
            List<Collider2D> hits = new List<Collider2D>();
            hits.AddRange(Physics2D.OverlapCircleAll(actuator.position, 1f, enemyLayer));
            hits.AddRange(Physics2D.OverlapCircleAll(actuator.position, 1f, bossLayer));
            foreach (Collider2D hit in hits)
            {
                Debug.Log(hit);
                EnemyHealth eh = hit.gameObject.GetComponent<EnemyHealth>();
                Boss b = hit.gameObject.GetComponentInParent<Boss>();
                if (eh != null) eh.Damage(RollDamage(), attr.attr.RUN.defPierce, cond);
                else if (b != null) b.Damage(RollDamage(), attr.attr.RUN.defPierce, cond);
                else Debug.LogError($"|ERROR| Captured non-enemy, non-boss object in attack: {hit}");
                atkCooldown = eh != null || b != null ? 30 : 0;
            }
        }
    }

    private void OnInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f, interactLayer);

        if (hits.Length > 0)
        {
            GameObject obj = hits[0].gameObject;
            Debug.Log($"|INTERACT| Detected: {hits.Length}\nInteracted: {obj.name}");

            if (obj.name == "Fountain") obj.GetComponent<FountainController>().Interact(gameObject);
            else if (obj.name == "Chest") obj.GetComponent<ChestController>().Interact(gameObject);
            else if (obj.name == "Door") obj.GetComponent<DoorController>().Interact(gameObject);
            else if (obj.name == "OfferPedestal") obj.GetComponent<OfferingController>().Interact(gameObject);
            else Debug.LogError("|ERROR| Non-Interactable GameObject on interactLayer");
        }
    }
    
    public void OnPause()
    {
        paused = !paused;
        Time.timeScale = Convert.ToInt32(!paused);
        Cursor.visible = paused;
        if (paused)
        {
            pMap.Disable();
            uiMap.Enable();
        }
        else
        {
            uiMap.Disable();
            pMap.Enable();
        }
        pauseMenu.SetActive(paused);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private float RollDamage()
    {
        float min = 0.5f * attr.attr.RUN.attack;
        float max = 1.5f * attr.attr.RUN.attack;
        if (Crit()) return (Mathf.Round(Random.Range(min, max) * 100f) / 100f) * attr.attr.RUN.damageMult * attr.attr.RUN.critDamage;
        else return (Mathf.Round(Random.Range(min, max) * 100f) / 100f) * attr.attr.RUN.damageMult;
    }

    public void Reward(float enemyBonus)
    {
        attr.GainCoins((int) ((attr.attr.RUN.coinGain + enemyBonus) * Random.Range(0, 10) + attr.attr.RUN.level));
        attr.GainShards((int) ((attr.attr.RUN.shardGain + enemyBonus) * Random.Range(1, 3) + attr.attr.RUN.level));
        attr.LevelUp((int) (attr.attr.RUN.level + 1 + enemyBonus) * Random.Range(1, 25));
    }

    private void OnJump() {if (Grounded() && !paused) rb.linearVelocity = new Vector2(rb.linearVelocity.x, attr.attr.RUN.jumpPower);}
    private bool Crit() {return (Mathf.Round(Random.Range(0f, 1f) * 100f) / 100f) < attr.attr.RUN.critChance;}
    private bool Grounded() {return Physics2D.OverlapCircle(groundChecker.position, 0.2f, groundLayer);}
    public void DisableUIControls() {uiMap.Disable();}
    public bool IsFacingRight() {return facingRight;}
    private void OnDisable() {pMap.Disable();}
    private void OnEnable() {pMap.Enable();}
}