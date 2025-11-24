using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName= "Item Database", menuName = "Game/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> allItems = new List<ItemData>();
    public List<ItemData> shopItems = new List<ItemData>();
}