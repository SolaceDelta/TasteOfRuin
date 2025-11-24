using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class AttributeController : MonoBehaviour
{
    [System.Serializable]
    public class Attributes
    {
        public float health;
        public float maxHealth;

        public float attack;
        public float defense;
        public float speed;
        public float jumpPower;
        
        public float critChance;
        public float critDamage;
        
        public float sourChance;
        public float sourDMG;
        public int sourDuration;
        
        public float spiceChance;
        public float spiceDMG;
        public int spiceDuration;
        
        public float mintChance;
        public float mintDMG;
        public int mintDuration;
        
        public float damageMult;
        public float speedMult;

        public int coins;
        public float coinGain;
        public int shards;
        public float shardGain;

        public bool defPierce;

        public int upgradePrice;

        public Attributes Clone() {return (Attributes) this.MemberwiseClone();}
    }

    public class AttributeContainer
    {
        public Attributes PERSISTENT;
        public Attributes RUN;
    }

    private string filePath;
    public AttributeContainer attr;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerAttributes.json");
        LoadAttr();

        if (attr.PERSISTENT == null)
        {
            Debug.Log("|ATTRIBUTES| No persistent data found, creating default attributes.");
            attr.PERSISTENT = PersistentAttr();
            SaveAttr();
        }
        EndRun();
        BeginRun();
    }

    private Attributes PersistentAttr()
    {
        return new Attributes
        {
            maxHealth = 100f,
            attack = 20f,
            defense = 1f,
            speed = 8f,
            jumpPower = 12f,
            critChance = 0.01f,
            critDamage = 2f,
            sourChance = 0f,
            sourDMG = 0f,
            sourDuration = 0,
            spiceChance = 0f,
            spiceDMG = 0f,
            spiceDuration = 0,
            mintChance = 0f,
            mintDMG = 0f,
            mintDuration = 0,
            damageMult = 1f,
            speedMult = 1f,
            coinGain = 0f,
            shards = 0,
            shardGain = 0f,
            defPierce = false,
            upgradePrice = 1
        };
    }

    public void BeginRun()
    {
        if (attr.PERSISTENT == null)
        {
            Debug.LogError("|ERROR| Cannot begin run, persistent data missing!");
            return;
        }
        if (attr.RUN != null)
        {
            Debug.LogError("|ERROR| Cannot begin run, existing run data!");
            return;
        }

        attr.RUN = attr.PERSISTENT.Clone();
        attr.RUN.health = attr.RUN.maxHealth;
        attr.RUN.coins = 0;
        SaveAttr();
        Debug.Log("|ATTRIBUTES| Run entry created.");
    }

    public void EndRun()
    {
        if (attr.RUN == null)
        {
            Debug.LogError("|ERROR| Cannot end run, no run found!");
            return;
        }

        attr.RUN = null;
        SaveAttr();
        Debug.Log("|ATTRIBUTES| Run entry removed.");
    }

    public void LoadAttr()
    {
        /*
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            attr = JsonUtility.FromJson<AttributeContainer>(json);
        }
        else
        {
            attr = new AttributeContainer();
            SaveAttr();
        }
        */
        attr = File.Exists(filePath) ? JsonUtility.FromJson<AttributeContainer>(File.ReadAllText(filePath)) : new AttributeContainer();
        SaveAttr();
    }

    public void ApplyItem(ItemData id)
    {
        // Basic Attributes
        if (id.maxHealth != -1)
        {
            attr.RUN.maxHealth += id.maxHealth;
            attr.RUN.health += id.maxHealth;
        }

        if (id.attack != -1)  attr.RUN.attack += id.attack;
        if (id.defense != -1) attr.RUN.defense += id.defense;
        if (id.speed != -1)   attr.RUN.speed += id.speed;
        
        // Critical Hit
        if (id.critChance != -1) attr.RUN.critChance += id.critChance;
        if (id.critDamage != -1) attr.RUN.critDamage += id.critDamage;
        
        // Conditions
        // Sour
        if (id.sourChance != -1)   attr.RUN.sourChance += id.sourChance;
        if (id.sourDMG != -1)      attr.RUN.sourDMG += id.sourDMG;
        if (id.sourDuration != -1) attr.RUN.sourDuration += id.sourDuration;
        // Spice
        if (id.spiceChance != -1)   attr.RUN.spiceChance += id.spiceChance;
        if (id.spiceDMG != -1)      attr.RUN.spiceDMG += id.spiceDMG;
        if (id.spiceDuration != -1) attr.RUN.spiceDuration += id.spiceDuration;
        // Mint
        if (id.mintChance != -1)   attr.RUN.mintChance += id.mintChance;
        if (id.mintDMG != -1)      attr.RUN.mintDMG += id.mintDMG;
        if (id.mintDuration != -1) attr.RUN.mintDuration += id.mintDuration;
        
        // Defense Pierce
        if (id.defPierce) attr.RUN.defPierce = true;

        // Multipliers
        if (id.damageMult != -1) attr.RUN.damageMult += id.damageMult;
        if (id.speedMult != -1)  attr.RUN.speedMult += id.speedMult;
        
        SaveAttr();
        Debug.Log($"|ATTRIBUTES| Applied item {id.displayName} to player.");
    }

    public void SaveAttr() {File.WriteAllText(filePath, JsonUtility.ToJson(attr, true));}
    private void PrintJson() {Debug.Log(JsonUtility.ToJson(attr, true));}
}