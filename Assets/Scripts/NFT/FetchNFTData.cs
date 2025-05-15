using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FetchNFTData : MonoBehaviour
{
    [Header("전체 정보 조회 API")] [SerializeField]
    private string allItemsApiUrl = "http://13.125.167.56:8000/api/nft/getAllItems/";

    [Header("특정 유저 정보 조회 API")] [SerializeField]
    private string userApiUrl = "http://13.125.167.56:8000/api/nft/getUserItems/";

    [Header("특정 유저 마켓 정보 조회 API")] [SerializeField]
    private string userMarketApiUrl = "http://13.125.167.56:8000/api/nft/getListedUserItem/";

    [Header("저장할 JSON 파일 이름")] [SerializeField]
    private string fileName = "NFTData.json";

    [Header("갱신 주기(초 단위)")] [SerializeField]
    private float refreshInterval = 60.0f;

    private WaitForSeconds _refreshIntervalWait;
    private Coroutine _refreshCoroutine;

    private void Start()
    {
        _refreshIntervalWait = new WaitForSeconds(refreshInterval);
        StartCoroutine(IE_AutoRenewNFTData());
    }

    /// <summary>
    /// 중계서버에 있는 데이터 갱신
    /// </summary>
    [ContextMenu("Load NFT Data")]
    public void LoadNFTData()
    {
        if (_refreshCoroutine != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("NFTData 갱신 중복 요청 방지");
#endif
            return;
        }

        _refreshCoroutine = StartCoroutine(IE_SaveNFTData());
    }

    private IEnumerator IE_SaveNFTData()
    {
        UnityWebRequest request = UnityWebRequest.Get(allItemsApiUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error + "Error fetching NFTData");
            _refreshCoroutine = null;
            yield break;
        }

        NFTItemList nftItemList = JsonUtility.FromJson<NFTItemList>(request.downloadHandler.text);

        if (nftItemList == null && nftItemList.items.Count == 0)
        {
            _refreshCoroutine = null;
            yield break;
        }
        
        TradeItemDataList trades = new TradeItemDataList();
        foreach (var nft in nftItemList.items)
        {
            var itemData = ItemDataManager.Instance.GetItemDataById(nft.item_id);
            TradeItemData tradeItemData = new TradeItemData
            {
                TokenId = nft.token_id,
                Data = itemData,
                ItemPrice = float.Parse(nft.price_klay),
                SellerWalletAddress = nft.seller,
                LeftSeconds = nft.remaining_time,
            };
            
            trades.items.Add(tradeItemData);
        }
        
        string path = System.IO.Path.Combine(Application.dataPath, fileName);
        string tradeJson = JsonUtility.ToJson(trades);
        System.IO.File.WriteAllText(path, tradeJson);
        
        TradeManager.Instance.UpdateUI();
        
        _refreshCoroutine = null;
    }

    /// <summary>
    /// 특정 유저의 NFT 데이터 조회
    /// </summary>
    /// <param name="callback"> 데이터 로딩 완료시 호출하는 Action </param>
    public void FetchUserNFTData(Action<List<NFTItem>> callback)
    {
        StartCoroutine(IE_FetchUserItem(callback));
    }

    private IEnumerator IE_FetchUserItem(Action<List<NFTItem>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(userApiUrl);
        yield return request.SendWebRequest();
        
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error + "Error fetching User NFTData");
            yield break;
        }
        
        NFTItemList nftItemList = JsonUtility.FromJson<NFTItemList>(request.downloadHandler.text);
        if (nftItemList == null && nftItemList.items.Count == 0)
        {
            yield break;
        }
        callback?.Invoke(nftItemList.items);
    }

    private IEnumerator IE_AutoRenewNFTData()
    {
        while (true)
        {
            _refreshCoroutine = StartCoroutine(IE_SaveNFTData());

            yield return _refreshCoroutine;
            yield return _refreshIntervalWait;
        }
    }
}

[Serializable]
public class NFTItemList
{
    public bool success;
    public List<NFTItem> items;
}

[Serializable]
public class NFTItem
{
    public int token_id;
    public int item_id;
    public string seller;
    public string price_klay;
    public string metadata_uri;
    public int remaining_time;
}