using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HarvestableObject : MonoBehaviour,IDamageable,ILootDrop
{
    // ScriptableObject 데이터를 참조
    public HarvestableObjectData HarvestData;

    // 런타임 내구도 관리
    protected int currentDurability;

    public Vector3 Center;
    public float Radius = 1f;
    private float _randomAngle;
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

        Center = transform.position;
        _randomAngle = Random.Range(0f, 2 * Mathf.PI);
        transform.position = new Vector3(Center.x + Radius * Mathf.Cos(_randomAngle), Center.y, Center.z + Radius * Mathf.Sin(_randomAngle));
    }

    public void Chop(int attackPower)
    {
        int damage = attackPower; // 필요하면 angularSpeed를 곱하는 등 가중치를 추가
        TakeDamage(damage);
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
            Destroy(gameObject.transform.parent.gameObject);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{gameObject.name}의 harvestData 또는 materialData가 할당되지 않았습니다.");
#endif
        }
    }

    // 원주 안에 랜덤 좌표 배치
    public void SetRandomPos()
    {
        _randomAngle += Random.Range(10f, 30f) * Mathf.Deg2Rad;
        transform.position = new Vector3(Center.x + Radius * Mathf.Cos(_randomAngle), Center.y + Random.Range(-0.1f,0.1f), Center.z + Radius * Mathf.Sin(_randomAngle));
    }   

    public bool IsMaterial()
    {
        return HarvestData.name.Contains("Wood") || HarvestData.name.Contains("Rock");
    }
}
