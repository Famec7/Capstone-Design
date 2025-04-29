using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NFTUIController : MonoBehaviour
{
    /************ UI 요소 ***********/
    # region UI Buttons

    [Header("거래 등록 버튼")] [SerializeField] private Button listNFTButton;

    [Header("거래 구매 버튼")] [SerializeField] private Button buyNFTButton;
    
    #endregion
    
    [Header("Klip 요청 API")] [SerializeField]
    private KlipRequest klipRequest;
    [Header("유저 지갑 주소")][SerializeField]
    private WalletAddress walletAddress;

    private void Start()
    {
        if (listNFTButton == null)
        {
            Debug.LogError("listNFTButton이 inspector에서 할당되지 않았습니다.");
            return;
        }
        
        listNFTButton.onClick.AddListener(() => StartCoroutine(IE_ListNFT(0, 0, 0)));
        
        if (buyNFTButton == null)
        {
            Debug.LogError("buyNFTButton이 inspector에서 할당되지 않았습니다.");
            return;
        }
        
        buyNFTButton.onClick.AddListener(() => StartCoroutine(IE_RequestBuyNFT(0)));
    }

    /************ UI 버튼 기능 ***********/
    private IEnumerator IE_ListNFT(int tokenID, int price, int duration)
    {
        const string url = "http://13.125.167.56:8000/api/nft/listNFT/";
        
        WWWForm form = new WWWForm();
        form.AddField("tokenID", tokenID);
        form.AddField("price", price);
        form.AddField("sellerAddress", walletAddress.Address);
        form.AddField("listingDuration", duration);
        
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("거래 등록 요청 성공: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("거래 등록 요청 실패: " + request.error);
        }
    }

    private IEnumerator IE_RequestBuyNFT(int tokenID)
    {
        const string url = "http://13.125.167.56:8000/api/nft/buyNFT/";
        
        WWWForm form = new WWWForm();
        form.AddField("tokenID", tokenID);
        form.AddField("buyerAddress", walletAddress.Address);
        
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("거래 구매 요청 성공: " + request.downloadHandler.text);
            
            // Response에서 필요한 데이터를 처리
            ResponseData responseData = GetResponseData(request.downloadHandler.text);
            
            ResponseResult responseResult = responseData.result;
            string requestKey = responseResult.request_key;
            void OnRequestCompleted() => StartCoroutine(IE_ConfirmBuyNFT(requestKey, responseResult.token_id));

            // 구매 확정을 위한 Klip 요청
            klipRequest.Request(requestKey, OnRequestCompleted);
        }
        else
        {
            Debug.LogError("거래 구매 요청 실패: " + request.error);
        }
    }

    private IEnumerator IE_ConfirmBuyNFT(string requestKey, int tokenID)
    {
        const string url = "http://13.125.167.56:8000/api/nft/confirmBuyNFT/";
        
        WWWForm form = new WWWForm();
        form.AddField("requestKey", requestKey);
        form.AddField("result.token_id", tokenID);
        form.AddField("result.buyer_address", walletAddress.Address);
        
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("구매 확정 요청 성공: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("구매 확정 요청 실패: " + request.error);
        }
    }

    //************ JSON 파싱 ***********/
    private ResponseData GetResponseData(string json)
    {
        ResponseData responseData = JsonUtility.FromJson<ResponseData>(json);
        
        return responseData;
    }
    
    [Serializable]
    public class ResponseData
    {
        public bool success;
        public ResponseResult result;
    }

    [Serializable]
    public class ResponseResult
    {
        public string request_key;
        public string status;
        public int expiration_time;
        public int estimated_gas;
        public int token_id;
        public string buyer_address;
    }
}