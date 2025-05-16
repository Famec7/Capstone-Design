using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CollectionManager : Singleton<CollectionManager>
{

    public DetailedDescriptionSection DetailedDescriptionSection;
    public List<CatalogCollection> collections;
    public GameObject CollectionPrefab;

    [SerializeField]
    private Transform _contentParent;

    private List<CatalogCollection> _currentDisplayCollections;
    private CollectionSlot[] _collectionPreviews;
    private int _currentPage = 0;
    public int PreviewCounts = 2;

    [SerializeField]
    private PageComponent _pageComponent;

    protected override void Init()
    {
        // Preview 슬롯 생성
        _collectionPreviews = new CollectionSlot[PreviewCounts];
        for (int i = 0; i < PreviewCounts; i++)
        {
            GameObject go = Instantiate(CollectionPrefab, _contentParent);
            _collectionPreviews[i] = go.GetComponent<CollectionSlot>();
        }

        // 초기 UI 갱신
        UpdateUI();
    }

    /// <summary>
    /// 전체 컬렉션을 로드하고 페이지 수 설정
    /// </summary>
    public void UpdateUI()
    {
        _currentDisplayCollections = new List<CatalogCollection>(collections);
        int totalPages = _currentDisplayCollections.Count / PreviewCounts
            + (_currentDisplayCollections.Count % PreviewCounts != 0 ? 1 : 0);
        _pageComponent.TotalPageCount = totalPages;
        UpdatePage(0);
    }

    /// <summary>
    /// 특정 페이지를 화면에 표시
    /// </summary>
    public void UpdatePage(int pageIndex)
    {
        int startIndex = pageIndex * PreviewCounts;
        for (int i = 0; i < _collectionPreviews.Length; i++)
        {
            int dataIndex = startIndex + i;
            if (dataIndex < _currentDisplayCollections.Count)
            {
                var slot = _collectionPreviews[i];
                slot.gameObject.SetActive(true);
                slot.SetValid(_currentDisplayCollections[dataIndex]);

                // 상세 설명 버튼 바인딩
                int idx = i; // 클로저 보호
                foreach (var matSlot in slot.Slots)
                {
                    var required = _currentDisplayCollections[dataIndex].requiredItems[idx];
                    matSlot.GetComponent<Button>()
                        .onClick.RemoveAllListeners();
                    matSlot.GetComponent<Button>()
                        .onClick.AddListener(() =>
                            DetailedDescriptionSection
                                .SetDetailedDescriptionSection(required)
                        );
                    idx++;
                }
            }
            else
            {
                _collectionPreviews[i].gameObject.SetActive(false);
            }
        }
        _pageComponent.UpdatePaginationButtons(pageIndex + 1);
        _currentPage = pageIndex;
    }

    /// <summary>
    /// 다음 페이지로 이동
    /// </summary>
    public void NextPage()
    {
        if ((_currentPage + 1) * PreviewCounts < _currentDisplayCollections.Count)
        {
            UpdatePage(_currentPage + 1);
        }
        else
        {
            Debug.Log("마지막 페이지입니다.");
        }
    }

    /// <summary>
    /// 이전 페이지로 이동
    /// </summary>
    public void PrevPage()
    {
        if (_currentPage > 0)
        {
            UpdatePage(_currentPage - 1);
        }
        else
        {
            Debug.Log("첫 페이지입니다.");
        }
    }
}
