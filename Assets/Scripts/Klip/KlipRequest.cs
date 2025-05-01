using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class KlipRequest : MonoBehaviour
{
    [SerializeField]
    private QRCodeImage qrCodeImage;
    
    public Action OnRequestCompleted;

    /// <summary>
    /// Klip API 요청
    /// </summary>
    /// <param name="requestKey"> Request Key </param>
    /// <param name="onRequestCompleted"> request가 완료되었을 때 호출되는 Action </param>
    public void Request(string requestKey, Action onRequestCompleted = null)
    {
        OnRequestCompleted = onRequestCompleted;
        StartCoroutine(IE_Request(requestKey));
    }

    private IEnumerator IE_Request(string requestKey)
    {
        string url = "http://13.125.167.56:8000/api/klip/auth/request/" + requestKey + "/";
        
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
#if UNITY_EDITOR
            Debug.Log(request.downloadHandler.text);
#endif

            KlipLoginResponse response = JsonUtility.FromJson<KlipLoginResponse>(request.downloadHandler.text);
            qrCodeImage.GenerateQRCode(response.url);
            
            StartCoroutine(IE_RequestQRCode(requestKey));
        }
    }

    private IEnumerator IE_RequestQRCode(string requestKey)
    {
        while (true)
        {
            string url = "http://13.125.167.56:8000/api/klip/auth/result/" + requestKey + "/";
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
#if UNITY_EDITOR
                Debug.Log(request.downloadHandler.text);
#endif

                KlipResultResponse response = JsonUtility.FromJson<KlipResultResponse>(request.downloadHandler.text);

                if (response.status.CompareTo("completed") == 0)
                {
                    qrCodeImage.ClearQRCode();
                    OnRequestCompleted?.Invoke();
                }
            }
        }
    }
}