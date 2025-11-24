using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChestController : MonoBehaviour
{
    [SerializeField] private ItemDatabase db;

    public void Interact(GameObject player)
    {
        // Change from lit chest model to unlit chest model
        player.GetComponent<InventoryController>().AddItem(RandomItem());
        Destroy(gameObject.GetComponent<CircleCollider2D>());
        Destroy(this);
    }

    private ItemData RandomItem()
    {
        // choose random, ideally based on rarity
        int c = db.allItems.Count;
        if (db == null || c == 0)
            return null;
        return db.allItems[Random.Range(0, c)];
    }
}