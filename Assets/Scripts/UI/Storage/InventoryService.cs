using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class InventoryService
{
    /**********아이템 데이터***********/
    public Storage Storage { get; private set; }
    public Backpack Backpack { get; private set; }

    private InventoryDatabase _backpackDatabase;

    /**********페이징 상태***********/
    public int PageSize { get; }
    public int CurrentPage { get; private set; } = 1;

    /**********저장 경로***********/
    private readonly string _filePath = Path.Combine(Application.persistentDataPath, "Storage.json");
    
    public InventoryService(int pageSize, int backpackCapacity)
    {
        PageSize = pageSize;
        Load(backpackCapacity);
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(Storage.Items, true);
        File.WriteAllText(_filePath, json);

        foreach (var item in Backpack.Items)
        {
            _backpackDatabase.AddItem(item);
        }
    }

    public void Load(int capacity)
    {
        Storage = new Storage();

        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            var items = JsonUtility.FromJson<ItemData[]>(json);

            foreach (var item in items)
            {
                Storage.Add(item);
            }
        }

        Backpack = new Backpack(capacity);
        _backpackDatabase = Resources.Load<InventoryDatabase>("Items");

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

    public IEnumerable<ItemData> GetFilteredSorted(ItemType filter, SortType sortType, bool ascending)
    {
        if (Storage.Items == null)
        {
            return Enumerable.Empty<ItemData>();
        }
        
        var query = Storage.Items.AsEnumerable();

        if (filter != ItemType.None)
        {
            query = query.Where(item => item.ItemType == filter);
        }

        switch (sortType)
        {
            case SortType.Name:
                query = ascending
                    ? query.OrderBy(item => item.ItemName)
                    : query.OrderByDescending(item => item.ItemName);
                break;
            case SortType.Value:
                query = ascending
                    ? query.OrderBy(item => item.ItemValue)
                    : query.OrderByDescending(item => item.ItemValue);
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

    public IEnumerable<ItemData> GetCurrentPageItems(ItemType filter, SortType sortType, bool ascending)
    {
        return GetFilteredSorted(filter, sortType, ascending)
            .Skip(CurrentPage * PageSize)
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

    public void MoveToBackPack(ItemData item)
    {
        if (Backpack.IsFull)
        {
            return;
        }

        Storage.Remove(item);
        Backpack.Add(item);
    }

    public void MoveToStorage(ItemData item)
    {
        Backpack.Remove(item);
        Storage.Add(item);
    }

    public void DeleteItem(ItemData item)
    {
        Storage.Remove(item);
    }
}