using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Button 사용을 위해 추가

public class SearchComponent : MonoBehaviour
{
    public GameObject ContentHolder;         // 요소들을 담고 있는 부모 오브젝트
    public GameObject[] Elements;            // 개별 요소 배열
    public GameObject SearchBar;             // SearchBar 오브젝트 (TMP_InputField 포함)

    public int TotalElements;

    private TMP_InputField searchInputField; // SearchBar의 입력 필드

    private void Start()
    {
        // SearchBar의 TMP_InputField 컴포넌트 가져오기
        searchInputField = SearchBar.GetComponent<TMP_InputField>();

        TotalElements = ContentHolder.transform.childCount;
        Elements = new GameObject[TotalElements];

        for (int i = 0; i < TotalElements; i++)
        {
            // 요소 배열에 할당
            Elements[i] = ContentHolder.transform.GetChild(i).gameObject;

            // 각 요소에 Button 컴포넌트가 있다면 onClick 이벤트에 리스너 등록
            Button elementButton = Elements[i].GetComponent<Button>();
            if (elementButton != null)
            {
                // 클로저 문제를 방지하기 위해 지역 변수에 저장
                int index = i;
                elementButton.onClick.AddListener(() => OnElementClicked(Elements[index]));
            }
            else
            {
                // Button 컴포넌트가 없을 경우, IPointerClickHandler 인터페이스를 구현한 스크립트를 추가하는 방법도 있음.
                Debug.LogWarning("Element에 Button 컴포넌트가 없습니다. 클릭 이벤트 등록에 실패했습니다.");
            }
        }
    }

    public void Search()
    {
        string searchText = searchInputField.text;
        int searchTextlength = searchText.Length;

        foreach (GameObject element in Elements)
        {
            // 자식 텍스트 (TextMeshProUGUI)가 있는지 확인 후 검색 진행
            TextMeshProUGUI elementTMP = element.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (elementTMP != null && elementTMP.text.Length >= searchTextlength)
            {
                if (searchText.ToLower() == elementTMP.text.Substring(0, searchTextlength).ToLower())
                {
                    element.SetActive(true);
                }
                else
                {
                    element.SetActive(false);
                }
            }
        }
    }

    // 클릭한 요소의 자식 텍스트를 SearchBar의 InputField 텍스트로 설정하는 함수
    public void OnElementClicked(GameObject element)
    {
        TextMeshProUGUI elementTMP = element.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (elementTMP != null)
        {
            searchInputField.text = elementTMP.text;
        }
    }
}
