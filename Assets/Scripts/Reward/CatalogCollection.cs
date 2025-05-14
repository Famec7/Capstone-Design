using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Catalog/Collection")]
public class CatalogCollection : ScriptableObject
{
    public string collectionName;
    public string collectionDescription;

    public List<ItemData> requiredItems;
    public CatalogReward reward;
}
