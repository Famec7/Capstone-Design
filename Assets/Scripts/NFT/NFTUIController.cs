using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NFTUIController : MonoBehaviour
{
    /************ UI 요소 ***********/

    # region UI Buttons

    [Header("거래 등록 버튼")] [SerializeField] private Button listNFTButton;

    [Header("거래 구매 버튼")] [SerializeField] private Button buyNFTButton;

    [Header("거래 취소 버튼")] [SerializeField] private Button cancelNFTButton;

    #endregion

    private void Start()
    {
        if (listNFTButton == null)
        {
            Debug.LogError("listNFTButton이 inspector에서 할당되지 않았습니다.");
            return;
        }
        
        listNFTButton.onClick.AddListener(() => StartCoroutine(IE_ListNFT()));
        
        if (buyNFTButton == null)
        {
            Debug.LogError("buyNFTButton이 inspector에서 할당되지 않았습니다.");
            return;
        }
        
        buyNFTButton.onClick.AddListener(() => StartCoroutine(IE_BuyNFT()));
        
        if (cancelNFTButton == null)
        {
            Debug.LogError("cancelNFTButton이 inspector에서 할당되지 않았습니다.");
            return;
        }
        
        cancelNFTButton.onClick.AddListener(() => StartCoroutine(IE_CancelNFT()));
    }

    /************ UI 버튼 기능 ***********/
    private IEnumerator IE_ListNFT()
    {
        yield break;
    }

    private IEnumerator IE_BuyNFT()
    {
        yield break;
    }

    private IEnumerator IE_CancelNFT()
    {
        yield break;
    }
}