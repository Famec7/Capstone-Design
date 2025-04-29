using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.XR.Interaction.Toolkit.XRInteractionUpdateOrder;

[Serializable]
public class TradeItemData
{
    public string ItemName;
    public ItemType ItemType;
    public int ItemPrice;
    public string ListedAt;
    public bool IsExpired { get { DateTime listedDate = DateTime.Parse(ListedAt); return DateTime.Now > listedDate.AddHours(24); } }
    public string SellerWalletAddress;
    [NonSerialized]
    public DateTime ListedTime;
    public int LeftSeconds;
}

[Serializable]
public class TradeItemDataList
{
    public List<TradeItemData> items;
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

    /// <summary>
    /// Initialize TradeManager data, assign button listeners, and display all items.
    /// </summary>
    protected override void Init()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("TradeTest");
        if (jsonText == null)
        {
            Debug.LogError("Resources 폴더에서 TradeTest.json 파일을 찾을 수 없습니다!");
            return;
        }
        TradeItemDataList tradeItems = JsonUtility.FromJson<TradeItemDataList>(jsonText.text);
        Datas = tradeItems.items;
        foreach (var data in Datas)
        {
            if (DateTime.TryParse(data.ListedAt, out DateTime parsedTime))
            {
                data.ListedTime = parsedTime;
                Debug.Log($"{data.ItemName} - {data.ItemType}");
            }
        }
        _currentDisplayItems = new List<TradeItemData>(Datas);
        for (int i = 0; i < CategoryButtons.Length - 1; i++)
        {
            int index = i;
            CategoryButtons[i].onClick.AddListener(() => { FilterByItemType((ItemType)index); });
        }
        CategoryButtons[CategoryButtons.Length - 1].onClick.AddListener(() => { ShowAllItems(); });
        ShowAllItems();
        UpdateLatestValidPreviews();
        //StartCoroutine(UpdateTimeDisplay());
    }

    /// <summary>
    /// Coroutine that updates the remaining time display for each preview.
    /// </summary>
    //private IEnumerator UpdateTimeDisplay()
    //{
    //    while (true)
    //    {
    //        DateTime now = DateTime.Now;
    //        for (int i = 0; i < ItemPreviews.Length; i++)
    //        {
    //            if (!ItemPreviews[i].gameObject.activeSelf)
    //                continue;
    //            TimeSpan remainingTime = ItemPreviews[i].Data.ListedTime.AddHours(24) - now;
    //            if (remainingTime.TotalSeconds <= 0)
    //            {
    //                ItemPreviews[i].SetExpired();
    //                ItemPreviews[i].TimeText.text = "00:00:00";
    //            }
    //            else
    //            {
    //                ItemPreviews[i].TimeText.text = remainingTime.ToString(@"hh\:mm\:ss");
    //            }
    //        }
    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    /// <summary>
    /// Updates the remaining time display for the current page.
    /// </summary>
    //private void UpdateTimeForCurrentPage()
    //{
    //    DateTime now = DateTime.Now;
    //    for (int i = 0; i < ItemPreviews.Length; i++)
    //    {
    //        if (!ItemPreviews[i].gameObject.activeSelf)
    //            continue;
    //        TimeSpan remainingTime = ItemPreviews[i].Data.ListedTime.AddHours(24) - now;
    //        if (remainingTime.TotalSeconds <= 0)
    //        {
    //            if (!ItemPreviews[i].Data.IsExpired)
    //            {
    //                ItemPreviews[i].SetExpired();
    //            }
    //            ItemPreviews[i].TimeText.text = "00:00:00";
    //        }
    //        else
    //        {
    //            ItemPreviews[i].TimeText.text = remainingTime.ToString(@"hh\:mm\:ss");
    //        }
    //    }
    //}

    /// <summary>
    /// Sorts current display items in latest order and shows the first page.
    /// </summary>
    public void UpdateLatestValidPreviews()
    {
        RemoveExpiredData();
        _currentDisplayItems.Sort((a, b) =>
        {
            TimeSpan remainingA = a.ListedTime.AddHours(24) - DateTime.Now;
            TimeSpan remainingB = b.ListedTime.AddHours(24) - DateTime.Now;
            return remainingA.CompareTo(remainingB);
        });
        int totalPages = _currentDisplayItems.Count / PreviewCounts + (_currentDisplayItems.Count % PreviewCounts != 0 ? 1 : 0);
        _pageComponent.TotalPageCount = totalPages;
        UpdatePage(0);
    }

    /// <summary>
    /// Sorts current display items in ascending price order and shows the first page.
    /// </summary>
    public void UpdateAscendingPreviews()
    {
        RemoveExpiredData();
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
        RemoveExpiredData();
        _currentDisplayItems.Sort((a, b) => b.ItemPrice.CompareTo(a.ItemPrice));
        int totalPages = _currentDisplayItems.Count / PreviewCounts + (_currentDisplayItems.Count % PreviewCounts != 0 ? 1 : 0);
        _pageComponent.TotalPageCount = totalPages;
        UpdatePage(0);
    }

    /// <summary>
    /// Removes expired data from Datas.
    /// </summary>
    public void RemoveExpiredData()
    {
        DateTime now = DateTime.Now;
        Datas.RemoveAll(data => (now - data.ListedTime).TotalHours >= 24);
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
        RemoveExpiredData();
        List<TradeItemData> filteredItems = Datas.Where(item => item.ItemType == filterType).ToList();
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
        RemoveExpiredData();
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
}
