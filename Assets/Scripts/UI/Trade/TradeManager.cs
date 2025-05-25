using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.XR.Interaction.Toolkit.XRInteractionUpdateOrder;

[Serializable]
public class TradeItemData
{
    public int TokenId;
    public ItemData Data;
    public float ItemPrice;
    public string SellerWalletAddress;
    public int LeftSeconds;
}

[Serializable]
public class TradeItemDataList
{
    public List<TradeItemData> items = new List<TradeItemData>();
}

public class TradeManager : Singleton<TradeManager>
{
    public List<TradeItemData> Datas;
    public int PreviewCounts = 4;
    public ItemPreview[] ItemPreviews;
    public int CurrentPage = 1;
    public Image[] SortButtonImages;
    public DetailedDescriptionSection DetailedDescriptionSection;
    public Button[] CategoryButtons;

    [SerializeField]
    private PageComponent _pageComponent;

    private List<TradeItemData> _currentDisplayItems;
    private ItemType? _currentFilter = null; // null means ALL is selected
    
    [Header("중계 서버")]
    [SerializeField]
    private FetchNFTData fetchNFTData;

    /// <summary>
    /// Initialize TradeManager data, assign button listeners, and display all items.
    /// </summary>
    protected override void Init()
    {
        for (int i = 0; i < CategoryButtons.Length - 1; i++)
        {
            int index = i;
            CategoryButtons[i].onClick.AddListener(() => { FilterByItemType((ItemType)index); });
        }
        CategoryButtons[CategoryButtons.Length - 1].onClick.AddListener(() => { ShowAllItems(); });
    }

    private void Start()
    {
        fetchNFTData.OnNFTDataLoaded = (items) =>
        {
            TradeItemDataList trades = new TradeItemDataList();
            foreach (var item in items)
            {
                var itemData = ItemDataManager.Instance.GetItemDataById(item.item_id);
                TradeItemData tradeItemData = new TradeItemData
                {
                    TokenId = item.token_id,
                    Data = itemData,
                    ItemPrice = float.Parse(item.price_klay),
                    SellerWalletAddress = item.seller,
                    LeftSeconds = item.remaining_time,
                };

                trades.items.Add(tradeItemData);
            }

            Datas = trades.items;
            UpdateUI();
        };
    }

    public void UpdateUI()
    {
        ShowAllItems();
        UpdateLatestValidPreviews();
    }

    /// <summary>
    /// Sorts current display items in latest order and shows the first page.
    /// </summary>
    public void UpdateLatestValidPreviews()
    {
        int totalPages = _currentDisplayItems.Count / PreviewCounts + (_currentDisplayItems.Count % PreviewCounts != 0 ? 1 : 0);
        _pageComponent.TotalPageCount = totalPages;
        UpdatePage(0);
    }

    /// <summary>
    /// Sorts current display items in ascending price order and shows the first page.
    /// </summary>
    public void UpdateAscendingPreviews()
    {
        _currentDisplayItems.Sort((a, b) => a.ItemPrice.CompareTo(b.ItemPrice));
        int totalPages = _currentDisplayItems.Count / PreviewCounts + (_currentDisplayItems.Count % PreviewCounts != 0 ? 1 : 0);
        _pageComponent.TotalPageCount = totalPages;
        UpdatePage(0);
    }

    /// <summary>
    /// Sorts current display items in descending price order and shows the first page.
    /// </summary>
    public void UpdateDescendingPreviews()
    {
        _currentDisplayItems.Sort((a, b) => b.ItemPrice.CompareTo(a.ItemPrice));
        int totalPages = _currentDisplayItems.Count / PreviewCounts + (_currentDisplayItems.Count % PreviewCounts != 0 ? 1 : 0);
        _pageComponent.TotalPageCount = totalPages;
        UpdatePage(0);
    }

