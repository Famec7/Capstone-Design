using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Storage : IItemContainer
{
    private List<TradeItemData> _items = new List<TradeItemData>();
    public IReadOnlyList<TradeItemData> Items => _items;
    
    public void Add(TradeItemData item)
    {
        if (item == null) return;

        _items.Add(item);
    }

    public void Remove(TradeItemData item)
    {
        if (item == null) return;
        if (!_items.Contains(item)) return;

        _items.Remove(item);
    }
}