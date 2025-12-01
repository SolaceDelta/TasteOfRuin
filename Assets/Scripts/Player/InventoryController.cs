using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private List<(ItemData item, int count)> inventory;
    private AttributeController attr;

    void Awake()
    {
        inventory = new List<(ItemData item, int count)>();
        attr = gameObject.GetComponent<AttributeController>();
    }

    public void AddItem(ItemData i)
    {
        if (i != null)
        {
            if (!inventory.Any(p => p.item == i))
            {
                inventory.Add((i, 1));
                LogInv((i, 1));
            }
            else
            {
                int ind = inventory.FindIndex(p => p.item == i);
                if (ind != -1)
                {
                    inventory[ind] = (i, inventory[ind].count + 1);
                    LogInv(inventory[ind]);
                }
            }
            attr.ApplyItem(i);
        }
        else Debug.LogError("|ERROR| Cannot add null as an item!");
    }

    private void LogInv((ItemData i, int c) log)
    {
        Debug.Log($"|INVENTORY| Added {log.i.displayName}");
        foreach ((ItemData item, int count) id in inventory) Debug.Log(id.item.ToString() + $"\n{id.count}");
    }
}