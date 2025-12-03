using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    private AttributeController attr;
    private TextMeshProUGUI price;

    void Start() 
    {
        attr  = gameObject.GetComponent<AttributeController>();
        price = GameObject.Find("Upgrade_price").GetComponent<TextMeshProUGUI>();
        price.text = ((int) attr.attr.PERSISTENT.upgradePrice).ToString();
    }

    public void IncreaseAttribute(string attribute)
    {
        attr.LoadAttr();
        if (attr.attr.PERSISTENT.shards >= attr.attr.PERSISTENT.upgradePrice)
        {
            attr.attr.PERSISTENT.shards -= (int) attr.attr.PERSISTENT.upgradePrice;
            Debug.Log($"Increasing: {attribute}");
            
            if (attribute == "health") attr.attr.PERSISTENT.maxHealth += 5f;
            else if (attribute == "attack") attr.attr.PERSISTENT.attack += 5f;
            else if (attribute == "defense") attr.attr.PERSISTENT.defense += 1f;
            else if (attribute == "speed") attr.attr.PERSISTENT.speed += 0.1f;
            else if (attribute == "critChance") attr.attr.PERSISTENT.critChance += 0.01f;
            else if (attribute == "coinGain") attr.attr.PERSISTENT.coinGain += 0.25f;
            else if (attribute == "shardGain") attr.attr.PERSISTENT.shardGain += 1f;

            attr.attr.PERSISTENT.upgradePrice = attr.attr.PERSISTENT.upgradePrice * 1.25f > attr.attr.PERSISTENT.upgradePrice + 1f ? attr.attr.PERSISTENT.upgradePrice * 1.25f : attr.attr.PERSISTENT.upgradePrice + 1f;
            price.text = ((int) attr.attr.PERSISTENT.upgradePrice).ToString();
            attr.SaveAttr();
        }
        else GameObject.Find("MenuManager").GetComponent<NotEnoughShards>().ToggleDialogue();
    }
}