    /// <summary>
    /// Updates the page display based on current display items.
    /// </summary>
    public void UpdatePage(int pageIndex)
    {
        int startIndex = pageIndex * PreviewCounts;
        if (startIndex >= _currentDisplayItems.Count)
        {   
            Debug.LogWarning("해당 페이지에 표시할 데이터가 없습니다.");
            return;
        }
        
        for (int i = 0; i < ItemPreviews.Length; i++)
        {
            int dataIndex = startIndex + i;
            if (dataIndex < _currentDisplayItems.Count)
            {
                ItemPreviews[i].gameObject.SetActive(true);
                ItemPreviews[i].SetValid(_currentDisplayItems[dataIndex]);
            }
            else
            {
                ItemPreviews[i].gameObject.SetActive(false);
            }
        }
        int totalPages = _currentDisplayItems.Count / PreviewCounts + (_currentDisplayItems.Count % PreviewCounts != 0 ? 1 : 0);
        _pageComponent.TotalPageCount = totalPages;
        _pageComponent.UpdatePaginationButtons(pageIndex + 1);
        CurrentPage = pageIndex;
        //UpdateTimeForCurrentPage();
    }

    /// <summary>
    /// Moves to the next page.
    /// </summary>
    public void NextPage()
    {
        if ((CurrentPage + 1) * PreviewCounts < _currentDisplayItems.Count)
        {
            UpdatePage(CurrentPage + 1);
        }
        else
        {
            Debug.Log("마지막 페이지입니다.");
        }
    }

    /// <summary>
    /// Moves to the previous page.
    /// </summary>
    public void PrevPage()
    {
        if (CurrentPage > 0)
        {
            UpdatePage(CurrentPage - 1);
        }
        else
        {
            Debug.Log("첫 페이지입니다.");
        }
    }

    /// <summary>
    /// Sort button click handler that applies sorting while preserving current filter.
    /// </summary>
    public void OnClickSortButtons(int index)
    {
        switch (index)
        {
            case 0:
                UpdateLatestValidPreviews();
                break;
            case 1:
                UpdateAscendingPreviews();
                break;
            case 2:
                UpdateDescendingPreviews();
                break;
        }
        CurrentPage = 0;
        for (int i = 0; i < 3; i++)
        {
            if (index == i)
                SortButtonImages[i].color = new Color32(0, 0, 0, 200);
            else
                SortButtonImages[i].color = new Color32(0, 0, 0, 50);
        }
    }

    /// <summary>
    /// Applies filter based on a given ItemType and updates the category button appearance.
    /// </summary>
    public void FilterByItemType(ItemType filterType)
    {
        List<TradeItemData> filteredItems = Datas.Where(item => item.Data.ItemType == filterType).ToList();
        _currentDisplayItems = filteredItems;
        _currentFilter = filterType;
        int totalPages = filteredItems.Count / PreviewCounts + (filteredItems.Count % PreviewCounts != 0 ? 1 : 0);
        _pageComponent.TotalPageCount = totalPages;
        UpdatePage(0);
        for (int i = 0; i < CategoryButtons.Length - 1; i++)
        {
            if ((ItemType)i == filterType)
                CategoryButtons[i].image.color = new Color32(0, 0, 0, 200);
            else
                CategoryButtons[i].image.color = new Color32(0, 0, 0, 50);
        }
        CategoryButtons[CategoryButtons.Length - 1].image.color = new Color32(0, 0, 0, 50);
    }

    /// <summary>
    /// Shows all items and updates the category button appearance to reflect the ALL selection.
    /// </summary>
    public void ShowAllItems()
    {
        _currentDisplayItems = new List<TradeItemData>(Datas);
        _currentFilter = null;
        int totalPages = Datas.Count / PreviewCounts + (Datas.Count % PreviewCounts != 0 ? 1 : 0);
        _pageComponent.TotalPageCount = totalPages;
        UpdatePage(0);
        for (int i = 0; i < CategoryButtons.Length - 1; i++)
        {
            CategoryButtons[i].image.color = new Color32(0, 0, 0, 50);
        }
        CategoryButtons[CategoryButtons.Length - 1].image.color = new Color32(0, 0, 0, 200);
    }

    private void OnEnable()
    {
        fetchNFTData.LoadNFTData();
    }

    public void Load()
    {
        fetchNFTData.LoadNFTData();
    }
}
