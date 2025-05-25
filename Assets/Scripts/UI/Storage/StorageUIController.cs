using System;
using System.Linq;
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

    # region UI View

    [Header("UI View")] [SerializeField] private StorageSlotView slotPrefab;

    [SerializeField] private StorageDetailView detailView;

    [SerializeField] private CategoryTabView[] categoryTabs;

    # endregion

    # region UI 요소

    [Header("UI 요소")] [SerializeField] private TMP_Dropdown sortDropdown;

    [SerializeField] private Button sortOrderButton;

    [SerializeField] private Toggle deleteToggle;

    [SerializeField] private Button addButton;

    [SerializeField] private Button prevButton;

    [SerializeField] private Button nextButton;

    [SerializeField] private TextMeshProUGUI pageText;

    [SerializeField] private Button sellButton;

    [SerializeField] private InputField sellPriceInputField;

    # endregion

    # region Slot 부모

    [Header("슬롯 부모")] [SerializeField] private Transform slotParent;

    [SerializeField] private Transform backpackSlotParent;

    # endregion

    # region 팝업 UI

    [Header("판매 완료 팝업")] [SerializeField] private AutoHidePopup sellCompletePopup;

    [Header("가격 오류 팝업")] [SerializeField] private AutoHidePopup priceErrorPopup;

    # endregion

    [Header("Page 하나 당 아이템 수")] [SerializeField]
    private int pageSize = 20;

    [Header("중계 서버")] [SerializeField] private FetchNFTData fetchNFTData;

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
        fetchNFTData.FetchUserNFTData(items =>
        {
            _service.Load(items);
            Refresh();
        });
    }

    /// <summary>
    /// UI 요소와 이벤트를 바인딩합니다.
    /// </summary>
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
        });


        /*if (addButton)
        {
            addButton.onClick.AddListener(() =>
            {
                if (detailView.gameObject.activeSelf)
                {
                    _service.MoveToBackPack(detailView.CurrentItemData);

                    var slot = Instantiate(slotPrefab, backpackSlotParent);
                    slot.Bind(detailView.CurrentItemData, itemData =>
                    {
                        OnBackpackSlotClicked(itemData);
                        Destroy(slot.gameObject);
                    });
                    
                    Refresh();
                }
            });
        }*/

        if (sellButton)
        {
            sellButton.onClick.AddListener(() =>
            {
                TradeItemData itemData = detailView.CurrentItemData;
                
                if (itemData != null)
                {
                    if (float.TryParse(sellPriceInputField.text, out float price))
                    {
                        itemData.ItemPrice = float.Parse(sellPriceInputField.text);
                        NFTManager.Instance.ListNFT(itemData.TokenId, price);
                        _service.DeleteItem(itemData);
                        Refresh();
                        sellCompletePopup.ShowPopup();
                    }
                    else
                    {
                        priceErrorPopup.ShowPopup();
                    }
                }
            });
        }

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

    /// <summary>
    /// UI를 갱신합니다.
    /// </summary>
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

    private void CreateSlot(TradeItemData itemData)
    {
        var slot = Instantiate(slotPrefab, slotParent);
        slot.Bind(itemData, OnSlotClicked);

        slot.name = itemData.Data.name;
    }

    private void OnSlotClicked(TradeItemData itemData)
    {
        if (_isDeleteMode)
        {
            _service.DeleteItem(itemData);
            Refresh();
        }
        else
        {
            detailView.Show(itemData);
        }
    }

    private void OnBackpackSlotClicked(TradeItemData itemData)
    {
        _service.MoveToStorage(itemData);
        Refresh();
    }

    private void OnDisable()
    {
        _service.Save();
    }

    private void OnEnable()
    {
        fetchNFTData.FetchUserNFTData(items =>
        {
            _service.Load(items);
            Refresh();
        });
    }
}