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

    [Header("Defense Pierce")]
    public bool defPierce;

    [Header("Multipliers")]
    public float damageMult = 1f;
    public float speedMult = 1f;

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
        s += "Multipliers:\n";
        s += $"    Damage: {damageMult}\n";
        s += $"    Speed: {speedMult}";
        return s;
    }
}