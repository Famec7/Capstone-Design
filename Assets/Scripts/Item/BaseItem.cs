using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseItem : MonoBehaviour,ISellable
{
    public ItemData Data;
    public void SellToShop()
    {
        ;
    }

    // 아이템 선택 시 호출되는 함수
    public virtual void OnSelected()
    {
        Debug.Log($"{Data.ItemName} 선택됨");
    }

    // 아이템 선택 해제 시 호출되는 함수
    public virtual void OnDeselected()
    {
        Debug.Log($"{Data.ItemName} 선택 해제됨");
    }
}
