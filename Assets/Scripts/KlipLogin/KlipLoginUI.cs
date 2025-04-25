using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class KlipLoginUI : MonoBehaviour
{
    private string _requestKey = "";

    /*********** UI 요소 ***********/
    [SerializeField] private QRCodeImage qrCodeImage;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject loginPanel;
    
    public void OnClickLoginButton()
    {
        StartCoroutine(IE_RequestAPI());
    }

    #region Request API

    private IEnumerator IE_RequestAPI()
    {
        const string url = "http://13.125.167.56:8000/api/klip/auth/prepare/";
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
#if UNITY_EDITOR
            Debug.Log(request.downloadHandler.text);
#endif

            KlipAPIResponse response = JsonUtility.FromJson<KlipAPIResponse>(request.downloadHandler.text);

            if (!response.success)
            {
                Debug.LogError("API 요청 실패");
                yield break;
            }

            _requestKey = response.request_key;
            StartCoroutine(IE_RequestLogin());
        }
    }

    private IEnumerator IE_RequestLogin()
    {
        string url = "http://13.125.167.56:8000/api/klip/auth/request/" + _requestKey + "/";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
#if UNITY_EDITOR
            Debug.Log(request.downloadHandler.text);
#endif

            KlipLoginResponse response = JsonUtility.FromJson<KlipLoginResponse>(request.downloadHandler.text);
            qrCodeImage.GenerateQRCode(response.url);
            
            loginPanel.SetActive(false);

            statusText.text = "QR 코드를 스캔해주세요.";
            StartCoroutine(IE_RequestQRCode());
        }
    }

    private IEnumerator IE_RequestQRCode()
    {
        while (true)
        {
            string url = "http://13.125.167.56:8000/api/klip/auth/result/" + _requestKey + "/";
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
#if UNITY_EDITOR
                Debug.Log(request.downloadHandler.text);
#endif

                KlipResultResponse response = JsonUtility.FromJson<KlipResultResponse>(request.downloadHandler.text);
                
                switch (response.status)
                {
                    case "completed":
                        qrCodeImage.ClearQRCode();
                        yield break;
                    case "canceled":
                        qrCodeImage.ClearQRCode();
                        yield break;
                    case "error":
                        statusText.text = "Login Error";
                        qrCodeImage.ClearQRCode();
                        yield break;
                    case "requested":
                        break;
                    default:
                        break;
                }
            }
        }
    }

    #endregion
}

[System.Serializable]
internal class KlipAPIResponse
{
    public string request_key;
    public bool success;
}

[System.Serializable]
internal class KlipLoginResponse
{
    public string url;
}

[System.Serializable]
internal class KlipResultResponse
{
    public string request_key;
    public string status;
    public int expiration_time;
    public Result result;
}

[System.Serializable]
internal class Result
{
    public string klaytn_address;
}