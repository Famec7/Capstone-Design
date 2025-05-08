using System;
using System.Collections.Generic;

public class Backpack : IItemContainer
{
    private List<ItemData> _items = new List<ItemData>();
    public int Capacity { get; }
    public IReadOnlyList<ItemData> Items => _items;

    public Backpack(int capacity) { Capacity = capacity; }

    public bool IsFull => _items.Count >= Capacity;
    
    public void Add(ItemData item)
    {
        if (IsFull) return;
        if (item == null) throw new ArgumentNullException(nameof(item));
        if (_items.Contains(item)) throw new InvalidOperationException("Item already in backpack");
        
        
        _items.Add(item);
    }
    public void Remove(ItemData item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        if (!_items.Contains(item)) throw new InvalidOperationException("Item not found in backpack");
        
        _items.Remove(item);
    }
}