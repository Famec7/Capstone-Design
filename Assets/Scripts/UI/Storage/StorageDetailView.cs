using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageDetailView : MonoBehaviour
{
    [Header("아이템 아이콘")]
    [SerializeField]
    private Image itemIcon;
    
    [Header("아이템 이름")]
    [SerializeField]
    private TextMeshProUGUI itemNameText;
    
    [Header("아이템 분류")]
    [SerializeField]
    private TextMeshProUGUI itemTypeText;
    
    [Header("아이템 설명")]
    [SerializeField]
    private TextMeshProUGUI itemDescriptionText;
    
    [Header("아이템 가치")]
    [SerializeField]
    private TextMeshProUGUI itemValueText;

    public ItemData CurrentItemData { get; private set; }

    private void Awake()
    {
        if (itemNameText == null)
        {
            Debug.LogError("StorageDetailView: itemNameText가 할당되지 않았습니다.");
        }
        
        if (itemDescriptionText == null)
        {
            Debug.LogError("StorageDetailView: itemDescriptionText가 할당되지 않았습니다.");
        }
        
        if (itemValueText == null)
        {
            Debug.LogError("StorageDetailView: itemValueText가 할당되지 않았습니다.");
        }
        
        if (itemIcon == null)
        {
            Debug.LogError("StorageDetailView: itemIcon이 할당되지 않았습니다.");
        }
        
        if (itemTypeText == null)
        {
            Debug.LogError("StorageDetailView: itemTypeText가 할당되지 않았습니다.");
        }
    }

    public void Show(ItemData itemData)
    {
        CurrentItemData = itemData;
        
        itemIcon.sprite = itemData.ItemIcon;
        itemNameText.text = $"<b>이름</b>                   {itemData.ItemName}";
        itemTypeText.text = $"<b>분류</b>                   {itemData.ItemType}";
        itemValueText.text = $"<b>가치</b>                   {itemData.ItemValue}G";
        itemDescriptionText.text = $"<b>설명</b>\n{itemData.ItemDescription}";
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        CurrentItemData = null;
        gameObject.SetActive(false);
    }
}