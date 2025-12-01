using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChestController : MonoBehaviour
{
    [SerializeField] private ItemDatabase db;

    public void Interact(GameObject player)
    {
        player.GetComponent<InventoryController>().AddItem(RandomItem());
        Destroy(gameObject.GetComponent<CircleCollider2D>());
        Destroy(this);
    }

    private ItemData RandomItem()
    {
        int c = db.allItems.Count;
        if (db == null || c == 0) return null;
        return db.allItems[Random.Range(0, c)];
    }
}