using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum SortType
{
    Recent,
    Name,
    Value,
    None
}

public class StorageUIController : MonoBehaviour
{
    private InventoryService _service = null;

    [Header("UI View")]
    [SerializeField]
    private StorageSlotView slotPrefab;
    
    [SerializeField]
    private StorageDetailView detailView;
    
    [SerializeField]
    private CategoryTabView[] categoryTabs;
    
    [Header("UI 요소")]
    [SerializeField]
    private TMP_Dropdown sortDropdown;
    
    [SerializeField]
    private Button sortOrderButton;
    
    [SerializeField]
    private Toggle deleteToggle;
    
    [SerializeField]
    private Button addButton;
    
    [SerializeField]
    private Button prevButton;
    
    [SerializeField]
    private Button nextButton;
    
    [SerializeField]
    private TextMeshProUGUI pageText;
    
    [Header("슬롯 부모")]
    [SerializeField]
    private Transform slotParent;
    
    [Header("Page 하나 당 아이템 수")]
    [SerializeField]
    private int pageSize = 20;
    
    private bool _isDeleteMode = false;
    private bool _isSortAscending = true;
    private ItemType _currentItemType = ItemType.None;
    private SortType _currentSortType = SortType.Recent;

    private void Awake()
    {
        if (_service == null)
        {
            _service = new InventoryService(pageSize, 5);
        }
    }

    private void Start()
    {
        Bind();
        Refresh();
    }

    private void Bind()
    {
        foreach (var tab in categoryTabs)
        {
            tab.Bind(itemType =>
            {
                _currentItemType = itemType;
                Refresh();
            });
        }
        
        sortDropdown.onValueChanged.AddListener(value =>
        {
            _currentSortType = (SortType)value;
            Refresh();
        });
        
        sortOrderButton.onClick.AddListener(() =>
        {
            _isSortAscending = !_isSortAscending;
            Refresh();
        });
        
        deleteToggle.onValueChanged.AddListener(isOn =>
        {
            _isDeleteMode = isOn;
            deleteToggle.image.color = _isDeleteMode ? Color.red : Color.white;
            
            Refresh();
        });
        
        addButton.onClick.AddListener(() =>
        {
            if (detailView.gameObject.activeSelf)
            {
                _service.MoveToBackPack(detailView.CurrentItemData);
            }
            Refresh();
        });
        
        prevButton.onClick.AddListener(() =>
        {
            _service.PreviousPage();
            Refresh();
        });
        
        nextButton.onClick.AddListener(() =>
        {
            _service.NextPage(_currentItemType, _currentSortType, _isSortAscending);
            Refresh();
        });
    }

    private void Refresh()
    {
        ClearSlots();
        
        var items = _service.GetCurrentPageItems(_currentItemType, _currentSortType, _isSortAscending);
        
        foreach (var item in items)
        {
            CreateSlot(item);
        }
        
        pageText.text = $"{_service.CurrentPage} / {_service.TotalPages}";
        prevButton.gameObject.SetActive(_service.HasPreviousPage());
        nextButton.gameObject.SetActive(_service.HasNextPage(_currentItemType, _currentSortType, _isSortAscending));
        
        detailView.Hide();
    }

    private void ClearSlots()
    {
        foreach (Transform slot in slotParent)
        {
            Destroy(slot.gameObject);
        }
    }
    
    private void CreateSlot(ItemData itemData)
    {
        var slot = Instantiate(slotPrefab, slotParent);
        slot.Bind(itemData, OnSlotClicked);
            
        slot.name = itemData.name;
    }

    private void OnSlotClicked(ItemData itemData)
    {
        if (_isDeleteMode)
        {
            Refresh();
        }
        else
        {
            detailView.Show(itemData);
        }
    }
}