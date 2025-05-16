using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectionSlot : MonoBehaviour
{
    public TextMeshProUGUI CollectionNameText;
    public TextMeshProUGUI RewardText;
    public MaterialSlot[] Slots;

    public void Awake()
    {
        Slots = GetComponentsInChildren<MaterialSlot>();
    }

    public void SetValid(CatalogCollection data)
    {
        CollectionNameText.text = data.collectionName;
        RewardText.text = data.collectionDescription;
    }
}
