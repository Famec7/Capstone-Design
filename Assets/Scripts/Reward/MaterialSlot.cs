// MaterialSlot.cs
using UnityEngine;
using UnityEngine.UI;

public class MaterialSlot : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        UpdateVisual();
    }


    public void UpdateVisual()
    {
       //TODO 배낭에 있을 때 색 변화
    }
}
