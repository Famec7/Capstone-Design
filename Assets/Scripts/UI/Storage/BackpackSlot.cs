using System;
using UnityEngine;
using UnityEngine.UI;

public class BackpackSlot : MonoBehaviour
{
    [Header("슬롯 아이템 아이콘")]
    [SerializeField]
    private Image itemImage;
    
    private Button _slotButton;

    private ItemData _currentItemData;
    public ItemData GetItemData() => _currentItemData;

    private void Awake()
    {
        if (TryGetComponent(out _slotButton) == false)
        {
            Debug.LogError("BackpackSlot: Button 컴포넌트를 찾을 수 없습니다.");
        }
    }
    
    public void Bind(ItemData itemData, Action<ItemData> callback)
    {
        _currentItemData = itemData;
        
        _slotButton.onClick.RemoveAllListeners();
        _slotButton.onClick.AddListener(() =>
        {
            callback?.Invoke(itemData);
        });

        _slotButton.image.sprite = itemData.ItemIcon;
    }
}