using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryDatabase", menuName = "ScriptableObjects/InventoryDatabase", order = 1)]
public class InventoryDatabase : ScriptableObject
{
    private List<ItemData> _items = new List<ItemData>();
    public IReadOnlyList<ItemData> Items => _items;
    
    public void AddItem(ItemData item)
    {
        if (item == null) return;
        if (_items.Contains(item)) return;

        _items.Add(item);
    }
    
    public void RemoveItem(ItemData item)
    {
        if (item == null) return;
        if (!_items.Contains(item)) return;

        _items.Remove(item);
    }
    
    public void RemoveAllItems()
    {
        _items.Clear();
    }
}