using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Storage : IItemContainer
{
    private List<ItemData> _items = new List<ItemData>();
    public IReadOnlyList<ItemData> Items => _items;
    
    public void Add(ItemData item)
    {
        if (item == null) return;

        _items.Add(item);
    }

    public void Remove(ItemData item)
    {
        if (item == null) return;
        if (!_items.Contains(item)) return;

        _items.Remove(item);
    }
}