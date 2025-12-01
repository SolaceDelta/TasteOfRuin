using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string displayName;
    public string rarity;
    [TextArea] public string description;
    public int price;
    public Sprite icon;

    [Header("Basic Attributes")]
    public float maxHealth;
    public float attack;
    public float defense;
    public float speed;
    [Header("Defense Pierce")]
    public bool defPierce;

    [Header("Critical Hit")]
    public float critChance;
    public float critDamage;

    [Header("Conditions")]
    [Header("Sour")]
    public float sourChance;
    public float sourDMG;
    public int sourDuration;

    [Header("Spice")]
    public float spiceChance;
    public float spiceDMG;
    public int spiceDuration;

    [Header("Mint")]
    public float mintChance;
    public float mintDMG;
    public int mintDuration;

    [Header("Multipliers")]
    public float damageMult;
    public float speedMult;

    [Header("Econs")]
    [Header("Coins")]
    public int coins;
    public float coinGain;
    [Header("Shards")]
    public int shards;
    public float shardGain;

    public override string ToString()
    { 
        // MODIFY FOR NEW ITEM ATTRIBUTES
        string s = "";
        s += $"Name: {displayName}\n";
        s += $"Rarity: {rarity}\n";
        s += $"Description: {description}\n";
        s += $"Price: {price}\n";
        s += "Attributes:\n";
        s += $"    Max Health: {maxHealth}\n";
        s += $"    Attack: {attack}\n";
        s += $"    Defense: {defense}\n";
        s += $"    Speed: {speed}\n";
        s += $"    Crit Chance: {critChance}\n";
        s += $"    Crit Damage: {critDamage}\n";
        s += $"    Defense Pierce: {defPierce}\n";
        s += "Conditions\n";
        s += "    Sour\n";
        s += $"        Sour Chance: {sourChance}\n";
        s += $"        Sour Damage: {sourDMG}\n";
        s += $"        Sour Duration: {sourDuration}\n";
        s += "    Spice\n";
        s += $"        Spice Chance: {spiceChance}\n";
        s += $"        Spice Damage: {spiceDMG}\n";
        s += $"        Spice Duration: {spiceDuration}\n";
        s += "    Mint\n";
        s += $"        Mint Chance: {mintChance}\n";
        s += $"        Mint Damage: {mintDMG}\n";
        s += $"        Mint Duration: {mintDuration}\n";
        s += "Multipliers:\n";
        s += $"    Damage: {damageMult}\n";
        s += $"    Speed: {speedMult}";
        s += "Economies\n";
        s += "    Coins\n";
        s += $"        Coins: {coins}\n";
        s += $"        Coin Gain: {coinGain}\n";
        s += "    Shards\n";
        s += $"        Shards: {shards}\n";
        s += $"        Shard Gain: {shardGain}\n";
        return s;
    }
}