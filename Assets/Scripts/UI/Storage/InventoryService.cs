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
    
    /**********페이징 상태***********/
    public int PageSize { get; }
    public int CurrentPage { get; private set; } = 1;
    
    /**********저장 경로***********/
    private readonly string _filePath = Path.Combine(Application.dataPath, "Storage.json");
    
    public void Save()
    {
        var json = JsonUtility.ToJson(Storage.Items, true);
        File.WriteAllText(_filePath, json);
        
        // Todo: Save Backpack data
    }

    public void Load(int capacity)
    {
        if (!File.Exists(_filePath))
        {
            Debug.LogError($"File not found: {_filePath}");
            return;
        }
        
        var json = File.ReadAllText(_filePath);
        var items = JsonUtility.FromJson<ItemData[]>(json);
        
        Storage = new Storage();
        foreach (var item in items)
        {
            Storage.Add(item);
        }
        
        // Todo: Load Backpack data
    }
    
    public IEnumerable<ItemData> GetFilteredSorted(ItemType filter, SortType sortType, bool ascending)
    {
        var query = Storage.Items.AsEnumerable();
        
        if (filter != ItemType.None)
        {
            query = query.Where(item => item.ItemType == filter);
        }
        
        switch (sortType)
        {
            case SortType.Name:
                query = ascending ? query.OrderBy(item => item.ItemName) : query.OrderByDescending(item => item.ItemName);
                break;
            case SortType.Value:
                query = ascending ? query.OrderBy(item => item.ItemValue) : query.OrderByDescending(item => item.ItemValue);
                break;
            default:
                break;
        }

        return query;
    }
    
    /*********************Page*******************/
    public int TotalPages => (int)Math.Ceiling(GetFilteredSorted(ItemType.None, SortType.None, true).Count() / (float)PageSize);

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