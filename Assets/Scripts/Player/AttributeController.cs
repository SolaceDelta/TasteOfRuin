using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class AttributeController : MonoBehaviour
{
    [System.Serializable]
    public class Attributes
    {
        // Basic Attributes
        public float health;
        public float maxHealth;
        public float attack;
        public float defense;
        public float speed;
        public float jumpPower;
        // Defense Pierce
        public bool defPierce;

        // Critical Hit
        public float critChance;
        public float critDamage;
        
        // Conditions
        // Sour
        public float sourChance;
        public float sourDMG;
        public int sourDuration;
        // Spice
        public float spiceChance;
        public float spiceDMG;
        public int spiceDuration;
        // Mint
        public float mintChance;
        public float mintDMG;
        public int mintDuration;
        
        // Multipliers
        public float damageMult;
        public float speedMult;

        // Econs
        // Coins
        public int coins;
        public float coinGain;
        // Shards
        public int shards;
        public float shardGain;
        // Experience
        public int exp;
        public int targetExp;
        public int level;

        public float upgradePrice;

        public void Add(ItemData id)
        {
            // Basic Attributes
            maxHealth += id.maxHealth;
            health += id.maxHealth;
            attack += id.attack;
            defense += id.defense;
            speed += id.speed;
            
            // Critical Hit
            critChance += id.critChance;
            critDamage += id.critDamage;
            
            // Conditions
            // Sour
            sourChance += id.sourChance;
            sourDMG += id.sourDMG;
            sourDuration += id.sourDuration;
            // Spice
            spiceChance += id.spiceChance;
            spiceDMG += id.spiceDMG;
            spiceDuration += id.spiceDuration;
            // Mint
            mintChance += id.mintChance;
            mintDMG += id.mintDMG;
            mintDuration += id.mintDuration;
            
            // Multipliers
            damageMult += id.damageMult;
            speedMult += id.speedMult;

            // Econs
            // Coins
            coins += id.coins;
            coinGain += id.coinGain;
            //Shards
            shards += id.shards;
            shardGain += id.shardGain;
        }

        public void GainCoins(int gained)
        {
            coins += gained;
            Debug.Log($"|ATTRIBUTES| GainCoins Ran >\n\tGained Coins: {gained}\n\tCurrent Coins: {coins}");
        }

        public Attributes Clone() {return (Attributes) this.MemberwiseClone();}
    }

    public class AttributeContainer
    {
        public Attributes PERSISTENT;
        public Attributes RUN;

        public void LevelUp(int gained)
        {
            RUN.exp += gained;
            PERSISTENT.exp += gained;
            if (RUN.exp >= RUN.targetExp)
            {
                RUN.exp -= RUN.targetExp;
                PERSISTENT.exp -= PERSISTENT.targetExp;
                RUN.targetExp *= 2;
                PERSISTENT.targetExp *= 2;
                RUN.level++;
                PERSISTENT.level++;
            }
            Debug.Log($"|ATTRIBUTES| LevelUp Ran >\n\tGained Exp: {gained}\n\tCurrent Exp: {RUN.exp}\n\tCurrent Level: {RUN.level}");
        }
        public void GainShards(int gained)
        {
            RUN.shards += gained;
            PERSISTENT.shards += gained;
            Debug.Log($"|ATTRIBUTES| GainShards Ran >\n\tGained Shards: {gained}\n\tCurrent Shards: {RUN.shards}");
        }
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
            damageMult = 1f,
            speedMult = 1f,
            coinGain = 1.5f,
            shards = 0,
            shardGain = 1.5f,
            exp = 0,
            targetExp= 125,
            level = 0,
            upgradePrice = 1f
        };
    }

    public void BeginRun()
    {
        if (attr.PERSISTENT == null)
        {
            Debug.LogError("|ERROR| Cannot begin run, persistent data missing!");
            return;
        }
        if (attr.RUN != null) {return;}

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

    public void ApplyItem(ItemData id)
    {
        attr.RUN.Add(id);
        SaveAttr();
        Debug.Log($"|ATTRIBUTES| Applied item {id.displayName} to player.");
    }

    public void GainCoins(int gained)
    {
        attr.RUN.GainCoins(gained);
        SaveAttr();
    }

    public void LevelUp(int gained)
    {
        attr.LevelUp(gained);
        SaveAttr();
    }

    public void GainShards(int gained)
    {
        attr.GainShards(gained);
        SaveAttr();
    }

    public void SaveAttr() {File.WriteAllText(filePath, JsonConvert.SerializeObject(attr, Formatting.Indented, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));}
    public void LoadAttr() {attr = File.Exists(filePath) ? JsonConvert.DeserializeObject<AttributeContainer>(File.ReadAllText(filePath)) : new AttributeContainer();}
    public void PrintJson() {Debug.Log(JsonUtility.ToJson(attr, true));}
}