using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectionSlot : MonoBehaviour
{
    public TextMeshProUGUI CollectionNameText;
    public TextMeshProUGUI RewardText;

    public void SetValid(Collection data)
    {
        CollectionNameText.text = data.CollectionName;
        RewardText.text = data.RewardText;
    }
}
