using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "InventoryDatabase", menuName = "ScriptableObjects/InventoryDatabase", order = 1)]
public class InventoryDatabase : ScriptableObject
{
    [SerializeField]
    private List<ItemData> items = new List<ItemData>();
    public IReadOnlyList<ItemData> Items => items;
    
    public void AddItem(ItemData item)
    {
        if (item == null) return;
        if (items.Contains(item)) return;

        items.Add(item);
    }
    
    public void RemoveItem(ItemData item)
    {
        if (item == null) return;
        if (!items.Contains(item)) return;

        items.Remove(item);
    }
    
    public void RemoveAllItems()
    {
        items.Clear();
    }
}