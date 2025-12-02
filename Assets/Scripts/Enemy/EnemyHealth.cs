using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float hp = 100.0f;
    private const float maxHP = 100.0f;

    [SerializeField] private float defense = 0f;

    [SerializeField] private bool sour = false, spice = false, mint = false;
    [SerializeField] private float sourDMG = 0f, spiceDMG = 0f, mintDMG = 0f;
    [SerializeField] private int sourTime = 0, spiceTime = 0, mintTime = 0;
    private const int sourInterval = 60, spiceInterval = 30, mintInterval = 150;
    private int timeSinceSour = 0, timeSinceSpice = 0, timeSinceMint = 0;

    private EnemyController ec;
    
    private const string condErr = "|ERROR| RemoveCondition called on inactive condition: ";

    void Start()
    {
        ec = gameObject.GetComponent<EnemyController>();
        HealthToBar();
    }

    public void Damage(float dmg, bool defPierce, PlayerHealth.Condition[] conditions)
    {
        if (dmg <= 0)
        {
            Debug.LogError("|ERROR| Negative or zero damage");
            return;
        }
        if (!defPierce) dmg -= defense;
        hp -= dmg > 0 ? dmg : 0;

        foreach (PlayerHealth.Condition c in conditions) SetCondition(c);
        if (hp <= 0) Die();
    }

    public void Heal(float health)
    {
        if (health < 1)
        {
            Debug.LogError("|ERROR| Negative or zero healing");
            return;
        }

        hp = hp + health > maxHP ? maxHP : hp + health;
        //if (hp + health > maxHP) hp = maxHP;
        //else hp += health;
    }

    public void SetCondition(PlayerHealth.Condition c)
    {
        if (ApplyCondition(c.chance))
        {
            if (c.cond == "sour")
            {
                sour = true;
                sourDMG = c.dmg;
                sourTime = c.duration;
            }
            else if (c.cond == "spice")
            {
                spice = true;
                spiceDMG = c.dmg;
                spiceTime = c.duration;
            }
            else if (c.cond == "mint")
            {
                mint = true;
                mintDMG = c.dmg;
                mintTime = c.duration;
            }
            else Debug.LogError("|ERROR| Unrecognized condition: " + c.cond);
            Debug.Log($"|ENEMY_HEALTH| Set condition {c.cond} with duration: {c.duration} at {c.dmg} damage.");
        }        
    }

    public void RemoveCondition(string condition)
    {
        if (!sour && !spice && !mint)
        {
            Debug.LogError("|ERROR| RemoveCondition called with no active conditions");
            return;
        }
        if (condition == "sour")
        {
            if (!sour)
            {
                Debug.LogError(condErr + condition);
                return;
            }

            sour = false;
            sourDMG = 0f;
            sourTime = 0;
        }
        else if (condition == "spice")
        {
            if (!spice)
            {
                Debug.LogError(condErr + condition);
                return;
            }

            spice = false;
            spiceDMG = 0f;
            spiceTime = 0;
        }
        else if (condition == "mint")
        {
            if (!mint)
            {
                Debug.LogError(condErr + condition);
                return;
            }

            mint = false;
            mintDMG = 0f;
            mintTime = 0;
        }
        else Debug.LogError("|ERROR| Unrecognized condition: " + condition);
    }

    private void HealthToBar()
    {return;}//Return

    private void HandleConditions()
    {
        if (sour && sourTime == 0) RemoveCondition("sour");
        else if (sour && timeSinceSour >= sourInterval)
        {
            Damage(sourDMG, true, new PlayerHealth.Condition[] {});
            timeSinceSour = 0;
        }
        else timeSinceSour++;

        if (spice && spiceTime == 0) RemoveCondition("spice");
        else if (spice && timeSinceSpice >= spiceInterval)
        {
            Damage(spiceDMG, true, new PlayerHealth.Condition[] {});
            timeSinceSpice = 0;
        }
        else timeSinceSpice++;

        if (mint && mintTime == 0) RemoveCondition("mint");
        else if (mint && timeSinceMint >= mintInterval)
        {
            Damage(mintDMG, true, new PlayerHealth.Condition[] {});
            timeSinceMint = 0;
        }
        else timeSinceMint++;

        if (sourTime > 0) sourTime--;
        if (spiceTime > 0) spiceTime--;
        if (mintTime > 0) mintTime--;
    }

    private void Die()
    {
        //Create dead enemy prefab in scene
        ec.Reward();
        Destroy(gameObject);
    }

    private bool ApplyCondition(float chance) {return (Mathf.Round(Random.Range(0f, 1f) * 100f) / 100f) < chance;}
    void Update() {HandleConditions();}
}
