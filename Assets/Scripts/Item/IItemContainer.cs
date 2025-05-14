using System.Collections.Generic;

public interface IItemContainer
{
    IReadOnlyList<TradeItemData> Items { get; }
    void Add(TradeItemData item);
    void Remove(TradeItemData item);
}