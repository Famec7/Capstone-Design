using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CategoryTabView : MonoBehaviour
{
    [Header("카테고리")]
    [SerializeField]
    private ItemType itemType;
    
    private Toggle _categoryToggle;
    
    private void Awake()
    {
        if (TryGetComponent(out _categoryToggle) == false)
        {
            Debug.LogError("CategoryTabView: Toggle 컴포넌트가 할당되지 않았습니다.");
        }
    }

    public void Bind(Action<ItemType> callback)
    {
        _categoryToggle.onValueChanged.AddListener(
            isOn =>
            {
                if (isOn)
                {
                    callback?.Invoke(itemType);
                }
            });
    }
}