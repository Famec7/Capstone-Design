using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableObject : MonoBehaviour,IDamageable,ILootDrop
{
    // ScriptableObject 데이터를 참조
    public HarvestableObjectData HarvestData;

    // 런타임 내구도 관리
    protected int currentDurability;

    protected virtual void Awake()
    {
        // ScriptableObject에 설정된 내구도로 초기화
        if (HarvestData != null)
        {
            currentDurability = HarvestData.Durability;
        }
        else
        {
            Debug.LogError($"{gameObject.name}의 harvestData가 할당되지 않았습니다!");
        }
    }

    // 피해를 받을 때 호출되는 메서드
    public virtual void TakeDamage(int damage)
    {
        currentDurability -= damage;
        Debug.Log($"{gameObject.name}에 {damage} 피해. 남은 내구도: {currentDurability}");

        if (currentDurability <= 0)
        {
            DropLoot();
            Destroy(gameObject);
        }
    }

    // 내구도가 0 이하가 되었을 때 재료 드롭 처리
    public virtual void DropLoot()
    {
        if (HarvestData != null && HarvestData.MaterialData != null)
        {
            BaseItem item = ItemFactory.Instance.CreateItem(HarvestData.MaterialData.ItemName);
            item.gameObject.transform.position = transform.position;
            #if UNITY_EDITOR
            Debug.Log($"{gameObject.name} 채집 완료! {HarvestData.YieldAmount} x {HarvestData.MaterialData.ItemName} 지급");
            #endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{gameObject.name}의 harvestData 또는 materialData가 할당되지 않았습니다.");
#endif
        }
    }
}
