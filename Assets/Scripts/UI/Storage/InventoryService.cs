using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class InventoryService
{
    /**********아이템 데이터***********/
    public Storage Storage { get; private set; }
    public Backpack Backpack { get; private set; }

    private InventoryDatabase _backpackDatabase;
    
    private int _backpackCapacity;

    /**********페이징 상태***********/
    public int PageSize { get; }
    public int CurrentPage { get; private set; } = 1;

    /**********저장 경로***********/
    private readonly string _filePath = Path.Combine(Application.persistentDataPath, "Storage.json");

    public InventoryService(int pageSize, int backpackCapacity)
    {
        PageSize = pageSize;
        _backpackCapacity = backpackCapacity;
    }

    public void Save()
    {
        foreach (var item in Backpack.Items)
        {
            _backpackDatabase.AddItem(item);
        }
    }

    public void Load(List<NFTItem> items)
    {
        Storage = new Storage();

        foreach (var item in items)
        {
            var tradeItem = new TradeItemData()
            {
                TokenId = item.token_id,
                Data = ItemDataManager.Instance.GetItemDataById(item.item_id),
                ItemPrice = float.Parse(item.price_klay),
                SellerWalletAddress = item.seller,
                LeftSeconds = item.remaining_time,
            };

            Storage.Add(tradeItem);
        }

        Backpack = new Backpack(_backpackCapacity);
        _backpackDatabase = Resources.Load<InventoryDatabase>("Items/UserBackPack");

        if (_backpackDatabase == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Backpack database not found");
#endif
            return;
        }

        foreach (var item in _backpackDatabase.Items)
        {
            Storage.Add(item);
        }

        _backpackDatabase.RemoveAllItems();
    }

    public IEnumerable<TradeItemData> GetFilteredSorted(ItemType filter, SortType sortType, bool ascending)
    {
        if (Storage.Items == null)
        {
            return Enumerable.Empty<TradeItemData>();
        }

        var query = Storage.Items.AsEnumerable();

        if (filter != ItemType.None)
        {
            query = query.Where(item => item.Data.ItemType == filter);
        }

        switch (sortType)
        {
            case SortType.Name:
                query = ascending
                    ? query.OrderBy(item => item.Data.ItemName)
                    : query.OrderByDescending(item => item.Data.ItemName);
                break;
            case SortType.Value:
                query = ascending
                    ? query.OrderBy(item => item.ItemPrice)
                    : query.OrderByDescending(item => item.ItemPrice);
                break;
            default:
                break;
        }

        return query;
    }

    /*********************Page*******************/
    public int TotalPages
    {
        get
        {
            int count = GetFilteredSorted(ItemType.None, SortType.None, true).Count();
            int pages = (int)Math.Ceiling(count / (float)PageSize);
            return pages == 0 ? 1 : pages;
        }
    }

    public IEnumerable<TradeItemData> GetCurrentPageItems(ItemType filter, SortType sortType, bool ascending)
    {
        return GetFilteredSorted(filter, sortType, ascending)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);
    }

    public bool HasNextPage(ItemType filter, SortType sortType, bool ascending)
    {
        return CurrentPage < TotalPages - 1;
    }

    public bool HasPreviousPage()
    {
        return CurrentPage > 0;
    }

    public void NextPage(ItemType filter, SortType sortType, bool ascending)
    {
        if (HasNextPage(filter, sortType, ascending))
        {
            CurrentPage++;
        }
    }

    public void PreviousPage()
    {
        if (HasPreviousPage())
        {
            CurrentPage--;
        }
    }

    public void MoveToBackPack(TradeItemData item)
    {
        if (Backpack.IsFull)
        {
            return;
        }

        Storage.Remove(item);
        Backpack.Add(item);
    }

    public void MoveToStorage(TradeItemData item)
    {
        Backpack.Remove(item);
        Storage.Add(item);
    }

    public void AddItem(TradeItemData item)
    {
        if (item == null) return;

        Storage.Add(item);
    }

    public void DeleteItem(TradeItemData item)
    {
        Storage.Remove(item);
    }
}