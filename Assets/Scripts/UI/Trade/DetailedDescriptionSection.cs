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
    #endregion

    public void SetDetailedDescriptionSection(TradeItemData Data)
    {
        ItemImage.gameObject.SetActive(true);
        NameText.gameObject.SetActive(true);
        DescriptionText.gameObject.SetActive(true);
        CategoryText.gameObject.SetActive(true);

        NameText.text = Data.ItemName;
        // 임시
        DescriptionText.text = "Detailed Description";
        CategoryText.text = Data.ItemType.ToString();
    }
}
