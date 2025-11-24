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
    
    private const string condErr = "|ERROR| RemoveCondition called on inactive condition: ";

    void Start()
    {
        HealthToBar();
    }

    void Update()
    {
        HandleConditions();
    }

    public void Damage(float dmg, bool defPierce, Condition[] conditions)
    {
        if (dmg <= 0)
        {
            Debug.LogError("|ERROR| Negative or zero damage");
            return;
        }
        if (!defPierce)
            dmg -= defense;

        hp -= dmg;

        foreach (Condition c in conditions)
            SetCondition(c);

        if (hp <= 0)
            Die();
    }

    public void Heal(float health)
    {
        if (health < 1)
        {
            Debug.LogError("|ERROR| Negative or zero healing");
            return;
        }

        if (hp + health > maxHP)
            hp = maxHP;
        else
            hp += health;
    }

    public void SetCondition(Condition c)
    {
        if (ApplyCondition(c.chance))
        {
            switch (c.cond)
            {
                case "sour":
                    sour = true;
                    sourDMG = c.dmg;
                    sourTime = c.duration;
                    break;
                case "spice":
                    spice = true;
                    spiceDMG = c.dmg;
                    spiceTime = c.duration;
                    break;
                case "mint":
                    mint = true;
                    mintDMG = c.dmg;
                    mintTime = c.duration;
                    break;
                default:
                    Debug.LogError("|ERROR| Unrecognized condition: " + c.cond);
                    break;
            }
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
        switch (condition)
        {
            case "sour":
                if (!sour)
                {
                    Debug.LogError(condErr + condition);
                    break;
                }

                sour = false;
                sourDMG = 0f;
                sourTime = 0;
                break;
            case "spice":
                if (!spice)
                {
                    Debug.LogError(condErr + condition);
                    break;
                }

                spice = false;
                spiceDMG = 0f;
                spiceTime = 0;
                break;
            case "mint":
                if (!mint)
                {
                    Debug.LogError(condErr + condition);
                    break;
                }

                mint = false;
                mintDMG = 0f;
                mintTime = 0;
                break;
            default:
                Debug.LogError("|ERROR| Unrecognized condition: " + condition);
                break;
        }
    }

    private void HealthToBar()
    {return;}

    public struct Condition
    {
        public string cond;
        public float chance;
        public float dmg;
        public int duration;

        public Condition(string cond, float chance, float dmg, int duration)
        {
            this.cond = cond;
            this.chance = chance;
            this.dmg = dmg;
            this.duration = duration;
        }
    }

    private bool ApplyCondition(float chance)
    {
        return (Mathf.Round(Random.Range(0f, 1f) * 100f) / 100f) < chance;
    }

    private void HandleConditions()
    {
        if (sour && sourTime == 0)
            RemoveCondition("sour");
        else if (sour && timeSinceSour >= sourInterval)
        {
            Damage(sourDMG, true, new Condition[] {});
            timeSinceSour = 0;
        }
        else
            timeSinceSour++;

        if (spice && spiceTime == 0)
            RemoveCondition("spice");
        else if (spice && timeSinceSpice >= spiceInterval)
        {
            Damage(spiceDMG, true, new Condition[] {});
            timeSinceSpice = 0;
        }
        else
            timeSinceSpice++;

        if (mint && mintTime == 0)
            RemoveCondition("mint");
        else if (mint && timeSinceMint >= mintInterval)
        {
            Damage(mintDMG, true, new Condition[] {});
            timeSinceMint = 0;
        }
        else
            timeSinceMint++;

        if (sourTime > 0)
            sourTime--;

        if (spiceTime > 0)
            spiceTime--;

        if (mintTime > 0)
            mintTime--;
    }

    private void Die()
    {
        //Create dead enemy prefab in scene
        Destroy(gameObject);
    }
}
