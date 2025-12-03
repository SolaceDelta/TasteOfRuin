using System;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class OfferingController : MonoBehaviour
{
    [SerializeField] private ItemDatabase db;
    private ItemData item;
    private int price;

    void Awake()
    {
        item = RandomShopItem();
        price = item.price;
        transform.Find("PriceBacking").transform.Find("Price").GetComponent<TextMeshPro>().text = price.ToString();
    }

    public void Interact(GameObject player)
    {
        AttributeController attr = player.GetComponent<AttributeController>();
        
        if (attr.attr.RUN.coins >= price)
        {
            player.GetComponent<InventoryController>().AddItem(item);
            attr.attr.RUN.coins -= price;
            //Remove item and price but maintain structure
            Destroy(transform.GetChild(1).gameObject);
            Destroy(transform.GetChild(2).gameObject);
            Destroy(gameObject.GetComponent<BoxCollider2D>());
            Destroy(this);
        }
    }

    private ItemData RandomShopItem()
    {
        int c = db.shopItems.Count;
        if (db == null || c == 0) return null;
        return db.shopItems[Random.Range(0, c)];
    }
}