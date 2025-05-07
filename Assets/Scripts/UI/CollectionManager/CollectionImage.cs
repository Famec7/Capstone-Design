using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionImage : MonoBehaviour
{

    public TradeItemData Data;
    public void DisplayDescription()
    {
        CollectionManager.Instance.DetailedDescriptionSection.SetDetailedDescriptionSection(Data);
    }
}
