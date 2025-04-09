using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class SearchComponent : MonoBehaviour
{
    public GameObject ContentHolder;         
    public GameObject[] Elements;            
    public GameObject SearchBar;          

    public int TotalElements;

    private TMP_InputField searchInputField; 

    private void Start()
    {
        searchInputField = SearchBar.GetComponent<TMP_InputField>();

        TotalElements = ContentHolder.transform.childCount;
        Elements = new GameObject[TotalElements];

        for (int i = 0; i < TotalElements; i++)
        {
            Elements[i] = ContentHolder.transform.GetChild(i).gameObject;

            Button elementButton = Elements[i].GetComponent<Button>();
            if (elementButton != null)
            {
                int index = i;
                elementButton.onClick.AddListener(() => OnElementClicked(Elements[index]));
            }
        }
    }

    public void Search()
    {
        string searchText = searchInputField.text;
        int searchTextlength = searchText.Length;

        foreach (GameObject element in Elements)
        {
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

    public void OnElementClicked(GameObject selectedElement)
    {
        TextMeshProUGUI elementTMP = selectedElement.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (elementTMP != null)
        {
            searchInputField.text = elementTMP.text;
        }

        foreach (GameObject element in Elements)
        {
            if (element == selectedElement)
                continue;
            element.SetActive(false);
        }
    }
}
