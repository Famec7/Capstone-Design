﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    General,    
    Material,   
    Special,    
    Collection  
}

[CreateAssetMenu(fileName = "NewItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public int ItemId;
    public string ItemName;
    public ItemType ItemType;
    public int ItemValue;
    public Sprite ItemIcon;
    public GameObject ItemModel;
    public int AttackPower;
}
