using UnityEngine;
using UnityEngine.UI;

public class TradeUISwitcher : MonoBehaviour
{
    [Header("구매 UI")]
    [SerializeField]
    private GameObject buyUI;
    
    [Header("판매 UI")]
    [SerializeField]
    private GameObject sellUI;
    
    [Header("판매 버튼")]
    [SerializeField]
    private Button sellButton;
    
    [Header("구매 버튼")]
    [SerializeField]
    private Button buyButton;
    
    private void Awake()
    {
        if (sellButton == null)
        {
            Debug.LogError("TradeUISwitcher: sellButton이 할당되지 않았습니다.");
        }
        
        if (buyButton == null)
        {
            Debug.LogError("TradeUISwitcher: buyButton이 할당되지 않았습니다.");
        }
        
        if (buyUI == null)
        {
            Debug.LogError("TradeUISwitcher: buyUI가 할당되지 않았습니다.");
        }
        
        if (sellUI == null)
        {
            Debug.LogError("TradeUISwitcher: sellUI가 할당되지 않았습니다.");
        }
    }
    
    private void Start()
    {
        buyButton.onClick.AddListener(OnClickBuyButton);
        sellButton.onClick.AddListener(OnClickSellButton);
        
        // 초기 UI 설정
        buyUI.SetActive(true);
        sellUI.SetActive(false);
    }
    
    private void OnClickBuyButton()
    {
        buyUI.SetActive(true);
        sellUI.SetActive(false);
    }
    
    private void OnClickSellButton()
    {
        buyUI.SetActive(false);
        sellUI.SetActive(true);
    }
}