using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    #region InventorySlot Components
    #endregion

    public ItemData Data;
    public Image BorderImage;
    public bool IsEmpty => Data == null;

    /// <summary>
    /// 슬롯에 아이템 지정해주는 함수
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(BaseItem item)
    {
        Data = item.Data;
        item.gameObject.transform.SetParent(transform);
        SetBorderColor(item);
        SetSlotColor(item);
    }

    public void SetITem(ItemData data)
    {
        SetItem(ItemFactory.Instance.CreateItem(data.ItemName));
    }

    /// <summary>
    /// 슬롯 배경색 지정해주는 함수
    /// </summary>
    /// <param name="item"></param>
    public void SetBorderColor(BaseItem item)
    {
        BorderImage.color = Color.black;
        InventoryManager.Instance.Slots[InventoryManager.Instance.SelectedSlotIndex].BorderImage.color = Color.yellow;
    }

    public void SetSlotColor(BaseItem item)
    {
        Image slotImage = GetComponent<Image>();
        if (item is SpecialItem special)
        {
            switch (special.Type)
            {
                case SpecialItemType.HealthRecovery:
                    slotImage.color = Color.red;
                    break;
                case SpecialItemType.HungerRecovery:
                    slotImage.color = Color.yellow;
                    break;
                case SpecialItemType.ThirstRecovery:
                    slotImage.color = Color.blue;
                    break;
                default:
                    slotImage.color = Color.green;
                    break;
            }
        }
        else
        {
            slotImage.color = Color.green;
        }

        InventoryManager.Instance.Slots[InventoryManager.Instance.SelectedSlotIndex].BorderImage.color = Color.yellow;
    }

    /// <summary>
    /// 아이템 Transform 설정해주는 함수
    /// </summary>
    /// <param name="item"></param>
    public void SetItemTransform(BaseItem item)
    {
        Rigidbody rb = item.gameObject.GetComponent<Rigidbody>();

        rb.isKinematic = true;                       
        rb.useGravity = false;                       
        rb.constraints = RigidbodyConstraints.FreezeAll; 

        item.transform.localScale = Vector3.one;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity; 
    }
}
