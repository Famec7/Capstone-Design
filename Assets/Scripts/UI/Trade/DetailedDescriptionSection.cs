using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailedDescriptionSection : MonoBehaviour
{
    #region DetailedDescriptionSection Elements
    public Image ItemImage;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI CategoryText;
    public Button BuyButton;
    
    [SerializeField]
    private KlipRequest KlipRequest;
    
    #endregion

    public void SetDetailedDescriptionSection(TradeItemData Data)
    {
        ItemImage.gameObject.SetActive(true);
        NameText.gameObject.SetActive(true);
        DescriptionText.gameObject.SetActive(true);
        CategoryText.gameObject.SetActive(true);
        BuyButton.gameObject.SetActive(true);

        NameText.text = Data.Data.ItemName;
        DescriptionText.text = Data.Data.ItemDescription;
        CategoryText.text = Data.Data.ItemType.ToString();
        ItemImage.sprite = Data.Data.ItemIcon;
        
        BuyButton.onClick.RemoveAllListeners();
        
        KlipRequest.OnRequestCompleted = () =>
        {
            TradeManager.Instance.Load();
            TradeManager.Instance.DetailedDescriptionSection.gameObject.SetActive(false);
        };
        
        BuyButton.onClick.AddListener(() =>
        {
            NFTManager.Instance.BuyNFT(Data.TokenId, KlipRequest);
        });
    }

    public void SetDetailedDescriptionSection(ItemData Data)
    {
        ItemImage.gameObject.SetActive(true);
        NameText.gameObject.SetActive(true);
        DescriptionText.gameObject.SetActive(true);
        CategoryText.gameObject.SetActive(true);

        NameText.text = Data.ItemName;
        DescriptionText.text = Data.ItemDescription;
        CategoryText.text = Data.ItemType.ToString();
        ItemImage.sprite = Data.ItemIcon;
    }
}
