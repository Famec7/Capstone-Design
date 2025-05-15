using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public int InventoryCnt = 10;
    public int SelectedSlotIndex = 0;
    [SerializeField] private InventorySlot[] _slots;

    public InventorySlot[] Slots => _slots; // 읽기 전용 프로퍼티

    private InventoryDatabase _backpackDatabase;

    protected override void Init()
    {
        //InventoryCnt = ??;
        _slots = GetComponentsInChildren<InventorySlot>();

        _backpackDatabase = Resources.Load<InventoryDatabase>("Items/UserBackPack");
    }

    public void AddItem(BaseItem item)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].IsEmpty)
            {
                _slots[i].SetItem(item);
                break;
            }
        }
    }

    public BaseItem RemoveItem(int index)
    {
        if (index < 0 || index >= _slots.Length || _slots[index].IsEmpty)
            return null;

        // 1) 제거할 아이템 Detach
        BaseItem removedItem = _slots[index].GetComponentInChildren<BaseItem>();
        removedItem.transform.SetParent(null);

        // 2) 나머지 슬롯에 남은 아이템들 순서대로 수집
        List<BaseItem> remaining = new List<BaseItem>();
        foreach (var slot in _slots)
        {
            var child = slot.GetComponentInChildren<BaseItem>();
            if (child != null)
                remaining.Add(child);
        }

        // 3) 모든 슬롯 초기화
        foreach (var slot in _slots)
        {
            slot.Data = null;
            slot.GetComponent<Image>().color = Color.black;
            slot.BorderImage.color = Color.black;
        }

        // 4) 남은 아이템을 0번 슬롯부터 재배치
        for (int i = 0; i < remaining.Count; i++)
        {
            _slots[i].SetItem(remaining[i]);
            _slots[i].SetItemTransform(remaining[i]);
            _slots[i].SetSlotColor(remaining[i]);
        }


        return removedItem;
    }


    public bool HasEmptySlot()
    {
        int cnt = 0;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].IsEmpty)
                cnt++;
        }

        if (cnt == 0)
            StartCoroutine(FlashRedBorder());

        return cnt != 0;
    }

    IEnumerator FlashRedBorder()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].BorderImage.color = Color.red;
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < _slots.Length; i++)
        {
            BaseItem item = _slots[i].GetComponentInChildren<BaseItem>();
            Slots[i].SetBorderColor(item);
        }
    }

    [ContextMenu("Save Inventory")]
    public void SaveInventory()
    {
        if (_backpackDatabase == null)
        {
            Debug.LogError("Backpack database not found");
            return;
        }

        foreach (var slot in _slots)
        {
            if (slot.IsEmpty)
                continue;

            TradeItemData itemData = null;

            NFTManager.Instance.MintNFT(slot.Data.ItemId, (item) =>
            {
                itemData = new TradeItemData
                {
                    TokenId = item.token_id,
                    Data = slot.Data,
                    ItemPrice = float.Parse(item.price_klay),
                    SellerWalletAddress = item.seller,
                    LeftSeconds = item.remaining_time,
                };
            });

            _backpackDatabase.AddItem(itemData);
        }
    }

    [ContextMenu("Load Inventory")]
    public void LoadInventory()
    {
        foreach (var item in _backpackDatabase.Items)
        {
            // Todo: 아이템을 슬롯에 추가하는 로직
        }
    }
}