using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataManager : Singleton<ItemDataManager>
{

    private ItemData[] _itemDatas;

   protected override void Init()
   {

   }

    private void LoadAllItemData()
    {
        _itemDatas = Resources.LoadAll<ItemData>("ItemDatas");
#if UNITY_EDITOR
        Debug.Log("Loaded all item data: " + _itemDatas.Length);
#endif
    }

    public ItemData GetItemDataById(int id)
    {
        foreach (var data in _itemDatas)
        {
            if (data.ItemId == id) return data;
        }
#if UNITY_EDITOR
        Debug.LogWarning("ItemData not found for id: " + id);
#endif
        return null;
    }
}
