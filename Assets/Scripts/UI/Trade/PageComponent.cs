using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PageComponent : MonoBehaviour
{
    public GameObject PageButtonPrefab;
    public Transform PageButtonTransform;
    List<GameObject> PageButtonLists = new List<GameObject>();

    // TradeManager의 Datas를 기준으로 총 페이지 수를 계산 (1기준)
    public int TotalPageCount = 0;

    private void Start()
    {
        TotalPageCount = TradeManager.Instance.Datas.Count / 4 + 1;
        // 초기 페이지 번호(현재 1페이지, 1기준)로 버튼 구성
        UpdatePaginationButtons(1);
    }

    /// <summary>
    /// 현재 선택된 페이지(currentPage, 1기준)를 중심으로 페이지 번호를 구성하는 함수
    /// </summary>
    /// <param name="currentPage">현재 선택된 페이지 (1 기반)</param>
    public void UpdatePaginationButtons(int currentPage)
    {
        // 기존 버튼 삭제
        foreach (Transform child in PageButtonTransform)
        {
            Destroy(child.gameObject);
        }
        PageButtonLists.Clear();

        int maxVisible = 5; // 가운데에서 보여줄 페이지 번호의 최대 개수 (단, 첫 페이지와 마지막 페이지는 별도 노출)
        List<string> pageLabels = new List<string>();
        List<int> pageNumbers = new List<int>(); // 1기준 번호 저장, -1은 클릭 불가(ellipsis)

        if (TotalPageCount <= maxVisible)
        {
            // 총 페이지 수가 maxVisible개 이하이면 모든 페이지 번호를 그대로 보여줌
            for (int i = 1; i <= TotalPageCount; i++)
            {
                pageLabels.Add(i.ToString());
                pageNumbers.Add(i);
            }
        }
        else
        {
            if (currentPage <= 3)
            {
                // 앞쪽 페이지 수가 부족할 경우: 1,2,3,4, ... , 마지막
                for (int i = 1; i <= 4; i++)
                {
                    pageLabels.Add(i.ToString());
                    pageNumbers.Add(i);
                }
                pageLabels.Add("...");
                pageNumbers.Add(-1);
                pageLabels.Add(TotalPageCount.ToString());
                pageNumbers.Add(TotalPageCount);
            }
            else if (currentPage >= TotalPageCount - 2)
            {
                // 뒤쪽 페이지 수가 부족할 경우: 첫 페이지, ..., 마지막-3, 마지막-2, 마지막-1, 마지막
                pageLabels.Add("1");
                pageNumbers.Add(1);
                pageLabels.Add("...");
                pageNumbers.Add(-1);
                for (int i = TotalPageCount - 3; i <= TotalPageCount; i++)
                {
                    pageLabels.Add(i.ToString());
                    pageNumbers.Add(i);
                }
            }
            else
            {
                // 중간에 위치한 경우: 첫 페이지, ..., (current-1), current, (current+1), ..., 마지막
                pageLabels.Add("1");
                pageNumbers.Add(1);
                pageLabels.Add("...");
                pageNumbers.Add(-1);
                pageLabels.Add((currentPage - 1).ToString());
                pageNumbers.Add(currentPage - 1);
                pageLabels.Add(currentPage.ToString());
                pageNumbers.Add(currentPage);
                pageLabels.Add((currentPage + 1).ToString());
                pageNumbers.Add(currentPage + 1);
                pageLabels.Add("...");
                pageNumbers.Add(-1);
                pageLabels.Add(TotalPageCount.ToString());
                pageNumbers.Add(TotalPageCount);
            }
        }

        // 버튼 생성 및 설정
        for (int i = 0; i < pageLabels.Count; i++)
        {
            GameObject btn = Instantiate(PageButtonPrefab, PageButtonTransform);
            TextMeshProUGUI tmp = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tmp.text = pageLabels[i];

            // 현재 선택된 페이지는 초록색, 나머지는 검정색 (숫자가 아니면 클릭 불가)
            bool isNumeric = int.TryParse(pageLabels[i], out int num);
            if (isNumeric && num == currentPage)
                tmp.color = Color.green;
            else
                tmp.color = Color.black;

            // 숫자 버튼인 경우에만 클릭 이벤트 추가 (ellipsis는 클릭 불가)
            if (isNumeric)
            {
                int capturedPage = num; // 1기준 번호
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // UpdatePage는 0기준이므로 1을 빼서 전달
                    TradeManager.Instance.UpdatePage(capturedPage - 1);
                });
            }
            PageButtonLists.Add(btn);
        }
    }
}
