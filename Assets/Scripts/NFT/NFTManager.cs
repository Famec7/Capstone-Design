using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NFTManager : Singleton<NFTManager>
{
    /************ API JSON ***********/
    #region API JSON

    [System.Serializable]
    public class BuyNFTRequest
    {
        public int tokenID;
        public string buyerAddress;
    }
    
    [System.Serializable]
    public class ConfirmBuyNFTRequest
    {
        public string requestKey;
        public ConfirmBuyNFTResult result;
    }

    [System.Serializable]
    public class ConfirmBuyNFTResult
    {
        public string buyer_address;
        public int token_id;
    }
    
    [System.Serializable]
    public class ListNFTRequest
    {
        public int tokenID;
        public float price;
        public string sellerAddress;
        public int listingDuration;
    }

    #endregion
    
    protected override void Init()
    {
        walletAddress = Resources.Load<WalletAddress>("UserData/UserWallet");
        
        if (walletAddress == null)
        {
            Debug.LogError("지갑 주소를 찾을 수 없습니다.");
            return;
        }
    }
    
    [Header("유저 지갑 주소")][SerializeField]
    private WalletAddress walletAddress;
    
    private UnityWebRequest Post(string url, string jsonData)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    /************ NFT 마켓 등록 ***********/
    public void ListNFT(int tokenID, float price, int duration = 72)
    {
#if UNITY_EDITOR
        Debug.Log($"NFT 등록 요청: tokenID={tokenID}, price={price}, duration={duration}");
#endif
        
        StartCoroutine(IE_ListNFT(tokenID, price, duration));
    }
    
    private IEnumerator IE_ListNFT(int tokenID, float price, int duration = 72)
    {
        const string url = "http://13.125.167.56:8000/api/nft/listNFT/";
        
        ListNFTRequest listRequest = new ListNFTRequest
        {
            tokenID = tokenID,
            price = price,
            sellerAddress = walletAddress.Address,
            listingDuration = duration
        };
        
        string jsonData = JsonUtility.ToJson(listRequest);
        var request = Post(url, jsonData);
        
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

	/************ NFT 구매 요청 ***********/
    public void BuyNFT(int tokenID, KlipRequest klipRequest)
    {
#if UNITY_EDITOR
        Debug.Log($"NFT 구매 요청: tokenID={tokenID}");
#endif
        StartCoroutine(IE_RequestBuyNFT(tokenID, klipRequest));
    }
    
    private IEnumerator IE_RequestBuyNFT(int tokenID, KlipRequest klipRequest)
    {
        const string url = "http://13.125.167.56:8000/api/nft/buyNFT/";
        
        BuyNFTRequest buyRequest = new BuyNFTRequest
        {
            tokenID = tokenID,
            buyerAddress = walletAddress.Address
        };
        
        string jsonData = JsonUtility.ToJson(buyRequest);
        var request = Post(url, jsonData);
        
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

	/************ NFT 구매 확정 ***********/
    private IEnumerator IE_ConfirmBuyNFT(string requestKey, int tokenID)
    {
        const string url = "http://13.125.167.56:8000/api/nft/confirmBuyNFT/";
        
        ConfirmBuyNFTRequest confirmRequest = new ConfirmBuyNFTRequest
        {
            requestKey = requestKey,
            result = new ConfirmBuyNFTResult
            {
                buyer_address = walletAddress.Address,
                token_id = tokenID
            }
        };
        
        string jsonData = JsonUtility.ToJson(confirmRequest);
        UnityWebRequest request = Post(url, jsonData);
        
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