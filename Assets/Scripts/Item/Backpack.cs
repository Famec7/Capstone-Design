using System;
using System.Collections.Generic;

[Serializable]
public class Backpack : IItemContainer
{
    private List<TradeItemData> _items = new List<TradeItemData>();
    public int Capacity { get; }
    public IReadOnlyList<TradeItemData> Items => _items;

    public Backpack(int capacity) { Capacity = capacity; }

    public bool IsFull => _items.Count >= Capacity;
    
    public void Add(TradeItemData item)
    {
        if (IsFull) return;
        if (item == null) throw new ArgumentNullException(nameof(item));
        
        
        _items.Add(item);
    }
    public void Remove(TradeItemData item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        if (!_items.Contains(item)) throw new InvalidOperationException("Item not found in backpack");
        
        _items.Remove(item);
    }
}