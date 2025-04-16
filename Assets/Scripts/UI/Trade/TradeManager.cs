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
    public bool IsExpired
    {
        get
        {
            DateTime listedDate = DateTime.Parse(ListedAt);
            return DateTime.Now > listedDate.AddHours(24);
        }
    }
    public string SellerWalletAddress;

    [NonSerialized]
    public DateTime ListedTime;
}
[Serializable]
public class TradeItemDataList
{
    public List<TradeItemData> items;
}

public class TradeManager : Singleton<TradeManager>
{
    public List <TradeItemData> Datas;
    public int PreviewCounts = 4;
    public ItemPreview[] ItemPreviews;
    public int CurrentPage = 1;
    public Image[] SortButtonImages;
    [SerializeField]
    private PageComponent _pageComponent;
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
            }
        }

        UpdateLatestValidPreviews();
        StartCoroutine(UpdateTimeDisplay());
    }

    private IEnumerator UpdateTimeDisplay()
    {
        while (true)
        {
            DateTime now = DateTime.Now;
            for (int i = 0; i < ItemPreviews.Length; i++)
            {
                if (!ItemPreviews[i].gameObject.activeSelf)
                    continue;

                TimeSpan remainingTime = ItemPreviews[i].Data.ListedTime.AddHours(24) - now;

                if (remainingTime.TotalSeconds <= 0)
                {
                    
                    ItemPreviews[i].SetExpired();
                    ItemPreviews[i].TimeText.text = "00:00:00";
                }
                else
                {
                    ItemPreviews[i].TimeText.text = remainingTime.ToString(@"hh\:mm\:ss");
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateTimeForCurrentPage()
    {
        DateTime now = DateTime.Now;
        for (int i = 0; i < ItemPreviews.Length; i++)
        {
            if (!ItemPreviews[i].gameObject.activeSelf)
                continue;

            TimeSpan remainingTime = ItemPreviews[i].Data.ListedTime.AddHours(24) - now;
            if (remainingTime.TotalSeconds <= 0)
            {
                if (!ItemPreviews[i].Data.IsExpired)
                {
                    ItemPreviews[i].SetExpired();
                }
                ItemPreviews[i].TimeText.text = "00:00:00";
            }
            else
            {
                ItemPreviews[i].TimeText.text = remainingTime.ToString(@"hh\:mm\:ss");
            }
        }
    }
    public void UpdateLatestValidPreviews()
    {
        RemoveExpiredData();

        Datas.Sort((a, b) =>
        {
            TimeSpan remainingA = a.ListedTime.AddHours(24) - DateTime.Now;
            TimeSpan remainingB = b.ListedTime.AddHours(24) - DateTime.Now;
            return remainingA.CompareTo(remainingB);
        });

        int count = Mathf.Min(ItemPreviews.Length, Datas.Count);

        for (int i =0; i < count; i++)
        {
            ItemPreviews[i].SetValid(Datas[i]);
        }

        if (count != PreviewCounts) 
        {
            for (int i = count; i < PreviewCounts; i++)
            {
                ItemPreviews[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateAscendingPreviews()
    {
        RemoveExpiredData();

        Datas.Sort((a, b) => a.ItemPrice.CompareTo(b.ItemPrice));

        int count = Mathf.Min(ItemPreviews.Length, Datas.Count);

        for (int i = 0; i < count; i++)
        {
            ItemPreviews[i].SetValid(Datas[i]);
        }

        if (count != PreviewCounts)
        {
            for (int i = count; i < PreviewCounts; i++)
            {
                ItemPreviews[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateDescendingPreviews()
    {
       RemoveExpiredData();

        Datas.Sort((a, b) => b.ItemPrice.CompareTo(a.ItemPrice));

        int count = Mathf.Min(ItemPreviews.Length, Datas.Count);

        for (int i = 0; i < count; i++)
        {
            ItemPreviews[i].SetValid(Datas[i]);
        }

        if (count != PreviewCounts)
        {
            for (int i = count; i < PreviewCounts; i++)
            {
                ItemPreviews[i].gameObject.SetActive(false);
            }
        }
    }

    public void RemoveExpiredData()
    {
        DateTime now = DateTime.Now;
        Datas.RemoveAll(data =>
        {
            return (now - data.ListedTime).TotalHours >= 24;
        });
    }

    public void UpdatePage(int pageIndex)
    {
        int startIndex = pageIndex * PreviewCounts;
        if (startIndex >= Datas.Count)
        {
            Debug.LogWarning("해당 페이지에 표시할 데이터가 없습니다.");
            return;
        }

        for (int i = 0; i < ItemPreviews.Length; i++)
        {
            int dataIndex = startIndex + i;
            if (dataIndex < Datas.Count)
            {
                ItemPreviews[i].gameObject.SetActive(true);
                ItemPreviews[i].SetValid(Datas[dataIndex]);
            }
            else
            {
                ItemPreviews[i].gameObject.SetActive(false);
            }
        }
        // PageComponent 업데이트: 화면에는 1부터 시작하는 페이지 번호를 사용하므로 pageIndex+1 전달
        _pageComponent.UpdatePaginationButtons(pageIndex + 1);
        CurrentPage = pageIndex;
        UpdateTimeForCurrentPage();
    }

    /// <summary>
    /// 다음 페이지로 이동하는 함수
    /// </summary>
    public void NextPage()
    {
        // 다음 페이지의 시작 인덱스가 전체 데이터 개수보다 작은지 체크
        if ((CurrentPage + 1) * 4 < Datas.Count)
        {
            UpdatePage(CurrentPage + 1);
        }
        else
        {
            Debug.Log("마지막 페이지입니다.");
        }
    }

    /// <summary>
    /// 이전 페이지로 이동하는 함수
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

    public void OnClickSortButtons(int index)
    {
        switch(index)
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

        CurrentPage = 1;
        UpdatePage(1);

        for (int i = 0; i < 3; i++)
        {
            if (index == i)
                SortButtonImages[i].color = new Color32(0, 0, 0, 200);
            else
                SortButtonImages[i].color = new Color32(0, 0, 0, 50);
        }
    }
}

