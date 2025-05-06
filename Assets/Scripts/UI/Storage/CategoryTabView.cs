using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CategoryTabView : MonoBehaviour
{
    [Header("카테고리 토글")]
    [SerializeField]
    private Toggle categoryToggle;
    
    [Header("카테고리")]
    [SerializeField]
    private ItemType itemType;
    
    private void Awake()
    {
        if (categoryToggle == null)
        {
            Debug.LogError("CategoryTabView: categoryToggle이 할당되지 않았습니다.");
        }
    }

    public void Bind(Action<ItemType> callback)
    {
        categoryToggle.onValueChanged.AddListener(
            isOn =>
            {
                if (isOn)
                {
                    callback?.Invoke(itemType);
                }
            });
    }
}