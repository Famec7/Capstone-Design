using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseItem : MonoBehaviour,ISellable
{
    public ItemData Data;
    public bool IsInInventory = false;
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

    public void Update()
    {
        if(IsInInventory)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    public void InitTransform()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;

        // 월드 스케일을 1,1,1로 만들기 위해 부모 스케일에 반비례하게 조정
        if (transform.parent != null)
        {
            Vector3 parentScale = transform.parent.lossyScale;
            transform.localScale = new Vector3(
                1f / parentScale.x,
                1f / parentScale.y,
                1f / parentScale.z
            );
        }
        else
        {
            transform.localScale = Vector3.one;
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
