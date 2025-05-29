using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : Singleton<ItemFactory>
{
    public List<GameObject> ItemList;
    private Queue<BaseItem> _pool;

    override protected void Init()
    {
        _pool = new Queue<BaseItem>();
    }

    // ItemName으로 아이템 생성/반환하는 함수
    public BaseItem CreateItem(string itemName)
    {
        // 1. 풀에서 해당 ItemName을 가진 아이템이 있는지 확인
        int poolCount = _pool.Count;
        for (int i = 0; i < poolCount; i++)
        {
            BaseItem item = _pool.Dequeue();
            if (item != null && item.Data != null && item.Data.ItemName == itemName)
            {
                item.gameObject.SetActive(true);
                return item;
            }
            else
            {
                _pool.Enqueue(item);
            }
        }

        // 2. 풀에서 찾지 못했으므로, ItemList에서 해당 프리팹을 찾음
        foreach (GameObject prefab in ItemList)
        {
            BaseItem baseItem = prefab.GetComponent<BaseItem>();
            if (baseItem != null && baseItem.Data != null && baseItem.Data.ItemName == itemName)
            {
                // 프리팹 인스턴스화 후 BaseItem 컴포넌트를 반환
                GameObject newObj = Instantiate(prefab);
                return newObj.GetComponent<BaseItem>();
            }
        }

#if UNITY_EDITOR
        Debug.LogWarning("Item with name " + itemName + " not found in ItemList.");
#endif
        return null;
    }

    public void ReturnItem(BaseItem item)
    {
        if (item != null)
        {
            item.gameObject.SetActive(false);
            _pool.Enqueue(item);
        }
    }

    public BaseItem CreateItem(int index)
    {
        // 2. 풀에서 찾지 못했으므로, ItemList에서 해당 프리팹을 찾음
        GameObject item = Instantiate(ItemList[index]);
        return item.GetComponent<BaseItem>();
    }
}
