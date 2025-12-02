using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private bool sour = false, spice = false, mint = false;
    [SerializeField] private float sourDMG = 0f, spiceDMG = 0f, mintDMG = 0f;
    [SerializeField] private int sourTime = 0, spiceTime = 0, mintTime = 0;
    private const int sourInterval = 60, spiceInterval = 30, mintInterval = 150;
    private int timeSinceSour = 0, timeSinceSpice = 0, timeSinceMint = 0;
    private const string condErr = "|ERROR| RemoveCondition called on inactive condition: ";

    public TextMeshProUGUI ui;
    public GameObject healthBar;
    private AttributeController attr;

    private float oldAtk;
    private float oldSpd;

    void Start()
    {
        attr = gameObject.GetComponent<AttributeController>();
        HealthToHUD();
    }

    void FixedUpdate()
    {
        HandleConditions();
        HealthToHUD();
    }

    public void Damage(float dmg, bool defPierce, Condition[] conditions)
    {
        if (dmg < 1)
        {
            Debug.LogError("|ERROR| Negative or zero damage");
            return;
        }
        if (!defPierce) dmg -= attr.attr.RUN.defense;
        attr.attr.RUN.health -= dmg > 0 ? dmg : 0;
        attr.SaveAttr();
        foreach (Condition c in conditions) SetCondition(c);
        if (attr.attr.RUN.health <= 0) gameObject.GetComponent<PlayerController>().Lose();
    }

    public void Heal(float heal)
    {
        if (heal <= 0)
        {
            Debug.LogError("|ERROR| Negative or zero healing");
            return;
        }

        attr.attr.RUN.health = attr.attr.RUN.health + heal > attr.attr.RUN.maxHealth ? attr.attr.RUN.maxHealth : attr.attr.RUN.health + heal;
        attr.SaveAttr();
    }

    public void SetCondition(Condition c)
    {
        if (ApplyCondition(c.chance))
        {
            if (c.cond == "sour")
            {
                if (!sour)
                {
                    oldAtk = attr.attr.RUN.attack;
                    attr.attr.RUN.attack *= 0.75f;
                    attr.SaveAttr();
                }
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
                if (!mint)
                {
                    oldSpd = attr.attr.RUN.speed;
                    attr.attr.RUN.speed *= 0.25f;
                    attr.SaveAttr();
                }
                mint = true;
                mintDMG = c.dmg;
                mintTime = c.duration;
            }
            else Debug.LogError("|ERROR| Unrecognized condition: " + c.cond);
            Debug.Log($"|PLAYER_HEALTH| Set condition {c.cond} with duration: {c.duration} seconds at {c.dmg} damage.");
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
            attr.attr.RUN.attack = oldAtk;
            attr.SaveAttr();
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
            attr.attr.RUN.speed = oldSpd;
            attr.SaveAttr();
        }
        else Debug.LogError("|ERROR| Unrecognized condition: " + condition);
    }

    private void HealthToHUD()
    {
        try
        {
            ui.GetComponent<TextMeshProUGUI>().text = attr.attr.RUN.health.ToString();
            RectTransform rt = healthBar.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2((attr.attr.RUN.health / attr.attr.RUN.maxHealth) * 200, rt.sizeDelta.y);
        }
        catch (Exception e)
        {
            Debug.LogError("|ERROR| " + e.ToString());
        }
    }
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

    private void HandleConditions()
    {
        if (sour && sourTime == 0) RemoveCondition("sour");
        else if (sour && timeSinceSour >= sourInterval)
        {
            Damage(sourDMG, true, new Condition[] {});
            timeSinceSour = 0;
        }
        else timeSinceSour++;

        if (spice && spiceTime == 0) RemoveCondition("spice");
        else if (spice && timeSinceSpice >= spiceInterval)
        {
            Damage(spiceDMG, true, new Condition[] {});
            timeSinceSpice = 0;
        }
        else timeSinceSpice++;

        if (mint && mintTime == 0) RemoveCondition("mint");
        else if (mint && timeSinceMint >= mintInterval)
        {
            Damage(mintDMG, true, new Condition[] {});
            timeSinceMint = 0;
        }
        else timeSinceMint++;

        if (sourTime > 0) sourTime--;
        if (spiceTime > 0) spiceTime--;
        if (mintTime > 0) mintTime--;
    }
    
    private bool ApplyCondition(float chance) {return (Mathf.Round(Random.Range(0f, 1f) * 100f) / 100f) < chance;}
}
