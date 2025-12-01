using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class OfferingController : MonoBehaviour
{
    [SerializeField] private ItemDatabase db;
    private ItemData item;
    private int price;

    void Awake()
    {
        item = RandomShopItem();
        price = item.price;
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
        else
        {
            //Indicate lack of funds somehow
        }
    }

    private ItemData RandomShopItem()
    {
        int c = db.shopItems.Count;
        if (db == null || c == 0) return null;
        return db.shopItems[Random.Range(0, c)];
    }
}