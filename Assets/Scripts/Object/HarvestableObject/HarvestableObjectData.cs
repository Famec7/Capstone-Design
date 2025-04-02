using UnityEngine;

[CreateAssetMenu(fileName = "NewHarvestableObjectData", menuName = "HarvestableObject/Data")]
public class HarvestableObjectData : ScriptableObject
{
    // 드롭할 재료의 데이터 (예: 나무, 돌 등)
    public ItemData MaterialData;
    // 드롭될 재료의 수량
    public int YieldAmount = 1;
    // 오브젝트의 내구도 (몇 번의 피해를 견딜 수 있는지)
    public int Durability = 3;
}
