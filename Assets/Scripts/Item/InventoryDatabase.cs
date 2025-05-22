using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "InventoryDatabase", menuName = "ScriptableObjects/InventoryDatabase", order = 1)]
public class InventoryDatabase : ScriptableObject
{
    [SerializeField]
    private List<TradeItemData> items = new List<TradeItemData>();
    public IReadOnlyList<TradeItemData> Items => items;
    
    public void AddItem(TradeItemData item)
    {
        if (item == null) return;
        if (items.Contains(item)) return;

        items.Add(item);
    }
    
    public void RemoveItem(TradeItemData item)
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