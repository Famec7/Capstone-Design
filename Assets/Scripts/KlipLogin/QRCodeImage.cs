using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QRCodeImage : MonoBehaviour
{
    private RawImage _qrCodeImage;

    private void Awake()
    {
        if (TryGetComponent(out RawImage rawImage))
        {
            _qrCodeImage = rawImage;
            _qrCodeImage.raycastTarget = false;
        }
        else
        {
            Debug.LogError("RawImage 컴포넌트를 찾을 수 없습니다.");
        }
    }

    public void GenerateQRCode(string url)
    {
        var qrWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = 256,
                Width = 256
            }
        };

        Color32[] pixels = qrWriter.Write(url);
        Texture2D texture = new Texture2D(256, 256);
        texture.SetPixels32(pixels);
        texture.Apply();
        
        _qrCodeImage.texture = texture;
        _qrCodeImage.enabled = true;
    }
    
    public void ClearQRCode()
    {
        _qrCodeImage.texture = null;
        _qrCodeImage.enabled = false;
    }
}