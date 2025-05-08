using System.Collections.Generic;

public interface IItemContainer
{
    IReadOnlyList<ItemData> Items { get; }
    void Add(ItemData item);
    void Remove(ItemData item);
}