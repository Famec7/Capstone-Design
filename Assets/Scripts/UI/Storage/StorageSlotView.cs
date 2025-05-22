using System;
using UnityEngine;
using UnityEngine.UI;

public class StorageSlotView : MonoBehaviour
{
    [Header("슬롯 아이템 아이콘")]
    [SerializeField]
    private Image itemImage;
    
    private Button _slotButton;
    
    private TradeItemData _currentItemData;
    public TradeItemData GetItemData() => _currentItemData;

    private void Awake()
    {
        if (!TryGetComponent(out _slotButton))
        {
            Debug.LogError("StorageSlot: Button 컴포넌트를 찾을 수 없습니다.");
        }
        
        if (itemImage == null)
        {
            Debug.LogError("StorageSlot: itemImage가 할당되지 않았습니다.");
        }
    }

    public void Bind(TradeItemData itemData, Action<TradeItemData> callback)
    {
        _currentItemData = itemData;
        
        _slotButton.onClick.RemoveAllListeners();
        _slotButton.onClick.AddListener(() =>
        {
            callback?.Invoke(itemData);
        });
        
        itemImage.sprite = itemData.Data.ItemIcon;
    }
}