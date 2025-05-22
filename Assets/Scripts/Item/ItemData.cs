using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    General,    
    Special,    
    Collection,
    Material,
    None
}


[CreateAssetMenu(fileName = "NewItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public int ItemId;
    public string ItemName;
    public string ItemDescription;
    public ItemType ItemType;
    public int ItemValue;
    public Sprite ItemIcon;
    public string URL;
    public GameObject ItemModel;
    public int AttackPower;
    public bool IsNFT = false;
}
