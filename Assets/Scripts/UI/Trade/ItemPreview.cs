using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPreview : MonoBehaviour
{
    #region ItemPreview Elements
    public Image ItemImage;
    public Image BorderImage;
    public TextMeshProUGUI ExpiredText;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI PriceText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI DescriptionText;
    public TradeItemData Data;
    #endregion

    public void SetExpired()
    {
        BorderImage.gameObject.SetActive(true);
        ExpiredText.gameObject.SetActive(true);
    }

    public void SetValid(TradeItemData data)
    {
        Data = data;
        BorderImage.gameObject.SetActive(false);
        ExpiredText.gameObject.SetActive(false);
        NameText.text = Data.ItemName;
        PriceText.text = Data.ItemPrice.ToString();

        TimeSpan timeDiff = DateTime.Now - Data.ListedTime;
        TimeText.text = timeDiff.ToString(@"hh\:mm\:ss");
      
    }
}
